using System;
using System.Diagnostics;

using BremuGb.Cpu;
using BremuGb.Memory;
using BremuGb.Cartridge.MemoryBankController;
using BremuGb.Cartridge;
using BremuGb.Timer;
using BremuGb.Video;
using BremuGb.Common.Constants;

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
            Timer timer = new Timer(mainMemory);
            PPU ppu = new PPU(mainMemory);            

            ICpuCore cpuCore = new CpuCore(mainMemoryProxy, new CpuState());

            IRomLoader romLoader = new FileRomLoader("testRom.gb");
            IMemoryBankController mbc = MBCFactory.CreateMBC(romLoader);
            mainMemoryProxy.RegisterMemoryAccessDelegateReadRange(0x0000, 0x7FFF, mbc as IMemoryAccessDelegate);

            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.LcdStatus, VideoRegisters.LcdStatus, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.LcdStatus, VideoRegisters.LcdStatus, ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.LineY, VideoRegisters.LineY, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.LineY, VideoRegisters.LineY, ppu);

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
                timer.AdvanceMachineCycle();
                ppu.AdvanceMachineCycle();
            }

            stopWatch.Stop();

            Console.WriteLine($"RunTime: {100 * stopWatch.ElapsedMilliseconds / (16.74 * multiplier)}%");
        }
    }
}
