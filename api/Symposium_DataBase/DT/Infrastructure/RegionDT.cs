using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class RegionDT : IRegionDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public RegionDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return regions based on posinfo.Id
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="notables">not used</param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public RegionModelsPreview GetRegions(DBInfoModel Store, string storeid, bool notables, long posInfoId)
        {
            /*
            // get the results
            RegionModelsPreview getRegionsModel = new RegionModelsPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string regionQuery = "SELECT r.* \n"
                                   + "FROM PosInfo_Region_Assoc AS pira  \n"
                                   + "LEFT OUTER JOIN Region AS r ON r.Id = pira.RegionId \n"
                                   + "WHERE pira.PosInfoId =@pId";

                string tableQuery = "SELECT * FROM [Table] AS t WHERE t.RegionId =@rId";


                string posInfo_Region_AssocQuery = "SELECT * FROM PosInfo_Region_Assoc AS pira WHERE pira.RegionId =@rId";

                List<RegionModel> getRegions = db.Query<RegionModel>(regionQuery, new { pId = posInfoId }).ToList();
                foreach(RegionModel rd in getRegions)
                {
                    List<TablesModel> tableModel = db.Query<TablesModel>(tableQuery, new { rId = rd.Id }).ToList();
                    rd.Table = tableModel;

                    List<PosInfo_Region_AssocModels> posInfo_Region_AssocModel = db.Query<PosInfo_Region_AssocModels>(posInfo_Region_AssocQuery, new { rId = rd.Id }).ToList();
                    rd.PosInfo_Region_Assoc = posInfo_Region_AssocModel;
                }
                getRegionsModel.RegionModel = getRegions;
            }

            return getRegionsModel;
            */
            
            RegionModelsPreview getRegionsModel = new RegionModelsPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string regionQuery = "SELECT r.* \n"
                                   + "FROM PosInfo_Region_Assoc AS pira  \n"
                                   + "LEFT OUTER JOIN Region AS r ON r.Id = pira.RegionId \n"
                                   + "WHERE pira.PosInfoId =@pId";

                List<RegionModel> getRegions = db.Query<RegionModel>(regionQuery, new { pId = posInfoId }).ToList();

                List<TablesModel> tableModel = new List<TablesModel>();
                List<PosInfo_Region_AssocModels> PosInfo_Region_Assoc_model = new List<PosInfo_Region_AssocModels>();

                foreach (RegionModel rd in getRegions)
                {
                    rd.Table = tableModel;

                    rd.PosInfo_Region_Assoc = PosInfo_Region_Assoc_model;
                }
                getRegionsModel.RegionModel = getRegions;
            }
            
            return getRegionsModel;
        }
    }
}
