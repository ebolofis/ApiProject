using log4net;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.MealBoards;
using Symposium.WebApi.DataAccess.DT.Hotel;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.DT.Hotel;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Hotel;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Helpers;
using Symposium.WebApi.DataAccess.Interfaces.DT.CashedObjects;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Net.Http;
using System.Globalization;
using Symposium.Models.Models.Orders;
using static Symposium.Models.Models.MealBoards.CustomerModel;

namespace Symposium.WebApi.MainLogic.Tasks.Hotel
{
    public class HotelTasks : IHotelTasks
    {
        IHotelDT hoteldt;
        //  IHotelMacrosDT hotelMacrosDT;
         IHotelMacroTimezoneDT timezoneDT;
        IHotelMacrosDT hotelMacrosDT;
        IHotelCustomMessagesDT customMessagesDT;
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
 
        public HotelTasks(IHotelDT hoteldt, IHotelMacrosDT hotelMacrosDT, IHotelMacroTimezoneDT timezoneDT, IHotelCustomMessagesDT customMessagesDT)
        {
            this.hoteldt = hoteldt;
            this.timezoneDT = timezoneDT;
            this.hotelMacrosDT = hotelMacrosDT;
            this.customMessagesDT = customMessagesDT;
        }

        public void HandleHotelCustomerDataConfig(DBInfoModel DBInfo, List<Hotel__CustomerDataConfigModel> listmodel)
        {
             hoteldt.HandleHotelCustomerDataConfig( DBInfo,  listmodel);
        }
        public List<HotelPricelistModel> GetHotelPricelists(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetHotelPricelists(DBInfo, hotelInfoId, mpehotel);
        }

        public List<CustomerModel> GetReservationsNew(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId , DateTime? dt)
        {
            return hoteldt.GetReservationsNew(DBInfo, helper, hotelInfoId,dt);
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
            return hoteldt.GetReservationByConfirmCode(DBInfo, helper, hotelInfoId);
        }

