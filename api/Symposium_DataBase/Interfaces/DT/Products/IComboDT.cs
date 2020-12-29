using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IComboDT
    {

        /// <summary>
        /// Selects all active combos
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <returns> List of combo models See: <seealso cref="Symposium.Models.Models.ComboModel"</returns>
        List<ComboModel> selectAllActiveCombos(DBInfoModel store, long departmentId);

    }
}
