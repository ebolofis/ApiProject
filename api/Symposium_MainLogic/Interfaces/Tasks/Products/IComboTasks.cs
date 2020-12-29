using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IComboTasks
    {

        /// <summary>
        /// Selects active combo items
        /// </summary>
        /// <param name="store"> Store </param>
        /// <returns> List of combo models See: <seealso cref="Symposium.Models.Models.ComboModel"</returns>
        List<ComboModel> selectActiveComboItems(DBInfoModel store, long departmentId);

    }
}
