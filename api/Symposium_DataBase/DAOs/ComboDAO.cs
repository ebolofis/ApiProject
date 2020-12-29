using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class ComboDAO : IComboDAO
    {
        IGenericDAO<ComboDTO> genCombo;

        public ComboDAO(IGenericDAO<ComboDTO> genCombo)
        {
            this.genCombo = genCombo;
        }

        /// <summary>
        /// Selects all active combos
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <returns> List of combos </returns>
        public List<ComboDTO> selectAllActiveCombo(IDbConnection db, long departmentId)
        {
            return genCombo.Select(db, "where StartDate <= GETDATE() and EndDate >= GETDATE() and DepartmentId = @dep", new { dep = departmentId });
        }

    }
}
