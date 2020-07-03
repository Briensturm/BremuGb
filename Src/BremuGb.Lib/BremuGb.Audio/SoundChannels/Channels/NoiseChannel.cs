namespace BremuGb.Audio.SoundChannels
{
    internal class NoiseChannel : SoundChannelBase
    {
        private int _frequencyTimer;

        private int _envelopeTimer;
        private int _initialVolume;
        private int _currentVolume;
        private int _envelopePeriod;
        private bool _envelopeIncrease;
        private bool _envelopeEnable;

        private int _shiftClockFreq;
        private int _dividingRatio;
        private bool _widthMode;        
        private ushort _lfsr;

        private int[] _divisorLookup = new int[8] { 1, 2, 4, 6, 8, 10, 12, 14 };        

        protected override int ChannelMaxLength => 64;

        internal byte LengthLoad
        {
            get => 0xFF;
            set => _lengthCounter = ChannelMaxLength - (value & 0x3F);
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

        internal byte Polynomial
        {
            get => (byte)(_shiftClockFreq << 4 |
                         (_widthMode ? 1 : 0) << 3 |
                          _dividingRatio);

            set
            {
                _shiftClockFreq = value >> 4;
                _widthMode = (value & 0x8) == 0x8;
                _dividingRatio = value & 0x7;
            }
        }

        internal byte LengthEnable 
        {
            get => (byte)(_compareLength ? 0xFF : 0xBF);

            set
            {
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
            if (_frequencyTimer == 0)
            {
                //reload timer
                ReloadFrequencyTimer();

                //lfsr is not clocked if shift frequency is 14 or 15 (obscure behavior)
                if (_shiftClockFreq != 14 && _shiftClockFreq != 15)
                    AdvanceLfsr();
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
            if (!IsEnabled())
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
            _isEnabled = false;
            _envelopeEnable = false;
            _envelopeTimer = 0;
            _frequencyTimer = 0;

            Envelope = 0;
            Polynomial = 0;
            LengthEnable = 0;

            base.Disable();
        }

        private void AdvanceLfsr()
        {
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

        private void ReloadFrequencyTimer()
        {
            _frequencyTimer = _divisorLookup[_dividingRatio] << (_shiftClockFreq + 1);
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

            _lfsr = 0x7FFF;
            _currentVolume = _initialVolume;

            ReloadFrequencyTimer();
            ReloadLengthCounter();
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
