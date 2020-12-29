using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_GeoPolygonsFlows
    {
        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση AddressId
        /// </summary>
        /// <param name="Id">AddressId</param>
        /// <returns>StoreId (Εάν StoreId = 0 Τότε το κατάστημα δεν ανήκει στο πολύγωνο)</returns>
        DA_GeoPolygonsBasicModel SelectPolygonByAddressId(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση Address Model
        /// </summary>
        /// <param name="Model">Address Model</param>
        /// <returns>StoreId (Εάν StoreId = 0 Τότε το κατάστημα δεν ανήκει στο πολύγωνο)</returns>
        DA_GeoPolygonsBasicModel SelectPolygonByAddressModel(DBInfoModel dbInfo, DA_AddressModel Model);

        /// <summary>
        /// Get a List of Polygons (Header/details). 
        /// </summary>
        /// <returns></returns>
        List<DA_GeoPolygonsModel> GetPolygonsList(DBInfoModel dbInfo);

        /// <summary>
        /// Get a List of Polygons (Header/details) by StoreId. 
        /// </summary>
        /// /// <param name="StoreId"></param>
        /// <returns></returns>
        List<DA_GeoPolygonsModel> GetPolygonsByStore(DBInfoModel dbInfo, long StoreId);

        /// <summary>
        /// Update DA_GeoPolygons Set Notes to NUll and IsActive = true
        /// <param name="StoreId"></param>
        /// </summary>
        long UpdateDaPolygonsNotesActives(DBInfoModel dbInfo, long StaffId);

        /// <summary>
        /// Get a Polygon (Header/details) by Id. 
        /// </summary>
        /// /// <param name="Id"></param>
        /// <returns></returns>
        DA_GeoPolygonsModel GetPolygonById(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Insert new Polygon (Header/details). 
        /// </summary>
        /// /// <param name="Model"></param>
        /// <returns></returns>
        long InsertPolygon(DBInfoModel dbInfo, DA_GeoPolygonsModel Model);

        /// <summary>
        /// Update Polygon (Header/details). 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long UpdatePolygon(DBInfoModel dbInfo, DA_GeoPolygonsModel Model);

        /// <summary>
        /// Update Polygon's IsActive (true or false) by Id . 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long UpdatePolygonStatus(DBInfoModel dbInfo, long Id, bool IsActive);

        /// <summary>
        /// Delete Polygon (Header/details). 
        /// </summary>
        /// <param name="Id">Header Id</param>
        /// <returns></returns>
        long DeletePolygon(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Generate geometry shape value for each store polygon based on polygon details
        /// </summary>
        List<string> GenerateShapes(DBInfoModel DBInfo);

        /// <summary>
        /// Insert records in DA_GeopolygonDetails generated from DA_Geopolygons.Shape string
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <returns>int</returns>
        int GenerateGeoPolygonDetailsFromShapes(DBInfoModel DBInfo);
    }
}
