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
    public class DepartmentDT : IDepartmentDT
    {
        IGenericDAO<DepartmentDTO> dt;
        IUsersToDatabasesXML users;
        string connectionString;  

        public DepartmentDT(IGenericDAO<DepartmentDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Return's Department Description
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        public string GetDepartmentDescr(DBInfoModel Store, long DepartmentId)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DepartmentDTO dto = dt.SelectFirst(db, "WHERE Id = @Id", new { Id = DepartmentId });
                return dto.Description;
            }
        }
    }
}
