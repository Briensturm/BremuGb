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

                for (byte i = 0; i<160; i++)
                {
                    _context.PPU.WritePixel(_context.PPU.GetBackgroundPixel((byte)(i + scrollX), (byte)(lineNo + scrollY)), i, lineNo);
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
