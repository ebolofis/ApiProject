using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    /// <summary>
    /// Main Logic Class that handles the TableForDisplay activities
    /// </summary>
    public class TableForDisplayFlow : ITableForDisplayFlows 
    {
        ITableForDisplayTasks tablefordisplay;
        public TableForDisplayFlow(ITableForDisplayTasks tfd)
        {
            this.tablefordisplay = tfd;
        }

        /// <summary>
        /// Return TableForDisplay Model
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public TableForDisplayPreviewModel getSigleTable(DBInfoModel dbInfo, string storeid, long tableId)
        {
            // get the results
            TableForDisplayPreviewModel tfdPreview = tablefordisplay.getSigleTable(dbInfo, storeid, tableId);

            return tfdPreview;
        }

        /// <summary>
        /// Return Kitchen Instructions for a single Table
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public KitchenInstructionPreviewModel GetKitchenInstructionsForTable(DBInfoModel dbInfo, string storeid, long tableId)
        {
            // get the results
            KitchenInstructionPreviewModel kitcheninstructions = tablefordisplay.GetKitchenInstructionsForTable(dbInfo, storeid, tableId);

            return kitcheninstructions;
        }

        /// <summary>
        /// Return Tables Per Region
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public TablesPerRegionPreviewModel GetOpenTablesPerRegionStatusOnly(DBInfoModel dbInfo, string storeid, long regionId)
        {
            // get the results
            TablesPerRegionPreviewModel TablesPerRegion = tablefordisplay.GetOpenTablesPerRegionStatusOnly(dbInfo, storeid, regionId);

            return TablesPerRegion;
        }

        /// <summary>
        /// Return all tables for a specific POS
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public GetAllTablesModelPreview GetAllTables(DBInfoModel dbInfo, string storeid, long posInfoId)
        {
            // get the results
            GetAllTablesModelPreview getAllTables = tablefordisplay.GetAllTables(dbInfo, storeid, posInfoId);

            return getAllTables;
        }
    }
}
