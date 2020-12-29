using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IInvoiceTypeDAO
    {

        /// <summary>
        /// Function uses gen InvoiceTypes and Gets InvoiceType by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Invoice Types refered to Id asked </returns>
        InvoiceTypesDTO SelectById(IDbConnection db, long Id);

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        List<InvoiceTypesDTO> SelectAll(IDbConnection db, bool isDeleted = false);
    }
}
