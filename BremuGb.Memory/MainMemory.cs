using System;
using System.Collections.Generic;

using BremuGb.Common.Constants;

namespace BremuGb.Memory
{
    public class MainMemory : IRandomAccessMemory
    {
        private readonly Dictionary<ushort, IMemoryAccessDelegate> _memoryDelegates = new Dictionary<ushort, IMemoryAccessDelegate>();
        private readonly Dictionary<ushort, byte> _undelegatedMemory = new Dictionary<ushort, byte>();

        //todo: create memory delegate for interrupt registers (e.g. in cpu project)
        private byte _interruptFlags;
        private byte _interruptEnable;

        private readonly byte _key1 = 0xFF;

        private byte InterruptFlags
        {
            get
            {
                return (byte)(0xE0 | _interruptFlags);
            }
            set 
            {
                _interruptFlags = value;
            }
        }

        public byte ReadByte(ushort address)
        {
            if (_memoryDelegates.TryGetValue(address, out IMemoryAccessDelegate memoryDelegate))
                return memoryDelegate.DelegateMemoryRead(address);

            switch (address)
            {
                case MiscRegisters.InterruptEnable:
                    return _interruptEnable;

                case MiscRegisters.InterruptFlags:
                    return InterruptFlags;

                case 0xFF4D:
                    return _key1;

                default:
                    if (_undelegatedMemory.TryGetValue(address, out byte data))
                        return data;
                    break;
            }            
            
            return 0x00;
        }

        public void WriteByte(ushort address, byte data)
        {
            if (_memoryDelegates.TryGetValue(address, out IMemoryAccessDelegate memoryDelegate))
            {
                memoryDelegate.DelegateMemoryWrite(address, data);
                return;
            }

            switch(address)
            {
                case MiscRegisters.InterruptEnable:
                    _interruptEnable = data;
                    break;

                case MiscRegisters.InterruptFlags:
                    InterruptFlags = data;
                    break;

                case 0xFF4D:
                    break;

                default:
                    _undelegatedMemory[address] = data;
                    break;
            }            
        }

        public void RegisterMemoryAccessDelegate(IMemoryAccessDelegate memoryDelegate)
        {
            var delegatedAddresses = memoryDelegate.GetDelegatedAddresses();

            foreach (var delegatedAddress in delegatedAddresses)
            {
                if (_memoryDelegates.ContainsKey(delegatedAddress))
                    throw new InvalidOperationException("Cannot register multiple memory delegates per address");

                _memoryDelegates.Add(delegatedAddress, memoryDelegate);
            }
        }          
    }
}
