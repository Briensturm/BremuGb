using BremuGb.Cartridge;
using BremuGb.Cartridge.MemoryBankController;
using BremuGb.Common.Constants;
using BremuGb.Cpu;
using BremuGb.Memory;
using BremuGb.Video;

namespace BremuGb.GameBoy
{
    public class Emulator
    {
        private PPU _ppu;
        private Timer.Timer _timer;
        private ICpuCore _cpuCore;
        private DmaController _dmaController;
        
        public Emulator()
        {
            IRandomAccessMemory mainMemory = new MainMemory();
            IRandomAccessMemory mainMemoryProxy = new MainMemoryDmaProxy(mainMemory);

            _dmaController = new DmaController(mainMemory);
            _timer = new Timer.Timer(mainMemory);
            _ppu = new PPU(mainMemory);

            _cpuCore = new CpuCore(mainMemoryProxy, new CpuState());

            IRomLoader romLoader = new FileRomLoader("testRom.gb");
            IMemoryBankController mbc = MBCFactory.CreateMBC(romLoader);
            mainMemoryProxy.RegisterMemoryAccessDelegateReadRange(0x0000, 0x7FFF, mbc as IMemoryAccessDelegate);

            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.LcdStatus, VideoRegisters.LcdStatus, _ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.LcdStatus, VideoRegisters.LcdStatus, _ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.LineY, VideoRegisters.LineY, _ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.LineY, VideoRegisters.LineY, _ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.ScrollX, VideoRegisters.ScrollX, _ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.ScrollX, VideoRegisters.ScrollX, _ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(VideoRegisters.ScrollY, VideoRegisters.ScrollY, _ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(VideoRegisters.ScrollY, VideoRegisters.ScrollY, _ppu);

            mainMemory.RegisterMemoryAccessDelegateWriteRange(0x8000, 0x97FF, _ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(0x9800, 0x9BFF, _ppu);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(0x9C00, 0x9FFF, _ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(0x8000, 0x97FF, _ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(0x9800, 0x9BFF, _ppu);
            mainMemory.RegisterMemoryAccessDelegateReadRange(0x9C00, 0x9FFF, _ppu);

            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.Divider, TimerRegisters.Divider, _timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.Divider, TimerRegisters.Divider, _timer);
            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.Timer, TimerRegisters.Timer, _timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.Timer, TimerRegisters.Timer, _timer);
            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.TimerControl, TimerRegisters.TimerControl, _timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.TimerControl, TimerRegisters.TimerControl, _timer);
            mainMemory.RegisterMemoryAccessDelegateReadRange(TimerRegisters.TimerLoad, TimerRegisters.TimerLoad, _timer);
            mainMemory.RegisterMemoryAccessDelegateWriteRange(TimerRegisters.TimerLoad, TimerRegisters.TimerLoad, _timer);   
        }

        public byte[] EmulateNextFrame()
        {
            bool frameIsReady;
            do
            {
                _cpuCore.ExecuteCpuCycle();

                _dmaController.AdvanceMachineCycle();
                _timer.AdvanceMachineCycle();

                frameIsReady = _ppu.AdvanceMachineCycle();
            } while (!frameIsReady);

            return _ppu._screenBitmap;
        }
    }
}
