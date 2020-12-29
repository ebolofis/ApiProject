using Autofac;
using NUnit.Framework;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess;
using Symposium.WebApi.DataAccess.DAOs;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.DataAccess.DT;
using Symposium.WebApi.DataAccess.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Tasks;
using Symposium.WebApi.MainLogic.Flows;
using Symposium_Test.MockupObjs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.MainLogic.PublicMembers.MockUpTests
{
    [TestFixture]
    class EndOfday_MockupTest //: IoCSupportedTest<TestModule>//<-- the module to register
    {
        IEndOfDayFlows endOfDay;
        DBInfoModel store = new DBInfoModel();
        long posInfo = 1;

        IContainer container;

        [OneTimeSetUp]
        public void Init()
        {

            //1. Register AutoMapper
            AutoMapperConfig.RegisterMappings();

            //2. Register AutoFac
            ContainerBuilder builder = new ContainerBuilder();
            
            //Register MainLogic classes 
            builder.RegisterType<EndOfDayFlows>().As<IEndOfDayFlows>();
            builder.RegisterType<EndOfDayTasks>().As<IEndOfDayTasks>();
            builder.RegisterType<AccountTasks>().As<IAccountTasks>();

            //Register Helper classes 
            builder.RegisterType<CustomJsonDeserializers>().As<ICustomJsonDeserializers>();
            builder.RegisterType<CustomJsonSerializers>().As<ICustomJsonSerializers>();
            builder.RegisterGeneric(typeof(PaginationHelper<>)).As(typeof(IPaginationHelper<>)).InstancePerLifetimeScope();

            //Register DataAccess classes 
            builder.RegisterType<UsersToDatabasesXML>().As<IUsersToDatabasesXML>();
            builder.RegisterType<EndOfDayDT>().As<IEndOfDayDT>();
            builder.RegisterType<AccountsDT>().As<IAccountsDT>();
            builder.RegisterType<PosInfoDT>().As<IPosInfoDT>();
            builder.RegisterType<LockersDT>().As<ILockersDT>();
            builder.RegisterType<InvoicesDT>().As<IInvoicesDT>();
            builder.RegisterType<InvoicesDAO>().As<IInvoicesDAO>();
            builder.RegisterGeneric(typeof(GenericITableDAO<>)).As(typeof(IGenericITableDAO<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericDAOMock<>)).As(typeof(IGenericDAO<>)).InstancePerLifetimeScope();//<---- Register the mockup class GenericDAOMock -- insteed of GenericDAO ---<<<<<<<
            builder.RegisterType<EndOfDayDaoMock>().As<IEndOfDayDAO>();          //<---- Register the mockup class EndOfDayDaoMock -- insteed of EndOfDayDAO ---<<<<<<<

            container = builder.Build();
            //Resolve EndOfDay from DI container
            endOfDay = container.Resolve<IEndOfDayFlows>();

        }

        [Test, Order(1)]
        public void EndOfDay_GetPreviewMockup()
        {
            string customerClassType = "";
            EndOfDayPreviewModel results = endOfDay.GetPreview(posInfo, store, customerClassType);

            Assert.NotNull(results);

            //Test Totals
            Assert.True(results.Totals.Count == 14);

            Assert.True(results.Totals[0].Id == EndOfDayReceiptTypes.Default);
            Assert.True(results.Totals[0].Description == "Cash");
            Assert.True(results.Totals[0].ReceiptCount == 15);
            Assert.True(results.Totals[0].Amount == 221.5M);
            Assert.True(results.Totals[0].AccountId == 1);
            Assert.True(results.Totals[0].AccountType == 1);

            Assert.True(results.Totals[1].Id == EndOfDayReceiptTypes.Default);
            Assert.True(results.Totals[1].Description == "Room Charge-C.C");
            Assert.True(results.Totals[1].ReceiptCount == 14);
            Assert.True(results.Totals[1].Amount == 22.678M);
            Assert.True(results.Totals[1].AccountId == 3);
            Assert.True(results.Totals[1].AccountType == 3);

            Assert.True(results.Totals[13].Id == EndOfDayReceiptTypes.TotalToReturn);
            Assert.True(results.Totals[13].Description == "Total Return");
            Assert.True(results.Totals[13].ReceiptCount == 2);
            Assert.True(results.Totals[13].Amount == 9.12M);
            Assert.True(results.Totals[13].AccountId == 0);
            Assert.True(results.Totals[13].AccountType == 0);

            //Test TotalsByStaff
            Assert.True(results.TotalsByStaff.Count == 2);//there are 2 staffs
            Assert.True(results.TotalsByStaff[0].Count == 14);//the 1st staff: 14 records
            Assert.True(results.TotalsByStaff[1].Count == 2);//the 2nd staff: 2 records

            Assert.True(results.TotalsByStaff[0][0].Id == EndOfDayReceiptTypes.Default);
            Assert.True(results.TotalsByStaff[0][0].Description == "Cash");
            Assert.True(results.TotalsByStaff[0][0].ReceiptCount == 12);
            Assert.True(results.TotalsByStaff[0][0].Amount == 22.5M);
            Assert.True(results.TotalsByStaff[0][0].AccountId == 1);
            Assert.True(results.TotalsByStaff[0][0].AccountType == 1);
            Assert.True(results.TotalsByStaff[0][0].StaffId == 1);
            Assert.True(results.TotalsByStaff[0][0].StaffName == "staff1");

            Assert.True(results.TotalsByStaff[1][0].Id == EndOfDayReceiptTypes.Default);
            Assert.True(results.TotalsByStaff[1][0].Description == "Cash");
            Assert.True(results.TotalsByStaff[1][0].ReceiptCount == 5);
            Assert.True(results.TotalsByStaff[1][0].Amount == 51.5M);
            Assert.True(results.TotalsByStaff[1][0].AccountId == 1);
            Assert.True(results.TotalsByStaff[1][0].AccountType == 1);
            Assert.True(results.TotalsByStaff[1][0].StaffId == 2);
            Assert.True(results.TotalsByStaff[1][0].StaffName == "staff2");
        }

        [Test, Order(2)]
        public void EndOfDay_GetAnalysis_Default_ShouldReturnThe2ndPage()
        {
            int page = 2;
            int pageLenght = 10;
            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, page, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.Default);

            Assert.That<int>(results.ItemsCount, Is.EqualTo(21));
            Assert.That<int>(results.PageLength, Is.EqualTo(10));
            Assert.That<int>(results.PagesCount, Is.EqualTo(3));

            Assert.That<long>(results.PageList[0].Id, Is.EqualTo(11));
            Assert.That<string>(results.PageList[0].Abbreviation, Is.EqualTo("ΑΛΣ"));
            Assert.That<string>(results.PageList[0].StaffName, Is.EqualTo("Sabi Sabi"));
            Assert.That<string>(results.PageList[0].Description, Is.EqualTo("Beach club 2"));
            Assert.That<int>(results.PageList[0].Day.Minute, Is.EqualTo(24));
            Assert.That<int>(results.PageList[0].Day.Day, Is.EqualTo(22));
            Assert.That<decimal>(results.PageList[0].Total, Is.EqualTo(14.50));
            Assert.That<string>(results.PageList[8].Room.Trim(), Is.EqualTo("104")); 
            Assert.That<long>(results.PageList[4].ReceiptNo, Is.EqualTo(2365));
            Assert.That<string>(results.PageList[2].OrderNo.Trim(), Is.EqualTo("1493"));
        }


        [Test, Order(3)]
        public void EndOfDay_GetAnalysis_Canceled_ShouldReturnThe3rdPage()
        {
            int currpage = 3;
            int pageLenght = 10;

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.Canceled);
            Assert.That<int>(results.ItemsCount, Is.EqualTo(21));
            Assert.That<int>(results.PageLength, Is.EqualTo(1));
            Assert.That<int>(results.PagesCount, Is.EqualTo(3));

            Assert.That<long>(results.PageList[0].Id, Is.EqualTo(21));
            Assert.That<string>(results.PageList[0].Abbreviation, Is.EqualTo("ΑΛΣ"));
            Assert.That<string>(results.PageList[0].StaffName, Is.EqualTo("Sabi Sabi"));
            Assert.That<string>(results.PageList[0].Description, Is.EqualTo("Beach club 3"));
            Assert.That<int>(results.PageList[0].Day.Minute, Is.EqualTo(15));
            Assert.That<int>(results.PageList[0].Day.Day, Is.EqualTo(23));
            Assert.That<decimal>(results.PageList[0].Total, Is.EqualTo(15));
            Assert.That<string>(results.PageList[0].Room.Trim(), Is.EqualTo(String.Empty));
            Assert.That<long>(results.PageList[0].ReceiptNo, Is.EqualTo(2380));
            Assert.That<string>(results.PageList[0].OrderNo.Trim(), Is.EqualTo("1496"));
        }

        [Test, Order(4)]
        public void EndOfDay_GetAnalysis_DiscountTotal_ShouldReturnThe2ndPage()
        {
            int currpage = 2;
            int pageLenght = 10;

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.DiscountTotal);
            Assert.That<int>(results.ItemsCount, Is.EqualTo(21));
            Assert.That<int>(results.PageLength, Is.EqualTo(10));
            Assert.That<int>(results.PagesCount, Is.EqualTo(3));

            Assert.That<long>(results.PageList[0].Id, Is.EqualTo(11));
            Assert.That<string>(results.PageList[0].Abbreviation, Is.EqualTo("ΑΛΣ"));
            Assert.That<string>(results.PageList[0].StaffName, Is.EqualTo("Sabi Sabi"));
            Assert.That<string>(results.PageList[0].Description, Is.EqualTo("Beach club 2"));
            Assert.That<int>(results.PageList[0].Day.Minute, Is.EqualTo(24));
            Assert.That<int>(results.PageList[0].Day.Day, Is.EqualTo(22));
            Assert.That<decimal>(results.PageList[0].Total, Is.EqualTo(14.50));
            Assert.That<string>(results.PageList[8].Room.Trim(), Is.EqualTo("104"));
            Assert.That<long>(results.PageList[4].ReceiptNo, Is.EqualTo(2365));
            Assert.That<string>(results.PageList[2].OrderNo.Trim(), Is.EqualTo("1493"));
        }


        [Test, Order(5)]
        public void EndOfDay_GetAnalysis_NotInvoiced_ShouldReturnThe2ndPage()
        {
            int currpage = 2;
            int pageLenght = 10;

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.NotInvoiced);
            Assert.That<int>(results.ItemsCount, Is.EqualTo(21));
            Assert.That<int>(results.PageLength, Is.EqualTo(10));
            Assert.That<int>(results.PagesCount, Is.EqualTo(3));

            Assert.That<long>(results.PageList[0].Id, Is.EqualTo(11));
            Assert.That<string>(results.PageList[0].Abbreviation, Is.EqualTo("ΑΛΣ"));
            Assert.That<string>(results.PageList[0].StaffName, Is.EqualTo("Sabi Sabi"));
            Assert.That<string>(results.PageList[0].Description, Is.EqualTo("Beach club 2"));
            Assert.That<int>(results.PageList[0].Day.Minute, Is.EqualTo(24));
            Assert.That<int>(results.PageList[0].Day.Day, Is.EqualTo(22));
            Assert.That<decimal>(results.PageList[0].Total, Is.EqualTo(14.50));
            Assert.That<string>(results.PageList[8].Room.Trim(), Is.EqualTo("104"));
            Assert.That<long>(results.PageList[4].ReceiptNo, Is.EqualTo(2365));
            Assert.That<string>(results.PageList[2].OrderNo.Trim(), Is.EqualTo("1493"));
        }


        [Test, Order(6)]
        public void EndOfDay_GetAnalysis_NotPaid_ShouldReturnThe2ndPage()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned
            List<int> pageLenghts = new List<int>() { 0, pageLenght };//the length of current page. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.NotPaid);
            Assert.That<int>(results.ItemsCount, Is.EqualTo(21));
            Assert.That<int>(results.PageLength, Is.EqualTo(10));
            Assert.That<int>(results.PagesCount, Is.EqualTo(3));

            Assert.That<long>(results.PageList[0].Id, Is.EqualTo(11));
            Assert.That<string>(results.PageList[0].Abbreviation, Is.EqualTo("ΑΛΣ"));
            Assert.That<string>(results.PageList[0].StaffName, Is.EqualTo("Sabi Sabi"));
            Assert.That<string>(results.PageList[0].Description, Is.EqualTo("Beach club 2"));
            Assert.That<int>(results.PageList[0].Day.Minute, Is.EqualTo(24));
            Assert.That<int>(results.PageList[0].Day.Day, Is.EqualTo(22));
            Assert.That<decimal>(results.PageList[0].Total, Is.EqualTo(14.50));
            Assert.That<string>(results.PageList[8].Room.Trim(), Is.EqualTo("104"));
            Assert.That<long>(results.PageList[4].ReceiptNo, Is.EqualTo(2365));
            Assert.That<string>(results.PageList[2].OrderNo.Trim(), Is.EqualTo("1493"));
        }

        [Test, Order(7)]
        public void EndOfDay_GetAnalysis_NotPrinted_ShouldReturnThe2ndPage()
        {
            int currpage = 2;
            int pageLenght = 10;

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.NotPrinted);
            Assert.That<int>(results.ItemsCount, Is.EqualTo(21));
            Assert.That<int>(results.PageLength, Is.EqualTo(10));
            Assert.That<int>(results.PagesCount, Is.EqualTo(3));

            Assert.That<long>(results.PageList[0].Id, Is.EqualTo(11));
            Assert.That<string>(results.PageList[0].Abbreviation, Is.EqualTo("ΑΛΣ"));
            Assert.That<string>(results.PageList[0].StaffName, Is.EqualTo("Sabi Sabi"));
            Assert.That<string>(results.PageList[0].Description, Is.EqualTo("Beach club 2"));
            Assert.That<int>(results.PageList[0].Day.Minute, Is.EqualTo(24));
            Assert.That<int>(results.PageList[0].Day.Day, Is.EqualTo(22));
            Assert.That<decimal>(results.PageList[0].Total, Is.EqualTo(14.50));
            Assert.That<string>(results.PageList[8].Room.Trim(), Is.EqualTo("104"));
            Assert.That<long>(results.PageList[4].ReceiptNo, Is.EqualTo(2365));
            Assert.That<string>(results.PageList[2].OrderNo.Trim(), Is.EqualTo("1493"));
        }

        [Test, Order(8)]
        public void EndOfDay_GetAnalysis_ReceiptTotal_ShouldReturnThe2ndPage()
        {
            int currpage = 2;
            int pageLenght = 10;

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.ReceiptTotal);
            Assert.That<int>(results.ItemsCount, Is.EqualTo(21));
            Assert.That<int>(results.PageLength, Is.EqualTo(10));
            Assert.That<int>(results.PagesCount, Is.EqualTo(3));

            Assert.That<long>(results.PageList[0].Id, Is.EqualTo(11));
            Assert.That<string>(results.PageList[0].Abbreviation, Is.EqualTo("ΑΛΣ"));
            Assert.That<string>(results.PageList[0].StaffName, Is.EqualTo("Sabi Sabi"));
            Assert.That<string>(results.PageList[0].Description, Is.EqualTo("Beach club 2"));
            Assert.That<int>(results.PageList[0].Day.Minute, Is.EqualTo(24));
            Assert.That<int>(results.PageList[0].Day.Day, Is.EqualTo(22));
            Assert.That<decimal>(results.PageList[0].Total, Is.EqualTo(14.50));
            Assert.That<string>(results.PageList[8].Room.Trim(), Is.EqualTo("104"));
            Assert.That<long>(results.PageList[4].ReceiptNo, Is.EqualTo(2365));
            Assert.That<string>(results.PageList[2].OrderNo.Trim(), Is.EqualTo("1493"));
        }


        [Test, Order(8)]
        public void GetTotalReportAnalysisByAccount_ShouldReturn8Accounts()
        {
            EodTotalReportAnalysisModel results = endOfDay.GetTotalReportAnalysisByAccount(store, posInfo, 0);
            Assert.That<int>(results.ReceiptsPerAccount.Count, Is.EqualTo(8));

            Assert.That<long>(results.ReceiptsPerAccount[0].AccountId, Is.EqualTo(1));
            Assert.That<string>(results.ReceiptsPerAccount[0].Account, Is.EqualTo("Cash"));
            Assert.That<int>(results.ReceiptsPerAccount[0].Count, Is.EqualTo(18));
            Assert.That<decimal>(results.ReceiptsPerAccount[0].Total, Is.EqualTo(285.5));

            Assert.That<long>(results.ReceiptsPerAccount[1].AccountId, Is.EqualTo(2));
            Assert.That<string>(results.ReceiptsPerAccount[1].Account, Is.EqualTo("Complimentary"));
            Assert.That<int>(results.ReceiptsPerAccount[1].Count, Is.EqualTo(0));
            Assert.That<decimal>(results.ReceiptsPerAccount[1].Total, Is.EqualTo(0));

            Assert.That<long>(results.ReceiptsPerAccount[7].AccountId, Is.EqualTo(0));
            Assert.That<string>(results.ReceiptsPerAccount[7].Account, Is.EqualTo("Not Paid"));
            Assert.That<int>(results.ReceiptsPerAccount[7].Count, Is.EqualTo(3));
            Assert.That<decimal>(results.ReceiptsPerAccount[7].Total, Is.EqualTo(51.5));
        }


        [OneTimeTearDown]
        public void TestTearDown()
        {

        }

    }
}
