using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IDeliveryOrderTasks
    {
        List<OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel Store, long OrderId);
        List<StatusCounts> StateCountsTask(DBInfoModel Store, DeliveryFilters filter);
        PaginationModel<DeliveryStatusOrders> PagedOrdersByStateTask(DBInfoModel Store, int status, int pageNumber, int pageLength, DeliveryFilters filter);
        DeliveryCustomersShippingAddressModel UpdateShippingCoordinatesTask(DBInfoModel Store, long InvoiceId, DeliveryCustomersShippingAddressModel model);
        int UpdateOrderStatusForReturned(DBInfoModel Store, long OrderId, int status);

        long UpdateStaffStatus(DBInfoModel Store, long StaffId, bool IsOnRoad);

        List<StaffDeliveryModel> GetDeliveryStaffsOnly(DBInfoModel Store);

        List<StaffDeliveryModel> GetDeliveryStaffsSignInOnly(DBInfoModel Store, long PosInfoId);
    }
}
