using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Abstractions
{
    public class IOC
    {
        private static IOC? _instance;
        public static IOC Instance => _instance ??= (_instance = new IOC());

        private readonly Dictionary<Type, object> _services;

        IOC()
        {
            _services = new();
        }

        public T Resolve<T>() => (T)_services[typeof(T)];

        public IOC RegisterInstance<TService, TImplementation>(TImplementation instance) where TImplementation : class, TService
        {
            _services[typeof(TService)] = instance;
            return this;
        }
        public IOC RegisterInstance<TService>(TService instance) where TService : class =>
            RegisterInstance<TService, TService>(instance);
    }
}
