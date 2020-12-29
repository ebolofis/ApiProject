using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Pos_WebApi.Models;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Helpers;
using Newtonsoft.Json;
using System.Web;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using log4net;
using Symposium.Models.Enums;

namespace Pos_WebApi.Controllers
{
    public class HelperController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        //public class OrderDetailUpdateObj
        //{
        //    public long Id { get; set; }
        //    public Guid? Guid { get; set; }
        //    //public int? AA { get; set; }
        //    //public bool? IsOffline { get; set; }
        //    public long? PosId { get; set; }
        //    public int? OrderNo { get; set; }
        //    public byte Status { get; set; }
        //    public DateTime StatusTS { get; set; }
        //    public long? StaffId { get; set; }
        //    public byte? PaidStatus { get; set; }
        //    public ICollection<OrderDetailInvoices> OrderDetailInvoices { get; set; }
        //    public long? AccountId { get; set; }
        //    public Customers Customer { get; set; }
        //    public long? GuestId { get; set; }
        //    public decimal? TableDiscount { get; set; }
        //    public long? NewPrlId { get; set; }
        //    public long? invoiceCounter { get; set; }
        //    public decimal? TotalAfterDiscount { get; set; }
        //    public decimal? Discount { get; set; }
        //    public String DiscountRemark { get; set; }
        //}



        #region UpdateOrderDetails
        //public string PostOrderDetail(OrderDetailUpdateObjList OrderDet, bool updaterange)
        //{
        //    IEnumerable<OrderDetailUpdateObj> list = OrderDet.OrderDet;

        //    var od = list.Select(s => s.Id);
        //    List<OrderDetail> ordetlist = new List<OrderDetail>();
        //    List<OrderDetail> ordetPaidlist = new List<OrderDetail>();
        //    List<OrderDetail> ordetetailMappedList = db.OrderDetail.Include(i => i.OrderDetailIgredients).Include(i => i.OrderDetailVatAnal).Include("OrderDetailIgredients.OrderDetailIgredientVatAnal")
        //        .Where(w => od.Contains(w.Id)).ToList();
        //    decimal amount = 0;
        //    decimal discount = list.FirstOrDefault().TableDiscount != null ? list.FirstOrDefault().TableDiscount.Value : 0;
        //    decimal percentage = 0;
        //    try
        //    {
        //        if (discount > 0)
        //        {
        //            percentage = (discount / ordetetailMappedList.Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + (decimal)(sm.OrderDetailIgredients != null && sm.OrderDetailIgredients.Count > 0 ? sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    long updateReceiptNo = -1;
        //    int counter = 0;
        //    // decimal discountleft = discount;
        //    foreach (var o in list)
        //    {
        //        counter++;
        //        if (o.Guid != null)
        //        {
        //            var ord = ordetetailMappedList.Where(w => w.Guid == o.Guid).FirstOrDefault();//db.OrderDetail.Where(w => w.Id == o.Id).FirstOrDefault();
        //            if (ord != null)
        //            {
        //                if (o.GuestId != null)
        //                {
        //                    ord.GuestId = o.GuestId;
        //                }
        //                //if (o.StaffId != ord.Order.StaffId)
        //                //{
        //                //    ord.Order.StaffId = o.StaffId;
        //                //}
        //                ord.Status = o.Status;
        //                ord.StatusTS = DateTime.Now;
        //                if (o.PaidStatus != null)
        //                    ord.PaidStatus = o.PaidStatus;
        //                if (o.Status == 7)//Exoflisi
        //                {
        //                    ord.PaidStatus = 2;//Paid
        //                    //Change Pricelist OrderDetail
        //                    if (o.NewPrlId != null)
        //                    {
        //                        var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
        //                        if (pricelistdetail != null)
        //                        {
        //                            ord.PriceListDetailId = pricelistdetail.Id;
        //                            ord.Price = pricelistdetail.Price;
        //                            decimal tempprice = ord.Price != null ? Math.Round(ord.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
        //                            tempprice = ord.Qty != null && ord.Qty > 0 ? (decimal)(ord.Qty.Value) * tempprice : tempprice;
        //                            ord.TotalAfterDiscount = tempprice;
        //                            ord.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
        //                        }
        //                    }
        //                    //DISCOUNT
        //                    //percentage = percentage > 0 ? percentage : 0;
        //                    //decimal newamount = Math.Round(ord.TotalAfterDiscount.Value * percentage, 4);
        //                    //if (counter == list.Count())//An eimaste sto teleytaio dwse oti ekptwsh exei meinei gia na mhn exoume diafores me tis stroggylopoihseis
        //                    //{
        //                    //    newamount = discountleft;
        //                    //}
        //                    //discountleft = discountleft - newamount;
        //                    ord.TotalAfterDiscount = o.TotalAfterDiscount;//ord.TotalAfterDiscount.Value - newamount;
        //                    ord.Discount = o.Discount;//ord.Discount != null ? ord.Discount + newamount : newamount;

        //                    foreach (var anal in ord.OrderDetailVatAnal)
        //                    {
        //                        //Change Pricelist OrderDetailVatAnal
        //                        if (o.NewPrlId != null)
        //                        {
        //                            var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
        //                            if (pricelistdetail != null)
        //                            {
        //                                anal.Gross = ord.TotalAfterDiscount;
        //                                Vat vat = pricelistdetail.Vat;
        //                                Tax tax = pricelistdetail.Tax;
        //                                var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
        //                                var tempvat = (decimal)(anal.Gross - tempnetbyvat);
        //                                var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
        //                                var temptax = (decimal)(tempnetbyvat - tempnetbytax);
        //                                anal.Net = (decimal)(anal.Gross - tempvat - temptax);
        //                                anal.TaxAmount = temptax;
        //                                anal.VatAmount = tempvat;
        //                                anal.VatRate = vat != null ? vat.Percentage : 0;
        //                                anal.VatId = vat != null ? (long?)vat.Id : null;
        //                                anal.TaxId = tax != null ? (long?)tax.Id : null;
        //                            }
        //                        }
        //                        if (o.Discount != null && o.NewPrlId == null)
        //                        {
        //                            var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == ord.PriceListDetailId).FirstOrDefault();
        //                            anal.Gross = ord.TotalAfterDiscount;
        //                            Vat vat = pricelistdetail.Vat;
        //                            Tax tax = pricelistdetail.Tax;
        //                            var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
        //                            var tempvat = (decimal)(anal.Gross - tempnetbyvat);
        //                            var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
        //                            var temptax = (decimal)(tempnetbyvat - tempnetbytax);
        //                            anal.Net = (decimal)(anal.Gross - tempvat - temptax);
        //                            anal.TaxAmount = temptax;
        //                            anal.VatAmount = tempvat;
        //                            anal.VatRate = vat != null ? vat.Percentage : 0;
        //                            anal.VatId = vat != null ? (long?)vat.Id : null;
        //                            anal.TaxId = tax != null ? (long?)tax.Id : null;
        //                        }
        //                        //decimal Grossdis = Math.Round(anal.Gross.Value * percentage, 4);
        //                        //anal.Gross = anal.Gross - Grossdis;
        //                        //decimal Netdis = Math.Round(anal.Net.Value * percentage, 4);
        //                        //anal.Net = anal.Net - Netdis;
        //                        //decimal Taxdis = Math.Round(anal.TaxAmount.Value * percentage, 4);
        //                        //anal.TaxAmount = anal.TaxAmount - Taxdis;
        //                        //decimal Vatdis = Math.Round(anal.VatAmount.Value * percentage, 4);
        //                        //anal.VatAmount = anal.VatAmount - Vatdis;

        //                    }
        //                    foreach (var odi in ord.OrderDetailIgredients)
        //                    {
        //                        //Change Pricelist OrderDetailIgredients
        //                        if (o.NewPrlId != null)
        //                        {
        //                            var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
        //                            if (pricelistdetail != null)
        //                            {
        //                                odi.PriceListDetailId = pricelistdetail.Id;
        //                                odi.Price = pricelistdetail.Price;
        //                                decimal tempprice = odi.Price != null ? Math.Round(odi.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
        //                                tempprice = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice : tempprice;
        //                                odi.TotalAfterDiscount = tempprice;
        //                                odi.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
        //                            }
        //                        }
        //                        decimal newingamount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value * percentage), 2) : 0;
        //                        odi.TotalAfterDiscount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value - newingamount), 2) : 0;
        //                        odi.Discount = odi.Discount != null ? odi.Discount + newingamount : newingamount;
        //                        amount += odi.TotalAfterDiscount.Value;
        //                        //Change Pricelist
        //                        if (o.NewPrlId != null)
        //                        {
        //                            var pricelistdetailing = db.PricelistDetail.Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
        //                            if (pricelistdetailing != null)
        //                            {
        //                                odi.PriceListDetailId = pricelistdetailing.Id;
        //                                odi.Price = pricelistdetailing.Price;
        //                            }
        //                        }
        //                        foreach (var anal2 in odi.OrderDetailIgredientVatAnal)
        //                        {
        //                            //Change Pricelist OrderDetailIgredientVatAnal
        //                            if (o.NewPrlId != null)
        //                            {
        //                                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
        //                                if (pricelistdetail != null)
        //                                {
        //                                    anal2.Gross = odi.TotalAfterDiscount;
        //                                    Vat vat = pricelistdetail.Vat;
        //                                    Tax tax = pricelistdetail.Tax;
        //                                    var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
        //                                    var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
        //                                    var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
        //                                    var temptax = (decimal)(tempnetbyvat - tempnetbytax);
        //                                    anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
        //                                    anal2.TaxAmount = temptax;
        //                                    anal2.VatAmount = tempvat;
        //                                    anal2.VatRate = vat != null ? vat.Percentage : 0;
        //                                    anal2.VatId = vat != null ? (long?)vat.Id : null;
        //                                    anal2.TaxId = tax != null ? (long?)tax.Id : null;
        //                                }
        //                            }
        //                            //Discount on  OrderDetailIgredientVatAnal
        //                            if (odi.Discount != null && o.NewPrlId == null)
        //                            {
        //                                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault();
        //                                if (pricelistdetail != null)
        //                                {
        //                                    anal2.Gross = odi.TotalAfterDiscount;
        //                                    Vat vat = pricelistdetail.Vat;
        //                                    Tax tax = pricelistdetail.Tax;
        //                                    var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
        //                                    var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
        //                                    var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
        //                                    var temptax = (decimal)(tempnetbyvat - tempnetbytax);
        //                                    anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
        //                                    anal2.TaxAmount = temptax;
        //                                    anal2.VatAmount = tempvat;
        //                                    anal2.VatRate = vat != null ? vat.Percentage : 0;
        //                                    anal2.VatId = vat != null ? (long?)vat.Id : null;
        //                                    anal2.TaxId = tax != null ? (long?)tax.Id : null;
        //                                }
        //                            }
        //                            //anal2.Gross = anal2.Gross - anal2.Gross != null ? Math.Round(anal2.Gross.Value * percentage, 4) : 0;
        //                            //anal2.Net = anal2.Net - anal2.Net != null ? Math.Round(anal2.Net.Value * percentage, 4) : 0;
        //                            //anal2.TaxAmount = anal2.TaxAmount - anal2.TaxAmount != null ? Math.Round(anal2.TaxAmount.Value * percentage, 4) : 0;
        //                            //anal2.VatAmount = anal2.VatAmount - anal2.VatAmount != null ? Math.Round(anal2.VatAmount.Value * percentage, 4) : 0;
        //                        }
        //                    }
        //                    amount += ord.TotalAfterDiscount.Value;//- newamount;
        //                    ordetPaidlist.Add(ord);
        //                }
        //                if (o.Status == 5 && o.PaidStatus == 2)
        //                {
        //                    ordetPaidlist.Add(ord);
        //                }
        //                ordetlist.Add(ord);
        //            }
        //        }
        //        else if (o.IsOffline == true && o.AA != null && o.PosId != null && o.OrderNo != null)
        //        {
        //            var ord = db.Order.Include("OrderDetail").Where(w => w.OrderNo == o.OrderNo && w.PosId == o.PosId && w.EndOfDayId == null).FirstOrDefault();
        //            if (ord != null)
        //            {
        //                OrderDetail ordet = new OrderDetail();
        //                ordet = ord.OrderDetail.Skip(o.AA.Value).FirstOrDefault();
        //                ordet.Status = o.Status;
        //                ordet.StatusTS = DateTime.Now;
        //                if (o.GuestId != null)
        //                {
        //                    ordet.GuestId = o.GuestId;
        //                }
        //                //if (o.Status == 7)//Exoflisi
        //                //{
        //                //    ordet.PaidStatus = 2;//Paid
        //                //}
        //                ordetlist.Add(ordet);
        //            }
        //        }
        //        if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
        //        {
        //            foreach (var i in o.OrderDetailInvoices)
        //            {
        //                db.OrderDetailInvoices.Add(i);
        //                updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
        //            }
        //        }
        //    }
        //    //db.SaveChanges();
        //    long newCounter = -1;
        //    var pid = db.PosInfoDetail.FirstOrDefault();
        //    if (updateReceiptNo > -1)
        //    {
        //        pid = db.PosInfoDetail.Where(w => w.Id == updateReceiptNo).FirstOrDefault();
        //        long onlineCounter = pid.Counter != null ? (pid.Counter.Value + 1) : 1;
        //        newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : onlineCounter;
        //        pid.Counter = newCounter;
        //        var posinfo = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == pid.PosInfoId).FirstOrDefault();
        //        if (posinfo != null)
        //        {
        //            var piDetGroup = posinfo.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == pid.GroupId);
        //            foreach (var i in piDetGroup)
        //            {
        //                i.Counter = newCounter;
        //            }
        //        }
        //    }

        //    var group = ordetlist.GroupBy(g => g.OrderId).Select(s => new
        //    {
        //        OrderId = s.Key,
        //        OrderDetailStatus = s.FirstOrDefault().Status
        //    });

        //    foreach (var o in group)
        //    {
        //        var order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Include("OrderDetail.OrderDetailInvoices")
        //            .Include("OrderDetail.OrderDetailInvoices.PosInfoDetail").Include("OrderStatus").Include("OrderDetail.PricelistDetail")
        //            .Include(i => i.PosInfo).SingleOrDefault(s => s.Id == o.OrderId);
        //        var gr = order.OrderDetail.GroupBy(g => new { g.PaidStatus, g.Status }).Select(s => new
        //        {
        //            PaidStatus = s.Key.PaidStatus,
        //            Status = s.Key.Status,
        //            Count = s.Count(),
        //            PaidOrderDetails = s.Where(w => ordetPaidlist.Contains(w)).AsEnumerable<OrderDetail>(),
        //            OrderDetails = s
        //        });
        //        foreach (var g in gr)
        //        {
        //            if (g.Status != 5 && g.PaidStatus == 2 && order.OrderStatus.Where(w => w.Status == 7).Count() == 0)//Paid && AN DEN YPARXEI HDH STATUS EXOFLISIS
        //            {
        //                Transactions tr = new Transactions();
        //                tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
        //                tr.PosInfoId = order.PosInfo.Id;
        //                //tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price);
        //                tr.Day = DateTime.Now;
        //                tr.DepartmentId = order.PosInfo.DepartmentId;
        //                tr.Description = "Εξόφληση";
        //                if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
        //                {
        //                    tr.ExtDescription = "Μερική Εξόφληση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
        //                }
        //                tr.InOut = 0;
        //                tr.OrderId = order.Id;
        //                tr.StaffId = list.FirstOrDefault().StaffId;
        //                tr.TransactionType = 3;
        //                Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
        //                tr.Gross = t.Gross;
        //                tr.Amount = t.Gross;
        //                tr.Net = t.Net;
        //                tr.Tax = t.Tax;
        //                tr.Vat = t.Vat;
        //                tr.OrderDetail = new List<OrderDetail>();
        //                tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;
        //                //db.Transactions.Add(tr);
        //                //db.SaveChanges();
        //                foreach (var paidDet in g.PaidOrderDetails)
        //                {
        //                    var orderdetailinvoices = paidDet.OrderDetailInvoices.Where(w => w.PosInfoDetail.CreateTransaction == false && w.PosInfoDetail.IsInvoice == true);
        //                    if (orderdetailinvoices.Count() > 0)
        //                    {
        //                        foreach (var orinv in orderdetailinvoices)
        //                        {
        //                            orinv.StaffId = list.FirstOrDefault().StaffId;
        //                        }
        //                    }
        //                    // paidDet.TransactionId = tr.Id;
        //                }
        //                var hotel = db.HotelInfo.FirstOrDefault();
        //                var account = db.Accounts.Find(tr.AccountId);
        //                if (account != null && account.SendsTransfer == true && hotel != null)
        //                {
        //                    var query = (from f in g.PaidOrderDetails
        //                                 join st in db.SalesType on f.SalesTypeId equals st.Id
        //                                 join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                                 from ls in loj.DefaultIfEmpty()
        //                                 select new
        //                                 {
        //                                     Id = f.Id,
        //                                     SalesTypeId = st.Id,
        //                                     Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
        //                                     PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                                     PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                                     CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
        //                                     ReceiptNo = newCounter,
        //                                 }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
        //                                 {
        //                                     PmsDepartmentId = s.Key,
        //                                     PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                                     Total = s.Sum(sm => sm.Total),
        //                                     CustomerId = s.FirstOrDefault().CustomerId,
        //                                     ReceiptNo = s.FirstOrDefault().ReceiptNo
        //                                 });
        //                    long guestid = tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1;

        //                    Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

        //                    string storeid = HttpContext.Current.Request.Params["storeid"];
        //                    List<TransferObject> objTosendList = new List<TransferObject>();
        //                    List<TransferToPms> transferList = new List<TransferToPms>();

        //                    var IsCreditCard = false;
        //                    var roomOfCC = "";
        //                    if (account.Type == 4)
        //                    {
        //                        IsCreditCard = true;
        //                        roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
        //                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
        //                    }

        //                    foreach (var acg in query)
        //                    {
        //                        TransferToPms tpms = new TransferToPms(); //newCounter
        //                        tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + order.PosInfo.Description;
        //                        tpms.PmsDepartmentId = acg.PmsDepartmentId;
        //                        tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
        //                        tpms.ProfileId = acg.CustomerId;
        //                        tpms.ProfileName = !IsCreditCard ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
        //                        tpms.ReceiptNo = newCounter.ToString();
        //                        tpms.RegNo = !IsCreditCard ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
        //                        tpms.RoomDescription = !IsCreditCard ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
        //                        tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
        //                        tpms.SendToPMS = true;
        //                        tpms.PmsDepartmentDescription = acg.PmsDepDescription;
        //                        //tpms.TransactionId = tr.Id;
        //                        tpms.TransferType = 0;//Xrewstiko
        //                        tpms.SendToPmsTS = DateTime.Now;
        //                        tpms.Total = (decimal)acg.Total;
        //                        var identifier = Guid.NewGuid();
        //                        tpms.TransferIdentifier = identifier;
        //                        transferList.Add(tpms);
        //                        db.TransferToPms.Add(tpms);

        //                        TransferObject to = new TransferObject();
        //                        //
        //                        to.TransferIdentifier = tpms.TransferIdentifier;
        //                        //
        //                        to.HotelId = (int)hotel.Id;
        //                        to.amount = (decimal)tpms.Total;
        //                        int PmsDepartmentId = 0;
        //                        var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
        //                        to.departmentId = PmsDepartmentId;
        //                        to.description = tpms.Description;
        //                        to.profileName = tpms.ProfileName;
        //                        int resid = 0;
        //                        var toint = int.TryParse(tpms.RegNo, out resid);
        //                        to.resId = resid;
        //                        to.TransferIdentifier = identifier;
        //                        to.HotelUri = hotel.HotelUri;
        //                        to.RoomName = tpms.RoomDescription;
        //                        if (IsCreditCard)
        //                        {
        //                            to.RoomName = roomOfCC;
        //                        }

        //                        if (to.amount != 0)
        //                            objTosendList.Add(to);
        //                    }
        //                    tr.TransferToPms = new List<TransferToPms>();
        //                    tr.TransferToPms = transferList as ICollection<TransferToPms>;

        //                    db.SaveChanges();
        //                    foreach (var to in objTosendList)
        //                    {
        //                        SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //                        sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
        //                    }
        //                }
        //                db.Transactions.Add(tr);
        //            }
        //            if (g.Status == 5 && g.PaidStatus == 2)//Einai Akyrwseis kai einai exoflimena
        //            {
        //                Transactions tr = new Transactions();
        //                tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
        //                tr.PosInfoId = order.PosInfo.Id;
        //                //tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price) * -1;
        //                tr.Day = DateTime.Now;
        //                tr.DepartmentId = order.PosInfo.DepartmentId;
        //                tr.Description = "Ακύρωση";
        //                if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
        //                {
        //                    tr.ExtDescription = "Μερική Ακύρωση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
        //                }
        //                tr.InOut = 1;//Εκροη
        //                tr.OrderId = order.Id;
        //                tr.StaffId = list.FirstOrDefault().StaffId;
        //                tr.TransactionType = (short)TransactionType.Cancel;
        //                Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
        //                //tr.TransactionType = (short)TransactionType.Cancel;
        //                tr.Gross = t.Gross;
        //                tr.Amount = t.Gross;
        //                tr.Net = t.Net;
        //                tr.Tax = t.Tax;
        //                tr.Vat = t.Vat;
        //                tr.OrderDetail = new List<OrderDetail>();
        //                tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;

        //                //db.Transactions.Add(tr);
        //                //db.SaveChanges();
        //                var hotel = db.HotelInfo.FirstOrDefault();
        //                var account = db.Accounts.Find(tr.AccountId);
        //                if (account != null && account.SendsTransfer == true && hotel != null)
        //                {
        //                    var query = (from f in g.PaidOrderDetails
        //                                 join st in db.SalesType on f.SalesTypeId equals st.Id
        //                                 join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                                 from ls in loj.DefaultIfEmpty()
        //                                 select new
        //                                 {
        //                                     Id = f.Id,
        //                                     SalesTypeId = st.Id,
        //                                     Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
        //                                     PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                                     PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                                     CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
        //                                     ReceiptNo = f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault() != null ? f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault().Counter : newCounter,
        //                                 }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
        //                                 {
        //                                     PmsDepartmentId = s.Key,
        //                                     PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                                     Total = s.Sum(sm => sm.Total),
        //                                     CustomerId = s.FirstOrDefault().CustomerId,
        //                                     ReceiptNo = s.FirstOrDefault().ReceiptNo
        //                                 });
        //                    //Customers curcustomer = list.FirstOrDefault().Customer;

        //                    long guestid = tr.OrderDetail.FirstOrDefault() != null ? tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1 : -1;

        //                    Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

        //                    string storeid = HttpContext.Current.Request.Params["storeid"];
        //                    List<TransferObject> objTosendList = new List<TransferObject>();
        //                    List<TransferToPms> transferList = new List<TransferToPms>();

        //                    var IsCreditCard = false;
        //                    var roomOfCC = "";
        //                    if (account.Type == 4)
        //                    {
        //                        IsCreditCard = true;
        //                        roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
        //                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
        //                    }
        //                    foreach (var acg in query)
        //                    {
        //                        TransferToPms tpms = new TransferToPms(); // newCounter
        //                        tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + order.PosInfo.Description;
        //                        tpms.PmsDepartmentId = acg.PmsDepartmentId;
        //                        tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
        //                        tpms.ProfileId = acg.CustomerId;
        //                        tpms.ProfileName = !IsCreditCard ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
        //                        tpms.ReceiptNo = newCounter.ToString();
        //                        tpms.RegNo = !IsCreditCard ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
        //                        tpms.RoomDescription = !IsCreditCard ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
        //                        tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
        //                        tpms.SendToPMS = true;
        //                        tpms.PmsDepartmentDescription = acg.PmsDepDescription;
        //                        //tpms.TransactionId = tr.Id;
        //                        tpms.TransferType = 0;//Xrewstiko
        //                        tpms.Total = (decimal)acg.Total * -1;
        //                        tpms.SendToPmsTS = DateTime.Now;
        //                        var identifier = Guid.NewGuid();
        //                        tpms.TransferIdentifier = identifier;
        //                        transferList.Add(tpms);
        //                        db.TransferToPms.Add(tpms);

        //                        TransferObject to = new TransferObject();
        //                        //
        //                        to.TransferIdentifier = tpms.TransferIdentifier;
        //                        //
        //                        to.HotelId = (int)hotel.Id;
        //                        to.amount = (decimal)tpms.Total;
        //                        int PmsDepartmentId = 0;
        //                        var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
        //                        to.departmentId = PmsDepartmentId;
        //                        to.description = tpms.Description;
        //                        to.profileName = tpms.ProfileName;
        //                        int resid = 0;
        //                        var toint = int.TryParse(tpms.RegNo, out resid);
        //                        to.resId = resid;
        //                        to.TransferIdentifier = identifier;
        //                        to.HotelUri = hotel.HotelUri;
        //                        to.RoomName = tpms.RoomDescription;
        //                        if (IsCreditCard)
        //                        {
        //                            to.RoomName = roomOfCC;
        //                        }

        //                        if (to.amount != 0)
        //                            objTosendList.Add(to);
        //                    }
        //                    tr.TransferToPms = new List<TransferToPms>();
        //                    tr.TransferToPms = transferList as ICollection<TransferToPms>;

        //                    db.SaveChanges();
        //                    foreach (var to in objTosendList)
        //                    {
        //                        SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //                        sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
        //                    }
        //                }
        //                db.Transactions.Add(tr);
        //            }
        //            if (g.Count == order.OrderDetail.Count) // OLA TA OrderDetail THS PARAGGELIAS EINAI STO GROUPAKI
        //            {
        //                //Add New Order Status
        //                OrderStatus os = new OrderStatus();
        //                os.OrderId = order.Id;
        //                if (g.Status == 3 || g.Status == 2 || g.Status == 5)
        //                {
        //                    os.Status = g.Status;
        //                }
        //                else
        //                {
        //                    if (g.PaidStatus == 2)//paid
        //                    {
        //                        os.Status = 7;//"Εξοφλημένη";
        //                        if (order.OrderStatus.Where(w => w.Status == 8).Count() == 0)//AN EINAI EXOFLISI KAI DEN EXEI TIMOLOGITHEI
        //                        {
        //                            OrderStatus os8 = new OrderStatus(); //PROSTHESE STATUS TIMOLOGISHS (8)
        //                            os8.OrderId = order.Id;
        //                            os8.Status = 8;
        //                            os8.StaffId = list.FirstOrDefault().StaffId;
        //                            os8.TimeChanged = DateTime.Now;
        //                            db.OrderStatus.Add(os8);
        //                        }
        //                    }
        //                    else if (g.PaidStatus == 1)//invoiced
        //                    {
        //                        os.Status = 8;// "Τιμολογημένη";
        //                    }
        //                    else
        //                    {
        //                        os.Status = g.Status;
        //                    }
        //                }
        //                os.TimeChanged = DateTime.Now;
        //                os.StaffId = list.FirstOrDefault().StaffId;
        //                if (order.OrderStatus.Where(s => s.Status == os.Status).Count() == 0)//VALE TO NEO OrserStatus MONO AN DEN YPARXEI HDH
        //                {
        //                    db.OrderStatus.Add(os);
        //                }
        //            }
        //        }
        //    }

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return ex.ToString();// Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //    var listupdate = OrderDet.OrderDet.Select(s => new
        //    {
        //        AA = s.AA,
        //        AccountId = s.AccountId,
        //        Customer = s.Customer,
        //        Id = s.Id,
        //        Guid = s.Guid,
        //        IsOffline = s.IsOffline,
        //        OrderNo = s.OrderNo,
        //        PaidStatus = s.PaidStatus,
        //        PosId = s.PosId,
        //        StaffId = s.StaffId,
        //        Status = s.Status,
        //        StatusTS = s.StatusTS,
        //        OrderDetailInvoices = s.OrderDetailInvoices != null ? s.OrderDetailInvoices.Select(ss => new
        //        {
        //            Counter = ss.Counter,
        //            CreationTS = ss.CreationTS,
        //            CustomerId = ss.CustomerId,
        //            IsPrinted = ss.IsPrinted,
        //            OrderDetailId = ss.OrderDetailId,
        //            PosInfoDetailId = ss.PosInfoDetailId,
        //            StaffId = ss.StaffId
        //        }) : null
        //    });
        //    return JsonConvert.SerializeObject(listupdate);//Request.CreateResponse(HttpStatusCode.Created, OrderDet.OrderDet);
        //}
        #endregion

