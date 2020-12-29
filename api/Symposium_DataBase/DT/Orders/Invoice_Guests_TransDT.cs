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
    public class Invoice_Guests_TransDT : IInvoice_Guests_TransDT
    {
        IGenericDAO<Invoice_Guests_TransDTO> dt;
        IUsersToDatabasesXML users;
        string connectionstring;

        public Invoice_Guests_TransDT(IGenericDAO<Invoice_Guests_TransDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Return Invoice_Guests_Trans record using invoice id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public Invoice_Guests_TransModel GetInvoiceGuestByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            connectionstring = users.ConfigureConnectionString(Store);
            using(IDbConnection db = new SqlConnection(connectionstring))
            {
                return AutoMapper.Mapper.Map<Invoice_Guests_TransModel>(dt.Select(db, "WHERE InvoiceId = @InvoiceId", new { InvoiceId = InvoiceId }).FirstOrDefault());
            }
        }


        /// <summary>
        /// Add new record to Invoice Guest Trnsaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoiceGuestTransaction(DBInfoModel Store, Invoice_Guests_TransModel item)
        {
            string Error;
            connectionstring = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionstring))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<Invoice_Guests_TransDTO>(item), out Error);
            }
        }

        /// <summary>
        /// Add new record to Invoice Guest Trnsaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewInvoiceGuestTransaction(IDbConnection db, Invoice_Guests_TransModel item, IDbTransaction dbTransact, out string Error)
        {
            Error = "";
            return dt.Upsert(db, AutoMapper.Mapper.Map<Invoice_Guests_TransDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Delete's an Invoice_Guests_TransModel from db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int DeleteInvoiceGuestTransaction(IDbConnection db, IDbTransaction dbTransaction, Invoice_Guests_TransModel item)
        {
            return dt.Delete(db, dbTransaction, AutoMapper.Mapper.Map<Invoice_Guests_TransDTO>(item));
        }

    }
}
