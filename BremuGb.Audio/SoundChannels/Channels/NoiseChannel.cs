using System;

namespace BremuGb.Audio.SoundChannels
{
    internal class NoiseChannel : SoundChannelBase
    {
        private int _timer = 0;
        private int _envelopeTimer;
        private int _initialVolume;
        private int _currentVolume;
        private bool _envelopeIncrease;
        private int _envelopePeriod;
        private int _shiftClockFreq;
        private bool _widthMode;
        private int _dividingRatio;
        private ushort _lfsr;
        private int[] _divisorLookup;

        private bool _envelopeEnable;

        protected override int ChannelMaxLength => 64;

        public NoiseChannel()
        {
            _divisorLookup = new int[8] { 1, 2, 4, 6, 8, 10, 12, 14 };
        }

        public byte Envelope
        {
            get
            {
                return (byte)(_initialVolume << 4 |
                                (_envelopeIncrease ? 1 : 0) << 3 |
                                _envelopePeriod);
            }
            internal set
            {
                _initialVolume = (value & 0xF0) >> 4;
                _envelopeIncrease = (value & 0x8) == 0x8;
                _envelopePeriod = value & 0x7;

                if (IsDacDisabled())
                {
                    _isEnabled = false;
                    _envelopeEnable = false;
                }
            }
        }

        public byte InitConsecutive 
        { 
            get
            {
                if (_compareLength)
                    return 0xFF;
                else
                    return 0xBF;
            }

            internal set
            {
                var oldCompareLength = _compareLength;
                _compareLength = (value & 0x40) == 0x40;

                //obscure behavior
                if(!oldCompareLength && _compareLength && !_prepareClockLength && _lengthCounter > 0)
                {
                    _lengthCounter--;

                    //if decremented to zero and no trigger, disable channel
                    if (_lengthCounter == 0 && ((value & 0x80) == 0))
                        _isEnabled = false;
                }

                if((value & 0x80) == 0x80)
                    Trigger();
            }
        }

        public byte PolynomialCounter 
        { 
            get
            {
                return (byte)(_shiftClockFreq << 4 |
                             (_widthMode ? 1 : 0) << 3 |
                              _dividingRatio);
            }

            internal set
            {
                _shiftClockFreq = value >> 4;
                _widthMode = (value & 0x8) == 0x8;
                _dividingRatio = value & 0x7;
            }
        }

        public byte SoundLength 
        { 
            get
            {
                return 0xFF;
            }

            internal set
            {
                _lengthCounter = ChannelMaxLength - (value & 0x3F);
            }
        }

        public override void AdvanceMachineCycle()
        {
            if (_timer == 0)
                return;

            _timer--;

            if (_timer == 0)
            {
                //reload timer
                ReloadTimer();

                //obscure behavior where lfsr receives no clock
                if (_shiftClockFreq == 14 || _shiftClockFreq == 15)
                    return;

                //advance LFSR
                var newBit = (_lfsr & 0x1) ^ ((_lfsr & 0x2) >> 1);
                _lfsr = (ushort)(_lfsr >> 1);

                if(newBit == 1)
                {
                    _lfsr = (ushort)(_lfsr | 0x4000);
                    if (_widthMode)
                        _lfsr = (ushort)(_lfsr | 0x0040);
                }
                else
                {
                    _lfsr = (ushort)(_lfsr & 0x3FFF);
                    if (_widthMode)
                        _lfsr = (ushort)(_lfsr & 0x7FBF);
                }                
            }
        }

        public override void ClockEnvelope()
        {
            if (_envelopeTimer > 0)
                _envelopeTimer--;

            if(_envelopeTimer == 0)
            {
                ReloadEnvelopeTimer();

                if (!_envelopeEnable || _envelopePeriod == 0)
                    return;

                if (_envelopeIncrease && _currentVolume < 0xF)
                    _currentVolume++;
                else if (!_envelopeIncrease && _currentVolume > 0)
                    _currentVolume--;
                else if ((_envelopeIncrease && _currentVolume == 0xF) || (!_envelopeIncrease && _currentVolume == 0))
                    _envelopeEnable = false;

                //TODO: When else to disable _envelopeEnable?
            }            
        }

        public override byte GetSample()
        {
            if(!IsEnabled())
                return 0;

            //get output from LFSR
            return (byte)(((_lfsr & 0x1) ^ 1) * _currentVolume * 17);
        }

        public override bool IsEnabled()
        {
            return _isEnabled;
        }

        public override void Disable()
        {
            _envelopeTimer = 0;
            _timer = 0;
            _isEnabled = false;
            _envelopeEnable = false;

            Envelope = 0;
            InitConsecutive = 0;
            PolynomialCounter = 0;

            base.Disable();
        }

        private void ReloadTimer()
        {
            _timer = _divisorLookup[_dividingRatio] << (_shiftClockFreq + 1);
        }

        private void ReloadEnvelopeTimer()
        {
            //TODO: Fix envelope also for other channels
            if (_envelopePeriod == 0)
                _envelopeTimer = 8;
            else
                _envelopeTimer = _envelopePeriod;
        }

        private void Trigger()
        {
            _isEnabled = true;
            _envelopeEnable = true;

            ReloadTimer();

            _lfsr = 0x7FFF;

            ReloadLengthCounter();

            _currentVolume = _initialVolume;

            ReloadEnvelopeTimer();

            if (IsDacDisabled())
            {
                _isEnabled = false;
                _envelopeEnable = false;
            }
        }

        private bool IsDacDisabled()
        {
            return _initialVolume == 0 && !_envelopeIncrease;
        }
    }
}
