namespace BremuGb.Memory
{
    public interface IMemoryAccessDelegate
    {
        public byte DelegateMemoryRead(ushort address);
        public void DelegateMemoryWrite(ushort address, byte data);
    }
}
