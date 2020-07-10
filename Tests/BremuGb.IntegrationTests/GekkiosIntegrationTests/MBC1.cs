using System;

using NUnit.Framework;

namespace BremuGb.IntegrationTests.GekkiosIntegrationTests
{
    class MBC1
    {
        [Test]
        public void Test_bits_bank1()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/bits_bank1.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_bits_bank2()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/bits_bank2.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_bits_mode()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/bits_mode.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_bits_ramg()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/bits_ramg.gb");
            testRomRunner.Run(10000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_ram_256kb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/ram_256kb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_ram_64kb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/ram_64kb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_rom_16Mb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/rom_16Mb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_rom_1Mb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/rom_1Mb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_rom_2Mb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/rom_2Mb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_rom_4Mb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/rom_4Mb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_rom_512kb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/rom_512kb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }

        [Test]
        public void Test_rom_8Mb()
        {
            var testRomRunner = new TestRomRunner("GekkiosIntegrationTests/Roms/mbc1/rom_8Mb.gb");
            testRomRunner.Run(5000000);

            testRomRunner.AssertGekkioTestResult();
        }
    }
}
