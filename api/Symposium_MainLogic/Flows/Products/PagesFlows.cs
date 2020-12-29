using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class PagesFlows : IPagesFlows
    {
        IPagesTasks pagesTasks;
        public PagesFlows(IPagesTasks pages)
        {
            this.pagesTasks = pages;
        }

        /// <summary>
        /// Return all active pages (pageSets and pageButtons are NOT included) for a specific POS for the current date ordered by Pages.Sort
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <returns></returns>
        public PagesModelsPreview GetPagesForPosId(DBInfoModel dbInfo, string storeid, long posid)
        {
            // get the results
            PagesModelsPreview getPages = pagesTasks.GetPagesForPosId(dbInfo, storeid, posid);

            return getPages;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<PagesSched_Model> model)
        {
            return pagesTasks.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, PagesModel item)
        {
            return pagesTasks.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<PagesModel> item)
        {
            return pagesTasks.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, PagesModel item)
        {
            return pagesTasks.InsertModel(dbInfo, item);
        }
    }
}
