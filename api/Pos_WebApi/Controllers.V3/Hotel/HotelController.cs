using Symposium.Models.Models.Hotel;
using Symposium.WebApi.Controllers.V3;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using log4net;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Models.MealBoards;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Hotel;
using Symposium.Models.Models.Helper;
using Newtonsoft.Json;
using Symposium.Models.Models;

namespace Pos_WebApi.Controllers.V3.Hotel
{
    [RoutePrefix("api/v3/Hotel")]
    public class HotelController : BasicV3Controller
    {
        IHotelFlows flow;

        public HotelController(IHotelFlows flow)
        {
            this.flow = flow; 
        }


        #region protelLists
        [HttpGet, Route("GetFilteredProduct/{name}")]
        public HttpResponseMessage GetFilteredProduct( string name)
        {
            List<ProductLookupHelper> response = flow.GetFilteredProduct(DBInfo, name);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetFilteredProtelGroupList/{mpehotel}/{name}")]
        public HttpResponseMessage GetFilteredProtelGroupList(int mpehotel, string name)
        {
            List<GroupModel> response = flow.GetFilteredProtelGroupList(DBInfo, mpehotel, name);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetFilteredProtelTravelAgentList/{mpehotel}/{name}")]
        public HttpResponseMessage GetFilteredProtelTravelAgentList( int mpehotel, string name)
        {
            List<TravelAgentModel> response = flow.GetFilteredProtelTravelAgentList(DBInfo,  mpehotel,name);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetProtelTravelAgentList/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetProtelTravelAgentList(int hotelInfoId, int mpehotel)
        {
            List<TravelAgentModel> response = flow.GetProtelTravelAgentList(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetProtelRoomNo/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetProtelRoomNo(int hotelInfoId, int mpehotel)
        {
            List<RoomModel> response = flow.GetProtelRoomNo(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetSource/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetSource(int hotelInfoId, int mpehotel)
        {
            List<SourceModel> response = flow.GetSource(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetHotelInfo")]
        public HttpResponseMessage GetHotelInfo()
        {
            Dictionary<int, string> response = flow.GetHotelInfo(DBInfo);
            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetHotels/{hotelInfoId}")]
        public HttpResponseMessage GetHotels(int hotelInfoId)
        {
            List<ProtelHotelModel> response = flow.GetHotels(DBInfo, hotelInfoId);
            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetRoomType/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetRoomType(int hotelInfoId, int mpehotel)
        {
            List<RoomTypeModel> response = flow.GetRoomType(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpGet, Route("GetBookedRoomType/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetBookedRoomType(int hotelInfoId, int mpehotel)
        {
            List<BookedRoomTypeModel> response = flow.GetBookedRoomType(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetNationalityCode/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetNationalityCode(int hotelInfoId, int mpehotel)
        {
            List<NationalityCodeModel> response = flow.GetNationalityCode(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpGet, Route("GetVip/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetVip(int hotelInfoId,int mpehotel)
        {
            List<VipModel> response = flow.GetVip(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetGroups/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetGroups(int hotelInfoId,int mpehotel)
        {
           
            List<GroupModel> response = flow.GetGroups(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpGet, Route("GetBoards/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetBoards(int hotelInfoId,int mpehotel)
        {
            List<BoardModel> response = flow.GetBoards(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetHotelPricelists/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetHotelPricelists(int hotelInfoId, int mpehotel)
        {
            List<HotelPricelistModel> response = flow.GetHotelPricelists(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpGet, Route("GetMemberships/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetMemberships(int hotelInfoId, int mpehotel)
        {
            List<MembershipModel> response = flow.GetMemberships(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetSubmemberships/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetSubmemberships(int hotelInfoId, int mpehotel)
        {
            List<SubmembershipModel> response = flow.GetSubmemberships(DBInfo, hotelInfoId, mpehotel);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion protelLists

        #region allowance
        [HttpPost, Route("GetAllowance")]
        public HttpResponseMessage GetAllowance(PosReservationAllowanceHelper data)
        {
            Dictionary<string, AllowanceModel> response = flow.GetAllowance(DBInfo, data.helper, data.customer, data.dt, data.ovride);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetHotelAllowances/{dateFrom}/{dateTo}/{hotelInfoId}/{mpehotel}")]
        public HttpResponseMessage GetHotelAllowances(DateTime? dateFrom, DateTime? dateTo, int hotelInfoId, int mpehotel)
        {
            if (dateFrom == null)
            {
                dateFrom = DateTime.Now.AddDays(- 1);
            }

            if (dateTo == null)
            {
                dateTo = DateTime.Now.AddDays(6);
            }

            List<HotelAllowancesPerDay> response = flow.GetHotelAllowances(DBInfo, Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo), hotelInfoId, mpehotel);
            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion allowance

        #region reservations
        [HttpPost, Route("GetReservationsTimePeriod")]
        public HttpResponseMessage GetReservationsTimePeriod(PosReservationPeriodHelper helperPeriod) 
        {
            List<CustomerModel> response = flow.GetReservationsTimePeriod(DBInfo, helperPeriod.helper, helperPeriod.hotelInfoId, helperPeriod.dateFrom, helperPeriod.dateTo);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost, Route("GetReservationsNew")]
        public HttpResponseMessage GetReservationsNew(PosReservationDateHelper reservationDateHelper)
        {
            List<CustomerModel> response = flow.GetReservationsNew(DBInfo, reservationDateHelper.helper, reservationDateHelper.hotelInfoId, reservationDateHelper.dt);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Return a List of CustomerModel based on Reservation Confirmation Code
        /// </summary>
        /// <param name="reservationDateHelper"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReservationByReservationCode")]
        public HttpResponseMessage GetReservationByReservationCode(PosReservationDateHelper reservationDateHelper)
        {
            List<CustomerModel> response = flow.GetReservationByConfirmCode(DBInfo, reservationDateHelper.helper, reservationDateHelper.hotelInfoId);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


#endregion reservations

        #region customer data config

        [HttpGet, Route("GetCustomerDataConfig")]
        public HttpResponseMessage GetCustomerDataConfig()
        {
            List<Hotel__CustomerDataConfigModel> response = flow.GetCustomerDataConfig(DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        [HttpPost, Route("InsertCustomerDataConfig")]
        public HttpResponseMessage InsertCustomerDataConfig(Hotel__CustomerDataConfigModel data)
        {
            flow.InsertCustomerDataConfig(DBInfo, data);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost, Route("UpdateCustomerDataConfig")]
        public HttpResponseMessage UpdateCustomerDataConfig(Hotel__CustomerDataConfigModel data)
        {
            flow.UpdateCustomerDataConfig(DBInfo, data);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet, Route("DeleteCustomerDataConfigField/{Id}")]
        public HttpResponseMessage DeleteCustomerDataConfigField(long Id)
        {
            flow.DeleteCustomerDataConfigField(DBInfo, Id);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion customer data config

        #region custom messages


        [HttpGet, Route("GetParams")]
        public HttpResponseMessage GetParams()
        {
           List<ParamModel> response = flow.GetParams(DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        [HttpGet, Route("GetCustomMessages")]
        public HttpResponseMessage GetCustomMessages()
        {
            List<CustomMessageModel> response = flow.GetCustomMessages(DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("GetCustomMessage/{Id}")]
        public HttpResponseMessage GetCustomMessage(Guid Id)
        {
            CustomMessageModel response = flow.GetCustomMessage(DBInfo, Id);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
               
        [HttpPost, Route("InsertCustomMessage")]
        public HttpResponseMessage InsertCustomMessage(CustomMessageModel data)
        {
            flow.InsertCustomMessage(DBInfo, data);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost, Route("UpdateCustomMessage")]
        public HttpResponseMessage UpdateCustomMessage(CustomMessageModel data)
        {
            flow.UpdateCustomMessage(DBInfo, data);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet, Route("DeleteCustomMessage/{Id}")]
        public HttpResponseMessage DeleteCustomMessage(Guid Id)
        {
            flow.DeleteCustomMessage(DBInfo, Id);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion custom messages

        #region macros
        [HttpGet, Route("GetMacros")]
        public HttpResponseMessage GetMacros()
        {
            List<MacroModel> response = flow.GetMacros(DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost, Route("InsertMacros")]
        public HttpResponseMessage InsertMacros(MacroModel json)
        {
            Guid?  result = flow.InsertMacros(DBInfo, json);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost, Route("UpdateMacros")]
        public HttpResponseMessage UpdateMacros(MacroModel json)
        {

            flow.UpdateMacros(DBInfo, json);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

       
        [HttpPost, Route("HandleHotelCustomerDataConfig")]
        public HttpResponseMessage HandleHotelCustomerDataConfig(List<Hotel__CustomerDataConfigModel> model)
        {

            flow.HandleHotelCustomerDataConfig(DBInfo, model);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        
        [HttpPost, Route("UpdateMacroRemainingConsumption")]
        public HttpResponseMessage UpdateMacroRemainingConsumption(UpdateConsumptionModel data)
        {
            if(data.macroGuid == null || data.couver <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }

            bool response = flow.UpdateMacroRemainingConsumption(DBInfo, data);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet, Route("DeleteMacros/{Id}")]
        public HttpResponseMessage DeleteMacros(Guid Id)
        {
             flow.DeleteMacros(DBInfo, Id);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost, Route("DeleteObsoleteMacros")]
        public async Task<HttpResponseMessage> DeleteObsoleteMacros()
        {
            await Task.Run(() => {
                flow.DeleteObsoleteMacros(DBInfo);
            });

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        #endregion macros

            #region timezones
        [HttpGet, Route("GetTimezones")]
        public HttpResponseMessage GetTimezones()
        {
            List<MacroTimezoneModel> response = flow.GetTimezones(DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost, Route("InsertTimezones")]
        public HttpResponseMessage InsertTimezones(MacroTimezoneModel json)
        {
            bool flag = false;
            flag=flow.InsertTimezones(DBInfo, json);
            if (flag == false)
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateResponse(HttpStatusCode.Conflict);
        }
        [HttpPost, Route("UpdateTimezones")]
        public HttpResponseMessage UpdateTimezones(MacroTimezoneModel json)
        {
            bool flag = false;
            flag = flow.UpdateTimezones(DBInfo, json);

            if(flag==false)
            return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateResponse(HttpStatusCode.Conflict);
        }

        [HttpGet, Route("DeleteTimezones/{Id}/{Code}")]
        public HttpResponseMessage DeleteTimezones(Guid Id,string Code)
        {
           int res= flow.DeleteTimezones(DBInfo, Id,Code);

            if (res == 0)
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateResponse(HttpStatusCode.PreconditionFailed);

        }
        
        [HttpPost, Route("ValidateTimezoneExpression")]
        public HttpResponseMessage ValidateTimezoneExpression(TimezoneExpressionModel model)
        {
            List<string> result = flow.ValidateTimezoneExpression(DBInfo, model.timezoneExpression);
            
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        #endregion timezones

        #region GuestId
        [HttpPost, Route("GetGuestIds")]
        public HttpResponseMessage GetGuestIds(GuestIdsList guestIdsList)
        {
            Dictionary<int, int> response = flow.GetGuestIds(DBInfo, guestIdsList);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion GuestId

    }
}