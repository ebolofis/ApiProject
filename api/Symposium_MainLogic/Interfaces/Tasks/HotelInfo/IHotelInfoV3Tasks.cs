using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.HotelInfo
{
    public interface IHotelInfoV3Tasks
    {
        List<HotelsInfoModel> GetHotelInfo(DBInfoModel dbInfo);
        List<HotelInfoBaseModel> GetHotelInfoBase(DBInfoModel dbInfo);
        HotelsInfoModel GetHotelInfoById(DBInfoModel dbInfo, long hotelInfoId);
        List<TransferMappingsModel> GetTransferMappings(DBInfoModel DBInfo, long HotelId, long ProdCatId);
        void UpdateTransferMappings(DBInfoModel DBInfo, long newPmsDepId, string newPmsDescr, long ProdCatId, long HotelId, long OldPmsDepartmentId);
    }
}
