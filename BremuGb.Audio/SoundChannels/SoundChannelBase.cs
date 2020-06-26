using System;

namespace BremuGb.Audio.SoundChannels
{
    internal abstract class SoundChannelBase : ISoundChannel
    {
        protected bool _prepareClockLength;
        protected int _lengthCounter;
        protected bool _isEnabled;
        protected bool _compareLength;

        public abstract void AdvanceMachineCycle();
        protected abstract int ChannelMaxLength { get; }

        public virtual void ClockEnvelope()
        {
        }

        public void ClockLength()
        {
            _prepareClockLength = false;

            if (_compareLength && _lengthCounter > 0)
            {
                _lengthCounter--;

                if (_lengthCounter == 0)
                    _isEnabled = false;
            }
        }

        public virtual void SetLengthCounter(int length)
        {
            _lengthCounter = ChannelMaxLength - length;
        }

        protected void ReloadLengthCounter()
        {
            if (_lengthCounter == 0)
            {
                if (_compareLength && !_prepareClockLength)
                    _lengthCounter = ChannelMaxLength - 1;
                else
                    _lengthCounter = ChannelMaxLength;
            }
        }

        public virtual void ClockSweep()
        {
        }

        public void PrepareClockLength()
        {
            _prepareClockLength = true;
        }

        public abstract byte GetSample();
        public abstract bool IsEnabled();
        public virtual void Disable()
        {
            _prepareClockLength = true;
        }
    }
}
