using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Models;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.Infrastructure;
using Symposium.Models.Models.MealBoards;

namespace Symposium.Helpers.Classes
{
    public class HotelAllowanceMacroHelper
    {
        /// <summary>
        /// return top priority macro result with matching conditions if exists
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="helper">PosReservationHelper</param>
        /// <param name="customer">CustomerModel</param>
        /// <param name="timezone">MacroTimezoneModel</param>
        /// <param name="macros">List<MacroModel></param>
        /// <returns>MacroResultModel</returns>
        public AllowanceModel GetMacroResults(
            DBInfoModel DBInfo, 
            PosReservationHelper helper, 
            CustomerModel customer,
            MacroTimezoneModel timezone, 
            List<MacroTimezoneModel> timezones,
            List<MacroModel> macros,
            List<Models.Models.Hotel.MealConsumptionModel> consumption, 
            List<MetadataModel> metadata = null
            )
        {
            AllowanceModel result = new AllowanceModel(customer.Adults, customer.Children);
            
            result.TimezoneCode = timezone.Code;
            result.adultConsumption = consumption.Sum(c => c.adultConsumption);
            result.childConsumption = consumption.Sum(c => c.childConsumption);
            result.adultConsumptionForTimezone = consumption.Where(c => c.timezoneCode == timezone.Code).Sum(c => c.adultConsumption);
            result.childConsumptionForTimezone = consumption.Where(c => c.timezoneCode == timezone.Code).Sum(c => c.childConsumption);

            List<MacroResultModel> selectedMacroResults = new List<MacroResultModel>();

            MacroModel validNotUseAllowanceResult = FilterValidMacros(ref result, customer, helper, timezone, timezones, macros, metadata, consumption, false);
            result.notUseAllowanceMacro = validNotUseAllowanceResult == null ? null : validNotUseAllowanceResult;

            MacroModel validUseAllowanceResult = FilterValidMacros(ref result, customer, helper, timezone, timezones, macros, metadata, consumption, true);
            result.useAllowanceMacro = validUseAllowanceResult == null ? null : validUseAllowanceResult;

            return result;
        }

        public Dictionary<string, AllowanceModel> GetMacroOverrideResult(
            DBInfoModel DBInfo, 
            PosReservationHelper helper, 
            CustomerModel customer,
            List<MacroTimezoneModel> timezones,
            List<MacroModel> macros,
            DateTime? dt, 
            List<MetadataModel> metadata = null
            )
        {
            Dictionary<string, AllowanceModel> result = new Dictionary<string, AllowanceModel>();

            List<Models.Models.Hotel.MealConsumptionModel> consumption = GetEmptyConsumption(timezones);

            foreach ( MacroTimezoneModel timezone in timezones)
            {
                AllowanceModel timezoneMacros = new AllowanceModel(customer.Adults, customer.Children);

                timezoneMacros.TimezoneCode = timezone.Code;

                timezoneMacros.useAllowanceMacro = FilterValidMacros(ref timezoneMacros, customer, helper, timezone, timezones, macros, metadata, consumption, true, dt, true);

                timezoneMacros.notUseAllowanceMacro = FilterValidMacros(ref timezoneMacros, customer, helper, timezone, timezones, macros, metadata, null, false, dt, true);

                result.Add(timezone.Code, timezoneMacros);
            }

            return result;
        }

        public List<Models.Models.Hotel.MealConsumptionModel> GetEmptyConsumption(List<MacroTimezoneModel> timezones)
        {
            List<Models.Models.Hotel.MealConsumptionModel> consumption = new List<Models.Models.Hotel.MealConsumptionModel>();

            foreach (MacroTimezoneModel timezone in timezones)
            {
                Models.Models.Hotel.MealConsumptionModel timezoneConsumption = new Models.Models.Hotel.MealConsumptionModel();

                timezoneConsumption.adultConsumption = 0;

                timezoneConsumption.childConsumption = 0;

                timezoneConsumption.timezoneCode = timezone.Code;

                consumption.Add(timezoneConsumption);
            }

            return consumption;
        }

