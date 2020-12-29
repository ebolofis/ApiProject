using Newtonsoft.Json;
using NUnit.Framework;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.Hotel;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.DAOs.Integration_Tests
{
    [TestFixture]
  public  class CashedDTsTest : IoCSupportedTest<TestModule>//<-- the module to register
    {
        ICashedDT<MacroTimezoneModel, HotelMacroTimezoneDTO> timezonesDT;


        ICustomJsonDeserializers jdes;
        IUsersToDatabasesXML usersToDatabases;
        DBInfoModel storeNikkiBeach;
        DBInfoModel storeDeliveryAgent;
        long posInfo;
        string connectionString;

        public CashedDTsTest()
        {
           // jdes = Resolve<ICustomJsonDeserializers>();
           // usersToDatabases = Resolve<IUsersToDatabasesXML>();
           timezonesDT = Resolve<ICashedDT<MacroTimezoneModel, HotelMacroTimezoneDTO>>();//IoC

        }

        [OneTimeSetUp]
        public void Init()
        {
            //Create Store from config file 
            string storeJson = Properties.Settings.Default.Store_NikkiBeach;
            string storeJson2 = Properties.Settings.Default.Store_DeliveryAgent;
            //  store = jdes.JsonToStore(storeJson);
            storeNikkiBeach = JsonConvert.DeserializeObject<DBInfoModel>(storeJson);
            storeDeliveryAgent = JsonConvert.DeserializeObject<DBInfoModel>(storeJson2);
            posInfo = Properties.Settings.Default.PosInfo;

            
        }


        [Test, Order(1)]
        public void InsertNewTimeZone1()
        {
            MacroTimezoneModel tm1 = new MacroTimezoneModel() { Code = "B", Description = "Breakfast", TimeFrom = new DateTime(2020, 01, 01, 7, 0, 0), TimeTo = new DateTime(2020, 01, 01, 10, 0, 0), HotelId = 1 };
            MacroTimezoneModel tm2 = new MacroTimezoneModel() { Code = "B", Description = "Breakfast2", TimeFrom = new DateTime(2020, 01, 01, 8, 0, 0), TimeTo = new DateTime(2020, 01, 01, 11, 0, 0), HotelId = 2 };
            Guid guid1 = timezonesDT.Insert(storeNikkiBeach, tm1);



            List<MacroTimezoneModel> list1 = timezonesDT.Select(storeNikkiBeach);
            List<MacroTimezoneModel> list2 = timezonesDT.Select(storeDeliveryAgent);

            Assert.True(list1.Count == 1);
            Assert.True(list1[0].Id == guid1);
            Assert.True(list1[0].Description == "Breakfast");

            Assert.True(list2.Count == 0);

            Guid guid2 = timezonesDT.Insert(storeDeliveryAgent, tm2);
            list2 = timezonesDT.Select(storeDeliveryAgent);
            Assert.True(list2.Count == 1);
            Assert.True(list2[0].Id == guid2);

        }

        [Test, Order(2)]
        public void InsertNewTimeZone2()
        {
            List<MacroTimezoneModel> list1 = timezonesDT.Select(storeNikkiBeach);
            Assert.True(list1.Count == 1);

            MacroTimezoneModel tm1 = new MacroTimezoneModel() { Code = "L", Description = "Launch", TimeFrom = new DateTime(2020, 01, 01, 13, 0, 0), TimeTo = new DateTime(2020, 01, 01, 16, 0, 0), HotelId = 1 };
            Guid guid1 = timezonesDT.Insert(storeNikkiBeach, tm1);



            list1 = timezonesDT.Select(storeNikkiBeach);

            Assert.True(list1.Count == 2);
            Assert.True(list1[1].Id == guid1);
            Assert.True(list1[1].Description == "Launch");


            List<MacroTimezoneModel> list2 = timezonesDT.Select(storeDeliveryAgent);
            Assert.True(list2.Count == 1);

        }
    }
}
