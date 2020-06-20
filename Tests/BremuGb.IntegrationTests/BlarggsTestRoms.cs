using System.Runtime.InteropServices;

using NUnit.Framework;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BremuGb.IntegrationTests
{
    public class BlarggsTestRoms
    {
        [Test]
        public void Test_cpu_instrs()
        {
            var screen = RunTestRom("Roms/Blargg/cpu_instrs.gb", 58740146);
            DoesScreenMatchExpectation(screen, "ExpectedScreens/Blargg/cpu_instrs.png");                   
        }

        [Test]
        public void Test_halt_bug()
        {
            var screen = RunTestRom("Roms/Blargg/halt_bug.gb", 23580883);
            DoesScreenMatchExpectation(screen, "ExpectedScreens/Blargg/halt_bug.png");
        }

        [Test]
        public void Test_instr_timing()
        {
            var screen = RunTestRom("Roms/Blargg/instr_timing.gb", 1000000);
            DoesScreenMatchExpectation(screen, "ExpectedScreens/Blargg/instr_timing.png");
        }

        [Test]
        public void Test_mem_timing()
        {
            var screen = RunTestRom("Roms/Blargg/mem_timing.gb", 3000000);
            DoesScreenMatchExpectation(screen, "ExpectedScreens/Blargg/mem_timing.png");
        }

        private byte[] RunTestRom(string romPath, int cycleCount)
        {
            var completedCycles = 0;

            var gameBoy = new GameBoy(romPath);

            while (completedCycles < cycleCount)
            {
                gameBoy.AdvanceMachineCycle(Input.JoypadState.None);
                completedCycles++;
            }

            var pixelData = gameBoy.GetScreen();
            var image = Image.LoadPixelData<Rgb24>(pixelData, 160, 144);

            image.Save(romPath + ".png");

            return pixelData;
        }

        private void DoesScreenMatchExpectation(byte[] actualScreen, string pathToExpectedScreen)
        {
            var expectedImage = Image.Load<Rgb24>(pathToExpectedScreen);

            Assert.True(expectedImage.TryGetSinglePixelSpan(out var span));
            var array = MemoryMarshal.AsBytes(span).ToArray();

            for(int i = 0; i<array.Length; i++)
                Assert.AreEqual(array[i], actualScreen[i]);
        }
    }
}