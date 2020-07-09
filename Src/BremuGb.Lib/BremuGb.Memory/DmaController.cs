using System;
using System.Collections.Generic;

using BremuGb.Common;
using BremuGb.Common.Constants;

//TODO: Check if dma stops in stop/halt or finishes normally

namespace BremuGb.Memory
{
    public class DmaController : IMemoryAccessDelegate
    {
        private readonly IRandomAccessMemory _mainMemory;
        private byte _dmaRegister;
        private bool _doSetupClock;

        private byte _currentAddressLsb;

        internal bool IsDmaRunning { get; private set; }

        internal byte DmaRegister
        {
            get => _dmaRegister;

            set
            {               
                if (value > 0xF1)
                    throw new InvalidOperationException($"DMA transfer address out of bounds: 0x{value:X2}");

                _dmaRegister = value;

                //initiate DMA transfer
                IsDmaRunning = true;
                _currentAddressLsb = 0x00;

                _doSetupClock = true;
            }
        }

        public DmaController(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;
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

            _currentAddressLsb++;

            //stop dma transfer when all bytes are copied
            if (_currentAddressLsb > 0x9F)
                IsDmaRunning = false;
        }

        public byte DelegateMemoryRead(ushort address)
        {
            if (address == VideoRegisters.DmaTransfer)
                return DmaRegister;

            throw new InvalidOperationException($"Invalid DMA address: 0x{address:X2}");
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address == VideoRegisters.DmaTransfer)
                DmaRegister = data;
        }

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            yield return VideoRegisters.DmaTransfer;
        }
    }
}
