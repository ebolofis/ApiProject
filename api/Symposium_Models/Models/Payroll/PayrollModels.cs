using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Payroll
{
    public class PayrollModels
    {
        public long Id { get; set; }

        public string Identification { get; set; }

        public DateTime ActionDate { get; set; }
        public int Type { get; set; }

        public long PosInfoId { get; set; }

        public long StaffId { get; set; }

        public long ShopId { get; set; }

        public string StaffDesc { get; set; }
        public string PosInfoDesc { get; set; }
    }
}
