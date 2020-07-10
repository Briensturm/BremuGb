using System;
using System.Collections.Generic;

using BremuGb.Memory;
using BremuGb.Common.Constants;

namespace BremuGb.Serial
{
    //only master mode and internal clock will be supported
    public class SerialController : IMemoryAccessDelegate
    {
        private IRandomAccessMemory _mainMemory;

        private int _serialTimer;
        private const int SerialPeriod = 128;

        private int _transferStartFlag;
        private int _shiftClock;

        private byte _serialTransferData;
        private byte _serialDataToBeTransfered;

        private bool _transferActive;
        private int _shiftCounter;

        private byte SerialTransferData
        {
            get => _serialTransferData;

            set
            {
                _serialTransferData = value;
                _serialDataToBeTransfered = value;
            }
        }

        private byte SerialTransferControl
        {
            get => (byte)(_transferStartFlag << 7 | 
                           0x7E |
                          _shiftClock);

            set
            {
                _transferStartFlag = value >> 7;
                _shiftClock = value & 0x1;

                if(_transferStartFlag == 1)
                    _transferActive = true;
            }
        }

        public event EventHandler<SerialEventArgs> SerialDataTransferedEvent;

        public SerialController(IRandomAccessMemory mainMemory)
        {
            _mainMemory = mainMemory;
        }

        public void AdvanceMachineCycle()
        {
            _serialTimer++;
            if (_serialTimer < SerialPeriod)
                return;

            _serialTimer = 0;

            if (!_transferActive)
                return;

            //shift out one bit
            _serialTransferData = (byte)((_serialTransferData << 1) | 1);
            _shiftCounter++;

            //once all bits are shifted, send the whole byte
            if(_shiftCounter == 8)
            {
                _shiftCounter = 0;
                _transferActive = false;
                _transferStartFlag = 0;

                NotifySerialDataTransfered();

                RequestSerialInterrupt();
            }
        }

        public byte DelegateMemoryRead(ushort address)
        {
            if(address == MiscRegisters.SerialTransferData)
                return SerialTransferData;

            else if(address == MiscRegisters.SerialTransferControl)
                return SerialTransferControl;

            throw new InvalidOperationException($"0x{address:X2} is not a valid serial address");
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address == MiscRegisters.SerialTransferData)
                SerialTransferData = data;

            else if (address == MiscRegisters.SerialTransferControl)
                SerialTransferControl = data;
        }

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            var serialAddresses = new ushort[2] { MiscRegisters.SerialTransferData, 
                                                  MiscRegisters.SerialTransferControl };

            return serialAddresses;
        }

        private void RequestSerialInterrupt()
        {
            var currentIf = _mainMemory.ReadByte(MiscRegisters.InterruptFlags);
            _mainMemory.WriteByte(MiscRegisters.InterruptFlags, (byte)(currentIf | 0x08));
        }

        private void NotifySerialDataTransfered()
        {
            SerialDataTransferedEvent?.Invoke(this, new SerialEventArgs(_serialDataToBeTransfered));
        }
    }
}
