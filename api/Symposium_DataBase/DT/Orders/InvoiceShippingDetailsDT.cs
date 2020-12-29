using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class InvoiceShippingDetailsDT : IInvoiceShippingDetailsDT
    {
        IGenericDAO<InvoiceShippingDetailsDTO> dt;
        IUsersToDatabasesXML users;
        string connectonString;

        public InvoiceShippingDetailsDT(IGenericDAO<InvoiceShippingDetailsDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Add's new Record To db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoiceShippindDetail(DBInfoModel Store, InvoiceShippingDetailsModel item)
        {
            string Error;
            connectonString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectonString))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<InvoiceShippingDetailsDTO>(item), out Error);
            }
        }

        /// <summary>
        /// Add's new Record To db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewInvoiceShippindDetail(IDbConnection db, InvoiceShippingDetailsModel item, IDbTransaction dbTransact, out string Error)
        {
            Error = "";
            return dt.Upsert(db, AutoMapper.Mapper.Map<InvoiceShippingDetailsDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Return's a record using InvoiceId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceShippingDetailsModel GetInvShippingByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            connectonString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectonString))
            {
                return AutoMapper.Mapper.Map<InvoiceShippingDetailsModel>(dt.SelectFirst(db, "WHERE InvoicesId = @InvoicesId", new { InvoicesId = InvoiceId }));
            }
        }
    }
}
