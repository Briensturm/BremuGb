namespace BremuGb.Audio.SoundChannels
{
    internal abstract class SoundChannelBase : ISoundChannel
    {
        protected int _lengthCounter;

        protected bool _isEnabled;
        protected bool _compareLength;
        protected bool _prepareClockLength;

        protected abstract int ChannelMaxLength { get; }

        public abstract void AdvanceMachineCycle();        

        public virtual void ClockEnvelope()
        {
            //channels with envelope must override this
        }

        public virtual void ClockSweep()
        {
            //channels with sweep must override this
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

        protected void HandleLengthClocking(byte triggerRegisterValue)
        {
            var oldCompareLength = _compareLength;
            _compareLength = (triggerRegisterValue & 0x40) == 0x40;

            //obscure behavior
            if (!oldCompareLength && _compareLength && !_prepareClockLength && _lengthCounter > 0)
            {
                _lengthCounter--;

                //if decremented to zero and no trigger, disable channel
                if (_lengthCounter == 0 && ((triggerRegisterValue & 0x80) == 0))
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
