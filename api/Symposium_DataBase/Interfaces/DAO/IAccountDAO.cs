using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IAccountDAO
    {
        /// <summary>
        /// Function uses gen AccountDAO instance and Gets Pos info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Account refered to Id asked </returns>
        AccountDTO SelectById(IDbConnection db, long Id);

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        List<AccountDTO> SelectAll(IDbConnection db, bool isDeleted = false);
    }
}
