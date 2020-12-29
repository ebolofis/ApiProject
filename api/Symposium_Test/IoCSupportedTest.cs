using Autofac;
using Autofac.Core;

namespace Symposium_Test
{
    /// <summary>
    /// class supporting Autofac for NUnit
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    public class IoCSupportedTest<TModule> where TModule : IModule, new()
    {
        private IContainer container;

        public IoCSupportedTest()
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
