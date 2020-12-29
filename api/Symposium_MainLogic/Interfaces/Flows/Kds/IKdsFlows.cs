

using Symposium.Models.Models;
using Symposium.Models.Models.Kds;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Kds
{
    public interface IKdsFlows
    {
        /// <summary>
        /// Get All Order Details In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>KdsResponceModel</returns>
        List<KdsResponceModel> GetKdsOrders(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model);

        /// <summary>
        /// Update Kitchen Status for Specific Order Detail
        /// </summary>
        /// <returns>KdsUpdateResponceModel</returns>
        KdsUpdateResponceModel UpdateKitchenStatus(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model);

        /// <summary>
        /// Delete one Ingredient and update PendingQty
        /// </summary>
        /// <returns>List KdsResponceModel</returns>
        List<KdsResponceModel> DeleteIngredient(DBInfoModel dbInfo, KdsDeleteIngredientsRequestModel Model);

        /// <summary>
        /// Get All Ingredients In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>List OrderDetailIngredientsModel</returns>
        List<KdsIngredientsResponceModel> GetKdsIngredients(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model);

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
