using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.TableReservations
{
    public class ReservationTypesFlows : IReservationTypesFlows
    {
        IReservationTypesTasks reservationTypesTasks;

        public ReservationTypesFlows(IReservationTypesTasks _reservationTypesTasks)
        {
            this.reservationTypesTasks = _reservationTypesTasks;
        }

        /// <summary>
        /// Get all reservation types from db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public List<ReservationTypeModel> GetAllReservationTypes(DBInfoModel dbInfo)
        {
            return reservationTypesTasks.GetAllReservationTypes(dbInfo);
        }
    }
}
