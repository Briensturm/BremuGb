using BremuGb.Audio.SoundChannels;

namespace BremuGb.Audio.SoundChannels
{
    internal class WaveChannel : SoundChannelBase
    {
        public byte OnOff { get; internal set; }
        public byte OutputLevel { get; internal set; }
        public byte FrequencyHi { get; internal set; }
        public byte FrequencyLo { get; internal set; }
        public byte SoundLength { get; internal set; }

        public override void AdvanceMachineCycle()
        {
            
        }

        public override void ClockEnvelope()
        {
        }

        public override void ClockLength()
        {
        }

        public override void ClockSweep()
        {
        }

        public override byte GetSample()
        {
            return 0;
        }
    }
}
