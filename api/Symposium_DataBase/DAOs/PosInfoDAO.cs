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
    public class PosInfoDAO : IPosInfoDAO
    {
        IGenericDAO<PosInfoDTO> genPosinfo;
        public PosInfoDAO(IGenericDAO<PosInfoDTO> _genPosinfo)
        {
            genPosinfo = _genPosinfo;
        }

        /// <summary>
        /// Function uses gen PosInfo and Gets Pos info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Pos Info refered to Id asked </returns>
        public PosInfoDTO SelectById(IDbConnection db, long Id)
        {
            return genPosinfo.Select(db, Id);
        }

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        public List<PosInfoDTO> SelectAll(IDbConnection db, bool isDeleted = false)
        {
            string wq = " where isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                return genPosinfo.Select(db);
            }
            else
            {
                return genPosinfo.Select(db, wq, null);
            }
        }
    }
}
