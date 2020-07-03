namespace BremuGb.Video
{
    public interface IPixelProcessingUnitState
    {
        public void Initialize(int clocks);
        public void AdvanceMachineCycle();
        public int GetModeNumber();
    }
}
