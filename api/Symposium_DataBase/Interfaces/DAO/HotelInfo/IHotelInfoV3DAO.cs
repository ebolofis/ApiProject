using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO.HotelInfo
{
    public interface IHotelInfoV3DAO
    {
        List<HotelInfoDTO> GetHotelInfo(IDbConnection db);
        List<TransferMappingsDTO> GetTransferMappings(IDbConnection db, long HotelId, long ProdCatId);
         void UpdateTransferMappings(IDbConnection db, long newPmsDepId, string newPmsDescr, long ProdCatId, long HotelId,long OldPmsDepartmentId);
    }
}
