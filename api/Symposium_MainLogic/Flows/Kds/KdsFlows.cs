using Symposium.Models.Models;
using Symposium.Models.Models.Kds;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Kds;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Kds;
using System.Collections.Generic;
using System.Linq;

namespace Symposium.WebApi.MainLogic.Flows.Kds
{
    public class KdsFlows : IKdsFlows
    {
        IKdsTasks kdsTasks;

        public KdsFlows(IKdsTasks _kdsTasks)
        {
            this.kdsTasks = _kdsTasks;
        }

        /// <summary>
        /// Get All Order Details In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>KdsResponceModel</returns>
        public List<KdsResponceModel> GetKdsOrders(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model)
        {
            List<KdsResponceModel> result = new List<KdsResponceModel>();
            result = kdsTasks.GetKdsOrdersMain(dbInfo, Model);
            foreach(KdsResponceModel item in result)
            {
                item.OrderStatus = kdsTasks.GetOrderStatusModel(dbInfo, item.OrderId, item.Status);
                item.OrderDetailIgredients = kdsTasks.GetDetailIgredients(dbInfo, item.OrderDetailsId);
                //Set Staff
                if (item.LoginStaffId != null)
                {
                    item.StaffModel = kdsTasks.GetOrderStaffModel(dbInfo, (long)item.LoginStaffId);
                }
                //Set Order and Items Comments
                item.ItemRemark = kdsTasks.SetRemarks(dbInfo, item.OrderDetailsId, item.OrderNotes);
            }

            return result;
        }

        /// <summary>
        /// Update Kitchen Status for Specific Order Detail
        /// </summary>
        /// <returns>KdsUpdateResponceModel</returns>
        public KdsUpdateResponceModel UpdateKitchenStatus(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model)
        {
            KdsUpdateResponceModel result = new KdsUpdateResponceModel();
            KdsGetOrdersRequestModel tmpModel = new KdsGetOrdersRequestModel();
            bool sendSignalR = false;
            tmpModel.KdsIds = kdsTasks.GetKdsIds(dbInfo);
            tmpModel.SaleTypesIds = Model.SaleTypesIds;
            Model.KdsIds = tmpModel.KdsIds;
            //Update Kitchen Status
            bool res = kdsTasks.UpdateKitchenStatus(dbInfo, Model);
            if(Model.KitchenStatus == 1 && Model.CurrentKitchenStatus == 0)
            {
                ReturnProductOnKdsTable(dbInfo, Model.OrderId, Model.OrderDetailId, Model.ProductId);
            }
            //Check Rest Order Details Kitchen Status and Update status
            sendSignalR = kdsTasks.CheckRestOrderDetails(dbInfo, Model);
            result.SendSignalR = sendSignalR;
            //Get Orders to Return
            result.KdsOrdersModel = GetKdsOrders(dbInfo, tmpModel);
            return result;
        }

        /// <summary>
        /// Delete one Ingredient and update PendingQty
        /// </summary>
        /// <returns>List KdsResponceModel</returns>
        public List<KdsResponceModel> DeleteIngredient(DBInfoModel dbInfo, KdsDeleteIngredientsRequestModel Model)
        {
            List<KdsResponceModel> result = new List<KdsResponceModel>();
            KdsGetOrdersRequestModel tmpModel = new KdsGetOrdersRequestModel();
            tmpModel.KdsIds = Model.KdsIds;
            tmpModel.SaleTypesIds = Model.SaleTypesIds;
            //Get OrderDetailIgredientId of top Order Detail
            KdsTopRowResponceModel topRowModel = kdsTasks.GetTopOrderDetailIngredientId(dbInfo, Model);
            //Update PendingQty
            bool res = kdsTasks.UpdatePendingQty(dbInfo, topRowModel);
            //Get Orders to Return
            result = GetKdsOrders(dbInfo, tmpModel);
            return result;
        }

        /// <summary>
        /// Get All Ingredients In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>List OrderDetailIngredientsModel</returns>
        public List<KdsIngredientsResponceModel> GetKdsIngredients(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model)
        {
            List<KdsIngredientsResponceModel> res = new List<KdsIngredientsResponceModel>();
            List<OrderDetailIngredientsKDSModel> result = new List<OrderDetailIngredientsKDSModel>();
            result = kdsTasks.GetKdsIngredients(dbInfo, Model);
            res = result.GroupBy(c => c.IgredientsId)
                                               .Select(
                                                    g =>
                                                        new KdsIngredientsResponceModel
                                                        {
                                                            IngredientId = g.Key,
                                                            IngredientDescr = g.First().Description,
                                                            IngredientPendingQty = g.Sum(s => s.Qty)
                                                        }
                                               ).ToList();

            return res;
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
            return kdsTasks.ReturnProductOnKdsTable(dbInfo, orderId, orderDetailId, productId);
        }
    }
}
