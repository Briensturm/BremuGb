namespace BremuGb.Audio.SoundChannels
{
    internal class SquareWaveSweepChannel : SquareWaveChannel
    {
        private const int OverflowValue = 2047;

        private uint _frequencyShadowRegister;

        private bool _sweepEnabled;
        private bool _sweepNegate;
        private int _sweepTimer;
        private int _sweepPeriod;
        private int _sweepShift;        

        private bool _prepareNegateDisable;

        internal byte Sweep
        {
            get => (byte)(_sweepPeriod << 4 |
                         (_sweepNegate ? 1 : 0) << 3 |
                          _sweepShift |
                           0x80);

            set
            {
                //obscure sweep behavior
                if (_sweepNegate && (value & 0x08) == 0 && _prepareNegateDisable)
                    _isEnabled = false;

                _sweepPeriod = (value & 0x70) >> 4;
                _sweepNegate = (value & 0x08) == 0x08;
                _sweepShift = value & 0x07;
            }
        }

        internal override byte LengthEnable 
        { 
            get => base.LengthEnable;

            set
            {
                base.LengthEnable = value;

                //handle sweep specific behavior in case of trigger
                if ((value & 0x80) == 0x80)
                {
                    _prepareNegateDisable = false;
                    _frequencyShadowRegister = (uint)_frequency;

                    ReloadSweepTimer();

                    _sweepEnabled = _sweepPeriod != 0 || _sweepShift != 0;

                    if(_sweepShift != 0)
                    {                        
                        if (CalculateNewFrequency() > OverflowValue)
                            _isEnabled = false;
                    }
                }
            }
        }

        internal SquareWaveSweepChannel()
        {
            LengthLoad = 0xBF;
            Envelope = 0xF3;

            _isEnabled = true;
        }

        public override void ClockSweep()
        {
            if (_sweepTimer > 0)
                _sweepTimer--;
            if (_sweepTimer > 0)
                return;

            ReloadSweepTimer();

            if (_sweepEnabled && _sweepPeriod > 0)
            {
                var newFrequency = CalculateNewFrequency();

                if (newFrequency > OverflowValue)
                    _isEnabled = false;

                else if(newFrequency <= OverflowValue && _sweepShift != 0)
                {
                    _frequencyShadowRegister = newFrequency;
                    _frequency = (int)newFrequency;                    

                    if (CalculateNewFrequency() > OverflowValue)
                        _isEnabled = false;
                }
            }
        }

        private void ReloadSweepTimer()
        {
            if (_sweepPeriod == 0)
                _sweepTimer = 8;
            else
                _sweepTimer = _sweepPeriod;
        }

        public override void Disable()
        {
            Sweep = 0;

            base.Disable();
        }

        private uint CalculateNewFrequency()
        {
            if (_sweepNegate)
                _prepareNegateDisable = true;

            return (uint)(_frequencyShadowRegister + (_frequencyShadowRegister >> _sweepShift) * (_sweepNegate ? -1 : 1));
        }
    }
}
