using Symposium.Models.Models;
using Symposium.Models.Models.ExternalSystems.Efood;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.ExternalSystems
{
   public interface IEfoodDT
    {

        /// <summary>
        /// search an item from efood bucket based on efood order id
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="id">efood order id</param>
        /// <returns>EFoodBucketModel</returns>
        ExtDeliveryBucketModel GetOrder(DBInfoModel Store, string id);

        /// <summary>
        /// return all bucket from DB
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
         List<ExtDeliveryBucketModel> GetOrders(DBInfoModel Store);

        /// <summary>
        /// returns size of bucket
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        int GetBucketSize(DBInfoModel Store);

        /// <summary>
        /// insert order into bucket
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
        void UpsertOrder(DBInfoModel Store, ExtDeliveryBucketModel model);


        /// <summary>
        /// delete order from bucket
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        void DeleteOrder(DBInfoModel Store, string Id);

        /// <summary>
        /// mark a bucket order as deleted
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        void MarkDeleted(DBInfoModel Store, string Id);

        /// <summary>
        /// delete old orders marked as deleted from bucket
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
        void DeleteOldOrder(DBInfoModel Store);


    }
}
