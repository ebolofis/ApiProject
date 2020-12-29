using Symposium.Models.Models;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IOrderDAO
    {
        /// <summary>
        /// Function uses OrderFilterModel to create a gen query sql string and an object 
        /// in order to collect info with specific filter results 
        /// returns true if at least one obj found with specific query results 
        /// else it returns false
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filter"></param>
        /// <param name="filterType">GeneralENUM.FilterTypeEnum flag </param>
        /// <returns> true if gen res > 0 </returns>
        bool OrderExists(IDbConnection db, OrderFilterModel filter, int filterType = 0);

        /// <summary>
        /// Function uses OrderFilterModel to create a gen query sql string and an object 
        /// in order to collect list of orders with specific filter results 
        /// returns list of orders that match filter on filter type  FilterTypeEnum.AND , OR
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filter"></param>
        /// <param name="filterType">GeneralENUM.FilterTypeEnum flag </param>
        /// <returns> list orders that match filter </returns>
        List<OrderDTO> FilteredOrders(IDbConnection db, OrderFilterModel filter, int filterType = 0);
    }
}
