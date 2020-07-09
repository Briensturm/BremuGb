using System.Collections.Generic;

namespace BremuGb.Audio
{
    internal class FrameSequencer
    {
        private int _timer;
        private bool _isEnabled;

        internal void AdvanceClock(ISoundChannel[] soundChannels)
        {
            if (!_isEnabled)
                return;            

            var clockLength = _timer % 2 == 0;
            var clockEnvelope = _timer == 7;
            var clockSweep = _timer == 2 || _timer == 6;

            for(int i = 0; i<soundChannels.Length; i++)            
            {
                var soundChannel = soundChannels[i];

                if (clockLength)
                    soundChannel.ClockLength();
                else
                    soundChannel.PrepareClockLength();

                if (clockEnvelope)
                    soundChannel.ClockEnvelope();

                if (clockSweep)
                    soundChannel.ClockSweep();
            }

            _timer++;
            if (_timer == 8)
                _timer = 0;
        }

        internal void Disable()
        {
            _isEnabled = false;
        }

        internal void ResetAndEnable()
        {
            _isEnabled = true;
            _timer = 0;
        }
    }
}
