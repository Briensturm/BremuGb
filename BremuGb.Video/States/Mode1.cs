namespace BremuGb.Video
{
    public class Mode1 : PPUStateBase
    {
        private int _dotCounter = 0;

        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if (_dotCounter % 456 == 0)
                _context._lineCounter++;

            if (_dotCounter == 4560)
            {
                _context._lineCounter = 0;
                _context.TransitionTo(new Mode2());
            }                
        }

        public override int GetStateNumber()
        {
            return 1;
        }
    }
}
