using Symposium.Models.Models.Infrastructure;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.Infrastructure
{
    [RoutePrefix("api/v3/Suppliers")]
    public class SuppliersV3Controller : BasicV3Controller
    {
        ISuppliersFlows suppliersFlows;

        public SuppliersV3Controller(ISuppliersFlows _suppliersFlows)
        {
            this.suppliersFlows = _suppliersFlows;
        }

        [HttpGet, Route("GetAll")]
        public HttpResponseMessage GetAllSuppliers()
        {
            List<SupplierModel> suppliers = suppliersFlows.GetSuppliers(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, suppliers);
        }

        [HttpPost, Route("Insert")]
        public HttpResponseMessage InsertSupplier(SupplierModel model)
        {
            List<SupplierModel> suppliers = suppliersFlows.InsertSupplier(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, suppliers);
        }

        [HttpPost, Route("Update")]
        public HttpResponseMessage UpdateSupplier(SupplierModel model)
        {
            List<SupplierModel> suppliers = suppliersFlows.UpdateSupplier(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, suppliers);
        }

        [HttpGet, Route("Delete/Ιd/{Id}")]
        public HttpResponseMessage DeleteSupplier(long Id)
        {
            List<SupplierModel> suppliers = suppliersFlows.DeleteSupplier(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, suppliers);
        }
    }
}
