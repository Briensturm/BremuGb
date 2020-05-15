namespace BremuGb.Video
{
    public class Mode0 : PPUStateBase
    {
        private int _dotCounter = 0;


        public override void AdvanceMachineCycle()
        {
            _dotCounter += 4;

            if(_dotCounter == 208)
            {
                _context._lineCounter++;

                if (_context._lineCounter == 144)
                {
                    _context.TransitionTo(new Mode1());
                    _context.RaiseVideoInterruptOccuredEvent(0);
                }
                else
                    _context.TransitionTo(new Mode2());                    
            }                
        }

        public override int GetStateNumber()
        {
            return 0;
        }
    }
}