        #region UpdateOrderDetails With Invoices
        /*
        public string PostOrderDetail(OrderDetailUpdateObjList OrderDet, bool updaterange)
        {
            IEnumerable<OrderDetailUpdateObj> list = OrderDet.OrderDet;

            var od = list.Select(s => s.Id);
            List<OrderDetail> ordetlist = new List<OrderDetail>();
            List<OrderDetail> ordetPaidlist = new List<OrderDetail>();
            List<OrderDetail> ordetetailMappedList = db.OrderDetail.Include(i => i.OrderDetailIgredients).Include(i => i.OrderDetailVatAnal).Include("OrderDetailIgredients.OrderDetailIgredientVatAnal").Include(i => i.Order)
                .Where(w => od.Contains(w.Id)).ToList();
            decimal amount = 0;
            decimal discount = list.FirstOrDefault().TableDiscount != null ? list.FirstOrDefault().TableDiscount.Value : 0;
            decimal percentage = 0;
            try
            {
                if (discount > 0)
                {
                    percentage = (discount / ordetetailMappedList.Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + (decimal)(sm.OrderDetailIgredients != null && sm.OrderDetailIgredients.Count > 0 ? sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0))));
                }
            }
            catch (Exception ex)
            {

            }
            long updateReceiptNo = -1;
            int counter = 0;
            Invoices invoice = new Invoices();
            // decimal discountleft = discount;
            foreach (var o in list)
            {
                counter++;
                if (o.Guid != null)
                {
                    var ord = ordetetailMappedList.Where(w => w.Guid == o.Guid).FirstOrDefault();//db.OrderDetail.Where(w => w.Id == o.Id).FirstOrDefault();
                    if (ord != null)
                    {
                        if (o.GuestId != null)
                        {
                            ord.GuestId = o.GuestId;
                        }
                        //if (o.StaffId != ord.Order.StaffId)
                        //{
                        //    ord.Order.StaffId = o.StaffId;
                        //}
                        if (o.Status != 7 && o.Status != 8)//Mhn kaneis UPDATE to status tou DETAIL gt kanei conflict sta TODO tou KDS
                        {
                            ord.Status = o.Status;
                        }
                        ord.StatusTS = DateTime.Now;
                        if (o.PaidStatus != null)
                            ord.PaidStatus = o.PaidStatus;
                        if (o.Status == 7)//Exoflisi
                        {
                            ord.PaidStatus = 2;//Paid
                            //Change Pricelist OrderDetail
                            if (o.NewPrlId != null)
                            {
                                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
                                if (pricelistdetail != null)
                                {
                                    ord.PriceListDetailId = pricelistdetail.Id;
                                    ord.Price = pricelistdetail.Price;
                                    decimal tempprice = ord.Price != null ? Math.Round(ord.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                                    tempprice = ord.Qty != null && ord.Qty > 0 ? (decimal)(ord.Qty.Value) * tempprice : tempprice;
                                    ord.TotalAfterDiscount = tempprice;
                                    ord.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
                                }
                            }
                            //DISCOUNT
                            //percentage = percentage > 0 ? percentage : 0;
                            //decimal newamount = Math.Round(ord.TotalAfterDiscount.Value * percentage, 4);
                            //if (counter == list.Count())//An eimaste sto teleytaio dwse oti ekptwsh exei meinei gia na mhn exoume diafores me tis stroggylopoihseis
                            //{
                            //    newamount = discountleft;
                            //}
                            //discountleft = discountleft - newamount;
                            ord.TotalAfterDiscount = o.TotalAfterDiscount;//ord.TotalAfterDiscount.Value - newamount;
                            ord.Discount = o.Discount;//ord.Discount != null ? ord.Discount + newamount : newamount;

                            foreach (var anal in ord.OrderDetailVatAnal)
                            {
                                //Change Pricelist OrderDetailVatAnal
                                if (o.NewPrlId != null)
                                {
                                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
                                    if (pricelistdetail != null)
                                    {
                                        anal.Gross = ord.TotalAfterDiscount;
                                        Vat vat = pricelistdetail.Vat;
                                        Tax tax = pricelistdetail.Tax;
                                        var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
                                        var tempvat = (decimal)(anal.Gross - tempnetbyvat);
                                        var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                        var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                        anal.Net = (decimal)(anal.Gross - tempvat - temptax);
                                        anal.TaxAmount = temptax;
                                        anal.VatAmount = tempvat;
                                        anal.VatRate = vat != null ? vat.Percentage : 0;
                                        anal.VatId = vat != null ? (long?)vat.Id : null;
                                        anal.TaxId = tax != null ? (long?)tax.Id : null;
                                    }
                                }
                                if (o.Discount != null && o.NewPrlId == null)
                                {
                                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == ord.PriceListDetailId).FirstOrDefault();
                                    anal.Gross = ord.TotalAfterDiscount;
                                    Vat vat = pricelistdetail.Vat;
                                    Tax tax = pricelistdetail.Tax;
                                    var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
                                    var tempvat = (decimal)(anal.Gross - tempnetbyvat);
                                    var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                    var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                    anal.Net = (decimal)(anal.Gross - tempvat - temptax);
                                    anal.TaxAmount = temptax;
                                    anal.VatAmount = tempvat;
                                    anal.VatRate = vat != null ? vat.Percentage : 0;
                                    anal.VatId = vat != null ? (long?)vat.Id : null;
                                    anal.TaxId = tax != null ? (long?)tax.Id : null;
                                }
                                //decimal Grossdis = Math.Round(anal.Gross.Value * percentage, 4);
                                //anal.Gross = anal.Gross - Grossdis;
                                //decimal Netdis = Math.Round(anal.Net.Value * percentage, 4);
                                //anal.Net = anal.Net - Netdis;
                                //decimal Taxdis = Math.Round(anal.TaxAmount.Value * percentage, 4);
                                //anal.TaxAmount = anal.TaxAmount - Taxdis;
                                //decimal Vatdis = Math.Round(anal.VatAmount.Value * percentage, 4);
                                //anal.VatAmount = anal.VatAmount - Vatdis;

                            }
                            foreach (var odi in ord.OrderDetailIgredients)
                            {
                                //Change Pricelist OrderDetailIgredients
                                if (o.NewPrlId != null)
                                {
                                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                                    if (pricelistdetail != null)
                                    {
                                        odi.PriceListDetailId = pricelistdetail.Id;
                                        odi.Price = pricelistdetail.Price;
                                        decimal tempprice = odi.Price != null ? Math.Round(odi.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                                        tempprice = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice : tempprice;
                                        odi.TotalAfterDiscount = tempprice;
                                        odi.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
                                    }
                                }
                                decimal newingamount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value * percentage), 2) : 0;
                                odi.TotalAfterDiscount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value - newingamount), 2) : 0;
                                odi.Discount = odi.Discount != null ? odi.Discount + newingamount : newingamount;
                                amount += odi.TotalAfterDiscount.Value;
                                //Change Pricelist
                                if (o.NewPrlId != null)
                                {
                                    var pricelistdetailing = db.PricelistDetail.Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                                    if (pricelistdetailing != null)
                                    {
                                        odi.PriceListDetailId = pricelistdetailing.Id;
                                        odi.Price = pricelistdetailing.Price;
                                    }
                                }
                                foreach (var anal2 in odi.OrderDetailIgredientVatAnal)
                                {
                                    //Change Pricelist OrderDetailIgredientVatAnal
                                    if (o.NewPrlId != null)
                                    {
                                        var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                                        if (pricelistdetail != null)
                                        {
                                            anal2.Gross = odi.TotalAfterDiscount;
                                            Vat vat = pricelistdetail.Vat;
                                            Tax tax = pricelistdetail.Tax;
                                            var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
                                            var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
                                            var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                            var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                            anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
                                            anal2.TaxAmount = temptax;
                                            anal2.VatAmount = tempvat;
                                            anal2.VatRate = vat != null ? vat.Percentage : 0;
                                            anal2.VatId = vat != null ? (long?)vat.Id : null;
                                            anal2.TaxId = tax != null ? (long?)tax.Id : null;
                                        }
                                    }
                                    //Discount on  OrderDetailIgredientVatAnal
                                    if (odi.Discount != null && o.NewPrlId == null)
                                    {
                                        var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault();
                                        if (pricelistdetail != null)
                                        {
                                            anal2.Gross = odi.TotalAfterDiscount;
                                            Vat vat = pricelistdetail.Vat;
                                            Tax tax = pricelistdetail.Tax;
                                            var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
                                            var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
                                            var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                            var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                            anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
                                            anal2.TaxAmount = temptax;
                                            anal2.VatAmount = tempvat;
                                            anal2.VatRate = vat != null ? vat.Percentage : 0;
                                            anal2.VatId = vat != null ? (long?)vat.Id : null;
                                            anal2.TaxId = tax != null ? (long?)tax.Id : null;
                                        }
                                    }
                                    //anal2.Gross = anal2.Gross - anal2.Gross != null ? Math.Round(anal2.Gross.Value * percentage, 4) : 0;
                                    //anal2.Net = anal2.Net - anal2.Net != null ? Math.Round(anal2.Net.Value * percentage, 4) : 0;
                                    //anal2.TaxAmount = anal2.TaxAmount - anal2.TaxAmount != null ? Math.Round(anal2.TaxAmount.Value * percentage, 4) : 0;
                                    //anal2.VatAmount = anal2.VatAmount - anal2.VatAmount != null ? Math.Round(anal2.VatAmount.Value * percentage, 4) : 0;
                                }
                            }
                            amount += ord.TotalAfterDiscount.Value;//- newamount;
                            ordetPaidlist.Add(ord);
                        }
                        if (o.Status == 5 && o.PaidStatus == 2)
                        {
                            ordetPaidlist.Add(ord);
                        }
                        ordetlist.Add(ord);
                        invoice.ClientPosId = ord.Order.ClientPosId;
                        invoice.PdaModuleId = ord.Order.PdaModuleId;
                        invoice.PosInfoId = o.PosId;
                        invoice.GuestId = o.GuestId;
                        invoice.StaffId = o.StaffId;
                        invoice.TableId = ord.TableId;
                        if (o.Status == 5 && o.PaidStatus != 2)//Akyrwsh mh exoflimenwn
                        {
                            decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
                            invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
                            decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
                            invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
                            decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
                            invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
                            decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
                            invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
                            var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;
                            invoice.Discount = invoice.Discount != null ? invoice.Discount + ord.Discount + ingrDis : ord.Discount + ingrDis;
                            if (o.OrderDetailInvoices.Count > 0)
                            {
                                invoice.IsPrinted = o.OrderDetailInvoices.FirstOrDefault().IsPrinted;
                            }
                        }
                    }
                }
                //else if (o.IsOffline == true && o.AA != null && o.PosId != null && o.OrderNo != null)
                //{
                //    var ord = db.Order.Include("OrderDetail").Where(w => w.OrderNo == o.OrderNo && w.PosId == o.PosId && w.EndOfDayId == null).FirstOrDefault();
                //    if (ord != null)
                //    {
                //        OrderDetail ordet = new OrderDetail();
                //        ordet = ord.OrderDetail.Skip(o.AA.Value).FirstOrDefault();
                //        ordet.Status = o.Status;
                //        ordet.StatusTS = DateTime.Now;
                //        if (o.GuestId != null)
                //        {
                //            ordet.GuestId = o.GuestId;
                //        }
                //        //if (o.Status == 7)//Exoflisi
                //        //{
                //        //    ordet.PaidStatus = 2;//Paid
                //        //}
                //        ordetlist.Add(ordet);
                //    }
                //}
                if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                {
                    invoice.Day = o.OrderDetailInvoices.FirstOrDefault().CreationTS;
                    invoice.Counter = (int?)o.OrderDetailInvoices.FirstOrDefault().Counter;
                    invoice.PosInfoDetailId = o.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId;
                    foreach (var i in o.OrderDetailInvoices)
                    {
                        invoice.OrderDetailInvoices.Add(i);
                        db.OrderDetailInvoices.Add(i);
                        updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
                    }
                }
            }

            bool isFromKds = ordetlist.All(a => a.Status == 2 || a.Status == 3);

            invoice.Cover = ordetetailMappedList.GroupBy(g => g.OrderId).Sum(s => s.FirstOrDefault().Couver);
            //db.SaveChanges();
            long newCounter = -1;
            var pid = db.PosInfoDetail.FirstOrDefault();
            if (updateReceiptNo > -1)
            {
                pid = db.PosInfoDetail.Where(w => w.Id == updateReceiptNo).FirstOrDefault();
                long onlineCounter = pid.Counter != null ? (pid.Counter.Value + 1) : 1;
                newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : onlineCounter;
                pid.Counter = newCounter;
                var posinfo = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == pid.PosInfoId).FirstOrDefault();
                if (posinfo != null)
                {
                    var piDetGroup = posinfo.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == pid.GroupId);
                    foreach (var i in piDetGroup)
                    {
                        i.Counter = newCounter;
                    }
                }
                invoice.Counter = (int)newCounter;
                invoice.Description = pid.Description;
                invoice.InvoiceTypeId = pid.InvoicesTypeId;

            }

            var group = ordetlist.ToList().GroupBy(g => g.OrderId).Select(s => new
            {
                OrderId = s.Key,
                OrderDetailStatus = s.FirstOrDefault().Status
            });

            foreach (var o in group)
            {
                var order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Include("OrderDetail.OrderDetailInvoices")
                    .Include("OrderDetail.OrderDetailInvoices.PosInfoDetail").Include("OrderStatus").Include("OrderDetail.PricelistDetail")
                    .Include(i => i.PosInfo).SingleOrDefault(s => s.Id == o.OrderId);
                var gr = order.OrderDetail.GroupBy(g => new { g.PaidStatus, g.Status }).Select(s => new
                {
                    PaidStatus = s.Key.PaidStatus,
                    Status = s.Key.Status,
                    Count = s.Count(),
                    PaidOrderDetails = s.Where(w => ordetPaidlist.Contains(w)).AsEnumerable<OrderDetail>(),
                    OrderDetails = s
                });
                foreach (var g in gr)
                {
                    if (!isFromKds)//DEN EXEI ERTHEI APO KDS TO STATUS
                    {
                        if (g.Status != 5 && g.PaidStatus == 2 && order.OrderStatus.Where(w => w.Status == 7).Count() == 0)//Paid && AN DEN YPARXEI HDH STATUS EXOFLISIS
                        {
                            Transactions tr = new Transactions();
                            tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
                            tr.PosInfoId = order.PosInfo.Id;
                            //tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price);
                            tr.Day = DateTime.Now;
                            tr.DepartmentId = order.PosInfo.DepartmentId;
                            tr.Description = "Εξόφληση";
                            if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
                            {
                                tr.ExtDescription = "Μερική Εξόφληση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
                            }
                            tr.InOut = 0;
                            tr.OrderId = order.Id;
                            tr.StaffId = list.FirstOrDefault().StaffId;
                            tr.TransactionType = 3;
                            Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
                            tr.Gross = t.Gross;
                            tr.Amount = t.Gross;
                            tr.Net = t.Net;
                            tr.Tax = t.Tax;
                            tr.Vat = t.Vat;
                            tr.OrderDetail = new List<OrderDetail>();
                            tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;

                            invoice.Total = invoice.Total != null ? invoice.Total + tr.Gross : tr.Gross;
                            invoice.Net = invoice.Net != null ? invoice.Net + tr.Net : tr.Net;
                            invoice.Vat = invoice.Vat != null ? invoice.Vat + tr.Vat : tr.Vat;
                            invoice.Tax = invoice.Tax != null ? invoice.Tax + tr.Tax : tr.Tax;
                            var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                            invoice.Discount = invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;
                            invoice.Transactions.Add(tr);
                            //db.Transactions.Add(tr);
                            //db.SaveChanges();
                            foreach (var paidDet in g.PaidOrderDetails)
                            {
                                var orderdetailinvoices = paidDet.OrderDetailInvoices.Where(w => w.PosInfoDetail.CreateTransaction == false && w.PosInfoDetail.IsInvoice == true);
                                if (orderdetailinvoices.Count() > 0)
                                {
                                    foreach (var orinv in orderdetailinvoices)
                                    {
                                        orinv.StaffId = list.FirstOrDefault().StaffId;
                                    }
                                }
                                // paidDet.TransactionId = tr.Id;
                            }
                            var hotel = db.HotelInfo.FirstOrDefault();
                            var account = db.Accounts.Find(tr.AccountId);
                            if (account != null && account.SendsTransfer == true && hotel != null)
                            {
                                //var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId);
                                //var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId);
                                //var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                                //var query = (from f in g.PaidOrderDetails
                                //             join st in db.SalesType on f.SalesTypeId equals st.Id
                                //             join jpr in db.PricelistDetail on f.PriceListDetailId equals jpr.Id
                                //             join jprl in db.Pricelist on jpr.PricelistId equals jprl.Id
                                //             join jord in db.Order on f.OrderId equals jord.Id
                                //             join jpos in db.PosInfo on jord.PosId equals jpos.Id
                                //             join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                //             on new { Product = f.ProductId, Pricelist = jprl.Id, PosDepartmentId = jpos.DepartmentId }
                                //             equals new { Product = tm.ProductId, Pricelist = tm.PriceListId.Value, PosDepartmentId = tm.PosDepartmentId } into loj
                                //             from ls in loj.DefaultIfEmpty()
                                //             select new
                                //             {
                                //                 Id = f.Id,
                                //                 SalesTypeId = st.Id,
                                //                 Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                //                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                //                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                //                 CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                //                 ReceiptNo = newCounter,
                                //             }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                //             {
                                //                 PmsDepartmentId = s.Key,
                                //                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                //                 Total = s.Sum(sm => sm.Total),
                                //                 CustomerId = s.FirstOrDefault().CustomerId,
                                //                 ReceiptNo = s.FirstOrDefault().ReceiptNo
                                //             });
                                var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId);
                                var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId);
                                var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                                var query = (from f in g.PaidOrderDetails
                                             join st in db.SalesType on f.SalesTypeId equals st.Id
                                             join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                             on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                             from ls in loj.DefaultIfEmpty()
                                             select new
                                             {
                                                 Id = f.Id,
                                                 SalesTypeId = st.Id,
                                                 Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                                 CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                                 ReceiptNo = newCounter,
                                             }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                             {
                                                 PmsDepartmentId = s.Key,
                                                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                                 Total = s.Sum(sm => sm.Total),
                                                 CustomerId = s.FirstOrDefault().CustomerId,
                                                 ReceiptNo = s.FirstOrDefault().ReceiptNo
                                             });
                                long guestid = tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1;

                                Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                                string storeid = HttpContext.Current.Request.Params["storeid"];
                                List<TransferObject> objTosendList = new List<TransferObject>();
                                List<TransferToPms> transferList = new List<TransferToPms>();

                                var IsCreditCard = false;
                                var roomOfCC = "";
                                if (account.Type == 4)
                                {
                                    IsCreditCard = true;
                                    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                                        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                                }

                                foreach (var acg in query)
                                {
                                    TransferToPms tpms = new TransferToPms(); //newCounter
                                    tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + order.PosInfo.Description;
                                    tpms.PmsDepartmentId = acg.PmsDepartmentId;
                                    tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                                    tpms.ProfileId = acg.CustomerId;
                                    tpms.ProfileName = !IsCreditCard ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                                    tpms.ReceiptNo = newCounter.ToString();
                                    tpms.RegNo = !IsCreditCard ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                                    tpms.RoomDescription = !IsCreditCard ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                                    tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                                    tpms.SendToPMS = true;
                                    tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                                    tpms.PosInfoId = order.PosId;
                                    //tpms.TransactionId = tr.Id;
                                    tpms.TransferType = 0;//Xrewstiko
                                    tpms.SendToPmsTS = DateTime.Now;
                                    tpms.Total = (decimal)acg.Total;
                                    var identifier = Guid.NewGuid();
                                    tpms.TransferIdentifier = identifier;
                                    transferList.Add(tpms);
                                    db.TransferToPms.Add(tpms);

                                    TransferObject to = new TransferObject();
                                    //
                                    to.TransferIdentifier = tpms.TransferIdentifier;
                                    //
                                    to.HotelId = (int)hotel.Id;
                                    to.amount = (decimal)tpms.Total;
                                    int PmsDepartmentId = 0;
                                    var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                                    to.departmentId = PmsDepartmentId;
                                    to.description = tpms.Description;
                                    to.profileName = tpms.ProfileName;
                                    int resid = 0;
                                    var toint = int.TryParse(tpms.RegNo, out resid);
                                    to.resId = resid;
                                    to.TransferIdentifier = identifier;
                                    to.HotelUri = hotel.HotelUri;
                                    to.RoomName = tpms.RoomDescription;
                                    if (IsCreditCard)
                                    {
                                        to.RoomName = roomOfCC;
                                    }

                                    if (to.amount != 0)
                                        objTosendList.Add(to);
                                }
                                tr.TransferToPms = new List<TransferToPms>();
                                tr.TransferToPms = transferList as ICollection<TransferToPms>;

                                db.SaveChanges();
                                foreach (var to in objTosendList)
                                {
                                    SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                    sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                                }
                            }
                            db.Transactions.Add(tr);
                        }
                        if (g.Status == 5 && g.PaidStatus == 2)//Einai Akyrwseis kai einai exoflimena
                        {
                            Transactions tr = new Transactions();
                            tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
                            tr.PosInfoId = order.PosInfo.Id;
                            //tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price) * -1;
                            tr.Day = DateTime.Now;
                            tr.DepartmentId = order.PosInfo.DepartmentId;
                            tr.Description = "Ακύρωση";
                            if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
                            {
                                tr.ExtDescription = "Μερική Ακύρωση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
                            }
                            tr.InOut = 1;//Εκροη
                            tr.OrderId = order.Id;
                            tr.StaffId = list.FirstOrDefault().StaffId;
                            tr.TransactionType = (short)TransactionType.Cancel;
                            Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
                            //tr.TransactionType = (short)TransactionType.Cancel;
                            tr.Gross = t.Gross;
                            tr.Amount = t.Gross;
                            tr.Net = t.Net;
                            tr.Tax = t.Tax;
                            tr.Vat = t.Vat;
                            tr.OrderDetail = new List<OrderDetail>();
                            tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;

                            foreach (var inv in tr.OrderDetail)
                            {
                                if (inv.OrderDetailInvoices != null && inv.OrderDetailInvoices.Count > 0)
                                {
                                    var invexofl = inv.OrderDetailInvoices.OrderByDescending(ob => ob.Id).FirstOrDefault().InvoicesId;
                                    if (invexofl != null)
                                    {
                                        var invoiceVoided = db.Invoices.Find(invexofl);
                                        if (invoiceVoided != null)
                                        {
                                            invoiceVoided.IsVoided = true;
                                        }
                                        break;
                                    }
                                }
                            }
                            invoice.Total = invoice.Total != null ? invoice.Total + (tr.Gross * -1) : (tr.Gross * -1);
                            invoice.Net = invoice.Net != null ? invoice.Net + (tr.Net * -1) : (tr.Net * -1);
                            invoice.Vat = invoice.Vat != null ? invoice.Vat + (tr.Vat * -1) : (tr.Vat * -1);
                            invoice.Tax = invoice.Tax != null ? invoice.Tax + (tr.Tax * -1) : (tr.Tax * -1);
                            var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                            invoice.Discount = invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;
                            invoice.Transactions.Add(tr);
                            //db.Transactions.Add(tr);
                            //db.SaveChanges();
                            var hotel = db.HotelInfo.FirstOrDefault();
                            var account = db.Accounts.Find(tr.AccountId);
                            if (account != null && account.SendsTransfer == true && hotel != null)
                            {
                                var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId);
                                var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId);
                                var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                                var query = (from f in g.PaidOrderDetails
                                             join st in db.SalesType on f.SalesTypeId equals st.Id
                                             join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                             on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                             from ls in loj.DefaultIfEmpty()
                                             select new
                                             {
                                                 Id = f.Id,
                                                 SalesTypeId = st.Id,
                                                 Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                                 CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                                 ReceiptNo = f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault() != null ? f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault().Counter : newCounter,
                                             }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                             {
                                                 PmsDepartmentId = s.Key,
                                                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                                 Total = s.Sum(sm => sm.Total),
                                                 CustomerId = s.FirstOrDefault().CustomerId,
                                                 ReceiptNo = s.FirstOrDefault().ReceiptNo
                                             });
                                //Customers curcustomer = list.FirstOrDefault().Customer;

                                long guestid = tr.OrderDetail.FirstOrDefault() != null ? tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1 : -1;

                                Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                                string storeid = HttpContext.Current.Request.Params["storeid"];
                                List<TransferObject> objTosendList = new List<TransferObject>();
                                List<TransferToPms> transferList = new List<TransferToPms>();

                                var IsCreditCard = false;
                                var roomOfCC = "";
                                if (account.Type == 4)
                                {
                                    IsCreditCard = true;
                                    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                                        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                                }
                                foreach (var acg in query)
                                {
                                    TransferToPms tpms = new TransferToPms(); // newCounter
                                    tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + order.PosInfo.Description;
                                    tpms.PmsDepartmentId = acg.PmsDepartmentId;
                                    tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                                    tpms.ProfileId = acg.CustomerId;
                                    tpms.ProfileName = !IsCreditCard ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                                    tpms.ReceiptNo = newCounter.ToString();
                                    tpms.RegNo = !IsCreditCard ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                                    tpms.RoomDescription = !IsCreditCard ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                                    tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                                    tpms.SendToPMS = true;
                                    tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                                    tpms.PosInfoId = order.PosId;
                                    //tpms.TransactionId = tr.Id;
                                    tpms.TransferType = 0;//Xrewstiko
                                    tpms.Total = (decimal)acg.Total * -1;
                                    tpms.SendToPmsTS = DateTime.Now;
                                    var identifier = Guid.NewGuid();
                                    tpms.TransferIdentifier = identifier;
                                    transferList.Add(tpms);
                                    db.TransferToPms.Add(tpms);

                                    TransferObject to = new TransferObject();
                                    //
                                    to.TransferIdentifier = tpms.TransferIdentifier;
                                    //
                                    to.HotelId = (int)hotel.Id;
                                    to.amount = (decimal)tpms.Total;
                                    int PmsDepartmentId = 0;
                                    var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                                    to.departmentId = PmsDepartmentId;
                                    to.description = tpms.Description;
                                    to.profileName = tpms.ProfileName;
                                    int resid = 0;
                                    var toint = int.TryParse(tpms.RegNo, out resid);
                                    to.resId = resid;
                                    to.TransferIdentifier = identifier;
                                    to.HotelUri = hotel.HotelUri;
                                    to.RoomName = tpms.RoomDescription;
                                    if (IsCreditCard)
                                    {
                                        to.RoomName = roomOfCC;
                                    }

                                    if (to.amount != 0)
                                        objTosendList.Add(to);
                                }
                                tr.TransferToPms = new List<TransferToPms>();
                                tr.TransferToPms = transferList as ICollection<TransferToPms>;

                                db.SaveChanges();
                                foreach (var to in objTosendList)
                                {
                                    SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                    sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                                }
                            }
                            db.Transactions.Add(tr);
                        }
                    }
                    if (g.Count == order.OrderDetail.Count) // OLA TA OrderDetail THS PARAGGELIAS EINAI STO GROUPAKI
                    {
                        //Add New Order Status
                        OrderStatus os = new OrderStatus();
                        os.OrderId = order.Id;
                        if (g.Status == 3 || g.Status == 2 || g.Status == 5)
                        {
                            os.Status = g.Status;
                        }
                        else
                        {
                            if (g.PaidStatus == 2)//paid
                            {
                                os.Status = 7;//"Εξοφλημένη";
                                if (order.OrderStatus.Where(w => w.Status == 8).Count() == 0)//AN EINAI EXOFLISI KAI DEN EXEI TIMOLOGITHEI
                                {
                                    OrderStatus os8 = new OrderStatus(); //PROSTHESE STATUS TIMOLOGISHS (8)
                                    os8.OrderId = order.Id;
                                    os8.Status = 8;
                                    os8.StaffId = list.FirstOrDefault().StaffId;
                                    os8.TimeChanged = DateTime.Now;
                                    db.OrderStatus.Add(os8);
                                }
                            }
                            else if (g.PaidStatus == 1)//invoiced
                            {
                                os.Status = 8;// "Τιμολογημένη";
                            }
                            else
                            {
                                os.Status = g.Status;
                            }
                        }
                        os.TimeChanged = DateTime.Now;
                        os.StaffId = list.FirstOrDefault().StaffId;
                        if (order.OrderStatus.Where(s => s.Status == os.Status).Count() == 0)//VALE TO NEO OrserStatus MONO AN DEN YPARXEI HDH
                        {
                            db.OrderStatus.Add(os);
                        }
                    }
                }
            }
            if (invoice.OrderDetailInvoices.Count > 0)
            {
                db.Invoices.Add(invoice);
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return ex.ToString();// Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var listupdate = OrderDet.OrderDet.Select(s => new
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

            try
            {
                if (OrderDet.CreditTransaction != null)
                {
                    Transactions tr = new Transactions();
                    var account = db.Accounts.Where(w => w.Type == 5).FirstOrDefault();
                    long? accountId = null;
                    if (account != null)
                    {
                        accountId = account.Id;
                    }
                    tr.AccountId = accountId;
                    tr.Amount = OrderDet.CreditTransaction.Amount;
                    tr.Day = DateTime.Now;
                    tr.DepartmentId = db.PosInfo.Find(OrderDet.CreditTransaction.PosInfoId) != null ? db.PosInfo.Find(OrderDet.CreditTransaction.PosInfoId).DepartmentId : null;
                    tr.Description = "Add amount on Barcode Credit Account";
                    tr.InOut = 0;
                    tr.PosInfoId = OrderDet.CreditTransaction.PosInfoId;
                    tr.StaffId = OrderDet.CreditTransaction.StaffId;
                    tr.TransactionType = (int)TransactionType.CreditCode;
                    tr.InvoicesId = invoice.Id;
                    db.Transactions.Add(tr);
                    db.CreditTransactions.Add(OrderDet.CreditTransaction);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return JsonConvert.SerializeObject(listupdate);//Request.CreateResponse(HttpStatusCode.Created, OrderDet.OrderDet);
        }
        */
        #endregion

