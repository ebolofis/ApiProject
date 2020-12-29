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
    public class RestrictionsDT : IRestrictionDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public RestrictionsDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the List of Restrictions
        /// </summary>
        /// <returns></returns>
        public RestrictionsListModel GetRestrictions(DBInfoModel Store)
        {
            // get the results
            RestrictionsListModel restrictionsList = new RestrictionsListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<RestrictionsModel> restDetails = db.Query<RestrictionsModel>("SELECT * FROM TR_Restrictions AS tr").ToList();
                restrictionsList.RestrictionsModelList = restDetails;
            }

            return restrictionsList;
        }

        /// <summary>
        /// Returns details for a specific Restriction 
        /// </summary>
        /// <param name="Id">RestrictionID</param>
        /// <returns></returns>
        public RestrictionsModel GetRestrictionById(DBInfoModel Store, long Id)
        {
            // get the results
            RestrictionsModel restrictionDetails = new RestrictionsModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                RestrictionsModel restictDetails = db.Query<RestrictionsModel>("SELECT * FROM TR_Restrictions AS tr WHERE tr.Id =@ID", new { ID = Id }).FirstOrDefault();
                restrictionDetails = restictDetails;
            }
            return restrictionDetails;
        }

        /// <summary>
        /// Insert new Restriction
        /// </summary>
        /// <returns></returns>
        public long insertRestriction(DBInfoModel Store, RestrictionsModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO TR_Restrictions(Id, [Description]) VALUES ( @Id, @Description )";

                db.Query(insertQuery, new
                {
                    Id = model.Id,
                    Description = model.Description
                });

                res = db.Query<long>("SELECT tr.Id FROM TR_Restrictions AS tr ORDER BY tr.id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update a Restriction
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestrictionsModel UpdateRestriction(DBInfoModel Store, RestrictionsModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"UPDATE TR_Restrictions SET Id=@Id, [Description]=@Description WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    Description = Model.Description
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete a Restriction
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestriction(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_Restrictions WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }
    }
}
