namespace BremuGb.Video
{
    public class Mode3 : PPUStateBase
    {
        private int _dotCounter = 0;

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter == 168)
            {
                //TODO: do not draw whole scanline, implement proper scanline rendering
                byte lineNo = (byte)_context.GetLineNumber();
                var scrollX = _context.PPU.ScrollX;
                var scrollY = _context.PPU.ScrollY;

                var windowX = _context.PPU.WindowX-6;
                var windowY = _context.PPU.WindowY;

                for (byte x = 0; x<160; x++)
                {
                    if(_context.PPU._windowDisplayEnable == 1 && windowX <= x && windowY <= lineNo)
                        _context.PPU.WritePixel(_context.PPU.GetBackgroundPixel((byte)(x + windowX), (byte)(lineNo + windowY), true), x, lineNo);
                    else if(_context.PPU._bgEnable == 1)
                        _context.PPU.WritePixel(_context.PPU.GetBackgroundPixel((byte)(x + scrollX), (byte)(lineNo + scrollY)), x, lineNo);
         
                    //todo: what happens for a pixel if window and bg disabled?
                }

                _context.TransitionTo(new Mode0());
            }
        }

        public override int GetStateNumber()
        {
            return 3;
        }
    }
}
