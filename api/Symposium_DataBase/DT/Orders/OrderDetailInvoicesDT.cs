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

namespace Symposium.WebApi.DataAccess.DT {

    /// <summary>
    /// Class that handles data related to Order Detail Invoices
    /// </summary>
    public class OrderDetailInvoicesDT : IOrderDetailInvoicesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<OrderDetailInvoicesDTO> orderDetailInvoicesGenericDao;

        public OrderDetailInvoicesDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<OrderDetailInvoicesDTO> orderDetailInvoicesGenericDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.orderDetailInvoicesGenericDao = orderDetailInvoicesGenericDao;
        }

        /// <summary>
        /// Gets order detail invoices of selected invoice based on InvoicesId
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to get order detail invoices from </param>
        /// <returns> List of order detail invoices. See: <seealso cref="Symposium.Models.Models.OrderDetailInvoicesModel"/> </returns>
        public List<OrderDetailInvoicesModel> GetOrderDetailInvoicesOfSelectedInvoice(DBInfoModel Store, long InvoiceId)
        {
            List<OrderDetailInvoicesDTO> orderDetailInvoices;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                orderDetailInvoices = orderDetailInvoicesGenericDao.Select(db, "where InvoicesId = @invId", new { invId = InvoiceId });
            }

            return AutoMapper.Mapper.Map<List<OrderDetailInvoicesModel>>(orderDetailInvoices);
        }

        /// <summary>
        /// Add's new record to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewOrderDetailInvoice(DBInfoModel Store, OrderDetailInvoicesModel item)
        {
            string Error;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return orderDetailInvoicesGenericDao.Upsert(db, AutoMapper.Mapper.Map<OrderDetailInvoicesDTO>(item), out Error);
            }
        }

        /// <summary>
        /// Add's new record to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewOrderDetailInvoice(IDbConnection db, OrderDetailInvoicesModel item, IDbTransaction dbTransact, out string Error)
        {
            return orderDetailInvoicesGenericDao.Upsert(db, AutoMapper.Mapper.Map<OrderDetailInvoicesDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Return's a record from OrderDetailInvoices using Parameters can use
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderDetailId"></param>
        /// <param name="ProductIngredId"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="IsExtra"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public OrderDetailInvoicesModel GetOrderDtailByOrderDetailId(DBInfoModel Store, long OrderDetailId, long ProductIngredId, long? InvoiceId,
            long? PosInfoId, long? PosInfoDetailId, bool IsExtra, out string Error)
        {
            Error = "";
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    if (IsExtra)
                        SQL = "SELECT * FROM OrderDetailInvoices WHERE OrderDetailId = @OrderDetailId AND ISNULL(OrderDetailIgredientsId,0) > 0 AND ProductId = @ProductId \n";
                    else
                        SQL = "SELECT * FROM OrderDetailInvoices WHERE OrderDetailId = @OrderDetailId AND ISNULL(OrderDetailIgredientsId,0) < 1 AND ProductId = @ProductId \n";

                    if ((InvoiceId ?? 0) > 0)
                        SQL += " AND InvoicesId = @InvoicesId";

                    if ((PosInfoId ?? 0) > 0)
                        SQL += " AND PosInfoId = @PosInfoId";

                    if ((PosInfoDetailId ?? 0) > 0)
                        SQL += " AND PosInfoDetailId = @PosInfoDetailId";

                    return AutoMapper.Mapper.Map<OrderDetailInvoicesModel>(orderDetailInvoicesGenericDao.SelectFirst(db, SQL,
                        new { OrderDetailId = OrderDetailId, ProductId = ProductIngredId, InvoicesId = InvoiceId, PosInfoId = PosInfoId, PosInfoDetailId = PosInfoDetailId }));
                }
            }
            catch(Exception ex)
            {
                Error = "GetOrderDtailByOrderDetailId [SQL: " + SQL + "] \r\n" + ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// Gets table labels of selected table for active orders
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="TableId"></param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        public List<string> GetTableLabelsInTable(DBInfoModel Store, long? TableId)
        {
            List<string> tableLables = new List<string>();
            if (TableId != null)
            {
                List<OrderDetailInvoicesDTO> activeItems;
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string whereConditions = "WHERE TableId = @tableId and ISNULL(TableLabel, '') != '' and EndOfDayId IS NULL";
                    activeItems = orderDetailInvoicesGenericDao.Select(db, whereConditions, new { tableId = TableId });
                    if (activeItems != null && activeItems.Count > 0)
                    {
                        foreach (OrderDetailInvoicesDTO item in activeItems)
                        {
                            bool labelFound = false;
                            foreach (string label in tableLables)
                            {
                                if (label.Equals(item.TableLabel))
                                {
                                    labelFound = true;
                                    break;
                                }
                            }
                            if (!labelFound)
                            {
                                tableLables.Add(item.TableLabel);
                            }
                        }
                    }
                }
            }
            return tableLables;
        }
    }
}
