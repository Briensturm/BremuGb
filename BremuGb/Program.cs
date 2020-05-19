using System;

using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Mathematics;

namespace BremuGb.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestPerformance();

            RunWithGui();
        }

        static void RunWithGui()
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
            {
                Title = "BremuGb",
                Size = new Vector2i(160 * 2, 144 * 2),
                WindowBorder = OpenToolkit.Windowing.Common.WindowBorder.Fixed
            };

            GameWindowSettings gameWindowSettings = new GameWindowSettings
            {
                RenderFrequency = 60
            };
            //gameWindowSettings.UpdateFrequency = 60;

            using (var window = new Window(nativeWindowSettings, gameWindowSettings))
            {
                window.Run();
            }
        }

        /*private static void TestPerformance()
        {
            Console.WriteLine("Testing Performance...");

            IRandomAccessMemory mainMemory = new MainMemory();
            IRandomAccessMemory mainMemoryProxy = new MainMemoryDmaProxy(mainMemory);

            DmaController dmaController = new DmaController(mainMemory);
            Timer.Timer timer = new Timer.Timer(mainMemory);
            PPU ppu = new PPU(mainMemory);            

            ICpuCore cpuCore = new CpuCore(mainMemoryProxy, new CpuState());

            IRomLoader romLoader = new FileRomLoader("testRom.gb");
            IMemoryBankController mbc = MBCFactory.CreateMBC(romLoader);
            mainMemoryProxy.RegisterMemoryAccessDelegateReadRange(0x0000, 0x7FFF, mbc as IMemoryAccessDelegate);

            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.LcdStatus, VideoRegisters.LcdStatus, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.LcdStatus, VideoRegisters.LcdStatus, ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.LineY, VideoRegisters.LineY, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.LineY, VideoRegisters.LineY, ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.ScrollX, VideoRegisters.ScrollX, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.ScrollX, VideoRegisters.ScrollX, ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.ScrollY, VideoRegisters.ScrollY, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.ScrollY, VideoRegisters.ScrollY, ppu);

            mainMemory.RegisterMemoryAccessDelegateWriteRange(0x8000, 0x97FF, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(0x9800, 0x9BFF, ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(0x9C00, 0x9FFF, ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(0x8000, 0x97FF, ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(0x9800, 0x9BFF, ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(0x9C00, 0x9FFF, ppu);

            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.Divider, TimerRegisters.Divider, timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.Divider, TimerRegisters.Divider, timer);
            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.Timer, TimerRegisters.Timer, timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.Timer, TimerRegisters.Timer, timer);
            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.TimerControl, TimerRegisters.TimerControl, timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.TimerControl, TimerRegisters.TimerControl, timer);
            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.TimerLoad, TimerRegisters.TimerLoad, timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.TimerLoad, TimerRegisters.TimerLoad, timer);


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
        }*/
    }
}
