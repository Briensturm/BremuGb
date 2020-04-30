
namespace BremuGb.Common.Constants
{
    public static class AudioRegisters
    {
        public const ushort MasterVolume        = 0xFF24;
        public const ushort ChannelOutputSelect = 0xFF25;
        public const ushort SoundOnOff          = 0xFF26;

        //channel 1 - square wave with sweep and envelope
        public struct Channel1
        {
            public const ushort Sweep       = 0xFF10;
            public const ushort DutyLength  = 0xFF11;
            public const ushort Envelope    = 0xFF12;
            public const ushort FrequencyLo = 0xFF13;
            public const ushort FrequencyHi = 0xFF14;
        }

        //channel 2 - square wave with envelope
        public struct Channel2
        {
            public const ushort DutyLength  = 0xFF16;
            public const ushort Envelope    = 0xFF17;
            public const ushort FrequencyLo = 0xFF18;
            public const ushort FrequencyHi = 0xFF19;
        }

        //channel 3 - wave channel
        public struct Channel3
        {
            public const ushort OnOff       = 0xFF1A;
            public const ushort SoundLength = 0xFF1B;
            public const ushort OutputLevel = 0xFF1C;
            public const ushort FrequencyLo = 0xFF1D;
            public const ushort FrequencyHi = 0xFF1E;
        }

        //channel 4 - white noise
        public struct Channel4
        {
            public const ushort SoundLength       = 0xFF20;
            public const ushort Envelope          = 0xFF21;
            public const ushort PolynomialCounter = 0xFF22;
            public const ushort InitConsecutive   = 0xFF23;
        }
    }
}
