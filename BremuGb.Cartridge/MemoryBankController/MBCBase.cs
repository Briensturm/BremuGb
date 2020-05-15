using BremuGb.Memory;

namespace BremuGb.Cartridge.MemoryBankController
{
    abstract class MBCBase : IMemoryBankController, IMemoryAccessDelegate
    {
        protected byte[] _romData;

        public MBCBase(byte[] romData)
        {
            _romData = romData;
        }

        abstract public byte DelegateMemoryRead(ushort address);
        abstract public void DelegateMemoryWrite(ushort address, byte data);
    }
}
