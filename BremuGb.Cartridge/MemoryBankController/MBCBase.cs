using System;
using System.Linq;
using System.Collections.Generic;

using BremuGb.Memory;

namespace BremuGb.Cartridge.MemoryBankController
{
    abstract class MBCBase : IMemoryBankController, IMemoryAccessDelegate
    {
        protected byte[] _romData;
        protected byte[] _ramData;        

        protected RomSizeType _romSizeType;
        protected RamSizeType _ramSizeType;

        protected CartridgeType _cartridgeType;

        public MBCBase(byte[] romData)
        {
            _romData = romData;

            //check if CGB-only game (not supported for now)
            if (_romData[0x0143] == 0xC0)
                throw new NotSupportedException("CGB-only games are not supported"); 
            
            _cartridgeType = (CartridgeType)romData[0x0147];

            _romSizeType = (RomSizeType)_romData[0x0148];
            _ramSizeType = (RamSizeType)_romData[0x0149];

            _ramData = new byte[RamSizeInBytes];
        }

        public void LoadRam(IRamManager ramManager)
        {
            if (!CartridgeCanSave)
                return;

            var data = ramManager.TryLoadRam();

            if (data != null)
                _ramData = data;
        }

        public void SaveRam(IRamManager ramManager)
        {
            if (CartridgeCanSave)
                ramManager.SaveRam(_ramData);
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

        protected abstract bool CartridgeCanSave { get; }

        protected int RamSizeInBytes
        {
            get
            {
                return _ramSizeType switch
                {
                    RamSizeType.Ram_2KB   => 0x800,
                    RamSizeType.Ram_8KB   => 0x2000,
                    RamSizeType.Ram_32KB  => 0x8000,
                    RamSizeType.Ram_64KB  => 0x10000,
                    RamSizeType.Ram_128KB => 0x20000,

                    RamSizeType.None => 0,
                    _ => 0,
                };
            }
        }
    }
}
