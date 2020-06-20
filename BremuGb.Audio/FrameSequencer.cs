using System.Collections.Generic;

namespace BremuGb.Audio
{
    class FrameSequencer
    {
        private int _timer = 0;

        public void AdvanceClock(IEnumerable<ISoundChannel> soundChannels)
        {
            _timer++;
            if (_timer == 8)
                _timer = 0;

            var clockLength = _timer % 2 == 0;
            var clockEnvelope = _timer == 7;
            var clockSweep = _timer == 2 || _timer == 6;

            foreach(var soundChannel in soundChannels)
            {
                if(clockLength)
                    soundChannel.ClockLength();

                if (clockEnvelope)
                    soundChannel.ClockEnvelope();

                if (clockSweep)
                    soundChannel.ClockSweep();
            }
        }
    }
}
