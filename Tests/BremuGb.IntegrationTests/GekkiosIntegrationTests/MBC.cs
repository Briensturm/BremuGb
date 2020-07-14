using System.IO;
using NUnit.Framework;

namespace BremuGb.IntegrationTests.GekkiosIntegrationTests
{
    class MBC
    {
        [TestCaseSource("GetMbc1RomFiles")]
        public void Test_mbc1(string romPath)
        {
            var testRomRunner = new TestRomRunner(romPath);
            testRomRunner.RunUntilSerialResponse(50000000);

            testRomRunner.AssertGekkioTestResult();
        }

        private static string[] GetMbc1RomFiles()
        {
            return Directory.GetFiles("GekkiosIntegrationTests/Roms/mbc1/", "*.gb");
        }

        [TestCaseSource("GetMbc2RomFiles")]
        public void Test_mbc2(string romPath)
        {
            var testRomRunner = new TestRomRunner(romPath);
            testRomRunner.RunUntilSerialResponse(50000000);

            testRomRunner.AssertGekkioTestResult();
        }

        private static string[] GetMbc2RomFiles()
        {
            return Directory.GetFiles("GekkiosIntegrationTests/Roms/mbc2/", "*.gb");
        }
    }
}
