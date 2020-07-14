using System;
using System.Collections.Generic;

using BremuGb.Common.Constants;
using BremuGb.Memory;

//Todo: implement joypad interrupt

namespace BremuGb.Input
{
    public class Joypad : IMemoryAccessDelegate
    {
        private JoypadState _joypadState;
        private int _activeKeys = 0b00;

        private byte JoypadRegister
        {
            get
            {
                byte joypadRegister = (byte)((_activeKeys << 4) | 0xCF);

                if(_activeKeys == 0b11)
                {
                    return 0xFF;
                }
                if (_activeKeys == 0b10 || _activeKeys == 0b00)
                {
                    //return direction keys
                    if (_joypadState.HasFlag(JoypadState.Up))
                        joypadRegister &= 0xFB;
                    if (_joypadState.HasFlag(JoypadState.Down))
                        joypadRegister &= 0xF7;
                    if (_joypadState.HasFlag(JoypadState.Left))
                        joypadRegister &= 0xFD;
                    if (_joypadState.HasFlag(JoypadState.Right))
                        joypadRegister &= 0xFE;
                }
                if(_activeKeys == 0b01 || _activeKeys == 0b00)
                {
                    //return button keys
                    if (_joypadState.HasFlag(JoypadState.Select))
                        joypadRegister &= 0xFB;
                    if (_joypadState.HasFlag(JoypadState.Start))
                        joypadRegister &= 0xF7;
                    if (_joypadState.HasFlag(JoypadState.B))
                        joypadRegister &= 0xFD;
                    if (_joypadState.HasFlag(JoypadState.A))
                        joypadRegister &= 0xFE;
                }

                return joypadRegister;
            }

            set
            {
                _activeKeys = (value >> 4) & 0b11;
            }
        }

        public byte DelegateMemoryRead(ushort address)
        {
            if (address == MiscRegisters.Joypad)
                return JoypadRegister;

            throw new InvalidOperationException($"0x{address:X2} is not a valid joypad address");
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address == MiscRegisters.Joypad)
                JoypadRegister = data;
            else
                throw new InvalidOperationException($"0x{address:X2} is not a valid joypad address");
        }

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            return new ushort[1] { MiscRegisters.Joypad };
        }

        public void SetJoypadState(JoypadState joypadState)
        {
            _joypadState = joypadState;
        }
    }
}
