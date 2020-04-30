
namespace BremuGb.Memory
{
    public class MainMemory : IRandomAccessMemory
    {
        public byte ReadByte(ushort address)
        {
            throw new System.NotImplementedException();
        }

        public bool WriteByte(ushort address)
        {
            //check for registered delegates
            //...

            throw new System.NotImplementedException();
        }
    }
}
