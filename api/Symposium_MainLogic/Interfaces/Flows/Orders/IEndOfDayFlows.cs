using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    /// <summary>
    /// handles the EndOfDay activities
    /// </summary>
    public interface IEndOfDayFlows
    {
        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        List<CashierStatisticsModel> GetCashCashierStatistics(DBInfoModel dbInfo, long StaffId, long PosInfo);

        /// <summary>
        /// Get Cashier Analytic Statistics For Credit Card
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        List<CashierStatisticsModel> GetCreditCashierStatistics(DBInfoModel dbInfo, long StaffId, long PosInfo);

        /// <summary>
        /// Get Cashier Total Cash Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        decimal GetCashierTotalCash(DBInfoModel dbInfo, long StaffId, long PosInfo);

        /// <summary>
        /// Get Cashier Total Credit Card Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        decimal GetCashierTotalCredit(DBInfoModel dbInfo, long StaffId, long PosInfo);

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type 
        /// <param name="PosInfo"></param>
        /// </summary>
        List<CashierTotalAmountsModel> GetCashierAmounts(DBInfoModel dbInfo, long PosInfo);

        /// <summary>
        /// Get Cashier Total Model  
        /// <param name="PosInfo"></param>
        /// </summary>
        CashierTotals GetCashierTotalAmounts(DBInfoModel dbInfo, long PosInfo);

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type by Staff
        /// <param name="StaffId"></param>
        /// <param name="PosInfo"></param>
        /// </summary>
        List<CashierTotalAmountsModel> GetCashierAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo);

        /// <summary>
        /// Get Cashier Total Model by Staff
        /// <param name="StaffId"></param>
        /// <param name="PosInfo"></param>
        /// </summary>
        CashierTotals GetCashierTotalAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo);

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false
        /// <param name="PosInfo"></param>
        /// </summary>
        CreditCardsReceiptsCounts GetTransactionCreditCardsCount(DBInfoModel dbInfo, long PosInfo);

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false By StaffId
        /// <param name="PosInfo"></param>
        /// <param name="StaffId"></param>
        /// </summary>
        CreditCardsReceiptsCounts GetTransactionCreditCardsCountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId);

        long UpdateStaffStatus(DBInfoModel dbInfo);

        /// <summary>
        /// Get Tips Total
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        decimal GetTipsTotal(DBInfoModel dbInfo, long PosInfo);

        /// <summary>
        /// Get Tips Total by Staff
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        decimal GetTipsTotalByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId);

        /// <summary>
        /// Return the (End of) Day Preview for a POS to controller.
        /// 1. the list of totals per type
        /// 2. the list of totals per type per staff
        /// </summary>
        /// <param name="PosInfo">PosInfo</param>
        /// <param name="dbInfo">dbInfo</param>
        /// <returns>EndOfDayPreviewModel : List of EndOfDayTotalModel and List of EndOfDayByStaffModel  <seealso cref="Symposium.Models.Models.EndOfDayPreviewModel"/></returns>
        EndOfDayPreviewModel GetPreview(long PosInfo, DBInfoModel dbInfo, string customerClassType);

        List<EndOfDayTotalModel> PreviewSalesType(DBInfoModel dbInfo, long PosInfo);


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
        PaginationModel<EODAnalysisReceiptModel> GetAnalysis(DBInfoModel dbInfo, long posInfo, long staffId, long accountId, int pageNumber, int pageLength, EndOfDayReceiptTypes type);


        /// <summary>
        /// Return  ListS of Receipts for a specific POS and EndOfDayReceiptTypes.ReceiptTotal (-100). Every item in the list contains a list of receipts with a specific Account
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId. Used only for AnalysisByAccount (FOR ID 0)</param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns></returns>
        EodTotalReportAnalysisModel GetTotalReportAnalysisByAccount(DBInfoModel dbInfo, long posInfo, long staffId);


        /// <summary>
        /// Return X and Z Totals for printZ, reprintZ and printX for Extecr
        /// </summary>
        /// <param name="posInfo">POS</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="endOfDayId">endOfDayId.Id of EndOfDay table</param>
        /// <param name="eodPMS">PMS Date sto kleisimo imeras</param>
        /// <param name="Enum">Check if printZ or reprintZ or printX</param>
        /// <returns></returns>
        EodXAndZTotalsForExtecr EodXAndZ(DBInfoModel dbInfo, long posInfo, long staffId, long? endOfDayId, DateTime eodPMS, EndOfDayActions Enum);


        /// <summary>
        /// Get Lockers Statistics
        /// </summary>
        /// <param name="posInfo">POS</param>
        /// <returns></returns>
        LockersStatisticsModel GetLockersStatistics(DBInfoModel dbInfo, long posInfo);

        /// <summary>
        /// Insert z-logger to end-of-day table
        /// </summary>
        /// <param name="zlogger">POS</param>
        /// <returns></returns>
        void InsertZlogger(DBInfoModel dbInfo, string zlogger);

        /// <summary>
        /// Staff Insert Cash Amount
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="CashAmount"></param>
        /// <returns>
        /// </returns>
        long InsertStaffCash(DBInfoModel dbInfo, long StaffId, decimal CashAmount);
    }
}
