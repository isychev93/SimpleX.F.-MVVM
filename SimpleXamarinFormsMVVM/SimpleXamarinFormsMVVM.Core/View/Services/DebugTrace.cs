using System.Diagnostics;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public class DebugTrace : ITraceService
    {
        public void Info(string format, params object[] args)
        {
            Debug.WriteLine("INFO: " + format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine("WARN: " + format, args);
        }

        public void Error(string format, params object[] args)
        {
            Debug.WriteLine("ERROR" + format, args);
        }
    }
}
