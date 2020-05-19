using System;

using BremuGb.Memory;
using BremuGb.Common.Constants;

namespace BremuGb.Video
{
    public class PPU : IMemoryAccessDelegate
    {
        PPUContext _ppuContext;
        IRandomAccessMemory _mainMemory;

        private int _coincidenceInterrupt;
        private int _oamInterrupt;
        private int _vblankInterrupt;
        private int _hblankInterrupt;

        private bool _statSignal;
        private byte _lycRegister;

        public byte[] _screenBitmap;

        internal byte ScrollX { get; private set; }
        internal byte ScrollY { get; private set; }

        private byte[] _tileData;
        private byte[] _tileMap0;
        private byte[] _tileMap1;

        private bool _frameIsReady;

        private byte LcdControl
        {
            get;
            set;
        }

        private byte LcdStatus
        {
            get
            {
                return (byte)(0x80 |
                            (_coincidenceInterrupt << 6) |
                            (_oamInterrupt << 5) |
                            (_vblankInterrupt << 4) |
                            (_hblankInterrupt << 3) |
                            (CheckLyCoincidence() << 2) |
                            _ppuContext.GetStateNumber());

                //TODO: state return 0 when lcd off
            }

            set
            {
                _coincidenceInterrupt = (value >> 6) & 0x01;
                _oamInterrupt = (value >> 5) & 0x01;
                _vblankInterrupt = (value >> 4) & 0x01;
                _hblankInterrupt = (value >> 3) & 0x01;


            }
        }

        public PPU(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;            

            _ppuContext = new PPUContext(new Mode2(), this);
            _ppuContext.VideoInterruptOccuredEvent += VideoInterruptOccured;

            _tileMap0 = new byte[32*32];
            _tileMap1 = new byte[32 * 32];
            _tileData = new byte[6144];

            _screenBitmap = new byte[Common.Constants.Video.ScreenWidth * Common.Constants.Video.ScreenHeight];
        }

        public void VideoInterruptOccured(int interruptType)
        {
            //request vblank interrupt
            var currentIf = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);
            _mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(currentIf | 0x01));

            //todo: request interrupt, but not if IF is executed
        }

        public byte DelegateMemoryRead(ushort address)
        {
            if (address >= 0x8000 && address <= 0x97FF)
                return _tileData[address - 0x8000];
            else if (address >= 0x9800 && address <= 0x9BFF)
                return _tileMap0[address - 0x9800];
            else if (address >= 0x9C00 && address <= 0x9FFF)
                return _tileMap1[address - 0x9C00];

            switch (address)
            {
                case VideoRegisters.LcdStatus:
                    return LcdStatus;

                case VideoRegisters.LineY:
                    //todo: force 0 when LCD is off
                    return (byte)_ppuContext.GetLineNumber();

                case VideoRegisters.LineYCompare:
                    return _lycRegister;

                case VideoRegisters.ScrollX:
                    return ScrollX;

                case VideoRegisters.ScrollY:
                    return ScrollY;

                default:
                    throw new InvalidOperationException($"0x{address:X2} is not a valid ppu address");
            }
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x8000 && address <= 0x97FF)
            {
                //Console.WriteLine($"Writing tile data 0x{data:X2} at 0x{address:X2}");
                _tileData[address - 0x8000] = data;
            }
            else if (address >= 0x9800 && address <= 0x9BFF)
            {
                //Console.WriteLine($"Writing tile map 0 data 0x{data:X2} at 0x{address:X2}");
                _tileMap0[address - 0x9800] = data;                
            }
            else if (address >= 0x9C00 && address <= 0x9FFF)
            {
                //Console.WriteLine($"Writing tile map 1 data 0x{data:X2} at 0x{address:X2}");
                _tileMap1[address - 0x9C00] = data;                
            }
            else
            {
                switch (address)
                {
                    case VideoRegisters.LcdStatus:
                        LcdStatus = data;

                        if (ShouldStatIrqBeRaised())
                            RaiseStatInterrupt();
                        break;

                    case VideoRegisters.LineY:
                        break;

                    case VideoRegisters.LineYCompare:
                        _lycRegister = data;

                        if (ShouldStatIrqBeRaised())
                            RaiseStatInterrupt();
                        break;

                    case VideoRegisters.ScrollX:
                        ScrollX = data;
                        break;

                    case VideoRegisters.ScrollY:
                        ScrollY = data;
                        break;

                    default:
                        throw new InvalidOperationException($"0x{address:X2} is not a valid ppu address");
                }
            }
        }

        private int CheckLyCoincidence()
        {
            return _ppuContext.GetLineNumber() == _lycRegister ? 1 : 0;
        }

        public bool AdvanceMachineCycle()
        {
            _ppuContext.AdvanceMachineCycle();

            if (ShouldStatIrqBeRaised())
                RaiseStatInterrupt();

            if(_frameIsReady)
            {
                _frameIsReady = false;
                return true;
            }

            return false;
        }

        private void RaiseStatInterrupt()
        {
            var currentIf = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);
            _mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(currentIf | 0x02));

            //todo: request interrupt, but not if IF is executed
        }

        private bool GetStatSignal()
        {
            //TODO handle LCD off in signal

            return ((CheckLyCoincidence() & _coincidenceInterrupt) == 0x01) ||
                (_ppuContext.GetStateNumber() == 0 && _hblankInterrupt == 1) ||
                (_ppuContext.GetStateNumber() == 2 && _oamInterrupt == 1) ||
                (_ppuContext.GetStateNumber() == 1 && (_vblankInterrupt == 1 || _oamInterrupt == 1));
        }

        private bool ShouldStatIrqBeRaised()
        {
            //check for stat interrupt
            var newStatSignal = GetStatSignal();

            //detect rising edge
            var raiseStatIrq = newStatSignal && !_statSignal;             
            _statSignal = newStatSignal;

            return raiseStatIrq;
        }

        internal int GetBackgroundPixel(byte x, byte y)
        {
            int tileX = x / 8;
            int tileY = y / 8;

            int tileOffsetX = x % 8;
            int tileOffsetY = y % 8;

            //todo: select active tilemap
            var tileIndex = (sbyte)_tileMap0[tileX + tileY*32];

            //get tile data
            var tileData0 = _tileData[0x1000 + (tileIndex << 4) + tileOffsetY * 2];
            var tileData1 = _tileData[0x1000 + (tileIndex << 4) + tileOffsetY * 2 + 1];

            var msbSet = (tileData0 & (0x80 >> tileOffsetX)) > 0;
            var lsbSet = (tileData1 & (0x80 >> tileOffsetX)) > 0;

            return (msbSet ? 0x2 : 0) | (lsbSet ? 0x1 : 0);

            //todo: check tile addressing mode

        }

        internal void WritePixel(int shade, int x, int y)
        {
            //todo: handle palette?  

            _screenBitmap[y * 160 + x] = (byte)shade;

            //if (shade != 0)
                //Console.WriteLine("pixel " + x + " " + y + " in shade " + shade);
        }

        internal void RaiseNextFrameIsReadyEvent()
        {
            NextFrameIsReadyEvent?.Invoke(_screenBitmap);

            _frameIsReady = true;
        }

        public delegate void NextFrameIsReady(byte[] frameBitmap);

        public event NextFrameIsReady NextFrameIsReadyEvent;
    }
}
