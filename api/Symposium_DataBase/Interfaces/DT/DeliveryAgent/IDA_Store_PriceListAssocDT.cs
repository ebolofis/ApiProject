using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_Store_PriceListAssocDT
    {
        /// <summary>
        /// Επιστρέφει όλες τις  pricelist ανα κατάστημα
        /// </summary>
        /// <returns>DAStore_PriceListAssocModel</returns>
        List<DAStore_PriceListAssocModel> GetDAStore_PriceListAssoc(DBInfoModel dbInfo);
    }
}
