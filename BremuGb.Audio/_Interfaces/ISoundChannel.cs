namespace BremuGb.Audio
{
    internal interface ISoundChannel
    {
        void ClockLength();
        void ClockEnvelope();
        void ClockSweep();

        byte GetSample();

        void AdvanceMachineCycle();
        bool IsEnabled();

        void Disable();
    }
}
