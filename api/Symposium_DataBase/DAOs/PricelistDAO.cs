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
    public class PricelistDAO : IPricelistDAO
    {
        IGenericDAO<PricelistDTO> genPricelist;
        public PricelistDAO(IGenericDAO<PricelistDTO> _genPricelist)
        {
            genPricelist = _genPricelist;
        }

        /// <summary>
        /// Function uses genPricelist and Gets Pricelist info by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> Pricelist by id  </returns>
        public PricelistDTO SelectById(IDbConnection db, long Id)
        {
            return genPricelist.Select(db, Id);
        }

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        public List<PricelistDTO> SelectAll(IDbConnection db, bool isDeleted = false)
        {
            string wq = " where isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                return genPricelist.Select(db);
            }
            else
            {
                return genPricelist.Select(db, wq, null);
            }
        }
    }
}
