using SimpleXamarinFormsMVVM.Core.IOC;
using SimpleXamarinFormsMVVM.Core.View.Services;

namespace SimpleXamarinFormsMVVM.Core
{
    public abstract class MVVMSetup
    {
        public virtual void Initialize()
        {
            RegisterServices();
            RegisterViews(SimpleServiceLocator.Instance.Resolve<IPageLoaderService>());
        }

        protected virtual void RegisterServices()
        {
            SimpleServiceLocator.Instance.RegisterSingleton(GetPageLoaderService);
            SimpleServiceLocator.Instance.RegisterSingleton(GeTraceService);
            SimpleServiceLocator.Instance.RegisterSingleton(GetNavigationService);
        }

        protected virtual INavigationService GetNavigationService()
        {
            return new NavigationService(SimpleServiceLocator.Instance.Resolve<IPageLoaderService>(), SimpleServiceLocator.Instance.Resolve<ITraceService>());
        }

        protected virtual IPageLoaderService GetPageLoaderService()
        {
            return new PageLoaderService();
        }

        protected virtual ITraceService GeTraceService()
        {
            return new DebugTrace();
        }

        protected abstract void RegisterViews(IPageLoaderService pageLoaderService);
    }
}
