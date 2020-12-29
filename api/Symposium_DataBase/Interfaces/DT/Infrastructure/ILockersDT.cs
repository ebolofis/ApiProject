using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    /// <summary>
    /// handle data related to End Of Day Procedure
    /// </summary>
    public interface ILockersDT
    {

        /// <summary>
        /// Selects lockers for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> Lockers model. See: <seealso cref="Symposium.Models.Models.LockersModel"/> </returns>
        LockersModel GetLockers(DBInfoModel Store, long posInfoId, long? endOfDayId);
    }
}
