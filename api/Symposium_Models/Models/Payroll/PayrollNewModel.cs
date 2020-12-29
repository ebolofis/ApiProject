using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Payroll
{
    public class PayrollNewModel
    {
        public long Id { get; set; }
        public string Identification { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public long PosInfoId { get; set; }
        public long StaffId { get; set; }
        public string ShopId { get; set; }
        public string StaffDesc { get; set; }
        public string PosInfoDesc { get; set; }
        public string TotalHours { get; set; }
        public string StaffPositionIds { get; set; }
        public bool FromPos { get; set; }
        public long TotalMinutes { get; set; }
        public Nullable<Int16> IsSend { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

    }
}
