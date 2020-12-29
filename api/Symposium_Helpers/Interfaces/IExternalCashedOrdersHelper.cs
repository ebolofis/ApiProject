using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    /// <summary>
    ///  Class that cashes the list of inserted to DB orders from External Delivery Systems (like e-food)
    /// </summary>
    public interface IExternalCashedOrdersHelper
    {
        /// <summary>
        /// the list of inserted to DB orders from External Delivery Systems (like e-food)
        /// </summary>
        List<DA_OrderIdsModel> Orders { get; set; }

        /// <summary>
        /// Add an order to list
        /// </summary>
        /// <param name="Order"></param>
        void AddOrder(DA_OrderIdsModel Order);

        /// <summary>
        /// return true if an order exists to the cashed order list
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        bool OrderExists(DA_OrderIdsModel Order);
    }
}
