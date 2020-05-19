namespace BremuGb.Video
{
    public class PPUContext
    {
        private PPUStateBase _state;
        internal PPU PPU { get; private set; }        

        public int _lineCounter = 0;

        public PPUContext(PPUStateBase state, PPU ppu)
        {
            PPU = ppu;

            TransitionTo(state);            
        }

        public void TransitionTo(PPUStateBase state)
        {
            //Console.WriteLine($"Context: Transition to {state.GetType().Name}. Line: {_lineCounter}");

            _state = state;
            _state.SetContext(this);
        }

        public void AdvanceMachineCycle()
        {
            _state.AdvanceMachineCycle();
        }

        public int GetStateNumber()
        {
            return _state.GetStateNumber();
        }        

        public int GetLineNumber()
        {
            return _lineCounter;
        }

        public void RaiseVideoInterruptOccuredEvent(int interruptType)
        {
            VideoInterruptOccuredEvent?.Invoke(interruptType);
        }

        public delegate void VideoInterruptOccured(int interruptType);

        public event VideoInterruptOccured VideoInterruptOccuredEvent;
    }
}
