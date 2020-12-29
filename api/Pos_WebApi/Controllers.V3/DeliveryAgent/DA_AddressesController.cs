using Pos_WebApi.Modules;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    [RoutePrefix("api/v3/da/Address")]
    public class DA_AddressesController : BasicV3Controller
    {
        IDA_AddressesFlows addressesFlow;

        public DA_AddressesController(IDA_AddressesFlows _addressesFlow)
        {
            this.addressesFlow = _addressesFlow;
        }

        /// <summary>
        /// Add new Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage AddAddress(DA_AddressModel Model)
        {
            long res = addressesFlow.AddAddress(DBInfo, Model,CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update an Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage UpdateAddress(DA_AddressModel Model)
        {
            long res = addressesFlow.UpdateAddress(DBInfo, Model, CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update address with phone and clear same phone from other addresses
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateAddressPhone")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage UpdateAddressPhone(DA_AddressPhoneModel model)
        {
            addressesFlow.UpdateAddressPhone(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Delete an Address 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("Delete/Id/{Id}")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage DeleteAddress(long Id)
        {
            long res = addressesFlow.DeleteAddress(DBInfo, Id, CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Retreive Coordinate Informations From Google or Terra Maps by giving an Address Model
        /// </summary>
        /// <param name="Model">DA_AddressModel </param>
        /// <returns>DA_AddressModel</returns>
        [HttpPost, Route("GeoLocationMaps")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage GeoLocationMaps(DA_AddressModel Model)
        {
            DA_AddressModel res = addressesFlow.GeoLocationMaps(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Retreive Coordinate Informations From Google or Terra Maps by giving an Address Model
        /// </summary>
        /// <param name="Model">DA_AddressModel </param>
        /// <returns>DA_AddressModel</returns>
        [HttpGet, Route("createphonetics")]
        [AllowAnonymous]
        // [ValidateModelState]
        // [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage CreateAllAddressPhonetics()
        {
            addressesFlow.CreateAllAddressPhonetics(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}