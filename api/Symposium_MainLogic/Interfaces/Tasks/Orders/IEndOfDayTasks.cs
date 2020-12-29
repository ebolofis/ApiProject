using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    /// <summary>
    /// return totals of any kind for a POS for the current date
    /// </summary>
    public interface IEndOfDayTasks
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
        /// Return the list of Totals for the current date by type for all staffs for a specific POS
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns></returns>
        List<EndOfDayTotalModel> DayTotals(long PosInfo, DBInfoModel Store);

        List<EndOfDayTotalModel> PreviewSalesType(DBInfoModel dbInfo, long PosInfo);

        /// <summary>
        /// Return the list of Totals for the current date by type by Staff for a specific POS
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns></returns>
        List<List<EndOfDayByStaffModel>> DayTotalsPerStaff(long PosInfo, DBInfoModel Store);

        /// <summary>
        /// Return the sum of cash and credit cards of barcode transaction types 7 
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="Store"></param>
        /// <returns></returns>
        EndOfDayBarcodesModel DayBarcodeTotals(long PosInfo, DBInfoModel Store);

        /// <summary>
        /// Return the sum of locker transaction types 8, 9
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="Store"></param>
        /// <returns></returns>
        long DayLockerTotals(long PosInfo, DBInfoModel Store);

        /// <summary>
        /// Return the list of invoises for a specific type for a specific POS.
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId. Used only for AnalysisByAccount (FOR ID 0)</param>
        /// <param name="type">the type of invoices to return see<seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/></param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> GetDayAnalysis(DBInfoModel Store, long posInfo, long staffId, long accountId,  EndOfDayReceiptTypes type);


        /// <summary>
        /// Group Receipts by account
        /// </summary>
        /// <returns></returns>
        EodTotalReportAnalysisModel GroupReceiptsByAccount();


        /// <summary>
        /// Return a list of all printed and not canseled invoices (not ΔΠ) for a specific POS and Staff (EndOfDayReceiptTypes = -100).
        /// For a specific transaction return one record for all accounts
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> GetDayAnalysisAll(DBInfoModel Store, long posInfo, long staffId);

        /// <summary>
        /// Get a list of receipts (EODAnalysisReceiptModel) and return many lists of receipts by Account
        /// </summary>
        /// <param name="list">list of receipts </param>
        /// <param name="accounts">list of accounts</param>
        /// <returns></returns>
        /// 
        EodTotalReportAnalysisModel GroupReceiptsByAccount(List<EODAnalysisReceiptModel> list, List<AccountModel> accounts);

        /// <summary>
        /// Return a EodXAndZTotalsModel model        
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="endOfDayId">endOfDayId.Id of EndOfDay table</param>
        /// <returns>EodXAndZTotalsModel model</returns>
        EodXAndZTotalsModel GetEodXAndZTotals(DBInfoModel Store, long posInfo, long? endOfDayId);

        /// <summary>
        /// Return a EodXAndZTotalsForExtecr model to send to Extecr        
        /// </summary>
        /// <param name="databaseXZTotalsModel">EodXAndZTotalsModel model</param>
        /// <param name="endOfDayId">endOfDayId.Id of EndOfDay table</param>
        /// <returns>EodXAndZTotalsForExtecr model</returns>
        EodXAndZTotalsForExtecr ConvertXZModelToXZForExtecr(EodXAndZTotalsModel databaseXZTotalsModel, long? endOfDayId);

        /// <summary>
        /// Get a list of receipts (EODAnalysisReceiptModel) and return many lists of receipts by Account
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <returns>List of EodTransferToPmsModel model</returns>
        List<EodTransferToPmsModel> GetEodTransferToPms(DBInfoModel Store, long posInfo);

        /// <summary>
        /// Add data to a model of EndOfDayModel and to a List of EndOfDayTransferToPmsModel, EndOfDayDetailModel, EndOfDayPaymentAnalysisModel, EndOfDayVoidsAnalysisModel and the call GetUpdateDatabaseAfterEod
        /// </summary>
        /// <param name="staffId">StaffId</param>
        /// <param name="eodPMS">PMS Date sto kleisimo imeras</param>
        /// <param name="eodXAndZTotalsForPrintZ">EodXAndZTotalsModel model</param>
        /// <param name="XAndZTotalsForPrintZForExtecr">EodXAndZTotalsForExtecr model</param>
        /// <param name="EodTransferToPms">List of EodTransferToPmsModel model</param>
        /// <returns></returns>
        void UpdateDatabaseAfterEod(DBInfoModel Store, long staffId, DateTime eodPMS, EodXAndZTotalsModel eodXAndZTotalsForPrintZ, EodXAndZTotalsForExtecr XAndZTotalsForPrintZForExtecr, List<EodTransferToPmsModel> EodTransferToPms);

        /// <summary>
        /// Get Lockers Statistics
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <returns></returns>
        LockersStatisticsModel GetLockersStatistics(DBInfoModel Store, long posInfo);

        /// <summary>
        /// Insert zlogger to endofday table
        /// </summary>
        /// <param name="zlogger">Pos</param>
        /// <returns></returns>
        void InsertZlogger(DBInfoModel Store, string zlogger);

        /// <summary>
        /// Staff Insert Cash Amount
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="CashAmount"></param>
        /// <returns>
        /// </returns>
        long InsertStaffCash(DBInfoModel dbInfo, long StaffId, decimal CashAmount);

        /// <summary>
        /// Set EndOfDayId to StaffCash Where EndOfDayId is Null
        /// </summary>
        /// <param name="EndOfDayId"></param>
        /// <returns>
        /// </returns>
        void SetEndOfDayId(DBInfoModel dbInfo, long EndOfDayId);

        /// <summary>
        /// Delete Everything from OrderDetailIngredientsKDS
        /// </summary>
        /// <param name="dbInfo"></param>
        void DeleteOrderDetailIngredientsKDS(DBInfoModel dbInfo);
    }
}