        #region New UpdateOrderDetails With Invoices and Types
        /*
        public string PostOrderDetail(OrderDetailUpdateObjList OrderDet, bool updaterange, int type)
        {
            IEnumerable<OrderDetailUpdateObj> list = OrderDet.OrderDet;
            var od = list.Select(s => s.Id);
            List<OrderDetail> ordetlist = new List<OrderDetail>();
            List<OrderDetail> ordetPaidlist = new List<OrderDetail>();
            Invoices invoice = new Invoices();
            if (type == (int)OrderDetailUpdateType.Kds)
            {
                foreach (var item in OrderDet.OrderDet)
                {
                    if (item.Id != 0)
                    {
                        var orderdetail = db.OrderDetail.Include("Order").Where(w => w.Order.PosId == item.PosId && w.Order.EndOfDayId == null && w.Id == item.Id && (w.IsDeleted ?? false) == false).FirstOrDefault();
                        if (orderdetail != null)
                        {
                            orderdetail.Status = item.Status;
                            orderdetail.StatusTS = item.StatusTS;
                        }
                    }
                    else if (item.Guid != null)
                    {
                        var orderdetails = db.OrderDetail.Include("Order").Where(w => w.Order.PosId == item.PosId && w.Order.EndOfDayId == null && w.Guid == item.Guid && (w.IsDeleted ?? false) == false);
                        //An to kanei o diaolos kai exoun parei polla to idio Guid
                        foreach (var orderdetail in orderdetails)
                        {
                            orderdetail.Status = item.Status;
                            orderdetail.StatusTS = item.StatusTS;
                        }
                    }
                }
            }
            else
            {
                List<OrderDetail> ordetetailMappedList = db.OrderDetail.Include(i => i.OrderDetailIgredients).Include(i => i.OrderDetailVatAnal).Include("OrderDetailIgredients.OrderDetailIgredientVatAnal").Include(i => i.Order)
                    .Where(w => od.Contains(w.Id) && (w.IsDeleted ?? false) == false).ToList();
                decimal amount = 0;
                decimal? Netamount = 0;
                decimal? Vatamount = 0;
                decimal? Taxamount = 0;
                decimal? Discountamount = 0;
                decimal discount = list.FirstOrDefault().TableDiscount != null ? list.FirstOrDefault().TableDiscount.Value : 0;
                decimal percentage = 0;
                try
                {
                    if (discount > 0)
                    {
                        percentage = (discount / ordetetailMappedList.Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + (decimal)(sm.OrderDetailIgredients != null && sm.OrderDetailIgredients.Count > 0 ? sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0))));
                    }
                }
                catch (Exception ex)
                {

                }
                long updateReceiptNo = -1;
                int counter = 0;

                foreach (var o in list)
                {
                    counter++;
                    if (o.Guid != null)
                    {
                        var ord = ordetetailMappedList.Where(w => w.Guid == o.Guid).FirstOrDefault();//db.OrderDetail.Where(w => w.Id == o.Id).FirstOrDefault();
                        if (ord != null)
                        {
                            if (o.GuestId != null)
                            {
                                ord.GuestId = o.GuestId;
                            }
                            //if (o.StaffId != ord.Order.StaffId)
                            //{
                            //    ord.Order.StaffId = o.StaffId;
                            //}
                            if (o.Status != 7 && o.Status != 8)//Mhn kaneis UPDATE to status tou DETAIL gt kanei conflict sta TODO tou KDS
                            {
                                ord.Status = o.Status;
                            }
                            ord.StatusTS = DateTime.Now;
                            if (o.PaidStatus != null)
                                ord.PaidStatus = o.PaidStatus;
                            if (type == (int)OrderDetailUpdateType.PayOff)
                            {
                                if (o.Status == 7)//Exoflisi
                                {
                                    ord.PaidStatus = 2;//Paid
                                    //Change Pricelist OrderDetail
                                    if (o.NewPrlId != null)
                                    {
                                        var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
                                        if (pricelistdetail != null)
                                        {
                                            ord.PriceListDetailId = pricelistdetail.Id;
                                            ord.Price = pricelistdetail.Price;
                                            decimal tempprice = ord.Price != null ? Math.Round(ord.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                                            tempprice = ord.Qty != null && ord.Qty > 0 ? (decimal)(ord.Qty.Value) * tempprice : tempprice;
                                            ord.TotalAfterDiscount = tempprice;
                                            ord.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
                                        }
                                    }
                                    //DISCOUNT
                                    //percentage = percentage > 0 ? percentage : 0;
                                    //decimal newamount = Math.Round(ord.TotalAfterDiscount.Value * percentage, 4);
                                    //if (counter == list.Count())//An eimaste sto teleytaio dwse oti ekptwsh exei meinei gia na mhn exoume diafores me tis stroggylopoihseis
                                    //{
                                    //    newamount = discountleft;
                                    //}
                                    //discountleft = discountleft - newamount;
                                    ord.TotalAfterDiscount = o.TotalAfterDiscount;//ord.TotalAfterDiscount.Value - newamount;
                                    ord.Discount = o.Discount;//ord.Discount != null ? ord.Discount + newamount : newamount;

                                    foreach (var anal in ord.OrderDetailVatAnal)
                                    {
                                        //Change Pricelist OrderDetailVatAnal
                                        if (o.NewPrlId != null)
                                        {
                                            var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
                                            if (pricelistdetail != null)
                                            {
                                                anal.Gross = ord.TotalAfterDiscount;
                                                Vat vat = pricelistdetail.Vat;
                                                Tax tax = pricelistdetail.Tax;
                                                var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
                                                var tempvat = (decimal)(anal.Gross - tempnetbyvat);
                                                var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                                var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                                anal.Net = (decimal)(anal.Gross - tempvat - temptax);
                                                anal.TaxAmount = temptax;
                                                anal.VatAmount = tempvat;
                                                anal.VatRate = vat != null ? vat.Percentage : 0;
                                                anal.VatId = vat != null ? (long?)vat.Id : null;
                                                anal.TaxId = tax != null ? (long?)tax.Id : null;
                                            }
                                        }
                                        if (o.Discount != null && o.NewPrlId == null)
                                        {
                                            var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == ord.PriceListDetailId).FirstOrDefault();
                                            anal.Gross = ord.TotalAfterDiscount;
                                            Vat vat = pricelistdetail.Vat;
                                            Tax tax = pricelistdetail.Tax;
                                            var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
                                            var tempvat = (decimal)(anal.Gross - tempnetbyvat);
                                            var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                            var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                            anal.Net = (decimal)(anal.Gross - tempvat - temptax);
                                            anal.TaxAmount = temptax;
                                            anal.VatAmount = tempvat;
                                            anal.VatRate = vat != null ? vat.Percentage : 0;
                                            anal.VatId = vat != null ? (long?)vat.Id : null;
                                            anal.TaxId = tax != null ? (long?)tax.Id : null;
                                        }

                                        Netamount = Netamount != null ? Netamount + anal.Net : anal.Net;
                                        Vatamount = Vatamount != null ? Vatamount + anal.VatAmount : anal.VatAmount;
                                        Taxamount = Taxamount != null ? Taxamount + anal.TaxAmount : anal.TaxAmount;
                                    }
                                    foreach (var odi in ord.OrderDetailIgredients)
                                    {
                                        //Change Pricelist OrderDetailIgredients
                                        if (o.NewPrlId != null)
                                        {
                                            var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                                            if (pricelistdetail != null)
                                            {
                                                odi.PriceListDetailId = pricelistdetail.Id;
                                                odi.Price = pricelistdetail.Price;
                                                decimal tempprice = odi.Price != null ? Math.Round(odi.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                                                tempprice = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice : tempprice;
                                                odi.TotalAfterDiscount = tempprice;
                                                odi.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
                                            }
                                        }
                                        decimal newingamount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value * percentage), 2) : 0;
                                        odi.TotalAfterDiscount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value - newingamount), 2) : 0;
                                        odi.Discount = odi.Discount != null ? odi.Discount + newingamount : newingamount;
                                        amount += odi.TotalAfterDiscount.Value;
                                        Discountamount = Discountamount != null ? Discountamount + odi.Discount : odi.Discount;
                                        //Change Pricelist
                                        if (o.NewPrlId != null)
                                        {
                                            var pricelistdetailing = db.PricelistDetail.Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                                            if (pricelistdetailing != null)
                                            {
                                                odi.PriceListDetailId = pricelistdetailing.Id;
                                                odi.Price = pricelistdetailing.Price;
                                            }
                                        }
                                        foreach (var anal2 in odi.OrderDetailIgredientVatAnal)
                                        {
                                            //Change Pricelist OrderDetailIgredientVatAnal
                                            if (o.NewPrlId != null)
                                            {
                                                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                                                if (pricelistdetail != null)
                                                {
                                                    anal2.Gross = odi.TotalAfterDiscount;
                                                    Vat vat = pricelistdetail.Vat;
                                                    Tax tax = pricelistdetail.Tax;
                                                    var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
                                                    var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
                                                    var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                                    var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                                    anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
                                                    anal2.TaxAmount = temptax;
                                                    anal2.VatAmount = tempvat;
                                                    anal2.VatRate = vat != null ? vat.Percentage : 0;
                                                    anal2.VatId = vat != null ? (long?)vat.Id : null;
                                                    anal2.TaxId = tax != null ? (long?)tax.Id : null;
                                                }
                                            }
                                            //Discount on  OrderDetailIgredientVatAnal
                                            if (odi.Discount != null && o.NewPrlId == null)
                                            {
                                                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault();
                                                if (pricelistdetail != null)
                                                {
                                                    anal2.Gross = odi.TotalAfterDiscount;
                                                    Vat vat = pricelistdetail.Vat;
                                                    Tax tax = pricelistdetail.Tax;
                                                    var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
                                                    var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
                                                    var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                                                    var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                                                    anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
                                                    anal2.TaxAmount = temptax;
                                                    anal2.VatAmount = tempvat;
                                                    anal2.VatRate = vat != null ? vat.Percentage : 0;
                                                    anal2.VatId = vat != null ? (long?)vat.Id : null;
                                                    anal2.TaxId = tax != null ? (long?)tax.Id : null;
                                                }
                                            }
                                            Netamount = Netamount != null ? Netamount + anal2.Net : anal2.Net;
                                            Vatamount = Vatamount != null ? Vatamount + anal2.VatAmount : anal2.VatAmount;
                                            Taxamount = Taxamount != null ? Taxamount + anal2.TaxAmount : anal2.TaxAmount;
                                        }
                                    }
                                    amount += ord.TotalAfterDiscount.Value;//- newamount;
                                    Discountamount = Discountamount != null ? Discountamount + ord.Discount : ord.Discount;
                                    ordetPaidlist.Add(ord);
                                }
                            }
                            if (o.Status == 5 && o.PaidStatus == 2)
                            {
                                ordetPaidlist.Add(ord);
                            }
                            ordetlist.Add(ord);
                            invoice.ClientPosId = ord.Order.ClientPosId;
                            invoice.PdaModuleId = ord.Order.PdaModuleId;
                            invoice.PosInfoId = o.PosId;
                            invoice.GuestId = o.GuestId;
                            invoice.StaffId = o.StaffId;
                            invoice.TableId = ord.TableId;
                            if (type == (int)OrderDetailUpdateType.UnPaidCancel || type == (int)OrderDetailUpdateType.PaidCancel)//Akyrwsh mh exoflimenwn
                            {
                                //if (o.Status == 5 && o.PaidStatus != 2)
                                //{
                                decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
                                invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
                                decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
                                invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
                                decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
                                invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
                                decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
                                invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
                                var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;
                                invoice.Discount = invoice.Discount != null ? invoice.Discount + ord.Discount + ingrDis : ord.Discount + ingrDis;
                                if (o.OrderDetailInvoices.Count > 0)
                                {
                                    invoice.IsPrinted = o.OrderDetailInvoices.FirstOrDefault().IsPrinted;
                                }
                                //}
                            }
                        }
                    }
                    if (type != (int)OrderDetailUpdateType.Kds)
                    {
                        if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                        {
                            invoice.Day = o.OrderDetailInvoices.FirstOrDefault().CreationTS;
                            invoice.Counter = (int?)o.OrderDetailInvoices.FirstOrDefault().Counter;
                            invoice.PosInfoDetailId = o.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId;
                            foreach (var i in o.OrderDetailInvoices)
                            {
                                invoice.OrderDetailInvoices.Add(i);
                                db.OrderDetailInvoices.Add(i);
                                updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
                            }
                        }
                        else//EINAI EKSOFLISH TIMOLOGIMENWN (EXEI VGEI INVOICE PIO PRIN)
                        {
                            if (type == (int)OrderDetailUpdateType.PayOff)
                            {

                            }
                        }
                    }
                }
                List<Transactions> curTransactions = new List<Transactions>();
                if (type == (int)OrderDetailUpdateType.PayOff || type == (int)OrderDetailUpdateType.PaidCancel)
                {
                    var posinfo = db.PosInfo.Find(list.FirstOrDefault().PosId);
                    if (OrderDet.AccountsObj != null)
                    {
                        foreach (var a in OrderDet.AccountsObj)
                        {
                            Transactions tr = new Transactions();
                            tr.AccountId = a.AccountId;
                            tr.PosInfoId = posinfo.Id;
                            tr.Amount = type == (int)OrderDetailUpdateType.PayOff ? a.Amount : a.Amount * -1;
                            tr.Day = DateTime.Now;
                            tr.DepartmentId = posinfo.DepartmentId;
                            tr.Description = type == (int)OrderDetailUpdateType.PayOff ? "Pay Off" : "Cancel receipt";
                            tr.ExtDescription = "Helper Controller";
                            tr.InOut = type == (int)OrderDetailUpdateType.PayOff ? (short)0 : (short)1;
                            //tr.OrderId = order.Id;
                            tr.StaffId = list.FirstOrDefault().StaffId;
                            tr.TransactionType = type == (int)OrderDetailUpdateType.PayOff ? (short)TransactionType.Sale : (short)TransactionType.Cancel;
                            //Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
                            //tr.Gross = t.Gross;
                            //tr.Amount = t.Gross;
                            //tr.Net = t.Net;
                            //tr.Tax = t.Tax;
                            //tr.Vat = t.Vat;
                            //tr.OrderDetail = new List<OrderDetail>();
                            //tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;
                            if (type == (int)OrderDetailUpdateType.PayOff)
                            {
                                invoice.Total = amount;//invoice.Total != null ? invoice.Total + tr.Gross : tr.Gross;
                                invoice.Net = Netamount;//invoice.Net != null ? invoice.Net + tr.Net : tr.Net;
                                invoice.Vat = Vatamount;//invoice.Vat != null ? invoice.Vat + tr.Vat : tr.Vat;
                                invoice.Tax = Taxamount;//invoice.Tax != null ? invoice.Tax + tr.Tax : tr.Tax;
                                //var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                                invoice.Discount = Discountamount;//invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;

                                if (a.GuestId != null)
                                {
                                    Invoice_Guests_Trans igt = new Invoice_Guests_Trans();
                                    igt.GuestId = a.GuestId;
                                    if (tr.Invoice_Guests_Trans == null)
                                    {
                                        tr.Invoice_Guests_Trans = new List<Invoice_Guests_Trans>();
                                    }
                                    tr.Invoice_Guests_Trans.Add(igt);
                                    if (invoice.Invoice_Guests_Trans == null)
                                    {
                                        invoice.Invoice_Guests_Trans = new List<Invoice_Guests_Trans>();
                                    }
                                    invoice.Invoice_Guests_Trans.Add(igt);
                                }
                            }
                            invoice.Transactions.Add(tr);
                            //db.Transactions.Add(tr);
                            curTransactions.Add(tr);
                        }
                    }
                    else
                    {
                        Transactions tr = new Transactions();
                        tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : posinfo.AccountId;
                        tr.PosInfoId = posinfo.Id;
                        tr.Amount = type == (int)OrderDetailUpdateType.PayOff ? amount : amount * -1;
                        tr.Day = DateTime.Now;
                        tr.DepartmentId = posinfo.DepartmentId;
                        tr.Description = type == (int)OrderDetailUpdateType.PayOff ? "Pay Off" : "Cancel receipt";
                        tr.ExtDescription = "Helper Controller";
                        tr.InOut = type == (int)OrderDetailUpdateType.PayOff ? (short)0 : (short)1;
                        //tr.OrderId = order.Id;
                        tr.StaffId = list.FirstOrDefault().StaffId;
                        tr.TransactionType = type == (int)OrderDetailUpdateType.PayOff ? (short)TransactionType.Sale : (short)TransactionType.Cancel;
                        //Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
                        //tr.Gross = t.Gross;
                        //tr.Amount = t.Gross;
                        //tr.Net = t.Net;
                        //tr.Tax = t.Tax;
                        //tr.Vat = t.Vat;
                        //tr.OrderDetail = new List<OrderDetail>();
                        //tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;
                        if (type == (int)OrderDetailUpdateType.PayOff)
                        {
                            invoice.Total = amount;//invoice.Total != null ? invoice.Total + tr.Gross : tr.Gross;
                            invoice.Net = Netamount;//invoice.Net != null ? invoice.Net + tr.Net : tr.Net;
                            invoice.Vat = Vatamount;//invoice.Vat != null ? invoice.Vat + tr.Vat : tr.Vat;
                            invoice.Tax = Taxamount;//invoice.Tax != null ? invoice.Tax + tr.Tax : tr.Tax;
                            //var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                            invoice.Discount = Discountamount;//invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;
                            if (invoice.GuestId != null)
                            {
                                Invoice_Guests_Trans igt = new Invoice_Guests_Trans();
                                igt.GuestId = invoice.GuestId;
                                tr.Invoice_Guests_Trans = new List<Invoice_Guests_Trans>();
                                tr.Invoice_Guests_Trans.Add(igt);
                                invoice.Invoice_Guests_Trans = new List<Invoice_Guests_Trans>();
                                invoice.Invoice_Guests_Trans.Add(igt);
                            }
                        }
                        invoice.Transactions.Add(tr);
                        //db.Transactions.Add(tr);
                        curTransactions.Add(tr);
                    }
                }
                //bool isFromKds = ordetlist.All(a => a.Status == 2 || a.Status == 3);

                long newCounter = -1;
                var pid = new PosInfoDetail();
                if (type != (int)OrderDetailUpdateType.Kds)
                {
                    invoice.Cover = ordetetailMappedList.GroupBy(g => g.OrderId).Sum(s => s.FirstOrDefault().Couver);
                    //db.SaveChanges();
                    pid = db.PosInfoDetail.FirstOrDefault();
                    if (updateReceiptNo > -1)
                    {
                        pid = db.PosInfoDetail.Where(w => w.Id == updateReceiptNo).FirstOrDefault();
                        long onlineCounter = pid.Counter != null ? (pid.Counter.Value + 1) : 1;
                        newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : onlineCounter;
                        pid.Counter = newCounter;
                        var posinfo = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == pid.PosInfoId).FirstOrDefault();
                        if (posinfo != null)
                        {
                            var piDetGroup = posinfo.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == pid.GroupId);
                            foreach (var i in piDetGroup)
                            {
                                i.Counter = newCounter;
                            }
                        }
                        invoice.Counter = (int)newCounter;
                        invoice.Description = pid.Description;
                        invoice.InvoiceTypeId = pid.InvoicesTypeId;

                    }
                }
                var group = ordetlist.ToList().GroupBy(g => g.OrderId).Select(s => new
                {
                    OrderId = s.Key,
                    OrderDetailStatus = s.FirstOrDefault().Status
                });

                foreach (var o in group)
                {
                    var order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Include("OrderDetail.OrderDetailInvoices")
                        .Include("OrderDetail.OrderDetailInvoices.PosInfoDetail").Include("OrderStatus").Include("OrderDetail.PricelistDetail")
                        .Include(i => i.PosInfo).SingleOrDefault(s => s.Id == o.OrderId);
                    var gr = order.OrderDetail.Where(w=> (w.IsDeleted ?? false) == false).GroupBy(g => new { g.PaidStatus, g.Status }).Select(s => new
                    {
                        PaidStatus = s.Key.PaidStatus,
                        Status = s.Key.Status,
                        Count = s.Count(),
                        PaidOrderDetails = s.Where(w => ordetPaidlist.Contains(w)).AsEnumerable<OrderDetail>(),
                        OrderDetails = s
                    });
                    foreach (var g in gr)
                    {
                        if (type != (int)OrderDetailUpdateType.Kds)//DEN EXEI ERTHEI APO KDS TO STATUS
                        {
                            if (type == (int)OrderDetailUpdateType.PayOff)
                            {
                                if (g.Status != 5 && g.PaidStatus == 2 && order.OrderStatus.Where(w => w.Status == 7).Count() == 0)//Paid && AN DEN YPARXEI HDH STATUS EXOFLISIS
                                {

                                    foreach (var paidDet in g.PaidOrderDetails)
                                    {
                                        var orderdetailinvoices = paidDet.OrderDetailInvoices.Where(w => w.PosInfoDetail.CreateTransaction == false && w.PosInfoDetail.IsInvoice == true);
                                        if (orderdetailinvoices.Count() > 0)
                                        {
                                            foreach (var orinv in orderdetailinvoices)
                                            {
                                                orinv.StaffId = list.FirstOrDefault().StaffId;
                                            }
                                        }
                                        // paidDet.TransactionId = tr.Id;
                                    }
                                    long accountId = list.FirstOrDefault().AccountId.Value;
                                    var hotel = db.HotelInfo.FirstOrDefault();
                                    List<AccountsObj> accountsList = new List<AccountsObj>();
                                    if (OrderDet.AccountsObj != null)
                                    {
                                        foreach (var a in OrderDet.AccountsObj)
                                        {
                                            var acc = db.Accounts.Find(a.AccountId);
                                            if (acc != null)
                                            {
                                                AccountsObj accobj = new AccountsObj();
                                                accobj.Account = acc;
                                                accobj.AccountId = a.AccountId;
                                                accobj.Amount = a.Amount;
                                                accobj.GuestId = a.GuestId;
                                                accountsList.Add(accobj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        AccountsObj accobj = new AccountsObj();
                                        accobj.Account = db.Accounts.Find(accountId);
                                        accobj.AccountId = accountId;
                                        accobj.Amount = g.PaidOrderDetails.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                                        accobj.GuestId = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;
                                        accountsList.Add(accobj);
                                    }
                                    decimal TotalAmounts = accountsList.Sum(s => s.Amount);
                                    foreach (var ac in accountsList)
                                    {
                                        var account = ac.Account;
                                        if (account != null && hotel != null)//&& account.SendsTransfer == true (tha chekaristei otan einai na stalei)
                                        {

                                            var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId);
                                            var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId);
                                            var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                                            var query = (from f in g.PaidOrderDetails
                                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                                         join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                                         on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                                         from ls in loj.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             Id = f.Id,
                                                             SalesTypeId = st.Id,
                                                             Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                                             OrderDetail = f,
                                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                                             // CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                                             ReceiptNo = newCounter,
                                                         }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                                         {
                                                             PmsDepartmentId = s.Key,
                                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                                             Total = s.Sum(sm => sm.Total),
                                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                                                             //CustomerId = s.FirstOrDefault().CustomerId,
                                                             ReceiptNo = s.FirstOrDefault().ReceiptNo
                                                         });
                                            // long guestid = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;//tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1;
                                            long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;
                                            Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                                            string storeid = HttpContext.Current.Request.Params["storeid"];
                                            List<TransferObject> objTosendList = new List<TransferObject>();
                                            List<TransferToPms> transferList = new List<TransferToPms>();

                                            //var IsCreditCard = false;
                                            //var roomOfCC = "";
                                            //if (account.Type == 4)
                                            //{
                                            //    IsCreditCard = true;
                                            //    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                                            //        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                                            //}
                                            var IsNotSendingTransfer = account.SendsTransfer == false;
                                            var IsCreditCard = false;
                                            var roomOfCC = "";
                                            if (account.Type == 4 || IsNotSendingTransfer == true)
                                            {
                                                IsCreditCard = true;
                                                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                                            }


                                            //EpimerismosPerDepartment
                                            decimal totalDiscount = TotalAmounts - ac.Amount;
                                            decimal percentageEpim = 1 - (decimal)(ac.Amount / TotalAmounts);
                                            decimal totalEpim = 0;
                                            decimal remainingDiscount = totalDiscount;
                                            decimal ctr = 1;
                                            List<dynamic> query2 = new List<dynamic>();
                                            query = query.OrderBy(or => or.Total);
                                            foreach (var f in query)
                                            {
                                                if (ctr < query.Count())
                                                {
                                                    decimal subtotal = f.Total;
                                                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                                                    totalEpim += subtotal - percSub;
                                                    query2.Add(new
                                                    {
                                                        PmsDepartmentId = f.PmsDepartmentId,
                                                        Total = subtotal - percSub
                                                    });
                                                    remainingDiscount = remainingDiscount - percSub;
                                                }
                                                else
                                                {
                                                    decimal subtotal = f.Total;
                                                    query2.Add(new
                                                    {
                                                        PmsDepartmentId = f.PmsDepartmentId,
                                                        Total = subtotal - remainingDiscount
                                                    });
                                                    totalEpim += subtotal - remainingDiscount;
                                                }
                                                ctr++;
                                            }
                                            //
                                            var query3 = from q in query
                                                         join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                                                         select new
                                                         {
                                                             PmsDepartmentId = q.PmsDepartmentId,
                                                             PmsDepDescription = q.PmsDepDescription,
                                                             Total = j.Total,
                                                             OrderDetails = q.OrderDetails,
                                                             //CustomerId = q.CustomerId,
                                                             ReceiptNo = q.ReceiptNo
                                                         };
                                            //
                                            foreach (var acg in query3)
                                            {
                                                decimal total = acg.Total;//new Economic().EpimerisiAccountTotal(acg.OrderDetails, acg.Total);
                                                TransferToPms tpms = new TransferToPms(); //newCounter
                                                tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + order.PosInfo.Description;
                                                tpms.PmsDepartmentId = acg.PmsDepartmentId;
                                                tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                                                tpms.ProfileId = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ProfileNo.ToString() : "" : null;//acg.CustomerId;
                                                tpms.ProfileName = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                                                tpms.ReceiptNo = newCounter.ToString();
                                                tpms.RegNo = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                                                tpms.RoomDescription = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                                                tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                                                tpms.SendToPMS =  !IsNotSendingTransfer;
                                                //Set Status Flag (0: Cash, 1: Not Cash)
                                                tpms.Status = IsNotSendingTransfer ? (short)0 : (short)1;
                                                tpms.PosInfoId = order.PosInfo.Id;
                                                tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                                                //tpms.TransactionId = tr.Id;
                                                tpms.TransferType = 0;//Xrewstiko
                                                tpms.SendToPmsTS = DateTime.Now;
                                                tpms.Total = total;// (decimal)acg.Total;
                                                var identifier = Guid.NewGuid();
                                                tpms.TransferIdentifier = identifier;
                                                transferList.Add(tpms);
                                                db.TransferToPms.Add(tpms);

                                                //Link transferToPms with correct Transaction
                                                var trans = curTransactions.Where(w => w.AccountId == ac.AccountId).FirstOrDefault();
                                                if (trans != null)
                                                {
                                                    trans.TransferToPms.Add(tpms);
                                                }


                                                TransferObject to = new TransferObject();
                                                //
                                                to.TransferIdentifier = tpms.TransferIdentifier;
                                                //
                                                to.HotelId = (int)hotel.Id;
                                                to.amount = (decimal)tpms.Total;
                                                int PmsDepartmentId = 0;
                                                var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                                                to.departmentId = PmsDepartmentId;
                                                to.description = tpms.Description;
                                                to.profileName = tpms.ProfileName;
                                                int resid = 0;
                                                var toint = int.TryParse(tpms.RegNo, out resid);
                                                to.resId = resid;
                                                to.TransferIdentifier = identifier;
                                                to.HotelUri = hotel.HotelUri;
                                                to.RoomName = tpms.RoomDescription;
                                                if (IsCreditCard)
                                                {
                                                    to.RoomName = roomOfCC;
                                                }

                                                if (to.amount != 0 && ac.Account.SendsTransfer == true)
                                                    objTosendList.Add(to);
                                            }


                                            //tr.TransferToPms = new List<TransferToPms>();
                                            //tr.TransferToPms = transferList as ICollection<TransferToPms>;

                                            db.SaveChanges();
                                            foreach (var to in objTosendList)
                                            {
                                                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                                sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                                            }
                                        }
                                    }
                                    //db.Transactions.Add(tr);
                                }
                            }
                            if (type == (int)OrderDetailUpdateType.PaidCancel)
                            {
                                if (g.Status == 5 && g.PaidStatus == 2)//Einai Akyrwseis kai einai exoflimena
                                {
                                    //Transactions tr = new Transactions();
                                    //tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
                                    //tr.PosInfoId = order.PosInfo.Id;
                                    ////tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price) * -1;
                                    //tr.Day = DateTime.Now;
                                    //tr.DepartmentId = order.PosInfo.DepartmentId;
                                    //tr.Description = "Ακύρωση";
                                    //if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
                                    //{
                                    //    tr.ExtDescription = "Μερική Ακύρωση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
                                    //}
                                    //tr.InOut = 1;//Εκροη
                                    //tr.OrderId = order.Id;
                                    //tr.StaffId = list.FirstOrDefault().StaffId;
                                    //tr.TransactionType = (short)TransactionType.Cancel;
                                    //Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
                                    ////tr.TransactionType = (short)TransactionType.Cancel;
                                    //tr.Gross = t.Gross;
                                    //tr.Amount = t.Gross;
                                    //tr.Net = t.Net;
                                    //tr.Tax = t.Tax;
                                    //tr.Vat = t.Vat;
                                    //tr.OrderDetail = new List<OrderDetail>();
                                    //tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;

                                    foreach (var inv in g.PaidOrderDetails)
                                    {
                                        if (inv.OrderDetailInvoices != null && inv.OrderDetailInvoices.Count > 0)
                                        {
                                            var invexofl = inv.OrderDetailInvoices.Where(w=> (w.IsDeleted ?? false) == false).OrderByDescending(ob => ob.Id).FirstOrDefault().InvoicesId;
                                            if (invexofl != null)
                                            {
                                                var invoiceVoided = db.Invoices.Find(invexofl);
                                                if (invoiceVoided != null)
                                                {
                                                    invoiceVoided.IsVoided = true;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    //invoice.Total = invoice.Total != null ? invoice.Total + (tr.Gross * -1) : (tr.Gross * -1);
                                    //invoice.Net = invoice.Net != null ? invoice.Net + (tr.Net * -1) : (tr.Net * -1);
                                    //invoice.Vat = invoice.Vat != null ? invoice.Vat + (tr.Vat * -1) : (tr.Vat * -1);
                                    //invoice.Tax = invoice.Tax != null ? invoice.Tax + (tr.Tax * -1) : (tr.Tax * -1);
                                    //var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                                    //invoice.Discount = invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;
                                    //invoice.Transactions.Add(tr);
                                    //db.Transactions.Add(tr);
                                    //db.SaveChanges();
                                    var hotel = db.HotelInfo.FirstOrDefault();
                                    List<AccountsObj> accountsList = new List<AccountsObj>();
                                    long accountId = list.FirstOrDefault().AccountId.Value;
                                    if (OrderDet.AccountsObj != null)
                                    {
                                        foreach (var a in OrderDet.AccountsObj)
                                        {
                                            var acc = db.Accounts.Find(a.AccountId);
                                            if (acc != null)
                                            {
                                                AccountsObj accobj = new AccountsObj();
                                                accobj.Account = acc;
                                                accobj.AccountId = a.AccountId;
                                                accobj.Amount = a.Amount;
                                                accobj.GuestId = a.GuestId;
                                                accountsList.Add(accobj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        AccountsObj accobj = new AccountsObj();
                                        accobj.Account = db.Accounts.Find(accountId);
                                        accobj.AccountId = accountId;
                                        accobj.Amount = g.PaidOrderDetails.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                                        accobj.GuestId = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;
                                        accountsList.Add(accobj);
                                    }
                                    decimal TotalAmounts = accountsList.Sum(s => s.Amount);
                                    foreach (var ac in accountsList)
                                    {
                                        var account = ac.Account;
                                        if (account != null && account.SendsTransfer == true && hotel != null)
                                        {
                                            var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId).Distinct();
                                            var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId).Distinct();
                                            var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                                            var query = (from f in g.PaidOrderDetails
                                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                                         join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                                         on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                                         from ls in loj.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             Id = f.Id,
                                                             SalesTypeId = st.Id,
                                                             Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                                             OrderDetail = f,
                                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                                             //CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                                             ReceiptNo = f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault() != null ? f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault().Counter : newCounter,
                                                         }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                                         {
                                                             PmsDepartmentId = s.Key,
                                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                                             Total = s.Sum(sm => sm.Total),
                                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                                                             // CustomerId = s.FirstOrDefault().CustomerId,
                                                             ReceiptNo = s.FirstOrDefault().ReceiptNo
                                                         });
                                            //Customers curcustomer = list.FirstOrDefault().Customer;

                                            //long guestid = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;// tr.OrderDetail.FirstOrDefault() != null ? tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1 : -1;
                                            long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;
                                            Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                                            string storeid = HttpContext.Current.Request.Params["storeid"];
                                            List<TransferObject> objTosendList = new List<TransferObject>();
                                            List<TransferToPms> transferList = new List<TransferToPms>();

                                            var IsCreditCard = false;
                                            var roomOfCC = "";
                                            if (account.Type == 4)
                                            {
                                                IsCreditCard = true;
                                                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                                            }
                                            //EpimerismosPerDepartment
                                            decimal totalDiscount = TotalAmounts - ac.Amount;
                                            decimal percentageEpim = 1 - (decimal)(ac.Amount / TotalAmounts);
                                            decimal totalEpim = 0;
                                            decimal remainingDiscount = totalDiscount;
                                            decimal ctr = 1;
                                            List<dynamic> query2 = new List<dynamic>();
                                            query = query.OrderBy(or => or.Total);
                                            foreach (var f in query)
                                            {
                                                if (ctr < query.Count())
                                                {
                                                    decimal subtotal = f.Total;
                                                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                                                    totalEpim += subtotal - percSub;
                                                    query2.Add(new
                                                    {
                                                        PmsDepartmentId = f.PmsDepartmentId,
                                                        Total = subtotal - percSub
                                                    });
                                                    remainingDiscount = remainingDiscount - percSub;
                                                }
                                                else
                                                {
                                                    decimal subtotal = f.Total;
                                                    query2.Add(new
                                                    {
                                                        PmsDepartmentId = f.PmsDepartmentId,
                                                        Total = subtotal - remainingDiscount
                                                    });
                                                    totalEpim += subtotal - remainingDiscount;
                                                }
                                                ctr++;
                                            }
                                            //
                                            var query3 = from q in query
                                                         join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                                                         select new
                                                         {
                                                             PmsDepartmentId = q.PmsDepartmentId,
                                                             PmsDepDescription = q.PmsDepDescription,
                                                             Total = j.Total,
                                                             OrderDetails = q.OrderDetails,
                                                             //CustomerId = q.CustomerId,
                                                             ReceiptNo = q.ReceiptNo
                                                         };
                                            //
                                            foreach (var acg in query3)
                                            {
                                                TransferToPms tpms = new TransferToPms(); // newCounter
                                                tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + order.PosInfo.Description;
                                                tpms.PmsDepartmentId = acg.PmsDepartmentId;
                                                tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                                                tpms.ProfileId = !IsCreditCard ? curcustomer != null ? curcustomer.ProfileNo.ToString() : "" : null;//acg.CustomerId;
                                                tpms.ProfileName = !IsCreditCard ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                                                tpms.ReceiptNo = newCounter.ToString();
                                                tpms.RegNo = !IsCreditCard ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                                                tpms.RoomDescription = !IsCreditCard ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                                                tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                                                tpms.SendToPMS = true;
                                                tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                                                //tpms.TransactionId = tr.Id;
                                                tpms.TransferType = 0;//Xrewstiko
                                                tpms.Total = (decimal)acg.Total * -1;
                                                tpms.SendToPmsTS = DateTime.Now;
                                                var identifier = Guid.NewGuid();
                                                tpms.TransferIdentifier = identifier;
                                                transferList.Add(tpms);
                                                db.TransferToPms.Add(tpms);

                                                //Link transferToPms with correct Transaction
                                                var trans = curTransactions.Where(w => w.AccountId == ac.AccountId).FirstOrDefault();
                                                if (trans != null)
                                                {
                                                    trans.TransferToPms.Add(tpms);
                                                }

                                                TransferObject to = new TransferObject();
                                                //
                                                to.TransferIdentifier = tpms.TransferIdentifier;
                                                //
                                                to.HotelId = (int)hotel.Id;
                                                to.amount = (decimal)tpms.Total;
                                                int PmsDepartmentId = 0;
                                                var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                                                to.departmentId = PmsDepartmentId;
                                                to.description = tpms.Description;
                                                to.profileName = tpms.ProfileName;
                                                int resid = 0;
                                                var toint = int.TryParse(tpms.RegNo, out resid);
                                                to.resId = resid;
                                                to.TransferIdentifier = identifier;
                                                to.HotelUri = hotel.HotelUri;
                                                to.RoomName = tpms.RoomDescription;
                                                if (IsCreditCard)
                                                {
                                                    to.RoomName = roomOfCC;
                                                }

                                                if (to.amount != 0)
                                                    objTosendList.Add(to);
                                            }
                                            //tr.TransferToPms = new List<TransferToPms>();
                                            //tr.TransferToPms = transferList as ICollection<TransferToPms>;

                                            db.SaveChanges();
                                            foreach (var to in objTosendList)
                                            {
                                                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                                sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                                            }
                                        }
                                    }
                                    //db.Transactions.Add(tr);
                                }
                            }
                        }
                        if (g.Count == order.OrderDetail.Count) // OLA TA OrderDetail THS PARAGGELIAS EINAI STO GROUPAKI
                        {
                            //Add New Order Status
                            OrderStatus os = new OrderStatus();
                            os.OrderId = order.Id;
                            if (g.Status == 3 || g.Status == 2 || g.Status == 5)
                            {
                                os.Status = g.Status;
                            }
                            else
                            {
                                if (g.PaidStatus == 2)//paid
                                {
                                    os.Status = 7;//"Εξοφλημένη";
                                    if (order.OrderStatus.Where(w => w.Status == 8).Count() == 0)//AN EINAI EXOFLISI KAI DEN EXEI TIMOLOGITHEI
                                    {
                                        OrderStatus os8 = new OrderStatus(); //PROSTHESE STATUS TIMOLOGISHS (8)
                                        os8.OrderId = order.Id;
                                        os8.Status = 8;
                                        os8.StaffId = list.FirstOrDefault().StaffId;
                                        os8.TimeChanged = DateTime.Now;
                                        db.OrderStatus.Add(os8);
                                    }
                                }
                                else if (g.PaidStatus == 1)//invoiced
                                {
                                    os.Status = 8;// "Τιμολογημένη";
                                }
                                else
                                {
                                    os.Status = g.Status;
                                }
                            }
                            os.TimeChanged = DateTime.Now;
                            os.StaffId = list.FirstOrDefault().StaffId;
                            if (order.OrderStatus.Where(s => s.Status == os.Status).Count() == 0)//VALE TO NEO OrserStatus MONO AN DEN YPARXEI HDH
                            {
                                db.OrderStatus.Add(os);
                            }
                        }
                    }
                }

                if (invoice.OrderDetailInvoices.Count > 0)
                {
                    db.Invoices.Add(invoice);
                }
                if (curTransactions.Count > 0)
                {
                    foreach (var t in curTransactions)
                    {
                        db.Transactions.Add(t);
                    }
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return ex.ToString();// Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var listupdate = OrderDet.OrderDet.Select(s => new
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
            try
            {
                if (OrderDet.CreditTransaction != null)
                {
                    Transactions tr = new Transactions();
                    var account = db.Accounts.Where(w => w.Type == 5).FirstOrDefault();
                    long? accountId = null;
                    if (account != null)
                    {
                        accountId = account.Id;
                    }
                    tr.AccountId = accountId;
                    tr.Amount = OrderDet.CreditTransaction.Amount;
                    tr.Day = DateTime.Now;
                    tr.DepartmentId = db.PosInfo.Find(OrderDet.CreditTransaction.PosInfoId) != null ? db.PosInfo.Find(OrderDet.CreditTransaction.PosInfoId).DepartmentId : null;
                    tr.Description = "Add amount on Barcode Credit Account";
                    tr.InOut = 0;
                    tr.PosInfoId = OrderDet.CreditTransaction.PosInfoId;
                    tr.StaffId = OrderDet.CreditTransaction.StaffId;
                    tr.TransactionType = (int)TransactionType.CreditCode;
                    tr.InvoicesId = invoice.Id;
                    db.Transactions.Add(tr);
                    db.CreditTransactions.Add(OrderDet.CreditTransaction);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return JsonConvert.SerializeObject(listupdate);//Request.CreateResponse(HttpStatusCode.Created, OrderDet.OrderDet);
        }
        */

