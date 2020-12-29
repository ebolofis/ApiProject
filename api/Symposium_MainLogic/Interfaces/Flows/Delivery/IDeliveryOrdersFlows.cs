using Symposium.Models.Models;
using System;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IDeliveryOrdersFlows
    {
        List<OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel dbInfo, long OrderId);
        List<StatusCounts> StateCounts(DBInfoModel dbInfo, DeliveryFilters filters);
        PaginationModel<DeliveryStatusOrders> PagedOrdersByStateFlow(DBInfoModel dbInfo, int status, int pageNumber, int pageLength, DeliveryFilters filter);
        DeliveryCustomersShippingAddressModel UpdateShippingCoordinates(DBInfoModel dbInfo, long InvoiceId, DeliveryCustomersShippingAddressModel model);

        int UpdateOrderStatusForReturned(DBInfoModel dbInfo, long OrderId, int status);

        long UpdateStaffStatus(DBInfoModel dbInfo, long StaffId, bool IsOnRoad);

        List<StaffDeliveryModel> GetDeliveryStaffsOnly(DBInfoModel dbInfo);

        List<StaffDeliveryModel> GetDeliveryStaffsSignInOnly(DBInfoModel Store, long PosInfoId);

        List<long> GetKdsOrdersIdList(DBInfoModel dbInfo);

        bool ClearKdsOrdersIdList(DBInfoModel dbInfo);
    }
}
