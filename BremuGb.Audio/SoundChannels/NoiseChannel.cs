using BremuGb.Audio.SoundChannels;

namespace BremuGb.Audio.SoundChannels
{
    internal class NoiseChannel : SoundChannelBase
    {
        public byte Envelope { get; internal set; }
        public byte InitConsecutive { get; internal set; }
        public byte PolynomialCounter { get; internal set; }
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
