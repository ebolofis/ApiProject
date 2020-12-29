using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ProductCategoriesModel
    {
        [Required]
        [Range(0, long.MaxValue)]
        public Nullable<long> Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(150)]
        public string Description { get; set; }

        /// <summary>
        /// not used
        /// </summary>
        public Nullable<byte> Type { get; set; }

        /// <summary>
        /// 1 = ενεργό / 0 = ανενεργό
        /// </summary>
        [Required]
        public Nullable<byte> Status { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        public Nullable<long> CategoryId { get; set; }

        public Nullable<long> DAId { get; set; }
    }

    public class ProductsCategoriesComboList
    {
        public long Id { get; set; }

        public string Descr { get; set; }
    }

    public class ProductsCategoriesIdList
    { 
        public List<long> productCategoryIdList { get; set; }
    }

    public class VatComboList
    {
        public long Id { get; set; }

        public string Descr { get; set; }
    }
}
