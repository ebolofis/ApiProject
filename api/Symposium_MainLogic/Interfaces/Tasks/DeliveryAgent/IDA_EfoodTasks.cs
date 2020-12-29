using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_EfoodTasks
    {
        /// <summary>
        ///  Get the list of orders from external delivery web-apis and convert to DA_ExtDeliveryModel.
        /// </summary>
        /// <returns>the  list of orders </returns>
        List<DA_ExtDeliveryModel> ConvertEfoodModelToDaDeliveryModel(DBInfoModel dbInfo, DA_EfoodModel Model);
    }
}
