using System;
using System.Collections.Generic;
using System.Linq;

using BremuGb.Video.Sprites;

namespace BremuGb.Video
{
    internal class PixelWritingState : PixelProcessingUnitStateBase
    {
        private int _dotCounter = 0;
        private List<Sprite> _spritesToBeDrawn;
        private IOrderedEnumerable<Sprite> _orderedSprites;

        private int _lastBackgroundPixel = 0;

        public PixelWritingState(PixelProcessingUnitContext context, PPUStateMachine stateMachine) 
            : base(context, stateMachine)
        {
            _spritesToBeDrawn = new List<Sprite>();
        }

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter == 168)
            {
                //TODO: do not draw whole scanline, implement proper scanline rendering
                byte lineNo = (byte)_context.CurrentLine;
                var scrollX = _context.ScrollX;
                var scrollY = _context.ScrollY;

                var windowX = _context.WindowX-7;
                var windowY = _context.WindowY;

                foreach (Sprite sprite in _context.SpriteTable.Sprites)
                {
                    if (SpriteInterceptsCurrentScanline(sprite))
                        _spritesToBeDrawn.Add(sprite);

                    //do not render more than 10 sprites per line
                    if (_spritesToBeDrawn.Count == 10)
                        break;
                }

                //sort sprites by priorities
                _orderedSprites = _spritesToBeDrawn.OrderBy(s => s.PositionX).ThenBy(s => s.OamIndex);

                for (byte x = 0; x<160; x++)
                {
                    //render bg and window
                    if (_context.WindowEnable == 1 && windowX <= x && windowY <= lineNo)
                        WritePixel(GetBackgroundPixel((byte)(x - windowX), (byte)(lineNo - windowY), true), _context.BackgroundPalette, x, lineNo);
                    else if (_context.BackgroundEnable == 1)
                        WritePixel(GetBackgroundPixel((byte)(x + scrollX), (byte)(lineNo + scrollY)), _context.BackgroundPalette, x, lineNo);
                    else
                    {
                        //todo: what happens for a pixel if window and bg disabled?

                        _lastBackgroundPixel = 0;                        
                    }

                    //render the sprites
                    if (_spritesToBeDrawn.Count > 0)
                    {              
                        WriteSpritePixel(x);                        
                    }
                }   

                _stateMachine.TransitionTo<HBlankState>();
            }
        }

        private void WriteSpritePixel(int x)
        {
            foreach(Sprite sprite in _orderedSprites)
            {
                if (!SpriteInterceptsXPosition(sprite, x))
                    continue;

                var tileNumber = 0;

                var tileX = x + 8 - sprite.PositionX;
                var tileY = _context.CurrentLine + 16 - sprite.PositionY;

                if (_context.SpriteSize == 1)
                {
                    if (tileY > 7 && sprite.FlipY == 0)
                    {
                        tileY -= 8;
                        tileNumber = sprite.TileNumber | 0x01;
                    }
                    else if (tileY <= 7 && sprite.FlipY == 0)
                    {
                        tileNumber = sprite.TileNumber & 0xFE;
                    }
                    else if (tileY > 7 && sprite.FlipY == 1)
                    {
                        tileY = 15 - tileY;
                        tileNumber = sprite.TileNumber & 0xFE;
                    }
                    else if (tileY <= 7 && sprite.FlipY == 1)
                    {
                        tileNumber = sprite.TileNumber | 0x01;
                        tileY = 7 - tileY;
                    }
                }
                else
                {
                    tileNumber = sprite.TileNumber;

                    if (sprite.FlipY == 1)
                        tileY = 7 - tileY;
                }

                if (sprite.FlipX == 1)
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
                    var pixelBehindBg = sprite.BgPriority == 1 && (
                        _lastBackgroundPixel == 1 || _lastBackgroundPixel == 2 || _lastBackgroundPixel == 3);

                    if (!pixelBehindBg)
                    {
                        byte palette;
                        if (sprite.PaletteNumber == 1)
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
            return sprite.PositionX > x && sprite.PositionX < x + 9;
        }

        private bool SpriteInterceptsCurrentScanline(Sprite sprite)
        {
            var spriteSize = 0;
            if (_context.SpriteSize == 0)
                spriteSize = 8;

            if (sprite.PositionY > 0
               && sprite.PositionY < _context.CurrentLine + 17
               && sprite.PositionY > _context.CurrentLine + spriteSize
               && sprite.PositionX < 168 && sprite.PositionX > 0)
            {
                return true;
            }

            return false;
        }

        public override int GetModeNumber()
        {
            return Common.Constants.Video.PixelWritingModeNo;
        }

        public override void Initialize(int clocks)
        {
            _dotCounter = 0;
            _spritesToBeDrawn.Clear();
        }

        private int GetBackgroundPixel(byte x, byte y, bool window = false)
        {
            //todo: do this for 4 pixels at a time?

            int tileX = x / 8;
            int tileY = y / 8;
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
            byte r, g, b;

            if (color == 3)
            {
                r = 8;
                g = 41;
                b = 85;
            }
            else if (color == 2)
            {
                r = 43;
                g = 111;
                b = 95;
            }
            else if (color == 1)
            {
                r = 121;
                g = 170;
                b = 109;
            }
            else if (color == 0)
            {
                r = 175;
                g = 203;
                b = 70;
            }
            else
                throw new InvalidOperationException("invalid shade: " + shade);

            _context.WriteToScreenBitmap(r, y * 160 * 3 + x * 3);
            _context.WriteToScreenBitmap(g, y * 160 * 3 + x * 3 + 1);
            _context.WriteToScreenBitmap(b, y * 160 * 3 + x * 3 + 2);
        }
    }
}
