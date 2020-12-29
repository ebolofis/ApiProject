using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Products
{
    public interface IProductBarcodesDT
    {
        /// <summary>
        /// Return's list of ProductBarcodes after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductBarcodesSched_Model> model);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<ProductBarcodesSched_Model> model);

        /// <summary>
        /// Return's a model based on DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        ProductBarcodesDTO GetModelByDAId(DBInfoModel Store, long DAId);

    }
}
