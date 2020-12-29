using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Pos_WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Pos_WebApi.Controllers {

    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/{storeId}/DayAudit")]
    public class DayAuditController : AppControllerBase {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
      //  private DayAuditRepository repo;

        public DayAuditController( ) {
           // repo = new DayAuditRepository(db);
        }

        /// <summary>
        /// Returns Not Audited sales (Ανάλυση Πληρωμών) by PosId for the current day
        /// </summary>
        /// <param name="storeId">StoreId Guid</param>
        /// <param name="posInfoId">Current Pos</param>
        /// <returns>
        /// <para>i.	Τα αθροίσματα (όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό) των παραστατικών ανά τρόπο πληρωμής (Account Type) </para>
        /// <para>ii. Τα αθροίσματα των μη εξοφλημένων αποδείξεων(όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό). Με εξοφλημένες είναι οι αποδείξεις που δεν υπάρχει για αυτέs εγγραφή στον transactions ή υπάρχει εγγραφή και είναι μερικώς εξοφλημένη.</para>
        /// <para>iii.Το άθροισμα των παραπάνω  (σύνολο Αποδείξεων, συνολικό ποσό)</para>
        /// <para>iv.Τη συνολική έκπτωση</para>
        /// <para> v.Τα αθροίσματα των ακυρωμένων αποδείξεων (όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό)</para>
        /// <para> vi.Το άθροισμα των μη τιμολογημένων αποδείξεων(όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό)</para>
        /// <para>vii.Ανάλυση couver(συνολικό ποσό)</para>
        /// </returns>
        [Route("{posInfoId}/ByPos")]
        [HttpGet]
        public IEnumerable<Object> GetDayAuditWithPosId( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSales(0, posInfoId).ToList();
                }
            }
        }

        /// <summary>
        /// Return details for not printed orders(sales) for the current day
        /// </summary>
        /// <param name="storeId">StoreId Guid</param>
        /// <param name="posInfoId">Current Pos</param>
        /// <returns></returns>
        [Route("{posInfoId}/ByPos/NotPrinted")]
        [HttpGet]
        public IEnumerable<Object> GetNotPrintedPosSales( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetNotPrintedPosSales(0, posInfoId).ToList();
                }
            }
        }

        /// <summary>
        /// Return not audited sales for specific staff by PosId 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/ByStaff")]
        [HttpGet]
        public IEnumerable<Object> GetDayAuditWithPosIdByStaff( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSalesPerStaff(null, posInfoId).ToList();
                }
            }
        }

        /// <summary>
        /// Return receipts (sales) for specific invoice type, PosId and the current day
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        [Route("{posInfoId}/ByinvoiceType/{invoiceType:int}")]
        [HttpGet]
        public IEnumerable<Object> GetDayAuditWithPosIdByInvoiceType( string storeId, long? posInfoId, int invoiceType ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSalesByInvoiceType(0, posInfoId, invoiceType).ToList();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/NotAuditedInvoicesByPos")]
        [HttpGet]
        public IEnumerable<Object> GetNotAuditedInvoicesByPos( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetAllPosSalesByInvoice(0, posInfoId).ToList();
                }
            }
        }

        [Route("{posInfoId}/NotAuditedInvoicesByStaff/{staffId}")]
        [HttpGet]
        public IEnumerable<Object> GetNotAuditedInvoicesByPos( string storeId, long? posInfoId, long? staffId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetAllPosSalesByStaff(0, posInfoId, staffId).ToList();
                }
            }

        }

        [Route("{departmentId}/MealConsumption/{posInfoId}")]
        [HttpGet]
        public IEnumerable<Object> GetMealConsumptionByPos( string storeId, long? posInfoId, long? departmentId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetConsumedMeals(0, departmentId, posInfoId).ToList();
                }
            }
        }

        [Route("{departmentId}/MealConsumptionByDepartment/")]
        [HttpGet]
        public IEnumerable<Object> GetMealConsumptionByDepartment( string storeId, long? posInfoId, long? departmentId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetConsumedMeals(0, departmentId, null).ToList();
                }
            }
          
        }


        /// <summary>
        /// Returns not audited sales for specific staff and invoice type by PosId and the current day
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="invoiceType"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/ByinvoiceType/{invoiceType:int}/Staff/{staffId}")]
        public IEnumerable<Object> GetDayAuditWithPosIdByInvoiceTypeAndStaff( string storeId, long? posInfoId, int invoiceType, long? staffId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSalesByInvoiceType(0, posInfoId, invoiceType).Where(w => w.StaffId == staffId).ToList();
                }
            }         
        }

        /// <summary>
        /// Return receipts (sales) for specific account type, PosId and the current day
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        [Route("{posInfoId}/ByAccountType/{accountType}")]
        public IEnumerable<Object> GetDayAuditWithPosIdByAccountType( string storeId, long? posInfoId, int accountType ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSalesByAccountType(0, posInfoId, accountType).ToList();
                }
            }
        }

        [Route("{departmentId}/AccountTypeByDepartment")]
        public IEnumerable<Object> GetDayAuditWithAccountTypeByDepartment( string storeId, long? departmentId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetDepartmentSalesByInvoiceType(0, departmentId).ToList();
                }
            }
        }



        [Route("{posInfoId}/SalesPerProductByCategories")]
        public IEnumerable<Object> GetSalesPerProductByCategories( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetSalesPerProductByCategories(0, null, posInfoId).ToList();

                }
            }
        }


        [Route("{posInfoId}/SalesPerProductByPricelists")]
        public IEnumerable<Object> GetSalesPerProductByPricelists( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetSalesPerProductByPricelists(0, null, posInfoId).ToList();
                }
            }    
        }

        [Route("{posInfoId}/HourlySalesPerProduct")]
        public IEnumerable<Object> GetHourlySalesPerProduct( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetHourlySalesPerProduct(0, null, posInfoId).ToList();
                }
            }
          
        }

        [Route("{posInfoId}/HourlySalesPerProductSalesType")]
        public IEnumerable<Object> GetHourlySalesPerProductSalesType( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetHourlySalesPerProductSalesType(0, null, posInfoId).ToList();
                }
            }
           
        }


        /// <summary>
        /// Returns not audited sales for specific staff and account type by PosId and the current day
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="accountType"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/ByAccountType/{accountType}/Staff/{staffId}")]
        public IEnumerable<Object> GetDayAuditWithPosIdByAccountType( string storeId, long? posInfoId, int accountType, long? staffId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSalesByAccountType(0, posInfoId, accountType).Where(w => w.StaffId == staffId).ToList();
                }
            }

        }

        /// <summary>
        /// Return not paid receipts (sales) for specific account type, PosId and the current day
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/UnpaidOnly")]
        public IEnumerable<Object> GetDayAuditWithPosIdUnpaidOnly( string storeId, long? posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSalesByNotPaid(0, posInfoId, null).ToList();
                }
            }
           
        }

        /// <summary>
        ///  Returns not audited and not paid sales for specific staff by PosId
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/UnpaidOnly/Staff/{staffId}")]
        public IEnumerable<Object> GetDayAuditWithPosIdUnpaidOnlyPerStaff( string storeId, long? posInfoId, long staffId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetPosSalesByNotPaid(0, posInfoId, staffId).ToList();
                }
            }       
        }

        /// <summary>
        /// Return XReport Data for not audited sales by posId
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/XTotals")]
        [HttpGet]
        public Object GetXTotals( string storeId, long posInfoId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    return repo.GetXTotals(posInfoId);
                }
            }
          
        }

        /// <summary>
        /// Day Auditing. 
        /// Returns ZReport Data for not audited sales by posId
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        [Route("{posInfoId}/ZTotals")]
        [HttpPost]
        public HttpResponseMessage GetZTotals( string storeId, long posInfoId, long staffId ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    try
                    {
                        var result = repo.GetZTotals(posInfoId, staffId, storeId);

                        return Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error GetZTotals :" + posInfoId + "|" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                    }
                    finally
                    {
                        Task.Run(() => GC.Collect());
                    }
                }
            }
           
        }

        [Route("{posInfoId}/ReprintZ/{eodId}/ExtecrName/{extecrName}")]
        [HttpGet]
        public HttpResponseMessage ReprintZ( string storeId, long posInfoId, long eodId, string extecrName, bool printReport ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    try
                    {
                        var result = repo.ReprintZ(posInfoId, 0, storeId, eodId);

                        if (printReport)
                        {
                            var zJson = JsonConvert.SerializeObject(result);
                            //hub.Clients.All.plainMessage(storeId + "|" + extecrName);

                            //hub.Clients.Group(storeId).newReceipt(storeId + "|" + extecrName, 76024, true, true);
                            hub.Clients.Group(storeId.ToLower()).zReport(storeId.ToLower() + "|" + extecrName, zJson);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error GetZTotals :" + posInfoId + "|" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                    }
                    finally
                    {
                        Task.Run(() => GC.Collect());
                    }
                }
            }
          
            // return;
        }


        /// <summary>
        /// For a given day return the list of z-reports per posinfo
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="foDay"></param>
        /// <returns></returns>
        [Route("AvailableZReports/{foDay}")]
        [HttpGet]
        public HttpResponseMessage AvailableZReports( string storeId, string foDay ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    try
                    {
                        DateTime? fodate = DateTime.Parse(foDay, this.clientLocalization);


                        try
                        {
                            var result = repo.AvailableZReports(fodate).ToList();
                            return Request.CreateResponse(HttpStatusCode.OK, result);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error GetZTotals :" + foDay + "|" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error AvailableZReports :" + foDay + " not valid" + "|" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                    }
                    finally
                    {
                        Task.Run(() => GC.Collect());
                    }
                }
            }

           
        }


        //[Route("{posInfoId}/AvailableZReports")]
        //[HttpGet]
        //public HttpResponseMessage AvailableZReports(string storeId, long posInfoId, long eodId)
        //{
        //    db = new PosEntities(false, Guid.Parse(storeId));
        //    repo = new DayAuditRepository(db);
        //    try
        //    {
        //        var result = repo.AvailableZReports(posInfoId);

        //        return Request.CreateResponse(HttpStatusCode.OK, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error GetZTotals :" + posInfoId + "|" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
        //    }
        //    // return;
        //}




        // Start New FODAY
        [Route("{posInfoId}/StartNewDay")]
        [HttpPost]
        public HttpResponseMessage StartNewAuditedDay( string storeId, long posInfoId, string foday ) {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (DayAuditRepository repo = new DayAuditRepository(db))
                {
                    repo.StartNewAuditedDay(posInfoId, foday);
                    try
                    {
                        if (repo.SaveChanges() == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        logger.Error(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error StartNewAuditedDay FoDay :" + foday + "|" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
          
        }


        [AllowAnonymous]
        [HttpOptions]
        public HttpResponseMessage Options( ) {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose( bool disposing ) {
            db.Dispose();

            base.Dispose(disposing);
        }

    }
}
