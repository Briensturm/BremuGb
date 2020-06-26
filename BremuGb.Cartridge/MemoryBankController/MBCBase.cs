using System;
using System.Linq;
using System.Collections.Generic;

using BremuGb.Memory;

namespace BremuGb.Cartridge.MemoryBankController
{
    abstract class MBCBase : IMemoryBankController, IMemoryAccessDelegate
    {
        protected byte[] _romData;

        protected byte _cartridgeType;
        protected byte _romSize;

        public MBCBase(byte[] romData)
        {
            _romData = romData;

            //check if CGB-only game (not supported for now)
            if (_romData[0x0143] == 0xC0)
                throw new NotSupportedException("CGB-only games are not supported"); 
            
            _cartridgeType = romData[0x0147];
            _romSize = _romData[0x0148];
        }

        virtual public void LoadRam(IRamManager ramManager)
        {
            //mbc-cartridgetype combinations which have battery backed up ram must override this
        }

        virtual public void SaveRam(IRamManager ramManager)
        {
            //mbc-cartridgetype combinations which have battery backed up rum must override this
        }

        abstract public byte DelegateMemoryRead(ushort address);
        abstract public void DelegateMemoryWrite(ushort address, byte data);        

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            var addressList = new List<ushort>();

            var delegatedRomAddresses = new ushort[0x8000];
            for (int i = 0; i < delegatedRomAddresses.Length; i++)
                delegatedRomAddresses[i] = (ushort)i;

            var delegatedRamAddresses = new ushort[0x2000];
            for (int i = 0; i < delegatedRamAddresses.Length; i++)
                delegatedRamAddresses[i] = (ushort)(0xA000 + i);

            addressList.AddRange(delegatedRomAddresses);
            addressList.AddRange(delegatedRamAddresses);

            return addressList.AsEnumerable();
        }
    }
}
