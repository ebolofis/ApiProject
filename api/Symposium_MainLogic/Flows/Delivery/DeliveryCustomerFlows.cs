using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;



namespace Symposium.WebApi.MainLogic.Flows
{
    public class DeliveryCustomerFlows : IDeliveryCustomerFlows
    {
        IDeliveryCustomerTasks delctask;
        IDelivery_CustomersShippingAddressTasks delShippingAddressTask;
        IDelivery_CustomersBillingAddressTasks delBillingAddressTask;
        IPaginationHelper<DeliveryCustomerModel> delcpageHlp;

        public DeliveryCustomerFlows(IDeliveryCustomerTasks _delctask, IDelivery_CustomersShippingAddressTasks _delShippingAddressTask, IDelivery_CustomersBillingAddressTasks _delBillingAddressTask, IPaginationHelper<DeliveryCustomerModel> _delcpageHlp)
        {
            this.delctask = _delctask;
            this.delShippingAddressTask = _delShippingAddressTask;
            this.delBillingAddressTask = _delBillingAddressTask;
            this.delcpageHlp = _delcpageHlp;
        }

        /// <summary>
        /// Return object with 3 List lookups for DeliveryCustomer Entities Types
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public DeliveryCustomerLookupModel GetLookups(DBInfoModel dbInfo)
        {
            return delctask.GetDeliveryLookups(dbInfo);
        }

        /// <summary>
        /// Search delivery customers in paged result with filters as flat search model 
        /// if a filter property is null or empty or does not exist it is been ignored
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters">Name , address , phone , trn</param>
        /// <returns>Paged result of flat lookup</returns>
        public PaginationModel<DeliveryCustomerSearchModel> SearchPagedCustomersFlow(DBInfoModel dbInfo, int page, int pageSize, DeliveryCustomerFilterModel filters)
        {
            return delctask.SearchPagedCustomersTask(dbInfo, page, pageSize, filters);
        }

        /// <summary>
        /// Provide Id to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="Id">Unique identifier of LocalDB Delivery_Customer table</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        public DeliveryCustomerModel GetCustomerById(DBInfoModel dbInfo, long Id, long PhoneId, long SAddressId, long BAddressId) {
            return delctask.GetCustomerById(dbInfo, Id , PhoneId , SAddressId, BAddressId);
        }

        /// <summary>
        /// Givven Customer flow parses model to tasks 
        /// if model id is 0 then is swiches to add Task
        /// else swiches to UpdateTask
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model">Model to handle with dtos </param>
        /// <returns></returns>
        public DeliveryCustomerModel UpsertCustomer(DBInfoModel dbInfo, DeliveryCustomerModel model)
        {
            if (model.ID == 0)
            {
                return delctask.InsertCustomerTask(dbInfo, model);
            } else {
                return delctask.UpdateCustomerTask(dbInfo, model);
            }
        }

        public DeliveryCustomerModel UpsertCustomerByExternalId(DBInfoModel dbInfo, DeliveryCustomerModel model)
        {
            ExternalSystemOrderEnum custExtType = model.ExtType != null ? (ExternalSystemOrderEnum)model.ExtType : ExternalSystemOrderEnum.Default;
            long? customerId = delctask.FindCustomerByExternalId(dbInfo, model.ExtCustId, custExtType);
            if (customerId == null || customerId == 0)
            {
                return delctask.InsertCustomerTask(dbInfo, model);
            }
            else
            {
                long custId = customerId ?? 0;
                model.ID = custId;
                if (model.ShippingAddresses != null)
                {
                    foreach (DeliveryCustomersShippingAddressModel address in model.ShippingAddresses)
                    {
                        ExternalSystemOrderEnum addrExtType = address.ExtType != null ? (ExternalSystemOrderEnum)address.ExtType : ExternalSystemOrderEnum.Default;
                        Delivery_CustomersShippingAddressModel addressInDB = delShippingAddressTask.GetModelByExternalKey(dbInfo, custId, address.ExtKey, addrExtType);
                        long addrId = addressInDB != null ? addressInDB.ID ?? 0 : 0;
                        address.ID = addrId;
                        address.CustomerID = customerId ?? 0;
                    }
                }
                if (model.BillingAddresses != null)
                {
                    foreach (DeliveryCustomersBillingAddressModel address in model.BillingAddresses)
                    {
                        ExternalSystemOrderEnum addrExtType = address.ExtType != null ? (ExternalSystemOrderEnum)address.ExtType : ExternalSystemOrderEnum.Default;
                        Delivery_CustomersBillingAddressModel addressInDB = delBillingAddressTask.GetModelByExternalKey(dbInfo, custId, address.ExtKey, addrExtType);
                        long addrId = addressInDB != null ? addressInDB.ID ?? 0 : 0;
                        address.ID = addrId;
                        address.CustomerID = customerId ?? 0;
                    }
                }
                return delctask.UpdateCustomerTask(dbInfo, model);
            }
        }

        /// <summary>
        /// Updates Delivery Customer by model provided 
        /// Updated Guest with new Values and selected phones and ship and bill  addresses 
        /// Uses Tasks of upsert and updateGuest
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model">Guest model</param>
        /// <returns> Updated Guest from database </returns>
        public GuestModel UpdateCustomerAndGuest(DBInfoModel dbInfo, DeliveryCustomerModel model)
        {
                return delctask.UpdateCustomerAndGuest(dbInfo, model);
        }

        /// <summary>
        /// Used for delivery service
        /// Upserts Delivery Customer by model provided Using *****ExtCustId**********
        /// UpsertsGuest with new Values and selected phones and ship and bill  addresses 
        /// Uses Tasks of upsert and updateGuest
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model">Guest model</param>
        /// <returns> Updated Guest from database </returns>
        public DeliveryCustomerModelDS UpsertCustomerAndGuest(DBInfoModel dbInfo, DeliveryCustomerModelDS model)
        {
            return delctask.UpsertCustomerAndGuest(dbInfo, model);
        }


        /// <summary>
        /// Delete Customer by id provided 
        /// calls  delivery customer task to delete by id givven
        /// Task also deletes addresses Assocs and Phones by this id as (FK) customerid 
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="id">Id of Delivery Customer Entry</param>
        /// <returns>Id Provided for delete Functionality</returns>
        public long DeleteCustomer(DBInfoModel dbInfo, long id) {
            return delctask.DeleteCustomerTask(dbInfo, id);
        }
    }
}
