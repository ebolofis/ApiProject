using Symposium.Models.Models;
using Symposium.Models.Models.Kds;
using Symposium.WebApi.DataAccess.Interfaces.DT.Kds;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Kds;
using System.Collections.Generic;


namespace Symposium.WebApi.MainLogic.Tasks.Kds
{

    public class KdsTasks : IKdsTasks
    {
        IKdsDT kdsDt;


        public KdsTasks(IKdsDT _kdsDt)
        {
            this.kdsDt = _kdsDt;
        }

        /// <summary>
        /// Get All Order Details In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>KdsResponceModel</returns>
        public List<KdsResponceModel> GetKdsOrdersMain(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model)
        {
            return kdsDt.GetKdsOrdersMain(dbInfo, Model);
        }

        /// <summary>
        /// Update Kitchen Status for Specific Order Detail
        /// </summary>
        /// <returns></returns>
        public bool UpdateKitchenStatus(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model)
        {
            return kdsDt.UpdateKitchenStatus(dbInfo, Model);
        }

        /// <summary>
        /// Get Top OrderDetailIngredientId
        /// </summary>
        /// <returns></returns>
        public KdsTopRowResponceModel GetTopOrderDetailIngredientId(DBInfoModel dbInfo, KdsDeleteIngredientsRequestModel Model)
        {
            return kdsDt.GetTopOrderDetailIngredientId(dbInfo, Model);
        }

        /// <summary>
        /// Update PendingQty
        /// </summary>
        /// <returns></returns>
        public bool UpdatePendingQty(DBInfoModel dbInfo, KdsTopRowResponceModel topRowModel)
        {
            return kdsDt.UpdatePendingQty(dbInfo, topRowModel);
        }

        public List<long> GetKdsIds(DBInfoModel dbInfo)
        {
            return kdsDt.GetKdsIds(dbInfo);
        }

        public bool CheckRestOrderDetails(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model)
        {
            return kdsDt.CheckRestOrderDetails(dbInfo, Model);
        }

        public OrderStatusModel GetOrderStatusModel(DBInfoModel dbInfo, long OrderId, byte? Status)
        {
            return kdsDt.GetOrderStatusModel(dbInfo, OrderId, Status);
        }

        public List<OrderDetailIngredientsModel> GetDetailIgredients(DBInfoModel dbInfo, long OrderDetailsId)
        {
            return kdsDt.GetDetailIgredients(dbInfo, OrderDetailsId);
        }

        public StaffModels GetOrderStaffModel(DBInfoModel dbInfo, long StaffId)
        {
            return kdsDt.GetOrderStaffModel(dbInfo, StaffId);
        }

        public string SetRemarks(DBInfoModel dbInfo, long OrderDetailsId, string OrderNotes)
        {
            return kdsDt.SetRemarks(dbInfo, OrderDetailsId, OrderNotes);
        }

        /// <summary>
        /// Get All Ingredients In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>List OrderDetailIngredientsKDSModel</returns>
        public List<OrderDetailIngredientsKDSModel> GetKdsIngredients(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model)
        {
            return kdsDt.GetKdsIngredients(dbInfo, Model);
        }

        /// <summary>
        /// Insert on OrderDetailIgredientsKDS a returnd product based on OrderId , OrderDetailId and productid
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="orderId"></param>
        /// <param name="orderDetailId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool ReturnProductOnKdsTable(DBInfoModel dbInfo, long orderId, long orderDetailId, long productId)
        {
            return kdsDt.ReturnProductOnKdsTable(dbInfo, orderId, orderDetailId, productId);
        }
    }
}

