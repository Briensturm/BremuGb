using System;

namespace BremuGb.Memory
{
    public class DmaController
    {
        private IRandomAccessMemory _mainMemory;
        private byte _dmaRegister;

        private byte _currentAddressLsb;

        internal bool IsDmaRunning { get; private set; }

        internal byte DmaRegister
        {
            get
            {
                return _dmaRegister;
            }

            set
            {               
                if (value > 0xF1)
                    throw new InvalidOperationException($"DMA transfer address out of bounds: 0x{value:X2}");

                _dmaRegister = value;

                //initiate DMA transfer
                IsDmaRunning = true;
                _currentAddressLsb = 0x00;
            }
        }

        //private byte 


        public DmaController(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;
        }

        public void AdvanceMachineCycle()
        {
            if (!IsDmaRunning)
                return;

            //copy one byte per machine cycle
            var sourceByte = _mainMemory.ReadByte((ushort)((DmaRegister << 8) | _currentAddressLsb));
            _mainMemory.WriteByte((ushort)((0xFE << 8) | _currentAddressLsb), sourceByte);

            _currentAddressLsb++;

            //stop dma transfer when all bytes are copied
            if (_currentAddressLsb > 0x9F)
                IsDmaRunning = false;
        }       
    }
}
