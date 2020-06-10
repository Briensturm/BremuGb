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

            foreach(var soundChannel in soundChannels)
            {
                if(_timer % 2 == 0)
                    soundChannel.ClockLength();

                if (_timer == 7)
                    soundChannel.ClockEnvelope();

                if (_timer == 2 || _timer == 6)
                    soundChannel.ClockSweep();
            }
        }
    }
}
