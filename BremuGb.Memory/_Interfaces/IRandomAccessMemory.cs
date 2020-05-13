namespace BremuGb.Memory
{
    public interface IRandomAccessMemory
    {
        public byte ReadByte(ushort address);
        public void WriteByte(ushort address, byte data);

        //TODO: refactor names
        public void RegisterMemoryAccessDelegateWriteRange(ushort addressLo, ushort addressHi, IMemoryAccessDelegate memDelegate);
        public void RegisterMemoryAccessDelegateReadRange(ushort addressLo, ushort addressHi, IMemoryAccessDelegate memDelegate);
    }
}
