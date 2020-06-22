namespace BremuGb.Audio.SoundChannels
{
    internal class WaveChannel : SoundChannelBase
    {
        private int _timer;
        private byte _waveBuffer;

        private bool _dacOn;

        private bool _compareLength;

        private int _frequency;
        private int _lengthCounter;

        public byte[] WaveTable { get; }
        public byte OnOff 
        { 
            get
            {
                return (byte)(_dacOn ? 0xFF : 0x7F);
            }

            internal set
            {
                _dacOn = (value & 0x80) == 0x80;
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

                _compareLength = (value & 0x40) == 0x40;

                //handle trigger
                if ((value & 0x80) == 0x80)
                {
                    _timer = (2048 - _frequency)*2;

                    if (_lengthCounter == 0)
                        _lengthCounter = 256;

                    //set wave index to 0 but do not reload buffer
                    _waveIndex = 0;
                }
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
                _lengthCounter = value;
            }
        }

        private int _waveIndex = 0;
        private int _volumeShift;

        public WaveChannel()
        {
            WaveTable = new byte[0x10] { 0x84, 0x40, 0x43, 0xAA, 
                                         0x2D, 0x78, 0x92, 0x3C, 
                                         0x60, 0x59, 0x59, 0xB0, 
                                         0x34, 0xB8, 0x2E, 0xDA };
        }

        public override void AdvanceMachineCycle()
        {
            if (_timer == 0)
                return;

            _timer--;

            if (_timer == 0)
            {
                _waveIndex++;

                if (_waveIndex > 0xF)
                    _waveIndex = 0;

                //fill buffer
                _waveBuffer = WaveTable[_waveIndex];

                //reload timer
                _timer = (2048 - _frequency)*2;
            }
        }

        public override void ClockLength()
        {
            if (_compareLength && _lengthCounter > 0)
                _lengthCounter--;
        }

        public override byte GetSample()
        {
            //length enabled and DAC power
            if (_lengthCounter == 0 || !_dacOn)
                return 0;

            return (byte)(_waveBuffer >> _volumeShift);
        }
    }
}
