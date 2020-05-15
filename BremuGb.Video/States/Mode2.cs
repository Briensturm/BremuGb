namespace BremuGb.Video
{
    public class Mode2 : PPUStateBase
    {
        private int _dotCounter = 0;

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter == 80)
                _context.TransitionTo(new Mode3());
        }

        public override int GetStateNumber()
        {
            return 2;
        }
    }
}
