using System;

namespace BremuGb.Cartridge.MemoryBankController
{
    public static class MBCFactory
    {
        public static IMemoryBankController CreateMBC(IRomLoader romLoader)
        {
            var romData = romLoader.LoadRom();
            var cartridgeType = (CartridgeType)romData[0x0147];

            switch (cartridgeType)
            {
                case CartridgeType.ROM_ONLY:
                case CartridgeType.ROM_RAM:
                case CartridgeType.ROM_RAM_BATTERY:
                    return new MBC0(romData);

                case CartridgeType.MBC1:
                case CartridgeType.MBC1_RAM:
                case CartridgeType.MBC1_RAM_BATTERY:
                    return new MBC1(romData);

                case CartridgeType.MBC2:
                case CartridgeType.MBC2_BATTERY:
                    return new MBC2(romData);

                case CartridgeType.MBC3_TIMER_BATTERY:
                case CartridgeType.MBC3_TIMER_RAM_BATTERY:
                case CartridgeType.MBC3:
                case CartridgeType.MBC3_RAM:
                case CartridgeType.MBC3_RAM_BATTERY:
                    return new MBC3(romData);

                case CartridgeType.MBC5:
                case CartridgeType.MBC5_RAM:
                case CartridgeType.MBC5_RAM_BATTERY:
                case CartridgeType.MBC5_RUMBLE:
                case CartridgeType.MBC5_RUMBLE_RAM:
                case CartridgeType.MBC5_RUMBLE_RAM_BATTERY:
                    return new MBC5(romData);

                default:
                    throw new NotSupportedException($"Cartridge type 0x{cartridgeType:X2} is not supported");
            }
        }
    }
}
