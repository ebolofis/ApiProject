using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class OrderDetailIngredientsKDSModel
    {
        /// <summary>
        /// Table Id auto incrment
        /// </summary>
       public long Id { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Order Detail Id
        /// </summary>
        public long OrderDetailId { get; set; }

        /// <summary>
        /// Product Id
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Igredients Id
        /// </summary>
        public long IgredientsId { get; set; }

        /// <summary>
        /// Igredients Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Totla qty per Orderdetail,product and Igredients
        /// </summary>
        public float Qty { get; set; }

        /// <summary>
        /// SalesType Id
        /// </summary>
        public long SalesTypeId { get; set; }

        /// <summary>
        /// KDS Id
        /// </summary>
        public long KDSId { get; set; }
    }
}
