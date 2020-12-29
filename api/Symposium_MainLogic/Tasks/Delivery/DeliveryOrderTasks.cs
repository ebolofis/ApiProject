using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class DeliveryOrderTasks : IDeliveryOrderTasks
    {
        IDeliveryOrdersDT doDB;
        public DeliveryOrderTasks(IDeliveryOrdersDT _doDB)//, IGuestDT _guestDB)
        {
            this.doDB = _doDB;
            // this.guestDB = _guestDB;
        }

        public List<StatusCounts> StateCountsTask(DBInfoModel Store, DeliveryFilters filter) => doDB.StatesCounts(Store, filter);

        public List<OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel Store, long OrderId)
        {
            return doDB.GetOrderStatusTimeChanges(Store, OrderId);
        }


        public PaginationModel<DeliveryStatusOrders> PagedOrdersByStateTask(DBInfoModel Store, int status, int pageNumber, int pageLength, DeliveryFilters filter)
        {
           return doDB.PagedOrdersByState(Store, status, pageNumber, pageLength, filter);
        }

        public DeliveryCustomersShippingAddressModel UpdateShippingCoordinatesTask(DBInfoModel Store, long InvoiceId, DeliveryCustomersShippingAddressModel model) => doDB.UpdateShippingCoordinates(Store, InvoiceId, model);

        public int UpdateOrderStatusForReturned(DBInfoModel Store, long OrderId, int status)
        {
            return doDB.UpdateOrderStatusForReturned(Store, OrderId, status);
        }

        public long UpdateStaffStatus(DBInfoModel Store, long StaffId, bool IsOnRoad)
        {
            return doDB.UpdateStaffStatus(Store, StaffId, IsOnRoad);
        }

        public List<StaffDeliveryModel> GetDeliveryStaffsOnly(DBInfoModel Store)
        {
            return doDB.GetDeliveryStaffsOnly(Store);
        }

        public List<StaffDeliveryModel> GetDeliveryStaffsSignInOnly(DBInfoModel Store, long PosInfoId)
        {
            List<StaffDeliveryModel> signInOnlyModel = new List<StaffDeliveryModel>();
            List<StaffDeliveryModel> deliveryStaffsOnlyModel = new List<StaffDeliveryModel>();
            deliveryStaffsOnlyModel = GetDeliveryStaffsOnly(Store);
            foreach(StaffDeliveryModel model in deliveryStaffsOnlyModel)
            {
                bool isClockIn = false;
                isClockIn = doDB.GetDeliveryStaffsSignInOnly(Store, PosInfoId, model.StaffId);
                if (isClockIn == true)
                {
                    signInOnlyModel.Add(model);
                }
            }

            return signInOnlyModel.OrderBy(o => o.StatusTimeChanged).ToList();
        }
    }
}
