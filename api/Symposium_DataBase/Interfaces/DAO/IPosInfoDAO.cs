using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IPosInfoDAO
    {
        /// <summary>
        /// Function uses gen PosInfo and Gets Pos info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Pos Info refered to Id asked </returns>
        PosInfoDTO SelectById(IDbConnection db, long Id);


        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        List<PosInfoDTO> SelectAll(IDbConnection db, bool isDeleted = false);
    }
}
