using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
   public interface IEndOfDayDAO
    {

        /// <summary>
        /// Return a list of EndofDay Totals per Type of Totals.
        /// <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        /// <param name="db">DB Connection</param>
        /// <returns>List of EndOfDayTotalModel</returns>
        List<EndOfDayTotalModel> EndOfDayTotal(long posInfo, IDbConnection db);

        List<EndOfDayTotalModel> PreviewSalesType(IDbConnection db, long posInfo);

        /// <summary>
        /// Return a list of EndofDay Totals per Type per Staff of Totals.
        /// <seealso cref="Symposium.Models.Enums.EndOfDayByStaffModel"/>
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        /// <param name="db">DB Connection</param>
        /// <returns>List of EndOfDayByStaffModel</returns>
         List<EndOfDayByStaffModel> EndOfDayPerStaffTotal(long posInfo, IDbConnection db);

        /// <summary>
        /// Return the sum of cash and credit cards of barcode transaction types 7 
        /// </summary>
        /// <param name="posInfo"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        EndOfDayBarcodesModel DayBarcodeTotals(long posInfo, IDbConnection db);

        /// <summary>
        /// Return the sum of locker transaction types 8, 9
        /// </summary>
        /// <param name="posInfo"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        long DayLockerTotals(long posInfo, IDbConnection db);

        /// <summary>
        /// Return a list of today invoices for a specific accountID (Cash, Credit Cards, Coplimentary each...), POS and Staff (EndOfDayReceiptTypes = 0). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="AccountId">AccountId</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
         List<EODAnalysisReceiptModel> AnalysisByAccount(IDbConnection db, long posInfo, long AccountId, long staffId = 0);
        

        /// <summary>
        /// Return a list of not paid invoices for a specific POS and Staff (EndOfDayReceiptTypes = -99). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
         List<EODAnalysisReceiptModel> AnalysisNotPaid(IDbConnection db, long posInfo, long staffId = 0);


        /// <summary>
        /// Return a list of all printed and not canseled invoices (not ΔΠ) for a specific POS and Staff (EndOfDayReceiptTypes = -100). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <param name="all">if all=1 then for splited transactions return a record per account (way of payment). If all=1 then return one record for a transaction for all accounts</param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> AnalysisReceiptTotal(IDbConnection db, long posInfo, long staffId = 0, int all = 0);
        

        /// <summary>
        /// Return a list of not invoiced invoices for a specific POS and Staff (EndOfDayReceiptTypes = -101). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
         List<EODAnalysisReceiptModel> AnalysisNotInvoiced(IDbConnection db, long posInfo, long staffId = 0);
        

        /// <summary>
        /// Return a list of canceld invoices for a specific POS and Staff (EndOfDayReceiptTypes =  -102). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> AnalysisCanceled(IDbConnection db, long posInfo, long staffId = 0);
        

        /// <summary>
        /// Return a list of not printed invoices for a specific POS and Staff (EndOfDayReceiptTypes =  -103). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
         List<EODAnalysisReceiptModel> AnalysisNotPrinted(IDbConnection db, long posInfo, long staffId = 0);


        /// <summary>
        /// Return a list of invoices with Discount for a specific POS and Staff (EndOfDayReceiptTypes =  -104). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> AnalysisDiscount(IDbConnection db, long posInfo, long staffId = 0);

        /// <summary>
        /// Return a list of invoices with Loyalty Discount for a specific POS and Staff (EndOfDayReceiptTypes =  -110). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        List<EODAnalysisReceiptModel> AnalysisLoyaltyDiscount(IDbConnection db, long posInfo, long staffId = 0);

        /// <summary>
        /// Returns a list of analysis per vat for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection</param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day vat analysis models. See: <seealso cref="Symposium.Models.Models.EodVatAnalysisModel"/> </returns>
        List<EodVatAnalysisModel> VatAnalysisForCurrentDay(IDbConnection db, long posInfo);

        /// <summary>
        /// Returns a list of analysis per vat for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day vat analysis models. See: <seealso cref="Symposium.Models.Models.EodVatAnalysisModel"/> </returns>
        List<EodVatAnalysisModel> VatAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId);

        /// <summary>
        /// Returns a list of analysis per account for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> PaymentAnalysisForCurrentDay(IDbConnection db, long posInfo);

        /// <summary>
        /// Returns a list of analysis per account for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> PaymentAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId);

        /// <summary>
        /// Returns a list of analysis per account of voids for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> VoidAnalysisForCurrentDay(IDbConnection db, long posInfo);

        /// <summary>
        /// Returns a list of analysis per account of voids for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> VoidAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId);

        /// <summary>
        /// Returns a list of analysis per account of barcodes for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> BarcodeAnalysisForCurrentDay(IDbConnection db, long posInfo);

        /// <summary>
        /// Returns a list of analysis per account of barcodes for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        List<EodAccountAnalysisModel> BarcodeAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId);

        /// <summary>
        /// Returns a list of analysis per product for eod pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <returns> List of product for eod analysis models. See: <seealso cref="Symposium.Models.Models.ProductForEodAnalysisModel"/> </returns>
        List<ProductForEodAnalysisModel> ProductForEodAnalysisForCurrentDay(IDbConnection db, long posInfo);

        /// <summary>
        /// Returns a list of analysis per product for eod pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of product for eod analysis models. See: <seealso cref="Symposium.Models.Models.ProductForEodAnalysisModel"/> </returns>
        List<ProductForEodAnalysisModel> ProductForEodAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId);

        /// <summary>
        /// Returns a list of transfer to pms charges for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day transfer to pms models. See: <seealso cref="Symposium.Models.Models.EodTransferToPmsModel"/> </returns>
        List<EodTransferToPmsModel> EodTransferToPms(IDbConnection db, long posInfo);

        long EodDeleteSign(IDbConnection db);
    }
}
