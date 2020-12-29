using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    /// <summary>
    /// handle data related to End Of Day Procedure
    /// </summary>
    public interface IEndOfDayDT
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
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = false
        /// <param name="PosInfo"></param>
        /// </summary>
        long GetExpectedPaidCount(DBInfoModel dbInfo, long PosInfo);


        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true
        /// <param name="PosInfo"></param>
        /// </summary>
        long GetAlreadyPaidCount(DBInfoModel dbInfo, long PosInfo);

        decimal GetExpectedPaidTotalAmount(DBInfoModel dbInfo, long PosInfo);
        decimal GetAlreadyPaidTotalAmount(DBInfoModel dbInfo, long PosInfo);

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = false By StaffId
        /// <param name="PosInfo"></param>
        /// </summary>
        long GetExpectedPaidCountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId);
        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true By StaffId
        /// <param name="PosInfo"></param>
        /// </summary>
        long GetAlreadyPaidCountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId);

        decimal GetExpectedPaidTotalAmountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId);
        decimal GetAlreadyPaidTotalAmountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId);

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
        /// Return the list of receipt's totals per type for a POS for the current day. 
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        List<EndOfDayTotalModel> DayTotals(DBInfoModel Store, long posInfo);

        List<EndOfDayTotalModel> PreviewSalesType(DBInfoModel dbInfo, long PosInfo);

        /// <summary>
        /// Return the list of receipt's totals per type per Staff for a POS for the current day. 
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        List<EndOfDayByStaffModel> DayTotalsPerStaff(DBInfoModel Store, long posInfo);

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
        /// <param name="Store">a Symposium.Models.Models.Store object to construct connection String from it</param>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId. Used only for AnalysisByAccount (FOR ID 0)</param>
        /// <param name="type">the type of invoices to return see<seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/></param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> GetDayAnalysis(DBInfoModel Store, long posInfo, long staffId, long accountId,  EndOfDayReceiptTypes type);

        /// <summary>
        /// Return a list of all printed and not canseled invoices (not ΔΠ) for a specific POS and Staff (EndOfDayReceiptTypes = -100). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> GetTotalReportAnalysis(DBInfoModel Store, long posInfo, long staffId);

        /// <summary>
        /// Creates vat analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day vat analysis models. See: <seealso cref="Symposium.Models.Models.EodVatAnalysisModel"/> </returns>
        List<EodVatAnalysisModel> GetVatAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId);

        /// <summary>
        /// Creates payment analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> GetPaymentAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId);

        /// <summary>
        /// Creates void analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> GetVoidAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId);

        /// <summary>
        /// Creates barcode analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> GetBarcodeAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId);

        /// <summary>
        /// Creates product for eod analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day product analysis models. See: <seealso cref="Symposium.Models.Models.ProductForEodAnalysisModel"/> </returns>
        List<ProductForEodAnalysisModel> GetProductForEodAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId);

        /// <summary>
        /// Selects single end of day according to Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> End of day model. See: <seealso cref="Symposium.Models.Models.EndOfDayModel"/> </returns>
        EndOfDayModel GetSingleEndOfDay(DBInfoModel Store, long endOfDayId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        long GetLastEndOfDayId(DBInfoModel store);

        /// <summary>
        /// Selects transfers to pms for selected pos
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> Transfer to pms model. See: <seealso cref="Symposium.Models.Models.EodTransferToPmsModel"/> </returns>
        List<EodTransferToPmsModel> GetListEodTransferToPms(DBInfoModel Store, long posInfo);

        /// <summary>
        /// Updates database after closing day and printing Z report
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfoId"> Id of posinfo </param>
        /// <param name="staffId"> Id of staff </param>
        /// <param name="eodModel"> End of day model </param>
        /// <param name="endOfDayDetailModel"> End of day details model </param>
        /// <param name="endOfDayPaymentAnalysisModel"> End of day payments analysis model </param>
        /// <param name="endOfDayVoidsAnalysisModel"> End of day voids analysis model </param>
        /// <param name="eodTransferToPmsModel"> End of day transfer to pms model </param>
        long GetUpdateDatabaseAfterEod(DBInfoModel Store, long posInfoId, long staffId, EndOfDayModel eodModel, List<EndOfDayDetailModel> endOfDayDetailModel, List<EndOfDayPaymentAnalysisModel> endOfDayPaymentAnalysisModel, List<EndOfDayVoidsAnalysisModel> endOfDayVoidsAnalysisModel, List<EndOfDayTransferToPmsModel> eodTransferToPmsModel);

        /// <summary>
        /// Delete All Data from Digital Signature
        /// </summary>
        /// <param name="Store"> Store </param>
        long ExecuteDeleteDigitalSignatureAfterEod(DBInfoModel Store);

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
