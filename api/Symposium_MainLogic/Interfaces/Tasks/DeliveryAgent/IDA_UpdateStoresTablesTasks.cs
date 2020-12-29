using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_UpdateStoresTablesTasks
    {
        /// <summary>
        /// Updates store tables with data from server 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DelAfterFaild"></param>
        /// <param name="ClientId"></param>
        void UpdateTables(DBInfoModel Store, int DelAfterFaild, long? ClientId);


    }
}
