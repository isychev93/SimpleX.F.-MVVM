namespace SimpleXamarinFormsMVVM.Core.View.Services
{
    public interface ITraceService
    {
        void Info(string format, params object[] args);
        void Warn(string format, params object[] args);
        void Error(string format, params object[] args);
    }
}
