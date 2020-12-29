using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ComboModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long ProductComboId { get; set; }
        public List<ComboDetailModel> ComboDetails { get; set; }
        public int ComboDetailsLength { get; set; }
    }
}
