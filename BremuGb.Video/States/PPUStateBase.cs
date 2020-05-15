namespace BremuGb.Video
{
    public abstract class PPUStateBase
    {
        protected PPUContext _context;

        public void SetContext(PPUContext context)
        {
            _context = context;
        }

        public abstract void AdvanceMachineCycle();
        public abstract int GetStateNumber();
    }
}
