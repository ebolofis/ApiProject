﻿using Symposium.Models.Models;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.MealBoards;
using Symposium.Models.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Hotel
{
    public interface IHotelFlows
    {
        List<VipModel> GetVip(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<ProductLookupHelper> GetFilteredProduct(DBInfoModel DBInfo, string name);
        List<TravelAgentModel> GetProtelTravelAgentList(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<TravelAgentModel> GetFilteredProtelTravelAgentList(DBInfoModel DBInfo, int mpehotel, string name);
        List<GroupModel> GetFilteredProtelGroupList(DBInfoModel DBInfo, int mpehotel, string name);
        List<RoomModel> GetProtelRoomNo(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<SourceModel> GetSource(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        Dictionary<int, string> GetHotelInfo(DBInfoModel DBInfo);
        List<ProtelHotelModel> GetHotels(DBInfoModel DBInfo, int hotelInfoId);
        List<RoomTypeModel> GetRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<BookedRoomTypeModel> GetBookedRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<NationalityCodeModel> GetNationalityCode(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<BoardModel> GetBoards(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<CustomerModel> GetReservationsNew(DBInfoModel DBInfo, PosReservationHelper helper, int hotelinfoId, DateTime? dt);

        /// <summary>
        /// Return a List of CustomerModel based on Reservation Confirmation Code
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="helper"></param>
        /// <param name="hotelInfoId"></param>
        /// <returns></returns>
        List<CustomerModel> GetReservationByConfirmCode(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId);

        Dictionary<string, AllowanceModel> GetAllowance(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt, int ovride);
        List<HotelAllowancesPerDay> GetHotelAllowances(DBInfoModel DBInfo, DateTime dateFrom, DateTime dateTo, int hotelInfoId, int mpehotel);

        List<GroupModel> GetGroups(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<CustomerModel> GetReservationsTimePeriod(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId, DateTime dateFrom, DateTime dateTo);
        List<MembershipModel> GetMemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<SubmembershipModel> GetSubmemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);

        List<HotelPricelistModel>  GetHotelPricelists(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        bool UpdateMacroRemainingConsumption(DBInfoModel DBInfo, UpdateConsumptionModel data);

        ///////////////////////////////////// BO ///////////////////////////////////////////

        //-------------------------------CustomMessages
        List<ParamModel> GetParams(DBInfoModel DBInfo);

        List<CustomMessageModel> GetCustomMessages(DBInfoModel DBInfo);
        CustomMessageModel GetCustomMessage(DBInfoModel DBInfo, Guid Id);
        void InsertCustomMessage(DBInfoModel DBInfo, CustomMessageModel data);
        void UpdateCustomMessage(DBInfoModel DBInfo, CustomMessageModel data);
        void DeleteCustomMessage(DBInfoModel DBInfo, Guid Id);

        //-------------------------------CustomerDataConfig
        List<Hotel__CustomerDataConfigModel> GetCustomerDataConfig(DBInfoModel DBInfo);
        void InsertCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data);
        void UpdateCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data);
        void DeleteCustomerDataConfigField(DBInfoModel DBInfo, long Id);
        void HandleHotelCustomerDataConfig(DBInfoModel DBInfo, List<Hotel__CustomerDataConfigModel> listmodel);

        //-------------------------------Macros
        List<MacroModel> GetMacros(DBInfoModel DBInfo);
        void DeleteMacros(DBInfoModel DBInfo, Guid Id);
        void UpdateMacros(DBInfoModel DBInfo, MacroModel json);
        Guid? InsertMacros(DBInfoModel DBInfo, MacroModel json);
        void DeleteObsoleteMacros(DBInfoModel DBInfo);
        List<string> ValidateTimezoneExpression(DBInfoModel DBInfo, string timezoneExpression);

        //-------------------------------Timezones
        bool InsertTimezones(DBInfoModel DBInfo, MacroTimezoneModel json);

        int DeleteTimezones(DBInfoModel DBInfo, Guid Id, string Code);
        bool UpdateTimezones(DBInfoModel DBInfo, MacroTimezoneModel json);

        List<MacroTimezoneModel> GetTimezones(DBInfoModel DBInfo);

        Dictionary<int, int> GetGuestIds(DBInfoModel DBInfo, GuestIdsList guestIdsList);

        bool IsCustomerCheckOut(DBInfoModel dbInfo, HotelsInfoModel hotelInfo, PMSCustomerModel customer);

        int GetGuestIdFromProfileAndRoom(DBInfoModel DBInfo, GuestGetIdModel guestGetIdModel);
    }
}