using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ComboDetailModel
    {
        public long Id { get; set; }
        public long ComboId { get; set; }
        public long ProductComboItemId { get; set; }
    }
}
