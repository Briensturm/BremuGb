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
using BremuGb.Serial;

namespace BremuGb
{
    public class GameBoy
    {
        private readonly ICpuCore _cpuCore;

        private readonly PixelProcessingUnit _pixelProcessingUnit;
        private readonly Timer _timer;        
        private readonly DmaController _dmaController;
        private readonly AudioProcessingUnit _audioProcessingUnit;
        private readonly SerialController _serialController;
        private readonly Joypad _joypad;

        private readonly IRandomAccessMemory _mainMemory;
        private readonly IMemoryBankController _memoryBankController;
        private readonly IRamManager _ramManager;

        private readonly Logger _logger;

        public event EventHandler OutputTerminalChangedEvent
        {
            add { _audioProcessingUnit.OutputTerminalChangedEvent += value; }
            remove { _audioProcessingUnit.OutputTerminalChangedEvent -= value; }
        }

        public event EventHandler MasterVolumeChangedEvent
        {
            add { _audioProcessingUnit.MasterVolumeChangedEvent += value; }
            remove { _audioProcessingUnit.MasterVolumeChangedEvent -= value; }
        }

        public event EventHandler NextFrameReadyEvent
        {
            add { _pixelProcessingUnit.NextFrameReadyEvent += value; }
            remove { _pixelProcessingUnit.NextFrameReadyEvent -= value; }
        }

        public event EventHandler<SerialEventArgs> SerialDataTransferedEvent
        {
            add { _serialController.SerialDataTransferedEvent += value; }
            remove { _serialController.SerialDataTransferedEvent -= value; }
        }        

        public GameBoy(string romPath)
        {
            _logger = new Logger();

            _mainMemory = new MainMemory();
            _dmaController = new DmaController(_mainMemory);
            
            _timer = new Timer(_mainMemory);            
            _joypad = new Joypad();

            _serialController = new SerialController(_mainMemory);

            _pixelProcessingUnit = new PixelProcessingUnit(_mainMemory, _logger);
            _audioProcessingUnit = new AudioProcessingUnit();

            _cpuCore = new CpuCore(_mainMemory, new CpuState(), _logger);

            IRomLoader romLoader = new FileRomLoader(romPath);
            _ramManager = new FileRamManager(Path.ChangeExtension(romPath, ".sav"));

            _memoryBankController = MBCFactory.CreateMBC(romLoader);
            _memoryBankController.LoadRam(_ramManager);

            _mainMemory.RegisterMemoryAccessDelegate(_memoryBankController as IMemoryAccessDelegate);
            _mainMemory.RegisterMemoryAccessDelegate(_dmaController);
            _mainMemory.RegisterMemoryAccessDelegate(_pixelProcessingUnit);
            _mainMemory.RegisterMemoryAccessDelegate(_timer);
            _mainMemory.RegisterMemoryAccessDelegate(_joypad);
            _mainMemory.RegisterMemoryAccessDelegate(_audioProcessingUnit);
            _mainMemory.RegisterMemoryAccessDelegate(_serialController);
        }

        public void AdvanceMachineCycle(JoypadState joypadState)
        {
            _joypad.SetJoypadState(joypadState);

            _audioProcessingUnit.AdvanceMachineCycle();            
            
            _cpuCore.AdvanceMachineCycle();

            _dmaController.AdvanceMachineCycle();
            _timer.AdvanceMachineCycle();
            _serialController.AdvanceMachineCycle();
            
            _pixelProcessingUnit.AdvanceMachineCycle();
        }

        public byte MemoryRead(ushort address)
        {
            return _mainMemory.ReadByte(address);
        }

        public SoundOutputTerminal GetOutputTerminal(Channels soundChannel)
        {
            return _audioProcessingUnit.GetOutputTerminal(soundChannel);
        }

        public int GetMasterVolumeLeft()
        {
            return _audioProcessingUnit.GetMasterVolumeLeft();
        }

        public int GetMasterVolumeRight()
        {
            return _audioProcessingUnit.GetMasterVolumeRight();
        }

        public byte GetAudioSample(Channels soundChannel)
        {
            return _audioProcessingUnit.GetCurrentSample(soundChannel);
        }

        public void SaveRam()
        {
            _memoryBankController.SaveRam(_ramManager);
        }

        public byte[] GetScreen()
        {
            return _pixelProcessingUnit.GetScreen();
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
