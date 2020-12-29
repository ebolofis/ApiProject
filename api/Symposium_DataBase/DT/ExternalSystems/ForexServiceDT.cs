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
    public class ForexServiceDT : IForexServiceDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<ForexServiceDTO> genForexService;

        public ForexServiceDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<ForexServiceDTO> genForexService)
        {
            this.usersToDatabases = usersToDatabases;
            this.genForexService = genForexService;
        }

        public List<ForexServiceModel> SelectAllForex(DBInfoModel store)
        {
            List<ForexServiceDTO> forexList;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                forexList = genForexService.Select(db, "ForexService");
            }

            return AutoMapper.Mapper.Map<List<ForexServiceModel>>(forexList);
        }
    }
}
