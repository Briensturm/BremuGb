using System;
using NUnit.Framework;

namespace BremuGb.IntegrationTests
{
    public class BlarggsIntegrationTests
    {
        [Test]
        public void Test_cpu_instrs()
        {
            var expectedResult = "cpu_instrs\n\n01:ok  02:ok  03:ok  04:ok  05:ok  06:ok  07:ok  08:ok  09:ok  10:ok  11:ok  \n\nPassed all tests\n";

            var testRomRunner = new TestRomRunner("BlarggsIntegrationTests/Roms/cpu_instrs.gb");
            testRomRunner.Run(58740146);

            var result = testRomRunner.GetSentSerialDataAsString();
            Console.WriteLine(result);

            Assert.AreEqual(expectedResult, result);            
        }
        
        [Test]
        public void Test_halt_bug()
        {
            var testRomRunner = new TestRomRunner("BlarggsIntegrationTests/Roms/halt_bug.gb");
            testRomRunner.Run(23580883);

            testRomRunner.AssertScreen("BlarggsIntegrationTests/ExpectedScreens/halt_bug.png");
        }

        [Test]
        public void Test_instr_timing()
        {
            var expectedResult = "instr_timing\n\n\nPassed\n";

            var testRomRunner = new TestRomRunner("BlarggsIntegrationTests/Roms/instr_timing.gb");
            testRomRunner.Run(1000000);

            var result = testRomRunner.GetSentSerialDataAsString();
            Console.WriteLine(result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Test_mem_timing()
        {
            var expectedResult = "mem_timing\n\n01:ok  02:ok  03:ok  \n\nPassed\n";

            var testRomRunner = new TestRomRunner("BlarggsIntegrationTests/Roms/mem_timing.gb");
            testRomRunner.Run(3000000);

            var result = testRomRunner.GetBlarggTestResultFromMemory();
            Console.WriteLine(result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Test_dmg_sound()
        {
            var expectedResult = "dmg_sound\n\n01:ok  02:ok  03:ok  04:ok  05:ok  06:ok  07:ok  08:ok  09:ok  10:ok  11:ok  12:ok  \n\nPassed\n";

             var testRomRunner = new TestRomRunner("BlarggsIntegrationTests/Roms/dmg_sound.gb");
            testRomRunner.Run(40000000);

            var result = testRomRunner.GetBlarggTestResultFromMemory();
            Console.WriteLine(result);

            Assert.AreEqual(expectedResult, result);
        }        
    }
}