using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems.Efood;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems
{
    /// <summary>
    /// Flows for External Delivery Systems like e-food
    /// </summary>
    public interface IExtDeliverySystemsFlows
    {
        /// <summary>
        /// Get the list of orders from e-food and add them to DA_Orders or to EfoodBucket
        /// </summary>
         void GetEFoodOrders();

        /// <summary>
        /// Insert an E-food Order to DA_Orders or to EfoodBucket. 
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="order"></param>
        /// <param name="changePrices">true: for an e-food order ADD prices to extras and REDUCE prices from main products</param>
        /// <returns></returns>
        DA_ExtDeliveryModel InsertEFoodOrder(DBInfoModel DBInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// get the list of orders from e-food bucket
        /// </summary>
        /// <returns></returns>
        List<DA_ExtDeliveryModel> getBucket(DBInfoModel dbInfo);

        /// <summary>
        /// delete an item from bucket
        /// </summary>
        /// <param name="id">e-food order id</param>
        void DeleteBucketItem(DBInfoModel dbInfo, string id);

        /// <summary>
        /// insert an item from bucket
        /// </summary>
        /// <param name="model">EFoodBucketModel</param>
         void InsertBucketItem(DBInfoModel dbInfo, ExtDeliveryBucketModel model);

        /// <summary>
        /// check e-food order again and insert order to DA_Orders or EfoodBucket 
        /// This action is used when OrderEfood had been MANUALLY repaired .
        /// Return the same OrderEfood with updated error messages. 
        ///   If error='' then order is inserted into DA_Orders and deleted from EfoodBucket.
        /// </summary>
        /// <param name="DBInfo">db </param>
        /// <param name="order">OrderEfood</param>
        DA_ExtDeliveryModel ResendDAOrder(DBInfoModel DBInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// mark a bucket order as deleted
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        void MarkDeleted(DBInfoModel dbInfo, string Id);
    }
}
