namespace BremuGb.Common.Constants
{
    public static class MiscRegisters
    {
        public const ushort InterruptEnable = 0xFFFF;
        public const ushort InterruptFlags = 0xFF0F;

        public const ushort SerialTransferData = 0xFF01;
        public const ushort SerialTransferControl = 0xFF02;

        public const ushort Joypad = 0xFF00;
    }
}
