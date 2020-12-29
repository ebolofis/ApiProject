using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_ConfigDT
    {
        /// <summary>
        /// Get PosId
        /// </summary>
        /// <returns></returns>
        long GetPosId(DBInfoModel Store);
    }
}
