using System;
using System.Collections.Generic;

using BremuGb.Common.Constants;

namespace BremuGb.Memory
{
    public class MainMemory : IRandomAccessMemory
    {
        private Dictionary<ushort, IMemoryAccessDelegate> _writeDelegates = new Dictionary<ushort, IMemoryAccessDelegate>();
        private Dictionary<ushort, IMemoryAccessDelegate> _readDelegates = new Dictionary<ushort, IMemoryAccessDelegate>();

        private Dictionary<ushort, byte> _undelegatedMemory = new Dictionary<ushort, byte>();


        public byte ReadByte(ushort address)
        {
            if (_readDelegates.TryGetValue(address, out IMemoryAccessDelegate readDelegate))
                return readDelegate.DelegateMemoryRead(address);

            switch (address)
            {
                default:
                    if (_undelegatedMemory.TryGetValue(address, out byte data))
                        return data;
                    break;
            }            

            //Console.WriteLine($"Read from uninitialized undelegated memory address 0x{address:X2}");
            return 0x00;
        }

        public void WriteByte(ushort address, byte data)
        {
            //Console.WriteLine($"WriteByte memory address: 0x{address:X4}, data: 0x{data:X2}");

            if (_writeDelegates.TryGetValue(address, out IMemoryAccessDelegate writeDelegate))
            {
                writeDelegate.DelegateMemoryWrite(address, data);
                return;
            }

            switch(address)
            {
                default:
                    //Console.WriteLine($"WriteByte undelegated memory address: 0x{address:X4}, data: 0x{data:X2}");
                    _undelegatedMemory[address] = data;
                    break;
            }            
        }        

        public void RegisterMemoryAccessDelegateWriteRange(ushort addressLo, ushort addressHi, IMemoryAccessDelegate memDelegate)
        {
            if (memDelegate == null)
                throw new ArgumentNullException(nameof(memDelegate));
            if (addressLo > addressHi)
                throw new InvalidOperationException($"Invalid address range: {addressLo:X4} - {addressHi:X4}");

            for (ushort address = addressLo; address <= addressHi; address++)
                RegisterMemoryAccessDelegateWrite(address, memDelegate);
        }

        public void RegisterMemoryAccessDelegateReadRange(ushort addressLo, ushort addressHi, IMemoryAccessDelegate memDelegate)
        {
            if (memDelegate == null)
                throw new ArgumentNullException(nameof(memDelegate));
            if (addressLo > addressHi)
                throw new InvalidOperationException($"Invalid address range: {addressLo:X4} - {addressHi:X4}");

            for (ushort address = addressLo; address <= addressHi; address++)
                RegisterMemoryAccessDelegateRead(address, memDelegate);
        }        

        private void RegisterMemoryAccessDelegateWrite(ushort address, IMemoryAccessDelegate memDelegate)
        {
            if (_writeDelegates.ContainsKey(address))
                throw new InvalidOperationException("Cannot register multiple memory delegates per address");

            _writeDelegates.Add(address, memDelegate);
        }

        private void RegisterMemoryAccessDelegateRead(ushort address, IMemoryAccessDelegate memDelegate)
        {
            if (_readDelegates.ContainsKey(address))
                throw new InvalidOperationException("Cannot register multiple memory delegates per address");

            _readDelegates.Add(address, memDelegate);
        }
    }
}
