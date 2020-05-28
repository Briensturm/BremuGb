using System;

namespace BremuGb.Video
{
    internal class PixelWritingState : PixelProcessingUnitStateBase
    {
        private int _dotCounter = 0;

        public PixelWritingState(PixelProcessingUnitContext context, PixelProcessingUnitStateMachine stateMachine) 
            : base(context, stateMachine)
        {
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

                var windowX = _context.WindowX-6;
                var windowY = _context.WindowY;

                for (byte x = 0; x<160; x++)
                {
                    if(_context.WindowEnable == 1 && windowX <= x && windowY <= lineNo)
                        WritePixel(GetBackgroundPixel((byte)(x + windowX), (byte)(lineNo + windowY), true), x, lineNo);
                    else if(_context.BackgroundEnable == 1)
                        WritePixel(GetBackgroundPixel((byte)(x + scrollX), (byte)(lineNo + scrollY)), x, lineNo);
         
                    //todo: what happens for a pixel if window and bg disabled?
                }

                _stateMachine.TransitionTo<HBlankState>();
            }
        }

        public override int GetModeNumber()
        {
            return Common.Constants.Video.PixelWritingModeNo;
        }

        public override void Initialize(int clocks)
        {
            _dotCounter = 0;
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

            return (msbSet ? 0x2 : 0) | (lsbSet ? 0x1 : 0);
        }

        internal void WritePixel(int shade, int x, int y)
        {
            var color = (_context.BackgroundPalette >> shade * 2) & 0b11;
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

            _context.ScreenBitmap[y * 160 * 3 + x * 3] = r;
            _context.ScreenBitmap[y * 160 * 3 + x * 3 + 1] = g;
            _context.ScreenBitmap[y * 160 * 3 + x * 3 + 2] = b;
        }
    }
}
