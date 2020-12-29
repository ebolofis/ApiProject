using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.MealBoards;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Hotel;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Hotel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Hotel
{
    public class HotelFlows : IHotelFlows
    {
        IHotelTasks hotelTasks;

        public HotelFlows(IHotelTasks hotelTasks)
        {
            this.hotelTasks = hotelTasks;
        }

        public List<ProductLookupHelper> GetFilteredProduct(DBInfoModel DBInfo, string name)
        {
            return hotelTasks.GetFilteredProduct(DBInfo, name);
        }
        public List<TravelAgentModel> GetFilteredProtelTravelAgentList(DBInfoModel DBInfo, int mpehotel, string name)
        {
            return hotelTasks.GetFilteredProtelTravelAgentList(DBInfo, mpehotel, name);
        }
        public List<GroupModel> GetFilteredProtelGroupList(DBInfoModel DBInfo, int mpehotel, string name)
        {
            return hotelTasks.GetFilteredProtelGroupList(DBInfo, mpehotel, name);
        }
        public List<CustomerModel> GetReservationsNew(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId, DateTime? dt)
        {
            //checks if nedd to call Api Url for Hotelizer or other external systems
            bool extApi = false;
            extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
            if (extApi)
            {
                HotelizerFlows hotelizer = new HotelizerFlows();
                return hotelizer.GetRoomsAsCustomerModel(helper.Room, 0, helper.Page ?? 0, helper.Pagesize ?? 12);
            }
            else
                return hotelTasks.GetReservationsNew(DBInfo, helper, hotelInfoId, dt);
        }

        /// <summary>
        /// Return a List of CustomerModel based on Reservation Confirmation Code
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="helper"></param>
        /// <param name="hotelInfoId"></param>
        /// <returns></returns>
        public List<CustomerModel> GetReservationByConfirmCode(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId)
        {
            return hotelTasks.GetReservationByConfirmCode(DBInfo, helper, hotelInfoId);
        }

        public List<TravelAgentModel> GetProtelTravelAgentList(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetProtelTravelAgentList(DBInfo, hotelInfoId, mpehotel);
        }
        public List<RoomModel> GetProtelRoomNo(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetProtelRoomNo(DBInfo, hotelInfoId, mpehotel);
        }
        public List<SourceModel> GetSource(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetSource(DBInfo, hotelInfoId, mpehotel);
        }
        public Dictionary<int, string> GetHotelInfo(DBInfoModel DBInfo)
        {
            return hotelTasks.GetHotelInfo(DBInfo);
        }
        public List<ProtelHotelModel> GetHotels(DBInfoModel DBInfo, int hotelInfoId)
        {
            return hotelTasks.GetHotels(DBInfo, hotelInfoId);
        }
        public List<RoomTypeModel> GetRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetRoomType(DBInfo, hotelInfoId, mpehotel);
        }
        public List<VipModel> GetVip(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetVip(DBInfo, hotelInfoId, mpehotel);
        }
        public List<GroupModel> GetGroups(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetGroups(DBInfo, hotelInfoId, mpehotel);
        }

        public List<BookedRoomTypeModel> GetBookedRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetBookedRoomType(DBInfo, hotelInfoId, mpehotel);
        }
        public List<NationalityCodeModel> GetNationalityCode(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetNationalityCode(DBInfo, hotelInfoId, mpehotel);
        }
        public List<BoardModel> GetBoards(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetBoards(DBInfo, hotelInfoId, mpehotel);
        }

        public List<HotelPricelistModel> GetHotelPricelists(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetHotelPricelists(DBInfo, hotelInfoId, mpehotel);
        }

        public List<MembershipModel> GetMemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetMemberships(DBInfo, hotelInfoId, mpehotel);
        }

        public List<SubmembershipModel> GetSubmemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetSubmemberships(DBInfo, hotelInfoId, mpehotel);
        }

        public Dictionary<string, AllowanceModel> GetAllowance(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt, int ovride = 0)
        {
            if (ovride == 0)
            {
                int GuestId = hotelTasks.GetGuestId(DBInfo, customer);

                Dictionary<string, AllowanceModel> result = hotelTasks.GetAllowance(DBInfo, helper, customer, dt);

                List<string> messages = hotelTasks.GetCustomerCustomMessages(DBInfo, customer);

                if (result != null)
                {
                    result.Values.First().GuestId = GuestId;

                    result.Values.First().messages = messages;
                }

                return result;
            }
            else
            {
                return hotelTasks.GetMacroOverride(DBInfo, helper, customer, dt);
            }
        }

        public List<HotelAllowancesPerDay> GetHotelAllowances(DBInfoModel DBInfo, DateTime dateFrom, DateTime dateTo, int hotelInfoId, int mpehotel)
        {
            return hotelTasks.GetHotelAllowances(DBInfo, dateFrom, dateTo, hotelInfoId, mpehotel);
        }
        public List<MacroModel> GetMacros(DBInfoModel DBInfo)
        {
            return hotelTasks.GetMacros(DBInfo);
        }
        public void DeleteMacros(DBInfoModel DBInfo, Guid Id)
        {
            hotelTasks.DeleteMacros(DBInfo, Id);
        }
        public List<MacroTimezoneModel> GetTimezones(DBInfoModel DBInfo)
        {
            return hotelTasks.GetTimezones(DBInfo);
        }
        public int DeleteTimezones(DBInfoModel DBInfo, Guid Id, string Code)
        {
            return hotelTasks.DeleteTimezones(DBInfo, Id, Code);
        }

        public bool InsertTimezones(DBInfoModel DBInfo, MacroTimezoneModel json)
        {
            return hotelTasks.InsertTimezones(DBInfo, json);
        }
        public bool UpdateTimezones(DBInfoModel DBInfo, MacroTimezoneModel json)
        {
            return hotelTasks.UpdateTimezones(DBInfo, json);
        }
        public void DeleteTimezones(DBInfoModel DBInfo, MacroTimezoneModel json)
        {
            hotelTasks.InsertTimezones(DBInfo, json);
        }

        public List<string> ValidateTimezoneExpression(DBInfoModel DBInfo, string timezoneExpression)
        {
            return hotelTasks.ValidateTimezoneExpression(DBInfo, timezoneExpression);
        }

        public Guid? InsertMacros(DBInfoModel DBInfo, MacroModel json)
        {
            return hotelTasks.InsertMacros(DBInfo, json);
        }
        public void UpdateMacros(DBInfoModel DBInfo, MacroModel json)
        {
            hotelTasks.UpdateMacros(DBInfo, json);
        }
        public void DeleteObsoleteMacros(DBInfoModel DBInfo)
        {
            hotelTasks.DeleteObsoleteMacros(DBInfo);
        }

        public List<CustomerModel> GetReservationsTimePeriod(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId, DateTime dateFrom, DateTime dateTo)
        {
            //checks if nedd to call Api Url for Hotelizer or other external systems
            bool extApi = false;
            extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
            if (extApi)
            {
                HotelizerFlows hotelizer = new HotelizerFlows();
                return hotelizer.GetRoomsAsCustomerModel(helper.Room, 0, helper.Page ?? 0, helper.Pagesize ?? 12, dateFrom, dateTo);
            }
            else
                return hotelTasks.GetReservationsTimePeriod(DBInfo, helper, hotelInfoId, dateFrom, dateTo);
        }

        public List<Hotel__CustomerDataConfigModel> GetCustomerDataConfig(DBInfoModel DBInfo)
        {
            return hotelTasks.GetCustomerDataConfig(DBInfo);
        }

        public void InsertCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data)
        {
            hotelTasks.InsertCustomerDataConfig(DBInfo, data);
        }

        public void HandleHotelCustomerDataConfig(DBInfoModel DBInfo, List<Hotel__CustomerDataConfigModel> listmodel)
        {
            hotelTasks.HandleHotelCustomerDataConfig(DBInfo, listmodel);
        }

        public void UpdateCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data)
        {
            hotelTasks.UpdateCustomerDataConfig(DBInfo, data);
        }

        public void DeleteCustomerDataConfigField(DBInfoModel DBInfo, long Id)
        {
            hotelTasks.DeleteCustomerDataConfigField(DBInfo, Id);
        }


        public List<ParamModel> GetParams(DBInfoModel DBInfo)
        {
            return hotelTasks.GetParams(DBInfo);
        }
        public List<CustomMessageModel> GetCustomMessages(DBInfoModel DBInfo)
        {
            return hotelTasks.GetCustomMessages(DBInfo);
        }

        public CustomMessageModel GetCustomMessage(DBInfoModel DBInfo, Guid Id)
        {
            return hotelTasks.GetCustomMessage(DBInfo, Id);
        }

        public void InsertCustomMessage(DBInfoModel DBInfo, CustomMessageModel data)
        {
            hotelTasks.InsertCustomMessage(DBInfo, data);
        }

        public void UpdateCustomMessage(DBInfoModel DBInfo, CustomMessageModel data)
        {
            hotelTasks.UpdateCustomMessage(DBInfo, data);
        }

        public void DeleteCustomMessage(DBInfoModel DBInfo, Guid Id)
        {
            hotelTasks.DeleteCustomMessage(DBInfo, Id);
        }

        public bool UpdateMacroRemainingConsumption(DBInfoModel DBInfo, UpdateConsumptionModel data)
        {
            return hotelTasks.UpdateMacroRemainingConsumption(DBInfo, data);
        }

        public Dictionary<int, int> GetGuestIds(DBInfoModel DBInfo, GuestIdsList guestIdsList)
        {
            return hotelTasks.GetGuestIds(DBInfo, guestIdsList);
        }

        public int GetGuestIdFromProfileAndRoom(DBInfoModel DBInfo, GuestGetIdModel guestGetIdModel)
        {
            return hotelTasks.GetGuestIdFromProfileAndRoom(DBInfo, guestGetIdModel);
        }

        public bool IsCustomerCheckOut(DBInfoModel dbInfo, HotelsInfoModel hotelInfo, PMSCustomerModel customer)
        {
            bool isCheckedOut = true;
            PosReservationHelper reservationHelperModel = hotelTasks.CreateReservationHelperModel(customer, hotelInfo);
            List<CustomerModel> currentCustomers;

            //checks if nedd to call Api Url for Hotelizer or other external systems
            bool extApi = false;
            extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
            if (extApi)
            {
                HotelizerFlows hotelizer = new HotelizerFlows();
                currentCustomers = hotelizer.GetRoomsAsCustomerModel(reservationHelperModel.Room, 0, reservationHelperModel.Page ?? 0, reservationHelperModel.Pagesize ?? 12);
            }
            else
                currentCustomers = hotelTasks.GetReservationsNew(dbInfo, reservationHelperModel, (int)hotelInfo.Id, null);

            if (currentCustomers != null)
            {
                isCheckedOut = hotelTasks.IsCustomerCheckOut(hotelInfo, customer, currentCustomers);
            }
            return isCheckedOut;
        }
    }
}
