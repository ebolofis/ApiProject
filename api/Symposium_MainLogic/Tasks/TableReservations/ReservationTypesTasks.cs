using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.TableReservations
{
    public class ReservationTypesTasks : IReservationTypesTasks
    {
        IReservationTypesDT reservationTypesDT;

        public ReservationTypesTasks(IReservationTypesDT _reservationTypesDT)
        {
            this.reservationTypesDT = _reservationTypesDT;
        }

        /// <summary>
        /// Get all reservation types from db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public List<ReservationTypeModel> GetAllReservationTypes(DBInfoModel dbInfo)
        {
            return reservationTypesDT.GetAllReservationTypes(dbInfo);
        }
    }
}
