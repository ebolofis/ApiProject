using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Hubs;
using Symposium.Helpers.Interfaces;

namespace Symposium.Helpers
{
    public class HelperDIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {


            //Register Helper classes 
            builder.RegisterType<CustomJsonDeserializers>().As<ICustomJsonDeserializers>();
            builder.RegisterType<CustomJsonSerializers>().As<ICustomJsonSerializers>();
            builder.RegisterType<EmailHelper>().As<IEmailHelper>();
            builder.RegisterType<WebApiClientHelper>().As<IWebApiClientHelper>();
            builder.RegisterType<WebApiClientRestSharpHelper>().As<IWebApiClientRestSharpHelper>();

            builder.RegisterType<StoreIdsPropertiesHelper>().As<IStoreIdsPropertiesHelper>();

            builder.RegisterType<FilesHelper>().As<IFilesHelper>();

            builder.RegisterGeneric(typeof(PaginationHelper<>)).As(typeof(IPaginationHelper<>)).InstancePerLifetimeScope();

            builder.RegisterType<LocalConfigurationHelper>().SingleInstance();

            builder.RegisterType<LoginFailuresHelper>().SingleInstance();

            builder.RegisterType<GreekVowelsHelper>().As<IGreekVowelsHelper>();

            builder.RegisterType<ExternalCashedOrdersHelper>().As<IExternalCashedOrdersHelper>().SingleInstance();
            builder.RegisterType<CashedLoginsHelper>().As<ICashedLoginsHelper>().SingleInstance();

            builder.RegisterType<DeliveryRoutingHubParticipants>().As<IDeliveryRoutingHubParticipants>().SingleInstance();

            builder.RegisterType<PhoneticGrHelper>().As<PhoneticAbstHelper>();

            // ... register more services for that layer

        }

    }
}
