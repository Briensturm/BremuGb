namespace BremuGb.Audio
{
    internal interface ISoundChannel
    {
        void PrepareClockLength();
        void ClockLength();
        void ClockEnvelope();
        void ClockSweep();

        byte GetSample();

        void AdvanceMachineCycle();
        bool IsEnabled();

        void Disable();

        void SetLengthCounter(int length);
    }
}
