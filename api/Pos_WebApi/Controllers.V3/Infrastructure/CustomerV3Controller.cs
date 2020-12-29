using Microsoft.AspNet.SignalR;
using PMSConnectionLib;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Symposium.Helpers.Classes;
using System;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Http;
using Pos_WebApi.Helpers.V3;
using Pos_WebApi.Repositories.PMSRepositories;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.Models.Models.Infrastructure;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Customer")]
    public class CustomerV3Controller : BasicV3Controller
    {
        ProtelRepository protelRepo;

        ICustomerFlows customerFlow;

        IHotelInfoFlows hotelInfoFlows;

        public CustomerV3Controller(ICustomerFlows customerFlow, IHotelInfoFlows hotelInfoFlows)
        {
            this.customerFlow = customerFlow;

            this.hotelInfoFlows = hotelInfoFlows;
        }

        /// <summary>
        /// search customer by vat number, first in database and if not found through taxis api
        /// </summary>
        /// <param name="afm">vat number to search for</param>
        /// <param name="hotelInfoId">hotelinfo id</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet, Route("SearchAFMCustomers/AFM/{afm}/HotelInfo/{hotelInfoId}")]
        public HttpResponseMessage SearchAFMCustomers(string afm, long hotelInfoId)
        {   
            HotelsInfoModel hotelInfo = hotelInfoFlows.selectHotelInfoByHotelId(DBInfo, hotelInfoId);
            
            protelRepo = new ProtelRepository(hotelInfo.ServerName, hotelInfo.DBUserName, hotelInfo.DBPassword, hotelInfo.DBName, hotelInfo.allHotels, hotelInfo.HotelType);

            IEnumerable<VATCustomerResultModel> res = protelRepo.GetCustomersByAFM(afm, DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}