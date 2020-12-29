using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium_DTOs.PosModel_Info;
using Symposium.Models.Models.Products;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IPageButtonFlows
    {
        /// <summary>
        /// Get page buttons for a specific Pos, PageId and Pricelist
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <param name="pageid"></param>
        /// <param name="pricelistid"></param>
        PageButtonPreviewModel GetPageButtons(DBInfoModel Store,string storeid, int posid, int pageid, int pricelistid, bool isPos = false);

        /// <summary>
        /// Return's list of Page Buttons after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PageButtonSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, PageButtonModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<PageButtonModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, PageButtonModel item);

        //Search External Product WebPos
        List<PageButtonPricelistDetailsAssoc> SearchExternalProduct(DBInfoModel DBInfo, string description);
    }
}
