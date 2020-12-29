using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IHotelInfoDT
    {
        HotelsInfoModel selectHotelInfoById(DBInfoModel store, long Id);

        HotelsInfoModel selectHotelInfoByHotelId(DBInfoModel store, long hotelId);

        /// <summary>
        /// Get's first record from hotelInfo table
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        HotelsInfoModel SelectFirstHotelInfo(DBInfoModel store);
    }
}
