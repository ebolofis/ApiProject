using Symposium.Models.Enums.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Promos
{
    public class PromotionsPricelistModel
    {
        public long Id { get; set; }

        public long PricelistId { get; set; }

        public string PricelistDescr { get; set; }

    }
}