        public  List<GroupModel> GetGroups(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetGroups(DBInfo, hotelInfoId, mpehotel);
        }
        public List<VipModel> GetVip(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetVip(DBInfo, hotelInfoId, mpehotel);
        }
        public List<TravelAgentModel> GetProtelTravelAgentList(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetProtelTravelAgentList(DBInfo, hotelInfoId, mpehotel);
        }
        public List<RoomModel> GetProtelRoomNo(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetProtelRoomNo(DBInfo, hotelInfoId, mpehotel);
        }
        public List<SourceModel> GetSource(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetSource(DBInfo, hotelInfoId, mpehotel);
        }
        public Dictionary<int, string> GetHotelInfo(DBInfoModel DBInfo)
        {
            return hoteldt.GetHotelInfo(DBInfo);
        }
        public List<ProtelHotelModel> GetHotels(DBInfoModel DBInfo, int hotelInfoId)
        {
            return hoteldt.GetHotels(DBInfo, hotelInfoId);
        }
        public List<RoomTypeModel> GetRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetRoomType(DBInfo, hotelInfoId, mpehotel);
        }
        public List<BookedRoomTypeModel> GetBookedRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetBookedRoomType(DBInfo, hotelInfoId, mpehotel);
        }
        public List<NationalityCodeModel>  GetNationalityCode(DBInfoModel DBInfo,int hotelInfoId,int mpehotel)
            {
            return hoteldt.GetNationalityCode(DBInfo, hotelInfoId, mpehotel);
        }
        public List<BoardModel> GetBoards(DBInfoModel DBInfo,int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetBoards(DBInfo, hotelInfoId, mpehotel);
        }

        public Dictionary<string, AllowanceModel> GetAllowance(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt)
        {
            return hoteldt.GetAllowance(DBInfo, helper, customer, dt);
        }

        public int GetGuestId(DBInfoModel DBInfo, CustomerModel customer)
        {
            int GuestId = hoteldt.GetGuestId(DBInfo, customer);

            if (GuestId > 0)
            {
                hoteldt.UpdateGuest(DBInfo, customer, GuestId);
            }
            else
            {
                GuestId = hoteldt.InsertGuest(DBInfo, customer);
            }

            return GuestId;
        }

        public List<string> GetCustomerCustomMessages(DBInfoModel DBInfo, CustomerModel customer)
        {
            List<CustomMessageModel> messages = GetCustomMessages(DBInfo);

            return hoteldt.GetCustomerCustomMessages(DBInfo, customer, messages);
        }

        public Dictionary<string, AllowanceModel> GetMacroOverride(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt)
        {
            return hoteldt.GetMacroOverride(DBInfo, helper, customer, dt);
        }

        public List<HotelAllowancesPerDay> GetHotelAllowances(DBInfoModel DBInfo, DateTime dateFrom, DateTime dateTo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetHotelAllowances(DBInfo, dateFrom, dateTo, hotelInfoId, mpehotel);
        }
        public List<TravelAgentModel> GetFilteredProtelTravelAgentList(DBInfoModel DBInfo, int mpehotel, string name)
        {
            return hoteldt.GetFilteredProtelTravelAgentList( DBInfo, mpehotel, name);
        }
        public List<GroupModel> GetFilteredProtelGroupList(DBInfoModel DBInfo, int mpehotel, string name)
        {
            return hoteldt.GetFilteredProtelGroupList(DBInfo, mpehotel, name);
        }

        public List<ProductLookupHelper> GetFilteredProduct(DBInfoModel DBInfo, string name)
        {
            return hoteldt.GetFilteredProduct(DBInfo, name);
        }
        public List<CustomerModel> GetReservationsTimePeriod(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId, DateTime dateFrom, DateTime dateTo)
        {
            List<CustomerModel> result = new List<CustomerModel>();

            result = hoteldt.GetReservationsTimePeriod(DBInfo, helper, hotelInfoId, dateFrom, dateTo);

            return result;
        }

        public List<MembershipModel> GetMemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetMemberships(DBInfo, hotelInfoId, mpehotel);
        }

        public List<SubmembershipModel> GetSubmemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            return hoteldt.GetSubmemberships(DBInfo, hotelInfoId, mpehotel);
        }

        #region customer data config

        public List<Hotel__CustomerDataConfigModel> GetCustomerDataConfig(DBInfoModel DBInfo)
        {
            return hoteldt.GetCustomerDataConfig(DBInfo);
        }

        public void InsertCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data)
        {
            hoteldt.InsertCustomerDataConfig(DBInfo, data);
        }

        public void UpdateCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data)
        {
            hoteldt.UpdateCustomerDataConfig(DBInfo, data);
        }

        public void DeleteCustomerDataConfigField(DBInfoModel DBInfo, long Id)
        {
            hoteldt.DeleteCustomerDataConfigField(DBInfo, Id);
        }

        #endregion customer data config

        public List<MacroModel> GetMacros(DBInfoModel DBInfo)
        {
            List<MacroModel> model = new List<MacroModel>();
            try
            {
             model= hotelMacrosDT.CashedDT.Select(DBInfo);
            }
            catch(Exception e)
            {
                logger.Error("GetMacros Error : " + e.ToString());
            }
            return model;
        }

        public MacroModel GetMacro(DBInfoModel DBInfo, Guid macroGuid)
        {
            MacroModel model = new MacroModel();
            try
            {
                model = hotelMacrosDT.CashedDT.Select(DBInfo).Where(r => r.Id == macroGuid).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.Error("GetMacros Error : " + e.ToString());
            }
            return model;
        }

        public Guid? InsertMacros(DBInfoModel DBInfo, MacroModel json)
        {
            json.MacroRules.Id= Guid.NewGuid(); 

            json.MacroResults.Id= Guid.NewGuid();

            Guid? resultID = null;

            try
            {
                resultID = hotelMacrosDT.CashedDT.Insert(DBInfo, json);
            }
            catch (Exception e)
            {
                logger.Error("InsertMacros Error : " + e.ToString());
            }

            return resultID;
        }
        public void DeleteMacros(DBInfoModel DBInfo, Guid Id)
        {
            try { 
            hotelMacrosDT.CashedDT.Delete(DBInfo, Id);
            }
            catch(Exception e)
            {
                logger.Error("DeleteMacros Error : " + e.ToString());
            }
        }

        public void UpdateMacros(DBInfoModel DBInfo, MacroModel model)
        {
            hotelMacrosDT.CashedDT.Update(DBInfo, model);
        }
        public List<MacroTimezoneModel> GetTimezones(DBInfoModel DBInfo)
        {
            List<MacroTimezoneModel> model = new List<MacroTimezoneModel>();
            try
            {
                model= timezoneDT.CashedDT.Select(DBInfo);
            }
            catch (Exception e)
            {
                logger.Error("GetTimezones Error : " + e.ToString());
            }
            return model;
            
        }

        public void DeleteObsoleteMacros(DBInfoModel DBInfo)
        {
            hoteldt.DeleteObsoleteMacros(DBInfo);
        }

        public bool InsertTimezones(DBInfoModel DBInfo, MacroTimezoneModel json)
        {
            bool flag = false;
            try
            {
                flag=OverlayChecks(DBInfo, json);

                if(flag==false)
                    timezoneDT.CashedDT.Insert(DBInfo, json);
                else
                    throw new BusinessException("Timezone Overlay", "failure");
            }
            catch (Exception e)
            {
                logger.Error("InsertTimezones Error : " + e.ToString());
            }

            return flag;
        }

        public bool  OverlayChecks(DBInfoModel DBInfo, MacroTimezoneModel json)
        {
            bool flag = false;
            List<MacroTimezoneModel> model = new List<MacroTimezoneModel>();
            try
            {
                model=timezoneDT.CashedDT.Select(DBInfo);

                TimeSpan myTimeFrom = json.TimeFrom.TimeOfDay;
                TimeSpan myTimeTo = json.TimeTo.TimeOfDay;

                TimeSpan minTime = myTimeFrom;
                TimeSpan maxTime = myTimeTo;

                foreach (MacroTimezoneModel row in model)
                {

                    TimeSpan rowTimeFrom = row.TimeFrom.TimeOfDay;
                    TimeSpan rowTimeTo = row.TimeTo.TimeOfDay;

                    if (rowTimeFrom < minTime)
                        minTime = rowTimeFrom;
                    if (rowTimeTo < minTime)
                        minTime = rowTimeTo;

                    if (rowTimeFrom > maxTime)
                        maxTime = rowTimeFrom;
                    if (rowTimeTo > maxTime)
                        maxTime = rowTimeTo;

                    if (rowTimeFrom <= myTimeTo && rowTimeTo >= myTimeTo)
                        flag = true;
                    else if (rowTimeFrom <= myTimeFrom && rowTimeTo >= myTimeTo)
                        flag = true;
                    else
                        flag = false;

                    if (flag == true)
                        break;
   
                }

                if (minTime==myTimeFrom && maxTime == myTimeTo )
                        flag = true;

                if (model.Count == 0)
                    flag = false;

            }
            catch (Exception e)
            {
                logger.Error("No Available TImezones Error : " + e.ToString());
            }

          

            return flag;

        }

        public bool UpdateTimezones(DBInfoModel DBInfo, MacroTimezoneModel json)
        {
            bool flag = false;
            try
            {
                flag = OverlayChecks(DBInfo, json);

                if (flag == false)
                    timezoneDT.CashedDT.Update(DBInfo, json);
                else
                    throw new BusinessException("Timezone Overlay", "failure");
            }
            catch(Exception e)
            {
                logger.Error("Update Timezones Error : " + e.ToString());
            }
            return flag;
        }
        public int DeleteTimezones(DBInfoModel DBInfo, Guid Id, string Code)
        {
            try
            {
                var validation = 0;
              validation =checkTimezoneCodeOnMacros(DBInfo, Id,Code);
                if(validation==-1)
                {
                    throw new BusinessException("Can not Delete a Timezone which is included in Macros");
                    
                }
                timezoneDT.CashedDT.Delete( DBInfo, Id);
            }
            catch (Exception e)
            {
                logger.Error("DeleteTimezones Error : " + e.ToString());
                return -1;
            }
            return 0;
        }

        public List<string> ValidateTimezoneExpression(DBInfoModel DBInfo, string timezoneExpression)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrWhiteSpace(timezoneExpression))
            {
                result.Add("Timezone expression cannot be empty");
                return result;
            }

            if (timezoneExpression.ToUpper() != timezoneExpression)
            {
                result.Add("Timezone expression can contain only Capital letters");
                return result;
            }

            timezoneExpression = timezoneExpression.Replace(" ", "");

            string[] timezoneExpressionChars = new string[timezoneExpression.Length];
            for(int i = 0;i< timezoneExpression.Length;i++)
            {
                timezoneExpressionChars[i] = timezoneExpression[i].ToString();
            }

            List<string> timezoneCodes = timezoneDT.CashedDT.Select(DBInfo).Select(t => t.Code).ToList();
            List<string> acceptableCharacters = new List<string>(timezoneCodes);
            acceptableCharacters.Add("(");
            acceptableCharacters.Add(")");
            acceptableCharacters.Add("+");
            acceptableCharacters.Add(".");

            if(timezoneExpressionChars.Except(acceptableCharacters).Any())
            {
                string err = "Timezone expression can contain only '(', ')', '+', '.'";
                foreach(string tzc in timezoneCodes)
                {
                    err += ", '" + tzc + "'";
                }
                result.Add(err);
                return result;
            }

            int lp = timezoneExpression.Count(x => x == '(');
            int rp = timezoneExpression.Count(x => x == ')');
            if(lp != rp)
                if (lp > rp)
                    result.Add("Timezone expression is missing ')'");
                else
                    result.Add("Timezone expression is missing '('");

            if (timezoneExpression.StartsWith(")") || timezoneExpression.StartsWith("+") || timezoneExpression.StartsWith("."))
                result.Add("Timezone expression cannot start with ')', '+', '.'");

            if (timezoneExpression.EndsWith("(") || timezoneExpression.EndsWith("+") || timezoneExpression.EndsWith("."))
                result.Add("Timezone expression cannot end with '(', '+', '.'");

            if (timezoneExpression.Contains("+."))
                result.Add("Timezone expression cannot contain the following sequence: '+.'");

            if (timezoneExpression.Contains("++"))
                result.Add("Timezone expression cannot contain the following sequence: '++'");

            if (timezoneExpression.Contains(".+"))
                result.Add("Timezone expression cannot contain the following sequence: '.+'");

            if (timezoneExpression.Contains(".."))
                result.Add("Timezone expression cannot contain the following sequence: '..'");

            if (timezoneExpression.Contains("(."))
                result.Add("Timezone expression cannot contain the following sequence: '(.'");

            if (timezoneExpression.Contains("(+"))
                result.Add("Timezone expression cannot contain the following sequence: '(+'");

            if (timezoneExpression.Contains(".)"))
                result.Add("Timezone expression cannot contain the following sequence: '.)'");

            if (timezoneExpression.Contains("+)"))
                result.Add("Timezone expression cannot contain the following sequence: '+)'");
            
            for(int i = 0; i < timezoneCodes.Count; i++)
            {
                for(int y = 0; y < timezoneCodes.Count; y++)
                {
                    string pair = timezoneCodes[i] + timezoneCodes[y];
                    if (timezoneExpression.Contains(pair))
                    {
                        result.Add("Timezone expression cannot contain the following sequence: '" + pair + "'");
                    }                    
                }
            }

            TreeManager treeCreator = new TreeManager();
            try
            {
                treeCreator.Construct(timezoneExpression);
            }
            catch(Exception ex)
            {
                result.Add("Timezone expression cannot contain both '+' and '.' outside of parenthesis");
            }

            return result;
        }

        public int checkTimezoneCodeOnMacros(DBInfoModel DBInfo, Guid Id, string Code)
        {

            List<MacroModel> model = new List<MacroModel>();

            foreach(MacroModel row in model)
            {
                row.MacroResults = new MacroResultModel();
                row.MacroRules = new MacroRuleModel();
            }
           model =hotelMacrosDT.CashedDT.Select(DBInfo);

            foreach (MacroModel row in model)
            {
               if(row.MacroRules.TimeZones.Contains(Code))
                {   
                    return -1;
                }
            }
            return 0;
        }

        public List<CustomMessageModel> GetCustomMessages(DBInfoModel DBInfo)
        {
            List<CustomMessageModel> result = new List<CustomMessageModel>();

            try
            {
                result = customMessagesDT.CashedDT.Select(DBInfo);
            }
            catch (Exception e)
            {
                logger.Error("Get custom message error : " + e.ToString());
            }

            return result;
        }

        public CustomMessageModel GetCustomMessage(DBInfoModel DBInfo, Guid Id)
        {
            CustomMessageModel result = new CustomMessageModel();

            try
            {
                result = customMessagesDT.CashedDT.Select(DBInfo).Where(c => c.Id == Id).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.Error("Get custom message error : " + e.ToString());
            }

            return result;
        }
        public List<ParamModel> GetParams(DBInfoModel DBInfo)
        {
            return hoteldt.GetParams(DBInfo);
        }
        public void InsertCustomMessage(DBInfoModel DBInfo, CustomMessageModel data)
        {
            try
            {
                customMessagesDT.CashedDT.Insert(DBInfo, data);
            }
            catch (Exception e)
            {
                logger.Error("Insert custom message error : " + e.ToString());
            }
        }

        public void UpdateCustomMessage(DBInfoModel DBInfo, CustomMessageModel data)
        {
            try
            {
                customMessagesDT.CashedDT.Update(DBInfo, data);
            }
            catch (Exception e)
            {
                logger.Error("Update custom message error : " + e.ToString());
            }
        }

        public void DeleteCustomMessage(DBInfoModel DBInfo, Guid Id)
        {
            try
            {
                customMessagesDT.CashedDT.Delete(DBInfo, Id);
            }
            catch (Exception e)
            {
                logger.Error("Delete custom message error : " + e.ToString());
            }
        }

        public bool UpdateMacroRemainingConsumption(DBInfoModel DBInfo, UpdateConsumptionModel data)
        {
            MacroModel macro = GetMacro(DBInfo, data.macroGuid);

            if (macro != null && macro.MacroRules.TotalConsumption > 0)
            {
                int newRemainingConsumption = macro.MacroRules.RemainingConsumption - data.couver;

                if (newRemainingConsumption < 0)
                {
                    return false;
                }

                macro.MacroRules.RemainingConsumption = newRemainingConsumption;

                UpdateMacros(DBInfo, macro);

                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<int, int> GetGuestIds(DBInfoModel DBInfo, GuestIdsList guestIdsList)
        {
            return hoteldt.GetGuestIds(DBInfo, guestIdsList);
        }

        public int GetGuestIdFromProfileAndRoom(DBInfoModel DBInfo, GuestGetIdModel guestGetIdModel)
        {
            return hoteldt.GetGuestIdFromProfileAndRoom(DBInfo, guestGetIdModel);
        }

        public PosReservationHelper CreateReservationHelperModel(PMSCustomerModel customer, HotelsInfoModel hotelInfo)
        {
            PosReservationHelper reservationHelperModel = new PosReservationHelper();
            reservationHelperModel.HotelId = hotelInfo.MPEHotel != null ? (int)hotelInfo.MPEHotel : 0;
            //reservationHelperModel.Name = customer.FirstName + " " + customer.LastName;
            reservationHelperModel.Room = customer.Room;
            reservationHelperModel.Page = 0;
            reservationHelperModel.Pagesize = 100;
            return reservationHelperModel;
        }

        public bool IsCustomerCheckOut(HotelsInfoModel hotelInfo, PMSCustomerModel customer, List<CustomerModel> currentCustomers)
        {
            bool isCheckOut = true;
            if (hotelInfo.Type == (byte)HotelInfoTypeEnum.PmsHotel || hotelInfo.Type == (byte)HotelInfoTypeEnum.PmsInterface)
            {
                foreach (CustomerModel currentCustomer in currentCustomers)
                {
                    if (currentCustomer.ReservationId == customer.ReservationId && currentCustomer.ReservStatus == ReservStatusEnum.CheckedIn)
                    {
                        isCheckOut = false;
                        break;
                    }
                }
            }
            return isCheckOut;
        }

    }
}
