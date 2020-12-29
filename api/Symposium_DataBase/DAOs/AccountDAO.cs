using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class AccountDAO : IAccountDAO
    {
        IGenericDAO<AccountDTO> genacc;
        public AccountDAO(IGenericDAO<AccountDTO> _genacc)
        {
            genacc = _genacc;
        }

        /// <summary>
        /// Function uses gen AccountDAO instance and Gets Pos info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Account refered to Id asked </returns>
        public AccountDTO SelectById(IDbConnection db, long Id)
        {
            return genacc.Select(db, Id);
        }

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public List<AccountDTO> SelectAll(IDbConnection db, bool isDeleted = false)
        {
            string wq = " where isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                return genacc.Select(db);
            }
            else
            {
                return genacc.Select(db, wq, null);
            }
        }
    }
}
