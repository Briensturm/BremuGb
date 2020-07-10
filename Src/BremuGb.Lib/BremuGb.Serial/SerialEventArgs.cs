using System;

namespace BremuGb.Serial
{
    public class SerialEventArgs : EventArgs
    {
        public byte SerialData { get; private set; }

        public SerialEventArgs(byte serialData)
        {
            SerialData = serialData;
        }
    }
}
