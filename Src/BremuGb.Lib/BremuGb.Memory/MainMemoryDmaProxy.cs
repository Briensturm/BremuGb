using System;

using BremuGb.Common.Constants;

namespace BremuGb.Memory
{
    public class MainMemoryDmaProxy : IRandomAccessMemory
    {
        IRandomAccessMemory _mainMemory;
        DmaController _dmaController;

        public MainMemoryDmaProxy(IRandomAccessMemory mainMemory, DmaController dmaController)
        {
            _mainMemory = mainMemory;
            _dmaController = dmaController;
        }

        public byte ReadByte(ushort address)
        {
            if (_dmaController.IsOamLocked && address <= 0xFE9F && address >= 0xFE00)
                return 0xFF;

            else if (address == VideoRegisters.DmaTransfer)
                return _dmaController.DmaRegister;

            return _mainMemory.ReadByte(address);
        }

        public void WriteByte(ushort address, byte data)
        {
            if (_dmaController.IsOamLocked && address <= 0xFE9F && address >= 0xFE00)
                return;

            else if (address == VideoRegisters.DmaTransfer)
                _dmaController.DmaRegister = data;

            _mainMemory.WriteByte(address, data);
        }

        public void RegisterMemoryAccessDelegate(IMemoryAccessDelegate memoryDelegate)
        {
            _mainMemory.RegisterMemoryAccessDelegate(memoryDelegate);
        }
    }
}