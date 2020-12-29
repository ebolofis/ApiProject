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
    public class RestaurantsDT : IRestaurantsDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public RestaurantsDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns Trading Hours
        /// </summary>
        /// <returns></returns>
        public TradingHoursModel GetTradingHours(DBInfoModel Store)
        {
            TradingHoursModel tradingHours = new TradingHoursModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                tradingHours = db.Query<TradingHoursModel>("SELECT * FROM TR_TradingHours").FirstOrDefault();
            }

            return tradingHours;
        }

        /// <summary>
        /// Update Trading Hours
        /// </summary>
        /// <returns></returns>
        public TradingHoursModel UpdateTradingHours(DBInfoModel Store, TradingHoursModel model)
        {
            TradingHoursModel tradingHours = new TradingHoursModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute("UPDATE TR_TradingHours SET TimeFrom =@start , TimeTo =@end", new { start = model.TimeFrom, end = model.TimeTo });
                tradingHours = db.Query<TradingHoursModel>("SELECT * FROM TR_TradingHours").FirstOrDefault();
            }

            return tradingHours;
        }

        /// <summary>
        /// Returns the List of Restaurants
        /// </summary>
        /// <returns></returns>
        public RestaurantsListModel GetRestaurants(DBInfoModel Store)
        {
            // get the results
            RestaurantsListModel restaurantsList = new RestaurantsListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<RestaurantsModel> restDetails = db.Query<RestaurantsModel>("SELECT * FROM TR_Restaurants AS tr").ToList();
                restaurantsList.RestaurantsList = restDetails;
            }

            return restaurantsList;
        }

        /// <summary>
        /// Returns details for a specific Restaurant 
        /// </summary>
        /// <param name="Id">RestaurantID</param>
        /// <returns></returns>
        public RestaurantsModel GetRestaurantById(DBInfoModel Store, long Id)
        {
            // get the results
            RestaurantsModel restaurantDetails = new RestaurantsModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                RestaurantsModel restDetails = db.Query<RestaurantsModel>("SELECT * FROM TR_Restaurants AS tr WHERE tr.Id =@ID", new { ID = Id }).FirstOrDefault();
                restaurantDetails = restDetails;
            }
            return restaurantDetails;
        }

        /// <summary>
        /// Select a list optimised for Combo Boxes. Return only Ids and Names from Table.
        /// </summary>
        /// <param name="language">proper Language</param>
        /// <returns>Return only Ids and a proper Restaurant Name from Table.</returns>
        public List<ComboListModel> GetComboList(DBInfoModel Store, string language)
        {
            List<ComboListModel> restaurantComboList = new List<ComboListModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string comboListQuery = @"DECLARE @rLang VARCHAR(10)
                                          SET @rLang = @setLang
                                          SELECT tr.Id, CASE WHEN UPPER(@rLang) = 'GR' THEN tr.NameGR
				                                             WHEN UPPER(@rLang) = 'EN' THEN tr.NameEn
				                                             WHEN UPPER(@rLang) = 'RU' THEN tr.NameRu
				                                             WHEN UPPER(@rLang) = 'FR' THEN tr.NameFr
				                                             WHEN UPPER(@rLang) = 'DE' THEN tr.NameDe
				                                        ELSE tr.NameGR END RestaurantName
                                          FROM TR_Restaurants AS tr";

                List<ComboListModel> restComboList = db.Query<ComboListModel>(comboListQuery, new { setLang = language }).ToList();
                restaurantComboList = restComboList;
            }
            return restaurantComboList;
        }

        /// <summary>
        /// Insert RestaurantModels
        /// </summary>
        /// <returns>id of RestaurantModels</returns>
        public long insertRestaurant(DBInfoModel Store, RestaurantsModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO TR_Restaurants(NameGR, NameEn, NameRu, NameFr, NameDe, [Image], DescriptionGR, DescriptionEn,
                                       DescriptionRu, DescriptionFr, DescriptionDe) VALUES ( @NameGR, @NameEn, @NameRu , @NameFr ,
                                       @NameDe, @Image , @DescriptionGR, @DescriptionEn ,@DescriptionRu, @DescriptionFr, @DescriptionDe)";

                db.Query(insertQuery, new
                {
                  NameGR = model.NameGR,
                  NameEn = model.NameEn,
                  NameRu = model.NameRu,
                  NameFr = model.NameFr,
                  NameDe = model.NameDe,
                  Image = model.Image,
                  DescriptionGR = model.DescriptionGR ,
                  DescriptionEn = model.DescriptionEn ,
                  DescriptionRu = model.DescriptionRu ,
                  DescriptionFr = model.DescriptionFr ,
                  DescriptionDe = model.DescriptionDe });

                res = db.Query<long>("SELECT tr.Id FROM TR_Restaurants AS tr ORDER BY tr.id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update a Restaurant
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestaurantsModel UpdateRestaurant(DBInfoModel Store, RestaurantsModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"UPDATE TR_Restaurants SET NameGR=@NameGR, NameEn=@NameEn, NameRu=@NameRu , NameFr=@NameFr ,
                                       NameDe=@NameDe, [Image]=@Image , DescriptionGR=@DescriptionGR, DescriptionEn=@DescriptionEn ,
                                       DescriptionRu=@DescriptionRu, DescriptionFr=@DescriptionFr, DescriptionDe=@DescriptionDe WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    NameGR = Model.NameGR,
                    NameEn = Model.NameEn,
                    NameRu = Model.NameRu,
                    NameFr = Model.NameFr,
                    NameDe = Model.NameDe,
                    Image = Model.Image,
                    DescriptionGR = Model.DescriptionGR,
                    DescriptionEn = Model.DescriptionEn,
                    DescriptionRu = Model.DescriptionRu,
                    DescriptionFr = Model.DescriptionFr,
                    DescriptionDe = Model.DescriptionDe
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete a Restaurant
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestaurant(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_Restaurants WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }
    }
}
