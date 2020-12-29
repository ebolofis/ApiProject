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
    public class InvoiceTypeDAO: IInvoiceTypeDAO
    {
        IGenericDAO<InvoiceTypesDTO> genInvt;
        public InvoiceTypeDAO(IGenericDAO<InvoiceTypesDTO> _genInvt)
        {
            genInvt = _genInvt;
        }

        /// <summary>
        /// Function uses gen InvoiceTypes and Gets InvoiceType by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Invoice Types refered to Id asked </returns>
        public InvoiceTypesDTO SelectById(IDbConnection db, long Id)
        {
            return genInvt.Select(db, Id);
        }

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        public List<InvoiceTypesDTO> SelectAll(IDbConnection db, bool isDeleted = false)
        {
            string wq = " where isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                return genInvt.Select(db);
            }
            else
            {
                return genInvt.Select(db, wq, null);
            }
        }
    }
}
