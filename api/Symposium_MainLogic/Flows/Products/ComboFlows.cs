using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class ComboFlows : IComboFlows
    {
        IComboTasks comboTasks;

        public ComboFlows(IComboTasks comboTasks)
        {
            this.comboTasks = comboTasks;
        }

        /// <summary>
        /// Selects active combo items
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <returns> List of combo models See: <seealso cref="Symposium.Models.Models.ComboModel"</returns>
        public List<ComboModel> selectActiveComboItems(DBInfoModel dbInfo, long departmentId)
        {
            List<ComboModel> comboItems = comboTasks.selectActiveComboItems(dbInfo, departmentId);
            return comboItems;
        }

    }
}
