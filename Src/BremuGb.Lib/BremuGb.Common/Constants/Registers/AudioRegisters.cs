namespace BremuGb.Common.Constants
{
    public static class AudioRegisters
    {
        public const ushort NR50 = 0xFF24;
        public const ushort NR51 = 0xFF25;
        public const ushort NR52 = 0xFF26;

        public const ushort WaveRamFirstAddress = 0xFF30;
        public const ushort WaveRamLastAddress  = 0xFF3F;

        public const ushort UnusedAudioAddressRangeStart = 0xFF27;
        public const ushort UnusedAudioAddressRangeEnd   = 0xFF2F;


        //channel 1 - square wave with sweep and envelope
        public struct Channel1
        {
            public const ushort NR10 = 0xFF10;
            public const ushort NR11 = 0xFF11;
            public const ushort NR12 = 0xFF12;
            public const ushort NR13 = 0xFF13;
            public const ushort NR14 = 0xFF14;
        }

        //channel 2 - square wave with envelope
        public struct Channel2
        {
            public const ushort NR20 = 0xFF15;
            public const ushort NR21 = 0xFF16;
            public const ushort NR22 = 0xFF17;
            public const ushort NR23 = 0xFF18;
            public const ushort NR24 = 0xFF19;
        }

        //channel 3 - wave channel
        public struct Channel3
        {
            public const ushort NR30 = 0xFF1A;
            public const ushort NR31 = 0xFF1B;
            public const ushort NR32 = 0xFF1C;
            public const ushort NR33 = 0xFF1D;
            public const ushort NR34 = 0xFF1E;
        }

        //channel 4 - noise
        public struct Channel4
        {
            public const ushort NR40 = 0xFF1F;
            public const ushort NR41 = 0xFF20;
            public const ushort NR42 = 0xFF21;
            public const ushort NR43 = 0xFF22;
            public const ushort NR44 = 0xFF23;
        }
    }
}
