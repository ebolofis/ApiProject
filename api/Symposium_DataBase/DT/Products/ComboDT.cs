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
    public class ComboDT : IComboDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IComboDAO genComboDAO;

        public ComboDT(IUsersToDatabasesXML usersToDatabases, IComboDAO genComboDAO)
        {
            this.usersToDatabases = usersToDatabases;
            this.genComboDAO = genComboDAO;
        }

        /// <summary>
        /// Selects all active combos
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <returns> List of combo models See: <seealso cref="Symposium.Models.Models.ComboModel"</returns>
        public List<ComboModel> selectAllActiveCombos(DBInfoModel store, long departmentId)
        {
            List<ComboDTO> combos;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                combos = genComboDAO.selectAllActiveCombo(db, departmentId);
            }
            return AutoMapper.Mapper.Map<List<ComboModel>>(combos);
        }

    }
}
