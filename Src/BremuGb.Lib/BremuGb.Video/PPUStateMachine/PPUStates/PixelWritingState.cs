using BremuGb.Video.Sprites;

namespace BremuGb.Video
{
    internal class PixelWritingState : PixelProcessingUnitStateBase
    {
        private int _dotCounter;       

        private int _lastBackgroundPixel;

        private byte[] _redValues   = new byte[] { 175, 121, 43,  8  };
        private byte[] _greenValues = new byte[] { 203, 170, 111, 41 };
        private byte[] _blueValues  = new byte[] { 70,  109, 95,  85 };

        private byte _lineNo;
        private byte _yPosWindow;
        private byte _yPosBg;
        private int _scrollX;
        private int _scrollY;
        private int _windowX;
        private int _windowY;

        private int _x;

        public PixelWritingState(PixelProcessingUnitContext context, PPUStateMachine stateMachine) 
            : base(context, stateMachine)
        {
        }

        private void RenderStuff()
        {
            //render bg and window
            if (_context.WindowEnable == 1 && _windowX <= _x && _windowY <= _lineNo)
                WritePixel(GetBackgroundPixel((byte)(_x - _windowX), _yPosWindow, true), _context.BackgroundPalette, _x, _lineNo);
            else if (_context.BackgroundEnable == 1)
                WritePixel(GetBackgroundPixel((byte)(_x + _scrollX), _yPosBg), _context.BackgroundPalette, _x, _lineNo);
            else
            {
                //todo: what happens for a pixel if window and bg disabled?

                _lastBackgroundPixel = 0;
            }

            //render the sprites
            if (_context._spritesToBeDrawn.Count > 0)
            {
                WriteSpritePixel(_x);
            }
        }

        public override void AdvanceMachineCycle()
        {
            //TODO: Line 0 is different, see kirby 2

            if (_dotCounter > 4 && _x < 160)
            {
                RenderStuff();
                _x++;
                RenderStuff();
                _x++;
                RenderStuff();
                _x++;
                RenderStuff();
                _x++;
            }

            _dotCounter += 4;

            if (_dotCounter == 168)
            {   
                _stateMachine.TransitionTo<HBlankState>();
            }
        }

        private void WriteSpritePixel(int x)
        {
            for(int i = 0; i<_context._spritesToBeDrawn.Count; i++)
            {
                var sprite = _context._spritesToBeDrawn[i];

                if (!SpriteInterceptsXPosition(sprite, x))
                    continue;

                var tileNumber = 0;

                var tileX = x + 8 - sprite.GetPositionX(true);
                var tileY = _context.CurrentLine + 16 - sprite.GetPositionY(true);

                if (_context.SpriteSize == 1)
                {
                    if (tileY > 7 && sprite.GetFlipY(true) == 0)
                    {
                        tileY -= 8;
                        tileNumber = sprite.GetTileNumber(true) | 0x01;
                    }
                    else if (tileY <= 7 && sprite.GetFlipY(true) == 0)
                    {
                        tileNumber = sprite.GetTileNumber(true) & 0xFE;
                    }
                    else if (tileY > 7 && sprite.GetFlipY(true) == 1)
                    {
                        tileY = 15 - tileY;
                        tileNumber = sprite.GetTileNumber(true) & 0xFE;
                    }
                    else if (tileY <= 7 && sprite.GetFlipY(true) == 1)
                    {
                        tileNumber = sprite.GetTileNumber(true) | 0x01;
                        tileY = 7 - tileY;
                    }
                }
                else
                {
                    tileNumber = sprite.GetTileNumber(true);

                    if (sprite.GetFlipY(true) == 1)
                        tileY = 7 - tileY;
                }

                if (sprite.GetFlipX(true) == 1)
                    tileX = 7 - tileX;

                var tileData0 = _context.TileData[(tileNumber << 4) + tileY * 2];
                var tileData1 = _context.TileData[(tileNumber << 4) + tileY * 2 + 1];


                //extract selected pixel
                var lsbSet = (tileData0 & (0x80 >> tileX)) > 0;
                var msbSet = (tileData1 & (0x80 >> tileX)) > 0;

                var shade = (msbSet ? 0x2 : 0) | (lsbSet ? 0x1 : 0);                

                //transparent
                if (shade != 0)
                {
                    //check bg priority
                    var pixelBehindBg = sprite.GetBgPriority(true) == 1 && (
                        _lastBackgroundPixel == 1 || _lastBackgroundPixel == 2 || _lastBackgroundPixel == 3);

                    if (!pixelBehindBg)
                    {
                        byte palette;
                        if (sprite.GetPaletteNumber(true) == 1)
                            palette = _context.ObjectPalette1;
                        else
                            palette = _context.ObjectPalette0;

                        WritePixel(shade, palette, x, _context.CurrentLine);
                    }

                    break;
                }                    
            }           
        }

        private bool SpriteInterceptsXPosition(Sprite sprite, int x)
        {
            return sprite.GetPositionX(true) > x && sprite.GetPositionX(true) < x + 9;
        }        

        public override int GetModeNumber()
        {
            return Common.Constants.Video.PixelWritingModeNo;
        }        

        public override void Initialize(int clocks)
        {
            _dotCounter = 0;

            _x = 0;

            _lineNo = (byte)_context.CurrentLine;
            _scrollX = _context.ScrollX;
            _scrollY = _context.ScrollY;

            _windowX = _context.WindowX - 7;
            _windowY = _context.WindowY;            
 

            _yPosWindow = (byte)(_lineNo - _windowY);
            _yPosBg = (byte)(_lineNo + _scrollY);
        }

        private int GetBackgroundPixel(byte x, byte y, bool window = false)
        {
            int tileX = x >> 3;
            int tileY = y >> 3;
            int tileOffsetX = x % 8;
            int tileOffsetY = y % 8;

            //get tile index from active tilemap
            byte tileIndex;
            if ((window ? _context.WindowTileMap : _context.BackgroundTileMap) == 0)
                tileIndex = _context.FirstTileMap[tileX + tileY * 32];
            else
                tileIndex = _context.SecondTileMap[tileX + tileY * 32];

            //get tile data based on selected addressing mode
            byte tileData0, tileData1;
            if (_context._bgWindowTileData == 0)
            {
                tileData0 = _context.TileData[0x1000 + (((sbyte)tileIndex) << 4) + tileOffsetY * 2];
                tileData1 = _context.TileData[0x1000 + (((sbyte)tileIndex) << 4) + tileOffsetY * 2 + 1];
            }
            else
            {
                tileData0 = _context.TileData[(tileIndex << 4) + tileOffsetY * 2];
                tileData1 = _context.TileData[(tileIndex << 4) + tileOffsetY * 2 + 1];
            }

            //extract selected pixel
            var lsbSet = (tileData0 & (0x80 >> tileOffsetX)) > 0;
            var msbSet = (tileData1 & (0x80 >> tileOffsetX)) > 0;

            _lastBackgroundPixel = (msbSet ? 0x2 : 0) | (lsbSet ? 0x1 : 0);
            return _lastBackgroundPixel;
        }

        internal void WritePixel(int shade, byte palette, int x, int y)
        {
            var color = (palette >> shade * 2) & 0b11;            

            _context.WriteToScreenBitmap(_redValues[color], y * 160 * 3 + x * 3);
            _context.WriteToScreenBitmap(_greenValues[color], y * 160 * 3 + x * 3 + 1);
            _context.WriteToScreenBitmap(_blueValues[color], y * 160 * 3 + x * 3 + 2);
        }
    }
}
