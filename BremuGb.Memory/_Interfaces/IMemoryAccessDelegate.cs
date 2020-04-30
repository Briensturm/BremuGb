
namespace BremuGb.Memory
{
    public interface IMemoryAccessDelegate
    {
        public byte DelegateMemoryRead(byte currentByte);
        public bool DelegateMemoryWrite(byte writtenByte);
    }
}
