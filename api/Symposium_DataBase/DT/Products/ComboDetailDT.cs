using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class ComboDetailDT : IComboDetailDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IComboDetailDAO genComboDetailDAO;

        public ComboDetailDT(IUsersToDatabasesXML usersToDatabases, IComboDetailDAO genComboDetailDAO)
        {
            this.usersToDatabases = usersToDatabases;
            this.genComboDetailDAO = genComboDetailDAO;
        }

        /// <summary>
        /// Selects combo details for selected combo
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="comboId"> Id of combo </param>
        /// <returns> List of combo detail models See: <seealso cref="Symposium.Models.Models.ComboDetailModel"</returns>
        public List<ComboDetailModel> selectComboDetailsForSelectedCombo (DBInfoModel Store, long comboId)
        {
            List<ComboDetailDTO> comboDetails;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                comboDetails = genComboDetailDAO.selectComboDetailsForSelectedCombo(db, comboId);
            }
            return AutoMapper.Mapper.Map<List<ComboDetailModel>>(comboDetails);
        }

    }
}
