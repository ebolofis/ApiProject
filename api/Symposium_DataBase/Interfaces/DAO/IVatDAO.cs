using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IVatDAO
    {
        /// <summary>
        /// Function uses Gen Vat and Gets Vats by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> Staff  by id  </returns>
        VatDTO SelectById(IDbConnection db, long Id);


        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        List<VatDTO> SelectAll(IDbConnection db);
    }
}
