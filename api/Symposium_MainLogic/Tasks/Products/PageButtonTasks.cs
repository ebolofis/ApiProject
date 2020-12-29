using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Products;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class PageButtonTasks : IPageButtonTasks
    {
        IPageButtonDT pageButtonDT;
        public PageButtonTasks(IPageButtonDT pButtonDT)
        {
            this.pageButtonDT = pButtonDT;
        }

        /// <summary>
        /// Return page buttons for a specific Pos, PageId and Pricelist
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <param name="pageid"></param>
        /// <param name="pricelistid"></param>
        public PageButtonPreviewModel GetPageButtons(DBInfoModel Store, string storeid, int posid, int pageid, int pricelistid, bool isPos = false)
        {
            // get the results
            PageButtonPreviewModel getpageButtons = pageButtonDT.GetPageButtons(Store, storeid, posid, pageid, pricelistid, isPos);

            return getpageButtons;
        }
        public List<PageButtonPricelistDetailsAssoc> SearchExternalProduct(DBInfoModel DBInfo, string description)
        {
            return pageButtonDT.SearchExternalProduct(DBInfo, description);
        }
        /// <summary>
        /// Return's list of Page Buttons after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PageButtonSched_Model> model)
        {
            List<PageButtonSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<PageButtonSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = pageButtonDT.InformTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = pageButtonDT.DeleteRecordsSendedFromDAServer(Store, Deleted);

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
        public int UpdateModel(DBInfoModel Store, PageButtonModel item)
        {
            return pageButtonDT.UpdateModel(Store, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PageButtonModel> item)
        {
            return pageButtonDT.UpdateModelList(Store, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PageButtonModel item)
        {
            return pageButtonDT.InsertModel(Store, item);
        }
    }
}
