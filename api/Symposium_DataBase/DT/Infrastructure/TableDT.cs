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
    public class TableDT : ITableDT
    {
        IGenericDAO<TableDTO> dt;
        IUsersToDatabasesXML user;
        string connectionString;

        public TableDT(IGenericDAO<TableDTO> dt, IUsersToDatabasesXML user)
        {
            this.dt = dt;
            this.user = user;
        }

        /// <summary>
        /// Returns A Table Model By Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TablesModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            if (dbTran != null)
                return AutoMapper.Mapper.Map<TablesModel>(dt.SelectFirst(dbTran, "WHERE Id = @Id", new { Id = Id }, dbTransact));
            else
            {
                connectionString = user.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return AutoMapper.Mapper.Map<TablesModel>(dt.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }));
                }
            }
        }
    }
}
