using Dapper;
using log4net;
using Symposium.Models.Enums;
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

namespace Symposium.WebApi.DataAccess.DT
{
    public class OrderDT : IOrderDT
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IGenericDAO<OrderDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        string connectionString;

        IOrderStatusDT orderStatusDT;
        IOrderDetailsDT orderDetDT;
        IOrderDetailIngredientsDT orderDetIngDT;
        IInvoicesDT invoiceDT;
        IInvoiceShippingDetailsDT invShippingDT;
        ITransactionsDT transactionDT;
        IInvoice_Guests_TransDT invGuestDT;
        ICreditTransactionDT credDT;
        ITransferToPmsDT transferToPmsDT;
        IOrderDetailInvoicesDT orderDetInvDT;
        IPosInfoDetailDT posInfoDetailDT;
        IPosInfoDT posInfoDT;
        IInvoiceTypesDT invType;

        public OrderDT(IGenericDAO<OrderDTO> dt, IUsersToDatabasesXML usersToDatabases,
            IOrderStatusDT orderStatusDT, IOrderDetailsDT orderDetDT,
            IOrderDetailIngredientsDT orderDetIngDT, IInvoicesDT invoiceDT,
            IInvoiceShippingDetailsDT invShippingDT, ITransactionsDT transactionDT,
            IInvoice_Guests_TransDT invGuestDT, ICreditTransactionDT credDT,
            ITransferToPmsDT transferToPmsDT, IOrderDetailInvoicesDT orderDetInvDT,
            IPosInfoDT posInfoDT, IPosInfoDetailDT posInfoDetailDT, IInvoiceTypesDT invType)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
            this.orderStatusDT = orderStatusDT;
            this.orderDetDT = orderDetDT;
            this.orderDetIngDT = orderDetIngDT;
            this.invoiceDT = invoiceDT;
            this.invShippingDT = invShippingDT;
            this.transactionDT = transactionDT;
            this.invGuestDT = invGuestDT;
            this.credDT = credDT;
            this.transferToPmsDT = transferToPmsDT;
            this.orderDetInvDT = orderDetInvDT;
            this.posInfoDT = posInfoDT;
            this.posInfoDetailDT = posInfoDetailDT;
            this.invType = invType;
        }

        /// <summary>
        /// Return's a list Of Order's using OrderDetail Ids
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderDetailsIds"></param>
        /// <returns></returns>
        public List<OrderModel> GetOrderListByOrderDetails(DBInfoModel Store, List<long> OrderDetailsIds)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "";
                string sIds = string.Join(", ", OrderDetailsIds);
                SQL = "SELECT DISTINCT OrderId FROM OrderDetail WHERE Id IN (" + sIds + ") ";
                List<long> OrderIds = db.Query<long>(SQL).ToList();
                sIds = "";
                sIds = string.Join(", ", OrderIds);
                return AutoMapper.Mapper.Map<List<OrderModel>>(dt.Select(db, "WHERE Id IN (@Id)", new { Id = sIds }));
            }
        }

        /// <summary>
        /// Return's a list Of Order's using OrderDetail Ids
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderDetailsIds"></param>
        /// <returns></returns>
        public List<OrderModel> GetOrderListByIds(DBInfoModel Store, List<long> Ids)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sIds = string.Join(", ", Ids);
                return AutoMapper.Mapper.Map<List<OrderModel>>(dt.Select(db, "WHERE Id IN (@Id)", new { Id = sIds }));
            }
        }

        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderModel GetOrderById(DBInfoModel Store, long OrderId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<OrderModel>(dt.SelectFirst(db, "WHERE ISNULL(IsDeleted,0) = 0 AND Id = @Id ", new { Id = OrderId }));
            }
        }

        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderModel GetOrderByDAId(DBInfoModel Store, long DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<OrderModel>(dt.SelectFirst(db, "WHERE ISNULL(IsDeleted,0) = 0 AND ExtKey = @DAOrderId AND ExtType = @ExtType ", new { DAOrderId = DAOrderId, ExtType = ExtType }));
            }
        }

        /// <summary>
        /// Add's new order to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewOrder(DBInfoModel Store, OrderModel item)
        {
            string Error;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<OrderDTO>(item), out Error);// dt.Insert(db, AutoMapper.Mapper.Map<OrderDTO>(item));
            }
        }

        /// <summary>
        /// Upsert's a Full Order Model Using Transaction.
        /// Roolback if not succeded.
        /// Inglude's all table 
        ///     Order
        ///     OrderDetail
        ///     OrderDetailIngredients
        ///     Invoices
        ///     OrderDetailInvoices
        ///     InvoiceShippingDetail
        ///     Transaction
        ///     Invoice_Guest_Trans
        ///     CreditTransaction
        ///     TransferToPms
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Model"></param>
        /// <param name="StoreOrderId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool UpSertFullOrderModel(DBInfoModel Store, FullOrderWithTablesModel Model, 
            out long StoreOrderId, out long OrderNo, out string Error, 
            IDbConnection dbExists = null, IDbTransaction dbTransactExist = null)
        {
            bool result = true;
            Error = "";
            StoreOrderId = -1;
            OrderNo = -1;
            long newId = 0, orderStatusOrderId = 0;
            try
            {
                IDbConnection db = null;
                if (dbExists != null)
                    db = dbExists;
                else
                {
                    connectionString = usersToDatabases.ConfigureConnectionString(Store);
                    db = new SqlConnection(connectionString);
                }
                List<OrderDetailInvoicesModel> allOrderDetailInv = new List<OrderDetailInvoicesModel>();

                using (db)
                {
                    if (db.State != ConnectionState.Open)
                        db.Open();
                    IDbTransaction dbTransact = null;
                    if (dbTransactExist != null)
                        dbTransact = dbTransactExist;
                    else
                        dbTransact = db.BeginTransaction(IsolationLevel.ReadCommitted);
                    
                    using (dbTransact)
                    {

                        if (Model != null)
                        {
                            long ReceiptNo = 0;
                            /*This code is for Transaction to lock tables*/
                            string tmpSql = "select * from  Version with(updlock)";
                            db.Query(tmpSql, null, dbTransact);
                            if (Model.OrderNo > 0 && Model.ReceiptNo > 0)
                            {
                                ReceiptNo = Model.ReceiptNo;
                                OrderNo = Model.OrderNo;
                            }
                            else
                            {
                                ReceiptNo = posInfoDetailDT.GetNextCounter(db, dbTransact, Model.Invoice[0].PosInfoId ?? 0, Model.Invoice[0].PosInfoDetailId ?? 0, Model.Invoice[0].InvoiceTypeId ?? 0);
                                OrderNo = posInfoDT.GetNextOrderNo(db, dbTransact, Model.Invoice[0].PosInfoId ?? 0, out Error);
                            }
                            //else
                            //if (Model.Invoice == null || Model.Invoice.Count < 1 || (Model.Invoice[0].Id ?? 0) == 0)
                            //{
                            //    ReceiptNo = posInfoDetailDT.GetNextCounter(db, dbTransact, Model.Invoice[0].PosInfoId ?? 0, Model.Invoice[0].PosInfoDetailId ?? 0, Model.Invoice[0].InvoiceTypeId ?? 0);
                            //    OrderNo = posInfoDT.GetNextOrderNo(db, dbTransact, Model.Invoice[0].PosInfoId ?? 0, out Error);
                            //}
                            //else
                            //{
                            //    string SQL = "SELECT * FROM [Order] WHERE Id = @Id";
                            //    OrderModel tmpOrder = db.Query<OrderModel>(SQL, new { Id = Model.Id }, dbTransact).FirstOrDefault();

                            //    if (tmpOrder != null)
                            //    {
                            //        ReceiptNo = tmpOrder.ReceiptNo;
                            //        OrderNo = tmpOrder.OrderNo;
                            //    }
                            //    else
                            //    {
                            //        ReceiptNo = posInfoDetailDT.GetNextCounter(db, dbTransact, Model.Invoice[0].PosInfoId ?? 0, Model.Invoice[0].PosInfoDetailId ?? 0, Model.Invoice[0].InvoiceTypeId ?? 0);
                            //        OrderNo = posInfoDT.GetNextOrderNo(db, dbTransact, Model.Invoice[0].PosInfoId ?? 0, out Error);
                            //    }
                            //}
                            if (ReceiptNo < 1 || OrderNo < 1)
                            {
                                Error += " \r\n RoolBack For ReceiptNo " + ReceiptNo.ToString() + " - OrderNo " + OrderNo.ToString();
                                dbTransact.Rollback();
                                return false;
                            }

                            //Create new Order
                            Model.OrderNo = OrderNo;
                            Model.ReceiptNo = ReceiptNo;
                            OrderModel addOrder = AutoMapper.Mapper.Map<OrderModel>(Model);
                            newId = dt.Upsert(db, AutoMapper.Mapper.Map<OrderDTO>(Model), dbTransact, out Error);
                            orderStatusOrderId = newId;

                            string sMess = "";
                            sMess = "Order with id " + newId.ToString() + " and externa key (Deluvery Agent Id ) " + (Model.ExtKey ?? "") + " and Invoice Id " +
                                    ((Model.Invoice != null) ? (Model.Invoice[0].Id ?? 0) : 0).ToString() + "  has Order No " + OrderNo.ToString() + " and Receipt No " + ReceiptNo.ToString();
                            logger.Info(sMess);
                            //"Order No : " + OrderNo.ToString() + " for order "+ newId.ToString()+" and External DA ID : "+ (Model.ExtKey ?? "")

                            if (OrderNo <= 2 || ReceiptNo <= 2)
                            {
                                sMess = "";
                                if (OrderNo == 1)
                                    sMess += "Order No equals 1. ";
                                if (ReceiptNo == 1)
                                    sMess += "Receipt No equals 1.";
                                sMess += " Delivery Agent with Id : " + (Model.ExtKey ?? "") + " and Store Order Id " + (Model.Id ?? 0).ToString() + " and Invoice Id : " +
                                    ((Model.Invoice != null) ? (Model.Invoice[0].Id ?? 0) : 0).ToString();
                                logger.Warn(sMess);
                            }


                            StoreOrderId = newId;
                            if (!string.IsNullOrEmpty(Error))
                            {
                                //if (dbTransactExist == null)
                                dbTransact.Rollback();
                                return false;
                            }
                            Model.Id = newId;

                            //Update all OrderDetails with new Id
                            if (Model.OrderDetails != null)
                                foreach (OrderDetailWithExtrasModel odDet in Model.OrderDetails)
                                    odDet.OrderId = newId;

                            ////Update and insert Order Status
                            //if (Model.OrderStatus != null)
                            //{
                            //    Model.OrderStatus.OrderId = newId;

                            //    OrderStatusModel ordStatusModel = orderStatusDT.GetLastOrdrStatusForOrderId(Store, newId);
                            //    if (ordStatusModel == null || (ordStatusModel != null && ordStatusModel.Status != Model.OrderStatus.Status))
                            //    {
                            //        Model.OrderStatus.Id = orderStatusDT.AddOrderStatus(db, Model.OrderStatus, dbTransact, out Error);

                            //        if (Model.OrderStatus == null || Model.OrderStatus.Id < 1)
                            //        {
                            //            //if (dbTransactExist == null)
                            //            Error += "\r\n RoolBack for No Order Status Found ";
                            //            dbTransact.Rollback();
                            //            return false;
                            //        }
                            //    }
                            //}


                            //Add all OrderDetails
                            if (Model.OrderDetails != null)
                                foreach (OrderDetailWithExtrasModel odDet in Model.OrderDetails)
                                {
                                    newId = orderDetDT.AddOrderDetail(db, AutoMapper.Mapper.Map<OrderDetailModel>(odDet), dbTransact, out Error);
                                    if (newId < 1)
                                    {
                                        //if (dbTransactExist == null)
                                        Error += " \r\n Rollback no new id returned for order detail";
                                        dbTransact.Rollback();
                                        return false;
                                    }
                                    odDet.Id = newId;

                                    //Update All OrderDetailInvoices With new OrderDetailId
                                    if (odDet.OrderDetailInvoices != null)
                                        foreach (OrderDetailInvoicesModel ordInv in odDet.OrderDetailInvoices)
                                        {
                                            ordInv.OrderNo = OrderNo;
                                            ordInv.Counter = ReceiptNo;
                                            ordInv.OrderDetailId = newId;
                                            ordInv.OrderId = Model.Id;
                                        }


                                    //Update All OrderDetailIngrendient with new OrderDetailId
                                    if (odDet.OrderDetIngrendients != null)
                                        foreach (OrderDetailIngredientsModel odIng in odDet.OrderDetIngrendients)
                                            odIng.OrderDetailId = newId;

                                    //Add all OrderDetailIngrendients
                                    if (odDet.OrderDetIngrendients != null)
                                        foreach (OrderDetailIngredientsModel odIng in odDet.OrderDetIngrendients)
                                        {
                                            newId = orderDetIngDT.AddOrderDetailIngredints(db, odIng, dbTransact, out Error);
                                            if (newId < 1)
                                            {
                                                //if (dbTransactExist == null)
                                                Error += "\r\n no new id returned for order detail extras";
                                                dbTransact.Rollback();
                                                return false;
                                            }
                                            odIng.Id = newId;
                                            var fld = odDet.OrderDetailInvoices.Find(f => f.IngredientId == odIng.IngredientId && f.OrderDetailIgredientsId == null);
                                            if (fld != null)
                                                fld.OrderDetailIgredientsId = newId;
                                        }
                                }


                            //Create new Invoice
                            if (Model.Invoice != null)
                                foreach (InvoiceWithTablesModel inv in Model.Invoice)
                                {
                                    inv.OrderNo = OrderNo.ToString();
                                    inv.Counter = (int)ReceiptNo;
                                    newId = invoiceDT.AddNewInvoice(db, inv, dbTransact, out Error);

                                    if (newId < 1)
                                    {
                                        //if (dbTransactExist == null)
                                        Error += "\r\n rollback no new id returned for invoice";
                                        dbTransact.Rollback();
                                        return false;
                                    }
                                    inv.Id = newId;
                                    InvoiceTypeModel invTypeModel = db.Query<InvoiceTypeModel>("SELECT * FROM InvoiceTypes WHERE Id = @Id", new { Id = inv.InvoiceTypeId }, dbTransact).FirstOrDefault();

                                    //Update all Order Detail Invoices
                                    if (Model.OrderDetails != null)
                                        foreach (OrderDetailWithExtrasModel odDet in Model.OrderDetails)
                                        {
                                            if (odDet.OrderDetailInvoices != null)
                                                foreach (OrderDetailInvoicesModel odInv in odDet.OrderDetailInvoices)
                                                {
                                                    odInv.InvoicesId = newId;
                                                    odInv.PosInfoId = inv.PosInfoId;
                                                    odInv.InvoiceType = invTypeModel.Type;
                                                    odInv.Abbreviation = invTypeModel.Abbreviation;
                                                    allOrderDetailInv.Add(odInv);
                                                }
                                        }

                                    //Add Invoice Shipping Detail
                                    if (inv.InvoiceShippings != null)
                                        foreach (InvoiceShippingDetailsModel invShp in inv.InvoiceShippings)
                                        {
                                            invShp.InvoicesId = inv.Id;
                                            invShp.Id = invShippingDT.AddNewInvoiceShippindDetail(db, invShp, dbTransact, out Error);
                                            if (invShp.Id < 1)
                                            {
                                                //if (dbTransactExist == null)
                                                Error += "\r\n Rollback not new id returned from invoice shipping detail";
                                                dbTransact.Rollback();
                                                return false;
                                            }
                                        }

                                    //Update transations with new Invoiceid
                                    if (inv.Transactions != null)
                                        foreach (TransactionsExtraModel transModel in inv.Transactions)
                                        {
                                            transModel.InvoicesId = inv.Id;

                                            //add new transaction
                                            TransactionsModel sendTransation = AutoMapper.Mapper.Map<TransactionsModel>(transModel);
                                            transModel.Id = transactionDT.AddNewTransaction(db, sendTransation, dbTransact, out Error);
                                            if (transModel.Id < 1)
                                            {
                                                //if (dbTransactExist == null)
                                                Error += "\r\n Rollback no new id returned for transaction";
                                                dbTransact.Rollback();
                                                return false;
                                            }

                                            //Add Record's for Invoice Guest Transaction
                                            if (transModel != null && transModel.InvoiceGuest != null)
                                                foreach (Invoice_Guests_TransModel invGuest in transModel.InvoiceGuest)
                                                {
                                                    invGuest.InvoiceId = inv.Id;
                                                    invGuest.TransactionId = transModel.Id;
                                                    invGuest.Id = invGuestDT.AddNewInvoiceGuestTransaction(db, invGuest, dbTransact, out Error);
                                                    if (invGuest.Id < 1)
                                                    {
                                                        //if (dbTransactExist == null)
                                                        Error += "\r\n Rollback no new id returned for Invoice Guest Transaction";
                                                        dbTransact.Rollback();
                                                        return false;
                                                    }
                                                }

                                            //Add Record's to Credit Transactions
                                            if (transModel != null && transModel.CreditTransaction != null)
                                                foreach (CreditTransactionsModel ctModel in transModel.CreditTransaction)
                                                {
                                                    ctModel.InvoiceId = inv.Id ?? 0;
                                                    ctModel.TransactionId = transModel.Id;
                                                    ctModel.Id = credDT.AddNewCreditTransaction(db, ctModel, dbTransact, out Error);
                                                    if (ctModel.Id < 1)
                                                    {
                                                        //if (dbTransactExist == null)
                                                        Error += " \r\n Rollback no new id returned for Credit Transaction";
                                                        dbTransact.Rollback();
                                                        return false;
                                                    }
                                                }

                                            //Add Record's for Transfer To Pms
                                            if (transModel != null && transModel.TransferToPms != null)
                                                foreach (TransferToPmsModel trToPms in transModel.TransferToPms)
                                                {
                                                    trToPms.TransactionId = transModel.Id;
                                                    trToPms.Id = transferToPmsDT.AddNewTransferToPms(db, trToPms, dbTransact, out Error);
                                                    if (trToPms.Id < 1)
                                                    {
                                                        //if (dbTransactExist == null)
                                                        Error += "\r\n Rollback no new id returned for Transfer to Pms";
                                                        dbTransact.Rollback();
                                                        return false;
                                                    }
                                                }
                                        }
                                }

                            foreach (OrderDetailInvoicesModel item in allOrderDetailInv)
                            {
                                item.Id = orderDetInvDT.AddNewOrderDetailInvoice(db, item, dbTransact, out Error);
                                if (item.Id < 1)
                                {
                                    //if (dbTransactExist == null)
                                    Error += "\r\n Rollback no new id returned for Order detail invoices";
                                    dbTransact.Rollback();
                                    return false;
                                }
                            }

                        }
                        else
                        {
                            Error = "The order model is null";
                            dbTransact.Rollback();
                            result = false;
                        }

                        if (result)
                        {
                            //Update and insert Order Status
                            if (Model.OrderStatus != null)
                            {
                                Model.OrderStatus.OrderId = orderStatusOrderId;

                                OrderStatusModel ordStatusModel = orderStatusDT.GetLastOrdrStatusForOrderId(Store, orderStatusOrderId);
                                if (ordStatusModel == null || (ordStatusModel != null && ordStatusModel.Status != Model.OrderStatus.Status))
                                {
                                    Model.OrderStatus.Id = orderStatusDT.AddOrderStatus(db, Model.OrderStatus, dbTransact, out Error);

                                    if (Model.OrderStatus == null || Model.OrderStatus.Id < 1)
                                    {
                                        //if (dbTransactExist == null)
                                        Error += "\r\n RoolBack for No Order Status Found ";
                                        dbTransact.Rollback();
                                        return false;
                                    }
                                }
                            }
                        }
                        //if (dbTransactExist == null)
                        dbTransact.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Error = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// Return's InvoiceIs using OrderId and Tables OrderDetail and OrderDetailInvoices
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public long GetInvoiceIdByOrderId(DBInfoModel Store, long OrderId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                StringBuilder SQL = new StringBuilder();
                SQL.Append("SELECT DISTINCT odi.InvoicesId \n"
                       + "FROM OrderDetail AS od \n"
                       + "INNER JOIN OrderDetailInvoices AS odi ON odi.OrderDetailId = od.Id \n"
                       + "WHERE od.OrderId = @OrderId");
                return db.Query<long>(SQL.ToString(), new { OrderId = OrderId }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Return's all isdelayed orders to send to kintchen if EstTakeoutDate if 10 min less 
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<OrderModel> GetDelayedOrders(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int status = (int)OrderStatusEnum.Preparing;
                //string SQL = "SELECT o.EstTakeoutDate, os.[Status] \n"
                //           + "FROM [Order] AS o \n"
                //           + "CROSS APPLY( \n"
                //           + "	SELECT TOP 1 * \n"
                //           + "	FROM OrderStatus AS os \n"
                //           + "	WHERE os.OrderId = o.Id AND os.ExtState = o.ExtType \n"
                //           + "	ORDER BY os.TimeChanged DESC	 \n"
                //           + ") os \n"
                //           + "WHERE CAST(CONVERT(VARCHAR(10), ISNULL(o.EstTakeoutDate,'1900-01-01'), 120) AS DATETIME) >= CAST(CONVERT(VARCHAR(10), GETDATE(), 120) AS DATETIME)  AND \n"
                //           + "	ISNULL(o.IsDelay,0) <> 0 AND os.[Status] = @Status ";
                string SQL = "SELECT o.EstTakeoutDate, os.[Status], o.Id, o.StaffId, o.PosId, o.ExtKey, o.ExtType \n"
                           + "FROM [Order] AS o \n"
                           + "CROSS APPLY( \n"
                           + "	SELECT TOP 1 os.* \n"
                           + "	FROM OrderStatus AS os \n"
                           + "CROSS APPLY ( \n"
                           + "		SELECT MAX(ost.TimeChanged) TimeChanged \n"
                           + "		FROM OrderStatus AS ost	 \n"
                           + "		WHERE ost.OrderId = o.Id \n"
                           + "	) ost \n"
                           + "	WHERE ost.TimeChanged = os.TimeChanged AND os.OrderId = o.Id AND os.ExtState = o.ExtType AND os.[Status] = 0 \n"
                           + "	ORDER BY os.TimeChanged DESC	 \n"
                           + ") os \n"
                           + "WHERE ISNULL(o.IsDelay,0) <> 0";
                return AutoMapper.Mapper.Map<List<OrderModel>>(dt.Select(SQL, new { Status = status }, db));
            }

        }

        /// <summary>
        /// Delete's all records for specific Order (OrderDetail,OrderDetailIgredients,OrderDetailInvoices)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="OrderId"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool DeleteOrderItemsForUpdate(IDbConnection db, IDbTransaction dbTransact, long OrderId, ref List<long> InvoiceId, out string Error)
        {
            string SQL = "";
            Error = "";
            try
            {
                SQL = "SELECT DISTINCT odi.InvoicesId  \n"
                   + "FROM OrderDetailInvoices AS odi  \n"
                   + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                   + "WHERE ISNULL(odi.IsDeleted,0) = 0 ";
                InvoiceId = db.Query<long>(SQL, new { OrderId = OrderId }, dbTransact).ToList();

                //Delete all records from OrderDetailInvoices
                SQL = "DELETE odi \n"
                   + "FROM OrderDetailInvoices AS odi  \n"
                   + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                   + "WHERE ISNULL(odi.IsDeleted,0) = 0 ";
                db.Execute(SQL, new { OrderId = OrderId }, dbTransact);

                //Delete all records from OrderDetailIgredients
                SQL = "DELETE odi \n"
                   + "FROM OrderDetailIgredients AS odi \n"
                   + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                   + "WHERE ISNULL(odi.IsDeleted,0) = 0 ";
                db.Execute(SQL, new { OrderId = OrderId }, dbTransact);

                //Delete All records from OrderDetail
                SQL = "DELETE FROM OrderDetail WHERE ISNULL(IsDeleted,0) = 0 AND OrderId = @OrderId";
                db.Execute(SQL, new { OrderId = OrderId }, dbTransact);

                return true;
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// Return's a list of invoice ids for specific order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<long> GetInvoiceIdsForOrderId(DBInfoModel Store, long OrderId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "SELECT DISTINCT odi.InvoicesId  \n"
                        + "FROM OrderDetailInvoices AS odi  \n"
                        + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                        + "WHERE ISNULL(odi.IsDeleted,0) = 0 ";
                return db.Query<long>(SQL, new { OrderId = OrderId }).ToList();
            }
        }

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public int CanCancelOrDeleteState(DBInfoModel Store, long OrderId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "IF EXISTS(SELECT i.Id  \n"
                          + "	FROM Invoices AS i \n"
                          + "	INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = i.Id \n"
                          + "	INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId  \n"
                          + "	INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId AND it.[Type] != 2) \n"
                          + "	SELECT 1 nCnt \n"
                          + "ELSE \n"
                          + "	SELECT 0 nCnt";
                return db.Query<int>(SQL, new { OrderId = OrderId }).FirstOrDefault();

            }
        }

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public int CanCancelOrDeleteState(DBInfoModel Store, long DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "IF EXISTS(SELECT i.Id  \n"
                          + "	FROM Invoices AS i \n"
                          + "	INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = i.Id \n"
                          + "	INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId \n"
                          + "   INNER JOIN [Order] o ON o.Id = od.OrderId AND o.ExtKey = @ExtKey AND o.ExtType = @ExtType \n"
                          + "	INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId AND it.[Type] != 2) \n"
                          + "	SELECT 1 nCnt \n"
                          + "ELSE \n"
                          + "	SELECT 0 nCnt";
                return db.Query<int>(SQL, new { ExtKey = DAOrderId, ExtType = ExtType }).FirstOrDefault();

            }
        }

        /// <summary>
        /// Check if order is already canceled
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool CheckIfOrderIsCanceled(DBInfoModel Store, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType, out string Error)
        {
            Error = "";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                OrderModel order = new OrderModel();
                if ((OrderId ?? 0) > 0)
                    order = GetOrderById(Store, OrderId ?? 0);
                else
                    order = GetOrderByDAId(Store, DAOrderId ?? 0, ExtType);
                if (order == null)
                {
                    Error = "Order is already canceled";
                    return true;
                }
                else
                    return order.IsDeleted ?? false;

            }
        }

        /// <summary>
        /// Return's an Order with all associated tables such as OrderDetail, OrderIngredients, OrderDetailInvoices......
        /// The result is an FullOrderWithTablesModel same to Post new Order Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public FullOrderWithTablesModel GetFullOrderModel(DBInfoModel Store, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            FullOrderWithTablesModel result = new FullOrderWithTablesModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                StringBuilder SQL = new StringBuilder();

                SQL.Append("DECLARE @OrderId BIGINT \n"
                           + " \n"
                           + "IF(" + (OrderId ?? 0).ToString() + " > 0) \n"
                           + "  SELECT @OrderId = o.Id FROM [Order] AS o WHERE o.ID = " + (OrderId ?? 0).ToString() + "  \n"
                           + "ELSE \n"
                           + "  SELECT @OrderId = o.Id FROM [Order] AS o WHERE o.ExtKey = '" + (DAOrderId ?? 0).ToString() + "' AND o.ExtType = " + ((int)ExtType).ToString() + "  \n"
                           + " \n"
                           + "SELECT *  \n"
                           + "FROM [Order] AS o \n"
                           + "WHERE o.Id = @OrderId \n"
                           + " \n"
                           + "SELECT *  \n"
                           + "FROM OrderDetail AS od  \n"
                           + "WHERE od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT odi.*  \n"
                           + "FROM OrderDetailIgredients AS odi \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT odi.*  \n"
                           + "FROM OrderDetailInvoices AS odi \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT isd.* \n"
                           + "FROM InvoiceShippingDetails AS isd \n"
                           + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = isd.InvoicesId \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT t.* \n"
                           + "FROM Transactions AS t \n"
                           + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = t.InvoicesId \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT igt.Id \n"
                           + "FROM Invoice_Guests_Trans AS igt \n"
                           + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = igt.InvoiceId \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT ct.* \n"
                           + "FROM CreditTransactions AS ct \n"
                           + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = ct.InvoiceId \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT ttp.Id, ttp.RegNo, ttp.PmsDepartmentId, ttp.[Description], \n"
                           + "ttp.ProfileId, ttp.ProfileName, ttp.TransactionId, ttp.TransferType, \n"
                           + "ttp.RoomId, ttp.RoomDescription, ttp.ReceiptNo, ttp.PosInfoDetailId, \n"
                           + "ttp.SendToPMS, ttp.Total, ttp.SendToPmsTS, ttp.ErrorMessage, \n"
                           + "ttp.PmsResponseId, ttp.TransferIdentifier, ttp.PmsDepartmentDescription, \n"
                           + "ttp.[Status], ttp.PosInfoId, ttp.EndOfDayId, ttp.HotelId, ttp.IsDeleted, \n"
                           + "ttp.Points, ttp.PMSPaymentId, ttp.PMSInvoiceId, ttp.InvoiceId, \n"
                           + "cast(ttp.HtmlReceipt AS  VARBINARY(8000))  HtmlReceipt \n"
                           + "FROM TransferToPms AS ttp \n"
                           + "INNER JOIN Transactions AS t ON t.Id = ttp.TransactionId \n"
                           + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = t.InvoicesId \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId \n"
                           + " \n"
                           + "SELECT DISTINCT i.*  \n"
                           + "FROM Invoices AS i \n"
                           + "INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = i.Id \n"
                           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = @OrderId");
                var multyQry = db.QueryMultiple(SQL.ToString());

                OrderModel mainOrder = multyQry.Read<OrderModel>().FirstOrDefault();
                List<OrderDetailModel> orderDet = multyQry.Read<OrderDetailModel>().ToList();
                List<OrderDetailIngredientsModel> orderIngred = multyQry.Read<OrderDetailIngredientsModel>().ToList();
                List<OrderDetailInvoicesModel> orderInv = multyQry.Read<OrderDetailInvoicesModel>().ToList();
                List<InvoiceShippingDetailsModel> invShp = multyQry.Read<InvoiceShippingDetailsModel>().ToList();
                List<TransactionsModel> transct = multyQry.Read<TransactionsModel>().ToList();
                List<Invoice_Guests_TransModel> invGuest = multyQry.Read<Invoice_Guests_TransModel>().ToList();
                List<CreditTransactionsModel> credTr = multyQry.Read<CreditTransactionsModel>().ToList();
                List<TransferToPmsModel> transfToPms = multyQry.Read<TransferToPmsModel>().ToList();
                List<InvoiceModel> inv = multyQry.Read<InvoiceModel>().ToList();

                result = AutoMapper.Mapper.Map<FullOrderWithTablesModel>(mainOrder);
                result.OrderDetails = new List<OrderDetailWithExtrasModel>();
                result.OrderDetails = AutoMapper.Mapper.Map<List<OrderDetailWithExtrasModel>>(orderDet);

                foreach (OrderDetailWithExtrasModel item in result.OrderDetails)
                {
                    item.OrderDetIngrendients = new List<OrderDetailIngredientsModel>();
                    item.OrderDetIngrendients.AddRange(orderIngred.FindAll(f => f.OrderDetailId == item.Id));

                    item.OrderDetailInvoices = new List<OrderDetailInvoicesModel>();
                    item.OrderDetailInvoices.AddRange(orderInv.FindAll(f => f.OrderDetailId == item.Id));
                }

                result.Invoice = new List<InvoiceWithTablesModel>();
                result.Invoice = AutoMapper.Mapper.Map<List<InvoiceWithTablesModel>>(inv);
                foreach (InvoiceWithTablesModel item in result.Invoice)
                {
                    item.InvoiceShippings = new List<InvoiceShippingDetailsModel>();
                    item.InvoiceShippings.AddRange(invShp.FindAll(f => f.InvoicesId == item.Id));

                    List<TransactionsExtraModel> tr = new List<TransactionsExtraModel>();
                    tr.AddRange(AutoMapper.Mapper.Map<List<TransactionsExtraModel>>(transct.FindAll(f => f.InvoicesId == item.Id)));

                    foreach (TransactionsExtraModel trModel in tr)
                    {
                        trModel.InvoiceGuest = new List<Invoice_Guests_TransModel>();
                        trModel.InvoiceGuest.AddRange(invGuest.FindAll(f => f.TransactionId == trModel.Id && f.InvoiceId == item.Id));

                        trModel.CreditTransaction = new List<CreditTransactionsModel>();
                        trModel.CreditTransaction.AddRange(credTr.FindAll(f => f.InvoiceId == item.Id && f.TransactionId == trModel.Id));

                        trModel.TransferToPms = new List<TransferToPmsModel>();
                        trModel.TransferToPms.AddRange(transfToPms.FindAll(f => f.TransactionId == trModel.Id));
                    }
                }
            }
            return result;
        }

    }
}
