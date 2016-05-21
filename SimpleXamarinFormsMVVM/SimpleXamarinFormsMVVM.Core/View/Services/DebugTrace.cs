using System.Diagnostics;

namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public class DebugTrace : ITraceService
    {
        public void Trace(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }
    }
}
