using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_GeoPolygonsFlows : IDA_GeoPolygonsFlows
    {
        IDA_GeoPolygonsTasks polygonsTasks;
        IStaffTasks staffTasks;
        public DA_GeoPolygonsFlows(IDA_GeoPolygonsTasks _polygonsTasks, IStaffTasks staffTasks)
        {
            this.polygonsTasks = _polygonsTasks;
            this.staffTasks = staffTasks;
        }

        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση AddressId
        /// </summary>
        /// <param name="Id">AddressId</param>
        /// <returns>StoreId (Εάν StoreId = 0 Τότε το κατάστημα δεν ανήκει στο πολύγωνο)</returns>
        public DA_GeoPolygonsBasicModel SelectPolygonByAddressId(DBInfoModel dbInfo, long Id)
        {
            return polygonsTasks.SelectPolygonByAddressId(dbInfo, Id);
        }

        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση Address Model
        /// </summary>
        /// <param name="Model">Address Model</param>
        /// <returns>StoreId (Εάν StoreId = 0 Τότε το κατάστημα δεν ανήκει στο πολύγωνο)</returns>
        public DA_GeoPolygonsBasicModel SelectPolygonByAddressModel(DBInfoModel dbInfo, DA_AddressModel Model)
        {
            return polygonsTasks.SelectPolygonByAddressModel(dbInfo, Model);
        }

        /// <summary>
        /// Get a List of Polygons (Header/details). 
        /// </summary>
        /// <returns></returns>
        public List<DA_GeoPolygonsModel> GetPolygonsList(DBInfoModel dbInfo)
        {
            return polygonsTasks.GetPolygonsList(dbInfo);
        }

        /// <summary>
        /// Get a List of Polygons (Header/details) by StoreId. 
        /// </summary>
        /// /// <param name="StoreId"></param>
        /// <returns></returns>
        public List<DA_GeoPolygonsModel> GetPolygonsByStore(DBInfoModel dbInfo, long StoreId)
        {
            return polygonsTasks.GetPolygonsByStore(dbInfo, StoreId);
        }

        /// <summary>
        /// Get a Polygon (Header/details) by Id. 
        /// </summary>
        /// /// <param name="Id"></param>
        /// <returns></returns>
        public DA_GeoPolygonsModel GetPolygonById(DBInfoModel dbInfo, long Id)
        {
            return polygonsTasks.GetPolygonById(dbInfo, Id);
        }

        /// <summary>
        /// Insert new Polygon (Header/details). 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertPolygon(DBInfoModel dbInfo, DA_GeoPolygonsModel Model)
        {
            return polygonsTasks.InsertPolygon(dbInfo, Model);
        }

        /// <summary>
        /// Update Polygon (Header/details). 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdatePolygon(DBInfoModel dbInfo, DA_GeoPolygonsModel Model)
        {
            return polygonsTasks.UpdatePolygon(dbInfo, Model);
        }

        /// <summary>
        /// Update DA_GeoPolygons Set Notes to NUll and IsActive = true
        /// <param name="StoreId"></param>
        /// </summary>
        public long UpdateDaPolygonsNotesActives(DBInfoModel dbInfo, long StaffId)
        {
            //1. Get store Id (if there is no store then throw exception)
            long daStoreId = staffTasks.GetDaStore(dbInfo, StaffId);
            if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);

            return polygonsTasks.UpdateDaPolygonsNotesActives(dbInfo, daStoreId);
        }

        /// <summary>
        /// Update Polygon's IsActive (true or false) by Id . 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdatePolygonStatus(DBInfoModel dbInfo, long Id, bool IsActive)
        {
            return polygonsTasks.UpdatePolygonStatus(dbInfo, Id, IsActive);
        }

        /// <summary>
        /// Delete Polygon (Header/details). 
        /// </summary>
        /// <param name="Id">Header Id</param>
        /// <returns></returns>
        public long DeletePolygon(DBInfoModel dbInfo, long Id)
        {
            return polygonsTasks.DeletePolygon(dbInfo, Id);
        }

        /// <summary>
        /// Generate geometry shape value for each store polygon based on polygon details
        /// </summary>
        public List<string> GenerateShapes(DBInfoModel DBInfo)
        {
            return polygonsTasks.GenerateShapes(DBInfo);
        }

        /// <summary>
        /// Insert records in DA_GeopolygonDetails generated from DA_Geopolygons.Shape string
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <returns>int</returns>
        public int GenerateGeoPolygonDetailsFromShapes(DBInfoModel DBInfo)
        {
            return polygonsTasks.GenerateGeoPolygonDetailsFromShapes(DBInfo);
        }
    }
}
