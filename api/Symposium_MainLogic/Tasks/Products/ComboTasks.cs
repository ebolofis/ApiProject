using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class ComboTasks : IComboTasks
    {
        IComboDT comboDT;
        IComboDetailDT comboDetailDT;

        public ComboTasks(IComboDT comboDT, IComboDetailDT comboDetailDT)
        {
            this.comboDT = comboDT;
            this.comboDetailDT = comboDetailDT;
        }

        /// <summary>
        /// Selects active combo items
        /// </summary>
        /// <param name="store"> Store </param>
        /// <returns> List of combo models See: <seealso cref="Symposium.Models.Models.ComboModel"</returns>
        public List<ComboModel> selectActiveComboItems(DBInfoModel store, long departmentId)
        {
            List<ComboModel> comboItems;
            comboItems = comboDT.selectAllActiveCombos(store, departmentId);
            foreach(ComboModel c in comboItems)
            {
                c.ComboDetails = comboDetailDT.selectComboDetailsForSelectedCombo(store, c.Id);
                c.ComboDetailsLength = c.ComboDetails.Count();
            }
            return comboItems;
        }

    }
}
