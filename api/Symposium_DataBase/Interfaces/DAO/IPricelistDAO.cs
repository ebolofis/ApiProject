using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IPricelistDAO
    {
        /// <summary>
        /// Function uses genPricelist and Gets Pricelist info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> Pricelist by id  </returns>
        PricelistDTO SelectById(IDbConnection db, long Id);

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        List<PricelistDTO> SelectAll(IDbConnection db, bool isDeleted = false);
    }
}
