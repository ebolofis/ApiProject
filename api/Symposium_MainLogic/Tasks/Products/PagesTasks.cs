using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class PagesTasks : IPagesTasks
    {
        IPagesDT pagesDT;
        public PagesTasks(IPagesDT pages)
        {
            this.pagesDT = pages;
        }

        /// <summary>
        /// Return all active pages (pageSets and pageButtons are NOT included) for a specific POS for the current date ordered by Pages.Sort
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <returns></returns>
        public PagesModelsPreview GetPagesForPosId(DBInfoModel Store, string storeid, long posid)
        {
            // get the results
            PagesModelsPreview getPages = pagesDT.GetPagesForPosId(Store, storeid, posid);

            return getPages;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PagesSched_Model> model)
        {
            List<PagesSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<PagesSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = pagesDT.InformTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = pagesDT.DeleteRecordsSendedFromDAServer(Store, Deleted);

            ups.TotalDeleted += del.TotalDeleted;
            ups.TotalFailed += del.TotalFailed;
            ups.TotalInserted += del.TotalInserted;
            ups.TotalRecords += del.TotalRecords;
            ups.TotalSucceded += del.TotalSucceded;
            ups.TotalUpdated += del.TotalUpdated;
            ups.TotalUpdated += del.TotalUpdated;
            if (ups.Results != null && ups.Results.Count > 0)
                ups.Results.Union(del.Results);
            else
                ups.Results.AddRange(del.Results);

            return ups;
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, PagesModel item)
        {
            return pagesDT.UpdateModel(Store, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PagesModel> item)
        {
            return pagesDT.UpdateModelList(Store, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PagesModel item)
        {
            return pagesDT.InsertModel(Store, item);
        }
    }
}
