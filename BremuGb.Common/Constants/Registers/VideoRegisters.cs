namespace BremuGb.Common.Constants
{
    static public class VideoRegisters
    {
        public const ushort c_LcdControl = 0xFF40;
        public const ushort c_LcdStatus  = 0xFF41;

        public const ushort c_ScrollY      = 0xFF42;
        public const ushort c_ScrollX      = 0xFF43;
        public const ushort c_LineY        = 0xFF44;
        public const ushort c_LineYCompare = 0xFF45;
        public const ushort c_WindowY      = 0xFF4A;
        public const ushort c_WindowX      = 0xFF4B;

        public const ushort c_DmaTransfer = 0xFF46;

        public const ushort c_BackgroundPalette  = 0xFF47;
        public const ushort c_ObjectPaletteData0 = 0xFF48;
        public const ushort c_ObjectPaletteData1 = 0xFF49;        
    }
}