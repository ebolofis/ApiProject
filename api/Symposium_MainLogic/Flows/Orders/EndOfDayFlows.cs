
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using Symposium.Helpers;
using Symposium.WebApi.DataAccess.Interfaces.DT.Delivery;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;

namespace Symposium.WebApi.MainLogic.Flows
{
    /// <summary>
    /// Main Logic Class that handles the EndOfDay activities
    /// </summary>
    public class EndOfDayFlows : IEndOfDayFlows
    {

        IEndOfDayTasks eod;
        IPaginationHelper<EODAnalysisReceiptModel> pageHlp;
        IAccountTasks accountHlp;
        IGdprFlows gdpr;
        IDeliveryRoutingDT DeliveryRoutingDT;

        public EndOfDayFlows(IEndOfDayTasks eod, IPaginationHelper<EODAnalysisReceiptModel> pageHlp, IAccountTasks accounts, IGdprFlows gdpr,
                    IDeliveryRoutingDT DeliveryRoutingDT)
        {
            this.eod = eod;
            this.pageHlp = pageHlp;
            this.accountHlp = accounts;
            this.gdpr = gdpr;
            this.DeliveryRoutingDT = DeliveryRoutingDT;
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public List<CashierStatisticsModel> GetCashCashierStatistics(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return eod.GetCashCashierStatistics(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Cash Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public decimal GetCashierTotalCash(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return eod.GetCashierTotalCash(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Credit Card
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public List<CashierStatisticsModel> GetCreditCashierStatistics(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return eod.GetCreditCashierStatistics(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Credit Card Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public decimal GetCashierTotalCredit(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return eod.GetCashierTotalCredit(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type 
        /// </summary>
        public List<CashierTotalAmountsModel> GetCashierAmounts(DBInfoModel dbInfo, long PosInfo)
        {
            return eod.GetCashierAmounts(dbInfo, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Model  
        /// </summary>
        public CashierTotals GetCashierTotalAmounts(DBInfoModel dbInfo, long PosInfo)
        {
            return eod.GetCashierTotalAmounts(dbInfo, PosInfo);
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type by Staff
        /// </summary>
        public List<CashierTotalAmountsModel> GetCashierAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return eod.GetCashierAmountsByStaff(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Cashier Total Model by Staff
        /// </summary>
        public CashierTotals GetCashierTotalAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            return eod.GetCashierTotalAmountsByStaff(dbInfo, StaffId, PosInfo);
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false
        /// <param name="PosInfo"></param>
        /// </summary>
        public CreditCardsReceiptsCounts GetTransactionCreditCardsCount(DBInfoModel dbInfo, long PosInfo)
        {
            return eod.GetTransactionCreditCardsCount(dbInfo, PosInfo);
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false By StaffId
        /// <param name="PosInfo"></param>
        /// <param name="StaffId"></param>
        /// </summary>
        public CreditCardsReceiptsCounts GetTransactionCreditCardsCountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            return eod.GetTransactionCreditCardsCountByStaff(dbInfo, PosInfo, StaffId);
        }

        public long UpdateStaffStatus(DBInfoModel dbInfo)
        {
            return eod.UpdateStaffStatus(dbInfo);
        }

        /// <summary>
        /// Get Tips Total
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        public decimal GetTipsTotal(DBInfoModel dbInfo, long PosInfo)
        {
            return eod.GetTipsTotal(dbInfo, PosInfo);
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
            return eod.GetTipsTotalByStaff(dbInfo, PosInfo, StaffId);
        }


        /// <summary>
        /// Return the (End of) Day Preview for a POS to controller.
        /// 1. the list of totals per type
        /// 2. the list of totals per type per staff
        /// </summary>
        /// <param name="PosInfo">PosInfo</param>
        /// <param name="dbInfo">dbInfo</param>
        /// <returns>EndOfDayPreviewModel : List of EndOfDayTotalModel and List of EndOfDayByStaffModel  <seealso cref="Symposium.Models.Models.EndOfDayPreviewModel"/></returns>
        public EndOfDayPreviewModel GetPreview(long PosInfo, DBInfoModel dbInfo, string customerClassType)
        {
            EndOfDayPreviewModel previewModel = new EndOfDayPreviewModel();

            previewModel.Totals = eod.DayTotals(PosInfo, dbInfo);
            previewModel.TotalsByStaff = eod.DayTotalsPerStaff(PosInfo, dbInfo);
            if (customerClassType == "Pos_WebApi.Customer_Modules.Waterpark")
            {
                previewModel.BarcodeTotals = eod.DayBarcodeTotals(PosInfo, dbInfo);
                previewModel.LockerTotals = eod.DayLockerTotals(PosInfo, dbInfo);
            }

            return previewModel;
        }
      
        public List<EndOfDayTotalModel> PreviewSalesType(DBInfoModel dbInfo, long PosInfo)
        {
            return eod.PreviewSalesType(dbInfo, PosInfo);
        }

        /// <summary>
        /// Return the list of invoises for a specific type for a specific POS.
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId. Used only for AnalysisByAccount (FOR ID 0)</param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <param name="type">the type of invoices to return see<seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/></param>
        /// <returns></returns>
        public PaginationModel<EODAnalysisReceiptModel> GetAnalysis(DBInfoModel dbInfo, long posInfo, long staffId, long accountId, int pageNumber, int pageLength, EndOfDayReceiptTypes type)
        {
            //1. get the results
            List<EODAnalysisReceiptModel> list = eod.GetDayAnalysis(dbInfo, posInfo, staffId, accountId, type);

            //2. return only the wanted page
            return pageHlp.GetPage(list, pageNumber, pageLength);
        }

        /// <summary>
        /// Return  ListS of Receipts for a specific POS and EndOfDayReceiptTypes.ReceiptTotal (-100). Every item in the list contains a list of receipts with a specific Account
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId. Used only for AnalysisByAccount (FOR ID 0)</param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns></returns>
        public EodTotalReportAnalysisModel GetTotalReportAnalysisByAccount(DBInfoModel dbInfo, long posInfo, long staffId)
        {
            //1. get the Receipts
            List<EODAnalysisReceiptModel> list = eod.GetDayAnalysisAll(dbInfo, posInfo, staffId);

            //2. get the list of active accounts
            List<AccountModel> accounts = accountHlp.GetActiveAccounts(dbInfo);

            //2. return receipts grouped by Account
            return eod.GroupReceiptsByAccount(list, accounts);
        }

        /// <summary>
        /// Return X and Z Totals for printZ, reprintZ and printX for Extecr
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="endOfDayId">endOfDayId.Id of EndOfDay table</param>
        /// <param name="eodPMS">PMS Date sto kleisimo imeras</param>
        /// <param name="Enum">Check if printZ or reprintZ or printX</param>
        /// <returns></returns>
        public EodXAndZTotalsForExtecr EodXAndZ(DBInfoModel dbInfo, long posInfo, long staffId, long? endOfDayId, DateTime eodPMS, EndOfDayActions Enum)
        {
            switch (Enum)
            { 
                case EndOfDayActions.PrintZ:
                    EodXAndZTotalsModel eodXAndZTotalsForPrintZ = eod.GetEodXAndZTotals(dbInfo, posInfo, endOfDayId);
                    EodXAndZTotalsForExtecr XAndZTotalsForPrintZForExtecr = eod.ConvertXZModelToXZForExtecr(eodXAndZTotalsForPrintZ, endOfDayId);
                    List<EodTransferToPmsModel> EodTransferToPms = eod.GetEodTransferToPms(dbInfo, posInfo);

                    //Send payments to hotelizer
                    bool extApi = false;
                    extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
                    if (extApi)
                    {
                        //post receipt using Hotelizer Api Url
                        HotelizerFlows hotelizer = new HotelizerFlows();
                        hotelizer.PostNewPaymentsToHotelizer(EodTransferToPms, dbInfo);
                    }

                    eod.UpdateDatabaseAfterEod(dbInfo, staffId, eodPMS, eodXAndZTotalsForPrintZ, XAndZTotalsForPrintZForExtecr, EodTransferToPms);
                    //Set EndOfDayId to StaffCash Where EndOfDayId is Null
                    eod.SetEndOfDayId(dbInfo, (long)XAndZTotalsForPrintZForExtecr.EndOfDayId);
                    //Delete Everything from OrderDetailIngredientsKDS
                    eod.DeleteOrderDetailIngredientsKDS(dbInfo);
                    //GDPR For WaterPark Customers(Update table OnlineRegistration).
                    gdpr.UpdateOnlineRegistration(dbInfo);
                    // GDPR For Pms Customers(Update table Quest).
                    gdpr.UpdateGuest(dbInfo);
                    // at end of day, move delivery routing records to hist table
                    DeliveryRoutingDT.moveDeliveryRoutingToHist(dbInfo);
                    return XAndZTotalsForPrintZForExtecr;

                case EndOfDayActions.ReprintZ:
                    if (endOfDayId == null)
                    {
                        throw new BusinessException(Symposium.Resources.Errors.REPRINTZREPORTFAILED, Symposium.Resources.Errors.EODIDNULL);
                    }
                    EodXAndZTotalsModel eodXAndZTotalsForReprintZ = eod.GetEodXAndZTotals(dbInfo, posInfo, endOfDayId);
                    EodXAndZTotalsForExtecr XAndZTotalsForReprintZForExtecr = eod.ConvertXZModelToXZForExtecr(eodXAndZTotalsForReprintZ, endOfDayId);
                    return XAndZTotalsForReprintZForExtecr;

                case EndOfDayActions.PrintX:
                    EodXAndZTotalsModel eodXAndZTotalsForPrintX = eod.GetEodXAndZTotals(dbInfo, posInfo, endOfDayId);
                    EodXAndZTotalsForExtecr XAndZTotalsForPrintXForExtecr = eod.ConvertXZModelToXZForExtecr(eodXAndZTotalsForPrintX, endOfDayId);
                    return XAndZTotalsForPrintXForExtecr;

                default:
                    return null;
            }

        }

        /// <summary>
        /// Get Lockers Statistics
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <returns></returns>
        public LockersStatisticsModel GetLockersStatistics(DBInfoModel dbInfo, long posInfo)
        {
            return eod.GetLockersStatistics(dbInfo, posInfo);
        }

        /// <summary>
        /// Insert zlogger to endofday table
        /// </summary>
        /// <param name="zlogger">Pos</param>
        /// <returns></returns>
        public void InsertZlogger(DBInfoModel Store, string zlogger)
        {
             eod.InsertZlogger(Store, zlogger);
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
            return eod.InsertStaffCash(dbInfo, StaffId, CashAmount);
        }

    }
}
