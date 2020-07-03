namespace BremuGb.Cartridge
{
    internal enum CartridgeType
    {
        ROM_ONLY                = 0x00,
        MBC1                    = 0x01,
        MBC1_RAM                = 0x02,
        MBC1_RAM_BATTERY        = 0x03,
        MBC2                    = 0x05,
        MBC2_BATTERY            = 0x06,
        ROM_RAM                 = 0x08,
        ROM_RAM_BATTERY         = 0x09,
        MBC3_TIMER_BATTERY      = 0x0F,
        MBC3_TIMER_RAM_BATTERY  = 0x10,
        MBC3                    = 0x11,
        MBC3_RAM                = 0x12,
        MBC3_RAM_BATTERY        = 0x13,
        MBC5                    = 0x19,
        MBC5_RAM                = 0x1A,
        MBC5_RAM_BATTERY        = 0x1B,
        MBC5_RUMBLE             = 0x1C,
        MBC5_RUMBLE_RAM         = 0x1D,
        MBC5_RUMBLE_RAM_BATTERY = 0x1E,
    }
}
