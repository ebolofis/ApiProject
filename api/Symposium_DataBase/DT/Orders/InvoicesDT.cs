using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.DAOs;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Symposium.WebApi.DataAccess.DT
{

    /// <summary>
    /// Class that handles data related to Invoices
    /// </summary>
    public class InvoicesDT : IInvoicesDT
    {
        protected ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string connectionString;
        IGenericDAO<InvoicesDTO> invoiceGenericDao;
        IInvoicesDAO invoiceDao;
        //IInvoiceTypesDAO invoiceTypesDao;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<InvoiceQRDTO> invoiceQRGenericDao;

        public InvoicesDT(
            IUsersToDatabasesXML usersToDatabases, 
            IGenericDAO<InvoicesDTO> invoiceGenericDao, 
            IInvoicesDAO invoiceDao,
            IGenericDAO<InvoiceQRDTO> invoiceQRGenericDao)
        {
            this.invoiceGenericDao = invoiceGenericDao;
            this.usersToDatabases = usersToDatabases;
            this.invoiceDao = invoiceDao;
            this.invoiceQRGenericDao = invoiceQRGenericDao;
        }

        /// <summary>
        /// create aade qr code image, based on provided url and linked to provided invoiceid
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="InvoiceId">long</param>
        /// <param name="url">string</param>
        /// <returns>long?</returns>
        public long? CreateInvoiceQR(DBInfoModel DBInfo, long InvoiceId, string url)
        {
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                QRCodeHelper qrCodeHelper = new QRCodeHelper();
                Image qrCodeImage = qrCodeHelper.GenerateQRImage(url);

                if (qrCodeImage == null)
                {
                    logger.Error("Failed to convert url to QR code image");

                    return null;
                }

                byte[] qrCode = null;

                using (var ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, ImageFormat.Bmp);
                    qrCode = ms.ToArray();
                }

                if (qrCode == null)
                {
                    logger.Error("Failed to convert QR code image to byte array");

                    return null;
                }

                InvoiceQRDTO invoiceQR = new InvoiceQRDTO();
                invoiceQR.Id = 0;
                invoiceQR.InvoiceId = InvoiceId;
                invoiceQR.QR = qrCode;

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return invoiceQRGenericDao.Insert(db, invoiceQR);
                }
            }
            catch(Exception ex)
            {
                logger.Error("Failed to insert invoice QR code to database: " + ex.ToString());

                return null;
            }
        }

        /// <summary>
        /// Selects single invoice according to Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Id of invoice to fetch </param>
        /// <returns> Invoice model. See: <seealso cref="Symposium.Models.Models.InvoiceModel"/> </returns>
        public InvoiceModel GetSingleInvoice(DBInfoModel Store, long InvoiceId)
        {
            InvoicesDTO invoice;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                invoice = invoiceGenericDao.Select(db, InvoiceId);
            }

            return AutoMapper.Mapper.Map<InvoiceModel>(invoice);
        }

        /// <summary>
        /// Selects single invoice according to Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Id of invoice to fetch </param>
        /// <returns> Invoice model. See: <seealso cref="Symposium.Models.Models.InvoiceModel"/> </returns>
        public InvoiceModel GetSingleInvoice(IDbConnection db, IDbTransaction dbTransact, long InvoiceId)
        {
            return AutoMapper.Mapper.Map<InvoiceModel>(invoiceGenericDao.Select(db, InvoiceId, dbTransact));
        }

        /// <summary>
        /// Cancels receipt
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <param name="StaffId"> Staff </param>
        /// <returns> Id of new invoice inserted </returns>
        public int cancelReceipt(DBInfoModel Store, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out long NewInvoiceId)
        {
            int resultStoredProcedure;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                resultStoredProcedure = invoiceDao.cancelReceipt(db, InvoiceId, PosInfoId, StaffId, CancelOrder, out NewInvoiceId);
            }
            return resultStoredProcedure;
        }

        public long GetTicketCount(DBInfoModel store, long posInfo)
        {
            long count;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                count = invoiceDao.getTicketCount(db, posInfo);
            }
            return count;
        }

        /// <summary>
        /// Return's Inovice model using HasCode
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="HashCode"></param>
        /// <returns></returns>
        public InvoiceModel GetInvoiceByHashCode(DBInfoModel Store, string HashCode)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<InvoiceModel>(invoiceGenericDao.Select(db, "WHERE Hashcode = @Hashcode", new { Hashcode = HashCode }).FirstOrDefault());
            }
        }

        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoice(DBInfoModel Store, InvoiceModel item)
        {
            string Error;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return invoiceGenericDao.Upsert(db, AutoMapper.Mapper.Map<InvoicesDTO>(item), out Error);
            }
        }


        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewInvoice(IDbConnection db, InvoiceModel item, IDbTransaction dbTransact, out string Error)
        {
            Error = "";
            return invoiceGenericDao.Upsert(db, AutoMapper.Mapper.Map<InvoicesDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Return's all invoices using OrderId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByOrderId(IDbConnection db, long OrderId, IDbTransaction dbTransact = null)
        {
            string SQL = "SELECT DISTINCT i.* \n"
               + "FROM Invoices AS i \n"
               + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = i.Id \n"
               + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
               + "WHERE ISNULL(i.IsDeleted,0) = 0 ";
            return db.Query<InvoiceModel>(SQL, new { OrderId = OrderId }, dbTransact).ToList();
        }

        /// <summary>
        /// Return's all invoices using OrderId without transaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByOrderId(DBInfoModel Store, long OrderId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                string SQL = "SELECT DISTINCT i.* \n"
               + "FROM Invoices AS i \n"
               + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = i.Id \n"
               + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
               + "WHERE ISNULL(i.IsDeleted,0) = 0 ";
                return db.Query<InvoiceModel>(SQL, new { OrderId = OrderId }).ToList();
            }
        }

        /// <summary>
        /// Return's all invoices using List Of Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByIds(DBInfoModel Store, List<long> InvoicesId)
        {
            List<InvoiceModel> ret = new List<InvoiceModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (long item in InvoicesId)
                    ret.Add(AutoMapper.Mapper.Map<InvoiceModel>(invoiceGenericDao.SelectFirst(db, "WHERE Id = @Id", new { Id = item })));
            }
            return ret;
        }

        /// <summary>
        /// Update's the field IsPrinted for table Invoices and OrderDetailInvoices for Specific invoie Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="IsPrinted"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool UpdateInvoicePrinted(DBInfoModel Store, long InvoiceId, bool IsPrinted, out string Error)
        {
            Error = "";
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (SqlConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    SQL = "UPDATE Invoices SET IsPrinted = '" + IsPrinted + "' WHERE Id = @Id";
                    db.Execute(SQL, new { Id = InvoiceId });

                    SQL = "UPDATE OrderDetailInvoices SET IsPrinted = '" + IsPrinted + "' WHERE InvoicesId = @Id";
                    db.Execute(SQL, new { Id = InvoiceId });
                }
                return true;
            }
            catch (Exception ex)
            {
                Error = "UpdateInvoicePrinted [SQL : " + SQL + "] \r\n" + ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// Get Invoice from Old Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceWithTablesModel GetInvoiceFromOldId(DBInfoModel Store, long InvoiceId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                InvoiceModel tmpInv = AutoMapper.Mapper.Map<InvoiceModel>(invoiceGenericDao.Select(db, InvoiceId));
                return AutoMapper.Mapper.Map<InvoiceWithTablesModel>(tmpInv);
            }
        }

        /// <summary>
        /// Get Invoice from Old Invoice Id using transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceWithTablesModel GetInvoiceFromOldId(IDbConnection db, IDbTransaction dbTransaction, long InvoiceId)
        {
            InvoiceModel tmpInv = AutoMapper.Mapper.Map<InvoiceModel>(invoiceGenericDao.Select(db, InvoiceId, dbTransaction));
            return AutoMapper.Mapper.Map<InvoiceWithTablesModel>(tmpInv);
        }

    }
}