        #endregion

        #region New UpdateOrderDetails With Invoices and Types and Exoflisi me ekdomenh apodeiksh

        //private string PostOrderDetailbyArt(OrderDetailUpdateObjList OrderDet, bool updaterange, int type, bool a = false)
        //{
        //    db.Configuration.LazyLoadingEnabled = true;
        //    try
        //    {
        //        switch ((OrderDetailUpdateType)type)
        //        {
        //            case OrderDetailUpdateType.Kds:
        //                foreach (var item in OrderDet.OrderDet)
        //                {
        //                    if (item.Id != 0)
        //                    {
        //                        var orderdetail = db.OrderDetail.Include("Order").Where(w => w.Order.PosId == item.PosId && w.Order.EndOfDayId == null && w.Id == item.Id && (w.IsDeleted ?? false) == false).FirstOrDefault();
        //                        if (orderdetail != null)
        //                        {
        //                            orderdetail.Status = item.Status;
        //                            orderdetail.StatusTS = item.StatusTS;
        //                        }
        //                    }
        //                    else if (item.Guid != null)
        //                    {
        //                        var orderdetails = db.OrderDetail.Include("Order").Where(w => w.Order.PosId == item.PosId && w.Order.EndOfDayId == null && w.Guid == item.Guid && (w.IsDeleted ?? false) == false);
        //                        //TODO: Change Pricelist recalculate 
        //                        //var cpl = OrderDet.OrderDet.Where(w => w.NewPrlId != null);
        //                        //if (cpl != null)
        //                        //    ChangePriceList(cpl, 1);

        //                        //An to kanei o diaolos kai exoun parei polla to idio Guid
        //                        foreach (var orderdetail in orderdetails)
        //                        {
        //                            orderdetail.Status = item.Status;
        //                            orderdetail.StatusTS = item.StatusTS;
        //                        }
        //                    }
        //                }
        //                break;
        //            case OrderDetailUpdateType.PaidCancel:

        //                break;
        //            case OrderDetailUpdateType.PayOff:
        //                var od = OrderDet.OrderDet.Select(s => s.Id);
        //                List<OrderDetail> ordetetailMappedList = db.OrderDetail.Where(w => od.Contains(w.Id) && (w.IsDeleted ?? false) == false).ToList();
        //                var odis = OrderDet.OrderDet.SelectMany(sm => sm.OrderDetailInvoices);
        //                var nodis = ordetetailMappedList.SelectMany(sm => sm.OrderDetailInvoices).Select(s => new
        //                {
        //                    InvoiceId = s.InvoicesId,
        //                    OrderDetailId = s.OrderDetail.Id,
        //                    ODTotalAfterDiscount = s.OrderDetail.TotalAfterDiscount,
        //                    ODStatus = s.OrderDetail.Status,
        //                    ODPaidStatus = s.OrderDetail.PaidStatus,
        //                    ODDiscount = s.OrderDetail.Discount,
        //                    Total = s.Invoices.Total,
        //                    Discount = s.Invoices.Discount,
        //                    InvoiceType = s.Invoices.InvoiceTypes.Type,
        //                    PosInfoId = s.Invoices.PosInfoId,
        //                    ProductId = s.OrderDetail.ProductId,


        //                });
        //                //Stelnei mono mi akyrwmena kai mi eksoflimena gia timologisi
        //                Invoices invoicedOrders = CreateInvoiceForOrder(nodis.Where(w => w.InvoiceType == (int)InvoiceTypesEnum.Order && w.ODStatus != 5 && w.ODPaidStatus != 2), OrderDet.OrderDet.FirstOrDefault().StaffId.Value);
        //                var asa = AmountSharing(invoicedOrders.Total.Value, OrderDet.AccountsObj);
        //                var invoicesToBePaid = nodis.Where(w => w.InvoiceType == (int)InvoiceTypesEnum.Receipt && w.ODStatus != 5 && w.ODPaidStatus != 2);

        //                break;
        //            case OrderDetailUpdateType.UnPaidCancel:
        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //    finally
        //    {
        //        db.Configuration.LazyLoadingEnabled = false;
        //    }
        //    return "";
        //}

        //private void PayOffOnly()
        //{
        //}
        private void CreateTransactionSendTransfer(Invoices inv, IEnumerable<AccountsObj> accObjs)
        {
            //Create transaction for each object in AccountObj

        }

        private List<AccountsObj> AmountSharing(Decimal TotalAmount, IEnumerable<AccountsObj> accObjs)
        {
            var result = new List<AccountsObj>();
            decimal totalDiscount = TotalAmount - accObjs.Sum(sm => sm.Amount);
            decimal percentageEpim = 1 - (decimal)(accObjs.Sum(sm => sm.Amount) / TotalAmount);
            decimal totalEpim = 0;
            decimal remainingDiscount = totalDiscount;
            decimal ctr = 1;

            foreach (var ac in accObjs)
            {



                if (ctr < accObjs.Count())
                {
                    decimal subtotal = ac.Amount;
                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                    totalEpim += subtotal - percSub;
                    result.Add(new AccountsObj
                    {
                        AccountId = ac.AccountId,
                        Amount = subtotal - percSub
                    });
                    remainingDiscount = remainingDiscount - percSub;
                }
                else
                {
                    decimal subtotal = ac.Amount;
                    result.Add(new AccountsObj
                    {
                        AccountId = ac.AccountId,
                        Amount = subtotal - remainingDiscount
                    });
                    totalEpim += subtotal - remainingDiscount;
                }
                ctr++;
            }

            return result;
        }

        /// <summary>
        /// E
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Invoices CreateInvoiceForOrder(IEnumerable<dynamic> list, Int64 staffId)
        {
            Int64 piid = list.FirstOrDefault().PosInfoId;
            PosInfo pi = db.PosInfo.FirstOrDefault(w => w.Id == piid);

            PosInfoDetail pid = db.PosInfoDetail.FirstOrDefault(w => w.PosInfoId == piid && w.IsInvoice == true && w.CreateTransaction == false && w.IsCancel == false);
            pid.Counter++;

            Invoices inv = new Invoices();
            inv.Total = list.Sum(sm => (decimal?)sm.ODTotalAfterDiscount);
            inv.Discount = list.Sum(sm => (decimal?)sm.ODDiscount);
            inv.PosInfoDetailId = (Int64?)pid.Id;
            inv.Counter = (Int32?)pid.Counter;
            inv.StaffId = staffId;
            foreach (var item in list)
            {
                OrderDetailInvoices odi = new OrderDetailInvoices();

                odi.OrderDetailId = item.OrderDetailId;
                odi.Counter = inv.Counter;
                odi.CreationTS = DateTime.Now;
                odi.PosInfoDetailId = pid.Id;
                odi.StaffId = staffId;
                //db.OrderDetailInvoices.Add(odi);
                inv.OrderDetailInvoices.Add(odi);

            }

            return inv;
        }

        private void ChangePriceList(IEnumerable<OrderDetailUpdateObjList> list, Int64 newPricelistId)
        {
            //foreach (var ord in list)
            //{
            //    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
            //    if (pricelistdetail != null)
            //    {
            //        ord.PriceListDetailId = pricelistdetail.Id;
            //        ord.Price = pricelistdetail.Price;
            //        decimal tempprice = ord.Price != null ? Math.Round(ord.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
            //        tempprice = ord.Qty != null && ord.Qty > 0 ? (decimal)(ord.Qty.Value) * tempprice : tempprice;
            //        ord.TotalAfterDiscount = tempprice;
            //        ord.Discount = null;//NO PRIOR DISCOUNT ALLOWED 


            //        ord.TotalAfterDiscount = o.TotalAfterDiscount;//ord.TotalAfterDiscount.Value - newamount;
            //        ord.Discount = o.Discount;//ord.Discount != null ? ord.Discount + newamount : newamount;

            //        foreach (var anal in ord.OrderDetailVatAnal)
            //        {
            //            //Change Pricelist OrderDetailVatAnal
            //            if (o.NewPrlId != null)
            //            {
            //                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
            //                if (pricelistdetail != null)
            //                {
            //                    anal.Gross = ord.TotalAfterDiscount;
            //                    Vat vat = pricelistdetail.Vat;
            //                    Tax tax = pricelistdetail.Tax;
            //                    var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
            //                    var tempvat = (decimal)(anal.Gross - tempnetbyvat);
            //                    var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
            //                    var temptax = (decimal)(tempnetbyvat - tempnetbytax);
            //                    anal.Net = (decimal)(anal.Gross - tempvat - temptax);
            //                    anal.TaxAmount = temptax;
            //                    anal.VatAmount = tempvat;
            //                    anal.VatRate = vat != null ? vat.Percentage : 0;
            //                    anal.VatId = vat != null ? (long?)vat.Id : null;
            //                    anal.TaxId = tax != null ? (long?)tax.Id : null;
            //                }
            //            }
            //            if (o.Discount != null && o.NewPrlId == null)
            //            {
            //                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == ord.PriceListDetailId).FirstOrDefault();
            //                anal.Gross = ord.TotalAfterDiscount;
            //                Vat vat = pricelistdetail.Vat;
            //                Tax tax = pricelistdetail.Tax;
            //                var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
            //                var tempvat = (decimal)(anal.Gross - tempnetbyvat);
            //                var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
            //                var temptax = (decimal)(tempnetbyvat - tempnetbytax);
            //                anal.Net = (decimal)(anal.Gross - tempvat - temptax);
            //                anal.TaxAmount = temptax;
            //                anal.VatAmount = tempvat;
            //                anal.VatRate = vat != null ? vat.Percentage : 0;
            //                anal.VatId = vat != null ? (long?)vat.Id : null;
            //                anal.TaxId = tax != null ? (long?)tax.Id : null;
            //            }

            //            Netamount = Netamount != null ? Netamount + anal.Net : anal.Net;
            //            Vatamount = Vatamount != null ? Vatamount + anal.VatAmount : anal.VatAmount;
            //            Taxamount = Taxamount != null ? Taxamount + anal.TaxAmount : anal.TaxAmount;
            //        }
            //        foreach (var odi in ord.OrderDetailIgredients)
            //        {
            //            //Change Pricelist OrderDetailIgredients
            //            if (o.NewPrlId != null)
            //            {
            //                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
            //                if (pricelistdetail != null)
            //                {
            //                    odi.PriceListDetailId = pricelistdetail.Id;
            //                    odi.Price = pricelistdetail.Price;
            //                    decimal tempprice = odi.Price != null ? Math.Round(odi.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
            //                    tempprice = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice : tempprice;
            //                    odi.TotalAfterDiscount = tempprice;
            //                    odi.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
            //                }
            //            }
            //            decimal newingamount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value * percentage), 2) : 0;
            //            odi.TotalAfterDiscount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value - newingamount), 2) : 0;
            //            odi.Discount = odi.Discount != null ? odi.Discount + newingamount : newingamount;
            //            amount += odi.TotalAfterDiscount.Value;
            //            Discountamount = Discountamount != null ? Discountamount + odi.Discount : odi.Discount;
            //            //Change Pricelist
            //            if (o.NewPrlId != null)
            //            {
            //                var pricelistdetailing = db.PricelistDetail.Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
            //                if (pricelistdetailing != null)
            //                {
            //                    odi.PriceListDetailId = pricelistdetailing.Id;
            //                    odi.Price = pricelistdetailing.Price;
            //                }
            //            }
            //            foreach (var anal2 in odi.OrderDetailIgredientVatAnal)
            //            {
            //                //Change Pricelist OrderDetailIgredientVatAnal
            //                if (o.NewPrlId != null)
            //                {
            //                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
            //                    if (pricelistdetail != null)
            //                    {
            //                        anal2.Gross = odi.TotalAfterDiscount;
            //                        Vat vat = pricelistdetail.Vat;
            //                        Tax tax = pricelistdetail.Tax;
            //                        var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
            //                        var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
            //                        var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
            //                        var temptax = (decimal)(tempnetbyvat - tempnetbytax);
            //                        anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
            //                        anal2.TaxAmount = temptax;
            //                        anal2.VatAmount = tempvat;
            //                        anal2.VatRate = vat != null ? vat.Percentage : 0;
            //                        anal2.VatId = vat != null ? (long?)vat.Id : null;
            //                        anal2.TaxId = tax != null ? (long?)tax.Id : null;
            //                    }
            //                }
            //                //Discount on  OrderDetailIgredientVatAnal
            //                if (odi.Discount != null && o.NewPrlId == null)
            //                {
            //                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault();
            //                    if (pricelistdetail != null)
            //                    {
            //                        anal2.Gross = odi.TotalAfterDiscount;
            //                        Vat vat = pricelistdetail.Vat;
            //                        Tax tax = pricelistdetail.Tax;
            //                        var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
            //                        var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
            //                        var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
            //                        var temptax = (decimal)(tempnetbyvat - tempnetbytax);
            //                        anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
            //                        anal2.TaxAmount = temptax;
            //                        anal2.VatAmount = tempvat;
            //                        anal2.VatRate = vat != null ? vat.Percentage : 0;
            //                        anal2.VatId = vat != null ? (long?)vat.Id : null;
            //                        anal2.TaxId = tax != null ? (long?)tax.Id : null;
            //                    }
            //                }
            //                //Netamount = Netamount != null ? Netamount + anal2.Net : anal2.Net;
            //                //Vatamount = Vatamount != null ? Vatamount + anal2.VatAmount : anal2.VatAmount;
            //                //Taxamount = Taxamount != null ? Taxamount + anal2.TaxAmount : anal2.TaxAmount;
            //            }
            //        }
            //        //amount += ord.TotalAfterDiscount.Value;//- newamount;
            //        //Discountamount = Discountamount != null ? Discountamount + ord.Discount : ord.Discount;

            //    }
            //}
        }


        private dynamic PostOrderByInvoiceType(IEnumerable<OrderDetailUpdateObj> list, List<OrderDetail> ordetetailMappedList, bool updaterange, OrderDetailUpdateObjList OrderDet, int type)
        {
            var od = list.Select(s => s.Id);
            List<OrderDetail> ordetlist = new List<OrderDetail>();
            List<OrderDetail> ordetPaidlist = new List<OrderDetail>();
            Invoices invoice = new Invoices();
            Invoices oldInvoice = new Invoices();

            if (OrderDet.AccountsObj != null)
            {
                //hereIam
                decimal sums = (decimal)ordetetailMappedList.Sum(sm => sm.TotalAfterDiscount);
                decimal accobjsums = OrderDet.AccountsObj.Sum(sm => sm.Amount);
                decimal percentageEpim = accobjsums == 0 ? 0 : 1 - (decimal)(sums / accobjsums);

                decimal remainingDiscount = accobjsums - sums;
                // decimal ctr = 1;
                decimal subtotal = sums;
                foreach (var epimItem in OrderDet.AccountsObj)
                {
                    epimItem.Amount -= Math.Round((decimal)(epimItem.Amount * percentageEpim), 2);
                    remainingDiscount = remainingDiscount - epimItem.Amount;
                }


            }
            decimal amount = 0;
            decimal? Netamount = 0;
            decimal? Vatamount = 0;
            decimal? Taxamount = 0;
            decimal? Discountamount = 0;
            decimal discount = list.FirstOrDefault().TableDiscount != null ? list.FirstOrDefault().TableDiscount.Value : 0;
            decimal percentage = 0;

            #region Where Magic Starts
            //EINAI EKSOFLISH TIMOLOGIMENWN (EXEI VGEI INVOICE PIO PRIN)
            if (type == (int)OrderDetailUpdateType.PayOff)
            {
                //VRES TO POSINFODETAIL ME TO PRWTO INVOICE POU EXEI
                var ordetinvoice = ordetetailMappedList.FirstOrDefault().OrderDetailInvoices.FirstOrDefault();
                if (ordetinvoice != null)
                {

                    oldInvoice = ordetinvoice.Invoices;

                    //SHMANTIKOS ELEGXOS
                    //PAEI NA KANEI EXOFLISH HDH EXOFLIMENWN
                    var groupedDetailsByInvoice = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).GroupBy(g => g.InvoicesId).Select(s => new
                    {
                        InvoicesId = s.Key,
                        Invoices = s.FirstOrDefault().Invoices
                    });
                    foreach (var i in groupedDetailsByInvoice)
                    {
                        if (i.Invoices.Transactions.Count > 0 && i.Invoices.Total <= i.Invoices.Transactions.Sum(s => s.Amount))
                        {
                            return null;
                        }
                    }

                }
            }

            long updateReceiptNo = -1;
            int counter = 0;
            long newCounter = -1;
            newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : newCounter;
            foreach (var o in list)
            {
                counter++;
                if (o.Guid != null)
                {

                    if (type == (int)OrderDetailUpdateType.PaidCancel || type == (int)OrderDetailUpdateType.UnPaidCancel)
                    {
                        if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                        {
                            var id = o.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId;
                            var pidi = db.PosInfoDetail.Include(i => i.PosInfo).Where(w => w.Id == id).FirstOrDefault();
                            invoice.Day = o.OrderDetailInvoices.FirstOrDefault().CreationTS;
                            invoice.Counter = (int?)o.OrderDetailInvoices.FirstOrDefault().Counter;
                            invoice.PosInfoDetailId = o.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId;
                            invoice.InvoiceTypeId = pidi.InvoicesTypeId;
                            invoice.Description = pidi.Description;
                            if (pidi.Counter < invoice.Counter)
                                pidi.Counter = invoice.Counter;
                            foreach (var i in o.OrderDetailInvoices)
                            {
                                invoice.OrderDetailInvoices.Add(i);
                                db.OrderDetailInvoices.Add(i);
                                updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
                            }
                        }
                    }
                    var ord = ordetetailMappedList.Where(w => w.Guid == o.Guid).FirstOrDefault();//db.OrderDetail.Where(w => w.Id == o.Id).FirstOrDefault();
                    if (ord != null)
                    {
                        if (o.GuestId != null)
                        {
                            ord.GuestId = o.GuestId;
                        }
                        //if (o.StaffId != ord.Order.StaffId)
                        //{
                        //    ord.Order.StaffId = o.StaffId;
                        //}
                        if (o.Status != 7 && o.Status != 8)//Mhn kaneis UPDATE to status tou DETAIL gt kanei conflict sta TODO tou KDS
                        {
                            ord.Status = o.Status;
                        }
                        ord.StatusTS = DateTime.Now;
                        if (o.PaidStatus != null)
                            ord.PaidStatus = o.PaidStatus;
                        if (type == (int)OrderDetailUpdateType.PayOff)
                        {
                            if (o.Status == 7)//Exoflisi
                                AdjustPricesWithNewPricelist(true, ordetPaidlist, ref amount, ref Netamount, ref Vatamount, ref Taxamount, ref Discountamount, percentage, o, ord);
                        }
                        if (type == (int)OrderDetailUpdateType.InvoiceOnly)
                        {
                            //if (o.Status == 7)//Exoflisi
                            AdjustPricesWithNewPricelist(false, ordetPaidlist, ref amount, ref Netamount, ref Vatamount, ref Taxamount, ref Discountamount, percentage, o, ord);
                        }
                        if (o.Status == 5 && o.PaidStatus == 2)
                        {
                            ordetPaidlist.Add(ord);
                        }
                        ordetlist.Add(ord);


                        invoice.ClientPosId = ord.Order.ClientPosId;
                        invoice.PdaModuleId = ord.Order.PdaModuleId;
                        invoice.PosInfoId = o.PosId;
                        invoice.GuestId = o.GuestId;
                        invoice.StaffId = o.StaffId;
                        invoice.TableId = ord.TableId;
                        if (type == (int)OrderDetailUpdateType.UnPaidCancel || type == (int)OrderDetailUpdateType.PaidCancel)//Akyrwsh mh exoflimenwn
                        {
                            //if (o.Status == 5 && o.PaidStatus != 2)
                            //{
                            decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
                            invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
                            decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
                            invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
                            decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
                            invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
                            decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
                            invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
                            var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;
                            invoice.Discount = invoice.Discount != null ? invoice.Discount + ord.Discount + ingrDis : ord.Discount + ingrDis;
                            if (o.OrderDetailInvoices.Count > 0)
                            {
                                invoice.IsPrinted = o.OrderDetailInvoices.FirstOrDefault().IsPrinted;
                            }
                            //}
                        }
                    }
                }
                if (type != (int)OrderDetailUpdateType.Kds)
                {
                    //if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                    //{
                    //    invoice.Day = o.OrderDetailInvoices.FirstOrDefault().CreationTS;
                    //    invoice.Counter = (int?)o.OrderDetailInvoices.FirstOrDefault().Counter;
                    //    invoice.PosInfoDetailId = o.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId;
                    //    foreach (var i in o.OrderDetailInvoices)
                    //    {
                    //        invoice.OrderDetailInvoices.Add(i);
                    //        db.OrderDetailInvoices.Add(i);
                    //        updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
                    //    }
                    //}
                    //else//EINAI EKSOFLISH TIMOLOGIMENWN (EXEI VGEI INVOICE PIO PRIN)
                    {
                        if (type == (int)OrderDetailUpdateType.PayOff)
                        {
                            //VRES TO POSINFODETAIL ME TO PRWTO INVOICE POU EXEI
                            var ordetinvoice = ordetetailMappedList.FirstOrDefault().OrderDetailInvoices.FirstOrDefault();
                            if (ordetinvoice != null)
                            {
                                updateReceiptNo = ordetinvoice.PosInfoDetailId != null ? ordetinvoice.PosInfoDetailId.Value : -1;
                                newCounter = ordetinvoice.Counter.Value; //Vale ReceiptNo iso me to palio Invoice Exoflisis
                                //oldInvoice = ordetinvoice.Invoices;

                                ////SHMANTIKOS ELEGXOS
                                ////PAEI NA KANEI EXOFLISH HDH EXOFLIMENWN
                                //var groupedDetailsByInvoice = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).GroupBy(g => g.InvoicesId).Select(s => new
                                //{
                                //    InvoicesId = s.Key,
                                //    Invoices = s.FirstOrDefault().Invoices
                                //});
                                //foreach (var i in groupedDetailsByInvoice)
                                //{
                                //    if (i.Invoices.Transactions.Count > 0 && i.Invoices.Total >= i.Invoices.Transactions.Sum(s => s.Amount))
                                //    {
                                //        return null;
                                //    }
                                //}
                            }
                        }

                    }
                }
            }
            //move after creating invoice
            //               List<Transactions> curTransactions = new List<Transactions>();
            List<Transactions> curTransactions = CreateTransactionsWithTransfer(OrderDet, type, list, invoice, oldInvoice, ordetetailMappedList, amount, Netamount, Vatamount, Taxamount, Discountamount, updateReceiptNo, newCounter);
            //               curTransactions = CreateTransactionsWithTransfer(OrderDet, type, list, invoice, oldInvoice, ordetetailMappedList, amount, Netamount, Vatamount, Taxamount, Discountamount, updateReceiptNo, newCounter);

            //bool isFromKds = ordetlist.All(a => a.Status == 2 || a.Status == 3);


            var pid = new PosInfoDetail();
            //if (type != (int)OrderDetailUpdateType.Kds)
            //{
            //    invoice.Cover = ordetetailMappedList.GroupBy(g => g.OrderId).Sum(s => s.FirstOrDefault().Couver);
            //    //db.SaveChanges();
            pid = db.PosInfoDetail.Include(i => i.PosInfo).FirstOrDefault();
            //    if (updateReceiptNo > -1)
            //    {
            //        pid = db.PosInfoDetail.Include(i => i.PosInfo).Where(w => w.Id == updateReceiptNo).FirstOrDefault();
            //        long onlineCounter = pid.Counter != null ? (pid.Counter.Value + 1) : 1;
            //        //newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : onlineCounter;
            //        newCounter = newCounter != -1 ? newCounter : onlineCounter;
            //        pid.Counter = newCounter;
            //        var posinfo = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == pid.PosInfoId).FirstOrDefault();
            //        if (posinfo != null)
            //        {
            //            var piDetGroup = posinfo.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == pid.GroupId);
            //            foreach (var i in piDetGroup)
            //            {
            //                i.Counter = newCounter;
            //            }
            //        }
            //        invoice.Counter = (int)newCounter;
            //        invoice.Description = pid.Description;
            //        invoice.InvoiceTypeId = pid.InvoicesTypeId;

