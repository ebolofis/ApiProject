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
    public interface IOrderStatusDAO
    {
        List<OrderStatusDTO> FilteredOrderStatus(IDbConnection db, OrderStatusFilter filter, int filterType = 0);
    }
}
