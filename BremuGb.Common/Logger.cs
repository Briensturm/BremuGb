using System;
using System.IO;
using System.Text;

namespace BremuGb.Common
{
    public class Logger
    {
        private StringBuilder _logStringBuilder;
        private bool _enabled;

        public Logger()
        {
            _logStringBuilder = new StringBuilder();
            _logStringBuilder.AppendLine($"BremuGb {DateTime.Now}");
        }

        public void Log(string message)
        {
            if(_enabled)
                _logStringBuilder.AppendLine(message);
        }

        public void Enable()
        {
            _enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }

        public void SaveLogFile(string path)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine(_logStringBuilder.ToString());
            }
        }
    }
}
