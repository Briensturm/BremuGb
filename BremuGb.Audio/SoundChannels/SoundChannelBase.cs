namespace BremuGb.Audio.SoundChannels
{
    internal abstract class SoundChannelBase : ISoundChannel
    {
        public abstract void AdvanceMachineCycle();

        public virtual void ClockEnvelope()
        {            
        }

        public virtual void ClockLength()
        {
        }

        public virtual void ClockSweep()
        {
        }

        public abstract byte GetSample();
        public abstract bool IsEnabled();
        public abstract void Disable();
    }
}
