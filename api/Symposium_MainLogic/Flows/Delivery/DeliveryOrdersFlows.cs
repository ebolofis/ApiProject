using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class DeliveryOrdersFlows : IDeliveryOrdersFlows
    {
        IDeliveryOrderTasks dotask;
        IDeliveryCustomerTasks dctask;
        //IPaginationHelper<DeliveryCustomerModel> delcpageHlp;

        public DeliveryOrdersFlows(IDeliveryOrderTasks _dotask, IDeliveryCustomerTasks _dctask)//, IPaginationHelper<DeliveryCustomerModel> _delcpageHlp)
        {
            this.dotask = _dotask;
            this.dctask = _dctask;
            //this.delcpageHlp = _delcpageHlp;
        }

        public List<long> GetKdsOrdersIdList(DBInfoModel dbInfo)
        {
            return KdsOrderIdListHelper.readKdsOrderIdsList();
        }

        public bool ClearKdsOrdersIdList(DBInfoModel dbInfo)
        {
            return KdsOrderIdListHelper.clearKdsOrderIdsList();
        }

        public List<StatusCounts> StateCounts(DBInfoModel dbInfo, DeliveryFilters filters)
        {
            return dotask.StateCountsTask(dbInfo, filters);
        }

        public List<OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel dbInfo, long OrderId)
        {
            return dotask.GetOrderStatusTimeChanges(dbInfo, OrderId);
        }

        public PaginationModel<DeliveryStatusOrders> PagedOrdersByStateFlow(DBInfoModel dbInfo, int status, int pageNumber, int pageLength, DeliveryFilters filter)
        {
            PaginationModel<DeliveryStatusOrders> ordersList = new PaginationModel<DeliveryStatusOrders>();
            ordersList = dotask.PagedOrdersByStateTask(dbInfo, status, pageNumber, pageLength, filter);
            foreach(DeliveryStatusOrders orders in ordersList.PageList)
            {
                orders.orderStatusModel = dotask.GetOrderStatusTimeChanges(dbInfo, orders.OrderId);
            }
            return ordersList;
        }

        public DeliveryCustomersShippingAddressModel UpdateShippingCoordinates(DBInfoModel dbInfo, long InvoiceId, DeliveryCustomersShippingAddressModel model)
        {
            return dotask.UpdateShippingCoordinatesTask(dbInfo, InvoiceId, model);
        }

        public int UpdateOrderStatusForReturned(DBInfoModel dbInfo, long OrderId, int status)
        {
            return dotask.UpdateOrderStatusForReturned(dbInfo, OrderId, status);
        }

        public long UpdateStaffStatus(DBInfoModel Store, long StaffId, bool IsOnRoad)
        {
            return dotask.UpdateStaffStatus(Store, StaffId, IsOnRoad);
        }

        public List<StaffDeliveryModel> GetDeliveryStaffsOnly(DBInfoModel Store)
        {
            return dotask.GetDeliveryStaffsOnly(Store);
        }

        public List<StaffDeliveryModel> GetDeliveryStaffsSignInOnly(DBInfoModel Store, long PosInfoId)
        {
            return dotask.GetDeliveryStaffsSignInOnly(Store, PosInfoId);
        }

    }
}
