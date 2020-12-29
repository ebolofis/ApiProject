using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IComboDAO
    {

        /// <summary>
        /// Selects all active combos
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <returns> List of combos </returns>
        List<ComboDTO> selectAllActiveCombo(IDbConnection db, long departmentId);

    }
}
