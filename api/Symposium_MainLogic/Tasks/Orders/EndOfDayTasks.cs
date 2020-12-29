using Autofac;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    /// <summary>
    /// class that returns totals of any kind for a POS for the current date
    /// </summary>
    public class EndOfDayTasks : IEndOfDayTasks
    {
        IEndOfDayDT endOfDayDB;  // part of DataAccess Layer
        IPosInfoDT posInfoDB;
        ILockersDT lockerDB;
        IInvoicesDT invoicesDB;

        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public List<CashierStatisticsModel> GetCashCashierStatistics(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return endOfDayDB.GetCashCashierStatistics(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Cash Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public decimal GetCashierTotalCash(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return endOfDayDB.GetCashierTotalCash(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Credit Card
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public List<CashierStatisticsModel> GetCreditCashierStatistics(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return endOfDayDB.GetCreditCashierStatistics(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Credit Card Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public decimal GetCashierTotalCredit(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return endOfDayDB.GetCashierTotalCredit(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type 
        /// </summary>
        public List<CashierTotalAmountsModel> GetCashierAmounts(DBInfoModel dbInfo, long PosInfo)
        {
            return endOfDayDB.GetCashierAmounts(dbInfo, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Model  
        /// </summary>
        public CashierTotals GetCashierTotalAmounts(DBInfoModel dbInfo, long PosInfo)
        {
            return endOfDayDB.GetCashierTotalAmounts(dbInfo, PosInfo);
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type by Staff
        /// </summary>
        public List<CashierTotalAmountsModel> GetCashierAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return endOfDayDB.GetCashierAmountsByStaff(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Model by Staff
        /// </summary>
        public CashierTotals GetCashierTotalAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return endOfDayDB.GetCashierTotalAmountsByStaff(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false
        /// <param name="PosInfo"></param>
        /// </summary>
        public CreditCardsReceiptsCounts GetTransactionCreditCardsCount(DBInfoModel dbInfo, long PosInfo)
        {
            CreditCardsReceiptsCounts model = new CreditCardsReceiptsCounts();
            model.ExpectedPaid = endOfDayDB.GetExpectedPaidCount(dbInfo, PosInfo);
            model.ExpectedPaidAmount = endOfDayDB.GetExpectedPaidTotalAmount(dbInfo, PosInfo);
            model.AlreadyPaid = endOfDayDB.GetAlreadyPaidCount(dbInfo, PosInfo);
            model.AlreadyPaidAmount = endOfDayDB.GetAlreadyPaidTotalAmount(dbInfo, PosInfo);
            return model;
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false By StaffId
        /// <param name="PosInfo"></param>
        /// <param name="StaffId"></param>
        /// </summary>
        public CreditCardsReceiptsCounts GetTransactionCreditCardsCountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            CreditCardsReceiptsCounts model = new CreditCardsReceiptsCounts();
            model.ExpectedPaid = endOfDayDB.GetExpectedPaidCountByStaff(dbInfo, PosInfo, StaffId);
            model.ExpectedPaidAmount = endOfDayDB.GetExpectedPaidTotalAmountByStaff(dbInfo, PosInfo, StaffId);
            model.AlreadyPaid = endOfDayDB.GetAlreadyPaidCountByStaff(dbInfo, PosInfo, StaffId);
            model.AlreadyPaidAmount = endOfDayDB.GetAlreadyPaidTotalAmountByStaff(dbInfo, PosInfo, StaffId);
            return model;
        }

        public long UpdateStaffStatus(DBInfoModel dbInfo)
        {
            return endOfDayDB.UpdateStaffStatus(dbInfo);
        }

        /// <summary>
        /// Get Tips Total
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        public decimal GetTipsTotal(DBInfoModel dbInfo, long PosInfo)
        {
            return endOfDayDB.GetTipsTotal(dbInfo, PosInfo);
        }

        /// <summary>
        /// Get Tips Total by Staff
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public decimal GetTipsTotalByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            return endOfDayDB.GetTipsTotalByStaff(dbInfo, PosInfo, StaffId);
        }

        public EndOfDayTasks(IEndOfDayDT eodDB, IPosInfoDT posInfoDB, ILockersDT lockerDB, IInvoicesDT invoicesDB)
        {
            this.endOfDayDB = eodDB;
            this.posInfoDB = posInfoDB;
            this.lockerDB = lockerDB;
            this.invoicesDB = invoicesDB;
        }


        /// <summary>
        /// Return the list of Totals for the current date by type for all staffs for a specific POS
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns></returns>
        public List<EndOfDayTotalModel> DayTotals(long PosInfo, DBInfoModel Store)
        {
            return endOfDayDB.DayTotals(Store, PosInfo);
        }

        public List<EndOfDayTotalModel> PreviewSalesType(DBInfoModel dbInfo, long PosInfo)
        {
            return endOfDayDB.PreviewSalesType(dbInfo, PosInfo);
        }


        /// <summary>
        /// Return the  Lists of Totals per staff for the current date by type  for a specific POS
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns></returns>
        public List<List<EndOfDayByStaffModel>> DayTotalsPerStaff(long PosInfo, DBInfoModel Store)
        {
            //1. get all records per type and staff
            List<EndOfDayByStaffModel> eodStaffs = endOfDayDB.DayTotalsPerStaff(Store, PosInfo);

            //2. create Lists of Totals per staff. One list for each staff
            List<List<EndOfDayByStaffModel>> lists = new List<List<EndOfDayByStaffModel>>();
            //2.1 get distinct staffIds 
            IEnumerable<long> staffs = eodStaffs.Select(p => p.StaffId).Distinct();
            //2.2 foreach staff get the list of his types and add this list to the List<List<EndOfDayByStaffModel>> lists
            foreach (long staff in staffs)
            {
                lists.Add(eodStaffs.Where(p => p.StaffId == staff).ToList<EndOfDayByStaffModel>());
            }
            return lists;
        }

        /// <summary>
        /// Return the sum of cash and credit cards of barcode transaction types 7 
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="Store"></param>
        /// <returns></returns>
        public EndOfDayBarcodesModel DayBarcodeTotals(long PosInfo, DBInfoModel Store)
        {
            EndOfDayBarcodesModel endOfDayBarcodesModel = endOfDayDB.DayBarcodeTotals(PosInfo, Store);
            return endOfDayBarcodesModel;
        }

        /// <summary>
        /// Return the sum of locker transaction types 8, 9
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="Store"></param>
        /// <returns></returns>
        public long DayLockerTotals(long PosInfo, DBInfoModel Store)
        {
            long total = endOfDayDB.DayLockerTotals(PosInfo, Store);
            return total;
        }


        /// <summary>
        /// Return the list of invoises for a specific type for a specific POS.
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId. Used only for AnalysisByAccount (FOR ID 0)</param>
        /// <param name="type">the type of invoices to return see<seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/></param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> GetDayAnalysis(DBInfoModel Store, long posInfo, long staffId, long accountId, EndOfDayReceiptTypes type)
        {
            return endOfDayDB.GetDayAnalysis(Store, posInfo, staffId, accountId, type);
        }


        /// <summary>
        /// Return a list of all printed and not canseled invoices (not ΔΠ) for a specific POS and Staff (EndOfDayReceiptTypes = -100).
        /// For a specific transaction return one record for all accounts
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> GetDayAnalysisAll(DBInfoModel Store, long posInfo, long staffId)
        {
            return endOfDayDB.GetTotalReportAnalysis(Store, posInfo, staffId);
        }

        /// <summary>
        /// Group Receipts by account
        /// </summary>
        /// <returns></returns>
        public EodTotalReportAnalysisModel GroupReceiptsByAccount()
        {
            return new EodTotalReportAnalysisModel();
        }

        /// <summary>
        /// Get a list of receipts (EODAnalysisReceiptModel) and return many lists of receipts by Account
        /// </summary>
        /// <param name="list">list of receipts </param>
        /// <param name="accounts">list of accounts</param>
        /// <returns></returns>
        public EodTotalReportAnalysisModel GroupReceiptsByAccount(List<EODAnalysisReceiptModel> list, List<AccountModel> accounts)
        {
            EodTotalReportAnalysisModel result = new EodTotalReportAnalysisModel();
            result.ReceiptsPerAccount = new List<EODReceiptListPerAccountModel>();

            //Add an account with id=0 to the list of existing accounts
            accounts.Add(new AccountModel() { Id = 0, Description = "Not Paid" });

            //for every account create a new list of receipts.
            foreach (AccountModel account in accounts)
            {
                EODReceiptListPerAccountModel receiptsPerAccount = new EODReceiptListPerAccountModel();
                receiptsPerAccount.List = list.Where(c => c.AccountId == account.Id).ToList<EODAnalysisReceiptModel>();
                receiptsPerAccount.Count = receiptsPerAccount.List.Count;
                receiptsPerAccount.Total = receiptsPerAccount.List.Sum(item => item.Total);
                receiptsPerAccount.AccountId = account.Id;
                receiptsPerAccount.Account = account.Description;
                result.ReceiptsPerAccount.Add(receiptsPerAccount);
            }

            return result;
        }

        /// <summary>
        /// Return a EodXAndZTotalsModel model
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="endOfDayId">endOfDayId.Id of EndOfDay table</param>
        /// <returns></returns>
        public EodXAndZTotalsModel GetEodXAndZTotals(DBInfoModel Store, long posInfo, long? endOfDayId)
        {
            EodXAndZTotalsModel model = new EodXAndZTotalsModel();
            model.vatAnalysis = endOfDayDB.GetVatAnalysis(Store, posInfo, endOfDayId);
            model.paymentAnalysis = endOfDayDB.GetPaymentAnalysis(Store, posInfo, endOfDayId);
            model.voidAnalysis = endOfDayDB.GetVoidAnalysis(Store, posInfo, endOfDayId);
            model.barcodeAnalysis = endOfDayDB.GetBarcodeAnalysis(Store, posInfo, endOfDayId);
            model.productsForEodAnalysis = endOfDayDB.GetProductForEodAnalysis(Store, posInfo, endOfDayId);
            model.lockers = lockerDB.GetLockers(Store, posInfo, endOfDayId);
            model.posInfo = posInfoDB.GetSinglePosInfo(Store, posInfo);
            if (endOfDayId != null)
            {
                model.endOfDay = endOfDayDB.GetSingleEndOfDay(Store, endOfDayId ?? 0);
                model.ticketsCount = model.endOfDay.TicketsCount;
            }
            else
            {
                model.endOfDay = null;
                model.ticketsCount = invoicesDB.GetTicketCount(Store, posInfo);
            }

            return model;
        }

        /// <summary>
        /// Return a EodXAndZTotalsForExtecr model to send to Extecr
        /// </summary>
        /// <param name="databaseXZTotalsModel">EodXAndZTotalsModel model</param>
        /// <param name="endOfDayId">endOfDayId.Id of EndOfDay table</param>
        /// <returns>EodXAndZTotalsForExtecr model</returns>
        public EodXAndZTotalsForExtecr ConvertXZModelToXZForExtecr(EodXAndZTotalsModel databaseXZTotalsModel, long? endOfDayId)
        {
            EodXAndZTotalsForExtecr model = new EodXAndZTotalsForExtecr();
            model.EndOfDayId = endOfDayId ?? 0;
            model.PosInfoId = databaseXZTotalsModel.posInfo.Id;
            DateTime foDay = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.FODay ?? DateTime.Now : databaseXZTotalsModel.posInfo.FODay ?? DateTime.Now;
            model.Day = foDay.ToString("yyyy-MM-dd HH:mm:ss.sss");
            DateTime reportDay = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.dtDateTime : DateTime.Now;
            model.dtDateTime = reportDay.ToString("yyyy-MM-dd HH:mm:ss.sss");
            model.PosCode = databaseXZTotalsModel.posInfo.Code;
            model.PosDescription = databaseXZTotalsModel.posInfo.Description;
            model.ReportNo = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.CloseId : databaseXZTotalsModel.posInfo.CloseId + 1;
            decimal gross = 0, vat = 0, net = 0, discount = 0;
            long itemsCount = 0;
            foreach (EodVatAnalysisModel item in databaseXZTotalsModel.vatAnalysis)
            {
                gross += item.Gross;
                vat += item.Gross - item.Net;
                net += item.Net;
                discount += item.Discount;
                itemsCount += item.ItemCount;
            }
            decimal barcodeAmount = 0;
            foreach (EodAccountAnalysisModel item in databaseXZTotalsModel.barcodeAnalysis)
            {
                barcodeAmount += item.Amount;
            }
            model.Gross = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.Gross : gross;
            model.VatAmount = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.Gross - databaseXZTotalsModel.endOfDay.Net : vat;
            model.Net = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.Net : net;
            model.Discount = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.Discount : discount;
            model.TicketCount = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.TicketsCount : databaseXZTotalsModel.ticketsCount;
            model.ItemsCount = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.ItemCount : itemsCount;
            model.Barcodes = databaseXZTotalsModel.endOfDay != null ? databaseXZTotalsModel.endOfDay.Barcodes : barcodeAmount;
            model.Lockers = databaseXZTotalsModel.lockers;
            model.PaymentAnalysis = new List<PaymentAnalysisModel>();
            model.CardAnalysis = new List<CardAnalysisModel>();
            foreach (EodAccountAnalysisModel item in databaseXZTotalsModel.paymentAnalysis)
            {
                PaymentAnalysisModel temp = new PaymentAnalysisModel();
                temp.Description = item.Description;
                temp.Amount = item.Amount;
                model.PaymentAnalysis.Add(temp);
                if (item.AccountType == (int)AccountTypeEnum.CreditCard || item.AccountType == (int)AccountTypeEnum.TicketCompliment)
                {
                    CardAnalysisModel temp2 = new CardAnalysisModel();
                    temp2.Description = item.Description;
                    temp2.Amount = item.Amount;
                    model.CardAnalysis.Add(temp2);
                }
            }
            model.VoidAnalysis = new List<VoidAnalysisModel>();
            foreach (EodAccountAnalysisModel item in databaseXZTotalsModel.voidAnalysis)
            {
                VoidAnalysisModel temp = new VoidAnalysisModel();
                temp.Description = item.Description;
                temp.Amount = item.Amount;
                model.VoidAnalysis.Add(temp);
            }
            model.vatAnalysis = new List<VatAnalysisModel>();
            foreach (EodVatAnalysisModel item in databaseXZTotalsModel.vatAnalysis)
            {
                VatAnalysisModel temp = new VatAnalysisModel();
                temp.VatId = item.VatId;
                temp.VatRate = item.VatRate;
                temp.Tax = item.Tax;
                temp.Gross = item.Gross;
                temp.VatAmount = item.Gross - item.Net;
                temp.Net = item.Net;
                model.vatAnalysis.Add(temp);
            }
            model.ProductsForEODStats = new List<ProductAnalysisModel>();
            foreach (ProductForEodAnalysisModel item in databaseXZTotalsModel.productsForEodAnalysis)
            {
                ProductAnalysisModel temp = new ProductAnalysisModel();
                temp.Description = item.Description;
                temp.Qty = item.Quantity;
                temp.Total = item.Total;
                model.ProductsForEODStats.Add(temp);
            }
            model.FiscalName = databaseXZTotalsModel.posInfo.FiscalName;
            return model;
        }

        /// <summary>
        /// Get a list of receipts (EODAnalysisReceiptModel) and return many lists of receipts by Account
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <returns>List of EodTransferToPmsModel model</returns>
        public List<EodTransferToPmsModel> GetEodTransferToPms(DBInfoModel Store, long posInfo)
        {
            List<EodTransferToPmsModel> model = endOfDayDB.GetListEodTransferToPms(Store, posInfo);
            return model;
        }

        /// <summary>
        /// Add data to a model of EndOfDayModel and to a List of EndOfDayTransferToPmsModel, EndOfDayDetailModel, EndOfDayPaymentAnalysisModel, EndOfDayVoidsAnalysisModel and the call GetUpdateDatabaseAfterEod
        /// </summary>
        /// <param name="staffId">StaffId</param>
        /// <param name="eodPMS">PMS Date sto kleisimo imeras</param>
        /// <param name="eodXAndZTotalsForPrintZ">EodXAndZTotalsModel model</param>
        /// <param name="XAndZTotalsForPrintZForExtecr">EodXAndZTotalsForExtecr model</param>
        /// <param name="EodTransferToPms">List of EodTransferToPmsModel model</param>
        /// <returns></returns>
        public void UpdateDatabaseAfterEod(DBInfoModel Store, long staffId, DateTime eodPMS, EodXAndZTotalsModel eodXAndZTotalsForPrintZ, EodXAndZTotalsForExtecr XAndZTotalsForPrintZForExtecr, List<EodTransferToPmsModel> EodTransferToPms)
        {
            EndOfDayModel eodModel = new EndOfDayModel();
            List<EndOfDayTransferToPmsModel> eodTransferToPmsModel = new List<EndOfDayTransferToPmsModel>();
            List<EndOfDayDetailModel> endOfDayDetailModel = new List<EndOfDayDetailModel>();
            List<EndOfDayPaymentAnalysisModel> endOfDayPaymentAnalysisModel = new List<EndOfDayPaymentAnalysisModel>();
            List<EndOfDayVoidsAnalysisModel> endOfDayVoidsAnalysisModel = new List<EndOfDayVoidsAnalysisModel>();
            eodModel.Id = 0;
            eodModel.FODay = eodXAndZTotalsForPrintZ.endOfDay != null ? eodXAndZTotalsForPrintZ.endOfDay.FODay ?? DateTime.Now : eodXAndZTotalsForPrintZ.posInfo.FODay ?? DateTime.Now;
            eodModel.PosInfoId = eodXAndZTotalsForPrintZ.posInfo.Id;
            eodModel.CloseId = eodXAndZTotalsForPrintZ.posInfo.CloseId + 1;
            eodModel.Gross = XAndZTotalsForPrintZForExtecr.Gross;
            eodModel.Net = XAndZTotalsForPrintZForExtecr.Net;
            eodModel.TicketsCount = XAndZTotalsForPrintZForExtecr.TicketCount;
            eodModel.ItemCount = XAndZTotalsForPrintZForExtecr.ItemsCount;
            eodModel.TicketAverage = eodModel.TicketsCount != 0 ? eodModel.Gross / eodModel.TicketsCount : 0;
            eodModel.StaffId = staffId;
            eodModel.Discount = XAndZTotalsForPrintZForExtecr.Discount;
            eodModel.dtDateTime = eodXAndZTotalsForPrintZ.endOfDay != null ? eodXAndZTotalsForPrintZ.endOfDay.dtDateTime : DateTime.Now;
            eodModel.Barcodes = XAndZTotalsForPrintZForExtecr.Barcodes;
            eodModel.eodPMS = eodPMS;
            foreach (EodTransferToPmsModel item in EodTransferToPms)
            {
                EndOfDayTransferToPmsModel temp = new EndOfDayTransferToPmsModel();
                temp.Id = 0;
                temp.RegNo = 0;
                temp.PmsDepartmentId = item.PmsDepartmentId;
                temp.Description = "Pos:" + XAndZTotalsForPrintZForExtecr.PosCode + " PosName:" + XAndZTotalsForPrintZForExtecr.PosDescription + " Descr:" + item.Profilename + " Ημέρας EOD";
                temp.ProfileId = null;
                temp.ProfileName = item.Profilename;
                temp.TransactionId = null;
                temp.TransferType = 0;
                temp.RoomId = "";
                temp.RoomDescription = item.roomDescription;
                temp.ReceiptNo = null;
                temp.PosInfoDetailId = null;
                temp.SendToPms = 1;
                temp.Total = item.total;
                temp.SendToPmsTS = DateTime.Now;
                temp.ErrorMessage = null;
                temp.PmsResponseId = null;
                temp.TransferIdentifier = Guid.NewGuid();
                temp.PmsDepartmentDescription = item.PmsDepartmentDescription;
                temp.Status = 0;
                temp.PosInfoId = eodXAndZTotalsForPrintZ.posInfo.Id;
                temp.EndOfDayId = eodModel.Id;
                temp.HotelId = item.hotelId;
                temp.IsDeleted = null;
                eodTransferToPmsModel.Add(temp);
            }

            foreach (EodVatAnalysisModel item in eodXAndZTotalsForPrintZ.vatAnalysis)
            {
                EndOfDayDetailModel temp = new EndOfDayDetailModel();
                temp.Id = 0;
                temp.EndOfdayId = eodModel.Id;
                temp.VatId = item.VatId;
                temp.VatRate = item.VatRate;
                temp.VatAmount = item.Gross;
                temp.TaxId = null;
                temp.TaxAmount = item.Tax;
                temp.Gross = item.Gross;
                temp.Net = item.Net;
                temp.Discount = item.Discount;
                endOfDayDetailModel.Add(temp);
            }

            foreach (EodAccountAnalysisModel item in eodXAndZTotalsForPrintZ.paymentAnalysis)
            {
                EndOfDayPaymentAnalysisModel temp = new EndOfDayPaymentAnalysisModel();
                temp.Id = 0;
                temp.EndOfdayId = eodModel.Id;
                temp.AccountId = item.AccountId;
                temp.Total = item.Amount;
                temp.Description = item.Description;
                temp.AccountType = item.AccountType;
                endOfDayPaymentAnalysisModel.Add(temp);
            }

            foreach (EodAccountAnalysisModel item in eodXAndZTotalsForPrintZ.voidAnalysis)
            {
                EndOfDayVoidsAnalysisModel temp = new EndOfDayVoidsAnalysisModel();
                temp.Id = 0;
                temp.EndOfdayId = eodModel.Id;
                temp.AccountId = item.AccountId;
                temp.Total = item.Amount;
                temp.Description = item.Description;
                temp.AccountType = item.AccountType;
                endOfDayVoidsAnalysisModel.Add(temp);
            }
            XAndZTotalsForPrintZForExtecr.EndOfDayId =
                       endOfDayDB.GetUpdateDatabaseAfterEod(Store, eodXAndZTotalsForPrintZ.posInfo.Id, staffId, eodModel, endOfDayDetailModel, endOfDayPaymentAnalysisModel, endOfDayVoidsAnalysisModel, eodTransferToPmsModel);

            // Delete all Data From Digital Signature
            endOfDayDB.ExecuteDeleteDigitalSignatureAfterEod(Store);

            return;
        }

        /// <summary>
        /// Get Lockers Statistics
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <returns></returns>
        public LockersStatisticsModel GetLockersStatistics(DBInfoModel Store, long posInfo)
        {
            return endOfDayDB.GetLockersStatistics(Store, posInfo);
        }

        /// <summary>
        /// Insert zlogger to endofday table
        /// </summary>
        /// <param name="zlogger">Pos</param>
        /// <returns></returns>
        public void InsertZlogger(DBInfoModel Store, string zlogger)
        {
            endOfDayDB.InsertZlogger(Store, zlogger);
        }

        /// <summary>
        /// Staff Insert Cash Amount
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="CashAmount"></param>
        /// <returns>
        /// </returns>
        public long InsertStaffCash(DBInfoModel dbInfo, long StaffId, decimal CashAmount)
        {
            return endOfDayDB.InsertStaffCash(dbInfo, StaffId, CashAmount);
        }

        /// <summary>
        /// Set EndOfDayId to StaffCash Where EndOfDayId is Null
        /// </summary>
        /// <param name="EndOfDayId"></param>
        /// <returns>
        /// </returns>
        public void SetEndOfDayId(DBInfoModel dbInfo, long EndOfDayId)
        {
            endOfDayDB.SetEndOfDayId(dbInfo, EndOfDayId);
            return;
        }

        /// <summary>
        /// Delete Everything from OrderDetailIngredientsKDS
        /// </summary>
        /// <param name="dbInfo"></param>
        public void DeleteOrderDetailIngredientsKDS(DBInfoModel dbInfo)
        {
            endOfDayDB.DeleteOrderDetailIngredientsKDS(dbInfo);
            return;
        }
    }
}
