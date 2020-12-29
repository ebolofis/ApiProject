using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class EndOfDayVoidsAnalysisModel
    {
        public long Id { get; set; }
        public long EndOfdayId { get; set; }
        public Nullable<long> AccountId { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; }
        public Nullable<int> AccountType { get; set; }
    }
}
