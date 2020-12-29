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
    public class PosInfoDetailDAO: IPosInfoDetailDAO
    {
        IGenericDAO<PosInfoDetailDTO> genpid;
        public PosInfoDetailDAO(IGenericDAO<PosInfoDetailDTO> _genpid)
        {
            genpid = _genpid;
        }

        /// <summary>
        /// Function uses gen and Gets PosInfoDetailDTO info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> PosInfoDetailDTO by id  </returns>
        public PosInfoDetailDTO SelectById(IDbConnection db, long Id)
        {
            return genpid.Select(db, Id);
        }

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        public List<PosInfoDetailDTO> SelectAll(IDbConnection db, bool isDeleted = false)
        {
            string wq = " where isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                return genpid.Select(db);
            }
            else
            {
                return genpid.Select(db, wq, null);
            }
        }
    }
}
