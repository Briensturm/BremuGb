namespace BremuGb.Video.Sprites
{
    internal class Sprite
    {
        internal byte PositionX { get; set; }
        internal byte PositionY { get; set; }

        internal byte TileNumber { get; set; }

        private byte _flags;
        internal byte Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                _flags = value;

                PaletteNumber = (value >> 4) & 0x01;

                FlipX = (value >> 5) & 0x01;
                FlipY = (value >> 6) & 0x01;

                BgPriority = (value >> 7) & 0x01;
            }
        }

        internal int PaletteNumber { get; set; }

        internal int FlipX { get; set; }
        internal int FlipY { get; set; }

        internal int BgPriority { get; set; }

        internal int OamIndex { get; set; }
    }
}
