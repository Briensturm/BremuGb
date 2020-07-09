namespace BremuGb.Video.Sprites
{
    internal class Sprite
    {
        private byte _positionX;
        private byte _positionXSnapshot;

        private byte _positionY;
        private byte _positionYSnapshot;

        private byte _tileNumber;
        private byte _tileNumberSnapshot;

        private byte _paletteNumber;
        private byte _paletteNumberSnapshot;

        private byte _flipX;
        private byte _flipXSnapshot;

        private byte _flipY;
        private byte _flipYSnapshot;

        private byte _bgPriority;
        private byte _bgPrioritySnapshot;

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

                _paletteNumber = (byte)((value >> 4) & 0x01);

                _flipX = (byte)((value >> 5) & 0x01);
                _flipY = (byte)((value >> 6) & 0x01);

                _bgPriority = (byte)((value >> 7) & 0x01);
            }
        }

        internal int OamIndex { get; set; }

        internal byte GetPositionX(bool fromSnapshot = false)
        {
            if (fromSnapshot)
                return _positionXSnapshot;

            return _positionX;
        }

        internal void SetPositionX(byte positionX)
        {
            _positionX = positionX;
        }        

        internal byte GetPositionY(bool fromSnapshot = false)
        {
            if (fromSnapshot)
                return _positionYSnapshot;

            return _positionY;
        }

        internal void SetPositionY(byte positionY)
        {
            _positionY = positionY;
        }

        internal void SetTileNumber(byte tileNumber)
        {
            _tileNumber = tileNumber;
        }

        internal byte GetTileNumber(bool fromSnapshot = false)
        {
            if (fromSnapshot)
                return _tileNumberSnapshot;

            return _tileNumber;
        }        

        internal void SetPaletteNumber(byte paletteNumber)
        {
            _paletteNumber = paletteNumber;
        }

        internal byte GetPaletteNumber(bool fromSnapshot = false)
        {
            if (fromSnapshot)
                return _paletteNumberSnapshot;

            return _paletteNumber;
        }

        internal void SetFlipX(byte flipX)
        {
            _flipX = flipX;
        }

        internal byte GetFlipX(bool fromSnapshot = false)
        {
            if (fromSnapshot)
                return _flipXSnapshot;

            return _flipX;
        }

        internal void SetFlipY(byte flipY)
        {
            _flipY = flipY;
        }

        internal byte GetFlipY(bool fromSnapshot = false)
        {
            if (fromSnapshot)
                return _flipYSnapshot;

            return _flipY;
        }

        internal void SetBgPriority(byte bgPriority)
        {
            _bgPriority = bgPriority;
        }

        internal byte GetBgPriority(bool fromSnapshot = false)
        {
            if (fromSnapshot)
                return _bgPrioritySnapshot;

            return _bgPriority;
        }

        internal void TakeSnapshot()
        {
            _positionXSnapshot = _positionX;
            _positionYSnapshot = _positionY;

            _tileNumberSnapshot = _tileNumber;
            _paletteNumberSnapshot = _paletteNumber;
            _bgPrioritySnapshot = _bgPriority;

            _flipXSnapshot = _flipX;
            _flipYSnapshot = _flipY;
        }
    }
}
