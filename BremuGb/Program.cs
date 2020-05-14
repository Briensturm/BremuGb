using System;
using System.Diagnostics;

using BremuGb.Cpu;
using BremuGb.Memory;
using BremuGb.Cartridge.MemoryBankController;
using BremuGb.Cartridge;

namespace bremugb.core
{
    class Program
    {
        static void Main(string[] args)
        {
            TestPerformance();
        }

        private static void TestPerformance()
        {
            Console.WriteLine("Testing Performance...");

            IRandomAccessMemory mainMemory = new MainMemory();
            IRandomAccessMemory mainMemoryProxy = new MainMemoryDmaProxy(mainMemory);
            DmaController dmaController = new DmaController(mainMemory);

            ICpuCore cpuCore = new CpuCore(mainMemoryProxy, new CpuState());

            IRomLoader romLoader = new FileRomLoader("testRom.gb");
            IMemoryBankController mbc = MBCFactory.CreateMBC(romLoader);
            mainMemoryProxy.RegisterMemoryAccessDelegateReadRange(0x0000, 0x7FFF, mbc as IMemoryAccessDelegate);

            Stopwatch stopWatch = new Stopwatch();
            if (!Stopwatch.IsHighResolution)
                throw new InvalidOperationException("No high res timer available in system!");

            stopWatch.Start();

            var multiplier = 10000;
            var cycle = 0;

            var cycles = 17556 * multiplier;
            while (cycle++ < cycles)
            {
                cpuCore.ExecuteCpuCycle();
                dmaController.AdvanceMachineCycle();
            }

            stopWatch.Stop();

            Console.WriteLine($"RunTime: {100 * stopWatch.ElapsedMilliseconds / (16.74 * multiplier)}%");
        }
    }
}
