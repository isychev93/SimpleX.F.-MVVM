using System;

namespace SimpleXamarinFormsMVVM.Core.IOC
{
    public interface ISimpleServiceLocator : IServiceProvider
    {
        bool CanResolve(Type serviceType);
        bool CanResolve<TInterface>() where TInterface : class;

        /// <remarks>
        /// Keep in mind rule "Last-registered wins".
        /// It's means that if you call <see cref="RegisterType{TInterface, TType}()"/> twice for 
        /// same interfaces but different implementation, this function return last registred.
        /// </remarks>
        TInterface Resolve<TInterface>() where TInterface : class;

        /// <summary>
        /// Register constructor for service type, which will return new instance for each <see cref="Resolve"/> call.
        /// </summary>
        /// <typeparam name="TInterface">Service implemented interface.</typeparam>
        /// <typeparam name="TType">Service type.</typeparam>
        void RegisterType<TInterface, TType>() where TInterface : class where TType : class, TInterface;
        /// <summary>
        /// Register constructor for service type, which will return new instance for each <see cref="Resolve"/> call.
        /// </summary>
        /// <typeparam name="TInterface">Service implemented interface.</typeparam>
        /// <param name="constructor">Service constructor.</param>
        void RegisterType<TInterface>(Func<TInterface> constructor) where TInterface : class;

        void RegisterSingleton<TInterface, TType>() where TInterface : class where TType : class, TInterface;
        void RegisterSingleton<TInterface>(Func<TInterface> constructor) where TInterface : class;
        void RegisterSingleton<TInterface>(TInterface service) where TInterface : class;

        void LazyRegisterSingleton<TInterface, TType>() where TInterface : class where TType : class, TInterface;
        void LazyRegisterSingleton<TInterface>(Func<TInterface> constructor) where TInterface : class;
    }
}
