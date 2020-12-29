using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IHotelInfoTasks
    {
        HotelsInfoModel SelectHotelInfoById(DBInfoModel store, long Id);

        HotelsInfoModel selectHotelInfoByHotelId(DBInfoModel store, long hotelId);

    }
}
