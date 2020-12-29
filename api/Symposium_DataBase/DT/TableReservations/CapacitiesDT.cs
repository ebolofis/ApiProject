using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.TableReservations
{
    public class CapacitiesDT : ICapacitiesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public CapacitiesDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the List of Capacities
        /// </summary>
        /// <returns></returns>
        public CapacitiesListModel GetCapacities(DBInfoModel Store)
        {
            // get the results
            CapacitiesListModel capacitiesList = new CapacitiesListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<CapacitiesModel> capDetails = db.Query<CapacitiesModel>("SELECT * FROM TR_Capacities AS tc").ToList();
                capacitiesList.CapacitiesModelList = capDetails;
            }

            return capacitiesList;
        }

        /// <summary>
        /// Returns the List of Capacities by Restaurant Id 
        /// </summary>
        /// <returns></returns>
        public CapacitiesListModel GetCapacitiesByRestId(DBInfoModel Store, long RestId)
        {
            // get the results
            CapacitiesListModel capacitiesList = new CapacitiesListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<CapacitiesModel> capDetails = db.Query<CapacitiesModel>("SELECT * FROM TR_Capacities AS tc WHERE tc.RestId = @RestID" , new { RestID = RestId }).ToList();
                capacitiesList.CapacitiesModelList = capDetails;
            }

            return capacitiesList;
        }

        /// <summary>
        /// Returns details for a specific Capacity 
        /// </summary>
        /// <param name="Id">CapacityID</param>
        /// <returns></returns>
        public CapacitiesModel GetCapacityById(DBInfoModel Store, long Id)
        {
            // get the results
            CapacitiesModel capacityDetails = new CapacitiesModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                CapacitiesModel capDetails = db.Query<CapacitiesModel>("SELECT * FROM TR_Capacities AS tc WHERE tc.Id =@ID", new { ID = Id }).FirstOrDefault();
                capacityDetails = capDetails;
            }

            return capacityDetails;
        }

        /// <summary>
        /// Insert new Capacity
        /// </summary>
        /// <returns></returns>
        public long insertCapacity(DBInfoModel Store, CapacitiesModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO TR_Capacities(RestId, Capacity, [Type], [Time]) VALUES ( @RestId, @Capacity, @Type , @Time )";

                db.Query(insertQuery, new
                {
                    RestId = model.RestId,
                    Capacity = model.Capacity,
                    Type = model.Type,
                    Time = model.Time
                });

                res = db.Query<long>("SELECT tc.Id FROM TR_Capacities AS tc ORDER BY tc.id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update a Capacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public CapacitiesModel UpdateCapacity(DBInfoModel Store, CapacitiesModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = "UPDATE TR_Capacities SET RestId=@RestId, Capacity=@Capacity, [Type]=@Type, [Time]=@Time WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    RestId = Model.RestId,
                    Capacity = Model.Capacity,
                    Type = Model.Type,
                    Time = Model.Time
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete a Capacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteCapacity(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_Capacities WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }
    }
}
