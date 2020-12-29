using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_ShortagesExtModel
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public string ProductDescr { get; set; }

        public string ProductCode { get; set; }

        public long StoreId { get; set; }

        public string StoreDescr { get; set; }

        /// <summary>
        /// 0: προσωρινή, 1: μόνημη
        /// </summary>
        public DAShortageEnum ShortType { get; set; }
    }

   
    public class DA_ShortageProdsModel
    {
        
        public long Id { get; set; }

 
        [Required]
        public long ProductId { get; set; }


        [Required]
        public long StoreId { get; set; }

        /// <summary>
        /// 0: προσωρινή, 1: μόνημη
        /// </summary>
        [Required]
        public DAShortageEnum ShortType { get; set; }
    }

    public class ProductsComboList
    {
        public long Id { get; set; }

        public string Descr { get; set; }

        public string Code { get; set; }
    }
}
