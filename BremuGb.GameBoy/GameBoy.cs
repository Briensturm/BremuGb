using BremuGb.Cartridge.MemoryBankController;
using BremuGb.Cartridge;
using BremuGb.Common;
using BremuGb.Cpu;
using BremuGb.Memory;
using BremuGb.Video;
using BremuGb.Input;

namespace BremuGb
{
    public class GameBoy
    {
        private readonly ICpuCore _cpuCore;

        private readonly PixelProcessingUnit _ppu;
        private readonly Timer _timer;        
        private readonly DmaController _dmaController;
        private readonly Joypad _joypad;

        private readonly Logger _logger;
        
        public GameBoy(string romPath)
        {
            _logger = new Logger();

            IRandomAccessMemory mainMemory = new MainMemory();
            _dmaController = new DmaController(mainMemory, _logger);

            IRandomAccessMemory mainMemoryProxy = new MainMemoryDmaProxy(mainMemory, _dmaController);
            
            _timer = new Timer(mainMemory);
            _ppu = new PixelProcessingUnit(mainMemory, _logger);
            _joypad = new Joypad();

            _cpuCore = new CpuCore(mainMemoryProxy, new CpuState(), _logger);

            IRomLoader romLoader = new FileRomLoader(romPath);
            IMemoryBankController mbc = MBCFactory.CreateMBC(romLoader);

            mainMemoryProxy.RegisterMemoryAccessDelegate(mbc as IMemoryAccessDelegate);
            mainMemory.RegisterMemoryAccessDelegate(_ppu);
            mainMemory.RegisterMemoryAccessDelegate(_timer);
            mainMemory.RegisterMemoryAccessDelegate(_joypad);
        }

        public bool AdvanceMachineCycle(JoypadState joypadState)
        {
            _joypad.SetJoypadState(joypadState);

            _cpuCore.AdvanceMachineCycle();

            _dmaController.AdvanceMachineCycle();
            _timer.AdvanceMachineCycle();

            return _ppu.AdvanceMachineCycle();
        }

        public byte[] GetScreen()
        {
            return _ppu.GetScreen();
        }

        public void SaveLog(string path)
        {
            _logger.SaveLogFile(path);
        }

        public void EnableLogging()
        {
            _logger.Enable();
        }

        public void DisableLogging()
        {
            _logger.Disable();
        }
    }
}
