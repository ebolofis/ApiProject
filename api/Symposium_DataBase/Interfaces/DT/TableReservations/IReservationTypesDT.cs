using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations
{
    public interface IReservationTypesDT
    {
        /// <summary>
        /// Get all reservation types from db
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<ReservationTypeModel> GetAllReservationTypes(DBInfoModel dbInfo);
    }
}
