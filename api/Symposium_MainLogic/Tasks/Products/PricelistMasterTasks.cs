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
    public class PricelistMasterTasks : IPricelistMasterTasks
    {
        IPricelistMasterDT dt;

        public PricelistMasterTasks(IPricelistMasterDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PriceListMasterSched_Model> model)
        {
            List<PriceListMasterSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<PriceListMasterSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = dt.InformTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = dt.DeleteRecordsSendedFromDAServer(Store, Deleted);

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
        public int UpdateModel(DBInfoModel Store, PricelistMasterModel item)
        {
            return dt.UpdateModel(Store, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PricelistMasterModel> item)
        {
            return dt.UpdateModelList(Store, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PricelistMasterModel item)
        {
            return dt.InsertModel(Store, item);
        }
    }
}
