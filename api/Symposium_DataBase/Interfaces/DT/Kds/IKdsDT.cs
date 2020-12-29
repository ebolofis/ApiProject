using Symposium.Models.Models;
using Symposium.Models.Models.Kds;
using System.Collections.Generic;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Kds
{
    public interface IKdsDT
    {
        List<KdsResponceModel> GetKdsOrdersMain(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model);
        OrderStatusModel GetOrderStatusModel(DBInfoModel dbInfo, long OrderId, byte? Status);
        List<OrderDetailIngredientsModel> GetDetailIgredients(DBInfoModel dbInfo, long OrderDetailsId);
        StaffModels GetOrderStaffModel(DBInfoModel dbInfo, long StaffId);
        string SetRemarks(DBInfoModel dbInfo, long OrderDetailsId, string OrderNotes);
        /// <summary>
        /// Get All Ingredients In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>List OrderDetailIngredientsModel</returns>
        List<OrderDetailIngredientsKDSModel> GetKdsIngredients(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model);
        bool UpdateKitchenStatus(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model);

        KdsTopRowResponceModel GetTopOrderDetailIngredientId(DBInfoModel dbInfo, KdsDeleteIngredientsRequestModel Model);

        bool UpdatePendingQty(DBInfoModel dbInfo, KdsTopRowResponceModel topRowModel);

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
