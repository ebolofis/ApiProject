
using Microsoft.AspNet.SignalR;
using Pos_WebApi.Controllers;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/EndOfDay")]
    public class EndOfDayV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IEndOfDayFlows endOfDay;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public EndOfDayV3Controller(IEndOfDayFlows eod)
        {
            this.endOfDay = eod;
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        [HttpGet, Route("GetCashCashierStatistics/{StaffId}/{PosInfo}")]
        public HttpResponseMessage GetCashierStatistics(long StaffId, long PosInfo)
        {
            List<CashierStatisticsModel> results = endOfDay.GetCashCashierStatistics(DBInfo, StaffId, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Cashier Total Cash Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        [HttpGet, Route("GetCashierTotalCash/{StaffId}/{PosInfo}")]
        public HttpResponseMessage GetCashierTotalCash(long StaffId, long PosInfo)
        {
            decimal results = endOfDay.GetCashierTotalCash(DBInfo, StaffId, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Credit Card
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        [HttpGet, Route("GetCreditCashierStatistics/{StaffId}/{PosInfo}")]
        public HttpResponseMessage GetCreditCashierStatistics(long StaffId, long PosInfo)
        {
            List<CashierStatisticsModel> results = endOfDay.GetCreditCashierStatistics(DBInfo, StaffId, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Cashier Total Credit Card Amount For Specific Staff Id
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        [HttpGet, Route("GetCashierTotalCredit/{StaffId}/{PosInfo}")]
        public HttpResponseMessage GetCashierTotalCredit(long StaffId, long PosInfo)
        {
            decimal results = endOfDay.GetCashierTotalCredit(DBInfo, StaffId, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type 
        /// </summary>
        [HttpGet, Route("GetCashierAmounts/{PosInfo}")]
        public HttpResponseMessage GetCashierAmounts(long PosInfo)
        {
            List<CashierTotalAmountsModel> results = endOfDay.GetCashierAmounts(DBInfo, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Cashier Total Income Amount  
        /// </summary>
        [HttpGet, Route("GetCashierTotalAmounts/{PosInfo}")]
        public HttpResponseMessage GetCashierTotalAmounts(long PosInfo)
        {
            CashierTotals results = endOfDay.GetCashierTotalAmounts(DBInfo, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type By Staff
        /// </summary>
        [HttpGet, Route("GetCashierAmountsByStaff/{StaffId}/{PosInfo}")]
        public HttpResponseMessage GetCashierAmountsByStaff(long StaffId, long PosInfo)
        {
            List<CashierTotalAmountsModel> results = endOfDay.GetCashierAmountsByStaff(DBInfo, StaffId, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Cashier Total Income Amount By Staff
        /// </summary>
        [HttpGet, Route("GetCashierTotalAmountsByStaff/{StaffId}/{PosInfo}")]
        public HttpResponseMessage GetCashierTotalAmountsByStaff(long StaffId, long PosInfo)
        {
            CashierTotals results = endOfDay.GetCashierTotalAmountsByStaff(DBInfo, StaffId, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false
        /// <param name="PosInfo"></param>
        /// </summary>
        [HttpGet, Route("GetTransactionCreditCardsCount/{PosInfo}")]
        public HttpResponseMessage GetTransactionCreditCardsCount(long PosInfo)
        {
            CreditCardsReceiptsCounts results = endOfDay.GetTransactionCreditCardsCount(DBInfo, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true and DA_IsPaid = false By StaffId
        /// <param name="PosInfo"></param>
        /// </summary>
        [HttpGet, Route("GetTransactionCreditCardsCountByStaff/{PosInfo}/{StaffId}")]
        public HttpResponseMessage GetTransactionCreditCardsCountByStaff(long PosInfo, long StaffId)
        {
            CreditCardsReceiptsCounts results = endOfDay.GetTransactionCreditCardsCountByStaff(DBInfo, PosInfo, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        [HttpPost, Route("UpdateStaffStatus")]
        public HttpResponseMessage UpdateStaffStatus()
        {
            long res = endOfDay.UpdateStaffStatus(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Tips Total
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        [HttpGet, Route("GetTipsTotal/{PosInfo}")]
        public HttpResponseMessage GetTipsTotal(long PosInfo)
        {
            decimal results = endOfDay.GetTipsTotal(DBInfo, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get Tips Total by Staff
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        [HttpGet, Route("GetTipsTotalByStaff/{StaffId}/{PosInfo}")]
        public HttpResponseMessage GetTipsTotalByStaff(long StaffId, long PosInfo)
        {
            decimal results = endOfDay.GetTipsTotalByStaff(DBInfo, StaffId, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns>
        ///<para> On success: 
        ///   EndOfDayPreviewModel: Contains the data for the (EndOf)Day Preview page for a specific POS. See: <see cref="Symposium.Models.Models.EndOfDayPreviewModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("Preview/{PosInfo}")]
        public HttpResponseMessage EndOfDayPreview(long PosInfo)
        {
            try
            {
                string customerClassType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
                EndOfDayPreviewModel results = endOfDay.GetPreview(PosInfo, DBInfo, customerClassType);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
            finally
            {
                Task.Run((() => GC.Collect()));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <returns>
        /// </returns>
        [HttpGet, Route("PreviewSalesType/{PosInfo}")]
        public HttpResponseMessage PreviewSalesType(long PosInfo)
        {
            List<EndOfDayTotalModel> results = endOfDay.PreviewSalesType(DBInfo, PosInfo);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        /// <summary>
        /// Return a list of invoices for a specific account ID (Cash, Credit Cards, Coplimentary each...) for the current day  (FOR ID 0)
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId, Cash (1), Credit Card (4) etc..</param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisByAccount/pos/{posInfo}/staff/{staffId}/account/{accountId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisByAccount(long posInfo, long staffId, long accountId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, accountId, pageNumber, pageLength, EndOfDayReceiptTypes.Default);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }


        /// <summary>
        /// Return a list of invoices for a specific account ID (Cash, Credit Cards, Coplimentary each...) for the current day (ID -99).
        /// see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisNotPaid/pos/{posInfo}/staff/{staffId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisNotPaid(long posInfo, long staffId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, 0, pageNumber, pageLength, EndOfDayReceiptTypes.NotPaid);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }


        /// <summary>
        /// Return a list of all printed and not voieded invoices for the current day (ID -100)
        /// see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisReceiptTotal/pos/{posInfo}/staff/{staffId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisReceiptTotal(long posInfo, long staffId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, 0, pageNumber, pageLength, EndOfDayReceiptTypes.ReceiptTotal);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }


        /// <summary>
        /// Return a list of not invoiced invoices for the current day (ID -101)
        /// see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisNotInvoiced/pos/{posInfo}/staff/{staffId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisNotInvoiced(long posInfo, long staffId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, 0, pageNumber, pageLength, EndOfDayReceiptTypes.NotInvoiced);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }


        /// <summary>
        /// Return a list of not invoiced invoices for the current day (ID -102)
        /// see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisCanceled/pos/{posInfo}/staff/{staffId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisCanceled(long posInfo, long staffId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, 0, pageNumber, pageLength, EndOfDayReceiptTypes.Canceled);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        /// <summary>
        /// Return a list of not printed invoices for the current day  (ID -103)
        /// see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisNotPrinted/pos/{posInfo}/staff/{staffId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisNotPrinted(long posInfo, long staffId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, 0, pageNumber, pageLength, EndOfDayReceiptTypes.NotPrinted);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }


        /// <summary>
        ///  Return a list of invoices with Discount for the current day  (ID -104)
        ///  see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisDiscount/pos/{posInfo}/staff/{staffId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisDiscount(long posInfo, long staffId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, 0, pageNumber, pageLength, EndOfDayReceiptTypes.DiscountTotal);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }


        /// <summary>
        ///  Return a list of invoices with Loyalty Discount for the current day  (ID -110)
        ///  see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("AnalysisLoyaltyDiscount/pos/{posInfo}/staff/{staffId}/page/{pageNumber}/length/{pageLength}")]
        public HttpResponseMessage AnalysisLoyaltyDiscount(long posInfo, long staffId, int pageNumber, int pageLength)
        {
            try
            {
                PaginationModel<EODAnalysisReceiptModel> results = endOfDay.GetAnalysis(DBInfo, posInfo, staffId, 0, pageNumber, pageLength, EndOfDayReceiptTypes.LoyaltyDiscountTotal);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }



        /// <summary>
        /// Return a list of all printed and not voieded invoices for the current day (ID -100)
        /// see <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="pageNumber">the number of current page (Start from 1. For 0 return the whole list as one page)</param>
        /// <param name="pageLength">the MAX number of items the page contains</param>
        /// <returns>
        ///<para> On success: 
        ///   PaginationModel: Model that contains the items of a single page (Pagination). See: <see cref="Symposium.Models.Models.PaginationModel"/>  </para>
        /// <para> On Failure:
        ///   404. Client must show the error message to user </para>
        /// </returns>
        [HttpGet, Route("TotalReportAnalysisByAccount/pos/{posInfo}/staff/{staffId}")]
        public HttpResponseMessage TotalReportAnalysisByAccount(long posInfo, long staffId)
        {
            try
            {
                EodTotalReportAnalysisModel results = endOfDay.GetTotalReportAnalysisByAccount(DBInfo, posInfo, staffId);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        /// <summary>
        /// Send EndOfDay X And Z Totals for printX to ExtEcr
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId</param>
        /// <returns></returns>
        [HttpGet, Route("XTotals/pos/{posInfo}/staff/{staffId}")]
        public HttpResponseMessage GetXTotals(long posInfo, long staffId)
        {
            logger.Info("End Of Day - X : PosInfo:" + posInfo.ToString() + ", StaffId:" + staffId.ToString());
            try
            {
                FODayFromPMSController fODayFromPMSController = new FODayFromPMSController();
                DateTime eodPMS = DateTime.Parse(fODayFromPMSController.GetFODay(DBInfo.Id.ToString(), posInfo));
                EodXAndZTotalsForExtecr results = endOfDay.EodXAndZ(DBInfo, posInfo, staffId, null, eodPMS, EndOfDayActions.PrintX);
                hub.Clients.Group(DBInfo.Id.ToString()).XReport(DBInfo.Id.ToString() + "|" + results.FiscalName, Newtonsoft.Json.JsonConvert.SerializeObject(results));
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        /// <summary>
        /// Send EndOfDay X And Z Totals for printZ to ExtEcr
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId</param>
        /// <returns></returns>
        [HttpGet, Route("ZTotals/pos/{posInfo}/staff/{staffId}")]
        public HttpResponseMessage GetZTotals(long posInfo, long staffId)
        {
            logger.Info(">> End Of Day - Z : PosInfo:" + posInfo.ToString() + ", StaffId:" + staffId.ToString());
            try
            {
                FODayFromPMSController fODayFromPMSController = new FODayFromPMSController();
                DateTime eodPMS = DateTime.Parse(fODayFromPMSController.GetFODay(DBInfo.Id.ToString(), posInfo));
                EodXAndZTotalsForExtecr results = endOfDay.EodXAndZ(DBInfo, posInfo, staffId, null, eodPMS, EndOfDayActions.PrintZ);
                hub.Clients.Group(DBInfo.Id.ToString()).zReport(DBInfo.Id.ToString() + "|" + results.FiscalName, Newtonsoft.Json.JsonConvert.SerializeObject(results));
                hub.Clients.Group(DBInfo.Id.ToString()).kdsMessage(DBInfo.Id.ToString() + "|" + results.FiscalName, Symposium.Resources.Messages.CLOSINGFODAY);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }
        }

        /// <summary>
        /// Send EndOfDay X And Z Totals for reprintZ to ExtEcr
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId</param>
        /// <param name="endOfDayId">Id of EndOfDay table</param>
        /// <returns></returns>
        [HttpGet, Route("ReprintZTotals/pos/{posInfo}/staff/{staffId}/eodId/{endOfDayId}")]
        public HttpResponseMessage GetReprintZTotals(long posInfo, long staffId, long endOfDayId)
        {
            logger.Info("End Of Day - Reprint Z : PosInfo:" + posInfo.ToString() + ", StaffId:" + staffId.ToString()+ ", endOfDayId: "+ endOfDayId.ToString());
            try
            {
                FODayFromPMSController fODayFromPMSController = new FODayFromPMSController();
                DateTime eodPMS = DateTime.Parse(fODayFromPMSController.GetFODay(DBInfo.Id.ToString(), posInfo));
                EodXAndZTotalsForExtecr results = endOfDay.EodXAndZ(DBInfo, posInfo, staffId, endOfDayId, eodPMS, EndOfDayActions.ReprintZ);
                hub.Clients.Group(DBInfo.Id.ToString()).zReport(DBInfo.Id.ToString() + "|" + results.FiscalName, Newtonsoft.Json.JsonConvert.SerializeObject(results));
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        /// <summary>
        /// Get Lockers Statistics
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <returns></returns>
        [HttpGet, Route("LockersStatistics/pos/{posInfo}")]
        public HttpResponseMessage GetLockersStatistics(long posInfo)
        {
            try
            {
                LockersStatisticsModel results = endOfDay.GetLockersStatistics(DBInfo, posInfo);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        /// <summary>
        /// Insert zlogger to endofday table
        /// </summary>
        /// <param name="zlogger">Pos</param>
        /// <returns></returns>
        [HttpGet, Route("InsertZlogger/{zlogger}")]
        public HttpResponseMessage InsertZlogger(string zlogger)
        {
            try
            {
                endOfDay.InsertZlogger(DBInfo, zlogger);
                return Request.CreateResponse(HttpStatusCode.OK); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        [HttpPost, Route("InsertStaffCash/StaffId/{StaffId}/CashAmount/{CashAmount}")]
        public HttpResponseMessage InsertStaffCash(long StaffId, decimal CashAmount)
        {
            long res = endOfDay.InsertStaffCash(DBInfo, StaffId, CashAmount);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
