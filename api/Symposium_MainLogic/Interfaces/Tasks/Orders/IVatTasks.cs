using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.DT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks {
    public interface IVatTasks
    {


        /// <summary>
        /// Return a list with all vats Included deleted
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
        List<VatModel> GetAllVats(DBInfoModel Store, IDbConnection dbTran = null, IDbTransaction dbTransact = null);

        /// <summary>
        /// return the Vat model of a product and a given price-list
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        VatModel GetProductVatFromPricelist(DBInfoModel dbInfo, long productID, long priceListId);

        /// <summary>
        /// return the Vat model of an extra/ingredient and a given price-list
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        VatModel GetExtraVatFromPricelist(DBInfoModel dbInfo, long extraID, long priceListId);

        /// <summary>
        /// Return list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<VatSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, VatModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<VatModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, VatModel item);
    }
}
