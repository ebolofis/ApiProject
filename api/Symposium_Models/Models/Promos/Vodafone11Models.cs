using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Promos
{
    /// <summary>
    /// Class that represents a Vodafone 1+1 promo
    /// </summary>
  public  class Vodafone11Model
    {

        public long Id { get; set; }

        /// <summary>
        /// promo description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// from number of items...
        /// </summary>
        public int FromItems { get; set; }

        /// <summary>
        /// ...remove these items.
        /// </summary>
        public int RemoveItems { get; set; }

        /// <summary>
        /// list of Product Categories the promo applied (the order is critical)
        /// </summary>
        public List<Vodafone11ProdCategoriesModel> ProductCategories { get; set; } = new List<Vodafone11ProdCategoriesModel>();
    }

    public class Vodafone11ProdCategoriesModel
    {
        public long Id { get; set; }
        public long HeaderId { get; set; }
        public long ProdCategoryId { get; set; }
        public long Position { get; set; }
        public string ProductDescr { get; set; }
    }
}
