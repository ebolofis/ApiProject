using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations
{
    public interface IReservationTypesTasks
    {
        /// <summary>
        /// Get all reservation types from db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        List<ReservationTypeModel> GetAllReservationTypes(DBInfoModel dbInfo);
    }
}
