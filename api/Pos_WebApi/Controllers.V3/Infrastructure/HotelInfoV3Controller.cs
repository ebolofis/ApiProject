using log4net;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Pos_WebApi.Controllers;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Models.TransferMappingsBO;
using Pos_WebApi.Modules;
using Pos_WebApi.Repositories.PMSRepositories;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.HotelInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


//Pos_WebApi.Controllers.V3
namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/HotelInfo")]
    public class HotelInfoV3Controller : BasicV3Controller
    {
        // IForkeyFlows forkyflow;
        IHotelInfoV3Flow hotelinfoFlows;

        public HotelInfoV3Controller(IHotelInfoV3Flow hotelInfoFlows)
        {
            this.hotelinfoFlows = hotelInfoFlows;

        }


        [HttpGet, Route("GetHotelInfo")]

        public HttpResponseMessage GetHotelInfo()
        {
            try
            {
                List<HotelsInfoModel> res = hotelinfoFlows.GetHotelInfo(DBInfo);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        [HttpGet, Route("GetHotelInfoBase")]
        public HttpResponseMessage GetHotelInfoBase()
        {
            try
            {
                List<HotelInfoBaseModel> res = hotelinfoFlows.GetHotelInfoBase(DBInfo);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet, Route("GetPMSDepartments/HotelInfoId/{hoteInfoId}")]
        public HttpResponseMessage GetPMSDepartmentsByHotelInfoId(long hoteInfoId)
        {
            try
            {
                HotelsInfoModel hotelInfo = hotelinfoFlows.GetHotelInfoById(DBInfo, hoteInfoId);
                ProtelRepository protelRepo = new ProtelRepository(hotelInfo.ServerName, hotelInfo.DBUserName, hotelInfo.DBPassword, hotelInfo.DBName, hotelInfo.allHotels, hotelInfo.HotelType);
                short mpeHotel = hotelInfo.MPEHotel ?? 0;
                var res = protelRepo.GetPMSDepartments(mpeHotel);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPost, Route("GetPMSDepartments")]

        public HttpResponseMessage GetPMSDepartments(HotelsInfoModel model)
        {
            try
            {
                ProtelRepository protelRepo = new ProtelRepository(model.ServerName, model.DBUserName, model.DBPassword, model.DBName, model.allHotels, model.HotelType);
                short mpeHotel = model.MPEHotel ?? 0;
                var res = protelRepo.GetPMSDepartments(mpeHotel);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        [HttpPost, Route("GetTransferMappings")]

        public HttpResponseMessage GetTransferMappings(TransferMappingsHelper obj)
        {
            try
            {
                List<TransferMappingsModel> res = hotelinfoFlows.GetTransferMappings(DBInfo, obj.HotelId, obj.ProdCatId);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }
        [HttpPost, Route("UpdateTransferMappings")]
        public HttpResponseMessage UpdateTransferMappings(UpdateTransferMappingsModel obj)
        {
            try
            {
                hotelinfoFlows.UpdateTransferMappings(DBInfo, obj.newPmsDepartmentId, obj.newPmsDescr, obj.ProdCatId, obj.HotelId, obj.OldPmsDepartmentId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }

        }
    }
}
