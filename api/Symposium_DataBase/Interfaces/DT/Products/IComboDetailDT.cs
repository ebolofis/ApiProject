using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IComboDetailDT
    {

        /// <summary>
        /// Selects combo details for selected combo
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="comboId"> Id of combo </param>
        /// <returns> List of combo detail models See: <seealso cref="Symposium.Models.Models.ComboDetailModel"</returns>
        List<ComboDetailModel> selectComboDetailsForSelectedCombo(DBInfoModel Store, long comboId);

    }
}
