﻿using System;
using System.Linq;
using System.Collections.Generic;

using BremuGb.Memory;
using BremuGb.Common;
using BremuGb.Common.Constants;


namespace BremuGb.Video
{
    public class PPU : IMemoryAccessDelegate
    {
        readonly PPUContext _ppuContext;
        readonly IRandomAccessMemory _mainMemory;

        private readonly Logger _logger;

        private int _coincidenceInterrupt;
        private int _oamInterrupt;
        private int _vblankInterrupt;
        private int _hblankInterrupt;

        private bool _statSignal;
        private byte _lycRegister;

        public byte[] _screenBitmap;

        internal byte ScrollX { get; private set; }
        internal byte ScrollY { get; private set; }

        private readonly byte[] _tileData;
        private readonly byte[] _tileMap0;
        private readonly byte[] _tileMap1;

        public bool _frameIsReady;
        private int _lcdEnable;

        private void SetLcdEnable(int value)
        {
            _lcdEnable = value;

            if (value == 0)
            {
                _ppuContext._lineCounter = 0;
                _ppuContext.TransitionTo(new Mode2());

                for (int i = 0; i < _screenBitmap.Length; i++)
                {
                    _screenBitmap[i] = 0;
                }

                _frameIsReady = true;
            }
        }
        private int _windowTileMap;
        private int _windowDisplayEnable;
        private int _bgWindowTileData;
        private int _bgTileMap;


        //todo: bits for sprite stuff
        private byte LcdControl
        {
            get
            {
                return (byte)(_lcdEnable << 7 |
                            (_windowTileMap << 6) |
                            (_windowDisplayEnable << 5) |
                            (_bgWindowTileData << 4) |
                            (_bgTileMap << 3));
            }
            set
            {
                SetLcdEnable((value >> 7) & 0x01);
                _windowTileMap = (value >> 6) & 0x01;
                _windowDisplayEnable = (value >> 5) & 0x01;
                _bgWindowTileData = (value >> 4) & 0x01;
                _bgTileMap = (value >> 3) & 0x01;
            }
        }

        private int GetPpuState()
        {
            if (_lcdEnable == 1)
                return _ppuContext.GetStateNumber();
            else
                return 0;
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
                            GetPpuState());              
            }

