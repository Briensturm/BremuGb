using System;

namespace BremuGb.Audio.SoundChannels
{
    internal class SquareWaveChannel : SoundChannelBase
    {
        private int _timer = 0;
        private int _frequency = 0;

        private int _envelopeTimer = 0;

        private int _initialVolume = 0;
        private int _currentVolume = 0;

        private int _lengthCounter = 0;

        private bool _compareLength = false;
        private bool _envelopeIncrease = false;
        private bool _isEnabled;

        private int _envelopePeriod = 0;

        private int _dutyPatternSelect = 0;
        private int _dutyIndex = 0;

        private byte[] _dutyPattern0;
        private byte[] _dutyPattern1;
        private byte[] _dutyPattern2;
        private byte[] _dutyPattern3;

        internal SquareWaveChannel()
        {
            _dutyPattern3 = new byte[8] { 0, 1, 1, 1, 1, 1, 1, 0 };
            _dutyPattern2 = new byte[8] { 1, 0, 0, 0, 0, 1, 1, 1 };
            _dutyPattern1 = new byte[8] { 1, 0, 0, 0, 0, 0, 0, 1 };
            _dutyPattern0 = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 1 };
        }

        public byte DutyLength
        {
            get
            {
                return (byte)((_dutyPatternSelect << 6) | 0x3F);
            }
            internal set
            {
                _dutyPatternSelect = (value & 0xC0) >> 6;
                _lengthCounter = 64 - (value & 0x3F);
            }
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
                    _isEnabled = false;
            }
        }
        
        public byte FrequencyHi
        {
            get
            {
                return (byte)(_compareLength ? 0xFF : 0xBF);
            }

            internal set
            {
                _frequency = (_frequency & 0xFF) | ((value & 0x7) << 8);

                _compareLength = (value & 0x40) == 0x40;

                if ((value & 0x80) == 0x80)
                    Trigger();
            }
        }

        public byte FrequencyLo
        {
            get
            {
                return 0xFF;
            }
            internal set
            {
                _frequency = (_frequency & 0x700) | value;
            }
        }

        public override void AdvanceMachineCycle()
        {
            if (_timer == 0)
                return;

            _timer--;

            if(_timer == 0)
            {
                _dutyIndex++;
                if (_dutyIndex == 8)
                    _dutyIndex = 0;

                //reload timer
                _timer = 2048 - _frequency;
            }
        }

        public override void ClockEnvelope()
        {
            if (_envelopeTimer == 0 || _envelopePeriod == 0)
                return;
            _envelopeTimer--;

            if(_envelopeTimer == 0)
            {
                if (_envelopeIncrease && _currentVolume < 0xF)
                    _currentVolume++;
                else if (!_envelopeIncrease && _currentVolume > 0)
                    _currentVolume--;

                _envelopeTimer = _envelopePeriod;
            }            
        }

        public override void ClockLength()
        {
            if (_compareLength && _lengthCounter > 0)
            {
                _lengthCounter--;

                if (_lengthCounter == 0)
                    _isEnabled = false;
            }            
        }

        public override byte GetSample()
        {            
            if(!IsEnabled())            
                return 0;

            int output;
            switch(_dutyPatternSelect)
            {
                case 0:
                    output = _dutyPattern0[_dutyIndex];
                    break;
                case 1:
                    output = _dutyPattern1[_dutyIndex];
                    break;
                case 2:
                    output = _dutyPattern2[_dutyIndex];
                    break;
                case 3:
                    output = _dutyPattern3[_dutyIndex];
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return (byte)(output * _currentVolume *17);
        }

        public override bool IsEnabled()
        {
            return _isEnabled;
        }

        public override void Disable()
        {
            _lengthCounter = 0;
            _envelopeTimer = 0;
            _timer = 0;
            _isEnabled = false;

            Envelope = 0;
            DutyLength = 0;
            FrequencyHi = 0;
            FrequencyLo = 0;
        }

        private void Trigger()
        {
            _isEnabled = true;

            //obscure behavior: update timer except two lower bits
            _timer = (2048 - _frequency) & 0xFFFC;

            if (_lengthCounter == 0)
                _lengthCounter = 64;

            _currentVolume = _initialVolume;

            _envelopeTimer = _envelopePeriod;

            //if dac is off, disable channel
            if (IsDacDisabled())
                _isEnabled = false;
        }

        private bool IsDacDisabled()
        {
            return _initialVolume == 0 && !_envelopeIncrease;
        }
    }
}
