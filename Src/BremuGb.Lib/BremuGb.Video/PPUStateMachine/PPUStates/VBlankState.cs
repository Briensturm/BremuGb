namespace BremuGb.Video
{
    internal class VBlankState : PixelProcessingUnitStateBase
    {
        private int _dotCounter;
        private int _vblankLineCounter;

        public VBlankState(PixelProcessingUnitContext context, PPUStateMachine stateMachine) 
            : base(context, stateMachine)
        {
        }

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter == 456)
            {
                _context.CurrentLine++;
                _vblankLineCounter++;

                _dotCounter = 0;
            }

            if (_vblankLineCounter == 10)
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
            _vblankLineCounter = 0;
        }
    }
}
