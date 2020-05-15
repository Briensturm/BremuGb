using System;

using BremuGb.Common.Constants;

namespace BremuGb.Memory
{
    public class MainMemoryDmaProxy : IRandomAccessMemory
    {
        IRandomAccessMemory _mainMemory;
        DmaController _dmaController;

        public MainMemoryDmaProxy(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;
            _dmaController = new DmaController(mainMemory);
        }

        public byte ReadByte(ushort address)
        {
            if (_dmaController.IsDmaRunning && address <= 0xFE9F && address >= 0xFE00)
                throw new InvalidOperationException($"Reading at OAM address 0x{address:X2} during dma transfer not allowed");

            if (address == VideoRegisters.DmaTransfer)
                return _dmaController.DmaRegister;

            return _mainMemory.ReadByte(address);
        }

        public void WriteByte(ushort address, byte data)
        {
            if (_dmaController.IsDmaRunning && address <= 0xFE9F && address >= 0xFE00)
                throw new InvalidOperationException($"Writing at OAM address 0x{address:X2} during dma transfer not allowed");

            if (address == VideoRegisters.DmaTransfer)
                _dmaController.DmaRegister = data;

            _mainMemory.WriteByte(address, data);
        }        

        public void RegisterMemoryAccessDelegateWriteRange(ushort addressLo, ushort addressHi, IMemoryAccessDelegate memDelegate)
        {
            _mainMemory.RegisterMemoryAccessDelegateWriteRange(addressLo, addressHi, memDelegate);
        }

        public void RegisterMemoryAccessDelegateReadRange(ushort addressLo, ushort addressHi, IMemoryAccessDelegate memDelegate)
        {
            _mainMemory.RegisterMemoryAccessDelegateReadRange(addressLo, addressHi, memDelegate);
        }
    }
}
