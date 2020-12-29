using log4net;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class TableForDisplayController : ApiController
    {
    
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TableForDisplayController()
        {
         
        }


        public Object GetSigleTable(string storeid, long tableId)
        {

            using (PosEntities db = new PosEntities(false))
            {
                using (TableRepository tr = new TableRepository(db))
                {
                    var table = tr.GetSingleTable(tableId);
                    return table;
                }
            }
            
            
        }

        [Route("api/TableForDisplay/KitchenInstructions")]
        public IEnumerable<Object> GetKitchenInstructionsLog(string storeid, long tableId)
        {

            using (PosEntities db = new PosEntities(false))
            {
                using (TableRepository tr = new TableRepository(db))
                {
                    return tr.GetKitchenInstructionsForTable(tableId);
                }
            }    
        }


        public Object GetTableByRegionStatusOnly(string storeid, long regionId)
        {
            using (PosEntities db = new PosEntities(false))
            {
                using (TableRepository tr = new TableRepository(db))
                {
                    var a = tr.GetOpenTablesPerRegionStatusOnly(regionId);
                    return a;
                }
            }
        }

        /// <summary>
        /// get all tables for a specific pos ordered by Region and code
        /// </summary>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public IEnumerable<Object> GetAllTables(long posInfoId)
        {
            try
            {
                using (PosEntities db = new PosEntities(false))
                {
                    using (TableRepository tr = new TableRepository(db))
                    {
                        var result = tr.GetAllTables(posInfoId);
                        return result.ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
               Task.Run(()=> GC.Collect());
            }
        }


        [HttpPut]
        public HttpResponseMessage ChangeTable(string storeid, IEnumerable<ReceiptDetails> rds, long newTableId)
        {
            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository ir = new InvoiceRepository(db))
                {
                    if (rds == null)//(!ModelState.IsValid)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                    ir.ChangeItemTable(rds, newTableId);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        logger.Error(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }

        }


        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
           // db.Dispose();
            base.Dispose(disposing);
        }
    }

}
