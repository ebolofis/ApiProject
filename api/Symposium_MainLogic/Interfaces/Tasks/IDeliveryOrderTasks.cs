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
        List<OrderStatusModel> GetOrderStatusTimeChanges(Store Store, long OrderId);
        List<StatusCounts> StateCountsTask(Store Store, DeliveryFilters filter);
        PaginationModel<DeliveryStatusOrders> PagedOrdersByStateTask(Store Store, int status, int pageNumber, int pageLength, DeliveryFilters filter);
        DeliveryCustomersShippingAddressModel UpdateShippingCoordinatesTask(Store Store, long InvoiceId, DeliveryCustomersShippingAddressModel model);
        int UpdateOrderStatusForReturned(Store Store, long OrderId, int status);

        long UpdateStaffStatus(Store Store, long StaffId, bool IsOnRoad);

        List<StaffDeliveryModel> GetDeliveryStaffsOnly(Store Store);
    }
}
