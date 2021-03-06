﻿using System;
using System.Collections.Generic;

using BremuGb.Memory;
using BremuGb.Common.Constants;
using BremuGb.Video.Sprites;

namespace BremuGb.Video
{
    internal class PixelProcessingUnitContext
    {
        private IRandomAccessMemory _mainMemory;

        public event EventHandler NextFrameReadyEvent;

        private PPUStateMachine _stateMachine;

        internal SpriteTable SpriteTable;
        internal List<Sprite> _spritesToBeDrawn;

        private int _currentLine;

        private int _coincidenceInterrupt;
        private int _oamInterrupt;
        private int _vblankInterrupt;
        private int _hblankInterrupt;

        private bool _statSignal;

        private bool _buffersSwapped = false;

        //when setting lycreg:
        // if (ShouldStatIrqBeRaised())
        //                  RaiseStatInterrupt();
        internal byte LycRegister;

        internal byte[] ScreenBitmap
        {
            get 
            {
                if (_buffersSwapped)
                    return _screenBuffer1;

                return _screenBuffer0;
            }
        }

        internal void WriteToScreenBitmap(byte data, int index)
        {
            if (_buffersSwapped)
                _screenBuffer0[index] = data;
            else
                _screenBuffer1[index] = data;
        }

        private byte[] _screenBuffer0;
        private byte[] _screenBuffer1;

        internal byte ScrollX { get; set; }
        internal byte ScrollY { get; set; }
        internal byte WindowX { get; set; }
        internal byte WindowY { get; set; }

        public byte ObjectPalette0 { get; internal set; }
        public byte ObjectPalette1 { get; internal set; }

        internal readonly byte[] TileData;
        internal readonly byte[] FirstTileMap;
        internal readonly byte[] SecondTileMap;

        internal int LcdEnable;

        private void SetLcdEnable(int value)
        {
            LcdEnable = value;

            if (value == 0)
            {
                CurrentLine = 0;
                _stateMachine.TransitionTo<OamScanState>();

                for (int i = 0; i < ScreenBitmap.Length / 3; i++)
                {
                    WriteToScreenBitmap(175, i * 3);
                    WriteToScreenBitmap(203, i * 3 +1);
                    WriteToScreenBitmap(70, i * 3 +2);
                }

                SwapBuffers();
                NotifyNextFrameReady();                
            }
        }
        internal int WindowTileMap { get; set; }
        internal int WindowEnable { get; set; }
        internal int _bgWindowTileData;
        internal int BackgroundTileMap { get; set; }
        internal int SpriteSize { get; set; }
        internal int SpriteEnable { get; set; }
        internal int BackgroundEnable { get; set; }

        internal byte BackgroundPalette = 0xFC;

        internal byte LcdStatus
        {
            get
            {
                return (byte)(0x80 |
                            (_coincidenceInterrupt << 6) |
                            (_oamInterrupt << 5) |
                            (_vblankInterrupt << 4) |
                            (_hblankInterrupt << 3) |
                            (CheckLyCoincidence() << 2) |
                            _stateMachine.GetModeNumber());
            }

            set
            {
                _coincidenceInterrupt = (value >> 6) & 0x01;
                _oamInterrupt = (value >> 5) & 0x01;
                _vblankInterrupt = (value >> 4) & 0x01;
                _hblankInterrupt = (value >> 3) & 0x01;

                if (ShouldStatIrqBeRaised())
                    RaiseStatInterrupt();
            }
        }

        internal byte LcdControl
        {
            get
            {
                return (byte)(LcdEnable << 7 |
                            (WindowTileMap << 6) |
                            (WindowEnable << 5) |
                            (_bgWindowTileData << 4) |
                            (BackgroundTileMap << 3) |
                            (SpriteSize << 2) |
                            (SpriteEnable << 1) |
                            (BackgroundEnable << 0));
            }
            set
            {
                SetLcdEnable((value >> 7) & 0x01);
                WindowTileMap = (value >> 6) & 0x01;
                WindowEnable = (value >> 5) & 0x01;
                _bgWindowTileData = (value >> 4) & 0x01;
                BackgroundTileMap = (value >> 3) & 0x01;
                SpriteSize = (value >> 2) & 0x01;
                SpriteEnable = (value >> 1) & 0x01;
                BackgroundEnable = (value >> 0) & 0x01;
            }
        }

        internal PixelProcessingUnitContext(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;

            _stateMachine = new PPUStateMachine(this);

            SpriteTable = new SpriteTable();
            _spritesToBeDrawn = new List<Sprite>();

            FirstTileMap = new byte[32 * 32];
            SecondTileMap = new byte[32 * 32];
            TileData = new byte[6144];

            _screenBuffer0 = new byte[Common.Constants.Video.ScreenWidth * Common.Constants.Video.ScreenHeight * 3];
            _screenBuffer1 = new byte[Common.Constants.Video.ScreenWidth * Common.Constants.Video.ScreenHeight * 3];

            LcdControl = 0x91;
        }

        private int CheckLyCoincidence()
        {
            return CurrentLine == LycRegister ? 1 : 0;
        }

        private void NotifyNextFrameReady()
        {
            NextFrameReadyEvent?.Invoke(this, null);
        }

        internal void RequestVBlankInterrupt()
        {
            var currentIf = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);
            _mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(currentIf | 0x01));

            NotifyNextFrameReady();

            SwapBuffers();
        }

        internal int CurrentLine
        {
            get
            {
                if (LcdEnable == 1)
                    return _currentLine;
                else
                    return 0;
            }
            set
            {
                _currentLine = value;
            }
        }       

        internal void AdvanceMachineCycle()
        {
            _stateMachine.AdvanceMachineCycle();

            if (ShouldStatIrqBeRaised())
                RaiseStatInterrupt();
        }
        
        private bool GetStatSignal()
        {
            if (LcdEnable == 0)
                return false;

            return ((CheckLyCoincidence() & _coincidenceInterrupt) == 0x01) ||
                (_stateMachine.GetModeNumber() == 0 && _hblankInterrupt == 1) ||
                (_stateMachine.GetModeNumber() == 2 && _oamInterrupt == 1) ||
                (_stateMachine.GetModeNumber() == 1 && (_vblankInterrupt == 1 || _oamInterrupt == 1));
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

        private void RaiseStatInterrupt()
        {
            var currentIf = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);
            _mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(currentIf | 0x02));
        }
    
        private void SwapBuffers()
        {
            _buffersSwapped = !_buffersSwapped;
        }
    }
}
