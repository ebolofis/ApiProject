using Autofac;
using Autofac.Core;

namespace Symposium.Helpers.Classes
{
    public class IoCSupported<TModule> where TModule : IModule, new()
    {
        private Autofac.IContainer container;

        public IoCSupported()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new TModule());

            container = builder.Build();
        }

        protected TEntity Resolve<TEntity>(string serviceName = "")
        {
            if (serviceName != "")
                return container.ResolveKeyed<TEntity>(serviceName);
            else
                return container.Resolve<TEntity>();
        }

        protected void ShutdownIoC()
        {
            container.Dispose();
        }
    }
}
