using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Plugins;

namespace Symposium.Plugins
{
    public class PluginsDIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Register Plugins classes 
            builder.RegisterType<PluginHelper>().SingleInstance(); 
        }

    }
}
