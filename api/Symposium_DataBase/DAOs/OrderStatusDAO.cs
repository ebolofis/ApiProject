using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class OrderStatusDAO : IOrderStatusDAO
    {
        IGenericDAO<OrderStatusDTO> genordst;
        public OrderStatusDAO(IGenericDAO<OrderStatusDTO> _genordst)
        {
            genordst = _genordst;
        }

        public List<OrderStatusDTO> FilteredOrderStatus(IDbConnection db, OrderStatusFilter filter, int filterType = 0)
        {

            string q = "select * from OrderStatus "; string wq = "";
            string flg = " AND ";
            switch (filterType)
            {
                case (int)FilterTypeEnum.AND: flg = " AND "; break;
                case (int)FilterTypeEnum.OR: flg = " OR "; break;
                default: flg = " AND "; break;
            }

            if (filter.Id != null) { wq += " Id = @Id "; }
            if (filter.Status != null) { if (wq.Length == 0) { wq += " Status = @Status "; } else { wq += flg + " Status = @Status "; } }
            if (filter.OrderId != null) { if (wq.Length == 0) { wq += " OrderId = @OrderId "; } else { wq += flg + " OrderId = @OrderId "; } }
            if (filter.ExtState != null) { if (wq.Length == 0) { wq += " ExtState = @ExtState "; } else { wq += flg + " ExtState = @ExtState "; } }
            if (filter.StaffId != null) { if (wq.Length == 0) { wq += " StaffId = @StaffId "; } else { wq += flg + " StaffId = @StaffId "; } }
            if (filter.TimeChanged != null) { if (wq.Length == 0) { wq += " TimeChanged = @TimeChanged "; } else { wq += flg + " TimeChanged = @TimeChanged "; } }

            object whereobj = new
            {
                Id = filter.Id,
                Status = filter.Status,
                OrderId = filter.OrderId,
                ExtState = filter.ExtState,
                StaffId = filter.StaffId,
                TimeChanged = filter.TimeChanged,
            };
            if (wq.Length != 0)
            {
                q = q + " where " + wq;
            }
            return genordst.Select(q, whereobj, db);
        }
    }
}
