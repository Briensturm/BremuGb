namespace BremuGb.Common.Constants
{
    static public class CartridgeConstants
    {
        public const ushort FirstRomBankAddressEnd = 0x3FFF;
        public const ushort RomAddressEnd          = 0x7FFF;
        public const ushort CartRamAddressBegin    = 0xA000;
        public const ushort CartRamAddressEnd      = 0xBFFF;

        public const ushort RomBankSize = 0x4000;
        public const ushort RamBankSize = 0x2000;
    }
}
