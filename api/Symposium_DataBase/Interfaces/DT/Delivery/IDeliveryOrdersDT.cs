using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IDeliveryOrdersDT
    {
        List<OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel store, long OrderId);
        List<StatusCounts> StatesCounts(DBInfoModel store, DeliveryFilters filter);
        PaginationModel<DeliveryStatusOrders> PagedOrdersByState(DBInfoModel store, int status, int pageNumber, int pageLength, DeliveryFilters flts);

        DeliveryCustomersShippingAddressModel UpdateShippingCoordinates(DBInfoModel store, long InvoiceId, DeliveryCustomersShippingAddressModel model);
        int UpdateOrderStatusForReturned(DBInfoModel Store, long OrderId, int status);

        long UpdateStaffStatus(DBInfoModel Store, long StaffId, bool IsOnRoad, IDbConnection db = null, IDbTransaction transact = null);

        List<StaffDeliveryModel> GetDeliveryStaffsOnly(DBInfoModel Store);

        bool GetDeliveryStaffsSignInOnly(DBInfoModel Store, long PosInfoId, long StaffId);

        /// <summary>
        /// get staff that is not deletes, has clocked in and not clocked out and is not assigned delivery route
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <returns>List<StaffDeliveryModel></returns>
        List<StaffDeliveryModel> GetAvailableDeliveryStaffs(DBInfoModel Store);

        /// <summary>
        /// Update staff IsOnRoad
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="StaffId">long</param>
        /// <param name="IsOnRoad">bool</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>long</returns>
        long UpdateStaffStatusTran(DBInfoModel Store, long StaffId, bool IsOnRoad, IDbConnection db = null, IDbTransaction transact = null);
    }
}
