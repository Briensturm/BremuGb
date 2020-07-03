namespace BremuGb.Common.Constants
{
    public static class Video
    {
        public const int ScreenWidth = 160;
        public const int ScreenHeight = 144;

        public const int LineCount = 154;
        public const int DotsPerLine = 456;

        public const int HBlankModeNo = 0;
        public const int VBlankModeNo = 1;
        public const int OamScanModeNo = 2;
        public const int PixelWritingModeNo = 3;

        public enum Shades
        {
            White      = 0x00,
            LightGrey  = 0x01,
            MediumGrey = 0x10,
            Black      = 0x11,
        }
    }
}
