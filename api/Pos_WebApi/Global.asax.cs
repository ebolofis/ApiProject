using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Pos_WebApi.Controllers;
using Pos_WebApi.Helpers.V3;
using Pos_WebApi.Interfaces;
using Symposium.Helpers;
using Symposium.WebApi.MainLogic;
using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Configuration;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Pos_WebApi.Modules;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using System.IO;
using System.Web;
using System.Linq;
using Symposium.Models.Models;
using System.Data.SqlClient;
using System.Data;
using Symposium.Models.Models.Scheduler;
using System.Collections.Generic;
using Symposium.Plugins;
using Symposium.Helpers.Hubs;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Delivery;
using Symposium.WebApi.MainLogic.Tasks.Delivery;
using Symposium.Models.Models.Delivery;

namespace Pos_WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        internal IStoreIdsPropertiesHelper stores;
        static DeliveryRoutingHubParticipants drHubParticipants;
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            var logger = log4net.LogManager.GetLogger(this.GetType());
            logger.Info("");
            logger.Info("");
            logger.Info("*****************************************");
            logger.Info("*                                       *");
            logger.Info("*           Application Started         *");
            logger.Info("*                                       *");
            logger.Info("*****************************************");
            logger.Info("");

            // Initialize main configuration
            MainConfigurationHelper.InitializeConfiguration();
            
            bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");

            if (isDeliveryAgent && isDeliveryStore)
            {
                logger.Fatal(Environment.NewLine + Environment.NewLine + " >>>>>>> API MUST NOT BE CONFIGURED AS 'DELIVERY AGENT' AND AS 'DA CLIENT' AT THE SAME TIME. <<<<<<<<<" + Environment.NewLine);
                throw new Exception("API MUST NOT BE CONFIGURED AS 'DELIVERY AGENT' AND AS 'DA CLIENT' AT THE SAME TIME");
            }

            if (isDeliveryAgent)
            {
                logger.Info("Web Api as DELIVERY AGENT");
                logger.Info("");
            }
            if (isDeliveryStore)
            {
                logger.Info("Web Api as DA-Client");
                logger.Info("");
            }


            System.Diagnostics.Debug.WriteLine("Application Started");
           
            try {

                AreaRegistration.RegisterAllAreas();

                System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                // Make long polling connections wait a maximum of 110 seconds for a
                // response. When that time expires, trigger a timeout command and
                // make the client reconnect.
                GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

                // Wait a maximum of 30 seconds after a transport connection is lost
                // before raising the Disconnected event to terminate the SignalR connection.
                GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);

                // For transports other than long polling, send a keepalive packet every
                // 10 seconds. 
                // This value must be no more than 1/3 of the DisconnectTimeout value.
                GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);

                //Register Autofac
                var container= registerDI();

                PluginHelper plugins = container.Resolve<PluginHelper>();
                logger.Info("Loading plugins...");
                plugins.LoadDLLFiles();
                logger.Info("Found " + PluginHelper.PluginList.Count.ToString()+ " plugins: " + String.Join(",", PluginHelper.PluginList.Select(x=>x.TypeInfo.Name+" ["+ x.CategoryName + " (" + x.Version + ")] ").ToList()));

                var pi = new ProductInfoController();
                logger.Info("WEBAPI version: "+pi.GetApiVersion());
                logger.Info("");

                bool useDeliveryRouting = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drUseRouting");
                if (useDeliveryRouting)
                {
                    drHubParticipants = container.Resolve<DeliveryRoutingHubParticipants>();
                    //drHubParticipants.AddClient("1", 78);
                    Application["drHubParticipants"] = drHubParticipants;
                    DeliveryRoutingTasks drTasks = container.Resolve<DeliveryRoutingTasks>();
                    stores = container.Resolve<IStoreIdsPropertiesHelper>();
                    string drDefaultGuid = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drDefaultGuid");
                    DBInfoModel DBInfo = stores.GetStoreById(new Guid(drDefaultGuid));
                    logger.Info("Web Api is running with Delivery Routing activated. StoreId used: " + drDefaultGuid);
                    drTasks.restartAsyncAssignRouteToStaff(DBInfo, GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>());
                }
            }
            catch (Exception x)
            {
                logger.Fatal(x);
                throw;
            }
        }

        void Application_Error( object sender, EventArgs e ) {
            var logger = log4net.LogManager.GetLogger(this.GetType());
            Exception exc = Server.GetLastError();
            logger.Error(exc);
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        void Application_End(object sender, EventArgs e)
        {
            //Set variable ApiRunning as false so all services have to stop
            var config = System.Web.Http.GlobalConfiguration.Configuration;
            System.Web.Http.Dependencies.IDependencyResolver autofac;
            autofac = config.DependencyResolver;
            IStoreIdsPropertiesHelper stores = (IStoreIdsPropertiesHelper)autofac.GetService(typeof(IStoreIdsPropertiesHelper));
            stores.ApiRunning = false;

            var logger = log4net.LogManager.GetLogger(this.GetType());
            logger.Error("Api stoped and store variable ApiRunnig is false");
            logger.Info("");
            logger.Info("");
            logger.Info("");
            logger.Info("========================================= APPLICATION END =========================================");
            logger.Info("");
            logger.Info("");
            logger.Info("");
            logger.Info("");
        }

        /// <summary>
        /// Register Autofac (Dependency Injection)
        /// </summary>
        private IContainer registerDI()
        {
            // Init AutoFac
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = System.Web.Http.GlobalConfiguration.Configuration;

            //Register DI Module from MainLogic and Helpers
            builder.RegisterModule<MainLogicDIModule>();

            //register 'UsersToDatabases.xml' to autofac.
            string usersToDB = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            string xmlDoc = File.ReadAllText(usersToDB);
            builder.RegisterType<StoreIdsPropertiesHelper>().As<IStoreIdsPropertiesHelper>().WithParameter("xdoc", xmlDoc).SingleInstance();

            builder.RegisterType<DeliveryRoutingHubParticipants>().SingleInstance();

            builder.RegisterType<DeliveryRoutingRoutesList>().SingleInstance();
            
            builder.RegisterType<DeliveryRoutingTasks>();

            builder.RegisterType<StoreIdsPropertiesHelper>();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // Set the dependency resolver to be Autofac.
            IContainer container = builder.Build();
            // container.Resolve<IDA_Jbof>
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            
            /*Adding Jobs To Scheduler*/
            var logger = log4net.LogManager.GetLogger(this.GetType());

            System.Web.Http.Dependencies.IDependencyResolver autofac;
            autofac = config.DependencyResolver;
            IStoreIdsPropertiesHelper stores = (IStoreIdsPropertiesHelper)autofac.GetService(typeof(IStoreIdsPropertiesHelper));
            stores.ApiRunning = true;
            
            Symposium_SchedulerJob.Main Scheduler = new Symposium_SchedulerJob.Main(container, stores);
            Scheduler.StartTimer();
            return container;
        }

    }    

}