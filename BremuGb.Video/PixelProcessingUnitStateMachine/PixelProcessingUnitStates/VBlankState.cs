namespace BremuGb.Video
{
    internal class VBlankState : PixelProcessingUnitStateBase
    {
        private int _dotCounter = 0;

        public VBlankState(PixelProcessingUnitContext context, PixelProcessingUnitStateMachine stateMachine) 
            : base(context, stateMachine)
        {
        }

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter % 456 == 0)
                _context.CurrentLine++;

            if (_dotCounter == 4560)
            {
                _context.CurrentLine = 0;
                _stateMachine.TransitionTo<OamScanState>();
            }                
        }

        public override int GetModeNumber()
        {
            return Common.Constants.Video.VBlankModeNo;
        }

        public override void Initialize(int clocks)
        {
            _dotCounter = 0;
        }
    }
}
