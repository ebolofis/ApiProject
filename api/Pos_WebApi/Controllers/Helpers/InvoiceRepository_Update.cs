using Pos_WebApi.Hubs;
using Pos_WebApi.Models;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Helpers
{
    public partial class InvoiceRepository : IDisposable
    {


        /// <summary>
        /// Provided a receipt based on posinfoid checks connected users for SignalR
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="extecr"></param>
        /// <returns></returns>
        public bool CheckConnectedExtecr(Receipts receipt, out string extecr)
        {
            try
            {
                var pi = db.PosInfo.Find(receipt.PosInfoId);
                extecr = pi.FiscalName;
                GroupedConnectionMapping connectedUsers = new GroupedConnectionMapping();// System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\signalRConnections.xml");
                string allUsers = connectedUsers.GetAllConnections();
                List<string> users = allUsers.Split(',').ToList<string>();
                foreach (var u in users)
                {
                    string name = u.Trim();
                    if (name.Equals(extecr))
                    {
                        return true;
                    }
                }
                logger.Error(Environment.NewLine+ Environment.NewLine+"                   [ERROR]         ExtECR '" + extecr + "' is NOT connected"+ Environment.NewLine+ Environment.NewLine);
                return false;
            }
            catch (Exception ex)
            {
                extecr = "";
                logger.Error("Error checking ExtECR '" + extecr + "' connectivity: "+ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Increase PosInfo.ReceiptCount and PosInfoDetail.Counter.
        /// Return: OrderNo = increased ReceiptCount, ReceiptNo = increased Counter
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns> { ReceiptNo, OrderNo } </returns>
        public dynamic UpdateInvoiceCounters(Receipts receipt)
        {
            lock (this)
            {
                long ReceiptNo = 0; long OrderNo = 0;
                PosEntities dbnew = new PosEntities(false);
                dbnew.Database.Connection.ConnectionString = db.Database.Connection.ConnectionString;
                using (var dbContextTransaction = dbnew.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // Used to lock database in order to avoid same Receipt Counter and Order Number
                        dbnew.Database.ExecuteSqlCommand( @"select * from  Version with(updlock)");

                        var pid = dbnew.PosInfoDetail.Include(i => i.PosInfo).FirstOrDefault(x => x.Id == receipt.PosInfoDetailId);
                        if (pid != null)
                        {
                            var pids = dbnew.PosInfoDetail.Where(w => w.PosInfoId == pid.PosInfoId && w.GroupId == pid.GroupId);
                            var newGroupCounter = pids.Max(mx => mx.Counter) + 1;
                            if (receipt.ModifyOrderDetails == (int)ModifyOrderDetailsEnum.FromScratch)
                                pids.FirstOrDefault().PosInfo.ReceiptCount++;

                            foreach (var p in pids)
                            {
                                p.Counter = newGroupCounter;
                                dbnew.Entry(p).State = EntityState.Modified;
                            }

                            if (dbnew.SaveChanges() > 0)
                            {
                                ReceiptNo = pid.Counter.Value;
                                if (receipt.ModifyOrderDetails == (int)ModifyOrderDetailsEnum.FromScratch)
                                    OrderNo = pid.PosInfo.ReceiptCount.Value;
                            }
                        }
                        dbContextTransaction.Commit();
                        logger.Info("PosInfoDetailId =" + receipt.PosInfoDetailId + ", PosInfoId = " + pid.PosInfoId + ", InvoiceId =" + receipt.Id + ", OrderNo = " + OrderNo + ", ReceiptNo = " + ReceiptNo);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        logger.Error(ex.ToString());
                    }
                }
                return new { ReceiptNo = ReceiptNo, OrderNo = OrderNo };
            }
        }

        /// <summary>
        /// Provide a Receipt Id and a ReceiptNo
        /// Updates Receipt.Counter , OrderDetailInvoices.Counter and Transactions.ReceiptNO 
        /// refered to specified ReceiptID and modifies entities on db Context
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="receiptNo"></param>
        /// <param name="isPrinted"></param>
        /// <returns></returns>
        public bool UpdateCountersFromFiscal(long receiptId, long receiptNo, bool isPrinted, string extecrCode)
        {
            var currentReceipt = db.Invoices.Include(i => i.OrderDetailInvoices).FirstOrDefault(x => x.Id == receiptId);
            if (currentReceipt != null)
            {
                try
                {
                    var oldCounter = currentReceipt.Counter;
                    currentReceipt.Counter = (int)receiptNo;
                    currentReceipt.IsPrinted = isPrinted;
                    currentReceipt.ExtECRCode = extecrCode;
                    foreach (var odi in currentReceipt.OrderDetailInvoices)
                    {
                        odi.Counter = receiptNo;
                        odi.IsPrinted = isPrinted;
                        db.Entry(odi).State = EntityState.Modified;
                    }
                    db.Entry(currentReceipt).State = EntityState.Modified;

                    var ttpms = db.Transactions.Include(i => i.TransferToPms).Where(w => w.InvoicesId == receiptId).SelectMany(sm => sm.TransferToPms);
                    foreach (var t in ttpms)
                    {
                        if (t.Status == 2)
                            t.SendToPMS = true;
                        t.ReceiptNo = receiptNo.ToString();
                        t.Description = t.Description.Replace(oldCounter.ToString(), receiptNo.ToString());
                        db.Entry(t).State = EntityState.Modified;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Changes by table found on provided ID
        /// OrderDetailInvoices , OrderDetails
        /// Logic Action to Move customer or order to an other Table
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <param name="newTableId"></param>
        /// <param name="receiptId"></param>
        /// <returns> true  if table found and action performed </returns>
        public bool ChangeItemTable(IEnumerable<ReceiptDetails> receiptDetails, long newTableId, long? receiptId = null)
        {
            var newtblModel = db.Table.FirstOrDefault(x => x.Id == newTableId);
            if (newtblModel != null)
            {
                var rdIds = receiptDetails.Select(s => s.OrderDetailId).ToList();
                var odis = db.OrderDetailInvoices.Where(w => rdIds.Contains(w.OrderDetailId));
                var ods = db.OrderDetail.Where(u => rdIds.Contains(u.Id));
                if (receiptId != null)
                    odis = odis.Where(w => w.InvoicesId == receiptId);
                try
                {
                    foreach (var odi in odis)
                    {
                        odi.RegionId = newtblModel.RegionId;
                        odi.TableId = newTableId;
                        odi.TableCode = newtblModel.Code;
                        db.Entry(odi).State = EntityState.Modified;
                    }
                    foreach (var od in ods)
                    {
                        od.TableId = newTableId;
                        db.Entry(od).State = EntityState.Modified;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates Staff Id on OrderDetailInvoices selected by provided Ids on receipt details
        /// Gets Receipt Details and newStaffId and foreach orderdetial invoices changes staffid and modifies entity on db Context
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <param name="newStaffId"></param>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        public bool ChangeItemWaiter(IEnumerable<ReceiptDetails> receiptDetails, long newStaffId, long? receiptId = null)
        {
            var rdIds = receiptDetails.Select(s => s.OrderDetailId).ToList();
            var odis = db.OrderDetailInvoices.Where(w => rdIds.Contains(w.OrderDetailId));
            if (receiptId != null)
                odis = odis.Where(w => w.InvoicesId == receiptId);
            try
            {
                foreach (var odi in odis)
                {
                    odi.StaffId = newStaffId;
                    db.Entry(odi).State = EntityState.Modified;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Updates OrderDetailInvoices and Transactions based on Receipt-Invoice Id
        /// </summary>
        /// <param name="r">Receipt as inVoice</param>
        /// <param name="newStaffId"></param>
        /// <returns> bool True if and invoices and staff exist </returns>
        public bool ChangeReceiptWaiter(Receipts r, long newStaffId)
        {
            try
            {
                var newStaff = db.Staff.SingleOrDefault(x => x.Id == newStaffId);
                var currentInvoice = db.Invoices.Include(i => i.OrderDetailInvoices).FirstOrDefault(x => x.Id == r.Id);
                if (newStaff != null && currentInvoice != null)
                {
                    foreach (var odi in currentInvoice.OrderDetailInvoices)
                    {
                        odi.StaffId = newStaffId;
                        db.Entry(odi).State = EntityState.Modified;
                    }
                    currentInvoice.StaffId = newStaffId;
                    db.Entry(currentInvoice).State = EntityState.Modified;

                    var trans = db.Transactions.Where(w => w.InvoicesId == r.Id);
                    foreach (var t in trans)
                    {
                        t.StaffId = newStaffId;
                        db.Entry(t).State = EntityState.Modified;
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public HotelInfo HotelInfo(HotelInfo h)
        {
            HotelInfo hotelI = new HotelInfo();
            PosInfo pos = new PosInfo();
            if (h.Type == 4)
            {
                hotelI = db.HotelInfo.FirstOrDefault();
                logger.Info("Select HotelId : " + hotelI.HotelId);
            }
            else
            {
                if (pos.DefaultHotelId != null)
                {
                    hotelI = db.HotelInfo.FirstOrDefault(x => x.HotelId == pos.DefaultHotelId);
                    logger.Info("Select HotelId : " + hotelI.HotelId);
                }
                else
                {
                    hotelI = db.HotelInfo.FirstOrDefault();
                    logger.Info("Select HotelId : " + hotelI.HotelId);
                }
            }
            return hotelI;
        }

        /// <summary>
        /// Checks if invoiceID is paid 
        /// if all items are paid then status = 2  and if not get all transactions
        /// and if Transaction amount provided + paid transactions are bigger than inv total then returns true
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="transAmount"></param>
        /// <returns></returns>
        public bool CheckIfPaid(long invoiceId, decimal transAmount)
        {
            var inv = db.Invoices.Include(i => i.Transactions).FirstOrDefault(w => w.Id == invoiceId);
            if (inv != null)
            {
                if (inv.IsPaid == 2)
                {
                    return true;
                }
                else
                {
                    var allreadyPaid = inv.Transactions.Sum(sm => sm.Amount) ?? 0;
                    if (inv.Total < allreadyPaid + transAmount)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if there is an invoice on DB with the same hashcode
        /// HashCode is unique on posinfo , posinfodetail, groupid, total,staff, foreach details description qty
        /// </summary>
        /// <param name="hash">String unique based on id and day</param>
        /// <returns></returns>
        public bool HashCodeExists(string hash)
        {
            var hashcode = MD5Helper.GetMD5Hash(hash);
            var res = db.Invoices.FirstOrDefault(x => x.Hashcode == hashcode);
            return res != null;
        }

        /// <summary>
        /// Provide a ReceiptDetail to Create an OrderDetail
        /// StatusTS refered on delivery status changed marked by DateTime.now 
        /// </summary>
        /// <param name="rd"></param>
        /// <returns></returns>
        private OrderDetail CreateOrderDetail(ReceiptDetails rd)
        {
            //όλα τα OrderDetail που δεν ανήκουν σε κουζίνα (KdsId = null) πάνε KitchenStatus έτοιμα τα υπόλοιπα pending  
            int kitchenStatus = 0;
            if(rd.KdsId == null)
            {
                kitchenStatus = (int)KdsKitchenStatusEnums.Ready;
            }
            else
            {
                kitchenStatus = (int)KdsKitchenStatusEnums.Pending;
            }
            OrderDetail od = new OrderDetail
            {
                PaidStatus = rd.PaidStatus,
                Status = rd.Status,
                TableId = rd.TableId,
                ProductId = rd.ProductId,
                PriceListDetailId = rd.PriceListDetailId,
                PreparationTime = rd.PreparationTime,
                Discount = rd.ItemDiscount,
                Guid = rd.Guid,
                KdsId = rd.KdsId,
                KitchenId = rd.KitchenId,
                Qty = rd.ItemQty,
                TotalAfterDiscount = rd.ItemGross,
                StatusTS = DateTime.Now,
                SalesTypeId = rd.SalesTypeId,
                Price = rd.Price,
                KitchenStatus = kitchenStatus
            };
            if (rd.OtherDiscount != null)
            {
                od.OtherDiscount = (short)rd.OtherDiscount;
            }
            return od;
        }


        /// <summary>
        /// Kds import  
        /// Provide OrderDetails Array and Datetime to update all Status and Pending Qty
        /// </summary>
        /// <param name="orderdetails"></param>
        /// <param name="newdt"></param>
        /// <returns></returns>
        public bool KdsUpdateOrderDetails(IEnumerable<OrderDetail> orderdetails, DateTime newdt)
        {
            List<OrderDetail> orderdetailsList = new List<OrderDetail>();
            try
            {
                foreach (OrderDetail odkds in orderdetails)
                {
                    OrderDetail ord = db.OrderDetail.Where(w => w.Id == odkds.Id).FirstOrDefault();
                    ord.Status = odkds.Status;
                    ord.PendingQty = odkds.PendingQty;
                    ord.StatusTS = newdt;
                    db.Entry(ord).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
            return true;
        }


        // ------------------------------------ Unused ------------------------------------
        /// <summary>
        /// Not used not refed pages and rows of Invoices by POS
        /// </summary>
        /// <param name="posid"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public dynamic GetInvoicesByPos(long posid, int page, int rows)
        {
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            var query = from q in db.Invoices
                        join s in db.Staff on q.StaffId equals s.Id
                        join t in db.Table on q.TableId equals t.Id into f
                        from tbl in f.DefaultIfEmpty()
                        join it in db.InvoiceTypes on q.InvoiceTypeId equals it.Id
                        where (q.IsDeleted == null || q.IsDeleted == false) && q.PosInfoId == posid
                        select new
                        {
                            Id = q.Id,
                            EndOfDayId = q.EndOfDayId ?? 0,
                            Day = q.Day,
                            OrderNo = q.OrderNo,
                            ReceiptNo = q.Counter,
                            Cover = q.Cover,
                            Discount = q.Discount,
                            Total = q.Total,
                            InvoiceTypeId = q.InvoiceTypeId,
                            IsPrinted = q.IsPrinted ?? false,
                            IsVoided = q.IsVoided ?? false,
                            TableId = q.TableId,
                            TableCode = q.TableId != null ? tbl.Code : "",
                            PaidTotal = q.PaidTotal,
                            PaymentsDesc = q.PaymentsDesc,
                            Room = q.Rooms,
                            StaffCode = s.Code,
                            StaffName = s.FirstName,
                            StaffLastName = s.LastName,
                            InvoiceTypeType = it.Type,
                            Abbreviation = it.Abbreviation,
                        };
            var extras = query.GroupBy(g => g.InvoiceTypeType).Select(s => new
            {
                InvoiceType = s.Key,
                Abbr = s.FirstOrDefault().Abbreviation,
                Amount = s.Sum(sm => sm.Total),
                Count = s.Count()
            }).ToList();

            var result = new PagedResult<dynamic>
            {
                Extras = extras,
                CurrentPage = page,
                PageSize = rows,
                RowCount = query.Count()
            };
            var pageCount = (double)result.RowCount / rows;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page) * rows;

            var paginationHeader = new
            {
                TotalPages = pageCount,
                PageSize = rows,
            };
            var query1 = query.OrderBy(o => o.Day).Skip(skip).Take(rows).ToList();
            return new { paginationHeader, query1 };

        }

        /// <summary>
        /// Based on Table.Id changes waiter on receipt details 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="newTableId"></param>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        public bool ChangeReceiptTable(Receipts r, long newTableId, long? receiptId = null)
        {
            try
            {
                var newTable = db.Table.SingleOrDefault(x => x.Id == newTableId);
                if (newTable != null)
                {
                    var detChanged = ChangeItemWaiter(r.ReceiptDetails, newTableId, r.Id);
                    var currentInvoice = db.Invoices.FirstOrDefault(x => x.Id == r.Id);
                    if (detChanged)
                    {
                        currentInvoice.StaffId = newTableId;
                    }
                    else
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
        }
    }
}