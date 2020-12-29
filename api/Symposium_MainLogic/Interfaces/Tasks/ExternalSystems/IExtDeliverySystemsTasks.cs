using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems.Efood;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    /// <summary>
    ///  Tasks for External Delivery Systems like e-food
    /// </summary>
    public interface IExtDeliverySystemsTasks
    {

        /// <summary>
        /// Return the list of instances of plugins 'ExternalDelivery'
        /// </summary>
        /// <returns></returns>
        List<object> InstansiateExternalDeliveryPlugins();


        /// <summary>
        ///  Get the list of orders from external delivery web-apis by calling plugins 'ExternalDelivery'.
        /// </summary>
        /// <param name="dbInfo">dbInfo</param>
        ///  <param name="extDeliveryPlugins">list of instances of plugins 'ExternalDelivery'</param>
        /// <returns>the  list of orders </returns>
        List<DA_ExtDeliveryModel> GetfromDeliveryApis(DBInfoModel dbInfo, List<object> extDeliveryPlugins);

        /// <summary>
        /// Confirm order back to External Delivery system
        /// </summary>
        /// <param name="plugins">plugins instances</param>
        /// <param name="order">order to confirm</param>
        void ConfirmOrder(List<object> plugins, DA_ExtDeliveryModel order);

        /// <summary>
        /// Get the list of orders from a disk's path -  
        /// CodeSuffix='W', origin='Web'
        /// <param name = "path" >path for DA web orders' xml files</param>
        /// </summary>
      //  OrdersEfood GetfromPath(string path);

        /// <summary>
        /// From the list of orders remove them that already exist in DA_Orders or EFoodBucket tables
        /// If ExtId1 not found return 0;
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="orders">OrdersEfood</param>
        void RemoveExistingOrders(DBInfoModel dbInfo, List<DA_ExtDeliveryModel> orders);

        /// <summary>
        /// Match e-food Shipping address based on ExtId1 against the existing Addresses from DB
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">DA_OrderModel order</param>
        bool MatchShippingDetailsFromDB(DBInfoModel dbInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// Match e-food Shipping address based on Proximity against the existing Addresses from DB
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">DA_OrderModel order</param>
        bool MatchShippingDetailsProximity(DBInfoModel dbInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// Match a customer from DB, or insert new customer
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">DA_OrderModel order</param>
        bool MatchCustomer(DBInfoModel dbInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// Match e-food order with  timologio details and billing address
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">e-food order</param>
        void MatchBillingDetails(DBInfoModel dbInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// Match e-food products/extras  with existing products/extras in DB. Also fill in the products/extras descriptions. 
        /// If matching fails then add error messages for every mismatch... 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">e-food order</param>
        void MatchProductsExtras(DBInfoModel dbInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// match store by pos address id. 
        /// Εάν StoreId = 0 Τότε η διεύθυνση δεν αντιστοιχεί σε κατάστημα
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">efood order</param>
        void SelectPolygonByAddressId(DBInfoModel dbInfo, DA_ExtDeliveryModel order);

        /// <summary>
        /// Find Longitude and Latitude for a DA_AddressModel only if Longitude & Latitude do not exist
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="addrExt">DA_AddressModel</param>
        void GeocodeAddress(DBInfoModel dbInfo, DA_AddressModel addrExt);


        /// <summary>
        /// return all efood Bucket from DB
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <returns></returns>
        List<DA_ExtDeliveryModel> GetOrders(DBInfoModel dbInfo);

        /// <summary>
        /// search an item from efood bucket based on efood order id
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="id">efood order id</param>
        /// <returns>EFoodBucketModel</returns>
        ExtDeliveryBucketModel GetOrder(DBInfoModel Store, string id);

        /// <summary>
        /// insert order into bucket
        /// </summary>
        /// <param name="dbInfo">db</param>
        ///  <param name="EFoodBucketModel">EFoodBucketModel  (model from table EFoodBucket)</param>
        /// <returns></returns>
        void UpsertBucketOrder(DBInfoModel dbInfo, ExtDeliveryBucketModel model);

        /// <summary>
        /// insert order into bucket
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="OrderEfood">OrderEfood</param>
        /// <returns></returns>
        void UpsertBucketOrder(DBInfoModel dbInfo, DA_ExtDeliveryModel orderEfood);

        /// <summary>
        /// Insert order to DA_Orders or to EfoodBucket.  
        /// Return true if order is saved to DA_Orders or to EfoodBucket...
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        bool SaveOrder(DBInfoModel DBInfo, DA_ExtDeliveryModel order);


        /// <summary>
        /// delete order from bucket
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        void DeleteOrder(DBInfoModel dbInfo, string Id);


        /// <summary>
        /// construct order's errors into the form: 'error1|error2|error3'
        /// </summary>
        /// <param name="order">OrderEfood</param>
        /// <param name="error">the error to add</param>
        void constructError(DA_OrderExtDeliveryModel order, string error);

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

        /// <summary>
        /// check if order has nulls. True if all are OK.
        /// </summary>
        /// <param name="order"></param>
      ///  bool checkNulls(OrderEfood order);

        /// <summary>
        /// delete an xml order file
        /// </summary>
        /// <param name="order"></param>
       // bool DeleteOrderFile(OrderEfood order);

        /// <summary>
        /// sanitize Orders
        /// </summary>
      //  void sanitizeOrders(OrdersEfood orders);

        /// <summary>
        ///  Send Acceptance to external delivery web-api.
        /// </summary>
        /// <param name="daorder">DA_OrderModel</param>
        /// <param name="order">OrderEfood</param>
        /// <returns> </returns>
       //  void UpdateDeliveryApi(DA_OrderModel daorder, OrderEfood order);

      
    }
}
