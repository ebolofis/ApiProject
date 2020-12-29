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
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.TableReservations
{
    public class ExcludeRestrictionsDT : IExcludeRestrictionsDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public ExcludeRestrictionsDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the List of Exclude Restrictions
        /// </summary>
        /// <returns></returns>
        public ExcludeRestrictionsListModel GetExcludeRestrictions(DBInfoModel Store)
        {
            // get the results
            ExcludeRestrictionsListModel exRestrictionsList = new ExcludeRestrictionsListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<ExcludeRestrictionsModel> exRestrDetails = db.Query<ExcludeRestrictionsModel>("SELECT * FROM TR_ExcludeRestrictions AS ter").ToList();
                exRestrictionsList.ExcludeRestrictionsList = exRestrDetails;
            }

            return exRestrictionsList;
        }

        /// <summary>
        /// Returns details for a specific ExcludeRestriction
        /// </summary>
        /// <param name="Id">ExcludeRestrictionID</param>
        /// <returns></returns>
        public ExcludeRestrictionsModel GetExcludeRestrictionById(DBInfoModel Store, long Id)
        {
            // get the results
            ExcludeRestrictionsModel excludeRestrictionsDetails = new ExcludeRestrictionsModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                ExcludeRestrictionsModel exRestrictionsDetails = db.Query<ExcludeRestrictionsModel>("SELECT * FROM TR_ExcludeRestrictions AS ter WHERE ter.Id =@ID", new { ID = Id }).FirstOrDefault();
                excludeRestrictionsDetails = exRestrictionsDetails;
            }

            return excludeRestrictionsDetails;
        }

        /// <summary>
        /// Insert new ExcludeRestriction
        /// </summary>
        /// <returns></returns>
        public long insertExcludeRestriction(DBInfoModel Store, ExcludeRestrictionsModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO TR_ExcludeRestrictions(RestRestrictAssocId, RestId, [Date]) VALUES ( @RestRestrictAssocId, @RestId, @Date )";

                db.Query(insertQuery, new
                {
                    RestRestrictAssocId = model.RestRestrictAssocId,
                    RestId = model.RestId,
                    Date = model.Date
                });

                res = db.Query<long>("SELECT ter.Id FROM TR_ExcludeRestrictions AS ter ORDER BY ter.id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update an ExcludeRestriction
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ExcludeRestrictionsModel UpdateExcludeRestriction(DBInfoModel Store, ExcludeRestrictionsModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = "UPDATE TR_ExcludeRestrictions SET RestRestrictAssocId=@RestRestrictAssocId, RestId=@RestId, [Date]=@Date WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    RestRestrictAssocId = Model.RestRestrictAssocId,
                    RestId = Model.RestId,
                    Date = Model.Date
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete an ExcludeRestriction
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteExcludeRestriction(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_ExcludeRestrictions WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }

        /// <summary>
        /// Deleting old Exclude Restrictions from DB
        /// </summary>
        /// <returns></returns>
        public bool DeleteOldExcludeRestrictions(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        string deleteExcludeRestrictionsQuery = @"DELETE FROM TR_ExcludeRestrictions WHERE [Date] < GETDATE()";
                        db.Query(deleteExcludeRestrictionsQuery);
                    }
                    catch
                    {
                        throw new Exception("Old Exclude Restrictions can not Deleted...!");
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }
            return true;
        }

    }
}
