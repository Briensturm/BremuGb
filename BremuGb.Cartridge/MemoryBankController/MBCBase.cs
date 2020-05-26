using System;
using System.Collections.Generic;

using BremuGb.Memory;

namespace BremuGb.Cartridge.MemoryBankController
{
    abstract class MBCBase : IMemoryBankController, IMemoryAccessDelegate
    {
        protected byte[] _romData;

        public MBCBase(byte[] romData)
        {
            _romData = romData;

            //check if CGB-only game (not supported for now)
            if (_romData[0x0143] == 0xC0)
                throw new NotSupportedException("CGB-only games are not supported");            
        }

        abstract public byte DelegateMemoryRead(ushort address);
        abstract public void DelegateMemoryWrite(ushort address, byte data);

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            var _delegatedAddresses = new ushort[0x7FFF + 1];
            for (int i = 0; i < _delegatedAddresses.Length; i++)
                _delegatedAddresses[i] = (ushort)i;

            //TODO: Add cartridge ram addresses

            return _delegatedAddresses as IEnumerable<ushort>;
        }
    }
}
