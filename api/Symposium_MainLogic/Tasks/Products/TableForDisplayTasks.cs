using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class TableForDisplayTasks : ITableForDisplayTasks
    {
        ITableForDisplayDT tablefordisplayDB;
        public TableForDisplayTasks(ITableForDisplayDT tfdDB)
        {
            this.tablefordisplayDB = tfdDB;
        }

        /// <summary>
        ///Return TableForDisplay Model
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public TableForDisplayPreviewModel getSigleTable(DBInfoModel Store, string storeid, long tableId)
        {
            // get the results
            TableForDisplayPreviewModel tfdPreview = tablefordisplayDB.getSigleTable(Store, storeid, tableId);

            return tfdPreview;
        }

        /// <summary>
        /// Return Kitchen Instructions for a single Table
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public KitchenInstructionPreviewModel GetKitchenInstructionsForTable(DBInfoModel Store, string storeid, long tableId)
        {
            // get the results
            KitchenInstructionPreviewModel kitcheninstructions = tablefordisplayDB.GetKitchenInstructionsForTable(Store, storeid, tableId);

            return kitcheninstructions;
        }

        /// <summary>
        /// Return Tables Per Region
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public TablesPerRegionPreviewModel GetOpenTablesPerRegionStatusOnly(DBInfoModel Store, string storeid, long regionId)
        {
            // get the results
            TablesPerRegionPreviewModel TablesPerRegion = tablefordisplayDB.GetOpenTablesPerRegionStatusOnly(Store, storeid, regionId);

            return TablesPerRegion;
        }

        /// <summary>
        /// Return all tables for a specific pos
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public GetAllTablesModelPreview GetAllTables(DBInfoModel Store, string storeid, long posInfoId)
        {
            // get the results
            GetAllTablesModelPreview getAllTables = tablefordisplayDB.GetAllTables(Store, storeid, posInfoId);

            return getAllTables;
        }
    }
}
