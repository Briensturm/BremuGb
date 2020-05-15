namespace BremuGb.Video
{
    public class PPUContext
    {
        private PPUStateBase _state;

        private byte[] _screenBitmap;

        public int _lineCounter = 0;

        public PPUContext(PPUStateBase state)
        {
            TransitionTo(state);

            _screenBitmap = new byte[Common.Constants.Video.ScreenWidth * Common.Constants.Video.ScreenHeight];
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

        public void WritePixels(int pixel0, int pixel1, int pixel2, int pixel3, int offset)
        {
            
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
