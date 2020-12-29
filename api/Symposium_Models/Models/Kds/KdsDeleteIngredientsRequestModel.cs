﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Kds
{
    public class KdsDeleteIngredientsRequestModel
    {
        public string Storeid { get; set; }
        public List<long> KdsIds { get; set; }
        public List<long> SaleTypesIds { get; set; }
        public long IngredientId { get; set; }
    }
}
