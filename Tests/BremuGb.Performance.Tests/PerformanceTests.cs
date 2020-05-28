using System;
using System.Diagnostics;

using NUnit.Framework;

using BremuGb.Input;

namespace BremuGb.Performance.Tests
{
    public class PerformanceTests
    {
        [Test]
        public void Emulation_in_release_mode_must_be_at_least_twice_as_fast_as_real_gameboy()
        {
#if DEBUG
            var expectedPerformance = 100.0;
#else
            var expectedPerformance = 50.0;
#endif
            var gameBoy = new GameBoy("Roms/Blargg/cpu_instrs_looped.gb");

            Stopwatch stopWatch = new Stopwatch();
            if (!Stopwatch.IsHighResolution)
                Assert.Fail("No high res timer available in system!");

            stopWatch.Start();

            var multiplier = 10000;
            var cycle = 0;

            var cycles = 17556 * multiplier;
            while (cycle++ < cycles)
                gameBoy.AdvanceMachineCycle(JoypadState.None);

            stopWatch.Stop();

            var actualPerformance = 100 * stopWatch.ElapsedMilliseconds / (16.74 * multiplier);

            if(actualPerformance < expectedPerformance)
                Assert.Pass($"RunTime: {actualPerformance}%");
            else
                Assert.Fail($"RunTime: {actualPerformance}% is worse than expected runtime: {expectedPerformance}%");
        }
    }
}