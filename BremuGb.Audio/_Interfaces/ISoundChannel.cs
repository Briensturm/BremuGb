namespace BremuGb.Audio
{
    public interface ISoundChannel
    {
        public void ClockLength();
        public void ClockEnvelope();
        public void ClockSweep();

        public byte GetSample();

        public void AdvanceMachineCycle();
    }
}
