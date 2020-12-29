using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IDeliveryOrdersDT
    {
        List<OrderStatusModel> GetOrderStatusTimeChanges(Store store, long OrderId);
        List<StatusCounts> StatesCounts(Store store, DeliveryFilters filter);
        PaginationModel<DeliveryStatusOrders> PagedOrdersByState(Store store, int status, int pageNumber, int pageLength, DeliveryFilters flts);

        DeliveryCustomersShippingAddressModel UpdateShippingCoordinates(Store store, long InvoiceId, DeliveryCustomersShippingAddressModel model);
        int UpdateOrderStatusForReturned(Store Store, long OrderId, int status);

        long UpdateStaffStatus(Store Store, long StaffId, bool IsOnRoad);

        List<StaffDeliveryModel> GetDeliveryStaffsOnly(Store Store);

    }
}
