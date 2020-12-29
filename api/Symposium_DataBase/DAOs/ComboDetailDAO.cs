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
    public class ComboDetailDAO : IComboDetailDAO
    {
        IGenericDAO<ComboDetailDTO> genComboDetail;

        public ComboDetailDAO(IGenericDAO<ComboDetailDTO> genComboDetail)
        {
            this.genComboDetail = genComboDetail;
        }

        /// <summary>
        /// Selects combo details for selected combo
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="comboId"> Id of combo </param>
        /// <returns> List of combo details </returns>
        public List<ComboDetailDTO> selectComboDetailsForSelectedCombo(IDbConnection db, long comboId)
        {
            return genComboDetail.Select(db, "where ComboId = @comb", new { comb = comboId });
        }

    }
}
