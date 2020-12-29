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
    public class OverwrittenCapacitiesDT : IOverwrittenCapacitiesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public OverwrittenCapacitiesDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the List of Overwritten Capacities
        /// </summary>
        /// <returns></returns>
        public OverwrittenCapacitiesListModel GetOverwrittenCapacities(DBInfoModel Store)
        {
            // get the results
            OverwrittenCapacitiesListModel overwrittenCapacitiesList = new OverwrittenCapacitiesListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<OverwrittenCapacitiesModel> overDetails = db.Query<OverwrittenCapacitiesModel>("SELECT * FROM TR_OvewrittenCapacities AS toc").ToList();
                overwrittenCapacitiesList.OverwrittenCapacitiesModelList = overDetails;
            }

            return overwrittenCapacitiesList;
        }

        /// <summary>
        /// Returns details for a specific Overwritten Capacity 
        /// </summary>
        /// <param name="Id">OverwrittenCapacityID</param>
        /// <returns></returns>
        public OverwrittenCapacitiesModel GetOverwrittenCapacityById(DBInfoModel Store, long Id)
        {
            // get the results
            OverwrittenCapacitiesModel overwrittenCapacitiesDetails = new OverwrittenCapacitiesModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                OverwrittenCapacitiesModel overDetails = db.Query<OverwrittenCapacitiesModel>("SELECT * FROM TR_OvewrittenCapacities AS toc WHERE toc.Id =@ID", new { ID = Id }).FirstOrDefault();
                overwrittenCapacitiesDetails = overDetails;
            }

            return overwrittenCapacitiesDetails;
        }

        /// <summary>
        /// Insert new OverwrittenCapacity
        /// </summary>
        /// <returns></returns>
        public long insertOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO TR_OvewrittenCapacities(RestId, CapacityId, Capacity, [Date]) VALUES ( @RestId, @CapacityId, @Capacity , @Date )";

                db.Query(insertQuery, new
                {
                    RestId = model.RestId,
                    CapacityId = model.CapacityId,
                    Capacity = model.Capacity,
                    Date = model.Date
                });

                res = db.Query<long>("SELECT toc.Id FROM TR_OvewrittenCapacities AS toc ORDER BY toc.id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update an OverwrittenCapacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public OverwrittenCapacitiesModel UpdateOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = "UPDATE TR_OvewrittenCapacities SET RestId=@RestId, CapacityId=@CapacityId, Capacity=@Capacity , [Date]=@Date WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    RestId = Model.RestId,
                    CapacityId = Model.CapacityId,
                    Capacity = Model.Capacity,
                    Date = Model.Date
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete an OverwrittenCapacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteOverwrittenCapacity(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_OvewrittenCapacities WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }
    }
}
