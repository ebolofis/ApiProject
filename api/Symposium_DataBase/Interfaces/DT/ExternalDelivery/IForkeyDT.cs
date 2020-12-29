using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.ExternalDelivery
{
    public interface IForkeyDT
    {
        /// <summary>
        /// Provides basic lookpus for forkey functionallity
        /// PosInfo, PosInfo Details, Pricelists, SalesTypes , Accounts, Staff, InvoiceTypes
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        ForkeyLookups GetLookups(DBInfoModel Store);

        /// <summary>
        /// Based on forkey order dishes recipe external_id witch is the mapping entity with the Product.Code 
        /// on WebPOS DB creates a receipt detail list and applies items based on recipe code
        /// Method also checks if forkeyorder vat is a valid pos ent else it returns a Vat missmatch
        /// Then it asks for product with specific code and if it does not exist on DB then returns a MISSING_DISH_ID error
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="order"></param>
        /// <param name="fposents"></param>
        /// <returns></returns>
        List<ReceiptDetails> CreateReceiptDetailsFromProductCodes(DBInfoModel Store, ForkeyDeliveryOrder order, ForkeyPosEntities fposents);
        
        /// <summary>
        /// Provide a filter model to return LocalEntities of Forkey Order
        /// Order, Invoices, OrderStatuses
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ForkeyLocalEntities GetForkeyOrderLocalEntities(DBInfoModel Store, ForkeyDeliveryOrder model);

        /// <summary>
        /// Based on forkey order creates a dynamic order filter asks order DAO  for entries with external key
        /// and external type having endof day id null and isdeleted false  returnes true if an order found with current creteria 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool CheckExist(DBInfoModel Store, ForkeyDeliveryOrder model);


        /// <summary>
        /// Function providing storeid and orderid  parses Forkey order entities 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        ForkeyLocalEntities GetForkeyEntities(string storeid, long orderid, int? extType);

        /// <summary>
        /// Providing storeid and orderno returns forkey entities
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderno"></param>
        /// <returns></returns>
        ForkeyLocalEntities GetForkeyEntitiesByOrderNo(string storeid, string orderno);

        /// <summary>
        /// Based on invoice id joins entities to update Order.ExtObj.Deseriallized.isPrinted
        /// selects invoices, binded to orderdetailinvs, binded to orderdetail , binded to Order
        /// Deserializes obj then updates isPrinted from Obj changes value then updates orders collected
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="printed"></param>
        /// <returns></returns>
        List<OrderDTO> ChangeForkeyIsPrintedExtObj(DBInfoModel Store, long InvoiceId, bool printed = true);

    }
}
