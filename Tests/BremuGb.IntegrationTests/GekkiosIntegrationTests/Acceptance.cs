using NUnit.Framework;
using System.IO;

namespace BremuGb.IntegrationTests.GekkiosIntegrationTests
{
    [TestFixture]
    class Acceptance
    {
        [TestCaseSource("GetRomFiles")]
        public void Test_acceptance(string romPath)
        {
            var testRomRunner = new TestRomRunner(romPath);
            testRomRunner.RunUntilSerialResponse(50000000);

            testRomRunner.AssertGekkioTestResult();
        }

        private static string[] GetRomFiles()
        {
            return Directory.GetFiles("GekkiosIntegrationTests/Roms/acceptance/", "*.gb"); 
        }
    }
}
