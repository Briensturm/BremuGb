namespace BremuGb.Audio.SoundChannels
{
    internal class WaveChannel : SoundChannelBase
    {
        private int _timer;
        private byte _waveBuffer;

        private bool _dacOn;

        private int _frequency;

        private bool _useLowNibble;

        internal bool AccessingWaveRam { get; set; }

        public byte OnOff 
        { 
            get
            {
                return (byte)(_dacOn ? 0xFF : 0x7F);
            }

            internal set
            {
                _dacOn = (value & 0x80) == 0x80;

                if (IsDacDisabled())
                    _isEnabled = false;
            }
        }
        public byte OutputLevel 
        { 
            get
            {
                int shiftCode = 0;

                switch(_volumeShift)
                {
                    case 0:
                        shiftCode = 1;
                        break;
                    case 1:
                        shiftCode = 2;
                        break;
                    case 2:
                        shiftCode = 3;
                        break;
                    case 4:
                        shiftCode = 0;
                        break;
                }

                return (byte)((shiftCode << 5) | 0x9F);
            }

            internal set
            {
                var shiftCode = (value & 0x60) >> 5; 

                switch(shiftCode)
                {
                    case 0:
                        _volumeShift = 4;
                        break;
                    case 1:
                        _volumeShift = 0;
                        break;
                    case 2:
                        _volumeShift = 1;
                        break;
                    case 3:
                        _volumeShift = 2;
                        break;
                }
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

                var oldCompareLength = _compareLength;
                _compareLength = (value & 0x40) == 0x40;

                //obscure behavior
                if (!oldCompareLength && _compareLength && !_prepareClockLength && _lengthCounter > 0)
                {
                    _lengthCounter--;

                    //if decremented to zero and no trigger, disable channel
                    if (_lengthCounter == 0 && ((value & 0x80) == 0))
                        _isEnabled = false;
                }

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
        public byte SoundLength 
        { 
            get
            {
                return 0xFF;
            }

            internal set
            {
                _lengthCounter = ChannelMaxLength - value;
            }
        }

        private int _waveIndex = 0;
        private int _volumeShift;

        protected override int ChannelMaxLength => 256;


        private byte[] _waveTable = new byte[0x10] { 0x84, 0x40, 0x43, 0xAA, 
                                                     0x2D, 0x78, 0x92, 0x3C, 
                                                     0x60, 0x59, 0x59, 0xB0, 
                                                     0x34, 0xB8, 0x2E, 0xDA };

        internal void WriteWaveRam(ushort address, byte data)
        {
            //TODO: wave ram corruption
            _waveTable[address - 0xFF30] = data;
        }

        internal byte ReadWaveRam(ushort address)
        {
            if (_isEnabled)
            {
                if (AccessingWaveRam)
                    return _waveTable[_waveIndex];
                else
                    return 0xFF;
            }
                
            else
                return _waveTable[address - 0xFF30];
        }

        public override void AdvanceMachineCycle()
        {
            if (_timer == 0)
                return;

            _timer--;

            if (_timer == 0)
            {
                if (_useLowNibble)
                {
                    _waveIndex++;

                    if (_waveIndex > 0xF)
                        _waveIndex = 0;
                }

                _useLowNibble = !_useLowNibble;

                //fill buffer
                if(_useLowNibble)
                    _waveBuffer = (byte)(_waveTable[_waveIndex] & 0x0F);
                else
                    _waveBuffer = (byte)((_waveTable[_waveIndex] & 0xF0) >> 4);

                AccessingWaveRam = true;

                ReloadFrequencyTimer();
            }
        }        

        public override byte GetSample()
        {
            //length enabled and DAC power
            if (!IsEnabled())
                return 0;

            return (byte)((_waveBuffer >> _volumeShift) * 17);
        }

        public override bool IsEnabled()
        {
            return _isEnabled;
        }

        public override void Disable()
        {
            _timer = 0;
            _isEnabled = false;

            _useLowNibble = false;
            _waveIndex = 0;

            OnOff = 0;
            OutputLevel = 0;
            FrequencyHi = 0;
            FrequencyLo = 0;

            base.Disable();
        }

        private void Trigger()
        {
            _isEnabled = true;

            ReloadFrequencyTimer();

            ReloadLengthCounter();

            //set wave index to 0 but do not reload buffer
            _waveIndex = 0;

            if (IsDacDisabled())
                _isEnabled = false;
        }

        private bool IsDacDisabled()
        {
            return !_dacOn;
        }

        private void ReloadFrequencyTimer()
        {
            _timer = (2048 - _frequency) * 2;
        }
    }
}
