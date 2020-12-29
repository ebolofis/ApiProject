using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System.Collections.Generic;
using System.Data;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class VatDAO : IVatDAO
    {
        IGenericDAO<VatDTO> genvat;
        public VatDAO(IGenericDAO<VatDTO> _genvat)
        {
            genvat = _genvat;
        }
        /// <summary>
        /// Function uses Gen Vat and Gets Vats by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> Staff  by id  </returns>
        public VatDTO SelectById(IDbConnection db, long Id)
        {
            return genvat.Select(db, Id);
        }

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        public List<VatDTO> SelectAll(IDbConnection db)//, bool isDeleted = false)
        {
            return genvat.Select(db);
            //string wq = " where isnull(isdeleted ,0) = 0 ";
            //if (isDeleted == true)
            //{
            //    return genvat.Select(db);
            //}
            //else
            //{
            //    return genvat.Select(db, wq, null);
            //}
        }
    }
}
