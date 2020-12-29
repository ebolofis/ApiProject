using Symposium.Models.Models;
using System;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IDeliveryOrdersFlows
    {
        List<OrderStatusModel> GetOrderStatusTimeChanges(Store Store, long OrderId);
        List<StatusCounts> StateCounts(Store store, DeliveryFilters filters);
        PaginationModel<DeliveryStatusOrders> PagedOrdersByStateFlow(Store Store, int status, int pageNumber, int pageLength, DeliveryFilters filter);
        DeliveryCustomersShippingAddressModel UpdateShippingCoordinates(Store Store, long InvoiceId, DeliveryCustomersShippingAddressModel model);

        int UpdateOrderStatusForReturned(Store Store, long OrderId, int status);

        long UpdateStaffStatus(Store Store, long StaffId, bool IsOnRoad);

        List<StaffDeliveryModel> GetDeliveryStaffsOnly(Store Store);
    }
}
