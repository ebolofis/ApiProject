using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IStoreMessagesTasks
    {
        /// <summary>
        /// Get Store Messages
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="filtered"></param>
        /// <returns></returns>
        StoreMessagesModelsPreview GetStoreMessages(DBInfoModel Store, string storeid, bool filtered);
    }
}