            set
            {
                _coincidenceInterrupt = (value >> 6) & 0x01;
                _oamInterrupt = (value >> 5) & 0x01;
                _vblankInterrupt = (value >> 4) & 0x01;
                _hblankInterrupt = (value >> 3) & 0x01;
            }
        }

        public PPU(IRandomAccessMemory mainMemory, Logger logger)
        {
            _mainMemory = mainMemory;
            _logger = logger;

            _ppuContext = new PPUContext(new Mode2(), this);
            _ppuContext.VideoInterruptOccuredEvent += VideoInterruptOccured;

            _tileMap0 = new byte[32*32];
            _tileMap1 = new byte[32 * 32];
            _tileData = new byte[6144];

            _screenBitmap = new byte[Common.Constants.Video.ScreenWidth * Common.Constants.Video.ScreenHeight];

            LcdControl = 0x91;
        }

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            var addressList = new List<ushort>();

            var videoRegisterAddresses = new ushort[] { VideoRegisters.LcdControl,
                                                         VideoRegisters.LcdStatus,
                                                         VideoRegisters.LineY,
                                                         VideoRegisters.ScrollX,
                                                         VideoRegisters.ScrollY,
                                                         VideoRegisters.LineYCompare};

            var vramAddresses = new ushort[0x9C00 - 0x8000 + 1];
            for (int i = 0; i < vramAddresses.Length; i++)
                vramAddresses[i] = (ushort)(i + 0x8000);

            addressList.AddRange(videoRegisterAddresses);
            addressList.AddRange(vramAddresses);

            return addressList.AsEnumerable();
        }

        public byte DelegateMemoryRead(ushort address)
        {
            if (address >= 0x8000 && address <= 0x97FF)
                return _tileData[address - 0x8000];
            else if (address >= 0x9800 && address <= 0x9BFF)
                return _tileMap0[address - 0x9800];
            else if (address >= 0x9C00 && address <= 0x9FFF)
                return _tileMap1[address - 0x9C00];

            return address switch
            {
                VideoRegisters.LcdStatus => LcdStatus,
                VideoRegisters.LcdControl => LcdControl,
                VideoRegisters.LineY => (byte)_ppuContext.GetLineNumber(),//todo: force 0 when LCD is off
                VideoRegisters.LineYCompare => _lycRegister,
                VideoRegisters.ScrollX => ScrollX,
                VideoRegisters.ScrollY => ScrollY,
                _ => throw new InvalidOperationException($"0x{address:X2} is not a valid ppu address"),
            };
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= 0x8000 && address <= 0x97FF)
            {
                _logger.Log($"Writing tile data 0x{data:X2} at 0x{address:X2}");
                _tileData[address - 0x8000] = data;
            }
            else if (address >= 0x9800 && address <= 0x9BFF)
            {
                _logger.Log($"Writing tile map 0 data 0x{data:X2} at 0x{address:X2}");
                _tileMap0[address - 0x9800] = data;
            }
            else if (address >= 0x9C00 && address <= 0x9FFF)
            {
                _logger.Log($"Writing tile map 1 data 0x{data:X2} at 0x{address:X2}");
                _tileMap1[address - 0x9C00] = data;
            }
            else
            {
                switch (address)
                {
                    case VideoRegisters.LcdControl:
                        LcdControl = data;

                        _logger.Log($"Write LCD Control: 0x{data:X2}");

                        //interrupts?

                        break;
                    case VideoRegisters.LcdStatus:
                        LcdStatus = data;

                        if (ShouldStatIrqBeRaised())
                            RaiseStatInterrupt();
                        break;

                    case VideoRegisters.LineY:
                        //read only
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

        public void VideoInterruptOccured(int interruptType)
        {
            //request vblank interrupt
            var currentIf = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);
            _mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(currentIf | 0x01));

            //todo: request interrupt, but not if IF is executed
        }        

        private int CheckLyCoincidence()
        {
            return _ppuContext.GetLineNumber() == _lycRegister ? 1 : 0;
        }

        public bool AdvanceMachineCycle()
        {
            if (_lcdEnable == 0)
                return false;

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
            //todo: do this for 4 pixel at a time?

            int tileX = x / 8;
            int tileY = y / 8;
            int tileOffsetX = x % 8;
            int tileOffsetY = y % 8;
            
            //get tile index from active tilemap
            byte tileIndex;
            if(_bgTileMap == 0)            
                tileIndex = _tileMap0[tileX + tileY*32];
            else
                tileIndex = _tileMap1[tileX + tileY * 32];

            //get tile data based on selected addressing mode
            byte tileData0, tileData1;
            if (_bgWindowTileData == 0)
            {
                tileData0 = _tileData[0x1000 + (((sbyte)tileIndex) << 4) + tileOffsetY * 2];
                tileData1 = _tileData[0x1000 + (((sbyte)tileIndex) << 4) + tileOffsetY * 2 + 1];
            }
            else
            {
                tileData0 = _tileData[(tileIndex << 4) + tileOffsetY * 2];
                tileData1 = _tileData[(tileIndex << 4) + tileOffsetY * 2 + 1];
            }

            //extract selected pixel
            var msbSet = (tileData0 & (0x80 >> tileOffsetX)) > 0;
            var lsbSet = (tileData1 & (0x80 >> tileOffsetX)) > 0;

            return (msbSet ? 0x2 : 0) | (lsbSet ? 0x1 : 0);
        }

        internal void WritePixel(int shade, int x, int y)
        {
            //todo: handle palette?  

            _screenBitmap[y * 160 + x] = (byte)shade;
        }        
    }
}
