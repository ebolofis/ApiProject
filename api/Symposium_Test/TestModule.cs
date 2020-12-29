using Autofac;
using Symposium.WebApi.DataAccess;
using Symposium.WebApi.MainLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test
{
    public class TestModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<MainLogicDIModule>();
            // ... register more services for that layer
        }


    }
}
