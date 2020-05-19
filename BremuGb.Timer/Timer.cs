using System;

using BremuGb.Memory;
using BremuGb.Common.Constants;

namespace BremuGb.Timer
{
    public class Timer : IMemoryAccessDelegate
    {
        private ushort _div = 0xABCC;
        private byte _tma;
        private byte _tima;
        private byte _tac;

        IRandomAccessMemory _mainMemory;

        private bool _loadTimaFromTmaCycle;
        private bool _waitCycle;

        private bool TimerEnabled
        {
            get
            {
                return (_tac & 0x04) == 0x04;
            }
        }

        public Timer(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;
        }

        public byte DelegateMemoryRead(ushort address)
        {
            switch(address)
            {
                case TimerRegisters.Divider:
                    return (byte)(_div >> 8);

                case TimerRegisters.Timer:
                    return _tima;

                case TimerRegisters.TimerLoad:
                    return _tma;

                case TimerRegisters.TimerControl:
                    return _tac;

                default:
                    throw new InvalidOperationException($"0x{address:X2} is not a valid timer address");
            }
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            switch (address)
            {
                case TimerRegisters.Divider:
                    Console.WriteLine($"Write timer divider: 0x{data:X2}");
                    var oldDiv = _div;
                    _div = 0;

                    if (TimerEnabled && DivFallingEdgeOccured(oldDiv, 0))
                        IncrementTima();
                    break;

                case TimerRegisters.Timer:
                    Console.WriteLine($"Write timer register: 0x{data:X2}");
                    if (_loadTimaFromTmaCycle)
                        return;

                    _tima = data;
                    _waitCycle = false;
                    break;

                case TimerRegisters.TimerLoad:
                    Console.WriteLine($"Write tma: 0x{data:X2}");

                    _tma = data;
                    break;

                case TimerRegisters.TimerControl:
                    Console.WriteLine($"Write timer control: 0x{data:X2}");

                    var oldMuxOut = TimerEnabled && ((_div >> GetControlBit()) & 0x01) == 0x01;
                    _tac = data;

                    var newMuxOut = TimerEnabled && ((_div >> GetControlBit()) & 0x01) == 0x01;

                    //detect falling edge
                    if (oldMuxOut && !newMuxOut)
                        IncrementTima();
                    break;

                default:
                    throw new InvalidOperationException($"0x{address:X2} is not a valid timer address");
            }
        }

        public void AdvanceMachineCycle()
        {
            //increase div with system clock
            var oldDiv = _div;
            _div += 4;

            if(_waitCycle)
            {
                _waitCycle = false;
                _loadTimaFromTmaCycle = true;
            }
            else if(_loadTimaFromTmaCycle)
            {
                _loadTimaFromTmaCycle = false;
                _tima = _tma;

                var currentIf = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);
                _mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(currentIf | 0x04));

                //todo: request interrupt, but not if IF is executed
            }
            else if (!TimerEnabled)
                return;            

            if (DivFallingEdgeOccured(oldDiv, _div))
            {
                IncrementTima();
            }
        }

        private void IncrementTima()
        { 
            _tima++;

            if (_tima == 0)
                _waitCycle = true;
        }

        private bool DivFallingEdgeOccured(ushort divBefore, ushort divAfter)
        {
            var bit = GetControlBit();

            var bitValueBefore = (divBefore >> bit) & 0x01;
            var bitValueAfter = (divAfter >> bit) & 0x01;

            return bitValueBefore == 1 && bitValueAfter == 0;
        }

        private int GetControlBit()
        {
            var freqSelect = _tac & 0x06;
            int bit;

            switch (freqSelect)
            {
                case 0x00:
                    bit = 9;
                    break;
                case 0x02:
                    bit = 3;
                    break;
                case 0x04:
                    bit = 5;
                    break;
                case 0x06:
                    bit = 7;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid timer control register state 0x{_tac:X2}");
            }

            return bit;
        }
    }
}
