using BremuGb.Video.Sprites;

namespace BremuGb.Video
{
    internal class OamScanState : PixelProcessingUnitStateBase
    {
        private int _dotCounter;

        public OamScanState(PixelProcessingUnitContext context, PPUStateMachine stateMachine) 
            : base(context, stateMachine)
        {
        }

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter == 80)
            {
                //scan for sprites which intercept the current scanline
                for(int i = 0; i< _context.SpriteTable.Sprites.Length; i++)
                {
                    var sprite = _context.SpriteTable.Sprites[i];

                    if (SpriteInterceptsCurrentScanline(sprite))
                    {
                        sprite.TakeSnapshot();
                        _context._spritesToBeDrawn.Add(sprite);
                    }

                    //do not render more than 10 sprites per line
                    if (_context._spritesToBeDrawn.Count == 10)
                        break;
                }

                //DMG priorities
                //_orderedSprites = _spritesToBeDrawn.OrderBy(s => s.PositionX).ThenBy(s => s.OamIndex);

                _stateMachine.TransitionTo<PixelWritingState>();
            }
        }

        public override int GetModeNumber()
        {
            return Common.Constants.Video.OamScanModeNo;
        }

        public override void Initialize(int clocks)
        {
            _dotCounter = 0;
            _context._spritesToBeDrawn?.Clear();
        }

        private bool SpriteInterceptsCurrentScanline(Sprite sprite)
        {
            var spriteSize = 0;
            if (_context.SpriteSize == 0)
                spriteSize = 8;

            if (sprite.GetPositionY() > 0
               && sprite.GetPositionY() < _context.CurrentLine + 17
               && sprite.GetPositionY() > _context.CurrentLine + spriteSize
               && sprite.GetPositionX() < 168 && sprite.GetPositionX() > 0)
            {
                return true;
            }

            return false;
        }
    }
}
