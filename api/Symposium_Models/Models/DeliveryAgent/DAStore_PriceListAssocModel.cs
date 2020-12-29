using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DAStore_PriceListAssocModel
    {
        public long Id { get; set; }

        /// <summary>
        /// Pricelist.Id
        /// </summary>
        public long PriceListId { get; set; }

        /// <summary>
        /// Περιγραφή Τιμοκαταλόγου
        /// </summary>
        public string PriceListDescription { get; set; }

        /// <summary>
        /// DA_Store.Id
        /// </summary>
        public long DAStoreId { get; set; }

        /// <summary>
        /// Ονομασία καταστήματος
        /// </summary>
        public string StoreTitle { get; set; }

        /// <summary>
        /// for Delivery or for TakeOut 
        /// </summary>
        public DAPriceListTypes PriceListType { get; set; }
    }
}
