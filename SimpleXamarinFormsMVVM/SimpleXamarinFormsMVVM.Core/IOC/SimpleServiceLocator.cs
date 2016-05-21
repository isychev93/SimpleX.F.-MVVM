using System;
using System.Collections.Generic;
using SimpleXamarinFormsMVVM.Core.Extentions;

namespace SimpleXamarinFormsMVVM.Core.IOC
{
    public sealed class SimpleServiceLocator : ISimpleServiceLocator
    {
        private readonly Dictionary<Type, ServiceInfo> servicesInfos = new Dictionary<Type, ServiceInfo>();
        private readonly object syncObject = new object();

        private SimpleServiceLocator()
        {
        }

        public static ISimpleServiceLocator Instance { get; } = new SimpleServiceLocator();

        public bool CanResolve(Type serviceType)
        {
            lock (syncObject)
            {
                return servicesInfos.ContainsKey(serviceType);
            }
        }

        public bool CanResolve<TInterface>() where TInterface : class
        {
            return CanResolve(typeof(TInterface));
        }

        public object GetService(Type serviceType)
        {
            lock (syncObject)
            {
                if (!CanResolve(serviceType))
                    throw new InvalidOperationException(string.Format("Service with interface {0} not registered.", serviceType));

                var serviceInfo = servicesInfos[serviceType];
                switch (serviceInfo.Behaviour)
                {
                    case ServicesBehaviour.Dynamic:
                        return serviceInfo.LazyConstructor();
                    case ServicesBehaviour.LazySingleton:
                        return serviceInfo.StoredObject ?? (serviceInfo.StoredObject = serviceInfo.LazyConstructor());
                    case ServicesBehaviour.Singleton:
                        return serviceInfo.StoredObject;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unknown ServicesBehaviour {0}", serviceInfo.Behaviour));
                }
            }
        }

        public TInterface Resolve<TInterface>() where TInterface : class
        {
            return (TInterface)GetService(typeof(TInterface));
        }

        public void RegisterType<TInterface, TType>() where TInterface : class where TType : class, TInterface
        {
            lock (syncObject)
            {
                Func<object> makeInstanceFunction = () => typeof(TType).CreateInstance();
                RegisterServiceInfo<TInterface>(ServicesBehaviour.Dynamic, makeInstanceFunction, null);
            }
        }

        public void RegisterType<TInterface>(Func<TInterface> constructor) where TInterface : class
        {
            lock (syncObject)
            {
                RegisterServiceInfo<TInterface>(ServicesBehaviour.Dynamic, constructor, null);
            }
        }

        public void RegisterSingleton<TInterface, TType>() where TInterface : class where TType : class, TInterface
        {
            lock (syncObject)
            {
                Func<object> makeInstanceFunction = () => typeof(TType).CreateInstance();
                RegisterServiceInfo<TInterface>(ServicesBehaviour.Singleton, null, makeInstanceFunction());
            }
        }

        public void RegisterSingleton<TInterface>(Func<TInterface> constructor) where TInterface : class
        {
            lock (syncObject)
            {
                RegisterServiceInfo<TInterface>(ServicesBehaviour.Singleton, null, constructor());
            }
        }

        public void RegisterSingleton<TInterface>(TInterface service) where TInterface : class
        {
            lock (syncObject)
            {
                RegisterServiceInfo<TInterface>(ServicesBehaviour.Singleton, null, service);
            }
        }

        public void LazyRegisterSingleton<TInterface, TType>() where TInterface : class where TType : class, TInterface
        {
            lock (syncObject)
            {
                Func<object> makeInstanceFunction = () => typeof(TType).CreateInstance();
                RegisterServiceInfo<TInterface>(ServicesBehaviour.LazySingleton, makeInstanceFunction, null);
            }
        }

        public void LazyRegisterSingleton<TInterface>(Func<TInterface> constructor) where TInterface : class
        {
            lock (syncObject)
            {
                RegisterServiceInfo<TInterface>(ServicesBehaviour.LazySingleton, constructor, null);
            }
        }

        private void RegisterServiceInfo<TInterface>(ServicesBehaviour behaviour, Func<object> instanceCreator, object service) where TInterface : class
        {
            var interfaceType = typeof(TInterface);
            if (servicesInfos.ContainsKey(interfaceType))
                servicesInfos.Remove(interfaceType);

            servicesInfos.Add(interfaceType, new ServiceInfo(behaviour, instanceCreator, service));
        }

        private struct ServiceInfo
        {
            public ServicesBehaviour Behaviour { get; }

            public Func<object> LazyConstructor { get; }

            public object StoredObject { get; set; }

            public ServiceInfo(ServicesBehaviour behaviour, Func<object> lazyConstructor, object storedObject)
            {
                Behaviour = behaviour;
                LazyConstructor = lazyConstructor;
                StoredObject = storedObject;
            }
        }

        private enum ServicesBehaviour
        {
            Dynamic,
            Singleton,
            LazySingleton
        }
    }
}
