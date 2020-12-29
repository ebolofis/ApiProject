using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Models;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.MealBoards;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Hotel
{
    public interface IHotelDT
    {
         List<VipModel> GetVip(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<TravelAgentModel> GetProtelTravelAgentList(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<ProductLookupHelper> GetFilteredProduct(DBInfoModel DBInfo, string name);
        List<TravelAgentModel> GetFilteredProtelTravelAgentList(DBInfoModel DBInfo, int mpehotel, string name);
        List<RoomModel> GetProtelRoomNo(DBInfoModel DBInfo,int hotelInfoId, int mpehotel);
         List<GroupModel> GetFilteredProtelGroupList(DBInfoModel DBInfo, int mpehotel, string name);
        List<SourceModel> GetSource(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        Dictionary<int, string> GetHotelInfo(DBInfoModel DBInfo);
        List<ProtelHotelModel> GetHotels(DBInfoModel DBInfo, int hotelInfoId);
        List<RoomTypeModel> GetRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<BookedRoomTypeModel> GetBookedRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<NationalityCodeModel> GetNationalityCode(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<BoardModel> GetBoards(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<CustomerModel> GetReservationsNew(DBInfoModel DBInfo, PosReservationHelper helper,int hotelInfoId, DateTime? dt);

        /// <summary>
        /// Return a List of CustomerModel based on Reservation Confirmation Code
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="helper"></param>
        /// <param name="hotelInfoId"></param>
        /// <returns></returns>
        List<CustomerModel> GetReservationByConfirmCode(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId);

        Dictionary<string, AllowanceModel> GetAllowance(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt);
        Dictionary<string, AllowanceModel> GetMacroOverride(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt);
        List<HotelAllowancesPerDay> GetHotelAllowances(DBInfoModel DBInfo, DateTime dateFrom, DateTime dateTo, int hotelInfoId, int mpehotel);
        List<CustomerModel> GetReservationsTimePeriod(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId, DateTime dateFrom, DateTime dateTo);
        void HandleHotelCustomerDataConfig(DBInfoModel DBInfo, List<Hotel__CustomerDataConfigModel> listmodel);
        List<GroupModel> GetGroups(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        int GetGuestId(DBInfoModel DBInfo, CustomerModel customer);
        int InsertGuest(DBInfoModel DBInfo, CustomerModel customer);
      //  void UpdateGuest(DBInfoModel DBInfo, CustomerModel customer, int id);
        void UpdateGuest(DBInfoModel DBInfo, CustomerModel customer, int? id);
        List<string> GetCustomerCustomMessages(DBInfoModel DBInfo, CustomerModel customer, List<CustomMessageModel> messages);
        List<MembershipModel> GetMemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        List<SubmembershipModel> GetSubmemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);
        void DeleteObsoleteMacros(DBInfoModel DBInfo);

        List<HotelPricelistModel> GetHotelPricelists(DBInfoModel DBInfo, int hotelInfoId, int mpehotel);

        ////////////////////////////////////////////////BO /////////////////////////////////////////////////////////////

        //-------------------------------CustomerDataConfig
        List<Hotel__CustomerDataConfigModel> GetCustomerDataConfig(DBInfoModel DBInfo);
        void InsertCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data);
        void UpdateCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data);
        void DeleteCustomerDataConfigField(DBInfoModel DBInfo, long Id);

        List<MacroModel> GetMacros(DBInfoModel DBInfo);
        List<ParamModel> GetParams(DBInfoModel DBInfo);

        Dictionary<int, int> GetGuestIds(DBInfoModel DBInfo, GuestIdsList guestIdsList);

        int GetGuestIdFromProfileAndRoom(DBInfoModel DBInfo, GuestGetIdModel guestGetIdModel);
    }
}