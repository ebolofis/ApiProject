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
    public class RestrictionsRestaurantsAssocDT : IRestrictionsRestaurantsAssocDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public RestrictionsRestaurantsAssocDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the List of Restrictions Restaurants Associations
        /// </summary>
        /// <returns></returns>
        public RestrictionsRestaurantsAssocListModel GetRestrictionsRestaurantsAssoc(DBInfoModel Store)
        {
            // get the results
            RestrictionsRestaurantsAssocListModel restrictionsRestaurantsAssocList = new RestrictionsRestaurantsAssocListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<RestrictionsRestaurantsAssocModel> assocDetails = db.Query<RestrictionsRestaurantsAssocModel>("SELECT * FROM TR_Restrictions_Restaurants_Assoc AS trra").ToList();
                restrictionsRestaurantsAssocList.RestrictionsRestaurantsAssocList = assocDetails;
            }

            return restrictionsRestaurantsAssocList;
        }

        /// <summary>
        /// Gets a specific Restrictions_Restaurants_Assoc by Id
        /// </summary>
        /// <param name="Id">RestrictionsRestaurantsAssocID</param>
        /// <returns></returns>
        public RestrictionsRestaurantsAssocModel GetRestrictionsRestaurantsAssocById(DBInfoModel Store, long Id)
        {
            // get the results
            RestrictionsRestaurantsAssocModel restrictionsRestaurantsAssocDetails = new RestrictionsRestaurantsAssocModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                RestrictionsRestaurantsAssocModel restDetails = db.Query<RestrictionsRestaurantsAssocModel>("SELECT * FROM TR_Restrictions_Restaurants_Assoc AS trra WHERE trra.Id =@ID", new { ID = Id }).FirstOrDefault();
                restrictionsRestaurantsAssocDetails = restDetails;
            }

            return restrictionsRestaurantsAssocDetails;
        }

        /// <summary>
        /// Insert new Restrictions_Restaurants_Assoc
        /// </summary>
        /// <returns></returns>
        public long insertRestrictionsRestaurantsAssoc(DBInfoModel Store, RestrictionsRestaurantsAssocModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO TR_Restrictions_Restaurants_Assoc(RestrictId, RestId, N)
                                       VALUES ( @RestrictId, @RestId, @N)";

                db.Query(insertQuery, new
                {
                    RestrictId = model.RestrictId,
                    RestId = model.RestId,
                    N = model.N
                });

                res = db.Query<long>("SELECT trra.Id FROM TR_Restrictions_Restaurants_Assoc AS trra ORDER BY trra.Id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update a Restrictions_Restaurants_Assoc
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestrictionsRestaurantsAssocModel UpdateRestrictionsRestaurantsAssoc(DBInfoModel Store, RestrictionsRestaurantsAssocModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = "UPDATE TR_Restrictions_Restaurants_Assoc SET RestrictId=@RestrictId, RestId=@RestId, N=@N WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    RestrictId = Model.RestrictId,
                    RestId = Model.RestId,
                    N = Model.N
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete a Restrictions_Restaurants_Assoc
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestrictionsRestaurantsAssoc(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_Restrictions_Restaurants_Assoc WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }
    }
}
