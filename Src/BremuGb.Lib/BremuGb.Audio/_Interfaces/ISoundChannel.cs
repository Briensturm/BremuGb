namespace BremuGb.Audio
{
    internal interface ISoundChannel
    {
        void AdvanceMachineCycle();

        void PrepareClockLength();
        void ClockLength();
        void ClockEnvelope();
        void ClockSweep();

        byte GetSample();
        
        bool IsEnabled();
        void Disable();

        void SetLengthCounter(int length);
    }
}
