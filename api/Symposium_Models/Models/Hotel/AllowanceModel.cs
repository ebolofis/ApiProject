using Symposium.Models.Models.MealBoards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotel
{
    public class AllowanceModel
    {
        public MacroModel useAllowanceMacro { get; set; }
        public MacroModel notUseAllowanceMacro { get; set; }
        public int adultAllowance { get; set; }
        public int adultAllowanceForTimezone { get; set; }
        public int adultConsumption { get; set; }
        public int adultConsumptionForTimezone { get; set; }
        public int adultRemainingAllowanceForTimezone { get; set; }
        public int childAllowance { get; set; }
        public int childAllowanceForTimezone { get; set; }
        public int childConsumption { get; set; }
        public int childConsumptionForTimezone { get; set; }
        public int childRemainingAllowanceForTimezone { get; set; }
        public string TimezoneCode { get; set; }
        public int GuestId { get; set; }
        public List<string> messages { get; set; }

        public AllowanceModel(int adultsAllowance = 0, int childrenAllowance = 0)
        {
            adultAllowance = adultsAllowance;
            childAllowance = childrenAllowance;
            useAllowanceMacro = new MacroModel();
            notUseAllowanceMacro = new MacroModel();
            adultAllowanceForTimezone = 0;
            adultRemainingAllowanceForTimezone = 0;
            childAllowanceForTimezone = 0;
            childRemainingAllowanceForTimezone = 0;
        }
    }

    public class AllowancesModel
    {
        public int allowanceAdult { get; set; }
        public int allowanceChild { get; set; }
        public string allowanceTimezone { get; set; }
    }

    public class CustomerAllowanceModel
    {
        public CustomerModel customer { get; set; }
        public List<MealConsumptionDetailModel> consumption { get; set; }
        public List<AllowancesModel> allowance { get; set; }

        public CustomerAllowanceModel()
        {
            consumption = new List<MealConsumptionDetailModel>();
            allowance = new List<AllowancesModel>();
        }
    }

    public class HotelAllowancesPerRoom
    {
        public string room { get; set; }
        public bool hasOverride { get; set; }
        public List<CustomerAllowanceModel> customers { get; set; }

        public HotelAllowancesPerRoom()
        {
            customers = new List<CustomerAllowanceModel>();
        }
    }

    public class HotelAllowancesPerDay
    {
        public DateTime date { get; set; }
        public List<HotelAllowancesPerRoom> rooms { get; set; }
        public HotelAllowancesPerDay()
        {
            rooms = new List<HotelAllowancesPerRoom>();
        }
    }

    public class BinaryCalculatorHelperModel
    {
        public string timezoneCode { get; set; }
        public int allowanceAdult { get; set; }
        public int allowanceChild { get; set; }
        public int remainingAllowanceAdult { get; set; }
        public int remainingAllowanceChild { get; set; }
        public List<BinaryCalculatorConsumptionHelperModel> consumptions { get; set; }

        public BinaryCalculatorHelperModel()
        {
            consumptions = new List<BinaryCalculatorConsumptionHelperModel>();
        }
    }

    public class BinaryCalculatorConsumptionHelperModel
    {
        public int consumptionAdult { get; set; }
        public int consumptionChild { get; set; }
        public string timezoneCode { get; set; }
    }
}
