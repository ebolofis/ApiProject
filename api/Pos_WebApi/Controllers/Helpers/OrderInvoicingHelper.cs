using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Models;
using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Helpers
{
    public class OrderInvoicingHelper:IDisposable
    {
        PosEntities db;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public OrderInvoicingHelper(PosEntities context )
        {
            db = context;
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        public string InvoiceOrder(OrderDetailUpdateObjList OrderDet, bool updaterange, int type)
        {

            var detailsforUpdate = OrderDet.OrderDet.Select(s => new
            {
                Guid = s.Guid,
                OrderDetailId = s.Id,
                InvoiceCounter = s.invoiceCounter,
                VoidCounter = s.OrderDetailInvoices != null ? s.OrderDetailInvoices.LastOrDefault().Counter : -1,
                PosInfoDetailId = s.OrderDetailInvoices != null ? s.OrderDetailInvoices.LastOrDefault().PosInfoDetailId : -1,
                IsPrinted = s.OrderDetailInvoices != null ? s.OrderDetailInvoices.LastOrDefault().IsPrinted : false,
                NewPrlId = s.NewPrlId,
                OrderNo = s.OrderNo,
                PaidStatus = s.PaidStatus,
                StaffId = s.StaffId,
                Status = s.Status,
                StatusTS = s.StatusTS,
                TotalAfterDiscount = s.TotalAfterDiscount,
                AccountId = s.AccountId,
                DiscountRemark = s.DiscountRemark,
                Discount = s.Discount,
                PosInfoId = s.PosId,
                TableDiscount = s.TableDiscount,
                GuestId = s.GuestId

            });

            var detailsForInvoice = detailsforUpdate.Where(w => w.InvoiceCounter != null);
            var detailsForPayOff = detailsforUpdate.Where(w => w.InvoiceCounter == null);
            var detailsForVoid = detailsforUpdate.Where(w => w.Status == 5);
            decimal discountPercentange = 0;
            if (detailsForInvoice.FirstOrDefault() != null && detailsForInvoice.FirstOrDefault().TableDiscount != null)
            {
                List<Int64> refIds = detailsForInvoice.Select(s => s.OrderDetailId).ToList();
                discountPercentange = CalculateTableDiscountPercentage(refIds, detailsForInvoice.FirstOrDefault().TableDiscount.Value);
            }

            AccountsObj defaultAccountObj = new AccountsObj()
            {
                AccountId = detailsforUpdate.FirstOrDefault().AccountId.Value,
                GuestId = detailsforUpdate.FirstOrDefault().GuestId,
            };
            switch ((OrderDetailUpdateType)type)
            {
                case OrderDetailUpdateType.Kds: ;
                    break;
                case OrderDetailUpdateType.InvoiceOnly:
                    InvoiceOrder(detailsForInvoice, type, discountPercentange);
                    break;
                case OrderDetailUpdateType.PayOff:
                    if (detailsForInvoice.Count() > 0)
                    {
                        var res = InvoiceOrder(detailsForInvoice, type, discountPercentange);
                        foreach (var itemToPay in res)
                        {
                            InvoiceOrderPayment(itemToPay.Key as Invoices, itemToPay.Value as List<Int64>, OrderDet.AccountsObj, type, defaultAccountObj);
                        }
                    }
                    if (detailsForPayOff.Count() > 0)
                    {
                        InvoicedOrderPayment(detailsForPayOff, OrderDet.AccountsObj, type, defaultAccountObj);
                    }

                    break;
                case OrderDetailUpdateType.PaidCancel:
                    if (detailsForVoid.Count() > 0)
                    {
                        VoidInvoicedOrder(detailsForVoid);
                    }
                    break;
                case OrderDetailUpdateType.UnPaidCancel:
                default:
                    break;
            }

            //  throw new System.ArgumentException("Parameter cannot be null", "original");
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return ex.ToString();// Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //return "";


            //temp return 
            var listupdate1 = OrderDet.OrderDet.Select(s => new
            {
                // AA = s.AA,
                AccountId = s.AccountId,
                Customer = s.Customer,
                Id = s.Id,
                Guid = s.Guid,
                //IsOffline = s.IsOffline,
                OrderNo = s.OrderNo,
                PaidStatus = s.PaidStatus,
                PosId = s.PosId,
                StaffId = s.StaffId,
                Status = s.Status,
                StatusTS = s.StatusTS,
                OrderDetailInvoices = s.OrderDetailInvoices != null ? s.OrderDetailInvoices.Select(ss => new
                {
                    Counter = ss.Counter,
                    CreationTS = ss.CreationTS,
                    CustomerId = ss.CustomerId,
                    IsPrinted = ss.IsPrinted,
                    OrderDetailId = ss.OrderDetailId,
                    PosInfoDetailId = ss.PosInfoDetailId,
                    StaffId = ss.StaffId
                }) : null
            });
            return JsonConvert.SerializeObject(listupdate1);//Request.CreateResponse(HttpStatusCode.Created, OrderDet.OrderDet);
        }
        private decimal CalculateTableDiscountPercentage(List<Int64> refIds, decimal discount)
        {
            decimal total = GetInvoiceOrderTotal(refIds);
            decimal percentage = 0;
            return percentage = (discount / total);
        }

        /// <summary>
        /// Sums total for OrderDetail and OrderDetailIngredients for the correct Invoice Total
        /// </summary>
        /// <param name="refIds"></param>
        /// <returns></returns>
        private decimal GetInvoiceOrderTotal(List<Int64> refIds)
        {
            Decimal? tot = db.OrderDetail.Where(w => refIds.Contains(w.Id)).Sum(sm => sm.TotalAfterDiscount);
            Decimal? tot1 = db.OrderDetailIgredients.Where(w => refIds.Contains(w.OrderDetailId.Value)).Sum(sm => sm.TotalAfterDiscount) ?? 0;
            return (tot + tot1) ?? 0;
        }

        /// <summary>
        /// Updates OrderDetail with the correct Status, Creates Invoice and OrderDetailInvoice
        /// </summary>
        /// <param name="details">The OrderDetails to invoice</param>
        /// <param name="type"></param>
        /// <param name="discountPercentange">applies discount to items and extras </param>
        /// <returns>A dictionary with the Invoices and OrderDetailIds</returns>
        private Dictionary<Invoices, List<Int64>> InvoiceOrder(IEnumerable<dynamic> details, Int32 type, decimal discountPercentange)
        {

            Dictionary<Invoices, List<Int64>> invs = new Dictionary<Invoices, List<Int64>>();
            //     var validIds = details.AsEnumerable<dynamic>().Select(s => new { Id = s.OrderDetailId }).ToList<Int64>();
            //     var ods = db.OrderDetail.Where(w=>validIds.Contains(w.Id));
            var detailsByInvoice = details.GroupBy(g => new { g.InvoiceCounter, g.PosInfoDetailId }).Select(s => new
            {
                InvoiceCounter = s.FirstOrDefault().InvoiceCounter,
                PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                Total = s.Sum(sm => (Decimal?)sm.TotalAfterDiscount),
                PosInfoId = s.FirstOrDefault().PosInfoId,
                StaffId = s.FirstOrDefault().StaffId,
                IsPrinted = s.FirstOrDefault().IsPrinted,
                Details = s.Select(ss => new
                {
                    Guid = ss.Guid,
                    OrderDetailId = ss.OrderDetailId,
                    NewPrlId = ss.NewPrlId,
                    OrderNo = ss.OrderNo,
                    PaidStatus = ss.PaidStatus,
                    StaffId = ss.StaffId,
                    Status = ss.Status,
                    StatusTS = ss.StatusTS,
                    TotalAfterDiscount = ss.TotalAfterDiscount,
                    AccountId = ss.AccountId,
                    DiscountRemark = ss.DiscountRemark,
                    Discount = ss.Discount
                })
            });
            foreach (var i in detailsByInvoice)
            {
                List<Int64> odIds = i.Details.Select(s => (Int64)s.OrderDetailId).ToList<Int64>();
                Decimal invoiceTotal = GetInvoiceOrderTotal(odIds);
                Invoices invoiceToCreate = new Invoices();
                invoiceToCreate.Counter = (Int32)i.InvoiceCounter;
                invoiceToCreate.PosInfoId = (Int64)i.PosInfoId;
                invoiceToCreate.PosInfoDetailId = (Int64)i.PosInfoDetailId;
                invoiceToCreate.Total = invoiceTotal;
                invoiceToCreate.IsPrinted = i.IsPrinted;
                invoiceToCreate.IsVoided = false;
                invoiceToCreate.IsDeleted = false;
                invoiceToCreate.StaffId = (Int64)i.StaffId;
                invoiceToCreate.DiscountRemark = i.Details.FirstOrDefault().DiscountRemark;
                invoiceToCreate.Day = i.Details.FirstOrDefault().StatusTS;
                PosInfoDetail pid = db.PosInfoDetail.Where(w => w.Id == invoiceToCreate.PosInfoDetailId).FirstOrDefault();
                if (pid.Counter < invoiceToCreate.Counter)
                    pid.Counter = invoiceToCreate.Counter;
                invoiceToCreate.Description = pid.Description;
                invoiceToCreate.InvoiceTypeId = pid.InvoicesTypeId;
                invoiceToCreate.Discount = discountPercentange > 0 ? (Decimal?)0 : null;

                foreach (var o in i.Details)
                {
                    var id = (Int64)o.OrderDetailId;
                    OrderDetail upd = db.OrderDetail.Where(w => w.Id == id).FirstOrDefault();

                    upd.PaidStatus = type == (int)OrderDetailUpdateType.PayOff ? (byte)2 : (byte)1;
                    upd.Status = o.Status;
                    upd.StatusTS = o.StatusTS;
                    invoiceToCreate.TableId = upd.TableId;
                    if (discountPercentange > 0)
                    {
                        decimal dis = upd.TotalAfterDiscount.Value * discountPercentange;
                        upd.Discount += dis;
                        upd.TotalAfterDiscount -= dis;
                        invoiceToCreate.Discount += dis;
                        invoiceToCreate.Total -= dis;
                        OrderDetailVatAnal odva = db.OrderDetailVatAnal.Where(w => w.OrderDetailId == upd.Id).FirstOrDefault();
                        if (odva != null)
                        {
                            odva.Gross = upd.TotalAfterDiscount;
                            odva.VatAmount = odva.VatAmount * discountPercentange;
                            odva.TaxAmount = odva.TaxAmount * discountPercentange ?? 0;
                            odva.Net = odva.Gross - odva.TaxAmount - odva.VatAmount;
                        }
                    }
                    OrderDetailInvoices odi = new OrderDetailInvoices();
                    odi.PosInfoDetailId = i.PosInfoDetailId;
                    odi.OrderDetailId = o.OrderDetailId;
                    odi.Invoices = invoiceToCreate;
                    odi.Counter = i.InvoiceCounter;
                    odi.StaffId = o.StaffId;
                    odi.CreationTS = o.StatusTS;
                    odi.IsDeleted = false;
                    odi.IsPrinted = false;

                    db.OrderDetailInvoices.Add(odi);

                    foreach (var ing in db.OrderDetailIgredients.Where(w => w.OrderDetailId == upd.Id))
                    {
                        if (discountPercentange > 0)
                        {
                            decimal dis = ing.TotalAfterDiscount.Value * discountPercentange;
                            ing.Discount += dis;
                            ing.TotalAfterDiscount -= dis;
                            invoiceToCreate.Discount += dis;
                            invoiceToCreate.Total -= dis;
                            OrderDetailIgredientVatAnal odva = db.OrderDetailIgredientVatAnal.Where(w => w.OrderDetailIgredientsId == ing.Id).FirstOrDefault();
                            if (odva != null)
                            {
                                odva.Gross = ing.TotalAfterDiscount;
                                odva.VatAmount = odva.VatAmount * discountPercentange;
                                odva.TaxAmount = odva.TaxAmount * discountPercentange ?? 0;
                                odva.Net = odva.Gross - odva.TaxAmount - odva.VatAmount;
                            }
                        }
                    }
                };
                db.Invoices.Add(invoiceToCreate);
                invs.Add(invoiceToCreate, odIds);
            }
            return invs;

        }

        private Transactions InvoiceTransaction(Invoices invoice, AccountsObj accObj, Int64? departmentId, int type)
        {
            Transactions tr = new Transactions();
            tr.Guid = Guid.NewGuid();
            tr.AccountId = accObj.AccountId;
            tr.PosInfoId = invoice.PosInfoId;
            tr.Amount = accObj.Amount;
            tr.Day = DateTime.Now;
            tr.DepartmentId = departmentId;
            tr.Description = type == (int)OrderDetailUpdateType.PayOff ? "Pay Off" : "Cancel receipt";
            tr.ExtDescription = "Helper Controller";
            tr.InOut = type == (int)OrderDetailUpdateType.PayOff ? (short)0 : (short)1;
            if (invoice.Id == 0)
                tr.Invoices = invoice;
            else
                tr.InvoicesId = invoice.Id;

            tr.StaffId = invoice.StaffId;
            tr.TransactionType = type == (int)OrderDetailUpdateType.PayOff ? (short)TransactionTypesEnum.Sale : (short)TransactionTypesEnum.Cancel;

            if (type == (int)OrderDetailUpdateType.PayOff)
            {
                if (accObj.GuestId != null)
                {
                    Invoice_Guests_Trans igt = new Invoice_Guests_Trans();
                    igt.GuestId = accObj.GuestId;
                    igt.Invoices = invoice;
                    tr.Invoice_Guests_Trans.Add(igt);
                }
            }
            return tr;
        }

        /// <summary>
        /// Pay off for new invoiced Items
        /// </summary>
        /// <param name="invoice">Invoice to pay</param>
        /// <param name="orderDetailIds">Items to pay</param>
        /// <param name="accObj">Total payment object</param>
        /// <param name="type"></param>
        /// <param name="defaultAccountId">used when accountObj is null. usually the firstOrDefault of orderdet list</param>
        private void InvoiceOrderPayment(Invoices invoice, List<Int64> orderDetailIds, IEnumerable<AccountsObj> accObj, int type, AccountsObj defaultAccountObj)
        {
            var totalToPay = GetInvoiceOrderTotal(orderDetailIds);//invoice.Total.Value;
            if (invoice.Id != 0) // Check for other transactions / partial payments
            {
                var trans = db.Transactions.Where(w => w.InvoicesId == invoice.Id).Sum(sm => sm.Amount);
            }
            if (totalToPay > invoice.Total)
                totalToPay = invoice.Total.Value;
            Int64? depId = db.PosInfo.Where(w => w.Id == invoice.PosInfoId).FirstOrDefault().DepartmentId;
            List<AccountsObj> payments = new List<AccountsObj>();
            if (accObj == null)
            {
                defaultAccountObj.Amount = totalToPay;
                payments.Add(defaultAccountObj);
            }
            else
                payments = Economic.AmountSharing(totalToPay, accObj);
            foreach (var pm in payments)
            {
                invoice.Transactions.Add(InvoiceTransaction(invoice, pm, depId, type));
            }
        }

        /// <summary>
        /// Pay off for invoiced items
        /// </summary>
        /// <param name="detailsForPayOff">items to pay</param>
        /// <param name="accObj">Total payment object</param>
        /// <param name="type"></param>
        /// <param name="defaultAccountId">used when accountObj is null. usually the firstOrDefault of orderdet list</param>
        private void InvoicedOrderPayment(IEnumerable<dynamic> detailsForPayOff, IEnumerable<AccountsObj> accObj, int type, AccountsObj defaultAccountObj)
        {
            Dictionary<Int64, List<Int64>> validLists = new Dictionary<Int64, List<Int64>>();


            var invLista = detailsForPayOff.Select(ss => ss.OrderDetailId);
            foreach (var item in invLista)
            {
                Int64 id = item;
                OrderDetail odet = db.OrderDetail.Where(w => w.Id == id).FirstOrDefault();
                odet.PaidStatus = 2;
                OrderDetailInvoices odi = db.OrderDetailInvoices.Where(w => w.OrderDetailId == id).OrderByDescending(o => o.Id).FirstOrDefault();
                if (odi != null)
                {
                    if (validLists.ContainsKey(odi.InvoicesId.Value))
                    {
                        validLists[odi.InvoicesId.Value].Add(odet.Id);

                    }
                    else
                    {
                        validLists.Add(odi.InvoicesId.Value, new List<Int64>() { odet.Id });
                    }
                }
            }



            foreach (var invs in validLists)
            {
                Invoices inv = db.Invoices.Where(w => w.Id == invs.Key).FirstOrDefault();
                if (inv != null)
                {
                    InvoiceOrderPayment(inv, invs.Value, accObj, type, defaultAccountObj);
                }
            }
        }

        private void VoidInvoicedOrder(IEnumerable<dynamic> detailsForPayOff)
        {
            Dictionary<Int64, List<dynamic>> validLists = new Dictionary<Int64, List<dynamic>>();


            var invLista = detailsForPayOff.Select(ss => ss);// new { OrderDetailId = ss.OrderDetailId, VoidCounter = ss.InvoiceCounter, PosInfoDetailId = ss.PosInfoDetailId, Day = ss.StatusTS, IsPrinted = ss.IsPrinted });
            foreach (var item in invLista)
            {
                Int64 id = item.OrderDetailId;
                OrderDetail odet = db.OrderDetail.Where(w => w.Id == id).FirstOrDefault();
                odet.PaidStatus = 2;
                OrderDetailInvoices odi = db.OrderDetailInvoices.Where(w => w.OrderDetailId == id).OrderByDescending(o => o.Id).FirstOrDefault();
                if (odi != null)
                {
                    if (validLists.ContainsKey(odi.InvoicesId.Value))
                    {
                        validLists[odi.InvoicesId.Value].Add(item);

                    }
                    else
                    {
                        validLists.Add(odi.InvoicesId.Value, new List<dynamic>() { item });
                    }
                }
            }
            foreach (var invs in validLists)
            {
                var temp = invs.Value.FirstOrDefault();
                Int64 pidid = temp.PosInfoDetailId;
                PosInfoDetail pid = db.PosInfoDetail.Where(w => w.Id == pidid).FirstOrDefault();
                if (pid.Counter < temp.VoidCounter)
                    pid.Counter = temp.VoidCounter;
                Invoices inv = db.Invoices.Where(w => w.Id == invs.Key).FirstOrDefault();
                if (inv != null)
                {
                    inv.IsVoided = true;
                    Invoices invoiceToVoid = new Invoices();
                    invoiceToVoid.PosInfoDetailId = temp.PosInfoDetailId;
                    invoiceToVoid.Counter = (int?)temp.VoidCounter;
                    invoiceToVoid.PosInfoId = inv.PosInfoId;
                    invoiceToVoid.Discount = inv.Discount;
                    invoiceToVoid.Total = inv.Total;
                    invoiceToVoid.Day = temp.StatusTS;
                    invoiceToVoid.Description = pid.Description;
                    invoiceToVoid.InvoiceTypeId = pid.InvoicesTypeId;
                    invoiceToVoid.IsVoided = false;
                    invoiceToVoid.IsDeleted = false;
                    invoiceToVoid.IsPrinted = temp.IsPrinted;
                    invoiceToVoid.TableId = inv.TableId;
                    invoiceToVoid.StaffId = inv.StaffId;
                    invoiceToVoid.PdaModuleId = inv.PdaModuleId;
                    invoiceToVoid.ClientPosId = inv.ClientPosId;

                    foreach (var o in invs.Value)
                    {
                        var id = (Int64)o.OrderDetailId;
                        OrderDetail upd = db.OrderDetail.Where(w => w.Id == id).FirstOrDefault();

                        upd.PaidStatus = (byte)2;
                        upd.Status = 5;
                        upd.StatusTS = o.StatusTS;
                        invoiceToVoid.TableId = upd.TableId;

                        OrderDetailInvoices odi = new OrderDetailInvoices();
                        odi.PosInfoDetailId = o.PosInfoDetailId;
                        odi.OrderDetailId = o.OrderDetailId;
                        odi.Invoices = invoiceToVoid;
                        odi.Counter = o.VoidCounter;
                        odi.StaffId = o.StaffId;
                        odi.CreationTS = o.StatusTS;
                        odi.IsDeleted = false;
                        odi.IsPrinted = o.IsPrinted;

                        db.OrderDetailInvoices.Add(odi);


                        //InvoiceOrderPayment(inv, invs.Value, accObj, type, defaultAccountObj);
                    }
                    var trans = db.Transactions.Where(w => w.InvoicesId == inv.Id);
                    foreach (var tr in trans)
                    {
                        var hasGuest = db.Invoice_Guests_Trans.Where(w => w.TransactionId == tr.Id).FirstOrDefault();
                        AccountsObj ao = new AccountsObj()
                        {
                            AccountId = tr.AccountId.Value,
                            GuestId = hasGuest != null ? hasGuest.GuestId : null,
                            Amount = tr.Amount.Value
                        };
                        invoiceToVoid.Transactions.Add(InvoiceTransaction(invoiceToVoid, ao, tr.DepartmentId, (int)OrderDetailUpdateType.PaidCancel));
                    }

                    db.Invoices.Add(invoiceToVoid);

                }
            }

        }

        public void Dispose()
        {

                db.Dispose();
        }
    }
    public class OrderDetailUpdateObjList
    {

        public ICollection<OrderDetailUpdateObj> OrderDet { get; set; }
        public ICollection<AccountsObj> AccountsObj { get; set; }
        public ICollection<CreditTransactions> CreditTransaction { get; set; }
    }

    public class OrderDetailUpdateObj
    {
        public long Id { get; set; }
        public Guid? Guid { get; set; }
        //public int? AA { get; set; }
        //public bool? IsOffline { get; set; }
        public long? PosId { get; set; }
        public int? OrderNo { get; set; }
        public byte Status { get; set; }
        public DateTime StatusTS { get; set; }
        public long? StaffId { get; set; }
        public byte? PaidStatus { get; set; }
        public ICollection<OrderDetailInvoices> OrderDetailInvoices { get; set; }
        public long? AccountId { get; set; }
        public Customers Customer { get; set; }
        public long? GuestId { get; set; }
        public decimal? TableDiscount { get; set; }
        public long? NewPrlId { get; set; }
        public long? invoiceCounter { get; set; }
        public decimal? TotalAfterDiscount { get; set; }
        public decimal? Discount { get; set; }
        public String DiscountRemark { get; set; }
    }

}