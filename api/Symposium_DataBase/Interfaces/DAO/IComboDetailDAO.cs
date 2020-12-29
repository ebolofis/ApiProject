using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IComboDetailDAO
    {

        /// <summary>
        /// Selects combo details for selected combo
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="comboId"> Id of combo </param>
        /// <returns> List of combo details </returns>
        List<ComboDetailDTO> selectComboDetailsForSelectedCombo(IDbConnection db, long comboId);

    }
}
