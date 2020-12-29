using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface ITableForDisplayTasks 
    {
        /// <summary>
        /// Get table details
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        TableForDisplayPreviewModel getSigleTable(DBInfoModel Store, string storeid, long tableId);

        /// <summary>
        /// Get Kitchen Instructions for a single Table
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        KitchenInstructionPreviewModel GetKitchenInstructionsForTable(DBInfoModel Store, string storeid, long tableId);

        /// <summary>
        /// Get Tables Per Region
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        TablesPerRegionPreviewModel GetOpenTablesPerRegionStatusOnly(DBInfoModel Store, string storeid, long regionId);

        /// <summary>
        /// Get all tables for a specific pos
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        GetAllTablesModelPreview GetAllTables(DBInfoModel Store, string storeid, long posInfoId);
    }
}
