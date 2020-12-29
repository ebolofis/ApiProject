using NUnit.Framework;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.MealBoards;
using Symposium.WebApi.DataAccess;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Hotel;
using Symposium_DTOs.PosModel_Info;
using Symposium_Test.MainLogic.PublicMembers.IntegrationTests;
using Symposium_Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium_Test.MainLogic.IntegrationTest
{
    [TestFixture]
    class Allowance_IntegrationTest : IoCSupportedTest<TestModule>
    {
        DBInfoModel store;
        long posInfo;
        string connectionString;
        IHotelFlows allowanceFlow;
        IUsersToDatabasesXML usersToDatabases;
        IGenericITableDAO<AccountDTO> accountTableDao;
        List<PosReservationHelper> helpers = new List<PosReservationHelper>();
        List<CustomerModel> customers = new List<CustomerModel>();
        int ovride = 1;

        public Allowance_IntegrationTest()
        {
            usersToDatabases = Resolve<IUsersToDatabasesXML>();
            allowanceFlow = Resolve<IHotelFlows>();
        }

        [OneTimeSetUp]
        public void Init()
        {
            AutoMapperConfig.RegisterMappings();

            CustomJsonDeserializers jdes = new CustomJsonDeserializers();
            string storeJson = Properties.Settings.Default.Store_NikkiBeach_Local;
            store = jdes.JsonToStore(storeJson);
            connectionString = usersToDatabases.ConfigureConnectionString(store);

            posInfo = Properties.Settings.Default.PosInfo;

            PosReservationHelper helper0 = new PosReservationHelper();
            helper0.HotelId = 1;
            helper0.PosDepartmentId = 1;
            helper0.Name = "";
            helper0.Page = 1;
            helper0.Pagesize = 50;
            helper0.Room = "200";

            PosReservationHelper helper1 = new PosReservationHelper();
            helper1.HotelId = 2;
            helper1.PosDepartmentId = 2;
            helper1.Name = "";
            helper1.Page = 1;
            helper1.Pagesize = 50;
            helper1.Room = "304";

            helpers.Add(helper0);
            helpers.Add(helper1);

            customers = new List<CustomerModel>();
            CustomerModel customer0 = new CustomerModel();
            customer0.ProfileNo         = 3691;
            customer0.Adults            = 1;
            customer0.Children          = 0;
            customer0.TravelAgentId     = 0;
            customer0.GroupId           = 0;
            customer0.BoardCode         = "BB";
            customer0.Room              = "200";
            customer0.RoomTypeId        = 1;
            customer0.BookedRoomTypeId  = 1;
            customer0.NationalityCode   = "GR";
            customer0.VIP               = "-1";
            customer0.SourceId          = 0;
            customer0.ReservationId     = 59290;


            CustomerModel customer1 = new CustomerModel();
            customer1.ProfileNo         = 3692;
            customer1.Adults            = 1;
            customer1.Children          = 0;
            customer1.TravelAgentId     = 0;
            customer1.GroupId           = 0;
            customer1.BoardCode         = "BB";
            customer1.Room              = "201";
            customer1.RoomTypeId        = 1;
            customer1.BookedRoomTypeId  = 1;
            customer1.NationalityCode   = "GR";
            customer1.VIP               = "-1";
            customer1.SourceId          = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer2 = new CustomerModel();
            customer2.ProfileNo = 3693;
            customer2.Adults = 1;
            customer2.Children = 0;
            customer2.TravelAgentId = 0;
            customer2.GroupId = 0;
            customer2.BoardCode = "BB";
            customer2.Room = "202";
            customer2.RoomTypeId = 1;
            customer2.BookedRoomTypeId = 1;
            customer2.NationalityCode = "GR";
            customer2.VIP = "-1";
            customer2.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer3 = new CustomerModel();
            customer3.ProfileNo = 3741;
            customer3.Adults = 1;
            customer3.Children = 0;
            customer3.TravelAgentId = 0;
            customer3.GroupId = 0;
            customer3.BoardCode = "HB";
            customer3.Room = "221";
            customer3.RoomTypeId = 2;
            customer3.BookedRoomTypeId = 2;
            customer3.NationalityCode = "DE";
            customer3.VIP = "1";
            customer3.SourceId = 1;
            customer0.ReservationId = 59290;

            CustomerModel customer4 = new CustomerModel();
            customer4.ProfileNo = 3742;
            customer4.Adults = 1;
            customer4.Children = 0;
            customer4.TravelAgentId = 0;
            customer4.GroupId = 1;
            customer4.BoardCode = "HB";
            customer4.Room = "222";
            customer4.RoomTypeId = 1;
            customer4.BookedRoomTypeId = 2;
            customer4.NationalityCode = "GR";
            customer4.VIP = "-1";
            customer4.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer5 = new CustomerModel();
            customer5.ProfileNo = 3743;
            customer5.Adults = 1;
            customer5.Children = 0;
            customer5.TravelAgentId = 1;
            customer5.GroupId = 1;
            customer5.BoardCode = "HB";
            customer5.Room = "223";
            customer5.RoomTypeId = 3;
            customer5.BookedRoomTypeId = 1;
            customer5.NationalityCode = "GR";
            customer5.VIP = "-1";
            customer5.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer6 = new CustomerModel();
            customer6.ProfileNo = 3744;
            customer6.Adults = 1;
            customer6.Children = 0;
            customer6.TravelAgentId = 1;
            customer6.GroupId = 0;
            customer6.BoardCode = "HB";
            customer6.Room = "304";
            customer6.RoomTypeId = 1;
            customer6.BookedRoomTypeId = 1;
            customer6.NationalityCode = "GR";
            customer6.VIP = "-1";
            customer6.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer7 = new CustomerModel();
            customer7.ProfileNo = 3745;
            customer7.Adults = 2;
            customer7.Children = 0;
            customer7.TravelAgentId = 1;
            customer7.GroupId = 0;
            customer7.BoardCode = "HB";
            customer7.Room = "314";
            customer7.RoomTypeId = 1;
            customer7.BookedRoomTypeId = 1;
            customer7.NationalityCode = "GR";
            customer7.VIP = "-1";
            customer7.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer8 = new CustomerModel();
            customer8.ProfileNo = 3746;
            customer8.Adults = 2;
            customer8.Children = 0;
            customer8.TravelAgentId = 1;
            customer8.GroupId = 0;
            customer8.BoardCode = "HB";
            customer8.Room = "315";
            customer8.RoomTypeId = 1;
            customer8.BookedRoomTypeId = 1;
            customer8.NationalityCode = "GR";
            customer8.VIP = "-1";
            customer8.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer9 = new CustomerModel();
            customer9.ProfileNo = 3747;
            customer9.Adults = 2;
            customer9.Children = 0;
            customer9.TravelAgentId = 1;
            customer9.GroupId = 0;
            customer9.BoardCode = "HB";
            customer9.Room = "316";
            customer9.RoomTypeId = 1;
            customer9.BookedRoomTypeId = 1;
            customer9.NationalityCode = "GR";
            customer9.VIP = "-1";
            customer9.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer10 = new CustomerModel();
            customer10.ProfileNo = 3748;
            customer10.Adults = 1;
            customer10.Children = 1;
            customer10.TravelAgentId = 1;
            customer10.GroupId = 0;
            customer10.BoardCode = "HB";
            customer10.Room = "317";
            customer10.RoomTypeId = 1;
            customer10.BookedRoomTypeId = 1;
            customer10.NationalityCode = "GR";
            customer10.VIP = "-1";
            customer10.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer11 = new CustomerModel();
            customer11.ProfileNo = 3749;
            customer11.Adults = 1;
            customer11.Children = 1;
            customer11.TravelAgentId = 1;
            customer11.GroupId = 0;
            customer11.BoardCode = "HB";
            customer11.Room = "318";
            customer11.RoomTypeId = 1;
            customer11.BookedRoomTypeId = 1;
            customer11.NationalityCode = "GR";
            customer11.VIP = "-1";
            customer11.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer12 = new CustomerModel();
            customer12.ProfileNo = 3760;
            customer12.Adults = 1;
            customer12.Children = 0;
            customer12.TravelAgentId = 1;
            customer12.GroupId = 0;
            customer12.BoardCode = "FB";
            customer12.Room = "340";
            customer12.RoomTypeId = 1;
            customer12.BookedRoomTypeId = 1;
            customer12.NationalityCode = "GR";
            customer12.VIP = "-1";
            customer12.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer13 = new CustomerModel();
            customer13.ProfileNo = 3761;
            customer13.Adults = 2;
            customer13.Children = 0;
            customer13.TravelAgentId = 1;
            customer13.GroupId = 0;
            customer13.BoardCode = "FB";
            customer13.Room = "341";
            customer13.RoomTypeId = 1;
            customer13.BookedRoomTypeId = 1;
            customer13.NationalityCode = "GR";
            customer13.VIP = "-1";
            customer13.SourceId = 0;
            customer0.ReservationId = 59290;

            CustomerModel customer14 = new CustomerModel();
            customer14.ProfileNo = 3762;
            customer14.Adults = 1;
            customer14.Children = 1;
            customer14.TravelAgentId = 1;
            customer14.GroupId = 0;
            customer14.BoardCode = "FB";
            customer14.Room = "342";
            customer14.RoomTypeId = 1;
            customer14.BookedRoomTypeId = 1;
            customer14.NationalityCode = "GR";
            customer14.VIP = "-1";
            customer14.SourceId = 0;
            customer0.ReservationId = 59290;

            customers.Add(customer0);
            customers.Add(customer1);
            customers.Add(customer2);
            customers.Add(customer3);
            customers.Add(customer4);
            customers.Add(customer5);
            customers.Add(customer6);
            customers.Add(customer7);
            customers.Add(customer8);
            customers.Add(customer9);
            customers.Add(customer10);
            customers.Add(customer11);
            customers.Add(customer12);
            customers.Add(customer13);
            customers.Add(customer14);
        }

        [Test, Order(1)]
        public void Evaluate_Macro_Result_TimezoneExpression1()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("29-01-2016 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("29-01-2016 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("29-01-2016 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                Dictionary<string, AllowanceModel> results = allowanceFlow.GetAllowance(store, helpers[0], customers[0], dt, ovride);
                AllowanceModel result = results.First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(result.adultRemainingAllowanceForTimezone == 1 && result.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(result.adultRemainingAllowanceForTimezone == 0 && result.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(result.adultRemainingAllowanceForTimezone == 0 && result.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(2)]
        public void Evaluate_Macro_Result_TimezoneExpression2()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[1], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(3)]
        public void Evaluate_Macro_Result_TimezoneExpression3()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[2], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(4)]
        public void Evaluate_Macro_Result_TimezoneExpression4()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[3], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(5)]
        public void Evaluate_Macro_Result_TimezoneExpression5()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[4], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(6)]
        public void Evaluate_Macro_Result_TimezoneExpression6()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[5], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(7)]
        public void Evaluate_Macro_Result_TimezoneExpression7()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[6], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(8)]
        public void Evaluate_Macro_Result_TimezoneExpression8()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[7], dt, ovride).First().Value; 
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 2 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 2 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(9)]
        public void Evaluate_Macro_Result_TimezoneExpression9()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[8], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(10)]
        public void Evaluate_Macro_Result_TimezoneExpression10()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach ( DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[9], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 2 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(11)]
        public void Evaluate_Macro_Result_TimezoneExpression11()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[10], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 1);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(12)]
        public void Evaluate_Macro_Result_TimezoneExpression12()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[11], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 1);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 1);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 1);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(13)]
        public void Evaluate_Macro_Result_TimezoneExpression13()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[12], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(14)]
        public void Evaluate_Macro_Result_TimezoneExpression14()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[13], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 2 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }

        [Test, Order(15)]
        public void Evaluate_Macro_Result_TimezoneExpression15()
        {
            List<DateTime?> dtList = new List<DateTime?>();
            dtList.Add(Convert.ToDateTime("14-01-2020 09:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 13:35")); // SUCCESS
            dtList.Add(Convert.ToDateTime("14-01-2020 21:35")); // SUCCESS
            foreach (DateTime? dt in dtList)
            {
                AllowanceModel results = allowanceFlow.GetAllowance(store, helpers[0], customers[14], dt, ovride).First().Value;
                DateTime processedDateTime = Convert.ToDateTime(dt);
                if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 700 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1000)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1200 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 1600)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 0 && results.childRemainingAllowanceForTimezone == 0);
                }
                else if (Int32.Parse(processedDateTime.ToString(("HHmm"))) >= 1600 && Int32.Parse(processedDateTime.ToString(("HHmm"))) <= 2200)
                {
                    Assert.True(results.adultRemainingAllowanceForTimezone == 1 && results.childRemainingAllowanceForTimezone == 1);
                }
                else
                {
                    Assert.Null(results);
                }
            }
        }
    }
}
