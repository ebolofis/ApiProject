using CsvHelper;
using CsvHelper.TypeConversion;
using NUnit.Framework;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium_DTOs.PosModel_Info;
using Symposium_Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium_Test.MainLogic.PublicMembers.IntegrationTests
{
    [TestFixture]
    class CancelReceipt_IntegrationTest : IoCSupportedTest<TestModule>
    {

        DBInfoModel store;
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IInvoicesFlows invoices;
        IGenericITableDAO<AccountDTO> accountTableDao;
        IGenericITableDAO<DepartmentDTO> departmentTableDao;
        IGenericITableDAO<PosInfoDTO> posinfoTableDao;
        IGenericITableDAO<InvoiceTypesDTO> invoicetypesTableDao;
        IGenericITableDAO<PosInfoDetailDTO> posinfodetailsTableDao;
        IGenericITableDAO<StaffDTO> staffTableDao;
        IGenericITableDAO<InvoicesDTO> invoicesTableDao;
        IGenericITableDAO<EndOfDayDTO> endOFDayDao;
        IGenericITableDAO<NFCcardDTO> nfcCardDao;
        IGenericDAO<NFCcardDTO> nfcCardDao2;
        IGenericITableDAO<VatDTO> vatDao;
        IGenericITableDAO<UnitsDTO> unitDao;
        IGenericITableDAO<KdsDTO> kdsDao;
        IGenericITableDAO<KitchenDTO> kitchenDao;
        IGenericITableDAO<KitchenRegionDTO> kitchenregionDao;
        IGenericITableDAO<OrderDetailInvoicesDTO> orderDetailInvoicesTableDao;
        IGenericITableDAO<OrderDTO> orderTableDao;
        IGenericITableDAO<OrderDetailDTO> orderDetailTableDao;
        IGenericITableDAO<TransactionsDTO> transactionsTableDao;
        IGenericITableDAO<TransferToPmsDTO> transferToPmsTableDao;
        IGenericITableDAO<OrderStatusDTO> orderStatusTableDao;
        IGenericDAO<InvoiceTypesDTO> invoiceTypesDao;
        IGenericDAO<TransactionsDTO> transactionsDao;
        IGenericDAO<OrderDetailInvoicesDTO> orderDetailInvoicesDao;
        IGenericDAO<InvoicesDTO> invoicesDao;

        public CancelReceipt_IntegrationTest()
        {
            usersToDatabases = Resolve<IUsersToDatabasesXML>();
            invoices = Resolve<IInvoicesFlows>();
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
            invoicesTableDao = Resolve<IGenericITableDAO<InvoicesDTO>>();
            invoiceTypesDao = Resolve<IGenericDAO<InvoiceTypesDTO>>();
            orderDetailInvoicesTableDao = Resolve<IGenericITableDAO<OrderDetailInvoicesDTO>>();
            orderTableDao = Resolve<IGenericITableDAO<OrderDTO>>();
            orderDetailTableDao = Resolve<IGenericITableDAO<OrderDetailDTO>>();
            transactionsTableDao = Resolve<IGenericITableDAO<TransactionsDTO>>();
            transferToPmsTableDao = Resolve<IGenericITableDAO<TransferToPmsDTO>>();
            orderStatusTableDao = Resolve<IGenericITableDAO<OrderStatusDTO>>();
            transactionsDao = Resolve<IGenericDAO<TransactionsDTO>>();
            orderDetailInvoicesDao = Resolve<IGenericDAO<OrderDetailInvoicesDTO>>();
            invoicesDao = Resolve<IGenericDAO<InvoicesDTO>>();
        }

        [OneTimeSetUp]
        public void Init()
        {
            //1. Register AutoMapper
            AutoMapperConfig.RegisterMappings();
            //2. Create Store from config file 
            string storeJson = Properties.Settings.Default.Store_NikkiBeach;
            CustomJsonDeserializers jdes = new CustomJsonDeserializers();
            store = jdes.JsonToStore(storeJson);
            connectionString = usersToDatabases.ConfigureConnectionString(store);
        }

        [Test, Order(1)]
        public void CancelNotPrintedReceipt()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishReceiptCancelationEnvironment(db);

                    List<InvoicesDTO> invoicesCSV = getInvoices();

                    invoicesCSV[0].Id = 0;
                    invoicesCSV[0].InvoiceTypeId = environment.InvoiceTypes[0].Id;
                    invoicesCSV[0].IsPrinted = false;
                    invoicesCSV[0].StaffId = environment.Staff.Id;
                    invoicesCSV[0].PosInfoId = environment.PosInfo.Id;
                    invoicesCSV[0].PosInfoDetailId = environment.PosInfoDetails[0].Id;
                    invoicesTableDao.Upsert(db, invoicesCSV[0]);

                    Assert.Throws<BusinessException>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                }
            }
        }

        [Test, Order(2)]
        public void CancelNotPaidReceipt()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishReceiptCancelationEnvironment(db);

                    List<InvoicesDTO> invoicesCSV = getInvoices();

                    for (int i = 0; i < 2; i++)
                    {
                        invoicesCSV[0].Id = 0;
                        invoicesCSV[0].InvoiceTypeId = environment.InvoiceTypes[0].Id;
                        invoicesCSV[0].IsPaid = Convert.ToInt16(i);
                        invoicesCSV[0].StaffId = environment.Staff.Id;
                        invoicesCSV[0].PosInfoId = environment.PosInfo.Id;
                        invoicesCSV[0].PosInfoDetailId = environment.PosInfoDetails[0].Id;
                        invoicesTableDao.Upsert(db, invoicesCSV[0]);

                        Assert.Throws<BusinessException>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                    }
                }
            }
        }

        [Test, Order(3)]
        public void CancelVoidedReceipt()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishReceiptCancelationEnvironment(db);

                    List<InvoicesDTO> invoicesCSV = getInvoices();

                    invoicesCSV[0].Id = 0;
                    invoicesCSV[0].InvoiceTypeId = environment.InvoiceTypes[0].Id;
                    invoicesCSV[0].IsVoided = true;
                    invoicesCSV[0].StaffId = environment.Staff.Id;
                    invoicesCSV[0].PosInfoId = environment.PosInfo.Id;
                    invoicesCSV[0].PosInfoDetailId = environment.PosInfoDetails[0].Id;
                    invoicesTableDao.Upsert(db, invoicesCSV[0]);

                    Assert.Throws<BusinessException>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                }
            }
        }

        [Test, Order(4)]
        public void CancelDeletedReceipt()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishReceiptCancelationEnvironment(db);

                    List<InvoicesDTO> invoicesCSV = getInvoices();

                    invoicesCSV[0].Id = 0;
                    invoicesCSV[0].InvoiceTypeId = environment.InvoiceTypes[0].Id;
                    invoicesCSV[0].IsDeleted = true;
                    invoicesCSV[0].StaffId = environment.Staff.Id;
                    invoicesCSV[0].PosInfoId = environment.PosInfo.Id;
                    invoicesCSV[0].PosInfoDetailId = environment.PosInfoDetails[0].Id;
                    invoicesTableDao.Upsert(db, invoicesCSV[0]);

                    Assert.Throws<BusinessException>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                }
            }
        }

        [Test, Order(5)]
        public void CancelInvoiceTypeReceipt()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishReceiptCancelationEnvironment(db);

                    List<InvoicesDTO> invoicesCSV = getInvoices();

                    invoicesCSV[0].Id = 0;
                    invoicesCSV[0].InvoiceTypeId = environment.InvoiceTypes[0].Id;
                    invoicesCSV[0].StaffId = environment.Staff.Id;
                    invoicesCSV[0].PosInfoId = environment.PosInfo.Id;
                    invoicesCSV[0].PosInfoDetailId = environment.PosInfoDetails[0].Id;
                    invoicesTableDao.Upsert(db, invoicesCSV[0]);

                    List<InvoiceTypesDTO> invoicesTypes;
                    invoicesTypes = invoiceTypesDao.Select("select *from InvoiceTypes WHERE Id = @id", new { id = environment.InvoiceTypes[0].Id }, db);

                    invoicesTypes[0].Type = 2;
                    invoiceTypesDao.Update(db, invoicesTypes[0]);
                    Assert.Throws<BusinessException>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                    invoicesTypes[0].Type = 3;
                    invoiceTypesDao.Update(db, invoicesTypes[0]);
                    Assert.Throws<BusinessException>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                    invoicesTypes[0].Type = 8;
                    invoiceTypesDao.Update(db, invoicesTypes[0]);
                    Assert.Throws<BusinessException>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                }
            }
        }

        [Test, Order(6)]
        public void CancelInvoiceIdReceipt()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishReceiptCancelationEnvironment(db);

                    List<InvoicesDTO> invoicesCSV = getInvoices();
                    invoicesCSV[0].Id = 0;

                    Assert.Throws<Exception>(() => invoices.CancelReceipt(store, invoicesCSV[0].Id, environment.PosInfo.Id, environment.Staff.Id, false));
                }
            }
        }

        [Test, Order(7)]
        public void CancelReceiptSuccess()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    //EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao);
                    //PosEnvironment environment = establish.EstablishReceiptCancelationEnvironment(db);

                    List<InvoicesDTO> newInvoices;
                    List<OrderDetailInvoicesDTO> orderDetailInvoices;
                    SignalRAfterInvoiceModel cancelInvoices;
                    List<TransactionsDTO> transactions;

                    List<InvoicesDTO> invoicesCSV = getInvoices();
                    List<OrderDetailInvoicesDTO> orderDetailInvoicesCSV = getOrderDetailInvoices();
                    List<OrderDTO> orderCSV = getOrder();
                    List<OrderDetailDTO> orderDetailCSV = getOrderDetail();
                    List<TransactionsDTO> transactionsCSV = getTransactions();
                    List<TransferToPmsDTO> transferToPmsCSV = getTransferToPms();
                    List<OrderStatusDTO> orderStatusCSV = getOrderStatus();

                    invoicesCSV[0].Id = 0;
                    orderDetailInvoicesCSV[0].Id = 0;
                    orderDetailInvoicesCSV[1].Id = 0;
                    orderDetailInvoicesCSV[2].Id = 0;
                    orderDetailInvoicesCSV[3].Id = 0;
                    orderDetailInvoicesCSV[4].Id = 0;
                    orderCSV[0].Id = 0;
                    orderDetailCSV[0].Id = 0;
                    orderDetailCSV[1].Id = 0;
                    orderDetailCSV[2].Id = 0;
                    transactionsCSV[0].Id = 0;
                    transactionsCSV[1].Id = 0;
                    transactionsCSV[2].Id = 0;
                    transferToPmsCSV[0].Id = 0;
                    orderStatusCSV[0].Id = 0;
                    invoicesTableDao.Upsert(db, invoicesCSV[0]);
                    orderDetailInvoicesCSV[0].InvoicesId = invoicesCSV[0].Id;
                    orderDetailInvoicesCSV[1].InvoicesId = invoicesCSV[0].Id;
                    orderDetailInvoicesCSV[2].InvoicesId = invoicesCSV[0].Id;
                    orderDetailInvoicesCSV[3].InvoicesId = invoicesCSV[0].Id;
                    orderDetailInvoicesCSV[4].InvoicesId = invoicesCSV[0].Id;
                    orderDetailInvoicesTableDao.Upsert(db, orderDetailInvoicesCSV[0]);
                    orderTableDao.Upsert(db, orderCSV[0]);
                    orderDetailCSV[0].OrderId = orderCSV[0].Id;
                    orderDetailCSV[1].OrderId = orderCSV[0].Id;
                    orderDetailCSV[2].OrderId = orderCSV[0].Id;
                    orderDetailTableDao.Upsert(db, orderDetailCSV[0]);
                    transactionsCSV[0].InvoicesId = invoicesCSV[0].Id;
                    transactionsCSV[1].InvoicesId = invoicesCSV[0].Id;
                    transactionsCSV[2].InvoicesId = invoicesCSV[0].Id;
                    transactionsTableDao.Upsert(db, transactionsCSV[0]);
                    transferToPmsTableDao.Upsert(db, transferToPmsCSV[0]);
                    orderStatusTableDao.Upsert(db, orderStatusCSV[0]);

                    long posInfoId = invoicesCSV[0].PosInfoId ?? 0;
                    long staffId = invoicesCSV[0].StaffId ?? 0;
                    cancelInvoices = invoices.CancelReceipt(store, invoicesCSV[0].Id, posInfoId, staffId, false);
                    newInvoices = invoicesDao.Select("select * from Invoices WHERE Id = @id", new { id = cancelInvoices.InvoiceId }, db);
                    orderDetailInvoices = orderDetailInvoicesDao.Select("select * from OrderDetailInvoices WHERE InvoicesId = @invoicesId", new { invoicesId = cancelInvoices.InvoiceId }, db);
                    transactions = transactionsDao.Select("select * from Transactions WHERE InvoicesId = @invoicesId", new { invoicesId = cancelInvoices.InvoiceId }, db);

                    Assert.That<bool?>(newInvoices[0].IsVoided, Is.EqualTo(true));
                    Assert.That<decimal?>(newInvoices[0].PaidTotal, Is.EqualTo(-invoicesCSV[0].PaidTotal));
                    Assert.That<int?>(orderDetailInvoices[0].InvoiceType, Is.EqualTo(3));
                    Assert.That<decimal?>(transactions[0].Amount, Is.EqualTo(-transactionsCSV[0].Amount));
                }
            }
        }

        private List<InvoicesDTO> getInvoices()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\invoices.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<InvoicesDTO>().ToList<InvoicesDTO>();
            }
        }

        private List<OrderDetailInvoicesDTO> getOrderDetailInvoices()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\orderDetailInvoices.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<OrderDetailInvoicesDTO>().ToList<OrderDetailInvoicesDTO>();
            }
        }

        private List<OrderDTO> getOrder()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\order.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<OrderDTO>().ToList<OrderDTO>();
            }
        }

        private List<OrderDetailDTO> getOrderDetail()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\orderDetail.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<OrderDetailDTO>().ToList<OrderDetailDTO>();
            }
        }

        private List<TransactionsDTO> getTransactions()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\transactions.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<TransactionsDTO>().ToList<TransactionsDTO>();
            }
        }

        private List<TransferToPmsDTO> getTransferToPms()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\transferToPms.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<TransferToPmsDTO>().ToList<TransferToPmsDTO>();
            }
        }

        private List<OrderStatusDTO> getOrderStatus()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\orderStatus.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss.fff"
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<OrderStatusDTO>().ToList<OrderStatusDTO>();
            }
        }
    }
}
