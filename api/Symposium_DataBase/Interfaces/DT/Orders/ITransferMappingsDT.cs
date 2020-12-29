using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface ITransferMappingsDT
    {
        /// <summary>
        /// Return's a Transfer Mapping Record To Create new TransfertoPMS
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosDepartmentId"></param>
        /// <param name="ProductCategoryId"></param>
        /// <param name="PriceListId"></param>
        /// <param name="HotelId"></param>
        /// <returns></returns>
        TransferMappingsModel GetTransferMappingForNewTransaction(DBInfoModel Store, long PosDepartmentId, long ProductCategoryId,
            long PriceListId, int HotelId);

        /// <summary>
        /// Return's a List of TransferMappingsModel for PosDepartmentId and HotelId if exists
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosDepartmentId"></param>
        /// <param name="HotelId"></param>
        /// <returns></returns>
        List<TransferMappingsModel> GetTransferMappingsByHotelAndDepartment(DBInfoModel Store, long PosDepartmentId, int HotelId);
    }
}
