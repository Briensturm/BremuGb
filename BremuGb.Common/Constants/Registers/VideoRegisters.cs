namespace BremuGb.Common.Constants
{
    static public class VideoRegisters
    {
        public const ushort LcdControl = 0xFF40;
        public const ushort LcdStatus  = 0xFF41;

        public const ushort ScrollY      = 0xFF42;
        public const ushort ScrollX      = 0xFF43;
        public const ushort LineY        = 0xFF44;
        public const ushort LineYCompare = 0xFF45;
        public const ushort WindowY      = 0xFF4A;
        public const ushort WindowX      = 0xFF4B;

        public const ushort DmaTransfer = 0xFF46;

        public const ushort BackgroundPalette  = 0xFF47;
        public const ushort ObjectPalette0 = 0xFF48;
        public const ushort ObjectPalette1 = 0xFF49;        
    }
}