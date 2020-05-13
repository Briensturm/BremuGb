namespace BremuGb.Memory
{
    public interface IMemoryAccessDelegate
    {
        public byte DelegateMemoryRead(ushort address);
        public bool DelegateMemoryWrite(byte data);
    }
}
