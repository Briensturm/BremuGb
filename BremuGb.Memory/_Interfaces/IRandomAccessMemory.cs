
namespace BremuGb.Memory
{
    public interface IRandomAccessMemory
    {
        public byte ReadByte(ushort address);
        public bool WriteByte(ushort address);
    }
}
