using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium_DTOs.PosModel_Info;
using Symposium.Models.Models.Products;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class PageButtonFlows : IPageButtonFlows
    {
        IPageButtonTasks pageButtonTasks;
        public PageButtonFlows(IPageButtonTasks pbutton)
        {
            this.pageButtonTasks = pbutton;
        }

        /// <summary>
        /// Return page buttons for a specific POS, PageId and Price-list
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <param name="pageid"></param>
        /// <param name="pricelistid"></param>
        public PageButtonPreviewModel GetPageButtons(DBInfoModel dbInfo, string storeid, int posid, int pageid, int pricelistid, bool isPos = false)
        {
            // get the results
            PageButtonPreviewModel getpageButtons = pageButtonTasks.GetPageButtons(dbInfo, storeid, posid, pageid, pricelistid, isPos);

            return getpageButtons;
        }

        public List<PageButtonPricelistDetailsAssoc> SearchExternalProduct(DBInfoModel DBInfo, string description)
        {
            return pageButtonTasks.SearchExternalProduct(DBInfo, description);
        }

        /// <summary>
        /// Return's list of Page Buttons after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<PageButtonSched_Model> model)
        {
            return pageButtonTasks.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, PageButtonModel item)
        {
            return pageButtonTasks.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<PageButtonModel> item)
        {
            return pageButtonTasks.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, PageButtonModel item)
        {
            return pageButtonTasks.InsertModel(dbInfo, item);
        }
    }
}
