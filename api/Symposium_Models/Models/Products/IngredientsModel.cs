using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class IngredientsModel
    {
        public Nullable<long> Id { get; set; }

        public string Description { get; set; }

        public string ExtendedDescription { get; set; }

        public string SalesDescription { get; set; }

        public Nullable<double> Qty { get; set; }

        public Nullable<long> ItemId { get; set; }

        public Nullable<long> UnitId { get; set; }

        public string Code { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public string Background { get; set; }

        public string Color { get; set; }

        public Nullable<long> IngredientCategoryId { get; set; }

        public Nullable<long> DAId { get; set; }

        public Nullable<bool> DisplayOnKds { get; set; }

    }
}
