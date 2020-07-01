using System.Collections.Generic;

namespace BremuGb.Audio.SoundChannels
{
    internal class SquareWaveChannel : SoundChannelBase
    {
        private int _frequencyTimer;
        protected int _frequency;

        private int _envelopeTimer;
        private int _initialVolume;
        private int _currentVolume;
        private int _envelopePeriod;
        private bool _envelopeIncrease;
        private bool _envelopeEnable;

        private int _dutyPatternSelect;
        private int _dutyIndex;        

        private List<byte[]> _dutyPattern = new List<byte[]> { new byte[8] { 0, 1, 1, 1, 1, 1, 1, 0 },
                                                               new byte[8] { 0, 1, 1, 1, 1, 1, 1, 0 },
                                                               new byte[8] { 1, 0, 0, 0, 0, 1, 1, 1 },
                                                               new byte[8] { 1, 0, 0, 0, 0, 0, 0, 1 },
                                                               new byte[8] { 0, 0, 0, 0, 0, 0, 0, 1 }};

        protected override int ChannelMaxLength => 64;

        internal byte LengthLoad
        {
            get => (byte)((_dutyPatternSelect << 6) | 0x3F);
            
            set
            {
                _dutyPatternSelect = (value & 0xC0) >> 6;
                _lengthCounter = ChannelMaxLength - (value & 0x3F);
            }
        }
        
        internal byte Envelope
        {
            get => (byte)(_initialVolume << 4 |
                         (_envelopeIncrease ? 1 : 0) << 3 |
                          _envelopePeriod);
            
            set
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

        internal byte FrequencyLsb
        {
            get => 0xFF;
            set => _frequency = (_frequency & 0x700) | value;
        }

        internal virtual byte LengthEnable
        {
            get => (byte)(_compareLength ? 0xFF : 0xBF);

            set
            {
                _frequency = (_frequency & 0xFF) | ((value & 0x7) << 8);

                HandleLengthClocking(value);

                if ((value & 0x80) == 0x80)
                    Trigger();
            }
        }        

        public override void AdvanceMachineCycle()
        {
            if (_frequencyTimer == 0)
                return;

            _frequencyTimer--;

            if(_frequencyTimer == 0)
            {
                _dutyIndex++;
                if (_dutyIndex == 8)
                    _dutyIndex = 0;

                //reload timer
                _frequencyTimer = 2048 - _frequency;
            }
        }

        public override void ClockEnvelope()
        {
            if (_envelopeTimer > 0)
                _envelopeTimer--;
            if (_envelopeTimer > 0)
                return;

            ReloadEnvelopeTimer();

            if (!_envelopeEnable || _envelopePeriod == 0)
                return;

            if (_envelopeIncrease && _currentVolume < 0xF)
                _currentVolume++;
            else if (!_envelopeIncrease && _currentVolume > 0)
                _currentVolume--;
            else if ((_envelopeIncrease && _currentVolume == 0xF) || (!_envelopeIncrease && _currentVolume == 0))
                _envelopeEnable = false;
        }

        public override byte GetSample()
        {            
            if(!IsEnabled())            
                return 0;

            return (byte)(_dutyPattern[_dutyPatternSelect][_dutyIndex] * _currentVolume *17);
        }

        public override bool IsEnabled()
        {
            return _isEnabled;
        }

        public override void Disable()
        {
            _isEnabled = false;
            _envelopeEnable = false;
            _envelopeTimer = 0;
            _frequencyTimer = 0;
            _dutyPatternSelect = 0;            

            Envelope = 0;            
            FrequencyLsb = 0;
            LengthEnable = 0;            

            base.Disable();
        }

        private void ReloadEnvelopeTimer()
        {
            if (_envelopePeriod == 0)
                _envelopeTimer = 8;
            else
                _envelopeTimer = _envelopePeriod;
        }

        private void Trigger()
        {
            _isEnabled = true;
            _envelopeEnable = true;

            //obscure behavior: update timer except two lower bits
            _frequencyTimer = (2048 - _frequency) & 0xFFFC;

            ReloadLengthCounter();
            ReloadEnvelopeTimer();

            _currentVolume = _initialVolume;

            //if dac is off, disable channel
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
