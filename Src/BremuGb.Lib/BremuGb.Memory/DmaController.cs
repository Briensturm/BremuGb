using System;

using BremuGb.Common;

//TODO: Check if dma stops in stop/halt or finishes normally

namespace BremuGb.Memory
{
    public class DmaController
    {
        private readonly IRandomAccessMemory _mainMemory;
        private byte _dmaRegister;
        private bool _doSetupClock;

        private readonly Logger _logger;

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

                //_logger.Log($"DMA transfer started: 0x{value:X2}");

                _dmaRegister = value;

                //initiate DMA transfer
                IsDmaRunning = true;
                _currentAddressLsb = 0x00;

                _doSetupClock = true;
            }
        }

        public DmaController(IRandomAccessMemory mainMemory, Logger logger)
        {
            _mainMemory = mainMemory;
            _logger = logger;
        }

        public void AdvanceMachineCycle()
        {
            if (!IsDmaRunning)
                return;

            if (_doSetupClock)
            {
                _doSetupClock = false;
                return;
            }            

            //copy one byte per machine cycle
            var sourceByte = _mainMemory.ReadByte((ushort)((DmaRegister << 8) | _currentAddressLsb));
            _mainMemory.WriteByte((ushort)((0xFE << 8) | _currentAddressLsb), sourceByte);

            //_logger.Log($"DMA transfer copied byte at: 0x{_currentAddressLsb:X2}");

            _currentAddressLsb++;

            //stop dma transfer when all bytes are copied
            if (_currentAddressLsb > 0x9F)
            {
                //_logger.Log("DMA transfer done");
                IsDmaRunning = false;
            }
        }       
    }
}
