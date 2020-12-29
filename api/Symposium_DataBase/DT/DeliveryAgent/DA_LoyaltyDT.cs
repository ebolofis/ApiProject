using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Configuration;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_LoyaltyDT : IDA_LoyaltyDT
    {
        string connectionString;
            IUsersToDatabasesXML usersToDatabases;
            protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_LoyaltyDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Get DA_LoyalPoints List
        /// </summary>
        /// <param name="dBInfo"></param>
        /// <returns></returns>
        public List<DA_LoyalPointsModels> GetCustomerDa_LoyalPointsHistory(DBInfoModel dBInfo,long CustomerId)
        {

            connectionString = usersToDatabases.ConfigureConnectionString(dBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = @"SELECT * FROM DA_LoyalPoints where CustomerId=" + CustomerId;
                List<DA_LoyalPointsModels> modellist = db.Query<DA_LoyalPointsModels>(sql).ToList();
                return modellist;
            }
            
        }

        /// <summary>
        /// Get Loyalty Configuration Tables
        /// </summary>
        /// <returns>Επιστρέφει τα περιεχόμενα των πινάκων DA_Loyalty  εκτός του DA_LoyalPoints</returns>
        public DA_LoyaltyFullConfigModel GetLoyaltyConfig(DBInfoModel Store)
        {
            DA_LoyaltyFullConfigModel LoyaltyFullConfigModel = new DA_LoyaltyFullConfigModel();
            DA_LoyalConfigModel ConfigModel = new DA_LoyalConfigModel();
            List<DA_LoyalGainAmountRangeModel> GainAmountRangeModel = new List<DA_LoyalGainAmountRangeModel>();
            DA_LoyalGainAmountRatioModel GainAmountRatioModel = new DA_LoyalGainAmountRatioModel();
            DA_LoyalRedeemDiscountModel RedeemDiscountModel = new DA_LoyalRedeemDiscountModel();
            List<DA_LoyalRedeemFreeProductModel> RedeemFreeProductModel = new List<DA_LoyalRedeemFreeProductModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlConfig = @"SELECT * FROM DA_LoyalConfig AS dlc";
                ConfigModel = db.Query<DA_LoyalConfigModel>(sqlConfig).FirstOrDefault();
                string sqlGainAmountRange = @"SELECT * FROM DA_LoyalGainAmountRange AS dlgar ORDER BY dlgar.FromAmount ASC";
                GainAmountRangeModel = db.Query<DA_LoyalGainAmountRangeModel>(sqlGainAmountRange).ToList();
                string sqlGainAmountRatio = @"SELECT * FROM DA_LoyalGainAmountRatio AS dlgar";
                GainAmountRatioModel = db.Query<DA_LoyalGainAmountRatioModel>(sqlGainAmountRatio).FirstOrDefault();
                string sqlRedeemDiscount = @"SELECT * FROM DA_LoyalRedeemDiscount AS dlrd";
                RedeemDiscountModel = db.Query<DA_LoyalRedeemDiscountModel>(sqlRedeemDiscount).FirstOrDefault();
                string sqlRedeemFreeProduct = @"SELECT * FROM DA_LoyalRedeemFreeProduct AS dlrfp ORDER BY dlrfp.Id DESC";
                RedeemFreeProductModel = db.Query<DA_LoyalRedeemFreeProductModel>(sqlRedeemFreeProduct).ToList();
            }
            LoyaltyFullConfigModel.LoyalConfigModel = ConfigModel;
            LoyaltyFullConfigModel.LoyalGainAmountRangeModel = GainAmountRangeModel;
            LoyaltyFullConfigModel.LoyalGainAmountRatioModel = GainAmountRatioModel;
            LoyaltyFullConfigModel.LoyalRedeemDiscountModel = RedeemDiscountModel;
            LoyaltyFullConfigModel.LoyalRedeemFreeProductModel = RedeemFreeProductModel;

            return LoyaltyFullConfigModel;
        }

        /// <summary>
        /// Set Loyalty Configuration Tables
        /// </summary>
        /// <param name="Model">DA_LoyaltyFullConfigModel</param>
        /// <returns></returns>
        public long SetLoyaltyConfig(DBInfoModel Store, DA_LoyaltyFullConfigModel Model)
        {
            long check = 0;
            int ConfigModelCount = 0;
            int GainAmountRatioModelCount = 0;
            int RedeemDiscountModelCount = 0;

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    string sqlConfig = @"SELECT COUNT(*) FROM DA_LoyalConfig AS dlc";
                    ConfigModelCount = db.Query<int>(sqlConfig).FirstOrDefault();
                    string sqlGainAmountRatio = @"SELECT COUNT(*) FROM DA_LoyalGainAmountRatio AS dlgar";
                    GainAmountRatioModelCount = db.Query<int>(sqlGainAmountRatio).FirstOrDefault();
                    string sqlRedeemDiscount = @"SELECT COUNT(*) FROM DA_LoyalRedeemDiscount AS dlrd";
                    RedeemDiscountModelCount = db.Query<int>(sqlRedeemDiscount).FirstOrDefault();

                    if (ConfigModelCount > 0)
                    {
                        string sqlUpdateConfig = @"UPDATE DA_LoyalConfig SET
	                                                GainPointsType =@gainPointsType,
	                                                RedeemType =@redeemType,
	                                                MaxPoints =@maxPoints,
	                                                ExpDuration =@expDuration,
	                                                MinAmount =@minAmount,
	                                                InitPoints =@initPoints";
                        db.Execute(sqlUpdateConfig,
                            new
                            {
                                gainPointsType = Model.LoyalConfigModel.GainPointsType,
                                redeemType = Model.LoyalConfigModel.RedeemType,
                                maxPoints = Model.LoyalConfigModel.MaxPoints,
                                expDuration = Model.LoyalConfigModel.ExpDuration,
                                minAmount = Model.LoyalConfigModel.MinAmount,
                                initPoints = Model.LoyalConfigModel.InitPoints
                            });
                    }
                    else
                    {
                        string sqlInsertConfig = @"INSERT INTO DA_LoyalConfig
                                                    (
	                                                    GainPointsType,
	                                                    RedeemType,
	                                                    MaxPoints,
	                                                    ExpDuration,
	                                                    MinAmount,
	                                                    InitPoints
                                                    )
                                                    VALUES
                                                    (
	                                                    @gainPointsType,
	                                                    @redeemType,
	                                                    @maxPoints,
	                                                    @expDuration,
	                                                    @minAmount,
	                                                    @initPoints
                                                    )";
                        db.Execute(sqlInsertConfig,
                            new
                            {
                                gainPointsType = Model.LoyalConfigModel.GainPointsType,
                                redeemType = Model.LoyalConfigModel.RedeemType,
                                maxPoints = Model.LoyalConfigModel.MaxPoints,
                                expDuration = Model.LoyalConfigModel.ExpDuration,
                                minAmount = Model.LoyalConfigModel.MinAmount,
                                initPoints = Model.LoyalConfigModel.InitPoints
                            });
                    }
                    if (Model.LoyalConfigModel.GainPointsType == DA_LoyaltyGainPointsTypeEnums.AmountRatio)
                    {
                        if (GainAmountRatioModelCount > 0)
                        {
                            string sqlUpdateGainAmountRatio = @"UPDATE DA_LoyalGainAmountRatio SET ToPoints =@toPoints";
                            db.Query<DA_LoyalConfigModel>(sqlUpdateGainAmountRatio, new { toPoints = Model.LoyalGainAmountRatioModel.ToPoints }).FirstOrDefault();
                        }
                        else
                        {
                            if (Model.LoyalGainAmountRatioModel != null)
                            {
                                string sqlInsertGainAmountRatio = @"INSERT INTO DA_LoyalGainAmountRatio(ToPoints) VALUES(@toPoints)";
                                db.Query<DA_LoyalConfigModel>(sqlInsertGainAmountRatio, new { toPoints = Model.LoyalGainAmountRatioModel.ToPoints }).FirstOrDefault();
                            }
                        }
                    }
                    if (Model.LoyalConfigModel.RedeemType != DA_LoyaltyRedeemTypeEnums.FreeProduct)
                    {
                        if (RedeemDiscountModelCount > 0)
                        {
                            string sqlUpdateRedeemDiscount = @"UPDATE DA_LoyalRedeemDiscount SET DiscountRatio =@discountRatio";
                            db.Query<DA_LoyalConfigModel>(sqlUpdateRedeemDiscount, new { discountRatio = Model.LoyalRedeemDiscountModel.DiscountRatio }).FirstOrDefault();
                        }
                        else
                        {
                            if (Model.LoyalRedeemDiscountModel != null)
                            {
                                string sqlInsertRedeemDiscount = @"INSERT INTO DA_LoyalRedeemDiscount(DiscountRatio) VALUES(@discountRatio)";
                                db.Query<DA_LoyalConfigModel>(sqlInsertRedeemDiscount, new { discountRatio = Model.LoyalRedeemDiscountModel.DiscountRatio }).FirstOrDefault();
                            }
                        }
                    }

                    scope.Complete();
                }
            }

            return check;
        }

        /// <summary>
        /// Insert Loyalty Gain Amount Range Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertGainAmountRange(DBInfoModel Store, DA_LoyalGainAmountRangeModel Model)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"INSERT INTO DA_LoyalGainAmountRange
                                    (
	                                    FromAmount,
	                                    ToAmount,
	                                    Points
                                    )
                                    VALUES
                                    (
	                                    @fromAmount,
                                        @toAmount,
                                        @points
                                    )";
                res = db.Query<long>(sqlData, new { fromAmount = Model.FromAmount, toAmount = Model.ToAmount, points = Model.Points }).FirstOrDefault();
            }

            return res;
        }


        /// <summary>
        /// Delete Loyalty Gain Points Range Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRangeRow(DBInfoModel Store, long Id)
        {
            long rowId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"DELETE FROM DA_LoyalGainAmountRange WHERE Id =@ID";
                rowId = db.Query<long>(sqlData, new { ID = Id }).FirstOrDefault();
            }

            return rowId;
        }

        /// <summary>
        /// Delte All Loyalty Gain Amount Range
        /// </summary>
        /// <returns></returns>
        public long DeleteGainAmountRange(DBInfoModel Store)
        {
            long rowId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"DELETE FROM DA_LoyalGainAmountRange";
                rowId = db.Query<long>(sqlData).FirstOrDefault();
            }
            return rowId;
        }

        /// <summary>
        /// Insert Redeem Free Product Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertRedeemFreeProduct(DBInfoModel Store, DA_LoyalRedeemFreeProductModel Model)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlInsertRedeemFreeProduct = @"INSERT INTO DA_LoyalRedeemFreeProduct
                                                        (
	                                                        Points,
	                                                        ProductId,
	                                                        ProdCategoryId,
	                                                        ProductName,
	                                                        ProdCategoryName,
	                                                        Qnt
                                                        )
                                                        VALUES
                                                        (
	                                                        @points,
	                                                        @productId,
	                                                        @prodCategoryId,
	                                                        @productName,
	                                                        @prodCategoryName,
	                                                        @qnt
                                                        )";

                db.Query<DA_LoyalRedeemFreeProductModel>(sqlInsertRedeemFreeProduct,
                        new
                        {
                            points = Model.Points,
                            productId = Model.ProductId,
                            prodCategoryId = Model.ProdCategoryId,
                            productName = Model.ProductName,
                            prodCategoryName = Model.ProdCategoryName,
                            qnt = Model.Qnt
                        }).FirstOrDefault();
                res = 1;
            }

            return res;
        }


        /// <summary>
        /// Delete Loyalty Redeem Free Product Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRedeemFreeProductRow(DBInfoModel Store, long Id)
        {
            long rowId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"DELETE FROM DA_LoyalRedeemFreeProduct WHERE Id =@ID";
                rowId = db.Query<long>(sqlData, new { ID = Id }).FirstOrDefault();
            }

            return rowId;
        }

        /// <summary>
        /// Delte All Redeem Free Product
        /// </summary>
        /// <returns></returns>
        public long DeleteRedeemFreeProduct(DBInfoModel Store)
        {
            long rowId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"DELETE FROM DA_LoyalRedeemFreeProduct";
                rowId = db.Query<long>(sqlData).FirstOrDefault();
            }
            return rowId;
        }

        /// <summary>
        /// Find Total Loyalty Point of a Customer
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <returns>Tο σύνολο των πόντων του πελάτη </returns>
        public int GetLoyaltyPoints(DBInfoModel Store, long Id)
        {
            int totalPoints = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string dt = GetTimeThresholdString();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"SELECT isnull(SUM(dlp.Points),0) AS TotalPoints FROM DA_LoyalPoints AS dlp WHERE  dlp.[Date]<=@Dt and dlp.CustomerId =@CustId";
                totalPoints = db.Query<int>(sqlData, new { CustId = Id, Dt = dt }).FirstOrDefault();
            }

            return totalPoints;
        }

        /// <summary>
        /// Choose Loyalty Redeem Options
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <param name="Amount">Order Total</param>
        /// <returns>Επιστρέφει λίστα με επιλογές  που έχει ο πελάτης(κατά τη διάρκεια της παραγγελίας του) να καταναλώσει τους  πόντους του</returns>
        public DA_LoyaltyRedeemOptionsModel GetLoyaltyRedeemOptions(DBInfoModel Store, long Id, decimal Amount)
        {
            DA_LoyaltyRedeemOptionsModel RedeemOptionsModel = new DA_LoyaltyRedeemOptionsModel();
            DA_LoyalConfigModel ConfigModel = new DA_LoyalConfigModel();
            DA_LoyalRedeemDiscountModel RedeemDiscountModel = new DA_LoyalRedeemDiscountModel();
            RedeemOptionsModel.DiscountRedeemModel = new DA_LoytaltyDiscountRedeemModel();
            int ConfigModelCount = 0;
            bool hasLoyalty = false;
            int totalPoints = 0;
            decimal maxDiscountAmount = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //1. Ελέγχουμε αν υπάρχει εγγραφή στον DA_LoyalConfig
                string sqlConfig = @"SELECT COUNT(*) FROM DA_LoyalConfig AS dlc";
                ConfigModelCount = db.Query<int>(sqlConfig).FirstOrDefault();
                if (ConfigModelCount > 0)
                {
                    //Παίρνουμε τις τιμές του DA_LoyalConfig
                    string sqlGetConfig = @"SELECT * FROM DA_LoyalConfig AS dlc";
                    ConfigModel = db.Query<DA_LoyalConfigModel>(sqlGetConfig).FirstOrDefault();

                    //Ελέγχουμε αν ο χρήστης έχει loyalty (DA_Customers.Loyalty=true)
                    string sqlHasLoyalty = @"SELECT dc.Loyalty FROM DA_Customers AS dc WHERE dc.Id =@CustId";
                    hasLoyalty = db.Query<bool>(sqlHasLoyalty, new { CustId = Id }).FirstOrDefault();
                    if (hasLoyalty == true)
                    {
                        //Παίρνουμε τους συνολικούς πόντους του πελάτη.
                        string sqlData = @"SELECT isnull(SUM(dlp.Points),0) AS TotalPoints FROM DA_LoyalPoints AS dlp WHERE dlp.CustomerId =@CustId";
                        totalPoints = db.Query<int>(sqlData, new { CustId = Id }).FirstOrDefault();

                        if (ConfigModel.RedeemType == DA_LoyaltyRedeemTypeEnums.FreeProduct || ConfigModel.RedeemType == DA_LoyaltyRedeemTypeEnums.Both)
                        {
                            string sqlFreeProduct = @"SELECT * FROM DA_LoyalRedeemFreeProduct AS dlrfp WHERE dlrfp.Points <= @points order by productid, ProdCategoryId, points";
                            RedeemOptionsModel.RedeemFreeProductModel = db.Query<DA_LoyalRedeemFreeProductModel>(sqlFreeProduct, new { points = totalPoints }).ToList();

                        }
                        if (ConfigModel.RedeemType == DA_LoyaltyRedeemTypeEnums.Discount || ConfigModel.RedeemType == DA_LoyaltyRedeemTypeEnums.Both)
                        {
                            string sqlRedeemDiscount = @"SELECT * FROM DA_LoyalRedeemDiscount AS dlrd";
                            RedeemDiscountModel = db.Query<DA_LoyalRedeemDiscountModel>(sqlRedeemDiscount).FirstOrDefault();
                            maxDiscountAmount = totalPoints * RedeemDiscountModel.DiscountRatio;
                            if (maxDiscountAmount >= Amount)
                            {
                                RedeemOptionsModel.DiscountRedeemModel.MaxDiscountAmount = Amount - (decimal)0.01;
                                RedeemOptionsModel.DiscountRedeemModel.RedeemPoints = (int)Math.Round(Amount / RedeemDiscountModel.DiscountRatio);
                            }
                            else
                            {
                                RedeemOptionsModel.DiscountRedeemModel.MaxDiscountAmount = maxDiscountAmount;
                                RedeemOptionsModel.DiscountRedeemModel.RedeemPoints = totalPoints;
                            }
                        }
                        else
                        {
                            string sqlFreeProduct = @"SELECT * FROM DA_LoyalRedeemFreeProduct AS dlrfp WHERE dlrfp.Points <= @points";
                            RedeemOptionsModel.RedeemFreeProductModel = db.Query<DA_LoyalRedeemFreeProductModel>(sqlFreeProduct, new { points = totalPoints }).ToList();

                            string sqlRedeemDiscount = @"SELECT * FROM DA_LoyalRedeemDiscount AS dlrd";
                            RedeemDiscountModel = db.Query<DA_LoyalRedeemDiscountModel>(sqlRedeemDiscount).FirstOrDefault();
                            maxDiscountAmount = totalPoints * RedeemDiscountModel.DiscountRatio;
                            if (maxDiscountAmount < Amount)
                            {
                                RedeemOptionsModel.DiscountRedeemModel.MaxDiscountAmount = maxDiscountAmount;
                                RedeemOptionsModel.DiscountRedeemModel.RedeemPoints = totalPoints;
                            }
                            else
                            {
                                RedeemOptionsModel.DiscountRedeemModel.MaxDiscountAmount = Amount - (decimal)0.01;
                                RedeemOptionsModel.DiscountRedeemModel.RedeemPoints = (int)Math.Round(totalPoints / RedeemDiscountModel.DiscountRatio);
                            }
                        }
                    }
                }

            }

            return RedeemOptionsModel;
        }

        /// <summary>
        /// υπολογισμός κερδισμένων πόντων σε παραγγελία 
        /// </summary>
        /// <param name="Model">order</param>
        /// <returns>gain points</returns>
        public int CalcPointsFromOrder(DBInfoModel Store, DA_OrderModel Model)
        {
            DA_LoyalConfigModel ConfigModel = new DA_LoyalConfigModel();
            List<DA_LoyalGainAmountRangeModel> GainAmountRangeModel = new List<DA_LoyalGainAmountRangeModel>();
            DA_LoyalGainAmountRatioModel GainAmountRatioModel = new DA_LoyalGainAmountRatioModel();
            int ConfigModelCount = 0;
            bool hasLoyalty = false;
            int totalPoints = 0; //πόντοι που κερδίζονται
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                //1. Ελέγχουμε αν υπάρχει εγγραφή στον DA_LoyalConfig
                string sqlConfig = @"SELECT COUNT(*) FROM DA_LoyalConfig AS dlc";
                ConfigModelCount = db.Query<int>(sqlConfig).FirstOrDefault();
                if (ConfigModelCount > 0)
                {
                    //2. Ελέγχουμε αν ο χρήστης έχει loyalty (DA_Customers.Loyalty=true)
                    string sqlHasLoyalty = @"SELECT IsNull(dc.Loyalty, 0) FROM DA_Customers AS dc WHERE dc.Id =@CustId";
                    hasLoyalty = db.Query<bool>(sqlHasLoyalty, new { CustId = Model.CustomerId }).FirstOrDefault();
                    if (hasLoyalty == true)
                    {
                        /*3. Ελέγχουμε Αν οι υπάρχοντες πόντοι υπερβαίνουν το DA_LoyalConfig. MaxPoints
                             Αν τους υπερβαίνουν τότε θα εισαχθούν στον DA_LoyalPoints τόσοι πόντοι ώστε το σύνολό τους να είναι ίσο με DA_LoyalConfig. MaxPoints */
                        string sqlGetConfig = @"SELECT * FROM DA_LoyalConfig AS dlc";
                        ConfigModel = db.Query<DA_LoyalConfigModel>(sqlGetConfig).FirstOrDefault();

                        // 3.a Σε περίπτωση που έχουμε AmountRange 
                        if (ConfigModel.GainPointsType == (int)DA_LoyaltyGainPointsTypeEnums.AmountRange)
                        {
                            string sqlGainAmountRange = @"SELECT * FROM DA_LoyalGainAmountRange AS dlgar";
                            GainAmountRangeModel = db.Query<DA_LoyalGainAmountRangeModel>(sqlGainAmountRange).ToList();
                            foreach (DA_LoyalGainAmountRangeModel range in GainAmountRangeModel)
                            {
                                if (Model.Total >= range.FromAmount && Model.Total < range.ToAmount)
                                {
                                    totalPoints = range.Points;
                                    break;
                                }
                            }
                            string sqlData = @"SELECT isnull(SUM(dlp.Points),0) AS TotalPoints FROM DA_LoyalPoints AS dlp WHERE dlp.CustomerId =@CustId";
                            int currentPoints = db.Query<int>(sqlData, new { CustId = Model.CustomerId }).FirstOrDefault();
                            if (currentPoints >= ConfigModel.MaxPoints)
                                totalPoints = 0;

                            else if (totalPoints + currentPoints > ConfigModel.MaxPoints)
                            {
                                totalPoints = ConfigModel.MaxPoints - currentPoints;
                                if (totalPoints < 0) totalPoints = 0;
                            }
                        }

                        // 3.b Σε περίπτωση που έχουμε AmountRatio
                        else
                        {
                            string sqlGainAmountRatio = @"SELECT * FROM DA_LoyalGainAmountRatio AS dlgar";
                            GainAmountRatioModel = db.Query<DA_LoyalGainAmountRatioModel>(sqlGainAmountRatio).FirstOrDefault();
                            totalPoints = (int)Math.Round(Model.Total * GainAmountRatioModel.ToPoints, 0);
                            string sqlData = @"SELECT isnull(SUM(dlp.Points),0) AS TotalPoints FROM DA_LoyalPoints AS dlp WHERE dlp.CustomerId =@CustId";
                            int currentPoints = db.Query<int>(sqlData, new { CustId = Model.CustomerId }).FirstOrDefault();
                            if (currentPoints >= ConfigModel.MaxPoints)
                                totalPoints = 0;

                            else if (totalPoints + currentPoints > ConfigModel.MaxPoints)
                            {
                                totalPoints = ConfigModel.MaxPoints - currentPoints;
                                if (totalPoints < 0) totalPoints = 0;
                            }
                        }

                        //4. Αν το ποσό της παραγγελίας είναι μεγαλύτερο του DA_LoyalConfig.MinAmount
                        if (Model.Total > ConfigModel.MinAmount)
                            return totalPoints;
                        else
                            return 0;
                    }
                }
            }
            return totalPoints;
        }

        /// <summary>
        /// εισαγωγή Αρχικών πόντων στο table DA_LoyalPoints
        /// </summary>
        /// <returns></returns>
        public long InsertInitPoints(DBInfoModel Store, DACustomerModel Model)
        {
            long CustId = 0;
            DA_LoyalConfigModel ConfigModel = new DA_LoyalConfigModel();
            int ConfigModelCount = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //Ελέγχουμε αν υπάρχει εγγραφή στον DA_LoyalConfig
                string sqlConfig = @"SELECT COUNT(*) FROM DA_LoyalConfig AS dlc";
                ConfigModelCount = db.Query<int>(sqlConfig).FirstOrDefault();
                if (ConfigModelCount > 0)
                {
                    //Παίρνουμε τις τιμές του DA_LoyalConfig
                    string sqlGetConfig = @"SELECT * FROM DA_LoyalConfig AS dlc";
                    ConfigModel = db.Query<DA_LoyalConfigModel>(sqlGetConfig).FirstOrDefault();

                    if (ConfigModel.InitPoints > 0)
                    {
                        AddPoints(Store, 0, Model.Id, ConfigModel.InitPoints ?? 0, GetTimeThreshold(), 1, 0);

                        CustId = Model.Id;
                    }
                }
            }
            return CustId;
        }


        /// <summary>
        /// Validate Κατανάλωσης Πόντων του Πελάτη με Βάση τους πόντους που δίνει ο Client. 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public void CheckRedeemPoints(DBInfoModel Store, DA_OrderModel Model)
        {
            long OrderId = 0;
            int ConfigModelCount = 0;
            int totalPoints = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //Ελέγχουμε αν υπάρχει εγγραφή στον DA_LoyalConfig
                string sqlConfig = @"SELECT COUNT(*) FROM DA_LoyalConfig AS dlc";
                ConfigModelCount = db.Query<int>(sqlConfig).FirstOrDefault();
                if (ConfigModelCount > 0)
                {
                    string sqlData = @"SELECT isnull(SUM(dlp.Points),0) AS TotalPoints FROM DA_LoyalPoints AS dlp WHERE dlp.CustomerId =@CustId";
                    totalPoints = db.QueryFirst<int>(sqlData, new { CustId = Model.CustomerId });
                    if (Model.PointsRedeem > totalPoints)
                    {
                        logger.Warn($"Client posted {Model.PointsRedeem} redeem points but customer {Model.CustomerId} has {totalPoints} points. Order origin :{Model.Origin}");
                        throw new BusinessException(Symposium.Resources.Errors.POINTSARENOTENOUGH);
                    }


                }
            }
        }

        /// <summary>
        /// Add loyalty points (gained/redeemed) to tables DA_LoyalPoints and DA_Orders
        /// </summary>
        ///  <param name="Store">Store</param>
        /// <param name="OrderId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="Points"> gain/redeem points </param>
        /// <param name="type">1= gain points. 2= redeem points </param>
        /// <param name="StoreId">Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0) </param>
        public void AddPoints(DBInfoModel Store, long OrderId, long CustomerId, int Points, DateTime Date, int type, long StoreId)
        {
            if (type == 2 && Points > 0) Points = -Points;
            if (Points < 0)
                Date = DateTime.Now.AddHours(-GetLoyaltypointsThresholdHounrs()).AddSeconds(-30);
            else
                Date = DateTime.Now;

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (Points != 0)
                {
                    string sqlInsertLoyalPoints = @"INSERT INTO DA_LoyalPoints
                                                            (
                                                             CustomerId,
                                                             Points,
                                                             [Date],
                                                             OrderId,
                                                             StoreId
                                                            )
                                                            VALUES
                                                            (
                                                             @customerId,
                                                             @points,
                                                             @date,
                                                             @orderId,
                                                             @StoreId
                                                            )";
                    db.Execute(sqlInsertLoyalPoints,
                        new
                        {
                            customerId = CustomerId,
                            points = Points,
                            date = Date,
                            orderId = OrderId,
                            StoreId = StoreId
                        });
                }

                if (OrderId > 0 && StoreId == 0 && type == 1)// gain points from DA order
                    db.Execute("update DA_Orders set PointsGain=@Points where Id=@Id", new { Points = Points, Id = OrderId });
                if (OrderId > 0 && StoreId == 0 && type == 2) // redeem points from DA order
                    db.Execute("update DA_Orders set PointsRedeem=@Points  where Id=@Id", new { Points = Points, Id = OrderId });

            }
        }

        /// <summary>
        /// Διαγραφή κερδισμένων πόντων από table DA_LoyalPoints βάση Customerid
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        public long DeleteCustomerGainPoints(DBInfoModel Store, long Id)
        {
            long CustId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlDeletePoints = @"DELETE FROM DA_LoyalPoints WHERE CustomerId =@custId";
                db.Execute(sqlDeletePoints, new { custId = Id });
                CustId = Id;
            }

            return CustId;//  <--- return to CustId γιατί ????
        }


        /// <summary>
        /// Διαγραφή κερδισμένων πόντων από table DA_LoyalPoints βάση orderid
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId">Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0) </param>
        /// <returns></returns>
        public long DeleteGainPoints(DBInfoModel Store, long Id, long StoreId)
        {
            long OrderId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlDeletePoints = @"DELETE FROM DA_LoyalPoints WHERE OrderId =@orderId and StoreId=@StoreId";
                db.Execute(sqlDeletePoints, new { orderId = Id, StoreId = StoreId });
                OrderId = Id;
            }
            return OrderId;//  <--- return to orderid γιατί ????
        }

        /// <summary>
        /// Διαγραφή πόντων από table DA_LoyalPoints βάση παλαιότητας
        /// </summary>
        /// <returns></returns>
        public long DeletePoints(DBInfoModel Store)
        {
            long CheckId = 0;
            int ConfigModelCount = 0;
            DA_LoyalConfigModel ConfigModel = new DA_LoyalConfigModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //Ελέγχουμε αν υπάρχει εγγραφή στον DA_LoyalConfig
                string sqlConfig = @"SELECT COUNT(*) FROM DA_LoyalConfig AS dlc";
                ConfigModelCount = db.Query<int>(sqlConfig).FirstOrDefault();
                if (ConfigModelCount > 0)
                {
                    SavePointsToDaLoyalPointsHist(Store); //Saving points to the historical table before deletion

                    List<long> distinctCustomerId = new List<long>();
                    distinctCustomerId = getDistinctCustomerIdsBeforeDeletion(Store);     //getting the list of distinct customers whose points will be deleted

                    string sqlDelete = @"DELETE FROM DA_LoyalPoints WHERE Id IN (
                                            SELECT dlp.Id
                                            FROM DA_LoyalPoints AS dlp
                                            INNER JOIN DA_LoyalConfig AS dlc ON 1 = 1
                                            WHERE dlp.[Date] < CAST(convert(nvarchar(10), DATEADD(MONTH, (-1) * dlc.ExpDuration, GETDATE()), 120) AS DATETIME))";
                    db.Execute(sqlDelete);

                    sanitizeNegativePointsSumAfterDeletion(Store, distinctCustomerId);

                    CheckId = 1;
                }

            }
            return CheckId;
        }



        public List<long> getDistinctCustomerIdsBeforeDeletion(DBInfoModel DBInfo)
        {
            string sqlPointsToBeDeleted = @"Select * FROM DA_LoyalPoints WHERE Id IN (
                                            SELECT dlp.Id
                                            FROM DA_LoyalPoints AS dlp
                                            INNER JOIN DA_LoyalConfig AS dlc ON 1 = 1
                                            WHERE dlp.[Date] < CAST(convert(nvarchar(10), DATEADD(MONTH, (-1) * dlc.ExpDuration, GETDATE()), 120) AS DATETIME))";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<DA_LoyalPointsDTO> pointsToBeDeletedList = new List<DA_LoyalPointsDTO>();
                pointsToBeDeletedList = db.Query<DA_LoyalPointsDTO>(sqlPointsToBeDeleted).ToList();
                List<long> distinctCustomerId = new List<long>();
                distinctCustomerId = pointsToBeDeletedList.Select(x => x.CustomerId).Distinct().ToList();
                return distinctCustomerId;
            }

        }


        public void sanitizeNegativePointsSumAfterDeletion(DBInfoModel DBInfo, List<long> customers)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (long custId in customers)
                {

                    string sqlPointsPerCustomer = @"select * from DA_LoyalPoints  WHERE CustomerId = " + custId;

                    List<DA_LoyalPointsDTO> pointsPerCustomerToBeSanitized = new List<DA_LoyalPointsDTO>();
                    pointsPerCustomerToBeSanitized = db.Query<DA_LoyalPointsDTO>(sqlPointsPerCustomer).ToList();

                  long customerSum =  pointsPerCustomerToBeSanitized.Select(x => x.Points).Sum();

                    if (customerSum >= 0) continue;

                    if(customerSum <0)
                    {
                       List<DA_LoyalPointsDTO> negativeRows = new  List<DA_LoyalPointsDTO>();
                        negativeRows = pointsPerCustomerToBeSanitized.Where(y => y.Points < 0).OrderBy(x => x.Date).ToList();
                        bool doneFlag = false;
                                foreach (DA_LoyalPointsDTO row in negativeRows)
                                {
                                    customerSum -= row.Points;
                            
                                    string sqlDeleteNegativeRow = @"delete from DA_LoyalPoints where Id =" + row.Id;
                                    string sqlInsertNegativeRowToHist = @"INSERT INTO DA_LoyalPoints_Hist select Year(Date),* from DA_LoyalPoints where Id =" + row.Id;
                                    db.Execute(sqlInsertNegativeRowToHist);
                                    db.Execute(sqlDeleteNegativeRow);

                                         if (customerSum >= 0) //stop
                                        {
                                            doneFlag = true;
                                            break;
                                        }
                                }

                                if(doneFlag)
                                {
                                    doneFlag = false;
                                    continue;
                                }

                    }

                }

            }

        }

        public void SavePointsToDaLoyalPointsHist(DBInfoModel DBInfo)
        {
            string sqlPointsToBeInsertedToDALoyalPointsHist = @"INSERT INTO DA_LoyalPoints_Hist
				Select Year(Date) as nYear, * FROM DA_LoyalPoints WHERE Id IN (
                SELECT dlp.Id
                FROM DA_LoyalPoints AS dlp
                INNER JOIN DA_LoyalConfig AS dlc ON 1 = 1
                WHERE dlp.[Date] < CAST(convert(nvarchar(10), DATEADD(MONTH, (-1) * dlc.ExpDuration, GETDATE()), 120) AS DATETIME))";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
                    {
                        db.Execute(sqlPointsToBeInsertedToDALoyalPointsHist);
                    }
            }
        public void SavePointsFromLoyaltyAdmin(DBInfoModel DBInfo, DA_LoyalPointsModels model)
        {
            var SumPoints = 0;
            DA_LoyalConfigModel ConfigModel = new DA_LoyalConfigModel();
            List<DA_LoyalPointsModels> LoyalPointsModel = new List<DA_LoyalPointsModels>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlConfig = @"SELECT MaxPoints FROM DA_LoyalConfig";
                ConfigModel = db.Query<DA_LoyalConfigModel>(sqlConfig).FirstOrDefault();
                var MaxPoints = ConfigModel.MaxPoints;

                string sqlDA_LoyalPointsSum = @"SELECT * FROM DA_LoyalPoints where CustomerId=@customerid";
                LoyalPointsModel = db.Query<DA_LoyalPointsModels>(sqlDA_LoyalPointsSum, new { CustomerId = model.CustomerId }).ToList();

                foreach (DA_LoyalPointsModels range in LoyalPointsModel)
                {
                    SumPoints = SumPoints + range.Points;
                }


                if (SumPoints + model.Points <= MaxPoints)
                {

                    var date = DateTime.Now.AddHours(-GetLoyaltypointsThresholdHounrs()).AddSeconds(-30);
                    string sql = @"INSERT INTO DA_LoyalPoints
                                                            (
                                                             CustomerId,
                                                             Points,
                                                             [Date],
                                                             OrderId,
                                                             StoreId,
                                                               StaffId,
                                                            Description
                                                            )
                                                            VALUES
                                                            (
                                                             @customerId,
                                                             @points,
                                                             @date,
                                                             @orderId,
                                                             @StoreId,
                                                            @StaffId,
                                                            @Description
                                                            )";
                    db.Execute(sql,
              new
              {
                  customerId = model.CustomerId,
                  points = model.Points,
                  date = date,
                  orderId = model.OrderId,
                  StoreId = model.StoreId,
                  StaffId = model.StaffId,
                  Description = model.Description
              });

                }
                else
                {
                    //Loyalty Points over MaxPoints i.e 320
                    throw new BusinessException(Symposium.Resources.Errors.MAXLOYALTYPOINTSLIMITREACHED);
                }

            }
        }

        /// <summary>
        /// Return the list of last Loyalty entries of the Current Customer
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="customerId"></param>
        /// <param name="entries"></param>
        /// <param name="ptype"></param>
        /// <returns></returns>
        public List<DA_LoyalPointsModels> GetCustomerLoyaltyPointsHistory(DBInfoModel DBInfo, long customerId, int entries, DA_LoyaltyHistory ptype)
        {
            string sqlCommand = "SELECT";
            if (entries != 0)
            {
                sqlCommand += " TOP " + entries;
            }
            sqlCommand += " * FROM DA_LoyalPoints WHERE CustomerId = " + customerId;
            switch (ptype)
            {
                case DA_LoyaltyHistory.LoyaltyWithoutOrder:
                    sqlCommand += " AND OrderId = 0";
                    break;
                case DA_LoyaltyHistory.LoyaltyWithOrder:
                    sqlCommand += " AND OrderId != 0";
                    break;
                case DA_LoyaltyHistory.All:
                default:
                    break;
            }
            sqlCommand += " ORDER BY Date DESC";
            logger.Info("Customer Loyalty Points History query to execute: " + sqlCommand);
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            List<DA_LoyalPointsDTO> loyaltyPoints = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                loyaltyPoints = db.Query<DA_LoyalPointsDTO>(sqlCommand).ToList();
            }
            List<DA_LoyalPointsModels> customerLoyaltyPointsHistory = AutoMapper.Mapper.Map<List<DA_LoyalPointsModels>>(loyaltyPoints);
            return customerLoyaltyPointsHistory;
        }

        /// <summary>
        /// return the time threshold for calculating total points
        /// </summary>
        /// <returns></returns>
        private DateTime GetTimeThreshold()
        {
            int h = GetLoyaltypointsThresholdHounrs();
            if (h < 0) h = 0;
            return DateTime.Now.AddHours(-h);
        }

        /// <summary>
        /// return the time threshold (as string  yyyy-MM-DD mm:ss) for calculating total points
        /// </summary>
        /// <returns></returns>
        private string GetTimeThresholdString()
        {
            return GetTimeThreshold().ToString("yyyy-MM-dd HH:mm");
        }

        private int GetLoyaltypointsThresholdHounrs()
        {
            long loyaltyPointsThresholdHoursRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_LoyaltypointsThresholdHours");
            int loyaltyPointsThresholdHours = Convert.ToInt32(loyaltyPointsThresholdHoursRaw);
            return loyaltyPointsThresholdHours;
        }

    }
}
