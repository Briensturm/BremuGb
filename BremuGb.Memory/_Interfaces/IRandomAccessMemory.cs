namespace BremuGb.Memory
{
    public interface IRandomAccessMemory
    {
        public byte ReadByte(ushort address);
        public void WriteByte(ushort address, byte data);

        public void RegisterMemoryAccessDelegate(IMemoryAccessDelegate memoryDelegate);
    }
}
