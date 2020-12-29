using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IProductDAO
    {
        /// <summary>
        /// Function uses gen PosInfo and Gets Pos info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Pos Info refered to Id asked </returns>
        ProductDTO SelectById(IDbConnection db, long Id);

        /// <summary>
        /// Function uses gen Product and Gets First Product by code provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="code"></param>
        /// <param name="isDeleted">If set to true then it returns collection from all</param>
        /// <returns></returns>
        ProductDTO SelectByCode(IDbConnection db, string code, bool isDeleted = false);
    }
}
