namespace BremuGb.Audio.SoundChannels
{
    internal class WaveChannel : SoundChannelBase
    {
        private int _frequencyTimer;
        private int _frequency;

        private bool _dacOn;        

        private bool _useLowNibble;
        private int _waveIndex;
        private byte _sampleBuffer;

        private byte[] _waveTable = new byte[0x10] { 0x84, 0x40, 0x43, 0xAA,
                                                     0x2D, 0x78, 0x92, 0x3C,
                                                     0x60, 0x59, 0x59, 0xB0,
                                                     0x34, 0xB8, 0x2E, 0xDA };

        private int _volumeCode;
        private int[] _volumeShift = new int[] { 4, 0, 1, 2 };

        private bool _accessWaveRamApuCycle0;
        private bool _accessWaveRamApuCycle1;

        protected override int ChannelMaxLength => 256;

        internal byte DacPower
        { 
            get => (byte)(_dacOn ? 0xFF : 0x7F);

            set
            {
                _dacOn = (value & 0x80) == 0x80;

                if (IsDacDisabled())
                    _isEnabled = false;
            }
        }

        internal byte LengthLoad
        {
            get => 0xFF;
            set => _lengthCounter = ChannelMaxLength - value;
        }

        internal byte VolumeCode 
        { 
            get => (byte)((_volumeCode << 5) | 0x9F);         
            set => _volumeCode = (value & 0x60) >> 5;
        }

        public byte FrequencyLsb
        {
            get => 0xFF;
            internal set => _frequency = (_frequency & 0x700) | value;
        }

        internal byte LengthEnable
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
            //use APU cycles for wave ram read timing, 2 APU cycles per m-cycle
            for (int i = 0; i < 2; i++)
                AdvanceApuCycle(i);
        }

        public override byte GetSample()
        {
            if (!IsEnabled())
                return 0;

            if (_useLowNibble)
                return (byte)(((_sampleBuffer & 0x0F) >> _volumeShift[_volumeCode]) * 17);
            else
                return (byte)((((_sampleBuffer & 0xF0) >> 4) >> _volumeShift[_volumeCode]) * 17);
        }

        public override bool IsEnabled()
        {
            return _isEnabled;
        }

        public override void Disable()
        {            
            _isEnabled = false;
            _useLowNibble = false;
            _frequencyTimer = 0;
            _waveIndex = 0;

            DacPower = 0;
            VolumeCode = 0;            
            FrequencyLsb = 0;
            LengthEnable = 0;

            base.Disable();
        }

        internal void WriteWaveRam(ushort address, byte data)
        {
            if (_isEnabled)
            {
                if (_accessWaveRamApuCycle0)
                    _waveTable[_waveIndex] = data;
            }

            else
                _waveTable[address - 0xFF30] = data;
        }

        internal byte ReadWaveRam(ushort address)
        {
            if (_isEnabled)
            {
                if (_accessWaveRamApuCycle0)
                    return _waveTable[_waveIndex];
                else
                    return 0xFF;             
            }
                
            else
                return _waveTable[address - 0xFF30];
        }               

        private void AdvanceApuCycle(int apuIteration)
        {
            if (apuIteration == 0)
                _accessWaveRamApuCycle0 = false;
            else if (apuIteration == 1)
                _accessWaveRamApuCycle1 = false;

            if (_frequencyTimer == 0)
                return;

            _frequencyTimer--;
            if (_frequencyTimer == 0)
            {
                if (_useLowNibble)
                {
                    _waveIndex++;

                    if (_waveIndex > 0xF)
                        _waveIndex = 0;
                }

                _useLowNibble = !_useLowNibble;

                //fill buffer
                _sampleBuffer = _waveTable[_waveIndex];

                if (apuIteration == 0)
                    _accessWaveRamApuCycle0 = true;
                else if (apuIteration == 1)
                    _accessWaveRamApuCycle1 = true;

                ReloadFrequencyTimer();
            }
        }         

        private void Trigger()
        {
            _isEnabled = true;

            HandleWaveRamCorruption();

            ReloadFrequencyTimer();

            //magic constant to achieve obscure wave ram behavior
            _frequencyTimer += 2;

            ReloadLengthCounter();

            //set wave index to 0 but do not reload buffer
            _waveIndex = 0;
            _useLowNibble = false;

            if (IsDacDisabled())
                _isEnabled = false;
        }

        private void HandleWaveRamCorruption()
        {
            if (_accessWaveRamApuCycle1)
            {
                if (_waveIndex <= 3)
                    _waveTable[0] = _waveTable[_waveIndex];

                else if (_waveIndex <= 7)
                {
                    for (int i = 0; i < 4; i++)
                        _waveTable[i] = _waveTable[i + 4];
                }
                else if (_waveIndex <= 11)
                {
                    for (int i = 0; i < 4; i++)
                        _waveTable[i] = _waveTable[i + 8];
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                        _waveTable[i] = _waveTable[i + 12];
                }
            }
        }

        private bool IsDacDisabled()
        {
            return !_dacOn;
        }

        private void ReloadFrequencyTimer()
        {
            _frequencyTimer = (2048 - _frequency);
        }
    }
}