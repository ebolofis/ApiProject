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
    public class ExcludeDaysDT : IExcludeDaysDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public ExcludeDaysDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the List of ExcludeDays
        /// </summary>
        /// <returns></returns>
        public ExcludeDaysListModel GetExcludeDays(DBInfoModel Store)
        {
            // get the results
            ExcludeDaysListModel excludeDaysList = new ExcludeDaysListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<ExcludeDaysModel> exDaysDetails = db.Query<ExcludeDaysModel>(" SELECT * FROM TR_ExcludeDays AS ted").ToList();
                excludeDaysList.ExcludeDaysModelList = exDaysDetails;
            }

            return excludeDaysList;
        }

        /// <summary>
        /// Returns details for a specific ExcludeDay 
        /// </summary>
        /// <param name="Id">ExcludeDayID</param>
        /// <returns></returns>
        public ExcludeDaysModel GetExcludeDayById(DBInfoModel Store, long Id)
        {
            // get the results
            ExcludeDaysModel excludeDayDetails = new ExcludeDaysModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                ExcludeDaysModel exDayDetails = db.Query<ExcludeDaysModel>("SELECT * FROM TR_ExcludeDays AS ted WHERE ted.Id =@ID", new { ID = Id }).FirstOrDefault();
                excludeDayDetails = exDayDetails;
            }

            return excludeDayDetails;
        }

        /// <summary>
        /// Insert new ExcludeDay
        /// </summary>
        /// <returns></returns>
        public long insertExcludeDay(DBInfoModel Store, ExcludeDaysModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO TR_ExcludeDays(RestId, [Date], [Type]) VALUES ( @RestId, @Date, @Type )";

                db.Query(insertQuery, new
                {
                    RestId = model.RestId,
                    Date = model.Date,
                    Type = model.Type
                });

                res = db.Query<long>("SELECT ted.Id FROM TR_ExcludeDays AS ted ORDER BY ted.id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update a ExcludeDay
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ExcludeDaysModel UpdateExcludeDay(DBInfoModel Store, ExcludeDaysModel Model)
        {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string updateQuery = "UPDATE TR_ExcludeDays SET RestId=@RestId, [Date]=@Date, [Type]=@Type WHERE Id=@ID";

                    db.Query(updateQuery, new
                    {
                        ID = Model.Id,
                        RestId = Model.RestId,
                        Date = Model.Date,
                        Type = Model.Type
                    });

                    return Model;
                }
            }

        /// <summary>
        /// Delete a ExcludeDay
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteExcludeDay(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_ExcludeDays WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }

        /// <summary>
        /// Deleting old Exclude Days from DB
        /// </summary>
        /// <returns></returns>
        public bool DeleteOldExcludeDays(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        string deleteExcludeDaysQuery = @"DELETE FROM TR_ExcludeDays WHERE [Date] < GETDATE()";
                        db.Query(deleteExcludeDaysQuery);
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
