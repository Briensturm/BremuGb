using System;
using System.IO;

using BremuGb.Cartridge.MemoryBankController;
using BremuGb.Cartridge;
using BremuGb.Common;
using BremuGb.Cpu;
using BremuGb.Memory;
using BremuGb.Video;
using BremuGb.Input;
using BremuGb.Audio;
using BremuGb.Audio.SoundChannels;

namespace BremuGb
{
    public class GameBoy
    {
        private readonly ICpuCore _cpuCore;

        private readonly PixelProcessingUnit _ppu;
        private readonly Timer _timer;        
        private readonly DmaController _dmaController;
        private readonly AudioProcessingUnit _apu;
        private readonly Joypad _joypad;

        private readonly IMemoryBankController _mbc;
        private readonly IRamManager _ramManager;

        private readonly Logger _logger;

        public event EventHandler OutputTerminalChangedEvent
        {
            add { _apu.OutputTerminalChangedEvent += value; }
            remove { _apu.OutputTerminalChangedEvent -= value; }
        }

        public event EventHandler MasterVolumeChangedEvent
        {
            add { _apu.MasterVolumeChangedEvent += value; }
            remove { _apu.MasterVolumeChangedEvent -= value; }
        }

        public event EventHandler NextFrameReadyEvent
        {
            add { _ppu.NextFrameReadyEvent += value; }
            remove { _ppu.NextFrameReadyEvent -= value; }
        }

        public GameBoy(string romPath)
        {
            _logger = new Logger();

            IRandomAccessMemory mainMemory = new MainMemory();
            _dmaController = new DmaController(mainMemory);
            
            _timer = new Timer(mainMemory);
            _ppu = new PixelProcessingUnit(mainMemory, _logger);
            _joypad = new Joypad();

            _apu = new AudioProcessingUnit();

            _cpuCore = new CpuCore(mainMemory, new CpuState(), _logger);

            IRomLoader romLoader = new FileRomLoader(romPath);
            _ramManager = new FileRamManager(Path.ChangeExtension(romPath, ".sav"));

            _mbc = MBCFactory.CreateMBC(romLoader);
            _mbc.LoadRam(_ramManager);

            mainMemory.RegisterMemoryAccessDelegate(_mbc as IMemoryAccessDelegate);
            mainMemory.RegisterMemoryAccessDelegate(_dmaController);
            mainMemory.RegisterMemoryAccessDelegate(_ppu);
            mainMemory.RegisterMemoryAccessDelegate(_timer);
            mainMemory.RegisterMemoryAccessDelegate(_joypad);
            mainMemory.RegisterMemoryAccessDelegate(_apu);
        }

        public void AdvanceMachineCycle(JoypadState joypadState)
        {
            _joypad.SetJoypadState(joypadState);

            _apu.AdvanceMachineCycle();            
            
            _cpuCore.AdvanceMachineCycle();

            _dmaController.AdvanceMachineCycle();
            _timer.AdvanceMachineCycle();
            
            _ppu.AdvanceMachineCycle();
        }

        public SoundOutputTerminal GetOutputTerminal(Channels soundChannel)
        {
            return _apu.GetOutputTerminal(soundChannel);
        }

        public int GetMasterVolumeLeft()
        {
            return _apu.GetMasterVolumeLeft();
        }

        public int GetMasterVolumeRight()
        {
            return _apu.GetMasterVolumeRight();
        }

        public byte GetAudioSample(Channels soundChannel)
        {
            return _apu.GetCurrentSample(soundChannel);
        }

        public void SaveRam()
        {
            _mbc.SaveRam(_ramManager);
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