            //    }
            //}



            #region Group By Order Updates

            var group = ordetlist.ToList().GroupBy(g => g.OrderId).Select(s => new
            {
                OrderId = s.Key,
                OrderDetailStatus = s.FirstOrDefault().Status
            });

            foreach (var o in group)
            {
                //Na ferei mono timologimena
                var order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Include("OrderDetail.OrderDetailInvoices.Invoices.InvoiceTypes")
                    .Include("OrderDetail.OrderDetailInvoices.PosInfoDetail").Include("OrderStatus").Include("OrderDetail.PricelistDetail")
                    .Include(i => i.PosInfo).SingleOrDefault(s => s.Id == o.OrderId);
                var gr = order.OrderDetail.Where(w => (w.IsDeleted ?? false) == false).GroupBy(g => new { g.PaidStatus, g.Status }).Select(s => new
                {
                    PaidStatus = s.Key.PaidStatus,
                    Status = s.Key.Status,
                    Count = s.Count(),
                    PaidOrderDetails = s.Where(w => ordetPaidlist.Contains(w)).AsEnumerable<OrderDetail>(),
                    OrderDetails = s
                });
                foreach (var g in gr)
                {
                    if (type != (int)OrderDetailUpdateType.Kds)//DEN EXEI ERTHEI APO KDS TO STATUS
                    {

                        if (type == (int)OrderDetailUpdateType.PaidCancel)
                        {
                            #region PaidCancel
                            if (g.Status == 5 && g.PaidStatus == 2)//Einai Akyrwseis kai einai exoflimena
                            {
                                foreach (var inv in g.PaidOrderDetails)
                                {
                                    if (inv.OrderDetailInvoices != null && inv.OrderDetailInvoices.Count > 0)
                                    {
                                        var invexofl = inv.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false).OrderByDescending(ob => ob.Id).FirstOrDefault().InvoicesId;
                                        if (invexofl != null)
                                        {
                                            var invoiceVoided = db.Invoices.Find(invexofl);
                                            if (invoiceVoided != null)
                                            {
                                                invoiceVoided.IsVoided = true;
                                            }
                                            break;
                                        }
                                    }
                                }
                                var aaa = curTransactions;
                                var hotel = db.HotelInfo.FirstOrDefault();
                                List<AccountsObj> accountsList = new List<AccountsObj>();
                                long accountId = list.FirstOrDefault().AccountId.Value;
                                if (OrderDet.AccountsObj != null)
                                {
                                    foreach (var a in OrderDet.AccountsObj)
                                    {
                                        var acc = db.Accounts.Find(a.AccountId);
                                        if (acc != null)
                                        {
                                            AccountsObj accobj = new AccountsObj();
                                            accobj.Account = acc;
                                            accobj.AccountId = a.AccountId;
                                            accobj.Amount = a.Amount;
                                            accobj.GuestId = a.GuestId;
                                            accountsList.Add(accobj);
                                        }
                                    }
                                }
                                else
                                {
                                    AccountsObj accobj = new AccountsObj();
                                    accobj.Account = db.Accounts.Find(accountId);
                                    accobj.AccountId = accountId;
                                    accobj.Amount = g.PaidOrderDetails.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                                    accobj.GuestId = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;
                                    accountsList.Add(accobj);
                                }
                                decimal TotalAmounts = accountsList.Sum(s => s.Amount);
                                // decimal TotalAmounts = g.PaidOrderDetails.Sum(sm=>sm.TotalAfterDiscount.Value);
                                foreach (var ac in accountsList)
                                {
                                    var account = ac.Account;
                                    if (account != null && account.SendsTransfer == true && hotel != null)
                                    {
                                        var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId).Distinct();
                                        var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId).Distinct();
                                        var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                                        var query = (from f in g.PaidOrderDetails
                                                     join st in db.SalesType on f.SalesTypeId equals st.Id
                                                     join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                                     on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                                     from ls in loj.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         Id = f.Id,
                                                         SalesTypeId = st.Id,
                                                         Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                                         OrderDetail = f,
                                                         PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                                         PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                                         //CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                                         ReceiptNo = f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault() != null ? f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault().Counter : newCounter,
                                                     }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                                     {
                                                         PmsDepartmentId = s.Key,
                                                         PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                                         Total = s.Sum(sm => sm.Total),
                                                         OrderDetails = s.Select(ss => ss.OrderDetail),
                                                         // CustomerId = s.FirstOrDefault().CustomerId,
                                                         ReceiptNo = s.FirstOrDefault().ReceiptNo
                                                     });
                                        //Customers curcustomer = list.FirstOrDefault().Customer;

                                        //long guestid = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;// tr.OrderDetail.FirstOrDefault() != null ? tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1 : -1;
                                        long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;
                                        Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                                        string storeid = HttpContext.Current.Request.Params["storeid"];
                                        List<TransferObject> objTosendList = new List<TransferObject>();
                                        List<TransferToPms> transferList = new List<TransferToPms>();

                                        var IsCreditCard = false;
                                        var roomOfCC = "";
                                        if (account.Type == 4)
                                        {
                                            IsCreditCard = true;
                                            roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                                                db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                                        }
                                        //EpimerismosPerDepartment
                                        decimal totalDiscount = TotalAmounts - ac.Amount;
                                        decimal percentageEpim = TotalAmounts == 0 ? 0 : 1 - (decimal)(ac.Amount / TotalAmounts);
                                        decimal totalEpim = 0;
                                        decimal remainingDiscount = totalDiscount;
                                        decimal ctr = 1;
                                        List<dynamic> query2 = new List<dynamic>();
                                        query = query.OrderBy(or => or.Total);
                                        foreach (var f in query)
                                        {
                                            if (ctr < query.Count())
                                            {
                                                decimal subtotal = f.Total;
                                                decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                                                totalEpim += subtotal - percSub;
                                                query2.Add(new
                                                {
                                                    PmsDepartmentId = f.PmsDepartmentId,
                                                    Total = subtotal - percSub
                                                });
                                                remainingDiscount = remainingDiscount - percSub;
                                            }
                                            else
                                            {
                                                decimal subtotal = f.Total;
                                                decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                                                totalEpim += subtotal - percSub;
                                                query2.Add(new
                                                {
                                                    PmsDepartmentId = f.PmsDepartmentId,
                                                    Total = subtotal - percSub
                                                });
                                                remainingDiscount = remainingDiscount - percSub;


                                                //decimal subtotal = f.Total;
                                                //query2.Add(new
                                                //{
                                                //    PmsDepartmentId = f.PmsDepartmentId,
                                                //    Total = subtotal - remainingDiscount
                                                //});
                                                //totalEpim += subtotal - remainingDiscount;
                                            }
                                            ctr++;
                                        }
                                        //
                                        var query3 = from q in query
                                                     join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                                                     select new
                                                     {
                                                         PmsDepartmentId = q.PmsDepartmentId,
                                                         PmsDepDescription = q.PmsDepDescription,
                                                         Total = j.Total,
                                                         OrderDetails = q.OrderDetails,
                                                         //CustomerId = q.CustomerId,
                                                         ReceiptNo = q.ReceiptNo
                                                     };
                                        //
                                        var departmentDescritpion = db.Department.FirstOrDefault(f => f.Id == order.PosInfo.DepartmentId);
                                        string depstr = departmentDescritpion != null ? departmentDescritpion.Description : order.PosInfo.Description;

                                        foreach (var acg in query3)
                                        {
                                            TransferToPms tpms = new TransferToPms(); // newCounter
                                            tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + depstr;//order.PosInfo.Description;
                                            tpms.PmsDepartmentId = acg.PmsDepartmentId;
                                            tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                                            tpms.ProfileId = !IsCreditCard ? curcustomer != null ? curcustomer.ProfileNo.ToString() : "" : null;//acg.CustomerId;
                                            tpms.ProfileName = !IsCreditCard ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                                            tpms.ReceiptNo = newCounter.ToString();
                                            tpms.RegNo = !IsCreditCard ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                                            tpms.RoomDescription = !IsCreditCard ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                                            tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                                            tpms.SendToPMS = true;
                                            tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                                            tpms.PosInfoId = order.PosId;
                                            //tpms.TransactionId = tr.Id;
                                            tpms.TransferType = 0;//Xrewstiko
                                            tpms.Total = (decimal)acg.Total * -1;
                                            tpms.SendToPmsTS = DateTime.Now;
                                            var identifier = Guid.NewGuid();
                                            tpms.TransferIdentifier = identifier;
                                            transferList.Add(tpms);
                                            db.TransferToPms.Add(tpms);

                                            //Link transferToPms with correct Transaction
                                            var trans = curTransactions.Where(w => w.AccountId == ac.AccountId).FirstOrDefault();
                                            if (trans != null)
                                            {
                                                trans.TransferToPms.Add(tpms);
                                            }

                                            TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, identifier);

                                            if (to.amount != 0)
                                                objTosendList.Add(to);
                                        }
                                        //tr.TransferToPms = new List<TransferToPms>();
                                        //tr.TransferToPms = transferList as ICollection<TransferToPms>;

                                        db.SaveChanges();
                                        foreach (var to in objTosendList)
                                        {
                                            SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                           // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                                        }
                                    }
                                }
                                //db.Transactions.Add(tr);
                            }
                            #endregion
                        }
                        //if (type == (int)OrderDetailUpdateType.InvoiceOnly)
                        //{
                        //    var ord = ordetetailMappedList.FirstOrDefault();
                        //    decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
                        //    invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
                        //    decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
                        //    invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
                        //    decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
                        //    invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
                        //    decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
                        //    invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
                        //    var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;


                        //    invoice.Total = ordetetailMappedList.Sum(s => s.TotalAfterDiscount) + ingrGross;
                        //    invoice.Discount = ordetetailMappedList.Sum(s => s.Discount) + ingrDis;
                        //    invoice.Vat = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm => sm.VatAmount)) + ingrVat;
                        //    invoice.Tax = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm => sm.TaxAmount)) + ingrTax;
                        //    invoice.Net = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm => sm.Net)) + ingrNet;

                        //}
                        if (type == (int)OrderDetailUpdateType.UnPaidCancel)
                        {
                            foreach (var inv in ordetetailMappedList)
                            {
                                if (inv.OrderDetailInvoices != null && inv.OrderDetailInvoices.Count > 0)
                                {
                                    var invexofl = inv.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false).OrderByDescending(ob => ob.Id).FirstOrDefault().InvoicesId;
                                    if (invexofl != null)
                                    {
                                        var invoiceVoided = db.Invoices.Include(i => i.Transactions).Where(w => w.Id == invexofl).FirstOrDefault();
                                        if (invoiceVoided != null)
                                        {
                                            invoiceVoided.IsVoided = true;
                                            //An den einai eksoflimeni, eksoflise tin prwta vazwntas ta transactions
                                            if (invoiceVoided.Transactions.Count == 0 && invoiceVoided.InvoiceTypes.Type != (int)InvoiceTypesEnum.Order)
                                            {
                                                Transactions moufaTrans = new Transactions();
                                                Transactions voidMoufaTrans = new Transactions();
                                                moufaTrans.AccountId = db.Accounts.FirstOrDefault().Id;
                                                moufaTrans.Amount = invoiceVoided.Total;
                                                moufaTrans.Day = DateTime.Now;
                                                moufaTrans.Description = "Εξόφληση για ακύρωση";
                                                moufaTrans.Guid = Guid.NewGuid();
                                                moufaTrans.InOut = 0;
                                                moufaTrans.InvoicesId = invexofl;
                                                moufaTrans.PosInfoId = invoiceVoided.PosInfoId;
                                                moufaTrans.StaffId = invoiceVoided.StaffId;
                                                moufaTrans.TransactionType = (int)TransactionTypesEnum.Sale;
                                                voidMoufaTrans.AccountId = db.Accounts.FirstOrDefault().Id;
                                                voidMoufaTrans.Amount = invoiceVoided.Total * (-1);
                                                voidMoufaTrans.Day = DateTime.Now;
                                                voidMoufaTrans.Description = "Ακύρωση";
                                                voidMoufaTrans.Guid = Guid.NewGuid();
                                                voidMoufaTrans.InOut = 1;
                                                voidMoufaTrans.PosInfoId = invoiceVoided.PosInfoId;
                                                voidMoufaTrans.StaffId = invoiceVoided.StaffId;
                                                voidMoufaTrans.TransactionType = (int)TransactionTypesEnum.Cancel;
                                                db.Transactions.Add(moufaTrans);
                                                //To void Transaction mpainei sto void Transactions
                                                invoice.Transactions.Add(voidMoufaTrans);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (g.Count == order.OrderDetail.Count) // OLA TA OrderDetail THS PARAGGELIAS EINAI STO GROUPAKI
                    {
                        //Add New Order Status
                        OrderStatus os = new OrderStatus();
                        os.OrderId = order.Id;
                        if (g.Status == 3 || g.Status == 2 || g.Status == 5)
                        {
                            os.Status = g.Status;
                        }
                        else
                        {
                            if (g.PaidStatus == 2)//paid
                            {
                                os.Status = 7;//"Εξοφλημένη";
                                if (order.OrderStatus.Where(w => w.Status == 8).Count() == 0)//AN EINAI EXOFLISI KAI DEN EXEI TIMOLOGITHEI
                                {
                                    OrderStatus os8 = new OrderStatus(); //PROSTHESE STATUS TIMOLOGISHS (8)
                                    os8.OrderId = order.Id;
                                    os8.Status = 8;
                                    os8.StaffId = list.FirstOrDefault().StaffId;
                                    os8.TimeChanged = DateTime.Now;
                                    db.OrderStatus.Add(os8);
                                }
                            }
                            else if (g.PaidStatus == 1)//invoiced
                            {
                                os.Status = 8;// "Τιμολογημένη";
                            }
                            else
                            {
                                os.Status = g.Status;
                            }
                        }
                        os.TimeChanged = DateTime.Now;
                        os.StaffId = list.FirstOrDefault().StaffId;
                        if (order.OrderStatus.Where(s => s.Status == os.Status).Count() == 0)//VALE TO NEO OrserStatus MONO AN DEN YPARXEI HDH
                        {
                            db.OrderStatus.Add(os);
                        }
                    }
                }
            }

            #endregion

            if (invoice.OrderDetailInvoices.Count > 0)
            {
                db.Invoices.Add(invoice);
            }
            if (curTransactions.Count > 0)
            {
                foreach (var t in curTransactions)
                {
                    db.Transactions.Add(t);
                }
            }
            #endregion
            if (invoice.OrderDetailInvoices.Count > 0)
            {
                db.Invoices.Add(invoice);
            }
            if (curTransactions.Count > 0)
            {
                foreach (var t in curTransactions)
                {
                    db.Transactions.Add(t);
                }
            }

        #endregion
            return OrderDet;
        }

        private dynamic CreateInvoiceForOrder(IEnumerable<OrderDetailUpdateObj> list, List<OrderDetail> ordetetailMappedList, bool updaterange, OrderDetailUpdateObjList OrderDet, int type)
        {
            var od = list.Select(s => s.Id);
            List<OrderDetail> ordetlist = new List<OrderDetail>();
            List<OrderDetail> ordetPaidlist = new List<OrderDetail>();
            Invoices invoice = new Invoices();
            Invoices oldInvoice = new Invoices();

            if (OrderDet.AccountsObj != null)
            {
                //hereIam
                decimal sums = (decimal)ordetetailMappedList.Sum(sm => sm.TotalAfterDiscount);
                decimal accobjsums = OrderDet.AccountsObj.Sum(sm => sm.Amount);
                decimal percentageEpim = accobjsums == 0 ? 0 : 1 - (decimal)(sums / accobjsums);

                decimal remainingDiscount = accobjsums - sums;
                // decimal ctr = 1;
                decimal subtotal = sums;
                foreach (var epimItem in OrderDet.AccountsObj)
                {
                    epimItem.Amount -= Math.Round((decimal)(epimItem.Amount * percentageEpim), 2);
                    // decimal percentageEpim = sums == 0 ? 0 : 1 - (decimal)(epimItem.Amount / remainingDiscount);
                    // decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                    remainingDiscount = remainingDiscount - epimItem.Amount;
                }


            }
            decimal amount = 0;
            decimal? Netamount = 0;
            decimal? Vatamount = 0;
            decimal? Taxamount = 0;
            decimal? Discountamount = 0;
            decimal discount = list.FirstOrDefault().TableDiscount != null ? list.FirstOrDefault().TableDiscount.Value : 0;
            decimal percentage = 0;

            #region Where Magic Starts
            //EINAI EKSOFLISH TIMOLOGIMENWN (EXEI VGEI INVOICE PIO PRIN)
            //if (type == (int)OrderDetailUpdateType.PayOff)
            //{
            //    //VRES TO POSINFODETAIL ME TO PRWTO INVOICE POU EXEI
            //    var ordetinvoice = ordetetailMappedList.FirstOrDefault().OrderDetailInvoices.FirstOrDefault();
            //    if (ordetinvoice != null)
            //    {

            //        oldInvoice = ordetinvoice.Invoices;

            //        //SHMANTIKOS ELEGXOS
            //        //PAEI NA KANEI EXOFLISH HDH EXOFLIMENWN
            //        var groupedDetailsByInvoice = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).GroupBy(g => g.InvoicesId).Select(s => new
            //        {
            //            InvoicesId = s.Key,
            //            Invoices = s.FirstOrDefault().Invoices
            //        });
            //        foreach (var i in groupedDetailsByInvoice)
            //        {
            //            if (i.Invoices.Transactions.Count > 0 && i.Invoices.Total <= i.Invoices.Transactions.Sum(s => s.Amount))
            //            {
            //                return null;
            //            }
            //        }

            //    }
            //}

            long updateReceiptNo = -1;
            int counter = 0;
            long newCounter = -1;
            newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : newCounter;
            foreach (var o in list)
            {
                counter++;
                if (o.Guid != null)
                {
                    var ord = ordetetailMappedList.Where(w => w.Guid == o.Guid).FirstOrDefault();//db.OrderDetail.Where(w => w.Id == o.Id).FirstOrDefault();
                    if (ord != null)
                    {
                        if (o.GuestId != null)
                        {
                            ord.GuestId = o.GuestId;
                        }
                        //if (o.StaffId != ord.Order.StaffId)
                        //{
                        //    ord.Order.StaffId = o.StaffId;
                        //}
                        if (o.Status != 7 && o.Status != 8)//Mhn kaneis UPDATE to status tou DETAIL gt kanei conflict sta TODO tou KDS
                        {
                            ord.Status = o.Status;
                        }
                        ord.StatusTS = DateTime.Now;
                        if (o.PaidStatus != null)
                            ord.PaidStatus = o.PaidStatus;
                        if (type == (int)OrderDetailUpdateType.PayOff)
                        {
                            if (o.Status == 7)//Exoflisi
                                AdjustPricesWithNewPricelist(true, ordetPaidlist, ref amount, ref Netamount, ref Vatamount, ref Taxamount, ref Discountamount, percentage, o, ord);
                        }
                        if (type == (int)OrderDetailUpdateType.InvoiceOnly)
                        {

                            AdjustPricesWithNewPricelist(false, ordetPaidlist, ref amount, ref Netamount, ref Vatamount, ref Taxamount, ref Discountamount, percentage, o, ord);
                        }
                        if (o.Status == 5 && o.PaidStatus == 2)
                        {
                            ordetPaidlist.Add(ord);
                        }
                        ordetlist.Add(ord);
                        invoice.ClientPosId = ord.Order.ClientPosId;
                        invoice.PdaModuleId = ord.Order.PdaModuleId;
                        invoice.PosInfoId = o.PosId;
                        invoice.GuestId = o.GuestId;
                        invoice.StaffId = o.StaffId;
                        invoice.TableId = ord.TableId;
                        invoice.DiscountRemark = o.DiscountRemark;
                        //                        if (type == (int)OrderDetailUpdateType.UnPaidCancel || type == (int)OrderDetailUpdateType.PaidCancel)//Akyrwsh mh exoflimenwn
                        //                        {
                        //if (o.Status == 5 && o.PaidStatus != 2)
                        //{
                        decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
                        invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
                        decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
                        invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
                        decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
                        invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
                        decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
                        invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
                        var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;
                        invoice.Discount = invoice.Discount != null ? invoice.Discount + ord.Discount + ingrDis : ord.Discount + ingrDis;
                        //invoice.Discount = invoice.Discount ?? 0 + ord.Discount ?? 0 + ingrDis ?? 0;
                        if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                        {
                            invoice.IsPrinted = o.OrderDetailInvoices.FirstOrDefault().IsPrinted;
                        }


                        //}
                        //                        }
                    }
                }

                if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                {
                    invoice.Day = o.OrderDetailInvoices.FirstOrDefault().CreationTS;
                    invoice.Counter = (int?)o.OrderDetailInvoices.FirstOrDefault().Counter;
                    invoice.PosInfoDetailId = o.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId;
                    foreach (var i in o.OrderDetailInvoices)
                    {
                        invoice.OrderDetailInvoices.Add(i);
                        db.OrderDetailInvoices.Add(i);
                        updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
                    }
                }
                //else//EINAI EKSOFLISH TIMOLOGIMENWN (EXEI VGEI INVOICE PIO PRIN)
                //{
                //    if (type == (int)OrderDetailUpdateType.PayOff)
                //    {
                //        //VRES TO POSINFODETAIL ME TO PRWTO INVOICE POU EXEI
                //        var ordetinvoice = ordetetailMappedList.FirstOrDefault().OrderDetailInvoices.FirstOrDefault();
                //        if (ordetinvoice != null)
                //        {
                //            updateReceiptNo = ordetinvoice.PosInfoDetailId != null ? ordetinvoice.PosInfoDetailId.Value : -1;
                //            newCounter = ordetinvoice.Counter.Value; //Vale ReceiptNo iso me to palio Invoice Exoflisis                         
                //        }
                //    }
                //}
            }



            var pid = new PosInfoDetail();
            if (type != (int)OrderDetailUpdateType.Kds)
            {
                invoice.Cover = ordetetailMappedList.GroupBy(g => g.OrderId).Sum(s => s.FirstOrDefault().Couver);
                //db.SaveChanges();
                pid = db.PosInfoDetail.Include(i => i.PosInfo).FirstOrDefault();
                if (updateReceiptNo > -1)
                {
                    pid = db.PosInfoDetail.Include(i => i.PosInfo).Where(w => w.Id == updateReceiptNo).FirstOrDefault();
                    long onlineCounter = pid.Counter != null ? (pid.Counter.Value + 1) : 1;
                    //newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : onlineCounter;
                    newCounter = newCounter != -1 ? newCounter : onlineCounter;
                    pid.Counter = newCounter;
                    var posinfo = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == pid.PosInfoId).FirstOrDefault();
                    if (posinfo != null)
                    {
                        var piDetGroup = posinfo.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == pid.GroupId);
                        foreach (var i in piDetGroup)
                        {
                            i.Counter = newCounter;
                        }
                    }
                    invoice.Counter = (int)newCounter;
                    invoice.Description = pid.Description;
                    invoice.InvoiceTypeId = pid.InvoicesTypeId;

                }
            }



            //#region Group By Order Updates

            //var group = ordetlist.ToList().GroupBy(g => g.OrderId).Select(s => new
            //{
            //    OrderId = s.Key,
            //    OrderDetailStatus = s.FirstOrDefault().Status
            //});

            //foreach (var o in group)
            //{
            //    var order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Include("OrderDetail.OrderDetailInvoices")
            //        .Include("OrderDetail.OrderDetailInvoices.PosInfoDetail").Include("OrderStatus").Include("OrderDetail.PricelistDetail")
            //        .Include(i => i.PosInfo).SingleOrDefault(s => s.Id == o.OrderId);
            //    var gr = order.OrderDetail.Where(w => (w.IsDeleted ?? false) == false).GroupBy(g => new { g.PaidStatus, g.Status }).Select(s => new
            //    {
            //        PaidStatus = s.Key.PaidStatus,
            //        Status = s.Key.Status,
            //        Count = s.Count(),
            //        PaidOrderDetails = s.Where(w => ordetPaidlist.Contains(w)).AsEnumerable<OrderDetail>(),
            //        OrderDetails = s
            //    });
            //    foreach (var g in gr)
            //    {



            //        if (type == (int)OrderDetailUpdateType.InvoiceOnly)
            //        {
            //            var ord = ordetetailMappedList.FirstOrDefault();
            //            decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
            //            invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
            //            decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
            //            invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
            //            decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
            //            invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
            //            decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
            //            invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
            //            var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;


            //            invoice.Total = ordetetailMappedList.Sum(s => s.TotalAfterDiscount) + ingrGross;
            //            invoice.Discount = ordetetailMappedList.Sum(s => s.Discount) + ingrDis;
            //            invoice.Vat = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm => sm.VatAmount)) + ingrVat;
            //            invoice.Tax = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm => sm.TaxAmount)) + ingrTax;
            //            invoice.Net = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm => sm.Net)) + ingrNet;

            //        }
            //        if (type == (int)OrderDetailUpdateType.UnPaidCancel)
            //        {
            //            foreach (var inv in ordetetailMappedList)
            //            {
            //                if (inv.OrderDetailInvoices != null && inv.OrderDetailInvoices.Count > 0)
            //                {
            //                    var invexofl = inv.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false).OrderByDescending(ob => ob.Id).FirstOrDefault().InvoicesId;
            //                    if (invexofl != null)
            //                    {
            //                        var invoiceVoided = db.Invoices.Include(i => i.Transactions).Where(w => w.Id == invexofl).FirstOrDefault();
            //                        if (invoiceVoided != null)
            //                        {
            //                            invoiceVoided.IsVoided = true;
            //                            //An den einai eksoflimeni, eksoflise tin prwta vazwntas ta transactions
            //                            if (invoiceVoided.Transactions.Count == 0 && invoiceVoided.InvoiceTypes.Type != (int)InvoiceTypesEnum.Order)
            //                            {
            //                                Transactions moufaTrans = new Transactions();
            //                                Transactions voidMoufaTrans = new Transactions();
            //                                moufaTrans.AccountId = db.Accounts.FirstOrDefault().Id;
            //                                moufaTrans.Amount = invoiceVoided.Total;
            //                                moufaTrans.Day = DateTime.Now;
            //                                moufaTrans.Description = "Εξόφληση για ακύρωση";
            //                                moufaTrans.Guid = Guid.NewGuid();
            //                                moufaTrans.InOut = 0;
            //                                moufaTrans.InvoicesId = invexofl;
            //                                moufaTrans.PosInfoId = invoiceVoided.PosInfoId;
            //                                moufaTrans.StaffId = invoiceVoided.StaffId;
            //                                moufaTrans.TransactionType = (int)TransactionType.Sale;
            //                                voidMoufaTrans.AccountId = db.Accounts.FirstOrDefault().Id;
            //                                voidMoufaTrans.Amount = invoiceVoided.Total * (-1);
            //                                voidMoufaTrans.Day = DateTime.Now;
            //                                voidMoufaTrans.Description = "Ακύρωση";
            //                                voidMoufaTrans.Guid = Guid.NewGuid();
            //                                voidMoufaTrans.InOut = 1;
            //                                voidMoufaTrans.PosInfoId = invoiceVoided.PosInfoId;
            //                                voidMoufaTrans.StaffId = invoiceVoided.StaffId;
            //                                voidMoufaTrans.TransactionType = (int)TransactionType.Cancel;
            //                                db.Transactions.Add(moufaTrans);
            //                                //To void Transaction mpainei sto void Transactions
            //                                invoice.Transactions.Add(voidMoufaTrans);
            //                            }
            //                        }
            //                        break;
            //                    }
            //                }
            //            }
            //        }

            //        if (g.Count == order.OrderDetail.Count) // OLA TA OrderDetail THS PARAGGELIAS EINAI STO GROUPAKI
            //        {
            //            //Add New Order Status
            //            OrderStatus os = new OrderStatus();
            //            os.OrderId = order.Id;
            //            if (g.Status == 3 || g.Status == 2 || g.Status == 5)
            //            {
            //                os.Status = g.Status;
            //            }
            //            else
            //            {
            //                if (g.PaidStatus == 2)//paid
            //                {
            //                    os.Status = 7;//"Εξοφλημένη";
            //                    if (order.OrderStatus.Where(w => w.Status == 8).Count() == 0)//AN EINAI EXOFLISI KAI DEN EXEI TIMOLOGITHEI
            //                    {
            //                        OrderStatus os8 = new OrderStatus(); //PROSTHESE STATUS TIMOLOGISHS (8)
            //                        os8.OrderId = order.Id;
            //                        os8.Status = 8;
            //                        os8.StaffId = list.FirstOrDefault().StaffId;
            //                        os8.TimeChanged = DateTime.Now;
            //                        db.OrderStatus.Add(os8);
            //                    }
            //                }
            //                else if (g.PaidStatus == 1)//invoiced
            //                {
            //                    os.Status = 8;// "Τιμολογημένη";
            //                }
            //                else
            //                {
            //                    os.Status = g.Status;
            //                }
            //            }
            //            os.TimeChanged = DateTime.Now;
            //            os.StaffId = list.FirstOrDefault().StaffId;
            //            if (order.OrderStatus.Where(s => s.Status == os.Status).Count() == 0)//VALE TO NEO OrserStatus MONO AN DEN YPARXEI HDH
            //            {
            //                db.OrderStatus.Add(os);
            //            }
            //        }
            //    }
            //}


            //#endregion
            //create invoice from order
            try
            {
                var tid = list.FirstOrDefault().Id;
                var isd = db.OrderDetailInvoices.Include("Invoices.InvoiceShippingDetails").Where(w => w.OrderDetailId == tid && w.Invoices.InvoiceShippingDetails.Count() > 0).Select(s => s.Invoices.InvoiceShippingDetails.FirstOrDefault()).FirstOrDefault();
                if (isd != null)
                {
                    InvoiceShippingDetails invoiceShippingDetails = new InvoiceShippingDetails();

                    invoiceShippingDetails.ShippingAddressId = isd.ShippingAddressId;
                    invoiceShippingDetails.ShippingAddress = isd.ShippingAddress;
                    invoiceShippingDetails.ShippingCity = isd.ShippingCity;
                    invoiceShippingDetails.ShippingZipCode = isd.ShippingZipCode;
                    invoiceShippingDetails.BillingAddressId = isd.BillingAddressId;
                    invoiceShippingDetails.BillingAddress = isd.BillingAddress;
                    invoiceShippingDetails.BillingCity = isd.BillingCity;
                    invoiceShippingDetails.BillingZipCode = isd.BillingZipCode;

                    invoiceShippingDetails.BillingName = isd.BillingName;
                    invoiceShippingDetails.BillingVatNo = isd.BillingVatNo;
                    invoiceShippingDetails.BillingDOY = isd.BillingDOY;
                    invoiceShippingDetails.BillingJob = isd.BillingJob;

                    invoiceShippingDetails.Floor = isd.Floor;
                    invoiceShippingDetails.CustomerRemarks = isd.CustomerRemarks;
                    invoiceShippingDetails.StoreRemarks = isd.StoreRemarks;
                    invoiceShippingDetails.Longtitude = isd.Longtitude;
                    invoiceShippingDetails.Latitude = isd.Latitude;
                    invoiceShippingDetails.CustomerName = isd.CustomerName;
                    invoiceShippingDetails.CustomerID = isd.CustomerID;
                    invoiceShippingDetails.Phone = isd.Phone;

                    invoice.InvoiceShippingDetails.Add(invoiceShippingDetails);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            if (invoice.OrderDetailInvoices.Count > 0)
            {
                db.Invoices.Add(invoice);
            }
            #endregion
            if (invoice.OrderDetailInvoices.Count > 0)
            {
                db.Invoices.Add(invoice);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return ex.ToString();// Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            return OrderDet;
        }
 

        public string PostOrderDetail(OrderDetailUpdateObjList OrderDet, bool updaterange, int type)
        {
            //string result = "";
            //using (OrderInvoicingHelper invHelp = new OrderInvoicingHelper(db))
            //{
            //    result = invHelp.InvoiceOrder(OrderDet, updaterange, type);
            //}
            //return result;

            ////return;
            ////throw new System.ArgumentException("Parameter cannot be null", "original");






            IEnumerable<OrderDetailUpdateObj> list = OrderDet.OrderDet;
            var od = list.Select(s => s.Id);
            List<OrderDetail> ordetlist = new List<OrderDetail>();
            List<OrderDetail> ordetPaidlist = new List<OrderDetail>();
            Invoices invoice = new Invoices();
            Invoices oldInvoice = new Invoices();
            if (type == (int)OrderDetailUpdateType.Kds)
            {
                foreach (var item in OrderDet.OrderDet)
                {
                    if (item.Id != 0)
                    {
                        var orderdetail = db.OrderDetail.Include("Order").Where(w => w.Order.PosId == item.PosId && w.Order.EndOfDayId == null && w.Id == item.Id && (w.IsDeleted ?? false) == false).FirstOrDefault();
                        if (orderdetail != null)
                        {
                            orderdetail.Status = item.Status;
                            orderdetail.StatusTS = item.StatusTS;
                        }
                    }
                    else if (item.Guid != null)
                    {
                        var orderdetails = db.OrderDetail.Include("Order").Where(w => w.Order.PosId == item.PosId && w.Order.EndOfDayId == null && w.Guid == item.Guid && (w.IsDeleted ?? false) == false);
                        //An to kanei o diaolos kai exoun parei polla to idio Guid
                        foreach (var orderdetail in orderdetails)
                        {
                            orderdetail.Status = item.Status;
                            orderdetail.StatusTS = item.StatusTS;
                        }
                    }
                }
            }
            else
            {
                List<OrderDetail> ordetetailMappedList = db.OrderDetail.Include(i => i.OrderDetailIgredients)
                                                                       .Include(i => i.OrderDetailVatAnal)
                    .Include(i => i.OrderDetailInvoices.Select(ii => ii.Invoices.InvoiceTypes))
                    .Include(i => i.OrderDetailInvoices.Select(ii => ii.Invoices.Transactions))
                    .Include("OrderDetailIgredients.OrderDetailIgredientVatAnal")
                    .Include(i => i.Order)
                    .Include("OrderDetailInvoices.PosInfoDetail")
                    .Include(i => i.Order.OrderStatus)
                    .Include(i => i.PricelistDetail)
                    .Include(ii => ii.Order.PosInfo)
                    .Include(i => i.TablePaySuggestion)
                    .Where(w => od.Contains(w.Id) && (w.IsDeleted ?? false) == false).ToList();
                //decimal amount = 0;
                //decimal? Netamount = 0;
                //decimal? Vatamount = 0;
                //decimal? Taxamount = 0;
                //decimal? Discountamount = 0;
                decimal discount = list.FirstOrDefault().TableDiscount != null ? list.FirstOrDefault().TableDiscount.Value : 0;
                decimal percentage = 0;
                try
                {
                    if (discount > 0)
                    {
                        var ingSum = (decimal)ordetetailMappedList.SelectMany(sm => sm.OrderDetailIgredients).Sum(sm1 => sm1.TotalAfterDiscount);
                        var odSum = (decimal)ordetetailMappedList.Sum(sm => sm.TotalAfterDiscount);
                        percentage = (discount / (odSum + ingSum));

                        foreach (var ods in OrderDet.OrderDet)
                        {
                            var t = ordetetailMappedList.Where(w => w.Id == ods.Id).FirstOrDefault();
                            if (t != null)
                            {
                                t.Discount = ods.Discount;
                                t.TotalAfterDiscount = ods.TotalAfterDiscount;
                                var vatAn = t.OrderDetailVatAnal.FirstOrDefault();
                                if (vatAn != null)
                                {
                                    vatAn.Gross = t.TotalAfterDiscount;
                                    vatAn.VatAmount = vatAn.VatAmount * percentage;
                                    vatAn.Net = vatAn.Gross - vatAn.VatAmount;
                                }
                            }

                        }

                        var itemsDiscount = (decimal)ordetetailMappedList.Sum(sm => sm.TotalAfterDiscount) * percentage;
                        if (OrderDet.OrderDet.FirstOrDefault().TableDiscount > itemsDiscount)
                        {

                            foreach (var ing in ordetetailMappedList.SelectMany(sm => sm.OrderDetailIgredients))
                            {
                                ing.Discount = ing.TotalAfterDiscount * percentage;
                                ing.TotalAfterDiscount = ing.TotalAfterDiscount - ing.Discount;
                                var vatAn = ing.OrderDetailIgredientVatAnal.FirstOrDefault();
                                if (vatAn != null)
                                {
                                    vatAn.Gross = ing.TotalAfterDiscount;
                                    vatAn.VatAmount = vatAn.VatAmount * percentage;
                                    vatAn.Net = vatAn.Gross - vatAn.VatAmount;
                                }
                            }

                        }
                        //percentage = (discount / ordetetailMappedList.Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + (decimal)(sm.OrderDetailIgredients != null && sm.OrderDetailIgredients.Count > 0 ? sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0))));
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
                var qr = ordetetailMappedList.SelectMany(sm => sm.OrderDetailInvoices).GroupBy(g => g.Invoices.InvoiceTypes.Type).Select(s => new
                                                                                                                                {
                                                                                                                                    Key = s.Key,
                                                                                                                                    validIds = s.Key == (int)InvoiceTypesEnum.Order ? s.Where(w => w.OrderDetail.PaidStatus == 0).Select(ss => ss.OrderDetailId.Value) : s.Select(ss => ss.OrderDetailId.Value)
                                                                                                                                });
                Int64 deltioId = db.InvoiceTypes.Where(w => w.Type == (int)InvoiceTypesEnum.Order).FirstOrDefault() != null ? db.InvoiceTypes.Where(w => w.Type == (int)InvoiceTypesEnum.Order).FirstOrDefault().Id : (Int64)InvoiceTypesEnum.Order;
                var deltiaGiaTimologisi = qr.Where(w => w.Key == (int)InvoiceTypesEnum.Order).FirstOrDefault();
                //Αν υπάρχουν μη τιμολογημένα δελτία προχωράει πρώτα με την τιμολόγιση.
                if (deltiaGiaTimologisi != null && deltiaGiaTimologisi.validIds.Count() > 0)
                {
                    var dp = CreateInvoiceForOrder(list.Where(w => deltiaGiaTimologisi.validIds.Contains(w.Id)).ToList(), ordetetailMappedList.Where(w => deltiaGiaTimologisi.validIds.Contains(w.Id)).ToList(), updaterange, OrderDet, type);
                }
                //foreach (var item in qr)
                //{
                //    //                    var valodids = ordetetailMappedList.SelectMany(sm => sm.OrderDetailInvoices).Where(w=>w.Invoices.InvoiceTypeId == item).Select(s=>s.OrderDetailId);
                //    if (item.validIds.Count() > 0)
                //    {
                //        var ret = PostOrderByInvoiceType(list.Where(w => item.validIds.Contains(w.Id)).ToList(), ordetetailMappedList.Where(w => item.validIds.Contains(w.Id)).ToList(), updaterange, OrderDet, type);
                //    }
                //}

                if (type == (int)OrderDetailUpdateType.PayOff || type == (int)OrderDetailUpdateType.PaidCancel || type == (int)OrderDetailUpdateType.UnPaidCancel)
                {
                    //qr = ordetetailMappedList.SelectMany(sm => sm.OrderDetailInvoices).GroupBy(g => g.Invoices.InvoiceTypes.Type).Select(s => new
                    var qr1 = ordetetailMappedList.SelectMany(sm => sm.OrderDetailInvoices).GroupBy(g => g.Invoices.InvoiceTypeId).Select(s => new
                    {
                        Key = s.Key,
                        validIds = s.Select(ss => ss.OrderDetailId.Value)
                    }).Where(w => w.Key != deltioId);
                    foreach (var item in qr1)
                    {
                        if (item.validIds.Count() > 0)
                        {
                            var ret = PostOrderByInvoiceType(list.Where(w => item.validIds.Contains(w.Id)).ToList(), ordetetailMappedList.Where(w => item.validIds.Contains(w.Id)).ToList(), updaterange, OrderDet, type);
                        }
                    }
                }
                //  var ret = PostOrderByInvoiceType(list, ordetetailMappedList, updaterange, OrderDet, type);
                //}
                //thanos
                if (type == (int)OrderDetailUpdateType.PaidCancel)
                {

                }
                else
                    if (type == (int)OrderDetailUpdateType.UnPaidCancel)
                    {
                    }



                #region Where Magic Starts
                ////EINAI EKSOFLISH TIMOLOGIMENWN (EXEI VGEI INVOICE PIO PRIN)
                //if (type == (int)OrderDetailUpdateType.PayOff)
                //{
                //    //VRES TO POSINFODETAIL ME TO PRWTO INVOICE POU EXEI
                //    var ordetinvoice = ordetetailMappedList.FirstOrDefault().OrderDetailInvoices.FirstOrDefault();
                //    if (ordetinvoice != null)
                //    {

                //        oldInvoice = ordetinvoice.Invoices;

                //        //SHMANTIKOS ELEGXOS
                //        //PAEI NA KANEI EXOFLISH HDH EXOFLIMENWN
                //        var groupedDetailsByInvoice = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).GroupBy(g => g.InvoicesId).Select(s => new
                //        {
                //            InvoicesId = s.Key,
                //            Invoices = s.FirstOrDefault().Invoices
                //        });
                //        foreach (var i in groupedDetailsByInvoice)
                //        {
                //            if (i.Invoices.Transactions.Count > 0 && i.Invoices.Total <= i.Invoices.Transactions.Sum(s => s.Amount))
                //            {
                //                return null;
                //            }
                //        }

                //    }
                //}

                //long updateReceiptNo = -1;
                //int counter = 0;
                //long newCounter = -1;
                //newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : newCounter;
                //foreach (var o in list)
                //{
                //    counter++;
                //    if (o.Guid != null)
                //    {
                //        var ord = ordetetailMappedList.Where(w => w.Guid == o.Guid).FirstOrDefault();//db.OrderDetail.Where(w => w.Id == o.Id).FirstOrDefault();
                //        if (ord != null)
                //        {
                //            if (o.GuestId != null)
                //            {
                //                ord.GuestId = o.GuestId;
                //            }
                //            //if (o.StaffId != ord.Order.StaffId)
                //            //{
                //            //    ord.Order.StaffId = o.StaffId;
                //            //}
                //            if (o.Status != 7 && o.Status != 8)//Mhn kaneis UPDATE to status tou DETAIL gt kanei conflict sta TODO tou KDS
                //            {
                //                ord.Status = o.Status;
                //            }
                //            ord.StatusTS = DateTime.Now;
                //            if (o.PaidStatus != null)
                //                ord.PaidStatus = o.PaidStatus;
                //            if (type == (int)OrderDetailUpdateType.PayOff)
                //            {
                //                if (o.Status == 7)//Exoflisi
                //                    AdjustPricesWithNewPricelist(ordetPaidlist, ref amount, ref Netamount, ref Vatamount, ref Taxamount, ref Discountamount, percentage, o, ord);
                //            }
                //            if (o.Status == 5 && o.PaidStatus == 2)
                //            {
                //                ordetPaidlist.Add(ord);
                //            }
                //            ordetlist.Add(ord);
                //            invoice.ClientPosId = ord.Order.ClientPosId;
                //            invoice.PdaModuleId = ord.Order.PdaModuleId;
                //            invoice.PosInfoId = o.PosId;
                //            invoice.GuestId = o.GuestId;
                //            invoice.StaffId = o.StaffId;
                //            invoice.TableId = ord.TableId;
                //            if (type == (int)OrderDetailUpdateType.UnPaidCancel || type == (int)OrderDetailUpdateType.PaidCancel)//Akyrwsh mh exoflimenwn
                //            {
                //                //if (o.Status == 5 && o.PaidStatus != 2)
                //                //{
                //                decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
                //                invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
                //                decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
                //                invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
                //                decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
                //                invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
                //                decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
                //                invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
                //                var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;
                //                invoice.Discount = invoice.Discount != null ? invoice.Discount + ord.Discount + ingrDis : ord.Discount + ingrDis;
                //                if (o.OrderDetailInvoices.Count > 0)
                //                {
                //                    invoice.IsPrinted = o.OrderDetailInvoices.FirstOrDefault().IsPrinted;
                //                }
                //                //}
                //            }
                //        }
                //    }
                //    if (type != (int)OrderDetailUpdateType.Kds)
                //    {
                //        if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                //        {
                //            invoice.Day = o.OrderDetailInvoices.FirstOrDefault().CreationTS;
                //            invoice.Counter = (int?)o.OrderDetailInvoices.FirstOrDefault().Counter;
                //            invoice.PosInfoDetailId = o.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId;
                //            foreach (var i in o.OrderDetailInvoices)
                //            {
                //                invoice.OrderDetailInvoices.Add(i);
                //                db.OrderDetailInvoices.Add(i);
                //                updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
                //            }
                //        }
                //        else//EINAI EKSOFLISH TIMOLOGIMENWN (EXEI VGEI INVOICE PIO PRIN)
                //        {
                //            if (type == (int)OrderDetailUpdateType.PayOff)
                //            {
                //                //VRES TO POSINFODETAIL ME TO PRWTO INVOICE POU EXEI
                //                var ordetinvoice = ordetetailMappedList.FirstOrDefault().OrderDetailInvoices.FirstOrDefault();
                //                if (ordetinvoice != null)
                //                {
                //                    updateReceiptNo = ordetinvoice.PosInfoDetailId != null ? ordetinvoice.PosInfoDetailId.Value : -1;
                //                    newCounter = ordetinvoice.Counter.Value; //Vale ReceiptNo iso me to palio Invoice Exoflisis
                //                    //oldInvoice = ordetinvoice.Invoices;

                //                    ////SHMANTIKOS ELEGXOS
                //                    ////PAEI NA KANEI EXOFLISH HDH EXOFLIMENWN
                //                    //var groupedDetailsByInvoice = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).GroupBy(g => g.InvoicesId).Select(s => new
                //                    //{
                //                    //    InvoicesId = s.Key,
                //                    //    Invoices = s.FirstOrDefault().Invoices
                //                    //});
                //                    //foreach (var i in groupedDetailsByInvoice)
                //                    //{
                //                    //    if (i.Invoices.Transactions.Count > 0 && i.Invoices.Total >= i.Invoices.Transactions.Sum(s => s.Amount))
                //                    //    {
                //                    //        return null;
                //                    //    }
                //                    //}
                //                }
                //            }
                //        }
                //    }
                //}
                ////move after creating invoice
                ////               List<Transactions> curTransactions = new List<Transactions>();
                //List<Transactions> curTransactions = CreateTransactionsWithTransfer(OrderDet, type, list, invoice, oldInvoice, ordetetailMappedList, amount, Netamount, Vatamount, Taxamount, Discountamount, updateReceiptNo, newCounter);
                ////               curTransactions = CreateTransactionsWithTransfer(OrderDet, type, list, invoice, oldInvoice, ordetetailMappedList, amount, Netamount, Vatamount, Taxamount, Discountamount, updateReceiptNo, newCounter);

                ////bool isFromKds = ordetlist.All(a => a.Status == 2 || a.Status == 3);


                //var pid = new PosInfoDetail();
                //if (type != (int)OrderDetailUpdateType.Kds)
                //{
                //    invoice.Cover = ordetetailMappedList.GroupBy(g => g.OrderId).Sum(s => s.FirstOrDefault().Couver);
                //    //db.SaveChanges();
                //    pid = db.PosInfoDetail.Include(i => i.PosInfo).FirstOrDefault();
                //    if (updateReceiptNo > -1)
                //    {
                //        pid = db.PosInfoDetail.Include(i => i.PosInfo).Where(w => w.Id == updateReceiptNo).FirstOrDefault();
                //        long onlineCounter = pid.Counter != null ? (pid.Counter.Value + 1) : 1;
                //        //newCounter = list.FirstOrDefault().invoiceCounter != null ? list.FirstOrDefault().invoiceCounter.Value : onlineCounter;
                //        newCounter = newCounter != -1 ? newCounter : onlineCounter;
                //        pid.Counter = newCounter;
                //        var posinfo = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == pid.PosInfoId).FirstOrDefault();
                //        if (posinfo != null)
                //        {
                //            var piDetGroup = posinfo.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == pid.GroupId);
                //            foreach (var i in piDetGroup)
                //            {
                //                i.Counter = newCounter;
                //            }
                //        }
                //        invoice.Counter = (int)newCounter;
                //        invoice.Description = pid.Description;
                //        invoice.InvoiceTypeId = pid.InvoicesTypeId;

                //    }
                //}



                //#region Group By Order Updates

                //var group = ordetlist.ToList().GroupBy(g => g.OrderId).Select(s => new
                //{
                //    OrderId = s.Key,
                //    OrderDetailStatus = s.FirstOrDefault().Status
                //});

                //foreach (var o in group)
                //{
                //    var order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Include("OrderDetail.OrderDetailInvoices")
                //        .Include("OrderDetail.OrderDetailInvoices.PosInfoDetail").Include("OrderStatus").Include("OrderDetail.PricelistDetail")
                //        .Include(i => i.PosInfo).SingleOrDefault(s => s.Id == o.OrderId);
                //    var gr = order.OrderDetail.Where(w => (w.IsDeleted ?? false) == false).GroupBy(g => new { g.PaidStatus, g.Status }).Select(s => new
                //    {
                //        PaidStatus = s.Key.PaidStatus,
                //        Status = s.Key.Status,
                //        Count = s.Count(),
                //        PaidOrderDetails = s.Where(w => ordetPaidlist.Contains(w)).AsEnumerable<OrderDetail>(),
                //        OrderDetails = s
                //    });
                //    foreach (var g in gr)
                //    {
                //        if (type != (int)OrderDetailUpdateType.Kds)//DEN EXEI ERTHEI APO KDS TO STATUS
                //        {
                //            if (type == (int)OrderDetailUpdateType.PayOff)
                //            {
                //                #region Update PayOFF
                //                /*
                //                if (g.Status != 5 && g.PaidStatus == 2 && order.OrderStatus.Where(w => w.Status == 7).Count() == 0)//Paid && AN DEN YPARXEI HDH STATUS EXOFLISIS
                //                {

                //                    foreach (var paidDet in g.PaidOrderDetails)
                //                    {
                //                        var orderdetailinvoices = paidDet.OrderDetailInvoices.Where(w => w.PosInfoDetail.CreateTransaction == false && w.PosInfoDetail.IsInvoice == true);
                //                        if (orderdetailinvoices.Count() > 0)
                //                        {
                //                            foreach (var orinv in orderdetailinvoices)
                //                            {
                //                                orinv.StaffId = list.FirstOrDefault().StaffId;
                //                                //if (orinv.InvoicesId != null)
                //                                //{
                //                                //    foreach (var tr in curTransactions)
                //                                //    {
                //                                //        tr.InvoicesId = orinv.InvoicesId;
                //                                //    }
                //                                //}
                //                            }
                //                        }
                //                        // paidDet.TransactionId = tr.Id;
                //                    }
                //                    long accountId = list.FirstOrDefault().AccountId.Value;
                //                    var hotel = db.HotelInfo.FirstOrDefault();
                //                    List<AccountsObj> accountsList = new List<AccountsObj>();
                //                    if (OrderDet.AccountsObj != null)
                //                    {
                //                        foreach (var a in OrderDet.AccountsObj)
                //                        {
                //                            var acc = db.Accounts.Find(a.AccountId);
                //                            if (acc != null)
                //                            {
                //                                AccountsObj accobj = new AccountsObj();
                //                                accobj.Account = acc;
                //                                accobj.AccountId = a.AccountId;
                //                                accobj.Amount = a.Amount;
                //                                accobj.GuestId = a.GuestId;
                //                                accountsList.Add(accobj);
                //                            }
                //                        }
                //                    }
                //                    else
                //                    {
                //                        AccountsObj accobj = new AccountsObj();
                //                        accobj.Account = db.Accounts.Find(accountId);
                //                        accobj.AccountId = accountId;
                //                        //accobj.Amount = g.PaidOrderDetails.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                //                        accobj.Amount = ordetetailMappedList.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                //                        accobj.GuestId = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;
                //                        accountsList.Add(accobj);
                //                    }
                //                    decimal TotalAmounts = accountsList.Sum(s => s.Amount);
                //                    foreach (var ac in accountsList)
                //                    {
                //                        var account = ac.Account;
                //                        if (account != null && hotel != null)//&& account.SendsTransfer == true (tha chekaristei otan einai na stalei)
                //                        {

                //                            //var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId);
                //                            //var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId);
                //                            //var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                //                            var deps = ordetetailMappedList.Select(s => s.Order.PosInfo.DepartmentId);
                //                            var pril = ordetetailMappedList.Select(s => s.PricelistDetail.PricelistId);
                //                            var prods = ordetetailMappedList.Select(s => s.ProductId);
                //                            var query = (from f in ordetetailMappedList//g.PaidOrderDetails
                //                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                //                                         join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                //                                         on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                //                                         from ls in loj.DefaultIfEmpty()
                //                                         select new
                //                                         {
                //                                             Id = f.Id,
                //                                             SalesTypeId = st.Id,
                //                                             Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                //                                             OrderDetail = f,
                //                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                //                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                //                                             // CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                //                                             ReceiptNo = newCounter,
                //                                         }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                //                                         {
                //                                             PmsDepartmentId = s.Key,
                //                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                //                                             Total = s.Sum(sm => sm.Total),
                //                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                //                                             //CustomerId = s.FirstOrDefault().CustomerId,
                //                                             ReceiptNo = s.FirstOrDefault().ReceiptNo
                //                                         });
                //                            // long guestid = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;//tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1;
                //                            long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;
                //                            Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                //                            string storeid = HttpContext.Current.Request.Params["storeid"];
                //                            List<TransferObject> objTosendList = new List<TransferObject>();
                //                            List<TransferToPms> transferList = new List<TransferToPms>();

                //                            //var IsCreditCard = false;
                //                            //var roomOfCC = "";
                //                            //if (account.Type == 4)
                //                            //{
                //                            //    IsCreditCard = true;
                //                            //    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                //                            //        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                //                            //}
                //                            var IsNotSendingTransfer = account.SendsTransfer == false;
                //                            var IsCreditCard = false;
                //                            var roomOfCC = "";
                //                            if (account.Type == 4 || IsNotSendingTransfer == true)
                //                            {
                //                                IsCreditCard = true;
                //                                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                //                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                //                            }


                //                            //EpimerismosPerDepartment
                //                            decimal totalDiscount = TotalAmounts - ac.Amount;
                //                            decimal percentageEpim = 1 - (decimal)(ac.Amount / TotalAmounts);
                //                            decimal totalEpim = 0;
                //                            decimal remainingDiscount = totalDiscount;
                //                            decimal ctr = 1;
                //                            List<dynamic> query2 = new List<dynamic>();
                //                            query = query.OrderBy(or => or.Total);
                //                            foreach (var f in query)
                //                            {
                //                                if (ctr < query.Count())
                //                                {
                //                                    decimal subtotal = f.Total;
                //                                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                //                                    totalEpim += subtotal - percSub;
                //                                    query2.Add(new
                //                                    {
                //                                        PmsDepartmentId = f.PmsDepartmentId,
                //                                        Total = subtotal - percSub
                //                                    });
                //                                    remainingDiscount = remainingDiscount - percSub;
                //                                }
                //                                else
                //                                {
                //                                    decimal subtotal = f.Total;
                //                                    query2.Add(new
                //                                    {
                //                                        PmsDepartmentId = f.PmsDepartmentId,
                //                                        Total = subtotal - remainingDiscount
                //                                    });
                //                                    totalEpim += subtotal - remainingDiscount;
                //                                }
                //                                ctr++;
                //                            }
                //                            //
                //                            var query3 = from q in query
                //                                         join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                //                                         select new
                //                                         {
                //                                             PmsDepartmentId = q.PmsDepartmentId,
                //                                             PmsDepDescription = q.PmsDepDescription,
                //                                             Total = j.Total,
                //                                             OrderDetails = q.OrderDetails,
                //                                             //CustomerId = q.CustomerId,
                //                                             ReceiptNo = q.ReceiptNo
                //                                         };
                //                            //
                //                            foreach (var acg in query3)
                //                            {
                //                                decimal total = acg.Total;//new Economic().EpimerisiAccountTotal(acg.OrderDetails, acg.Total);
                //                                TransferToPms tpms = new TransferToPms(); //newCounter
                //                                tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + order.PosInfo.Description;
                //                                tpms.PmsDepartmentId = acg.PmsDepartmentId;
                //                                tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                //                                tpms.ProfileId = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ProfileNo.ToString() : "" : null;//acg.CustomerId;
                //                                tpms.ProfileName = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                //                                tpms.ReceiptNo = newCounter.ToString();
                //                                tpms.RegNo = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                //                                tpms.RoomDescription = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                //                                tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                //                                tpms.SendToPMS = !IsNotSendingTransfer;
                //                                //Set Status Flag (0: Cash, 1: Not Cash)
                //                                tpms.Status = IsNotSendingTransfer ? (short)0 : (short)1;
                //                                tpms.PosInfoId = order.PosInfo.Id;
                //                                tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                //                                //tpms.TransactionId = tr.Id;
                //                                tpms.TransferType = 0;//Xrewstiko
                //                                tpms.SendToPmsTS = DateTime.Now;
                //                                tpms.Total = total;// (decimal)acg.Total;
                //                                var identifier = Guid.NewGuid();
                //                                tpms.TransferIdentifier = identifier;
                //                                transferList.Add(tpms);
                //                                db.TransferToPms.Add(tpms);

                //                                //Link transferToPms with correct Transaction
                //                                var trans = curTransactions.Where(w => w.AccountId == ac.AccountId).FirstOrDefault();
                //                                if (trans != null)
                //                                {
                //                                    trans.TransferToPms.Add(tpms);
                //                                }


                //                                TransferObject to = new TransferObject();
                //                                //
                //                                to.TransferIdentifier = tpms.TransferIdentifier;
                //                                //
                //                                to.HotelId = (int)hotel.Id;
                //                                to.amount = (decimal)tpms.Total;
                //                                int PmsDepartmentId = 0;
                //                                var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                //                                to.departmentId = PmsDepartmentId;
                //                                to.description = tpms.Description;
                //                                to.profileName = tpms.ProfileName;
                //                                int resid = 0;
                //                                var toint = int.TryParse(tpms.RegNo, out resid);
                //                                to.resId = resid;
                //                                to.TransferIdentifier = identifier;
                //                                to.HotelUri = hotel.HotelUri;
                //                                to.RoomName = tpms.RoomDescription;
                //                                if (IsCreditCard)
                //                                {
                //                                    to.RoomName = roomOfCC;
                //                                }

                //                                if (to.amount != 0 && ac.Account.SendsTransfer == true)
                //                                    objTosendList.Add(to);
                //                            }


                //                            //tr.TransferToPms = new List<TransferToPms>();
                //                            //tr.TransferToPms = transferList as ICollection<TransferToPms>;

                //                            db.SaveChanges();
                //                            foreach (var to in objTosendList)
                //                            {
                //                                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                //                                sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                //                            }
                //                        }
                //                    }
                //                    //db.Transactions.Add(tr);
                //                }
                //                  */
                //                #endregion
                //            }
                //            if (type == (int)OrderDetailUpdateType.PaidCancel)
                //            {
                //                #region PaidCancel
                //                if (g.Status == 5 && g.PaidStatus == 2)//Einai Akyrwseis kai einai exoflimena
                //                {
                //                    //Transactions tr = new Transactions();
                //                    //tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
                //                    //tr.PosInfoId = order.PosInfo.Id;
                //                    ////tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price) * -1;
                //                    //tr.Day = DateTime.Now;
                //                    //tr.DepartmentId = order.PosInfo.DepartmentId;
                //                    //tr.Description = "Ακύρωση";
                //                    //if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
                //                    //{
                //                    //    tr.ExtDescription = "Μερική Ακύρωση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
                //                    //}
                //                    //tr.InOut = 1;//Εκροη
                //                    //tr.OrderId = order.Id;
                //                    //tr.StaffId = list.FirstOrDefault().StaffId;
                //                    //tr.TransactionType = (short)TransactionType.Cancel;
                //                    //Transactions t = new Economic().SetEconomicNumbersOrderDetails(tr, g.PaidOrderDetails, db);
                //                    ////tr.TransactionType = (short)TransactionType.Cancel;
                //                    //tr.Gross = t.Gross;
                //                    //tr.Amount = t.Gross;
                //                    //tr.Net = t.Net;
                //                    //tr.Tax = t.Tax;
                //                    //tr.Vat = t.Vat;
                //                    //tr.OrderDetail = new List<OrderDetail>();
                //                    //tr.OrderDetail = g.PaidOrderDetails.ToList() as ICollection<OrderDetail>;

                //                    foreach (var inv in g.PaidOrderDetails)
                //                    {
                //                        if (inv.OrderDetailInvoices != null && inv.OrderDetailInvoices.Count > 0)
                //                        {
                //                            var invexofl = inv.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false).OrderByDescending(ob => ob.Id).FirstOrDefault().InvoicesId;
                //                            if (invexofl != null)
                //                            {
                //                                var invoiceVoided = db.Invoices.Find(invexofl);
                //                                if (invoiceVoided != null)
                //                                {
                //                                    invoiceVoided.IsVoided = true;
                //                                }
                //                                break;
                //                            }
                //                        }
                //                    }
                //                    //invoice.Total = invoice.Total != null ? invoice.Total + (tr.Gross * -1) : (tr.Gross * -1);
                //                    //invoice.Net = invoice.Net != null ? invoice.Net + (tr.Net * -1) : (tr.Net * -1);
                //                    //invoice.Vat = invoice.Vat != null ? invoice.Vat + (tr.Vat * -1) : (tr.Vat * -1);
                //                    //invoice.Tax = invoice.Tax != null ? invoice.Tax + (tr.Tax * -1) : (tr.Tax * -1);
                //                    //var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                //                    //invoice.Discount = invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;
                //                    //invoice.Transactions.Add(tr);
                //                    //db.Transactions.Add(tr);
                //                    //db.SaveChanges();

                //                    var aaa = curTransactions;
                //                    var hotel = db.HotelInfo.FirstOrDefault();
                //                    List<AccountsObj> accountsList = new List<AccountsObj>();
                //                    long accountId = list.FirstOrDefault().AccountId.Value;
                //                    if (OrderDet.AccountsObj != null)
                //                    {
                //                        foreach (var a in OrderDet.AccountsObj)
                //                        {
                //                            var acc = db.Accounts.Find(a.AccountId);
                //                            if (acc != null)
                //                            {
                //                                AccountsObj accobj = new AccountsObj();
                //                                accobj.Account = acc;
                //                                accobj.AccountId = a.AccountId;
                //                                accobj.Amount = a.Amount;
                //                                accobj.GuestId = a.GuestId;
                //                                accountsList.Add(accobj);
                //                            }
                //                        }
                //                    }
                //                    else
                //                    {
                //                        AccountsObj accobj = new AccountsObj();
                //                        accobj.Account = db.Accounts.Find(accountId);
                //                        accobj.AccountId = accountId;
                //                        accobj.Amount = g.PaidOrderDetails.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                //                        accobj.GuestId = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;
                //                        accountsList.Add(accobj);
                //                    }
                //                    decimal TotalAmounts = accountsList.Sum(s => s.Amount);
                //                    // decimal TotalAmounts = g.PaidOrderDetails.Sum(sm=>sm.TotalAfterDiscount.Value);
                //                    foreach (var ac in accountsList)
                //                    {
                //                        var account = ac.Account;
                //                        if (account != null && account.SendsTransfer == true && hotel != null)
                //                        {
                //                            var deps = g.PaidOrderDetails.Select(s => s.Order.PosInfo.DepartmentId).Distinct();
                //                            var pril = g.PaidOrderDetails.Select(s => s.PricelistDetail.PricelistId).Distinct();
                //                            var prods = g.PaidOrderDetails.Select(s => s.ProductId);
                //                            var query = (from f in g.PaidOrderDetails
                //                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                //                                         join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                //                                         on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                //                                         from ls in loj.DefaultIfEmpty()
                //                                         select new
                //                                         {
                //                                             Id = f.Id,
                //                                             SalesTypeId = st.Id,
                //                                             Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                //                                             OrderDetail = f,
                //                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                //                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                //                                             //CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                //                                             ReceiptNo = f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault() != null ? f.OrderDetailInvoices.Where(s => s.PosInfoDetail.CreateTransaction == true).FirstOrDefault().Counter : newCounter,
                //                                         }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                //                                         {
                //                                             PmsDepartmentId = s.Key,
                //                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                //                                             Total = s.Sum(sm => sm.Total),
                //                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                //                                             // CustomerId = s.FirstOrDefault().CustomerId,
                //                                             ReceiptNo = s.FirstOrDefault().ReceiptNo
                //                                         });
                //                            //Customers curcustomer = list.FirstOrDefault().Customer;

                //                            //long guestid = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;// tr.OrderDetail.FirstOrDefault() != null ? tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1 : -1;
                //                            long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;
                //                            Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                //                            string storeid = HttpContext.Current.Request.Params["storeid"];
                //                            List<TransferObject> objTosendList = new List<TransferObject>();
                //                            List<TransferToPms> transferList = new List<TransferToPms>();

                //                            var IsCreditCard = false;
                //                            var roomOfCC = "";
                //                            if (account.Type == 4)
                //                            {
                //                                IsCreditCard = true;
                //                                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                //                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                //                            }
                //                            //EpimerismosPerDepartment
                //                            decimal totalDiscount = TotalAmounts - ac.Amount;
                //                            decimal percentageEpim = TotalAmounts == 0 ? 0 : 1 - (decimal)(ac.Amount / TotalAmounts);
                //                            decimal totalEpim = 0;
                //                            decimal remainingDiscount = totalDiscount;
                //                            decimal ctr = 1;
                //                            List<dynamic> query2 = new List<dynamic>();
                //                            query = query.OrderBy(or => or.Total);
                //                            foreach (var f in query)
                //                            {
                //                                if (ctr < query.Count())
                //                                {
                //                                    decimal subtotal = f.Total;
                //                                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                //                                    totalEpim += subtotal - percSub;
                //                                    query2.Add(new
                //                                    {
                //                                        PmsDepartmentId = f.PmsDepartmentId,
                //                                        Total = subtotal - percSub
                //                                    });
                //                                    remainingDiscount = remainingDiscount - percSub;
                //                                }
                //                                else
                //                                {
                //                                    decimal subtotal = f.Total;
                //                                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                //                                    totalEpim += subtotal - percSub;
                //                                    query2.Add(new
                //                                    {
                //                                        PmsDepartmentId = f.PmsDepartmentId,
                //                                        Total = subtotal - percSub
                //                                    });
                //                                    remainingDiscount = remainingDiscount - percSub;


                //                                    //decimal subtotal = f.Total;
                //                                    //query2.Add(new
                //                                    //{
                //                                    //    PmsDepartmentId = f.PmsDepartmentId,
                //                                    //    Total = subtotal - remainingDiscount
                //                                    //});
                //                                    //totalEpim += subtotal - remainingDiscount;
                //                                }
                //                                ctr++;
                //                            }
                //                            //
                //                            var query3 = from q in query
                //                                         join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                //                                         select new
                //                                         {
                //                                             PmsDepartmentId = q.PmsDepartmentId,
                //                                             PmsDepDescription = q.PmsDepDescription,
                //                                             Total = j.Total,
                //                                             OrderDetails = q.OrderDetails,
                //                                             //CustomerId = q.CustomerId,
                //                                             ReceiptNo = q.ReceiptNo
                //                                         };
                //                            //
                //                            var departmentDescritpion = db.Department.FirstOrDefault(f => f.Id == order.PosInfo.DepartmentId);
                //                            string depstr = departmentDescritpion != null ? departmentDescritpion.Description : order.PosInfo.Description;

                //                            foreach (var acg in query3)
                //                            {
                //                                TransferToPms tpms = new TransferToPms(); // newCounter
                //                                tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + order.PosInfo.Id + ", " + depstr;//order.PosInfo.Description;
                //                                tpms.PmsDepartmentId = acg.PmsDepartmentId;
                //                                tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                //                                tpms.ProfileId = !IsCreditCard ? curcustomer != null ? curcustomer.ProfileNo.ToString() : "" : null;//acg.CustomerId;
                //                                tpms.ProfileName = !IsCreditCard ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                //                                tpms.ReceiptNo = newCounter.ToString();
                //                                tpms.RegNo = !IsCreditCard ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                //                                tpms.RoomDescription = !IsCreditCard ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                //                                tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                //                                tpms.SendToPMS = true;
                //                                tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                //                                tpms.PosInfoId = order.PosId;
                //                                //tpms.TransactionId = tr.Id;
                //                                tpms.TransferType = 0;//Xrewstiko
                //                                tpms.Total = (decimal)acg.Total * -1;
                //                                tpms.SendToPmsTS = DateTime.Now;
                //                                var identifier = Guid.NewGuid();
                //                                tpms.TransferIdentifier = identifier;
                //                                transferList.Add(tpms);
                //                                db.TransferToPms.Add(tpms);

                //                                //Link transferToPms with correct Transaction
                //                                var trans = curTransactions.Where(w => w.AccountId == ac.AccountId).FirstOrDefault();
                //                                if (trans != null)
                //                                {
                //                                    trans.TransferToPms.Add(tpms);
                //                                }

                //                                TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, identifier);

                //                                if (to.amount != 0)
                //                                    objTosendList.Add(to);
                //                            }
                //                            //tr.TransferToPms = new List<TransferToPms>();
                //                            //tr.TransferToPms = transferList as ICollection<TransferToPms>;

                //                            db.SaveChanges();
                //                            foreach (var to in objTosendList)
                //                            {
                //                                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                //                                sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                //                            }
                //                        }
                //                    }
                //                    //db.Transactions.Add(tr);
                //                }
                //                #endregion
                //            }
                //thanos1
                //            if (type == (int)OrderDetailUpdateType.InvoiceOnly)
                //            {
                //                var ord = ordetetailMappedList.FirstOrDefault();
                //                decimal? ingrGross = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0;
                //                invoice.Total = invoice.Total != null ? invoice.Total + ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross : ord.OrderDetailVatAnal.Sum(sm => sm.Gross) + ingrGross;
                //                decimal? ingrNet = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)) : 0;
                //                invoice.Net = invoice.Net != null ? invoice.Net + ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet : ord.OrderDetailVatAnal.Sum(sm => sm.Net) + ingrNet;
                //                decimal? ingrVat = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)) : 0;
                //                invoice.Vat = invoice.Vat != null ? invoice.Vat + ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat : ord.OrderDetailVatAnal.Sum(sm => sm.VatAmount) + ingrVat;
                //                decimal? ingrTax = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(sm => sm.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount)) : 0;
                //                invoice.Tax = invoice.Tax != null ? invoice.Tax + ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax : ord.OrderDetailVatAnal.Sum(sm => sm.TaxAmount) + ingrTax;
                //                var ingrDis = ord.OrderDetailIgredients.Count > 0 ? ord.OrderDetailIgredients.Sum(smm => smm.Discount) : 0;


                //                invoice.Total = ordetetailMappedList.Sum(s => s.TotalAfterDiscount) +ingrGross;
                //                invoice.Discount = ordetetailMappedList.Sum(s => s.Discount) + ingrDis;
                //                invoice.Vat = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm=>sm.VatAmount)) + ingrVat;
                //                invoice.Tax = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm=>sm.TaxAmount)) + ingrTax;
                //                invoice.Net = ordetetailMappedList.Sum(s => s.OrderDetailVatAnal.Sum(sm=>sm.Net)) +ingrNet;

                //            }
                //            if (type == (int)OrderDetailUpdateType.UnPaidCancel)
                //            {
                //                foreach (var inv in ordetetailMappedList)
                //                {
                //                    if (inv.OrderDetailInvoices != null && inv.OrderDetailInvoices.Count > 0)
                //                    {
                //                        var invexofl = inv.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false).OrderByDescending(ob => ob.Id).FirstOrDefault().InvoicesId;
                //                        if (invexofl != null)
                //                        {
                //                            var invoiceVoided = db.Invoices.Include(i => i.Transactions).Where(w => w.Id == invexofl).FirstOrDefault();
                //                            if (invoiceVoided != null)
                //                            {
                //                                invoiceVoided.IsVoided = true;
                //                                //An den einai eksoflimeni, eksoflise tin prwta vazwntas ta transactions
                //                                if (invoiceVoided.Transactions.Count == 0 && invoiceVoided.InvoiceTypes.Type != (int)InvoiceTypesEnum.Order)
                //                                {
                //                                    Transactions moufaTrans = new Transactions();
                //                                    Transactions voidMoufaTrans = new Transactions();
                //                                    moufaTrans.AccountId = db.Accounts.FirstOrDefault().Id;
                //                                    moufaTrans.Amount = invoiceVoided.Total;
                //                                    moufaTrans.Day = DateTime.Now;
                //                                    moufaTrans.Description = "Εξόφληση για ακύρωση";
                //                                    moufaTrans.Guid = Guid.NewGuid();
                //                                    moufaTrans.InOut = 0;
                //                                    moufaTrans.InvoicesId = invexofl;
                //                                    moufaTrans.PosInfoId = invoiceVoided.PosInfoId;
                //                                    moufaTrans.StaffId = invoiceVoided.StaffId;
                //                                    moufaTrans.TransactionType = (int)TransactionType.Sale;
                //                                    voidMoufaTrans.AccountId = db.Accounts.FirstOrDefault().Id;
                //                                    voidMoufaTrans.Amount = invoiceVoided.Total * (-1);
                //                                    voidMoufaTrans.Day = DateTime.Now;
                //                                    voidMoufaTrans.Description = "Ακύρωση";
                //                                    voidMoufaTrans.Guid = Guid.NewGuid();
                //                                    voidMoufaTrans.InOut = 1;
                //                                    voidMoufaTrans.PosInfoId = invoiceVoided.PosInfoId;
                //                                    voidMoufaTrans.StaffId = invoiceVoided.StaffId;
                //                                    voidMoufaTrans.TransactionType = (int)TransactionType.Cancel;
                //                                    db.Transactions.Add(moufaTrans);
                //                                    //To void Transaction mpainei sto void Transactions
                //                                    invoice.Transactions.Add(voidMoufaTrans);
                //                                }
                //                            }
                //                            break;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //        if (g.Count == order.OrderDetail.Count) // OLA TA OrderDetail THS PARAGGELIAS EINAI STO GROUPAKI
                //        {
                //            //Add New Order Status
                //            OrderStatus os = new OrderStatus();
                //            os.OrderId = order.Id;
                //            if (g.Status == 3 || g.Status == 2 || g.Status == 5)
                //            {
                //                os.Status = g.Status;
                //            }
                //            else
                //            {
                //                if (g.PaidStatus == 2)//paid
                //                {
                //                    os.Status = 7;//"Εξοφλημένη";
                //                    if (order.OrderStatus.Where(w => w.Status == 8).Count() == 0)//AN EINAI EXOFLISI KAI DEN EXEI TIMOLOGITHEI
                //                    {
                //                        OrderStatus os8 = new OrderStatus(); //PROSTHESE STATUS TIMOLOGISHS (8)
                //                        os8.OrderId = order.Id;
                //                        os8.Status = 8;
                //                        os8.StaffId = list.FirstOrDefault().StaffId;
                //                        os8.TimeChanged = DateTime.Now;
                //                        db.OrderStatus.Add(os8);
                //                    }
                //                }
                //                else if (g.PaidStatus == 1)//invoiced
                //                {
                //                    os.Status = 8;// "Τιμολογημένη";
                //                }
                //                else
                //                {
                //                    os.Status = g.Status;
                //                }
                //            }
                //            os.TimeChanged = DateTime.Now;
                //            os.StaffId = list.FirstOrDefault().StaffId;
                //            if (order.OrderStatus.Where(s => s.Status == os.Status).Count() == 0)//VALE TO NEO OrserStatus MONO AN DEN YPARXEI HDH
                //            {
                //                db.OrderStatus.Add(os);
                //            }
                //        }
                //    }
                //}

                #endregion

                //if (invoice.OrderDetailInvoices.Count > 0)
                //{
                //    db.Invoices.Add(invoice);
                //}
                //if (curTransactions.Count > 0)
                //{
                //    foreach (var t in curTransactions)
                //    {
                //        db.Transactions.Add(t);
                //    }
                //}
                //#endregion
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Info(ex.ToString());
                return ex.ToString();// Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var listupdate = OrderDet.OrderDet.Select(s => new
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
            try
            {
                if (OrderDet.CreditTransaction != null && OrderDet.CreditTransaction.Count > 0)
                {

                    foreach (var c in OrderDet.CreditTransaction)
                    {
                        if (invoice.Id > 0)
                        {
                            c.InvoiceId = invoice.Id;
                        }
                        else if (oldInvoice != null && oldInvoice.Id > 0)
                        {
                            c.InvoiceId = oldInvoice.Id;
                        }
                        db.CreditTransactions.Add(c);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return JsonConvert.SerializeObject(listupdate);//Request.CreateResponse(HttpStatusCode.Created, OrderDet.OrderDet);

        }

        private void AdjustPricesWithNewPricelist(bool paid, List<OrderDetail> ordetPaidlist, ref decimal amount, ref decimal? Netamount, ref decimal? Vatamount, ref decimal? Taxamount, ref decimal? Discountamount, decimal percentage, OrderDetailUpdateObj o, OrderDetail ord)
        {
            if (paid)
                ord.PaidStatus = 2;//Paid
            //Change Pricelist OrderDetail
            if (o.NewPrlId != null)
            {
                var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
                if (pricelistdetail != null)
                {
                    ord.PriceListDetailId = pricelistdetail.Id;
                    ord.Price = pricelistdetail.Price;
                    decimal tempprice = ord.Price != null ? Math.Round(ord.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                    tempprice = ord.Qty != null && ord.Qty > 0 ? (decimal)(ord.Qty.Value) * tempprice : tempprice;
                    ord.TotalAfterDiscount = tempprice;
                    ord.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
                }
            }
            //DISCOUNT
            //percentage = percentage > 0 ? percentage : 0;
            //decimal newamount = Math.Round(ord.TotalAfterDiscount.Value * percentage, 4);
            //if (counter == list.Count())//An eimaste sto teleytaio dwse oti ekptwsh exei meinei gia na mhn exoume diafores me tis stroggylopoihseis
            //{
            //    newamount = discountleft;
            //}
            //discountleft = discountleft - newamount;
            ord.TotalAfterDiscount = o.TotalAfterDiscount;//ord.TotalAfterDiscount.Value - newamount;
            ord.Discount = o.Discount;//ord.Discount != null ? ord.Discount + newamount : newamount;

            foreach (var anal in ord.OrderDetailVatAnal)
            {
                //Change Pricelist OrderDetailVatAnal
                if (o.NewPrlId != null)
                {
                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.ProductId == ord.ProductId).FirstOrDefault();
                    if (pricelistdetail != null)
                    {
                        anal.Gross = ord.TotalAfterDiscount;
                        Vat vat = pricelistdetail.Vat;
                        Tax tax = pricelistdetail.Tax;
                        var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
                        var tempvat = (decimal)(anal.Gross - tempnetbyvat);
                        var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                        var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                        anal.Net = (decimal)(anal.Gross - tempvat - temptax);
                        anal.TaxAmount = temptax;
                        anal.VatAmount = tempvat;
                        anal.VatRate = vat != null ? vat.Percentage : 0;
                        anal.VatId = vat != null ? (long?)vat.Id : null;
                        anal.TaxId = tax != null ? (long?)tax.Id : null;
                    }
                }
                if (o.Discount != null && o.NewPrlId == null)
                {
                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == ord.PriceListDetailId).FirstOrDefault();
                    anal.Gross = ord.TotalAfterDiscount;
                    Vat vat = pricelistdetail.Vat;
                    Tax tax = pricelistdetail.Tax;
                    var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal.Gross.Value) : anal.Gross.Value;
                    var tempvat = (decimal)(anal.Gross - tempnetbyvat);
                    var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                    var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                    anal.Net = (decimal)(anal.Gross - tempvat - temptax);
                    anal.TaxAmount = temptax;
                    anal.VatAmount = tempvat;
                    anal.VatRate = vat != null ? vat.Percentage : 0;
                    anal.VatId = vat != null ? (long?)vat.Id : null;
                    anal.TaxId = tax != null ? (long?)tax.Id : null;
                }

                Netamount = Netamount != null ? Netamount + anal.Net : anal.Net;
                Vatamount = Vatamount != null ? Vatamount + anal.VatAmount : anal.VatAmount;
                Taxamount = Taxamount != null ? Taxamount + anal.TaxAmount : anal.TaxAmount;
            }
            foreach (var odi in ord.OrderDetailIgredients)
            {
                //Change Pricelist OrderDetailIgredients
                if (o.NewPrlId != null)
                {
                    var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                    if (pricelistdetail != null)
                    {
                        odi.PriceListDetailId = pricelistdetail.Id;
                        odi.Price = pricelistdetail.Price;
                        decimal tempprice = odi.Price != null ? Math.Round(odi.Price.Value, 2) : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                        tempprice = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice : tempprice;
                        odi.TotalAfterDiscount = tempprice;
                        odi.Discount = null;//NO PRIOR DISCOUNT ALLOWED 
                    }
                }
                decimal newingamount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value * percentage), 2) : 0;
                odi.TotalAfterDiscount = odi.TotalAfterDiscount != null && odi.TotalAfterDiscount > 0 ? Math.Round((odi.TotalAfterDiscount.Value - newingamount), 2) : 0;
                odi.Discount = odi.Discount != null ? odi.Discount + newingamount : newingamount;
                amount += odi.TotalAfterDiscount.Value;
                Discountamount = Discountamount != null ? Discountamount + odi.Discount : odi.Discount;
                //Change Pricelist
                if (o.NewPrlId != null)
                {
                    var pricelistdetailing = db.PricelistDetail.Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                    if (pricelistdetailing != null)
                    {
                        odi.PriceListDetailId = pricelistdetailing.Id;
                        odi.Price = pricelistdetailing.Price;
                    }
                }
                foreach (var anal2 in odi.OrderDetailIgredientVatAnal)
                {
                    //Change Pricelist OrderDetailIgredientVatAnal
                    if (o.NewPrlId != null)
                    {
                        var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.PricelistId == o.NewPrlId && w.IngredientId == odi.IngredientId).FirstOrDefault();
                        if (pricelistdetail != null)
                        {
                            anal2.Gross = odi.TotalAfterDiscount;
                            Vat vat = pricelistdetail.Vat;
                            Tax tax = pricelistdetail.Tax;
                            var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
                            var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
                            var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                            var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                            anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
                            anal2.TaxAmount = temptax;
                            anal2.VatAmount = tempvat;
                            anal2.VatRate = vat != null ? vat.Percentage : 0;
                            anal2.VatId = vat != null ? (long?)vat.Id : null;
                            anal2.TaxId = tax != null ? (long?)tax.Id : null;
                        }
                    }
                    //Discount on  OrderDetailIgredientVatAnal
                    if (odi.Discount != null && o.NewPrlId == null)
                    {
                        var pricelistdetail = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault();
                        if (pricelistdetail != null)
                        {
                            anal2.Gross = odi.TotalAfterDiscount;
                            Vat vat = pricelistdetail.Vat;
                            Tax tax = pricelistdetail.Tax;
                            var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, anal2.Gross.Value) : anal2.Gross.Value;
                            var tempvat = (decimal)(anal2.Gross - tempnetbyvat);
                            var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                            var temptax = (decimal)(tempnetbyvat - tempnetbytax);
                            anal2.Net = (decimal)(anal2.Gross - tempvat - temptax);
                            anal2.TaxAmount = temptax;
                            anal2.VatAmount = tempvat;
                            anal2.VatRate = vat != null ? vat.Percentage : 0;
                            anal2.VatId = vat != null ? (long?)vat.Id : null;
                            anal2.TaxId = tax != null ? (long?)tax.Id : null;
                        }
                    }
                    Netamount = Netamount != null ? Netamount + anal2.Net : anal2.Net;
                    Vatamount = Vatamount != null ? Vatamount + anal2.VatAmount : anal2.VatAmount;
                    Taxamount = Taxamount != null ? Taxamount + anal2.TaxAmount : anal2.TaxAmount;
                }
            }
            amount += ord.TotalAfterDiscount.Value;//- newamount;
            Discountamount = Discountamount != null ? Discountamount + ord.Discount : ord.Discount;
            if (paid)

                ordetPaidlist.Add(ord);
        }

        private List<Transactions> CreateTransactionsWithTransfer(OrderDetailUpdateObjList OrderDet, int type, IEnumerable<OrderDetailUpdateObj> list, Invoices invoice, Invoices oldInvoice, List<OrderDetail> ordetetailMappedList, decimal amount, decimal? Netamount, decimal? Vatamount, decimal? Taxamount, decimal? Discountamount, long updateReceiptNo, long newCounter)
        {

            List<Transactions> curTransactions = new List<Transactions>();
            if (type == (int)OrderDetailUpdateType.PayOff || type == (int)OrderDetailUpdateType.PaidCancel)
            {
                var posinfo = db.PosInfo.Find(list.FirstOrDefault().PosId);
                var tempr = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).OrderByDescending(o => o.InvoicesId).GroupBy(g => g.OrderDetailId);
                var que = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).OrderByDescending(o => o.InvoicesId).GroupBy(g => g.OrderDetailId).Select(s => new
                {
                    OrderDetailId = s.FirstOrDefault().OrderDetailId,
                    InvoiceId = s.Max(m => m.InvoicesId),
                    TotalAfterDiscount = (decimal?)((decimal?)s.FirstOrDefault().OrderDetail.TotalAfterDiscount) + ((decimal?)(s.FirstOrDefault().OrderDetail.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount)) ?? 0),
                    Total = s.FirstOrDefault().Invoices.Total,
                    //  InvoiceType = s.FirstOrDefault().Invoices.InvoiceTypes.Type
                }).Distinct().ToList();

                //  var isFromOrder = que.Any(a => a.InvoiceType == 2);

                var groupedByInvoice = que/*.Where(w => w.InvoiceType != (int)InvoiceTypesEnum.Order)*/.GroupBy(g => g.InvoiceId).Select(s => new
                {
                    InvoiceId = s.FirstOrDefault().InvoiceId,
                    Total = s.Sum(sm => sm.TotalAfterDiscount),
                    InvTotal = s.FirstOrDefault().Total,
                    // InvoiceType = s.FirstOrDefault().InvoiceType
                });

                //var groupedByInvoiceType = que.Where(w => w.InvoiceType == (int)InvoiceTypesEnum.Order).GroupBy(g => g.InvoiceType).Select(s => new
                //{
                //    InvoiceId = s.FirstOrDefault().InvoiceId,
                //    Total = s.Sum(sm => (Decimal?)sm.TotalAfterDiscount),
                //    InvTotal = (Decimal?)s.FirstOrDefault().Total,
                //    InvoiceType = s.FirstOrDefault().InvoiceType
                //});

                IEnumerable<dynamic> uniongroup = groupedByInvoice;//.Union(groupedByInvoiceType);
                //if (isFromOrder)
                //{
                //    List<dynamic> temp = new List<dynamic>();
                //    temp.Add(new
                //    {
                //        InvoiceId = invoice.Id,
                //        Total = invoice.Total,
                //        InvTotal = invoice.Total,
                //        InvoiceType = 2
                //    });
                //    uniongroup = groupedByInvoiceType;
                //}


                //var uniongroup = groupedByInvoice.Union(groupedByInvoiceType);

                //Arxikopoiisi accountId kai GuestId se periptosi pou exei mono enan tropo exoflisis gia ola ta parastatika
                long? accountid = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : posinfo.AccountId;
                long? guestid = invoice.GuestId;
                //
                List<dynamic> query2 = new List<dynamic>();
                if (OrderDet.AccountsObj != null)
                {
                    decimal TotalAmounts = OrderDet.AccountsObj.Sum(s => s.Amount);
                    foreach (var a in OrderDet.AccountsObj)
                    {

                        //EpimerismosPerInvoice
                        decimal totalDiscount = TotalAmounts - a.Amount;
                        decimal percentageEpim = TotalAmounts == 0 ? 0 : 1 - (decimal)(a.Amount / TotalAmounts);
                        decimal totalEpim = 0;
                        decimal remainingDiscount = totalDiscount;
                        decimal ctr = 1;

                        uniongroup = uniongroup.OrderBy(or => or.Total);
                        foreach (var f in uniongroup)
                        {
                            if (ctr < uniongroup.Count())
                            {
                                decimal subtotal = f.Total;
                                decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                                totalEpim += subtotal - percSub;
                                query2.Add(new
                                {
                                    InvoiceId = f.InvoiceId,
                                    Total = subtotal - percSub,
                                    AccountId = a.AccountId,
                                    GuestId = a.GuestId
                                });
                                remainingDiscount = remainingDiscount - percSub;
                            }
                            else
                            {
                                decimal subtotal = f.Total;
                                query2.Add(new
                                {
                                    InvoiceId = f.InvoiceId,
                                    Total = subtotal - remainingDiscount,
                                    AccountId = a.AccountId,
                                    GuestId = a.GuestId
                                });
                                totalEpim += subtotal - remainingDiscount;
                            }
                            ctr++;
                        }
                    }
                }
                //

                List<dynamic> query3 = (from q in uniongroup
                                        select new
                                        {
                                            InvoiceId = q.InvoiceId,
                                            Total = q.Total,
                                            AccountId = accountid,
                                            GuestId = guestid,
                                        }).ToList<dynamic>();
                if (query2.Count() > 0)
                {
                    query3 = (from q in uniongroup
                              join j in query2 on q.InvoiceId equals j.InvoiceId
                              select new
                              {
                                  InvoiceId = q.InvoiceId,
                                  Total = j.Total,
                                  AccountId = j != null ? j.AccountId : accountid,
                                  GuestId = j != null ? j.GuestId : guestid,

                              }).ToList<dynamic>();
                }

                //Incase there are more than one accounts
                foreach (dynamic inv in query3)
                {
                    CreateTransactions(type, list, invoice, Discountamount, curTransactions, posinfo, inv.AccountId, inv.GuestId, inv);
                    if (type == (int)OrderDetailUpdateType.PayOff)
                    {
                        var totInv = uniongroup.Where(w => w.InvoiceId == inv.InvoiceId).FirstOrDefault();
                        var curInvDetails = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).Where(w => w.InvoicesId == inv.InvoiceId).Select(s => s.OrderDetailId);
                        // if (isFromOrder)
                        //       curInvDetails = ordetetailMappedList.SelectMany(s => s.OrderDetailInvoices).Select(s => s.OrderDetailId);
                        var temppid = db.PosInfoDetail.Include(i => i.PosInfo).Where(w => w.Id == updateReceiptNo).FirstOrDefault();
                        UpdatePayOffNew(invoice, curTransactions.LastOrDefault(), ordetetailMappedList.Where(w => curInvDetails.Contains(w.Id)).ToList(), inv.AccountId, inv.GuestId, totInv.Total, temppid, false/* isFromOrder*/);
                        // }
                    }
                }


                if (type == (int)OrderDetailUpdateType.PayOff)
                {
                    invoice.Total = amount;//invoice.Total != null ? invoice.Total + tr.Gross : tr.Gross;
                    invoice.Net = Netamount;//invoice.Net != null ? invoice.Net + tr.Net : tr.Net;
                    invoice.Vat = Vatamount;//invoice.Vat != null ? invoice.Vat + tr.Vat : tr.Vat;
                    invoice.Tax = Taxamount;//invoice.Tax != null ? invoice.Tax + tr.Tax : tr.Tax;
                    //var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                    invoice.Discount = Discountamount;//invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;
                }
            }
            return curTransactions;
        }



        private static void CreateTransactions(int type, IEnumerable<OrderDetailUpdateObj> list, Invoices invoice, decimal? Discountamount, List<Transactions> curTransactions, PosInfo posinfo, long? accountid, long? guestid, dynamic inv)
        {
            Transactions tr = new Transactions();
            tr.AccountId = accountid;//a.AccountId;
            tr.PosInfoId = posinfo.Id;
            tr.Amount = type == (int)OrderDetailUpdateType.PayOff ? inv.Total : inv.Total * -1;
            tr.Day = DateTime.Now;
            tr.DepartmentId = posinfo.DepartmentId;
            tr.Description = type == (int)OrderDetailUpdateType.PayOff ? "Pay Off" : "Cancel receipt";
            tr.ExtDescription = "Helper Controller";
            tr.InOut = type == (int)OrderDetailUpdateType.PayOff ? (short)0 : (short)1;

            tr.StaffId = list.FirstOrDefault().StaffId;
            tr.TransactionType = type == (int)OrderDetailUpdateType.PayOff ? (short)TransactionTypesEnum.Sale : (short)TransactionTypesEnum.Cancel;

            if (type == (int)OrderDetailUpdateType.PayOff)
            {
                //Vghkan ap eksw
                //invoice.Total = inv.Total;//amount;//invoice.Total != null ? invoice.Total + tr.Gross : tr.Gross;
                //invoice.Net = Netamount;//invoice.Net != null ? invoice.Net + tr.Net : tr.Net;
                //invoice.Vat = Vatamount;//invoice.Vat != null ? invoice.Vat + tr.Vat : tr.Vat;
                //invoice.Tax = Taxamount;//invoice.Tax != null ? invoice.Tax + tr.Tax : tr.Tax;
                //var discountPaid = g.PaidOrderDetails.Sum(s => s.Discount) + g.PaidOrderDetails.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
                invoice.Discount = Discountamount;//invoice.Discount != null ? invoice.Discount + discountPaid : discountPaid;

                if (guestid != null)
                {
                    Invoice_Guests_Trans igt = new Invoice_Guests_Trans();
                    igt.GuestId = guestid;
                    if (tr.Invoice_Guests_Trans == null)
                    {
                        tr.Invoice_Guests_Trans = new List<Invoice_Guests_Trans>();
                    }
                    tr.Invoice_Guests_Trans.Add(igt);
                    if (invoice.Invoice_Guests_Trans == null)
                    {
                        invoice.Invoice_Guests_Trans = new List<Invoice_Guests_Trans>();
                    }
                    invoice.Invoice_Guests_Trans.Add(igt);
                }


            }
            tr.InvoicesId = inv.InvoiceId;
            tr.Guid = Guid.NewGuid();
            invoice.Transactions.Add(tr);
            //db.Transactions.Add(tr);
            curTransactions.Add(tr);
        }

        private void UpdatePayOffNew(Invoices invoice, Transactions tr, List<OrderDetail> ordetetailMappedList, long accountId, long? guestId, decimal inv, PosInfoDetail pid, bool isFromOrder)
        //    Invoices invoice, List<Transactions> curTransactions, List<OrderDetail> ordetetailMappedList, long newCounter,
        //            OrderDetailUpdateObjList OrderDet, long updateReceiptNo, PosInfoDetail pid, long posid, Invoices oldInvoice)
        {
            var posInfo = db.PosInfo.FirstOrDefault(w => w.Id == invoice.PosInfoId);
            var gr = ordetetailMappedList.Where(w => (w.IsDeleted ?? false) == false && w.Status != 5 && w.PaidStatus == 2);
            foreach (var g in gr)
            {
                //Paid && AN DEN YPARXEI HDH STATUS EXOFLISIS

                var orderdetailinvoices = g.OrderDetailInvoices.Where(w => w.PosInfoDetail.CreateTransaction == false && w.PosInfoDetail.IsInvoice == true).FirstOrDefault();
                if (orderdetailinvoices != null)
                {
                    orderdetailinvoices.StaffId = tr.StaffId;
                }
                List<TablePaySuggestion> todelete = new List<TablePaySuggestion>();
                foreach (var t in g.TablePaySuggestion)
                {
                    todelete.Add(t);
                }
                foreach (var t in todelete)
                {
                    db.TablePaySuggestion.Remove(t);
                }
            }

            var hotel = db.HotelInfo.FirstOrDefault();


            List<AccountsObj> accountsList = new List<AccountsObj>();
            var acc = db.Accounts.Find(accountId);
            if (acc != null)
            {
                AccountsObj accobj = new AccountsObj();
                accobj.Account = acc;
                accobj.AccountId = accountId;
                accobj.Amount = tr.Amount.Value;
                accobj.GuestId = guestId;
                accountsList.Add(accobj);
            }

            decimal TotalAmounts = inv;// accountsList.Sum(s => s.Amount);

            foreach (var ac in accountsList)
            {

                var account = ac.Account;
                if (account != null && hotel != null)//&& account.SendsTransfer == true (tha chekaristei otan einai na stalei)
                {

                    var deps = gr.Select(s => s.Order.PosInfo.DepartmentId);
                    var pril = gr.Select(s => s.PricelistDetail.PricelistId);
                    var prods = gr.Select(s => s.ProductId);
                    var query = (from f in gr//g.PaidOrderDetails
                                 join st in db.SalesType on f.SalesTypeId equals st.Id
                                 join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                 on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                 from ls in loj.DefaultIfEmpty()
                                 select new
                                 {
                                     Id = f.Id,
                                     SalesTypeId = st.Id,
                                     Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                     OrderDetail = f,
                                     PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                     PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                     // CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                     ReceiptNo = isFromOrder ? invoice.Counter : f.OrderDetailInvoices.LastOrDefault().Counter,
                                     //InvoiceId = f.OrderDetailInvoices.FirstOrDefault().InvoicesId
                                 }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new   //new { ag.PmsDepartmentId, ag.InvoiceId}
                                 {
                                     PmsDepartmentId = s.Key,//.PmsDepartmentId,
                                     PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                     Total = s.Sum(sm => sm.Total),
                                     OrderDetails = s.Select(ss => ss.OrderDetail),
                                     //CustomerId = s.FirstOrDefault().CustomerId,
                                     ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                     //InvoiceId = s.FirstOrDefault().InvoiceId
                                 });


                    // long guestid = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;//tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1;
                    long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;
                    Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                    string storeid = HttpContext.Current.Request.Params["storeid"];
                    List<TransferObject> objTosendList = new List<TransferObject>();
                    List<TransferToPms> transferList = new List<TransferToPms>();

                    //var IsCreditCard = false;
                    //var roomOfCC = "";
                    //if (account.Type == 4)
                    //{
                    //    IsCreditCard = true;
                    //    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                    //        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                    //}
                    var IsNotSendingTransfer = account.SendsTransfer == false;
                    var IsCreditCard = false;
                    var roomOfCC = "";
                    if (account.Type == 4 || IsNotSendingTransfer == true)
                    {
                        IsCreditCard = true;
                        roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == posInfo.Id).FirstOrDefault() != null ?
                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == posInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                    }


                    //EpimerismosPerDepartment
                    decimal totalDiscount = TotalAmounts - ac.Amount;
                    decimal percentageEpim = TotalAmounts == 0 ? 0 : 1 - (decimal)(ac.Amount / TotalAmounts);
                    decimal totalEpim = 0;
                    decimal remainingDiscount = totalDiscount;
                    decimal ctr = 1;
                    List<dynamic> query2 = new List<dynamic>();
                    query = query.OrderBy(or => or.Total);
                    foreach (var f in query)
                    {
                        if (ctr < query.Count())
                        {
                            decimal subtotal = f.Total;
                            decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                            totalEpim += subtotal - percSub;
                            query2.Add(new
                            {
                                PmsDepartmentId = f.PmsDepartmentId,
                                Total = subtotal - percSub
                            });
                            remainingDiscount = remainingDiscount - percSub;
                        }
                        else
                        {
                            decimal subtotal = f.Total;
                            query2.Add(new
                            {
                                PmsDepartmentId = f.PmsDepartmentId,
                                Total = subtotal - remainingDiscount
                            });
                            totalEpim += subtotal - remainingDiscount;
                        }
                        ctr++;
                    }
                    //
                    var query3 = from q in query
                                 join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                                 select new
                                 {
                                     PmsDepartmentId = q.PmsDepartmentId,
                                     PmsDepDescription = q.PmsDepDescription,
                                     Total = j.Total,
                                     //OrderDetails = q.OrderDetails,
                                     //CustomerId = q.CustomerId,
                                     ReceiptNo = q.ReceiptNo,
                                     //TransactionId = q.TransactionId
                                 };
                    //
                    var departmentDescritpion = db.Department.FirstOrDefault(f => f.Id == posInfo.DepartmentId);
                    string depstr = departmentDescritpion != null ? departmentDescritpion.Description : posInfo.Description;

                    foreach (var acg in query3)
                    {
                        decimal total = acg.Total;//new Economic().EpimerisiAccountTotal(acg.OrderDetails, acg.Total);
                        TransferToPms tpms = new TransferToPms(); //newCounter
                        tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + posInfo.Id + ", " + depstr;// posInfo.Description;
                        tpms.PmsDepartmentId = acg.PmsDepartmentId;
                        tpms.PosInfoDetailId = pid.Id;
                        tpms.ProfileId = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ProfileNo.ToString() : "" : null;//acg.CustomerId;
                        tpms.ProfileName = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                        tpms.ReceiptNo = acg.ReceiptNo.ToString();
                        tpms.RegNo = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                        tpms.RoomDescription = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                        tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                        tpms.SendToPMS = !IsNotSendingTransfer;
                        //Set Status Flag (0: Cash, 1: Not Cash)
                        tpms.Status = IsNotSendingTransfer ? (short)0 : (short)1;
                        tpms.PosInfoId = posInfo.Id;
                        tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                        //tpms.TransactionId = tr.Id;
                        tpms.TransferType = 0;//Xrewstiko
                        tpms.SendToPmsTS = DateTime.Now;
                        tpms.Total = total;// (decimal)acg.Total;
                        var identifier = Guid.NewGuid();
                        tpms.TransferIdentifier = identifier;
                        transferList.Add(tpms);
                        db.TransferToPms.Add(tpms);

                        //Link transferToPms with correct Transaction
                        var trans = tr;
                        if (trans != null)
                        {
                            trans.TransferToPms.Add(tpms);
                        }


                        TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, identifier);


                        if (to.amount != 0 && ac.Account.SendsTransfer == true)
                            objTosendList.Add(to);
                    }
                    //tr.TransferToPms = new List<TransferToPms>();
                    //tr.TransferToPms = transferList as ICollection<TransferToPms>;

                    db.SaveChanges();
                    foreach (var to in objTosendList)
                    {
                        SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                        //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                    }
                }
            }
        }

        private void UpdatePayOff(Invoices invoice, List<Transactions> curTransactions, List<OrderDetail> ordetetailMappedList, long newCounter,
            OrderDetailUpdateObjList OrderDet, long updateReceiptNo, PosInfoDetail pid, long posid, Invoices oldInvoice)
        {
            var gr = ordetetailMappedList.Where(w => (w.IsDeleted ?? false) == false && w.Status != 5 && w.PaidStatus == 2);
            foreach (var g in gr)
            {
                //Paid && AN DEN YPARXEI HDH STATUS EXOFLISIS

                var orderdetailinvoices = g.OrderDetailInvoices.Where(w => w.PosInfoDetail.CreateTransaction == false && w.PosInfoDetail.IsInvoice == true).FirstOrDefault();
                if (orderdetailinvoices != null)
                {
                    orderdetailinvoices.StaffId = OrderDet.OrderDet.FirstOrDefault().StaffId;
                }
                List<TablePaySuggestion> todelete = new List<TablePaySuggestion>();
                foreach (var t in g.TablePaySuggestion)
                {
                    todelete.Add(t);
                }
                foreach (var t in todelete)
                {
                    db.TablePaySuggestion.Remove(t);
                }
            }

            long accountId = OrderDet.OrderDet.FirstOrDefault().AccountId.Value;
            var hotel = db.HotelInfo.FirstOrDefault();
            List<AccountsObj> accountsList = new List<AccountsObj>();
            if (OrderDet.AccountsObj != null)
            {
                foreach (var a in OrderDet.AccountsObj)
                {
                    var acc = db.Accounts.Find(a.AccountId);
                    if (acc != null)
                    {
                        AccountsObj accobj = new AccountsObj();
                        accobj.Account = acc;
                        accobj.AccountId = a.AccountId;
                        accobj.Amount = a.Amount;
                        accobj.GuestId = a.GuestId;
                        accountsList.Add(accobj);
                    }
                }
            }
            else
            {
                AccountsObj accobj = new AccountsObj();
                accobj.Account = db.Accounts.Find(accountId);
                accobj.AccountId = accountId;
                //accobj.Amount = g.PaidOrderDetails.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                accobj.Amount = gr.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
                accobj.GuestId = OrderDet.OrderDet.FirstOrDefault().GuestId != null ? OrderDet.OrderDet.FirstOrDefault().GuestId.Value : -1;
                accountsList.Add(accobj);
            }
            decimal TotalAmounts = accountsList.Sum(s => s.Amount);



            foreach (var ac in accountsList)
            {

                var account = ac.Account;
                if (account != null && hotel != null)//&& account.SendsTransfer == true (tha chekaristei otan einai na stalei)
                {

                    var deps = gr.Select(s => s.Order.PosInfo.DepartmentId);
                    var pril = gr.Select(s => s.PricelistDetail.PricelistId);
                    var prods = gr.Select(s => s.ProductId);
                    var query = (from f in gr//g.PaidOrderDetails
                                 join st in db.SalesType on f.SalesTypeId equals st.Id
                                 join tm in db.TransferMappings.Where(w => deps.Contains(w.PosDepartmentId) && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                 on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                 from ls in loj.DefaultIfEmpty()
                                 select new
                                 {
                                     Id = f.Id,
                                     SalesTypeId = st.Id,
                                     Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                     OrderDetail = f,
                                     PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                     PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                     // CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
                                     ReceiptNo = f.OrderDetailInvoices.FirstOrDefault().Counter,
                                     //InvoiceId = f.OrderDetailInvoices.FirstOrDefault().InvoicesId
                                 }).Distinct().GroupBy(ag => ag.PmsDepartmentId).Select(s => new   //new { ag.PmsDepartmentId, ag.InvoiceId}
                                 {
                                     PmsDepartmentId = s.Key,//.PmsDepartmentId,
                                     PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                     Total = s.Sum(sm => sm.Total),
                                     OrderDetails = s.Select(ss => ss.OrderDetail),
                                     //CustomerId = s.FirstOrDefault().CustomerId,
                                     ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                     //InvoiceId = s.FirstOrDefault().InvoiceId
                                 });

                    //var query = from q in query0
                    //            join t in curTransactions on q.InvoiceId equals t.InvoicesId
                    //             select new
                    //             {
                    //                 PmsDepartmentId = q.PmsDepartmentId,
                    //                 PmsDepDescription = q.PmsDepDescription,
                    //                 Total = (decimal)t.Amount,
                    //                 ReceiptNo = q.ReceiptNo,
                    //                 TransactionId = t.Guid
                    //             };

                    // long guestid = list.FirstOrDefault().GuestId != null ? list.FirstOrDefault().GuestId.Value : -1;//tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1;
                    long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;
                    Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                    string storeid = HttpContext.Current.Request.Params["storeid"];
                    List<TransferObject> objTosendList = new List<TransferObject>();
                    List<TransferToPms> transferList = new List<TransferToPms>();

                    //var IsCreditCard = false;
                    //var roomOfCC = "";
                    //if (account.Type == 4)
                    //{
                    //    IsCreditCard = true;
                    //    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault() != null ?
                    //        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == order.PosInfo.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                    //}
                    var IsNotSendingTransfer = account.SendsTransfer == false;
                    var IsCreditCard = false;
                    var roomOfCC = "";
                    if (account.Type == 4 || IsNotSendingTransfer == true)
                    {
                        IsCreditCard = true;
                        roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                    }


                    //EpimerismosPerDepartment
                    decimal totalDiscount = TotalAmounts - ac.Amount;
                    decimal percentageEpim = TotalAmounts == 0 ? 0 : 1 - (decimal)(ac.Amount / TotalAmounts);
                    decimal totalEpim = 0;
                    decimal remainingDiscount = totalDiscount;
                    decimal ctr = 1;
                    List<dynamic> query2 = new List<dynamic>();
                    query = query.OrderBy(or => or.Total);
                    foreach (var f in query)
                    {
                        if (ctr < query.Count())
                        {
                            decimal subtotal = f.Total;
                            decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                            totalEpim += subtotal - percSub;
                            query2.Add(new
                            {
                                PmsDepartmentId = f.PmsDepartmentId,
                                Total = subtotal - percSub
                            });
                            remainingDiscount = remainingDiscount - percSub;
                        }
                        else
                        {
                            decimal subtotal = f.Total;
                            query2.Add(new
                            {
                                PmsDepartmentId = f.PmsDepartmentId,
                                Total = subtotal - remainingDiscount
                            });
                            totalEpim += subtotal - remainingDiscount;
                        }
                        ctr++;
                    }
                    //
                    var query3 = from q in query
                                 join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                                 select new
                                 {
                                     PmsDepartmentId = q.PmsDepartmentId,
                                     PmsDepDescription = q.PmsDepDescription,
                                     Total = j.Total,
                                     //OrderDetails = q.OrderDetails,
                                     //CustomerId = q.CustomerId,
                                     ReceiptNo = q.ReceiptNo,
                                     //TransactionId = q.TransactionId
                                 };
                    //
                    foreach (var acg in query3)
                    {
                        decimal total = acg.Total;//new Economic().EpimerisiAccountTotal(acg.OrderDetails, acg.Total);
                        TransferToPms tpms = new TransferToPms(); //newCounter
                        tpms.Description = "Rec: " + acg.ReceiptNo + ", Pos: " + posid + ", " + pid.PosInfo.Description;
                        tpms.PmsDepartmentId = acg.PmsDepartmentId;
                        tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                        tpms.ProfileId = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ProfileNo.ToString() : "" : null;//acg.CustomerId;
                        tpms.ProfileName = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                        tpms.ReceiptNo = acg.ReceiptNo.ToString();
                        tpms.RegNo = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                        tpms.RoomDescription = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                        tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                        tpms.SendToPMS = !IsNotSendingTransfer;
                        //Set Status Flag (0: Cash, 1: Not Cash)
                        tpms.Status = IsNotSendingTransfer ? (short)0 : (short)1;
                        tpms.PosInfoId = posid;
                        tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                        //tpms.TransactionId = tr.Id;
                        tpms.TransferType = 0;//Xrewstiko
                        tpms.SendToPmsTS = DateTime.Now;
                        tpms.Total = total;// (decimal)acg.Total;
                        var identifier = Guid.NewGuid();
                        tpms.TransferIdentifier = identifier;
                        transferList.Add(tpms);
                        db.TransferToPms.Add(tpms);

                        //Link transferToPms with correct Transaction
                        var trans = curTransactions.Where(w => w.AccountId == ac.AccountId).LastOrDefault();
                        if (trans != null)
                        {
                            trans.TransferToPms.Add(tpms);
                        }


                        TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, identifier);


                        if (to.amount != 0 && ac.Account.SendsTransfer == true)
                            objTosendList.Add(to);
                    }


                    //tr.TransferToPms = new List<TransferToPms>();
                    //tr.TransferToPms = transferList as ICollection<TransferToPms>;

                    db.SaveChanges();
                    foreach (var to in objTosendList)
                    {
                        SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                        //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                    }
                }
            }
        }



        [System.Web.Http.HttpPut]
        public bool UpdateMetadataFields(IEnumerable<MetadataTable> meta, Int64 reportType)
        {
            // var flds = meta.Select(s => s.FieldName);
            var metaforUpdate = db.MetadataTable.Where(w => w.ReportType == reportType);

            foreach (var item in meta)
            {
                var upd = metaforUpdate.Where(w => w.FieldName == item.FieldName).FirstOrDefault();
                if (upd != null)
                {
                    upd.FieldName = item.FieldName;
                    upd.Description = item.Description;
                    upd.FieldType = item.FieldType;
                    upd.DefaultStyle = item.DefaultStyle;
                    upd.Summable = item.Summable;
                    upd.Expression = item.Expression;

                }
                else
                {
                    db.MetadataTable.Add(item);
                }


            }

            db.SaveChanges();
            return false;
        }

        [System.Web.Http.HttpGet]
        public string PutTable(string ids, long tableid)
        {
            List<long> orderdetails = JsonConvert.DeserializeObject<List<long>>(ids);
            string result = "";
            foreach (var o in orderdetails)
            {
                var orderdet = db.OrderDetail.Include(i => i.OrderDetailInvoices.Select(s => s.Invoices)).Where(w => w.Id == o).FirstOrDefault();
                if (orderdet != null)
                {
                    result += o + " changed from TableId " + orderdet.TableId + " to " + tableid;
                    orderdet.TableId = tableid;
                    if (orderdet.OrderDetailInvoices.Count > 0)
                    {
                        foreach (var inv in orderdet.OrderDetailInvoices)
                        {
                            if (inv.Invoices != null)
                            {
                                inv.Invoices.TableId = tableid;
                            }
                        }
                    }
                }
            }
            db.SaveChanges();

            return result;
        }

        [System.Web.Http.HttpGet]
        public dynamic PutTable(long regionId, string tableCode, long posid, string ids)
        {
            dynamic res = new ExpandoObject();
            var table = db.Table.Include(i => i.OrderDetail).Include("OrderDetail.Order").Where(w => w.Code == tableCode && w.RegionId == regionId).FirstOrDefault();
            if (table != null)
            {
                var query = table.OrderDetail.Where(ww => (ww.PaidStatus != 2 || ww.PaidStatus == null) && ww.Status != 5 && ww.Order.EndOfDayId == null);
                var fromotherpos = query.Where(w => w.Order.PosId != posid).Count();
                if (fromotherpos > 0)
                {

                    res.Error = "Table " + tableCode + " has orders from another POS.";
                    return res;
                }
                else
                {
                    List<long> orderdetails = JsonConvert.DeserializeObject<List<long>>(ids);
                    string result = "";
                    foreach (var o in orderdetails)
                    {
                        var orderdet = db.OrderDetail.Include(i => i.OrderDetailInvoices.Select(s => s.Invoices)).Include("Product").Include("Table").Where(w => w.Id == o).FirstOrDefault();
                        if (orderdet != null)
                        {
                            result += orderdet.Product.Description + " changed from Table " + orderdet.Table.Code + " to Table " + table.Code + "<br/>";
                            orderdet.TableId = table.Id;
                            if (orderdet.OrderDetailInvoices.Count > 0)
                            {
                                foreach (var inv in orderdet.OrderDetailInvoices)
                                {
                                    if (inv.Invoices != null)
                                    {
                                        inv.Invoices.TableId = table.Id;
                                    }
                                }
                            }
                        }
                    }
                    res.Success = result;
                    db.SaveChanges();
                    return res;
                }
            }
            //db.SaveChanges();
            res.Error = "Table Not Found";
            return res;
        }

        [System.Web.Http.HttpGet]
        public string UpdateTableStaff(string orderIds, long newstaffid, bool chngwaiter)
        {
            db.Configuration.LazyLoadingEnabled = true;
            List<long> orders = JsonConvert.DeserializeObject<List<long>>(orderIds);
            string result = "";
            foreach (var o in orders)
            {
                var order = db.Order.Find(o);
                if (order != null)
                {
                    order.StaffId = newstaffid;
                    foreach (var od in order.OrderDetail)
                    {
                        foreach (var odinv in od.OrderDetailInvoices)
                        {
                            odinv.StaffId = newstaffid;
                            odinv.Invoices.StaffId = newstaffid;
                        }
                    }
                }
            }
            db.SaveChanges();
            db.Configuration.LazyLoadingEnabled = false;
            return result;
        }

        //[System.Web.Http.HttpGet]
        //public string GenericMethod(int method, string query)
        //{
        //    switch (method)
        //    {
        //        case 1: return TableUpdate(query);

        //    }

        //}

        //private Object TableUpdate(string query)
        //{


        //}

        [System.Web.Http.HttpGet]
        public string DeleteOrder(string listOfGuids, bool isPayOffFromTable = false)
        {
            if (listOfGuids != "")
            {
                List<Guid> orderDetailsGuid = JsonConvert.DeserializeObject<List<Guid>>(listOfGuids);
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                if (orderDetailsGuid.Count > 0)
                {
                    try
                    {
                        var mapped = db.OrderDetail.Include("Order").Include("OrderDetailInvoices.Invoices").Include(i => i.OrderDetailIgredients)
                            .Include(i => i.OrderDetailIgredients.Select(ss => ss.OrderDetailIgredientVatAnal)).Include(i => i.OrderDetailInvoices.Select(s => s.PosInfoDetail.InvoiceTypes))
                            .Where(w => w.Guid != null ? orderDetailsGuid.Contains(w.Guid.Value) : false);
                        if (mapped.Count() > 0)
                        {
                            var delOrder = mapped.FirstOrDefault().Order;
                            var delOrderDetailInvoices = mapped.SelectMany(s => s.OrderDetailInvoices);
                            var delOrderDetailVatAnals = mapped.SelectMany(s => s.OrderDetailVatAnal);
                            var delOrderDetailIngredients = mapped.SelectMany(s => s.OrderDetailIgredients);
                            List<Invoices> delInvoices = new List<Invoices>();
                            foreach (var i in mapped)
                            {
                                foreach (var inv in i.OrderDetailInvoices)
                                {
                                    if (isPayOffFromTable == false)
                                    {
                                        delInvoices.Add(inv.Invoices);
                                        inv.Invoices.IsDeleted = true;
                                        inv.IsDeleted = true;
                                    }
                                    else
                                    {
                                        if (inv.PosInfoDetail.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || inv.PosInfoDetail.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio)
                                        {
                                            delInvoices.Add(inv.Invoices);
                                            inv.IsDeleted = true;
                                            inv.Invoices.IsDeleted = true;
                                        }
                                    }
                                }
                                if (isPayOffFromTable == false)
                                {
                                    i.IsDeleted = true;
                                }
                                else// An einai diagrafi apodeikshs apo exoflisi trapeziou
                                {
                                    i.PaidStatus = 0;
                                }
                            }

                            List<long> invoicesIds = (delInvoices.Select(s => s.Id).ToList());
                            var transactions = db.Transactions.Include(i => i.Accounts).Where(w => invoicesIds.Contains(w.InvoicesId.Value));
                            // if (mapped.All(a => a.OrderId == delOrder.Id))
                            // {
                            foreach (var tr in transactions)
                            {
                                tr.IsDeleted = true;
                            }
                            if (isPayOffFromTable == false)
                            {
                                foreach (var oinv in delOrderDetailVatAnals)
                                {
                                    oinv.IsDeleted = true;
                                }
                                foreach (var oinv in delOrderDetailIngredients)
                                {
                                    oinv.IsDeleted = true;
                                    foreach (var oigr in oinv.OrderDetailIgredientVatAnal)
                                    {
                                        oigr.IsDeleted = true;
                                    }
                                }
                                delOrder.IsDeleted = true;
                            }

                            db.SaveChanges();

                            // MAKE NEGATIVE TRANSACTION IF SENDS TRANSFER
                            if (transactions.Count() > 0)
                            {
                                foreach (var trans in transactions)
                                {
                                    if (trans.Accounts != null && trans.Accounts.SendsTransfer == true)
                                    {
                                        var hotel = db.HotelInfo.FirstOrDefault();
                                        long accountId = trans.AccountId.Value;
                                        List<TransferToPms> oldTransfers = new List<TransferToPms>();

                                        oldTransfers = db.TransferToPms.Where(w => w.TransactionId == trans.Id).ToList();

                                        List<TransferObject> objTosendList = new List<TransferObject>();
                                        foreach (var otpms in oldTransfers)
                                        {
                                            var account = db.Accounts.Find(accountId);
                                            if (account != null && account.SendsTransfer == true && hotel != null)
                                            {

                                                TransferToPms tpms = new TransferToPms(); // newCounter
                                                tpms.Description = otpms.Description + " ~ Deletion";
                                                tpms.PmsDepartmentId = otpms.PmsDepartmentId;
                                                tpms.PosInfoDetailId = otpms.PosInfoDetailId;
                                                tpms.ProfileId = otpms.ProfileId;
                                                tpms.ProfileName = otpms.ProfileName;
                                                tpms.ReceiptNo = otpms.ReceiptNo;
                                                tpms.RegNo = otpms.RegNo;
                                                tpms.RoomDescription = otpms.RoomDescription;
                                                tpms.RoomId = otpms.RoomId;
                                                tpms.SendToPMS = true;
                                                tpms.PmsDepartmentDescription = otpms.PmsDepartmentDescription;
                                                tpms.PosInfoId = otpms.PosInfoId;
                                                tpms.TransactionId = otpms.TransactionId;
                                                tpms.TransferType = 0;//Xrewstiko
                                                tpms.Total = (decimal)otpms.Total * -1;
                                                tpms.SendToPmsTS = DateTime.Now;
                                                var identifier = Guid.NewGuid();
                                                tpms.TransferIdentifier = identifier;
                                                db.TransferToPms.Add(tpms);

                                                TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, false, "", tpms, identifier);

                                                if (to.amount != 0)
                                                    objTosendList.Add(to);
                                            }

                                        }
                                        string storeid = HttpContext.Current.Request.Params["storeid"];
                                        foreach (var to in objTosendList)
                                        {
                                            SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                            //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                                        }
                                    }
                                }
                            }

                            db.SaveChanges();
                        }
                        else
                        {
                            return "NOT FOUND";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
            return "";
        }

        private static decimal DeVat(decimal perc, decimal tempnetbyvat)
        {
            return (decimal)(tempnetbyvat / (decimal)(1 + (decimal)(perc / 100)));
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }
        // get response from pms and update table tranferpms
        private void SendTransferCallback(IAsyncResult result)
        {
            try
            {
                // db = new PosEntities(false);
                SendTransfer del = (SendTransfer)result.AsyncState;

                TranferResultModel res = (TranferResultModel)del.EndInvoke(result);

                Guid storeid;

                if (Guid.TryParse(res.StoreId, out storeid))
                {

                    using (var ctx = new PosEntities(false, storeid))
                    {
                        var originalTransfer = ctx.TransferToPms.FirstOrDefault(f => f.TransferIdentifier == res.TransferObj.TransferIdentifier);

                        if (originalTransfer != null)
                        {
                            //originalTransfer.SendToPmsTS = DateTime.Now;
                            originalTransfer.ErrorMessage = res.TransferErrorMessage;
                            originalTransfer.PmsResponseId = res.TransferResponseId;

                        }

                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

        }

        private delegate TranferResultModel SendTransfer(TransferObject tpms, string storeid);
    }
}
