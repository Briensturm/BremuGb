
using BremuGb.Memory;

namespace BremuGb.Cpu
{
    public class CpuCore : ICpuCore
    {
        private IRandomAccessMemory mainMemory;

        private ushort programCounter;
        private ushort stackPointer;

        private bool interruptMasterEnable;

        private byte currentOpcode;

        public CpuCore(IRandomAccessMemory mainMemory)
        {
            this.mainMemory = mainMemory;
        }

        public void Clock()
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void Run()
        {
            throw new System.NotImplementedException();
        }
    }
}
