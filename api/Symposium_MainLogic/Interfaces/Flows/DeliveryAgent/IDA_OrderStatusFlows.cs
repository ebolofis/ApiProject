using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_OrderStatusFlows
    {
        /// <summary>
        /// Insert's a New Model To DB
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewModel(DBInfoModel dbInfo, DA_OrderStatusModel item);


        /// <summary>
        /// Insert's a list of DA_OrderStatus and return's Succeded and not
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        List<ResultsAfterDA_OrderActionsModel> AddNewList(DBInfoModel dbInfo, List<DA_OrderStatusModel> model);

        /// <summary>
        /// Inserts order status for dine in orders from invoice id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="invoiceId"></param>
        void InsertOrderStatusFromInvoiceId(DBInfoModel dbInfo, long invoiceId);
    }
}
