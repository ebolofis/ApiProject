﻿using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IInvoice_Guests_TransDT
    {

        /// <summary>
        /// Return Invoice_Guests_Trans record using invoice id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        Invoice_Guests_TransModel GetInvoiceGuestByInvoiceId(DBInfoModel Store, long InvoiceId);


        /// <summary>
        /// Add new record to Invoice Guest Trnsaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewInvoiceGuestTransaction(DBInfoModel Store, Invoice_Guests_TransModel item);

        /// <summary>
        /// Delete's an Invoice_Guests_TransModel from db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int DeleteInvoiceGuestTransaction(IDbConnection db, IDbTransaction dbTransaction, Invoice_Guests_TransModel item);

        /// <summary>
        /// Add new record to Invoice Guest Trnsaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddNewInvoiceGuestTransaction(IDbConnection db, Invoice_Guests_TransModel item, IDbTransaction dbTransact, out string Error);
    }
}
