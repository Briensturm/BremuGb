namespace BremuGb.Audio.SoundChannels
{
    internal class SquareWaveSweepChannel : SquareWaveChannel
    {
        //TODO: sweep
        public byte Sweep
        {
            get
            {
                return (byte)(_sweep | 0x80);
            }

            internal set
            {
                _sweep = value;
            }
        }

        private int _sweep;

        public override void Disable()
        {
            Sweep = 0;

            base.Disable();
        }
    }
}
