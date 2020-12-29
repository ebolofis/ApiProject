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
    public class SalesTypeDAO : ISalesTypeDAO
    {
        IGenericDAO<SalesTypeDTO> genslt;
        public SalesTypeDAO(IGenericDAO<SalesTypeDTO> _genslt)
        {
            genslt = _genslt;
        }

        /// <summary>
        /// Function uses Gen SalesTypes and Gets Sales Type by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> Sales Type  by id  </returns>
        public SalesTypeDTO SelectById(IDbConnection db, long Id)
        {
            return genslt.Select(db, Id);
        }


        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        public List<SalesTypeDTO> SelectAll(IDbConnection db, bool isDeleted = false)
        {
            string wq = " where isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                return genslt.Select(db);
            }
            else
            {
                return genslt.Select(db, wq, null);
            }
        }
    }
}
