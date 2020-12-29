using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT {
    public interface IVatDT {

        /// <summary>
        /// Return the list of vats registered Accounts
        /// </summary>
        /// <param name="Store">Store</param>
        /// <returns>a list of AccountModel</returns>
        List<VatModel> GetVatModels(Models.Models.DBInfoModel Store);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
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

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<VatSched_Model> model);

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        long? GetIdByDAIs(DBInfoModel Store, long dAId);

        /// <summary>
        /// Retrun's a list with all vats Included deleted for Delivery Agent;
        /// If One of them is deleted and on da is not then we have to update to not deleted
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<VatModel> GetAllVats(DBInfoModel Store, IDbConnection dbTran = null, IDbTransaction dbTransact = null);
    }
}
