using NUnit.Framework;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
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
    class EndOfday_IntegrationTest : IoCSupportedTest<TestModule>//<-- the module to register
    {
        DBInfoModel store;
        long posInfo;
        IEndOfDayFlows endOfDay;
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericITableDAO<AccountDTO> accountTableDao;
        IGenericITableDAO<DepartmentDTO> departmentTableDao;
        IGenericITableDAO<PosInfoDTO> posinfoTableDao;
        IGenericITableDAO<InvoiceTypesDTO> invoicetypesTableDao;
        IGenericITableDAO<PosInfoDetailDTO> posinfodetailsTableDao;
        IGenericITableDAO<StaffDTO> staffTableDao;
        IGenericITableDAO<EndOfDayDTO> endOFDayDao;
        IGenericITableDAO<NFCcardDTO> nfcCardDao;
        IGenericDAO<NFCcardDTO> nfcCardDao2;
        IGenericITableDAO<VatDTO> vatDao;
        IGenericITableDAO<UnitsDTO> unitDao;
        IGenericITableDAO<KdsDTO> kdsDao;
        IGenericITableDAO<KitchenDTO> kitchenDao;
        IGenericITableDAO<KitchenRegionDTO> kitchenregionDao;
        public EndOfday_IntegrationTest()
        {
            usersToDatabases = Resolve<IUsersToDatabasesXML>();
            accountTableDao = Resolve<IGenericITableDAO<AccountDTO>>();
            departmentTableDao = Resolve<IGenericITableDAO<DepartmentDTO>>();
            posinfoTableDao = Resolve<IGenericITableDAO<PosInfoDTO>>();
            invoicetypesTableDao = Resolve<IGenericITableDAO<InvoiceTypesDTO>>();
            posinfodetailsTableDao = Resolve<IGenericITableDAO<PosInfoDetailDTO>>();
            staffTableDao = Resolve<IGenericITableDAO<StaffDTO>>();
            endOFDayDao = Resolve<IGenericITableDAO<EndOfDayDTO>>();
            nfcCardDao = Resolve<IGenericITableDAO<NFCcardDTO>>();
            nfcCardDao2 = Resolve<IGenericDAO<NFCcardDTO>>();
            vatDao = Resolve<IGenericITableDAO<VatDTO>>();
            unitDao = Resolve<IGenericITableDAO<UnitsDTO>>();
            kdsDao = Resolve<IGenericITableDAO<KdsDTO>>();
            kitchenDao = Resolve<IGenericITableDAO<KitchenDTO>>();
            kitchenregionDao = Resolve<IGenericITableDAO<KitchenRegionDTO>>();
            endOfDay = Resolve<IEndOfDayFlows>();//IoC
        }

       [OneTimeSetUp]
        public void Init()
        {
            //Create Store from config file 
            string storeJson = Properties.Settings.Default.Store_NikkiBeach;
            CustomJsonDeserializers jdes = new CustomJsonDeserializers();
            store  = jdes.JsonToStore(storeJson);
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            posInfo = Properties.Settings.Default.PosInfo;
        }


        [Test, Order(1)]
        public void EndOfDay_GetPreviewResultsShouldBeNotNull()
        {
            string customerClassType = "";
            EndOfDayPreviewModel results = endOfDay.GetPreview(posInfo, store, customerClassType);
            Assert.NotNull(results);
            Assert.NotNull(results.Totals);
            Assert.True(results.Totals.Count>=7);
            Assert.NotNull(results.TotalsByStaff);
        }


        [Test, Order(2)]
        public void EndOfDay_GetAnalysis_Canceled_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned
          
            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store,posInfo,0,0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.Canceled);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(3)]
        public void EndOfDay_GetAnalysis_Default_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.Default);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(4)]
        public void EndOfDay_GetAnalysis_DiscountTotal_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.DiscountTotal);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(5)]
        public void EndOfDay_GetAnalysis_NotInvoiced_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.NotInvoiced);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(6)]
        public void EndOfDay_GetAnalysis_NotPaid_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned
            List<int> pageLenghts = new List<int>() { 0, pageLenght };//the length of current page. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.NotPaid);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(7)]
        public void EndOfDay_GetAnalysis_NotPrinted_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.NotPrinted);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(8)]
        public void EndOfDay_GetAnalysis_ReceiptTotal_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.ReceiptTotal);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(9)]
        public void EndOfDay_GetAnalysis_TotalToReturn_ResultsShouldBeNotNull()
        {
            int currpage = 2;
            int pageLenght = 10;
            List<int> currPages = new List<int>() { 0, currpage };//the list of possible current pages. 0 if no data reruned

            PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(store, posInfo, 0, 0, currpage, pageLenght, Symposium.Models.Enums.EndOfDayReceiptTypes.TotalToReturn);
            Assert.NotNull(results);
            Assert.NotNull(results.PageList);
            CollectionAssert.Contains(currPages, results.CurrentPage);
            Assert.That(results.PageLength, Is.LessThanOrEqualTo(pageLenght));
        }

        [Test, Order(10)]
        public void EndOfDay_GetTotalReportAnalysisByAccount_ResultsShouldBeNotNull()
        {
            EodTotalReportAnalysisModel results = endOfDay.GetTotalReportAnalysisByAccount(store, posInfo, 0);
            Assert.NotNull(results);
            Assert.NotNull(results.ReceiptsPerAccount);
            Assert.That(results.ReceiptsPerAccount.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test, Order(11)]
        public void EndOfDay_GetTotalReportAnalysisByAccount_WithNullStore_ShouldReturnException()
        {
            Assert.Throws<Exception>(() => endOfDay.GetTotalReportAnalysisByAccount(null, posInfo, 0));
        }

        [Test, Order(12)]
        public void EndOfDay_GetXReportForExtecr_ResultsShouldBeNotNull()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishEndOFDayXZReportsEnvironment(db);
                    DateTime value = new DateTime(2017, 1, 18);
                    EodXAndZTotalsForExtecr results = endOfDay.EodXAndZ(store, environment.PosInfo.Id, environment.Staff.Id, null, value, EndOfDayActions.PrintX);
                    Assert.NotNull(results);
                }
            }
        }

        [Test, Order(13)]
        public void EndOfDay_GetZReportForExtecr_ResultsShouldBeNotNull()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishEndOFDayXZReportsEnvironment(db);
                    DateTime value = new DateTime(2017, 1, 18);
                    EodXAndZTotalsForExtecr results = endOfDay.EodXAndZ(store, environment.PosInfo.Id, environment.Staff.Id, null, value, EndOfDayActions.PrintZ);
                    Assert.NotNull(results);
                }
            }
        }

        [Test, Order(14)]
        public void EndOfDay_ReprintZReportForExtecr_1_ResultsShouldBeNotNull()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishEndOFDayXZReportsEnvironment(db);
                    DateTime value = new DateTime(2017, 1, 18);
                    Assert.Throws<BusinessException>(() => endOfDay.EodXAndZ(store, environment.PosInfo.Id, environment.Staff.Id, null, value, EndOfDayActions.ReprintZ));
                }
            }
        }

        [Test, Order(15)]
        public void EndOfDay_ReprintZReportForExtecr_2_ResultsShouldBeNotNull()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishEndOFDayXZReportsEnvironment(db);
                    DateTime value = new DateTime(2017, 1, 18);
                    EodXAndZTotalsForExtecr results = endOfDay.EodXAndZ(store, environment.PosInfo.Id, environment.Staff.Id, environment.EndOFDay.Id, value, EndOfDayActions.ReprintZ);
                    Assert.NotNull(results);
                }
            }
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            
        }
    }
}
