using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ForexServiceModel
    {
        public long Id { get; set; }
        public string CurFrom { get; set; }
        public string CurTo { get; set; }
        public decimal Rate { get; set; }
        public System.DateTime Date { get; set; }
    }
}
