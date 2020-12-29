using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Device.Location;
using System.Text;
using System.Threading.Tasks;
using GeographyPolygons;
using System.Transactions;
using log4net;
using Symposium.Helpers.Classes;
using Symposium.Helpers;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_GeoPolygonsDT : IDA_GeoPolygonsDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        private static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        LocalConfigurationHelper configHlp;


        public DA_GeoPolygonsDT(IUsersToDatabasesXML usersToDatabases, LocalConfigurationHelper configHlp)
        {
            this.usersToDatabases = usersToDatabases;
            this.configHlp = configHlp;
        }

        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση AddressId : Εάν StoreId = 0 τότε η διεύθυνση δεν ανήκει σε πολύγωνο, Αν StoreId είναι μικρότερο του 0 τότε το πολύγωνο είναι ΑΝΕΝΕΡΓΟ. 
        /// </summary>
        /// <param name="Id">AddressId</param>
        /// <returns>StoreId (Εάν StoreId = 0 τότε η διεύθυνση δεν ανήκει σε πολύγωνο. Αν StoreId είναι μικρότερο του 0 τότε το πολύγωνο είναι ΑΝΕΝΕΡΓΟ)</returns>
        public DA_GeoPolygonsBasicModel SelectPolygonByAddressId(DBInfoModel dbInfo, long Id)
        {
            configHlp.CheckDeliveryAgent();
            bool isInPolygon = false;
            long StoreId = 0;
            List<DA_GeoPolygonsModel> PolygonsModel = null;
            DA_AddressModel addr;
            DA_GeoPolygonsBasicModel model = new DA_GeoPolygonsBasicModel();
            model.StoreId = 0;
            model.Id = 0;

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //1. get address
                string addrSQL = @"SELECT * FROM DA_Addresses AS da WHERE da.Id =@addressId";
                addr = db.Query<DA_AddressModel>(addrSQL, new { addressId = Id }).FirstOrDefault();
                if (addr == null)
                {
                    logger.Warn($">>>>---> UNABLE TO FIND ADDRESS WITH ID {Id} to DB. No store matched (return storeId=0) !!!! ");
                    return model;
                }


                //2. get polygons
                string getActivePolygons = @"SELECT * FROM DA_GeoPolygons";
                PolygonsModel = db.Query<DA_GeoPolygonsModel>(getActivePolygons).ToList();

                bool useGeopolygonShape = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_GeolocationUseShape");

                if (useGeopolygonShape)
                {
                    string intrSQL = CreateShapeIntersector(0, addr.Latitude ?? 0, addr.Longtitude ?? 0);
                    int polygId = db.QueryFirst<int>(intrSQL);
                    if (polygId > 0)
                    {
                        DA_GeoPolygonsModel shapePolygon = GetPolygonById(dbInfo, polygId);
                        StoreId = Convert.ToInt32(shapePolygon.StoreId);
                        if (shapePolygon.IsActive == false)
                        {
                            logger.Warn($" ADDRESS WITH ID {Id} matched into Inactive Polygon with Id {shapePolygon.Id}. Returning negative storeId -{StoreId}... ");
                            StoreId = -StoreId;
                        }
                        else
                        {
                            logger.Debug($" Address with {Id} matched into Polygon with Id {shapePolygon.Id}. Returning StoreId {StoreId}... ");
                        }
                        model.StoreId = StoreId;
                        model.Id = shapePolygon.Id;
                        return model;
                    }
                }
                else
                {
                    //3. search polygons
                    foreach (DA_GeoPolygonsModel polygon in PolygonsModel)
                    {
                        string interSQL = "";
                        try
                        {
                            interSQL = CreateIntersector(polygon.Id, addr.Latitude ?? 0, addr.Longtitude ?? 0);

                            isInPolygon = db.QueryFirst<bool>(interSQL);//return true if address is into polygon
                            if (isInPolygon == true)//eureka !!!!
                            {
                                StoreId = polygon.StoreId ?? 0;

                                if (polygon.IsActive == false)
                                {
                                    logger.Warn($" ADDRESS WITH ID {Id} matched into Inactive Polygon with Id {polygon.Id}. Returning negative storeId -{StoreId}... ");
                                    StoreId = -StoreId;
                                }
                                else
                                {
                                    logger.Debug($" Address with {Id} matched into Polygon with Id {polygon.Id}. Returning StoreId {StoreId}... ");
                                }
                                model.StoreId = StoreId;
                                model.Id = polygon.Id;
                                return model;
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Error("An error occurred searching polygon '" + polygon.Name + "' with Id: " + polygon.Id + " with Address with id:" + Id.ToString() + ". Error: " + e.ToString() + Environment.NewLine + interSQL);
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση Address Model: Εάν StoreId = 0 τότε η διεύθυνση δεν ανήκει σε πολύγωνο, Αν StoreId είναι μικρότερο του 0 τότε το πολύγωνο είναι ΑΝΕΝΕΡΓΟ. 
        /// </summary>
        /// <param name="Model">Address Model</param>
        /// <returns>StoreId. (Εάν StoreId = 0 τότε η διεύθυνση δεν ανήκει σε πολύγωνο. Αν StoreId είναι μικρότερο του 0 τότε το πολύγωνο είναι ΑΝΕΝΕΡΓΟ)</returns>
        public DA_GeoPolygonsBasicModel SelectPolygonByAddressModel(DBInfoModel dbInfo, DA_AddressModel Model)
        {
            DA_GeoPolygonsBasicModel model = new DA_GeoPolygonsBasicModel();
            model.StoreId = 0;
            model.Id = 0;

            bool isInPolygon = false;
            long StoreId = 0;
            List<DA_GeoPolygonsModel> PolygonsModel = new List<DA_GeoPolygonsModel>();
            List<DA_GeoPolygonsDetailsModel> polygonDetails = new List<DA_GeoPolygonsDetailsModel>();
            float Latitude = 0;
            float Longtitude = 0;

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Latitude = (float)Model.Latitude;
                Longtitude = (float)Model.Longtitude;
                var point = new GeoCoordinate(Latitude, Longtitude);

                string getActivePolygons = @"SELECT * FROM DA_GeoPolygons";
                PolygonsModel = db.Query<DA_GeoPolygonsModel>(getActivePolygons).ToList();
                bool useGeopolygonShape = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_GeolocationUseShape");

                if (useGeopolygonShape)
                {
                    string intrSQL = CreateShapeIntersector(0, Latitude, Longtitude);
                    int polygId = db.QueryFirst<int>(intrSQL);
                    if (polygId > 0)
                    {
                        DA_GeoPolygonsModel shapePolygon = GetPolygonById(dbInfo, polygId);
                        StoreId = Convert.ToInt32(shapePolygon.StoreId);
                        if (shapePolygon.IsActive == false)
                        {
                            logger.Warn($" ADDRESS WITH ID {Model.Id} matched into Inactive Polygon with Id {shapePolygon.Id}. Returning negative storeId -{StoreId}... ");
                            StoreId = -StoreId;
                        }
                        else
                        {
                            logger.Debug($" Address with {Model.Id} matched into Polygon with Id {shapePolygon.Id}. Returning StoreId {StoreId}... ");
                        }
                        model.StoreId = StoreId;
                        model.Id = shapePolygon.Id;
                        return model;
                    }
                }
                else
                {
                    foreach (DA_GeoPolygonsModel polygon in PolygonsModel)
                    {
                        try
                        {
                            string interSQL = CreateIntersector(polygon.Id, Latitude, Longtitude);
                            
                            isInPolygon = db.QueryFirst<bool>(interSQL);

                            if (isInPolygon == true)
                            {
                                //string getStoreId = @"SELECT dgp.StoreId FROM DA_GeoPolygons AS dgp WHERE dgp.Id =@ID";
                                //StoreId = db.Query<long>(getStoreId, new { ID = polygon.Id }).FirstOrDefault();
                                StoreId = polygon.StoreId ?? 0;
                                if (polygon.IsActive == false) StoreId = -StoreId;
                                model.StoreId = StoreId;
                                model.Id = polygon.Id;
                                return model;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Get a List of Polygons (Header/details). 
        /// </summary>
        /// <returns></returns>
        public List<DA_GeoPolygonsModel> GetPolygonsList(DBInfoModel dbInfo)
        {
            List<DA_GeoPolygonsModel> polygonsModel = new List<DA_GeoPolygonsModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"DECLARE @PolygonsHeaders TABLE (PolygId BIGINT) 
                                    DECLARE @PolygonsDetails TABLE (PolygDetailId BIGINT) 

                                    INSERT INTO @PolygonsHeaders(PolygId) 
                                    SELECT Id FROM DA_GeoPolygons AS dgp

                                    INSERT INTO @PolygonsDetails(PolygDetailId) 
                                    SELECT DISTINCT dgpd.Id 
                                    FROM DA_GeoPolygonsDetails AS dgpd
                                    INNER JOIN @PolygonsHeaders p ON p.PolygId = dgpd.PolygId

                                    SELECT dgp.*, s.Title AS StoreDescr, s.RGB AS PolygonsColor, da.Latitude, da.Longtitude
                                    FROM DA_GeoPolygons AS dgp
                                    INNER JOIN @PolygonsHeaders p ON p.PolygId = dgp.Id
                                    INNER JOIN DA_Stores AS s ON s.Id = dgp.StoreId
                                    INNER JOIN DA_Addresses AS da ON s.AddressId = da.Id 
                                    WHERE da.AddressType = 2

                                    SELECT dgpd.* 
                                    FROM DA_GeoPolygonsDetails AS dgpd
                                    INNER JOIN @PolygonsDetails p ON p.PolygDetailId = dgpd.Id";

                using (var multipleResult = db.QueryMultiple(sqlData))
                {
                    polygonsModel = multipleResult.Read<DA_GeoPolygonsModel>().ToList();
                    List<DA_GeoPolygonsDetailsModel> DetailsModel = multipleResult.Read<DA_GeoPolygonsDetailsModel>().ToList();

                    foreach (DA_GeoPolygonsModel polygons in polygonsModel)
                    {
                        polygons.Details = DetailsModel.Where(p => p.PolygId == polygons.Id).Select(s => s).ToList();
                    }
                }
            }

            return polygonsModel;
        }

        /// <summary>
        /// Get a List of Polygons (Header/details). 
        /// </summary>
        /// <returns></returns>
        public List<DA_GeoPolygonsModel> GetPolygonsByStore(DBInfoModel dbInfo, long StoreId)
        {
            configHlp.CheckDeliveryAgent();
            List<DA_GeoPolygonsModel> polygonsModel = new List<DA_GeoPolygonsModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"DECLARE @PolygonsHeaders TABLE (PolygId BIGINT) 
                                    DECLARE @PolygonsDetails TABLE (PolygDetailId BIGINT) 

                                    INSERT INTO @PolygonsHeaders(PolygId) 
                                    SELECT Id FROM DA_GeoPolygons AS dgp
                                    WHERE dgp.StoreId =" + StoreId + @"

                                    INSERT INTO @PolygonsDetails(PolygDetailId) 
                                    SELECT DISTINCT dgpd.Id 
                                    FROM DA_GeoPolygonsDetails AS dgpd
                                    INNER JOIN @PolygonsHeaders p ON p.PolygId = dgpd.PolygId

                                    SELECT dgp.*, s.Title AS StoreDescr, s.RGB AS PolygonsColor, da.Latitude, da.Longtitude
                                    FROM DA_GeoPolygons AS dgp
                                    INNER JOIN @PolygonsHeaders p ON p.PolygId = dgp.Id
                                    INNER JOIN DA_Stores AS s ON s.Id = dgp.StoreId
                                    LEFT OUTER JOIN DA_Addresses AS da ON s.AddressId = da.Id 
                                    WHERE da.AddressType = 2 AND dgp.StoreId = " + StoreId + @"

                                    SELECT dgpd.* 
                                    FROM DA_GeoPolygonsDetails AS dgpd
                                    INNER JOIN @PolygonsDetails p ON p.PolygDetailId = dgpd.Id";

                using (var multipleResult = db.QueryMultiple(sqlData))
                {
                    polygonsModel = multipleResult.Read<DA_GeoPolygonsModel>().ToList();
                    List<DA_GeoPolygonsDetailsModel> DetailsModel = multipleResult.Read<DA_GeoPolygonsDetailsModel>().ToList();

                    foreach (DA_GeoPolygonsModel polygons in polygonsModel)
                    {
                        polygons.Details = DetailsModel.Where(p => p.PolygId == polygons.Id).Select(s => s).ToList();
                    }
                }
            }

            return polygonsModel;
        }

        /// <summary>
        /// Get a Polygon (Header/details) by Id. 
        /// </summary>
        /// /// <param name="Id"></param>
        /// <returns></returns>
        public DA_GeoPolygonsModel GetPolygonById(DBInfoModel dbInfo, long Id)
        {
            configHlp.CheckDeliveryAgent();
            DA_GeoPolygonsModel polygonModel = new DA_GeoPolygonsModel();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"DECLARE @PolygonsHeaders TABLE (PolygId BIGINT) 
                                    DECLARE @PolygonsDetails TABLE (PolygDetailId BIGINT) 

                                    INSERT INTO @PolygonsHeaders(PolygId) 
                                    SELECT Id FROM DA_GeoPolygons AS dgp
                                    WHERE dgp.Id =" + Id + @"

                                    INSERT INTO @PolygonsDetails(PolygDetailId) 
                                    SELECT DISTINCT dgpd.Id 
                                    FROM DA_GeoPolygonsDetails AS dgpd
                                    INNER JOIN @PolygonsHeaders p ON p.PolygId = dgpd.PolygId

                                    SELECT dgp.*, s.Title AS StoreDescr, s.RGB AS PolygonsColor, da.Latitude, da.Longtitude
                                    FROM DA_GeoPolygons AS dgp
                                    INNER JOIN @PolygonsHeaders p ON p.PolygId = dgp.Id
                                    INNER JOIN DA_Stores AS s ON s.Id = dgp.StoreId
                                    LEFT OUTER JOIN DA_Addresses AS da ON s.AddressId = da.Id 
                                    WHERE da.AddressType = 2 AND dgp.Id = " + Id + @"

                                    SELECT dgpd.* 
                                    FROM DA_GeoPolygonsDetails AS dgpd
                                    INNER JOIN @PolygonsDetails p ON p.PolygDetailId = dgpd.Id";

                using (var multipleResult = db.QueryMultiple(sqlData))
                {
                    polygonModel = multipleResult.Read<DA_GeoPolygonsModel>().FirstOrDefault();
                    List<DA_GeoPolygonsDetailsModel> DetailsModel = multipleResult.Read<DA_GeoPolygonsDetailsModel>().ToList();

                    polygonModel.Details = DetailsModel.Where(p => p.PolygId == polygonModel.Id).Select(s => s).ToList();
                    
                }
            }

            return polygonModel;
        }

        /// <summary>
        /// Insert new Polygon (Header/details). 
        /// </summary>
        /// /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertPolygon(DBInfoModel dbInfo, DA_GeoPolygonsModel Model)
        {
            configHlp.CheckDeliveryAgent();
            long PolygonId = 0;

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    string sqlInsertHeader = @"INSERT INTO DA_GeoPolygons
                                        (
	                                        StoreId,
	                                        Name,
	                                        Notes,
	                                        IsActive
                                        )
                                        VALUES
                                        (
	                                        @storeId,
	                                        @name,
	                                        @notes,
	                                        @isActive
                                        )";
                    db.Query<DA_GeoPolygonsModel>(sqlInsertHeader, 
                        new { storeId = Model.StoreId, name = Model.Name, notes = Model.Notes, isActive = Model.IsActive }).FirstOrDefault();

                    foreach (DA_GeoPolygonsDetailsModel detail in Model.Details)
                    {
                        string sqlInsertDetails = @"INSERT INTO DA_GeoPolygonsDetails
                                                    (
	                                                    PolygId,
	                                                    Latitude,
	                                                    Longtitude
                                                    )
                                                    VALUES
                                                    (
	                                                    @polygId,
	                                                    @latitude,
	                                                    @longtitude
                                                    )";
                        db.Query<DA_GeoPolygonsDetailsModel>(sqlInsertDetails,
                            new { polygId = detail.PolygId, latitude = detail.Latitude, longtitude = detail.Longtitude }).FirstOrDefault();

                    }

                    PolygonId = db.Query<long>("SELECT dgp.Id FROM DA_GeoPolygons AS dgp ORDER BY dgp.Id DESC").FirstOrDefault();

                    scope.Complete();
                }
            }

            return PolygonId;
        }

        /// <summary>
        /// Update Polygon (Header/details). 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdatePolygon(DBInfoModel dbInfo, DA_GeoPolygonsModel Model)
        {
            long PolygonId = 0;
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    //Update Header
                    string sqlUpdateHeader = @"UPDATE DA_GeoPolygons SET
	                                            StoreId =@storeId,
	                                            Name =@name,
	                                            Notes =@notes,
	                                            IsActive =@isActive";
                    db.Query<DA_GeoPolygonsModel>(sqlUpdateHeader,
                        new { storeId = Model.StoreId, name = Model.Name, notes = Model.Notes, isActive = Model.IsActive }).FirstOrDefault();

                    PolygonId = Model.Id;

                    //Delete Details
                    string sqlDeleteDetails = @"DELETE FROM DA_GeoPolygonsDetails WHERE PolygId =@polygId";
                    db.Query<DA_GeoPolygonsDetailsModel>(sqlDeleteDetails, new { polygId = Model.Id }).FirstOrDefault();

                    //Insert Details
                    foreach (DA_GeoPolygonsDetailsModel detail in Model.Details)
                    {
                        string sqlInsertDetails = @"INSERT INTO DA_GeoPolygonsDetails
                                                    (
	                                                    PolygId,
	                                                    Latitude,
	                                                    Longtitude
                                                    )
                                                    VALUES
                                                    (
	                                                    @polygId,
	                                                    @latitude,
	                                                    @longtitude
                                                    )";
                        db.Query<DA_GeoPolygonsDetailsModel>(sqlInsertDetails,
                            new { polygId = detail.PolygId, latitude = detail.Latitude, longtitude = detail.Longtitude }).FirstOrDefault();
                    }
                    scope.Complete();
                }
            }
            return PolygonId;
        }

        /// <summary>
        /// Update DA_GeoPolygons Set Notes to NUll and IsActive = true
        /// <param name="StoreId"></param>
        /// </summary>
        public long UpdateDaPolygonsNotesActives(DBInfoModel dbInfo, long StoreId)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"UPDATE DA_GeoPolygons SET Notes = NULL, IsActive = 1 WHERE StoreId = @storeId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Query<long>(SqlData, new { storeId = StoreId }).FirstOrDefault();
                res = 1;
            }
            return res;
        }

        /// <summary>
        /// Update Polygon's IsActive (true or false) by Id . 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdatePolygonStatus(DBInfoModel dbInfo, long Id, bool IsActive)
        {
            long PolygonId = 0;
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                    string sqlUpdate = @"UPDATE DA_GeoPolygons SET IsActive =@isActive WHERE Id =@ID";
                    db.Query<DA_GeoPolygonsModel>(sqlUpdate, new { isActive = IsActive, ID = Id }).FirstOrDefault();

                    PolygonId = Id;
            }

            return PolygonId;
        }

        /// <summary>
        /// Delete Polygon (Header/details). 
        /// </summary>
        /// <param name="Id">Header Id</param>
        /// <returns></returns>
        public long DeletePolygon(DBInfoModel dbInfo, long Id)
        {
            long PolygonId = 0;
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    //Delete Details
                    string sqlDeleteDetails = @"DELETE FROM DA_GeoPolygonsDetails WHERE PolygId =@polygId";
                    db.Query<DA_GeoPolygonsDetailsModel>(sqlDeleteDetails, new { polygId = Id }).FirstOrDefault();

                    //Delete Header
                    string sqlDeleteHeader = @"DELETE FROM DA_GeoPolygons WHERE Id =@ID";
                    db.Query<DA_GeoPolygonsModel>(sqlDeleteHeader, new { ID = Id }).FirstOrDefault();

                    PolygonId = Id;

                    scope.Complete();
                }
            }

            return PolygonId;
        }

        /// <summary>
        /// create the sql query to intersect a point
        /// </summary>
        /// <param name="Id">Polygon Id</param>
        /// <param name="Latitude">point Latitude</param>
        /// <param name="Longtitude">point Longtitude</param>
        /// <returns></returns>
        private string CreateIntersector(long Id, float Latitude, float Longtitude)
        {
            return $@"    DECLARE @BuildString NVARCHAR(MAX)
                    SELECT @BuildString = COALESCE(@BuildString + ',', '') + CAST([Longtitude] AS NVARCHAR(50)) + ' ' + CAST([Latitude] AS NVARCHAR(50))
                    FROM [DA_GeoPolygonsDetails] where PolygId={Id} 
                      ORDER BY Id ---desc

                    SET @BuildString = 'POLYGON((' + @BuildString + '))';  
                    --select @BuildString
                    DECLARE @PolygonFromPoints geography = geography::STPolyFromText(@BuildString, 4326).MakeValid();
                    --SELECT @PolygonFromPoints

                    DECLARE @point GEOGRAPHY = GEOGRAPHY::Point({Latitude.ToString().Replace(",",".")},{Longtitude.ToString().Replace(",", ".")}, 4326).MakeValid()

                    SELECT  @point.STIntersects(@PolygonFromPoints)";
        }

        /// <summary>
        /// create the sql query to intersect a point
        /// </summary>
        /// <param name="Id">Polygon Id</param>
        /// <param name="Latitude">point Latitude</param>
        /// <param name="Longtitude">point Longtitude</param>
        /// <returns></returns>
        private string CreateShapeIntersector(long Id, float Latitude, float Longtitude)
        {
            return $@"    
                DECLARE @point AS GEOMETRY = GEOMETRY::STGeomFromText('POINT({Longtitude.ToString().Replace(",", ".")} {Latitude.ToString().Replace(",", ".")})',4326).MakeValid()

                IF EXISTS(
	                SELECT id
	                FROM DA_GeoPolygons
	                WHERE @point.STIntersects(Shape) = 1
                )
	            SELECT id
	            FROM DA_GeoPolygons
	            WHERE @point.STIntersects(Shape) = 1
                ELSE
                SELECT 0";
        }

        /// <summary>
        /// Generate shape string from geopolygon by combining geopolygondetail points
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <returns>List<string></returns>
        public List<string> GenerateShapes(DBInfoModel DBInfo)
        {
            List<string> errors = new List<string>();

            List<DA_GeoPolygonsModel> polygonsList = GetPolygonsList(DBInfo);

            List<DA_GeoPolygonsDetailsModel> details = new List<DA_GeoPolygonsDetailsModel>();

            string queryDetails = "SELECT id, polygid, latitude, longtitude FROM DA_GeoPolygonsDetails";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                details = db.Query<DA_GeoPolygonsDetailsModel>(queryDetails).ToList();
            }
            
            foreach(long polygonId in polygonsList.Select(p => p.Id).Distinct())
            {
                List<DA_GeoPolygonsDetailsModel> storeGeopolygons = details.Where(p => p.PolygId == polygonId).ToList();

                var sb = new StringBuilder();

                sb.Append(@"POLYGON((");

                int count = 0;

                foreach (DA_GeoPolygonsDetailsModel point in storeGeopolygons)
                {
                    if (count == 0)
                    {
                        count++;

                        sb.Append(point.Longtitude + " " + point.Latitude);
                    }
                    else
                    {
                        sb.Append("," + point.Longtitude + " " + point.Latitude);
                    }

                }

                sb.Append(@"))");

                string queryShape = @"
                    DECLARE @g geometry
                    SET @g = geometry::STGeomFromText('" + sb.ToString() + @"', 4326).MakeValid();
                    UPDATE DA_GeoPolygons SET Shape = @g WHERE id = @id
                ";
                
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    try
                    {
                        db.Execute(queryShape, new { id = polygonId });
                    }
                    catch(Exception ex)
                    {
                        logger.Error("GenerateShapes: Failed to create geometry for polygon " + polygonId.ToString());

                        errors.Add("GenerateShapes: Failed to create geometry for polygon " + polygonId.ToString());
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// insert records in DA_GeopolygonDetails generated from DA_Geopolygons.Shape string
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <returns>int</returns>
        public int GenerateGeoPolygonDetailsFromShapes(DBInfoModel DBInfo)
        {
            int result = 0;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            string sql = @"
                SET NOCOUNT ON

                DECLARE @Long FLOAT, @Lat FLOAT, @Shape GEOMETRY, @i INT, @max INT, @gpolyID INT, @extPolyID NVARCHAR(50)
                DECLARE @PolygonDetails TABLE (Id INT, Long FLOAT, Lat FLOAT) 
                DECLARE @cursorID CURSOR

                SET @cursorID = CURSOR FOR 
	                SELECT d.id, d.name, d.shape 
	                FROM DA_GeoPolygons d 

                OPEN @cursorID
                FETCH NEXT
                FROM @cursorID INTO @gpolyID, @extPolyID, @Shape    
	                WHILE @@FETCH_STATUS = 0
	                BEGIN
	
		                SET @i = 1
		                SET @max = @Shape.STNumPoints()
		                WHILE @i <= @max
		                BEGIN
			                 SELECT @Long = @Shape.STPointN(@i).STX, @Lat = @Shape.STPointN(@i).STY
			                 INSERT INTO @PolygonDetails (Id, Long, Lat) VALUES (@gpolyID, @Lat, @Long)
		                     SET @i = @i + 1
		                END

		                FETCH NEXT FROM @cursorID INTO @gpolyID, @extPolyID, @Shape   
	                END
                CLOSE @cursorID
                DEALLOCATE @cursorID

                DELETE FROM DA_GeoPolygonsDetails

                INSERT INTO DA_GeoPolygonsDetails
                SELECT * FROM @PolygonDetails

                SET NOCOUNT OFF
            ";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    result = db.Execute(sql);
                }
                catch(Exception ex)
                {
                    logger.Error("Failed to generate geopolygon details from shapes");

                    logger.Error(ex.ToString());

                    return -1;
                }
            }

            return result;
        }
    }
}