        /// <summary>
        /// return macros matching conditions
        /// </summary>
        /// <param name="customer">CustomerModel</param>
        /// <param name="helper">PosReservationHelper</param>
        /// <param name="currentTimezone">MacroTimezoneModel</param>
        /// <param name="macros">List<MacroModel></param>
        /// <returns>List<MacroModel></returns>
        public MacroModel FilterValidMacros(
            ref AllowanceModel allowanceResult,
            CustomerModel customer,
            PosReservationHelper helper, 
            MacroTimezoneModel currentTimezone,
            List<MacroTimezoneModel> timezones,
            List<MacroModel> macros, 
            List<MetadataModel> metadata,
            List<Models.Models.Hotel.MealConsumptionModel> consumption,
            bool useAllowance,
            DateTime? dt = null,
            bool ovride = false
            )
        {
            foreach (MacroModel macro in macros)
            {
                if (macro.MacroRules.UseAllowance != useAllowance) continue;
                if (!macro.MacroRules.ActiveDays[(int)(DateTime.Now.DayOfWeek + 6) % 7]) continue;
                if (macro.MacroRules.TotalConsumption > 0 && macro.MacroRules.RemainingConsumption <= 0) continue;
                if (!ValidateRuleTimezone(ref allowanceResult, macro, currentTimezone, timezones, consumption, dt, ovride || !useAllowance)) continue;
                if (macro.MacroRules.ReservationId != null && macro.MacroRules.ReservationId != customer.ReservationId) continue;
                if (macro.MacroRules.HotelId != null && macro.MacroRules.HotelId.Count > 0 && !macro.MacroRules.HotelId.Where(h => h == helper.HotelId).Any()) continue;
                if (macro.MacroRules.PosDepartment != null && macro.MacroRules.PosDepartment.Count > 0 && !macro.MacroRules.PosDepartment.Where(h => h == helper.PosDepartmentId).Any()) continue;
                if (macro.MacroRules.TravelAgent != null && macro.MacroRules.TravelAgent.kdnr != customer.TravelAgentId) continue;
                if (macro.MacroRules.Group != null && macro.MacroRules.Group.kdnr != customer.GroupId) continue;
                string[] boardCodes = customer.BoardCode != null ? customer.BoardCode.Split(',') : null;
                if (macro.MacroRules.Board != null && boardCodes != null && !macro.MacroRules.Board.Where(h => boardCodes.Where(c => c == h).Any()).Any()) continue;
                if (macro.MacroRules.RoomNo != null && macro.MacroRules.RoomNo != customer.Room) continue;
                if (macro.MacroRules.RoomType != null && macro.MacroRules.RoomType.Count > 0 && !macro.MacroRules.RoomType.Where(h => h == customer.RoomTypeId).Any()) continue;
                if (macro.MacroRules.BookedRoomType != null && macro.MacroRules.BookedRoomType.Count > 0 && !macro.MacroRules.BookedRoomType.Where(h => h == customer.BookedRoomTypeId).Any()) continue;
                if (macro.MacroRules.NationalityCode != null && macro.MacroRules.NationalityCode != customer.NationalityId.ToString()) continue;
                if (macro.MacroRules.VIP != null && macro.MacroRules.VIP != customer.VIP) continue;
                if (macro.MacroRules.ReservationSource != null && Convert.ToInt32(macro.MacroRules.ReservationSource) != customer.SourceId) continue;

                if (macro.MacroRules.ReservationUserDefinedField != null && !metadata.Where(m => m.data == macro.MacroRules.ReservationUserDefinedField).Any()) continue;
                if (macro.MacroRules.ProfileUserDefinedField != null && !metadata.Where(m => m.data == macro.MacroRules.ProfileUserDefinedField).Any()) continue;

                if (!macro.MacroRules.HotelPriceList.Equals(null) && macro.MacroRules.HotelPriceList > 0 && customer.RateCodeId > 0 && macro.MacroRules.HotelPriceList != customer.RateCodeId) continue;
                if (!macro.MacroRules.Membership.Equals(null) && macro.MacroRules.Membership > 0 && customer.LoyaltyProgramId > 0 && macro.MacroRules.Membership != customer.LoyaltyProgramId) continue;
                if (!macro.MacroRules.SubMembership.Equals(null) && macro.MacroRules.SubMembership > 0 && customer.LoyaltyLevelId > 0 && macro.MacroRules.SubMembership != customer.LoyaltyLevelId) continue;

                return macro;
            }
            return null;
        }

        //public static List<AllowancesModel> CalculateAllowances(DBInfoModel DBInfo, CustomerModel customer, List<MacroTimezoneModel> timezones, List<MacroModel> macros, DateTime date)
        //{
        //    List<AllowancesModel> result = new List<AllowancesModel>();



        //    return result;
        //}

        public bool ValidateRuleTimezone(
            ref AllowanceModel allowanceResult, 
            MacroModel macro, 
            MacroTimezoneModel currentTimezone, 
            List<MacroTimezoneModel> timezones, 
            List<Models.Models.Hotel.MealConsumptionModel> consumption,
            DateTime? dt,
            bool ovride)
        {
            bool result = false;

            if (dt != null)
            {
                if ((macro.ActiveTo != null && macro.ActiveTo < dt) || (macro.ActiveFrom != null && macro.ActiveFrom > dt) || macro.MacroRules.TimeZones == null || currentTimezone.Code == null)
                {
                    return result;
                }
            }
            else
            {
                if ((macro.ActiveTo != null && macro.ActiveTo < DateTime.Now) || (macro.ActiveFrom != null && macro.ActiveFrom > DateTime.Now) || macro.MacroRules.TimeZones == null || currentTimezone.Code == null)
                {
                    return result;
                }
            }

            Dictionary<string, bool> values = new Dictionary<string, bool>();

            BinaryCalculatorHelperModel helper = new BinaryCalculatorHelperModel();

            helper.timezoneCode = currentTimezone.Code;

            helper.allowanceAdult = allowanceResult.adultAllowance;

            helper.allowanceChild = allowanceResult.childAllowance;

            foreach(MacroTimezoneModel timezone in timezones)
            {
                BinaryCalculatorConsumptionHelperModel timezoneConsumption = new BinaryCalculatorConsumptionHelperModel();

                timezoneConsumption.timezoneCode = timezone.Code;

                timezoneConsumption.consumptionAdult = consumption == null ? 0 : consumption.Where(c => c.timezoneCode == timezone.Code).Sum(c => c.adultConsumption);

                timezoneConsumption.consumptionChild = consumption == null ? 0 : consumption.Where(c => c.timezoneCode == timezone.Code).Sum(c => c.childConsumption);

                helper.consumptions.Add(timezoneConsumption);

                values.Add(timezone.Code, helper.timezoneCode == timezone.Code ? true : false);
            }

            try
            {
                TreeManager treeCreator = new TreeManager();

                BinaryCalculator bc1 = new BinaryCalculator(helper, ovride);

                treeCreator.Construct(macro.MacroRules.TimeZones);

                result = bc1.Execute(values, treeCreator.Tree);
            }
            catch (Exception ex)
            {
                return result;
            }

            if (result)
            {
                allowanceResult.adultRemainingAllowanceForTimezone = helper.remainingAllowanceAdult;

                allowanceResult.childRemainingAllowanceForTimezone = helper.remainingAllowanceChild;

                return true;
            }

            return false;
        }
    }
}
