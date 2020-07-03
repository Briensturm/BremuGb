using System.Collections.Generic;

namespace BremuGb.Video
{
    internal class PPUStateMachine
    {
        private Dictionary<System.Type, IPixelProcessingUnitState> _states;

        private IPixelProcessingUnitState _currentState;
        private PixelProcessingUnitContext _context;

        public PPUStateMachine(PixelProcessingUnitContext context)
        {
            _context = context;
            _states = new Dictionary<System.Type, IPixelProcessingUnitState>();

            CreatePixelProcessingUnitStates(context);
            TransitionTo<OamScanState>();
        }

        public void TransitionTo<T>(int cycles = 0)
        {
            _currentState = _states[typeof(T)];
            _currentState.Initialize(cycles);
        }

        public void AdvanceMachineCycle()
        {
            _currentState.AdvanceMachineCycle();
        }

        public int GetModeNumber()
        {
            if(_context.LcdEnable == 1)            
                return _currentState.GetModeNumber();

            //when lcd is off, state is forced to 0
            return 0;
        }

        private void CreatePixelProcessingUnitStates(PixelProcessingUnitContext context)
        {
            _states.Add(typeof(HBlankState), new HBlankState(context, this));
            _states.Add(typeof(VBlankState), new VBlankState(context, this));
            _states.Add(typeof(PixelWritingState), new PixelWritingState(context, this));
            _states.Add(typeof(OamScanState), new OamScanState(context, this));
        }
    }
}
