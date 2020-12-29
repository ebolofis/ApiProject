using Symposium.Models.Models;
using Symposium.Models.Models.Kds;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Kds
{
    public interface IKdsTasks
    {
        /// <summary>
        /// Get All Order Details In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>KdsResponceModel</returns>
        List<KdsResponceModel> GetKdsOrdersMain(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model);
        OrderStatusModel GetOrderStatusModel(DBInfoModel dbInfo, long OrderId, byte? Status);
        List<OrderDetailIngredientsModel> GetDetailIgredients(DBInfoModel dbInfo, long OrderDetailsId);
        StaffModels GetOrderStaffModel(DBInfoModel dbInfo, long StaffId);
        string SetRemarks(DBInfoModel dbInfo, long OrderDetailsId, string OrderNotes);

        /// <summary>
        /// Get All Ingredients In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>List OrderDetailIngredientsKDSModel</returns>
        List<OrderDetailIngredientsKDSModel> GetKdsIngredients(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model);

        /// <summary>
        /// Update Kitchen Status for Specific Order Detail
        /// </summary>
        /// <returns></returns>
        bool UpdateKitchenStatus(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model);

        /// <summary>
        /// Get Top OrderDetailIngredientId
        /// </summary>
        /// <returns></returns>
        KdsTopRowResponceModel GetTopOrderDetailIngredientId(DBInfoModel dbInfo, KdsDeleteIngredientsRequestModel Model);

        /// <summary>
        /// Update PendingQty
        /// </summary>
        /// <returns></returns>
        bool UpdatePendingQty(DBInfoModel dbInfo, KdsTopRowResponceModel OrderDetailIngredientId);

        List<long> GetKdsIds(DBInfoModel dbInfo);

        bool CheckRestOrderDetails(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model);

        /// <summary>
        /// Insert on OrderDetailIgredientsKDS a returnd product based on OrderId , OrderDetailId and productid
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="orderId"></param>
        /// <param name="orderDetailId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        bool ReturnProductOnKdsTable(DBInfoModel dbInfo, long orderId, long orderDetailId, long productId);
    }
}
