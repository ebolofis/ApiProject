﻿using Symposium.Models.Models;
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
    public class TaxFlows : ITaxFlows
    {
        ITaxTasks Task;

        public TaxFlows(ITaxTasks Task)
        {
            this.Task = Task;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<TaxSched_Model> model)
        {
            return Task.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, TaxModel item)
        {
            return Task.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<TaxModel> item)
        {
            return Task.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, TaxModel item)
        {
            return Task.InsertModel(dbInfo, item);
        }
    }
}
