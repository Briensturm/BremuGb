namespace BremuGb.Video
{
    internal class HBlankState : PixelProcessingUnitStateBase
    {
        private int _dotCounter = 0;

        public HBlankState(PixelProcessingUnitContext context, PixelProcessingUnitStateMachine stateMachine)
            : base(context, stateMachine)
        {
        }

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if(_dotCounter == 208)
            {
                _context.CurrentLine++;

                if (_context.CurrentLine == 144)
                {
                    _stateMachine.TransitionTo<VBlankState>();
                    _context.RequestVBlankInterrupt();
                }
                else
                    _stateMachine.TransitionTo<OamScanState>();      
            }                
        }

        public override int GetModeNumber()
        {
            return Common.Constants.Video.HBlankModeNo;
        }

        public override void Initialize(int clocks)
        {
            _dotCounter = 0;
        }
    }
}
