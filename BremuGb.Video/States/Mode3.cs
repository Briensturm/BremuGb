namespace BremuGb.Video
{
    public class Mode3 : PPUStateBase
    {
        private int _dotCounter = 0;


        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter == 168)
                _context.TransitionTo(new Mode0());
        }

        public override int GetStateNumber()
        {
            return 3;
        }
    }
}
