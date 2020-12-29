using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using Symposium_Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.MainLogic.PublicMembers.IntegrationTests
{
    public class EstablishTestEnvironment
    {
        IGenericITableDAO<AccountDTO> accountDao;
        IGenericITableDAO<DepartmentDTO> departmentDao;
        IGenericITableDAO<PosInfoDTO> posinfoDao;
        IGenericITableDAO<InvoiceTypesDTO> invoicetypesDao;
        IGenericITableDAO<PosInfoDetailDTO> posinfodetailDao;
        IGenericITableDAO<StaffDTO> staffDao;
        IGenericITableDAO<EndOfDayDTO> endOFDayDao;
        IGenericITableDAO<NFCcardDTO> nfcCardDao;
        IGenericDAO<NFCcardDTO> nfcCardDao2;
        IGenericITableDAO<VatDTO> vatDao;
        IGenericITableDAO<UnitsDTO> unitDao;
        IGenericITableDAO<KdsDTO> kdsDao;
        IGenericITableDAO<KitchenDTO> kitchenDao;
        IGenericITableDAO<KitchenRegionDTO> kitchenregionDao;

        public EstablishTestEnvironment(IGenericITableDAO<AccountDTO> accDao, IGenericITableDAO<DepartmentDTO> depDao, IGenericITableDAO<PosInfoDTO> posDao, IGenericITableDAO<InvoiceTypesDTO> invtDao, IGenericITableDAO<PosInfoDetailDTO> posdetDao, IGenericITableDAO<StaffDTO> stDao, IGenericITableDAO<EndOfDayDTO> eodDao, IGenericITableDAO<VatDTO> vDao, IGenericITableDAO<UnitsDTO> uDao, IGenericITableDAO<KdsDTO> KDSDao, IGenericITableDAO<KitchenDTO> kitchDao, IGenericITableDAO<KitchenRegionDTO> kitchrDao, IGenericITableDAO<NFCcardDTO> nfcDao, IGenericDAO<NFCcardDTO> nfcDao2)
        {
            accountDao = accDao;
            departmentDao = depDao;
            posinfoDao = posDao;
            invoicetypesDao = invtDao;
            posinfodetailDao = posdetDao;
            staffDao = stDao;
            endOFDayDao = eodDao;
            nfcCardDao = nfcDao;
            nfcCardDao2 = nfcDao2;
            vatDao = vDao;
            unitDao = uDao;
            kdsDao = KDSDao;
            kitchenDao = kitchDao;
            kitchenregionDao = kitchrDao;
        }

        public PosEnvironment EstablishReceiptCancelationEnvironment(IDbConnection db)
        {
            PosEnvironment environment = new PosEnvironment();
            environment.Accounts = EstablishAccounts(db);
            environment.Department = EstablishDepartment(db);
            environment.PosInfo = EstablishPosInfo(db, environment.Department.Id, environment.Accounts[0].Id);
            environment.InvoiceTypes = EstablishInvoiceTypes(db);
            environment.PosInfoDetails = EstablishPosInfoDetails(db, environment.InvoiceTypes, environment.PosInfo.Id);
            environment.Staff = EstablishStaff(db);

            return environment;
        }

        public PosEnvironment EstablishEndOFDayXZReportsEnvironment(IDbConnection db)
        {
            PosEnvironment environment = new PosEnvironment();
            environment.Accounts = EstablishAccounts(db);
            environment.Department = EstablishDepartment(db);
            environment.PosInfo = EstablishPosInfo(db, environment.Department.Id, environment.Accounts[0].Id);
            environment.Staff = EstablishStaff(db);
            environment.EndOFDay = EstablishEndOfDay(db, environment.PosInfo.Id, environment.Staff.Id);

            return environment;
        }

        public PosEnvironment EstablishNFCCardEnvironment(IDbConnection db)
        {
            PosEnvironment environment = new PosEnvironment();
            environment.NFCCard = EstablishNFCCard(db);

            return environment;
        }

        private List<AccountDTO> EstablishAccounts(IDbConnection db)
        {
            List<AccountDTO> accountList = new List<AccountDTO>();

            AccountDTO account1 = new AccountDTO();
            account1.Id = 0;
            account1.Description = "Cash";
            account1.Type = 1;
            account1.IsDefault = null;
            account1.SendsTransfer = false;
            account1.IsDeleted = null;
            account1.KeyboardType = 1;
            account1.CardOnly = null;
            accountList.Add(account1);

            AccountDTO account2 = new AccountDTO();
            account2.Id = 0;
            account2.Description = "Complimentary";
            account2.Type = 2;
            account2.IsDefault = null;
            account2.SendsTransfer = true;
            account2.IsDeleted = null;
            account2.KeyboardType = 1;
            account2.CardOnly = null;
            accountList.Add(account2);

            AccountDTO account3 = new AccountDTO();
            account3.Id = 0;
            account3.Description = "Room Charge";
            account3.Type = 3;
            account3.IsDefault = null;
            account3.SendsTransfer = true;
            account3.IsDeleted = null;
            account3.KeyboardType = 1;
            account3.CardOnly = null;
            accountList.Add(account3);

            AccountDTO account4 = new AccountDTO();
            account4.Id = 0;
            account4.Description = "Credit Card";
            account4.Type = 4;
            account4.IsDefault = null;
            account4.SendsTransfer = true;
            account4.IsDeleted = null;
            account4.KeyboardType = 1;
            account4.CardOnly = null;
            accountList.Add(account4);

            AccountDTO account5 = new AccountDTO();
            account5.Id = 0;
            account5.Description = "Barcode";
            account5.Type = 5;
            account5.IsDefault = null;
            account5.SendsTransfer = true;
            account5.IsDeleted = null;
            account5.KeyboardType = 1;
            account5.CardOnly = null;
            accountList.Add(account5);

            AccountDTO account6 = new AccountDTO();
            account6.Id = 0;
            account6.Description = "Ticket Restaurant";
            account6.Type = 6;
            account6.IsDefault = null;
            account6.SendsTransfer = true;
            account6.IsDeleted = null;
            account6.KeyboardType = 1;
            account6.CardOnly = null;
            accountList.Add(account6);

            AccountDTO account7 = new AccountDTO();
            account7.Id = 0;
            account7.Description = "Allowance";
            account7.Type = 9;
            account7.IsDefault = null;
            account7.SendsTransfer = true;
            account7.IsDeleted = null;
            account7.KeyboardType = 1;
            account7.CardOnly = null;
            accountList.Add(account7);

            return accountDao.InsertList(db, accountList);
        }

        private DepartmentDTO EstablishDepartment(IDbConnection db)
        {
            DepartmentDTO department = new DepartmentDTO();
            department.Id = 0;
            department.Description = "Test Department";
            department.IsDeleted = null;

            return departmentDao.Upsert(db, department);
        }

        private PosInfoDTO EstablishPosInfo(IDbConnection db, long departmentId, long accountId)
        {
            PosInfoDTO posinfo = new PosInfoDTO();
            posinfo.Id = 0;
            posinfo.Code = "1";
            posinfo.Description = "Test Pos";
            posinfo.FODay = DateTime.Now.Date;
            posinfo.CloseId = 0;
            posinfo.IPAddress = "1.1.1.1";
            posinfo.Type = 1;
            posinfo.wsIP = null;
            posinfo.wsPort = null;
            posinfo.DepartmentId = departmentId;
            posinfo.FiscalName = "ECR-1";
            posinfo.FiscalType = 1;
            posinfo.IsOpen = true;
            posinfo.ReceiptCount = 0;
            posinfo.ResetsReceiptCounter = false;
            posinfo.Theme = null;
            posinfo.AccountId = accountId;
            posinfo.LogInToOrder = false;
            posinfo.ClearTableManually = false;
            posinfo.ViewOnly = null;
            posinfo.IsDeleted = null;
            posinfo.InvoiceSumType = null;
            posinfo.LoginToOrderMode = 1;
            posinfo.KeyboardType = 3;
            posinfo.CustomerDisplayGif = "view1.gif";
            posinfo.DefaultHotelId = null;
            posinfo.NfcDevice = "NFCTestComm";

            return posinfoDao.Upsert(db, posinfo);
        }

        private List<InvoiceTypesDTO> EstablishInvoiceTypes(IDbConnection db)
        {
            List<InvoiceTypesDTO> invoiceTypesList = new List<InvoiceTypesDTO>();

            InvoiceTypesDTO invoiceType1 = new InvoiceTypesDTO();
            invoiceType1.Id = 0;
            invoiceType1.Code = "1";
            invoiceType1.Abbreviation = "ΑΛΣ";
            invoiceType1.Description = "ΑΠΟΔΕΙΞΗ ΛΙΑΝΙΚΩΝ ΣΥΝΑΛΛΑΓΩΝ";
            invoiceType1.Type = 1;
            invoiceType1.IsDeleted = null;
            invoiceTypesList.Add(invoiceType1);

            InvoiceTypesDTO invoiceType2 = new InvoiceTypesDTO();
            invoiceType2.Id = 0;
            invoiceType2.Code = "2";
            invoiceType2.Abbreviation = "ΔΠ";
            invoiceType2.Description = "ΔΕΛΤΙΟ ΠΑΡΑΓΓΕΛΙΑΣ";
            invoiceType2.Type = 2;
            invoiceType2.IsDeleted = null;
            invoiceTypesList.Add(invoiceType2);

            InvoiceTypesDTO invoiceType3 = new InvoiceTypesDTO();
            invoiceType3.Id = 0;
            invoiceType3.Code = "3";
            invoiceType3.Abbreviation = "ΕΑΣ";
            invoiceType3.Description = "ΕΙΔΙΚΟ ΑΚΥΡΩΤΙΚΟ ΣΤΟΙΧΕΙΟ";
            invoiceType3.Type = 3;
            invoiceType3.IsDeleted = null;
            invoiceTypesList.Add(invoiceType3);

            InvoiceTypesDTO invoiceType4 = new InvoiceTypesDTO();
            invoiceType4.Id = 0;
            invoiceType4.Code = "4";
            invoiceType4.Abbreviation = "ΦΙΛ";
            invoiceType4.Description = "ΕΙΔΙΚΟ ΣΤΟΙΧΕΙΟ ΑΥΤΟΠΑΡΑΔΟΣΗΣ";
            invoiceType4.Type = 4;
            invoiceType4.IsDeleted = null;
            invoiceTypesList.Add(invoiceType4);

            InvoiceTypesDTO invoiceType5 = new InvoiceTypesDTO();
            invoiceType5.Id = 0;
            invoiceType5.Code = "5";
            invoiceType5.Abbreviation = "ΣΑΠ";
            invoiceType5.Description = "ΣΤΟΙΧΕΙΟ ΑΠΟΖΗΜΙΩΣΗΣ";
            invoiceType5.Type = 5;
            invoiceType5.IsDeleted = null;
            invoiceTypesList.Add(invoiceType5);

            InvoiceTypesDTO invoiceType6 = new InvoiceTypesDTO();
            invoiceType6.Id = 0;
            invoiceType6.Code = "7";
            invoiceType6.Abbreviation = "ΤΙΜ";
            invoiceType6.Description = "ΤΙΜΟΛΟΓΙΟ";
            invoiceType6.Type = 7;
            invoiceType6.IsDeleted = null;
            invoiceTypesList.Add(invoiceType6);

            InvoiceTypesDTO invoiceType7 = new InvoiceTypesDTO();
            invoiceType7.Id = 0;
            invoiceType7.Code = "8";
            invoiceType7.Abbreviation = "ΑΔΠ";
            invoiceType7.Description = "ΑΚΥΡΩΤΙΚΟ ΔΕΛΤΙΟ ΠΑΡΑΓΓΕΛΙΑΣ";
            invoiceType7.Type = 8;
            invoiceType7.IsDeleted = null;
            invoiceTypesList.Add(invoiceType7);

            InvoiceTypesDTO invoiceType8 = new InvoiceTypesDTO();
            invoiceType8.Id = 0;
            invoiceType8.Code = "11";
            invoiceType8.Abbreviation = "ΑΠ";
            invoiceType8.Description = "ΑΠΟΔΕΙΞΗ ΠΛΗΡΩΜΗΣ";
            invoiceType8.Type = 11;
            invoiceType8.IsDeleted = null;
            invoiceTypesList.Add(invoiceType8);

            InvoiceTypesDTO invoiceType9 = new InvoiceTypesDTO();
            invoiceType9.Id = 0;
            invoiceType9.Code = "12";
            invoiceType9.Abbreviation = "ΑΕ";
            invoiceType9.Description = "ΑΠΟΔΕΙΞΗ ΕΙΣΠΡΑΞΗΣ";
            invoiceType9.Type = 12;
            invoiceType9.IsDeleted = null;
            invoiceTypesList.Add(invoiceType9);

            return invoicetypesDao.InsertList(db, invoiceTypesList);
        }

        private List<PosInfoDetailDTO> EstablishPosInfoDetails(IDbConnection db, List<InvoiceTypesDTO> invoiceTypes, long posInfoId)
        {
            List<PosInfoDetailDTO> posInfoDetailList = new List<PosInfoDetailDTO>();

            foreach (InvoiceTypesDTO invoiceType in invoiceTypes)
            {
                if (invoiceType.Type == 1)
                {
                    PosInfoDetailDTO posInfoDetail1 = new PosInfoDetailDTO();
                    posInfoDetail1.Id = 0;
                    posInfoDetail1.PosInfoId = posInfoId;
                    posInfoDetail1.Counter = 0;
                    posInfoDetail1.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail1.Description = invoiceType.Description;
                    posInfoDetail1.ResetsAfterEod = false;
                    posInfoDetail1.InvoiceId = 1;
                    posInfoDetail1.ButtonDescription = "Εξόφληση";
                    posInfoDetail1.Status = 1;
                    posInfoDetail1.CreateTransaction = true;
                    posInfoDetail1.FiscalType = 1;
                    posInfoDetail1.IsInvoice = true;
                    posInfoDetail1.IsCancel = false;
                    posInfoDetail1.GroupId = 1;
                    posInfoDetail1.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail1.IsPdaHidden = false;
                    posInfoDetail1.IsDeleted = null;
                    posInfoDetail1.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail1);

                    PosInfoDetailDTO posInfoDetail2 = new PosInfoDetailDTO();
                    posInfoDetail2.Id = 0;
                    posInfoDetail2.PosInfoId = posInfoId;
                    posInfoDetail2.Counter = 0;
                    posInfoDetail2.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail2.Description = invoiceType.Description;
                    posInfoDetail2.ResetsAfterEod = false;
                    posInfoDetail2.InvoiceId = 1;
                    posInfoDetail2.ButtonDescription = "Απόδειξη";
                    posInfoDetail2.Status = 1;
                    posInfoDetail2.CreateTransaction = false;
                    posInfoDetail2.FiscalType = 1;
                    posInfoDetail2.IsInvoice = true;
                    posInfoDetail2.IsCancel = false;
                    posInfoDetail2.GroupId = 1;
                    posInfoDetail2.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail2.IsPdaHidden = false;
                    posInfoDetail2.IsDeleted = null;
                    posInfoDetail2.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail2);
                }
                else if (invoiceType.Type == 2)
                {
                    PosInfoDetailDTO posInfoDetail3 = new PosInfoDetailDTO();
                    posInfoDetail3.Id = 0;
                    posInfoDetail3.PosInfoId = posInfoId;
                    posInfoDetail3.Counter = 0;
                    posInfoDetail3.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail3.Description = invoiceType.Description;
                    posInfoDetail3.ResetsAfterEod = false;
                    posInfoDetail3.InvoiceId = 2;
                    posInfoDetail3.ButtonDescription = "Δελτίο Παραγγελίας";
                    posInfoDetail3.Status = 1;
                    posInfoDetail3.CreateTransaction = false;
                    posInfoDetail3.FiscalType = 1;
                    posInfoDetail3.IsInvoice = false;
                    posInfoDetail3.IsCancel = false;
                    posInfoDetail3.GroupId = 2;
                    posInfoDetail3.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail3.IsPdaHidden = false;
                    posInfoDetail3.IsDeleted = null;
                    posInfoDetail3.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail3);
                }
                else if (invoiceType.Type == 3)
                {
                    PosInfoDetailDTO posInfoDetail4 = new PosInfoDetailDTO();
                    posInfoDetail4.Id = 0;
                    posInfoDetail4.PosInfoId = posInfoId;
                    posInfoDetail4.Counter = 0;
                    posInfoDetail4.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail4.Description = invoiceType.Description;
                    posInfoDetail4.ResetsAfterEod = false;
                    posInfoDetail4.InvoiceId = 4;
                    posInfoDetail4.ButtonDescription = "Ακύρωση Απόδειξης";
                    posInfoDetail4.Status = 1;
                    posInfoDetail4.CreateTransaction = true;
                    posInfoDetail4.FiscalType = 1;
                    posInfoDetail4.IsInvoice = false;
                    posInfoDetail4.IsCancel = true;
                    posInfoDetail4.GroupId = 3;
                    posInfoDetail4.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail4.IsPdaHidden = false;
                    posInfoDetail4.IsDeleted = null;
                    posInfoDetail4.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail4);
                }
                else if (invoiceType.Type == 4)
                {
                    PosInfoDetailDTO posInfoDetail5 = new PosInfoDetailDTO();
                    posInfoDetail5.Id = 0;
                    posInfoDetail5.PosInfoId = posInfoId;
                    posInfoDetail5.Counter = 0;
                    posInfoDetail5.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail5.Description = invoiceType.Description;
                    posInfoDetail5.ResetsAfterEod = false;
                    posInfoDetail5.InvoiceId = 1;
                    posInfoDetail5.ButtonDescription = "Αυτοπαράδοση";
                    posInfoDetail5.Status = 1;
                    posInfoDetail5.CreateTransaction = true;
                    posInfoDetail5.FiscalType = 1;
                    posInfoDetail5.IsInvoice = true;
                    posInfoDetail5.IsCancel = false;
                    posInfoDetail5.GroupId = 4;
                    posInfoDetail5.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail5.IsPdaHidden = false;
                    posInfoDetail5.IsDeleted = null;
                    posInfoDetail5.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail5);
                }
                else if (invoiceType.Type == 5)
                {
                    PosInfoDetailDTO posInfoDetail6 = new PosInfoDetailDTO();
                    posInfoDetail6.Id = 0;
                    posInfoDetail6.PosInfoId = posInfoId;
                    posInfoDetail6.Counter = 0;
                    posInfoDetail6.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail6.Description = invoiceType.Description;
                    posInfoDetail6.ResetsAfterEod = false;
                    posInfoDetail6.InvoiceId = 1;
                    posInfoDetail6.ButtonDescription = "Αποζημίωση";
                    posInfoDetail6.Status = 1;
                    posInfoDetail6.CreateTransaction = true;
                    posInfoDetail6.FiscalType = 1;
                    posInfoDetail6.IsInvoice = true;
                    posInfoDetail6.IsCancel = false;
                    posInfoDetail6.GroupId = 5;
                    posInfoDetail6.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail6.IsPdaHidden = false;
                    posInfoDetail6.IsDeleted = null;
                    posInfoDetail6.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail6);
                }
                else if (invoiceType.Type == 7)
                {
                    PosInfoDetailDTO posInfoDetail7 = new PosInfoDetailDTO();
                    posInfoDetail7.Id = 0;
                    posInfoDetail7.PosInfoId = posInfoId;
                    posInfoDetail7.Counter = 0;
                    posInfoDetail7.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail7.Description = invoiceType.Description;
                    posInfoDetail7.ResetsAfterEod = true;
                    posInfoDetail7.InvoiceId = 1;
                    posInfoDetail7.ButtonDescription = "Τιμολόγιο";
                    posInfoDetail7.Status = 1;
                    posInfoDetail7.CreateTransaction = true;
                    posInfoDetail7.FiscalType = 1;
                    posInfoDetail7.IsInvoice = true;
                    posInfoDetail7.IsCancel = false;
                    posInfoDetail7.GroupId = 7;
                    posInfoDetail7.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail7.IsPdaHidden = false;
                    posInfoDetail7.IsDeleted = null;
                    posInfoDetail7.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail7);
                }
                else if (invoiceType.Type == 8)
                {
                    PosInfoDetailDTO posInfoDetail8 = new PosInfoDetailDTO();
                    posInfoDetail8.Id = 0;
                    posInfoDetail8.PosInfoId = posInfoId;
                    posInfoDetail8.Counter = 0;
                    posInfoDetail8.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail8.Description = invoiceType.Description;
                    posInfoDetail8.ResetsAfterEod = false;
                    posInfoDetail8.InvoiceId = 4;
                    posInfoDetail8.ButtonDescription = "Ακύρωση Δελτίου";
                    posInfoDetail8.Status = 1;
                    posInfoDetail8.CreateTransaction = false;
                    posInfoDetail8.FiscalType = 1;
                    posInfoDetail8.IsInvoice = false;
                    posInfoDetail8.IsCancel = true;
                    posInfoDetail8.GroupId = 8;
                    posInfoDetail8.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail8.IsPdaHidden = false;
                    posInfoDetail8.IsDeleted = null;
                    posInfoDetail8.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail8);
                }
                else if (invoiceType.Type == 11)
                {
                    PosInfoDetailDTO posInfoDetail9 = new PosInfoDetailDTO();
                    posInfoDetail9.Id = 0;
                    posInfoDetail9.PosInfoId = posInfoId;
                    posInfoDetail9.Counter = 0;
                    posInfoDetail9.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail9.Description = invoiceType.Description;
                    posInfoDetail9.ResetsAfterEod = false;
                    posInfoDetail9.InvoiceId = 3;
                    posInfoDetail9.ButtonDescription = "Απόδειξη Πληρωμής";
                    posInfoDetail9.Status = 1;
                    posInfoDetail9.CreateTransaction = true;
                    posInfoDetail9.FiscalType = 1;
                    posInfoDetail9.IsInvoice = false;
                    posInfoDetail9.IsCancel = false;
                    posInfoDetail9.GroupId = 11;
                    posInfoDetail9.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail9.IsPdaHidden = false;
                    posInfoDetail9.IsDeleted = null;
                    posInfoDetail9.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail9);
                }
                else if (invoiceType.Type == 12)
                {
                    PosInfoDetailDTO posInfoDetail10 = new PosInfoDetailDTO();
                    posInfoDetail10.Id = 0;
                    posInfoDetail10.PosInfoId = posInfoId;
                    posInfoDetail10.Counter = 0;
                    posInfoDetail10.Abbreviation = invoiceType.Abbreviation;
                    posInfoDetail10.Description = invoiceType.Description;
                    posInfoDetail10.ResetsAfterEod = false;
                    posInfoDetail10.InvoiceId = 3;
                    posInfoDetail10.ButtonDescription = "Απόδειξη Είσπραξης";
                    posInfoDetail10.Status = 1;
                    posInfoDetail10.CreateTransaction = true;
                    posInfoDetail10.FiscalType = 1;
                    posInfoDetail10.IsInvoice = false;
                    posInfoDetail10.IsCancel = false;
                    posInfoDetail10.GroupId = 12;
                    posInfoDetail10.InvoicesTypeId = invoiceType.Id;
                    posInfoDetail10.IsPdaHidden = false;
                    posInfoDetail10.IsDeleted = null;
                    posInfoDetail10.SendsVoidToKitchen = 1;
                    posInfoDetailList.Add(posInfoDetail10);
                }
            }

            return posinfodetailDao.InsertList(db, posInfoDetailList);
        }

        private StaffDTO EstablishStaff(IDbConnection db)
        {
            StaffDTO staff = new StaffDTO();
            staff.Id = 0;
            staff.Code = "user";
            staff.FirstName = "Test";
            staff.LastName = "Staff";
            staff.ImageUri = null;
            staff.Password = "pass";
            staff.IsDeleted = null;
            staff.Identification = ";1234567890123456=119O01111100000444?";

            return staffDao.Upsert(db, staff);
        }

        private EndOfDayDTO EstablishEndOfDay(IDbConnection db, long posInfoId, long staffId)
        {
            EndOfDayDTO endOfDay = new EndOfDayDTO();
            endOfDay.Id = 0;
            endOfDay.FODay = DateTime.Now.Date;
            endOfDay.PosInfoId = posInfoId;
            endOfDay.CloseId = 1;
            endOfDay.Gross = 100;
            endOfDay.Net = 76;
            endOfDay.TicketsCount = 20;
            endOfDay.ItemCount = 43;
            endOfDay.TicketAverage = 5;
            endOfDay.StaffId = staffId;
            endOfDay.Discount = 0;
            endOfDay.dtDateTime = DateTime.Now;
            endOfDay.Barcodes = 0;

            return endOFDayDao.Upsert(db, endOfDay);
        }

        private NFCcardDTO EstablishNFCCard(IDbConnection db)
        {
            NFCcardDTO nfcCard = new NFCcardDTO();
            nfcCard.Id = 0;
            nfcCard.Type = 0;
            nfcCard.RoomSector = 7;
            nfcCard.FirstDateSector = 8;
            nfcCard.SecondDateSector = 9;
            nfcCard.ValidateDate = true;

            nfcCardDao2.DeleteList(db, "where Id >= @Id", new { Id = 0 });
            return nfcCardDao.Upsert(db, nfcCard);
        }



        private List<VatDTO> EstablishVats(IDbConnection db)
        {
            List<VatDTO> vatList = new List<VatDTO>();

            VatDTO vat1 = new VatDTO();
            vat1.Id = 0;
            vat1.Description = "24%";
            vat1.Percentage = 24;
            vat1.Code = 1;
            vatList.Add(vat1);

            VatDTO vat2 = new VatDTO();
            vat2.Id = 0;
            vat2.Description = "13%";
            vat2.Percentage = 13;
            vat2.Code = 2;
            vatList.Add(vat2);

            VatDTO vat3 = new VatDTO();
            vat3.Id = 0;
            vat3.Description = "6%";
            vat3.Percentage = 6;
            vat3.Code = 3;
            vatList.Add(vat3);

            VatDTO vat4 = new VatDTO();
            vat4.Id = 0;
            vat4.Description = "0%";
            vat4.Percentage = 0;
            vat4.Code = 5;
            vatList.Add(vat4);

            return vatDao.InsertList(db, vatList);
        }

        private List<UnitsDTO> EstablishUnits(IDbConnection db)
        {
            List<UnitsDTO> unitsList = new List<UnitsDTO>();

            UnitsDTO unit1 = new UnitsDTO();
            unit1.Id = 0;
            unit1.Description = "Test Unit 1";
            unit1.Abbreviation = "TU1";
            unit1.Unit = 1;
            unitsList.Add(unit1);

            UnitsDTO unit2 = new UnitsDTO();
            unit2.Id = 0;
            unit2.Description = "Test Unit 2";
            unit2.Abbreviation = "TU2";
            unit2.Unit = 1;
            unitsList.Add(unit2);

            return unitDao.InsertList(db, unitsList);
        }

        private List<KdsDTO> EstablishKDS(IDbConnection db)
        {
            List<KdsDTO> kdsList = new List<KdsDTO>();

            KdsDTO kds1 = new KdsDTO();
            kds1.Id = 0;
            kds1.Description = "Test KDS 1";
            kds1.Status = 1;
            kds1.PosInfoId = null;
            kds1.IsDeleted = null;
            kdsList.Add(kds1);

            KdsDTO kds2 = new KdsDTO();
            kds2.Id = 0;
            kds2.Description = "Test KDS 2";
            kds2.Status = 1;
            kds2.PosInfoId = null;
            kds2.IsDeleted = null;
            kdsList.Add(kds2);

            return kdsDao.InsertList(db, kdsList);
        }

        private List<KitchenDTO> EstablishKitchen(IDbConnection db)
        {
            List<KitchenDTO> kitchenList = new List<KitchenDTO>();

            KitchenDTO kitchen1 = new KitchenDTO();
            kitchen1.Id = 0;
            kitchen1.Code = "1";
            kitchen1.Description = "Kitchen";
            kitchen1.Status = 1;
            kitchen1.IsDeleted = null;
            kitchenList.Add(kitchen1);

            KitchenDTO kitchen2 = new KitchenDTO();
            kitchen2.Id = 0;
            kitchen2.Code = "2";
            kitchen2.Description = "Bar";
            kitchen2.Status = 1;
            kitchen2.IsDeleted = null;
            kitchenList.Add(kitchen2);

            return kitchenDao.InsertList(db, kitchenList);
        }

        private List<KitchenRegionDTO> EstablishKitchenRegions(IDbConnection db)
        {
            List<KitchenRegionDTO> kitchenRegionsList = new List<KitchenRegionDTO>();

            KitchenRegionDTO kitchenRegion1 = new KitchenRegionDTO();
            kitchenRegion1.Id = 0;
            kitchenRegion1.ItemRegion = "Starters";
            kitchenRegion1.RegionPosition = 1;
            kitchenRegion1.Abbr = "1st";
            kitchenRegionsList.Add(kitchenRegion1);

            KitchenRegionDTO kitchenRegion2 = new KitchenRegionDTO();
            kitchenRegion2.Id = 0;
            kitchenRegion2.ItemRegion = "Main Dishes";
            kitchenRegion2.RegionPosition = 2;
            kitchenRegion2.Abbr = "2nd";
            kitchenRegionsList.Add(kitchenRegion2);

            KitchenRegionDTO kitchenRegion3 = new KitchenRegionDTO();
            kitchenRegion3.Id = 0;
            kitchenRegion3.ItemRegion = "Beverages";
            kitchenRegion3.RegionPosition = 3;
            kitchenRegion3.Abbr = "BV";
            kitchenRegionsList.Add(kitchenRegion3);

            return kitchenregionDao.InsertList(db, kitchenRegionsList);
        }
    }
}
