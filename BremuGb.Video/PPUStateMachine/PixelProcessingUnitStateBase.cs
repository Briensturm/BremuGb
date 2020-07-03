namespace BremuGb.Video
{
    internal abstract class PixelProcessingUnitStateBase : IPixelProcessingUnitState
    {
        protected PPUStateMachine _stateMachine;
        protected PixelProcessingUnitContext _context;

        protected PixelProcessingUnitStateBase(PixelProcessingUnitContext context, PPUStateMachine stateMachine)
        {
            _context = context;
            _stateMachine = stateMachine;
        }

        public abstract void AdvanceMachineCycle();

        public abstract int GetModeNumber();

        public abstract void Initialize(int clocks);
    }
}
