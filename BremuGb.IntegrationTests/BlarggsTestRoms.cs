using NUnit.Framework;

namespace BremuGb.IntegrationTests
{
    public class BlarggsTestRoms
    {
        [Test]
        public void Test_cpu_instrs()
        {
            var gameBoy = new GameBoy(@"Roms/Blargg/cpu_instrs.gb");

            for(int i = 0; i<100000; i++)
            {
                gameBoy.AdvanceMachineCycle();
            }
        }
    }
}