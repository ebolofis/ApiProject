using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class DeliveryOrdersController : ApiController
    {
       /// private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Object GetDeliveryOrdersStates(string storeid, string installFilters = "")
        {
            DeliveryFilters flts = new DeliveryFilters();
            if (installFilters != "")
            {
                flts = JsonConvert.DeserializeObject<DeliveryFilters>(installFilters);
            }
            if (flts.SelectedSalesTypes == null)
            {
                flts.SelectedSalesTypes = new List<long?>();
            }

            using (PosEntities db = new PosEntities(false))
            {
                //var flts = JsonConvert.DeserializeObject<DeliveryFilters>(installFilters);
                var query = (from q in db.Order.Include("OrderStatus").Include("Staff").Where(w => w.EndOfDayId == null)
                            select new
                            {
                                Id = q.Id,
                                Total = q.Total,
                                CurrentStatus = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().Status,
                                StatusChangedTS = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().TimeChanged,
                                Canceled = q.OrderStatus.Any(a => a.Status == 5),
                                ReceivedDate = q.Day,
                            }).ToList();

                var invs = from q in db.Invoices
                           join id in db.InvoiceShippingDetails.Where(w => w.ShippingAddress != null) on q.Id equals id.InvoicesId
                           join it in db.InvoiceTypes on q.InvoiceTypeId equals it.Id
                           where q.EndOfDayId == null && q.IsDeleted == null
                           select new
                           {
                               Id = id.InvoicesId,
                               InvoiceType = it.Type,
                               Total = q.Total,
                               IsVoided = q.IsVoided ?? false,
                           };

                //var query1 = (from q in db.OrderDetail
                var query1 = ((from q in db.OrderDetail.Where(od => (flts.SelectedSalesTypes.Count() > 0) ? flts.SelectedSalesTypes.Contains(od.SalesTypeId) : true)
                              join qq in query on q.OrderId equals qq.Id
                              join qqq in db.OrderDetailInvoices on q.Id equals qqq.OrderDetailId
                              join i in invs on qqq.InvoicesId equals i.Id
                              where (q.PaidStatus == 0 && i.InvoiceType == 2) || ((q.PaidStatus == 1 || q.PaidStatus == 2) && i.InvoiceType != 2)
                              select new
                              {
                                  InvoiceId = i.Id,
                                  OrderId = q.OrderId,
                                  Total = i.Total,
                                  IsVoided = i.IsVoided || qq.Canceled || q.Status == 5,
                                  CurrentStatus = qq.CurrentStatus == 0 && !(i.IsVoided || qq.Canceled || q.Status == 5) ? OrderStatusEnum.Received :
                                                    (qq.CurrentStatus == 1 || qq.CurrentStatus == 2) && !(i.IsVoided || qq.Canceled || q.Status == 5) ? OrderStatusEnum.Pending :
                                                    qq.CurrentStatus == 3 && !(i.IsVoided || qq.Canceled || q.Status == 5) ? OrderStatusEnum.Ready :
                                                    qq.CurrentStatus == 4 && !(i.IsVoided || qq.Canceled || q.Status == 5) ? OrderStatusEnum.Onroad :
                                                    qq.CurrentStatus == 5 ? OrderStatusEnum.Canceled : OrderStatusEnum.Complete,
                                  StatusChangedTS = qq.StatusChangedTS,
                                  ReceivedDate = qq.ReceivedDate,
                              }).Distinct()).ToList();
                var totals = query1.GroupBy(g => g.CurrentStatus).Select(s => new
                {
                    Status = s.Key,
                    OrdersCount = s.Count()
                });

                return totals;

            }
           
        }

        public Object GetDeliveryOrders(string storeid, int status, int pageno, int pagesize, string installationFilters = "")
        {
            DeliveryFilters flts = new DeliveryFilters();
            if (installationFilters != "")
            {
                flts = JsonConvert.DeserializeObject<DeliveryFilters>(installationFilters);
            }
            if (flts.SelectedSalesTypes == null)
            {
                flts.SelectedSalesTypes = new List<long?>();
            }
            using (PosEntities db = new PosEntities(false))
            {
                var query = from q in db.Order.Include("OrderStatus").Include("Staff").Where(w => w.EndOfDayId == null)
                            select new
                            {
                                Id = q.Id,
                                OrderNo = q.OrderNo,
                                Total = q.Total,
                                CurrentStatus = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().Status,
                                StatusChangedTS = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().TimeChanged,
                                OrderStatusId = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().Id,
                                Canceled = q.OrderStatus.Any(a => a.Status == 5),
                                ReceivedDate = q.Day,
                                StaffId = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().StaffId
                            };

                var invs = from q in db.Invoices
                           join id in db.InvoiceShippingDetails.Where(w => w.ShippingAddress != null) on q.Id equals id.InvoicesId
                           join it in db.InvoiceTypes on q.InvoiceTypeId equals it.Id
                           join st in db.Staff on q.StaffId equals st.Id
                           where q.EndOfDayId == null && q.IsDeleted == null
                           select new
                           {
                               Id = id.InvoicesId,
                               InvoiceAbbr = it.Abbreviation,
                               InvoiceCounter = q.Counter,
                               InvoiceType = it.Type,
                               CustomerName = id.CustomerName,
                               CustomerID = id.CustomerID,

                               Address = id.ShippingAddress,
                               City = id.ShippingCity,
                               Floor = id.Floor,
                               Phone = id.Phone,
                               Longtitude = id.Longtitude,
                               Latitude = id.Latitude,
                               Total = q.Total,
                               Discount = q.Discount,
                               StaffId = q.StaffId,
                               StaffName = st.LastName,
                               IsVoided = q.IsVoided ?? false
                           };

                var query1 = ((from q in db.OrderDetail.Where(od => (flts.SelectedSalesTypes.Count() > 0) ? flts.SelectedSalesTypes.Contains(od.SalesTypeId) : true)
                              join qq in query on q.OrderId equals qq.Id
                              join qqq in db.OrderDetailInvoices on q.Id equals qqq.OrderDetailId
                              join q7 in db.Staff on qq.StaffId equals q7.Id
                              join i in invs on qqq.InvoicesId equals i.Id
                              where ((q.PaidStatus == 0 && i.InvoiceType == 2) || ((q.PaidStatus == 1 || q.PaidStatus == 2) && i.InvoiceType != 2))
                              select new
                              {
                                  InvoiceId = i.Id,
                                  OrderId = q.OrderId,
                                  OrderNo = qq.OrderNo,
                                  InvoiceAbbr = i.InvoiceAbbr,
                                  InvoiceCounter = i.InvoiceCounter,
                                  InvoiceType = i.InvoiceType,
                                  IsPaid = q.PaidStatus,
                                  IsVoided = i.IsVoided || qq.Canceled || q.Status == 5,
                                  CustomerName = i.CustomerName,
                                  CustomerID = i.CustomerID,
                                  Address = i.Address,
                                  Floor = i.Floor,
                                  City = i.City,
                                  Phone = i.Phone,
                                  Total = i.Total,
                                  CurrentStatus = qq.CurrentStatus,
                                  StatusChangedTS = qq.StatusChangedTS,
                                  OrderStatusId = qq.OrderStatusId,
                                  ReceivedDate = qq.ReceivedDate,
                                  StaffId = i.StaffId,
                                  StaffName = i.StaffName,
                                  StatusStaffId = q7.Id,
                                  StatusStaffName = q7.LastName,
                                  Longtitude = i.Longtitude,
                                  Latitude = i.Latitude,
                                  PricelistId = qqq.PricelistId,
                                  DeliveryState = qq.CurrentStatus == 0 ? OrderStatusEnum.Received :
                                                    qq.CurrentStatus == 1 ? OrderStatusEnum.Pending :
                                                    qq.CurrentStatus == 3 ? OrderStatusEnum.Ready :
                                                    qq.CurrentStatus == 4 ? OrderStatusEnum.Onroad :
                                                    qq.CurrentStatus == 5 ? OrderStatusEnum.Canceled : OrderStatusEnum.Complete
                              }).Distinct());
                //query1.Dump();
                switch ((OrderStatusEnum)status)
                {
                    case OrderStatusEnum.Received:
                        query1 = query1.Where(w => w.CurrentStatus == 0 && w.IsVoided == false);
                        break;
                    case OrderStatusEnum.Pending:
                        query1 = query1.Where(w => (w.CurrentStatus == 1 || w.CurrentStatus == 2) && w.IsVoided == false);
                        break;
                    case OrderStatusEnum.Ready:
                        query1 = query1.Where(w => w.CurrentStatus == 3 && w.IsVoided == false);
                        break;
                    case OrderStatusEnum.Onroad:
                        query1 = query1.Where(w => w.CurrentStatus == 4 && w.IsVoided == false);
                        break;
                    case OrderStatusEnum.Canceled:
                        query1 = query1.Where(w => w.CurrentStatus == 5);
                        break;
                    case OrderStatusEnum.Complete:
                        query1 = query1.Where(w => w.CurrentStatus == 6 && w.IsVoided == false);
                        break;
                    default:
                        query1 = query1.Where(w => (w.CurrentStatus != 6 && w.CurrentStatus != 5) && w.IsVoided == false);
                        break;
                }

                var pages = query1.Count() / pagesize;
                if (pages == 0)
                    pageno = 0;
                var totals = query1.GroupBy(g => g.CurrentStatus).Select(s => new
                {
                    Status = s.Key,
                    OrdersCount = s.Count()
                });

                var finalobj = new
                {
                    TotalsPerStatus = totals.ToList(),
                    OrderByStatus = query1.OrderByDescending(o => o.ReceivedDate).Skip(pageno * pagesize).Take(pagesize).ToList(),
                    TotalPages = (int)pages + 1,
                    PageSize = pagesize,
                };


                return finalobj;

            }

           
        }

        public Object GetFilteredDeliveryOrders(string storeid, string deliveryFilters, int pageno, int pagesize)
        {
            DeliveryFilters flts = JsonConvert.DeserializeObject<DeliveryFilters>(deliveryFilters);
            using (PosEntities db = new PosEntities(false))
            {
                var query = from q in db.Order.Include("OrderStatus").Include("Staff").Where(w => w.EndOfDayId == null)
                            select new
                            {
                                Id = q.Id,
                                OrderNo = q.OrderNo,
                                Total = q.Total,
                                CurrentStatus = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().Status,
                                StatusChangedTS = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().TimeChanged,
                                OrderStatusId = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().Id,
                                Canceled = q.OrderStatus.Any(a => a.Status == 5),
                                ReceivedDate = q.Day,
                                StaffId = q.OrderStatus.OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().StaffId
                            };

                var invs = from q in db.Invoices
                           join id in db.InvoiceShippingDetails.Where(w => w.ShippingAddressId != null) on q.Id equals id.InvoicesId
                           join it in db.InvoiceTypes on q.InvoiceTypeId equals it.Id
                           join st in db.Staff on q.StaffId equals st.Id
                           where q.EndOfDayId == null
                           select new
                           {
                               Id = id.InvoicesId,
                               InvoiceAbbr = it.Abbreviation,
                               InvoiceCounter = q.Counter,
                               InvoiceType = it.Type,
                               CustomerName = id.CustomerName,
                               CustomerID = id.CustomerID,
                               AddressId = id.ShippingAddressId,
                               Address = id.ShippingAddress,
                               City = id.ShippingCity,
                               Floor = id.Floor,
                               Longtitude = id.Longtitude,
                               Latitude = id.Latitude,
                               Total = q.Total,
                               Discount = q.Discount,
                               StaffId = q.StaffId,
                               StaffName = st.LastName,
                               IsVoided = q.IsVoided ?? false
                           };

                if (flts != null)
                {
                    if (!String.IsNullOrEmpty(flts.Address))
                        invs = invs.Where(w => w.Address.Contains(flts.Address));
                    if (!String.IsNullOrEmpty(flts.CustomerName))
                        invs = invs.Where(w => w.CustomerName.Contains(flts.CustomerName));
                    if (flts.OrderId != null)
                        invs = invs.Where(w => w.InvoiceCounter == flts.OrderId);
                }

                var query1 = (from q in db.OrderDetail
                              join qq in query on q.OrderId equals qq.Id
                              join qqq in db.OrderDetailInvoices on q.Id equals qqq.OrderDetailId
                              join q7 in db.Staff on qq.StaffId equals q7.Id
                              join i in invs on qqq.InvoicesId equals i.Id
                              where (q.PaidStatus == 0 && i.InvoiceType == 2) || ((q.PaidStatus == 1 || q.PaidStatus == 2) && i.InvoiceType != 2)
                              select new
                              {
                                  InvoiceId = i.Id,
                                  OrderId = q.OrderId,
                                  OrderNo = qq.OrderNo,
                                  InvoiceAbbr = i.InvoiceAbbr,
                                  InvoiceCounter = i.InvoiceCounter,
                                  InvoiceType = i.InvoiceType,
                                  IsPaid = q.PaidStatus,
                                  IsVoided = i.IsVoided || qq.Canceled || q.Status == 5,
                                  CustomerName = i.CustomerName,
                                  CustomerID = i.CustomerID,
                                  AddressId = i.AddressId,
                                  Address = i.Address,
                                  Floor = i.Floor,
                                  City = i.City,
                                  Total = i.Total,
                                  CurrentStatus = qq.CurrentStatus,
                                  StatusChangedTS = qq.StatusChangedTS,
                                  OrderStatusId = qq.OrderStatusId,
                                  ReceivedDate = qq.ReceivedDate,
                                  StaffId = i.StaffId,
                                  StaffName = i.StaffName,
                                  StatusStaffId = q7.Id,
                                  StatusStaffName = q7.LastName,
                                  Longtitude = i.Longtitude,
                                  Latitude = i.Latitude,
                                  DeliveryState = qq.CurrentStatus == 0 ? OrderStatusEnum.Received :
                                                    qq.CurrentStatus == 1 ? OrderStatusEnum.Pending :
                                                    qq.CurrentStatus == 3 ? OrderStatusEnum.Ready :
                                                    qq.CurrentStatus == 4 ? OrderStatusEnum.Onroad :
                                                    qq.CurrentStatus == 5 ? OrderStatusEnum.Canceled : OrderStatusEnum.Complete
                              }).Distinct();
                if (flts != null)
                {

                    if (flts.OrderNo != null)
                        query1 = query1.Where(w => w.OrderNo == flts.OrderNo);
                }
                var pages = query1.Count() / pagesize;
                if (pages == 0)
                    pageno = 0;

                var finalobj = new
                {
                    //TotalsPerStatus = query1.Count(),
                    OrderByStatus = query1.OrderByDescending(o => o.InvoiceId).Skip(pageno * pagesize).Take(pagesize).ToList(),
                    TotalPages = (int)pages + 1,
                    PageSize = pagesize
                };


                return finalobj;

            }
          

        }

        public Object GetDeliveryCustomerFromGuest(string storeid, string phone, bool useguest)
        {
            using (PosEntities db = new PosEntities(false))
            {
                var res = db.Guest.Where(w => w.Telephone == phone).OrderBy(o => o.Id).FirstOrDefault();
                if (res == null)
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

                return res;
            }

        }

        //public Object 
        [HttpPost]
        public Object GetOrdersByGuestId(string storeid, Customers customer)
        {
            if (customer == null) return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };
            using (PosEntities db = new PosEntities(false))
            {
                var gid = db.Guest.Where(w => w.ProfileNo == customer.ProfileNo).OrderBy(o => o.Id).FirstOrDefault();
                var guestid = (long)0;

                if (gid != null)
                {
                    guestid = gid.Id;
                    gid.ProfileNo = customer.ProfileNo;
                    gid.FirstName = customer.FirstName;
                    gid.LastName = customer.LastName;
                    gid.Address = customer.Address;
                    gid.City = customer.City;
                    gid.PostalCode = customer.PostalCode;
                    gid.Country = customer.Country;
                    gid.Email = customer.Country;
                    gid.Telephone = customer.Telephone;
                    if (customer.VIP != null)
                        gid.VIP = customer.VIP;
                    db.Entry(gid).State = EntityState.Modified;

                }
                else
                {
                    Guest g = new Guest();
                    g.ProfileNo = customer.ProfileNo;
                    g.FirstName = customer.FirstName;
                    g.LastName = customer.LastName;
                    g.Address = customer.Address;
                    g.City = customer.City;
                    g.PostalCode = customer.PostalCode;
                    g.Country = customer.Country;
                    g.Email = customer.Country;
                    g.Telephone = customer.Telephone;
                    g.Note1 = "";
                    if (customer.VIP != null)
                        g.VIP = customer.VIP;
                    db.Guest.Add(g);
                }
                db.SaveChanges();
                gid = db.Guest.Where(w => w.ProfileNo == customer.ProfileNo).OrderBy(o => o.Id).FirstOrDefault();

                var defaultPriceListId = db.AllowedMealsPerBoard.Where(w => w.BoardId == gid.VIP).FirstOrDefault();
                var validinvs = db.InvoiceTypes.Where(w => w.Type != 2 && w.Type != 3).Select(s => s.Id);
                var query = db.Invoices.Include("InvoiceShippingDetails").Where(w => w.GuestId == guestid
                                                && validinvs.Contains(w.InvoiceTypeId.Value)
                                                && (w.IsVoided == null || w.IsVoided == false)
                                                && (w.IsDeleted == null || w.IsDeleted == false))
                .Select(q => new
                {
                    InvoiceId = q.Id,
                    GuestId = q.GuestId,
                    OrderDate = q.Day,
                    Total = q.Total,
                });

                var final = new
                {
                    GuestId = gid.Id,
                    LatestOrders = query.OrderByDescending(o => o.OrderDate).Take(5).ToList(),
                    LatestOrdetGross = query.OrderByDescending(o => o.OrderDate).Take(5).Sum(sm => sm.Total) ?? 0,
                    Gross = query.Sum(sm => sm.Total) ?? 0,
                    DefaultPriceListId = defaultPriceListId != null ? defaultPriceListId.PriceListId : -1,
                    StoreRemark = gid.Note1,
                    NextOrderRemark = gid.Note2
                };
                return final;

            }

           
        }


        // PUT api/Product/5
        public HttpResponseMessage PutCustomer(long id, string storeid, Guest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            using (PosEntities db = new PosEntities(false))
            {
                var guest = db.Guest.Find(id);
                if (guest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                //note 1 on delivery holds store message
                guest.Note1 = model.Note1;
                //note 1 on delivery holds next delivery comment
                guest.Note2 = model.Note2;
                //   db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
                return Request.CreateResponse(HttpStatusCode.OK);

            }
         
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
          //  db.Dispose();

            base.Dispose(disposing);
        }


    }


}