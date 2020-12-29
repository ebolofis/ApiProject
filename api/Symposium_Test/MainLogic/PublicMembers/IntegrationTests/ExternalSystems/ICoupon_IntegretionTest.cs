using NUnit.Framework;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.MainLogic.PublicMembers.IntegrationTests.ExternalSystems
{
    class ICoupon_IntegretionTest : IoCSupportedTest<TestModule>
    {

        DBInfoModel store;
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IiCouponTasks ictask;
        IWebApiClientHelper wapich;

        public ICoupon_IntegretionTest()
        {
            wapich = Resolve<IWebApiClientHelper>();//IoC
            ictask = Resolve<IiCouponTasks>();//IoC
        }


        [OneTimeSetUp]
        public void Init()
        {
            //1. Register AutoMapper
            //AutoMapperConfig.RegisterMappings();
            //2. Create Store from config file 
            //string storeJson = Properties.Settings.Default.Store_NikkiBeach;
            //CustomJsonDeserializers jdes = new CustomJsonDeserializers();
            //store = jdes.JsonToStore(storeJson);
            //connectionString = usersToDatabases.ConfigureConnectionString(store);
        }

        [Test, Order(1)]
        public void ICoupon_GetHeaderAuthICouponAPI()
        {
            //string ret = ictask.ConstructURI(0);
            string ret = ictask.RequestAuthHeader();
            Assert.NotNull(ret);
        }

        ///ConstructURI(ICouponAppSettingsEnum option)
    }
}
