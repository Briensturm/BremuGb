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

            

            _ppuContext = new PPUContext(new Mode2());
            _ppuContext.VideoInterruptOccuredEvent += VideoInterruptOccured;
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
            switch(address)
            {
                case VideoRegisters.LcdStatus:
                    return LcdStatus;

                case VideoRegisters.LineY:
                    //todo: force 0 when LCD is off
                    return (byte)_ppuContext.GetLineNumber();

                case VideoRegisters.LineYCompare:
                    return _lycRegister;

                default:
                    throw new InvalidOperationException($"0x{address:X2} is not a valid ppu address");
            }
        }

        public void DelegateMemoryWrite(ushort address, byte data)
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

                default:
                    throw new InvalidOperationException($"0x{address:X2} is not a valid ppu address");
            }
        }

        private int CheckLyCoincidence()
        {
            return _ppuContext.GetLineNumber() == _lycRegister ? 1 : 0;
        }

        public void AdvanceMachineCycle()
        {
            _ppuContext.AdvanceMachineCycle();

            if (ShouldStatIrqBeRaised())
                RaiseStatInterrupt();
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
    }
}
