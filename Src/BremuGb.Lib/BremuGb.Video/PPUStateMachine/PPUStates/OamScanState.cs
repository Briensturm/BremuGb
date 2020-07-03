namespace BremuGb.Video
{
    internal class OamScanState : PixelProcessingUnitStateBase
    {
        private int _dotCounter = 0;

        public OamScanState(PixelProcessingUnitContext context, PPUStateMachine stateMachine) 
            : base(context, stateMachine)
        {
        }

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter == 80)
                _stateMachine.TransitionTo<PixelWritingState>();
        }

        public override int GetModeNumber()
        {
            return Common.Constants.Video.OamScanModeNo;
        }

        public override void Initialize(int clocks)
        {
            _dotCounter = 0;
        }
    }
}
