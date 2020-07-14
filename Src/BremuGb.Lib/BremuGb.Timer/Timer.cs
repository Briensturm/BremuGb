using System;
using System.Collections.Generic;

using BremuGb.Memory;
using BremuGb.Common.Constants;

namespace BremuGb
{
    public class Timer : IMemoryAccessDelegate
    {
        private ushort _div = 0xABD0-4;
        private byte _tma;
        private byte _tima;
        private byte _tac;

        private int[] _controlBits = new int[4] { 9, 3, 5, 7 };

        private readonly IRandomAccessMemory _mainMemory;

        private bool _loadTimaFromTmaCycle;
        private bool _waitCycle;

        private bool TimerEnabled => (_tac & 0x04) == 0x04;

        public Timer(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;
        }

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            return new ushort[] { TimerRegisters.Divider,
                                  TimerRegisters.TimerControl,
                                  TimerRegisters.TimerLoad,
                                  TimerRegisters.Timer} as IEnumerable<ushort>;
        }

        public byte DelegateMemoryRead(ushort address)
        {
            return address switch
            {
                TimerRegisters.Divider => (byte)(_div >> 8),
                TimerRegisters.Timer => _tima,
                TimerRegisters.TimerLoad => _tma,
                TimerRegisters.TimerControl => (byte)(_tac | 0xF8),
                _ => throw new InvalidOperationException($"0x{address:X2} is not a valid timer address"),
            };
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            switch (address)
            {
                case TimerRegisters.Divider:
                    var oldDiv = _div;
                    _div = 0;

                    if (TimerEnabled && DivFallingEdgeOccured(oldDiv, 0))
                        IncrementTima();
                    break;

                case TimerRegisters.Timer:
                    if (_loadTimaFromTmaCycle)
                        return;

                    _tima = data;
                    _waitCycle = false;
                    break;

                case TimerRegisters.TimerLoad:
                    _tma = data;
                    break;

                case TimerRegisters.TimerControl:
                    var oldMuxOut = TimerEnabled && ((_div >> _controlBits[_tac & 0x03]) & 0x01) == 0x01;
                    _tac = data;

                    var newMuxOut = TimerEnabled && ((_div >> _controlBits[_tac & 0x03]) & 0x01) == 0x01;

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
            var bit = _controlBits[_tac & 0x03];

            var bitValueBefore = (divBefore >> bit) & 0x01;
            var bitValueAfter = (divAfter >> bit) & 0x01;

            return bitValueBefore == 1 && bitValueAfter == 0;
        }        
    }
}
