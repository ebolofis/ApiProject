using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Pos_WebApi.Helpers;
using System.Data.Entity.Core.Objects;
using Pos_WebApi.Models.FilterModels;
using Symposium.Models.Enums;

namespace Pos_WebApi.Controllers
{
    public class StatisticsController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        private StatisticsRepository statRepo;

        public StatisticsController()
        {
            statRepo = new StatisticsRepository(db);
        }

        private IEnumerable<dynamic> GetFlatSales(string filters, Int32 eodId = 0, bool isInvoice = true)
        {
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long?> staffList = new List<long?>();
            if (String.IsNullOrEmpty(filters))
                eod.Add(eodId);
            else
            {
                var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

                if (flts.UseEod)
                {
                    if (flts.EodId == null)
                    {
                        if (flts.UsePeriod)
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                                                                EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                        }
                        else
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                        }
                    }
                    else
                    {
                        eod.Add((long)flts.EodId);
                    }
                }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
                staffList.AddRange(flts.StaffList.ToList());
            }
            db.Configuration.LazyLoadingEnabled = true;
            var salesFlat = (from qq in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false)
                                 //  .Include("Order")
                                 //  .Include("PricelistDetail")
                                 //  .Include("PricelistDetail.Pricelist")
                                 //  .Include("SalesType")
                                 //  .Include("Order.Staff")
                                 //  .Include("Guest")
                                 //  .Include("Table")
                                 //  .Include("PosInfoDetail.PosInfo.Department")
                                 //  .Include("Transactions.Accounts")
                             join s in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on qq.Id equals s.OrderDetailId
                             where eod.Contains((s.OrderDetail.Order.EndOfDayId ?? 0))
                                   && posList.Contains(s.PosInfoDetail.PosInfoId)
                                   && staffList.Contains(s.StaffId)
                                   && s.PosInfoDetail.IsInvoice == isInvoice
                             select new
                             {
                                 FODay = qq.Order.EndOfDay != null ? qq.Order.EndOfDay.FODay : null,
                                 CloseId = qq.Order.EndOfDay != null ? qq.Order.EndOfDay.CloseId : null,
                                 PosInfoCode = s.PosInfoDetail.PosInfo.Code,
                                 DepartmentId = s.PosInfoDetail.PosInfo.DepartmentId,
                                 PosInfoId = s.PosInfoDetail.PosInfoId,
                                 PosInfoDetailId = s.PosInfoDetailId,
                                 OrderDetailId = s.OrderDetailId,
                                 StaffId = s.StaffId,
                                 GroupId = s.PosInfoDetail.GroupId,
                                 Counter = s.Counter,
                                 Table = qq.Table != null ? qq.Table.Code : "",
                                 Room = qq.Guest != null ? qq.Guest.Room : "",
                                 Abbreviation = s.PosInfoDetail.Abbreviation,
                                 IsInvoice = s.PosInfoDetail.IsInvoice,
                                 IsPrinted = s.IsPrinted,
                                 IsCancel = s.PosInfoDetail.IsCancel,
                                 Void = qq.Status == 5,
                                 Cover = qq.Couver,
                                 Day = s.CreationTS,
                                 CategoryId = qq.Product.ProductCategories != null ? qq.Product.ProductCategories.CategoryId : null,
                                 CategoryDescription = qq.Product.ProductCategories.Categories != null ? qq.Product.ProductCategories.Categories.Description : "",
                                 ProductId = s.OrderDetail.ProductId,
                                 ProductDescription = qq.Product.SalesDescription,
                                 Qty = qq.Qty,
                                 Price = qq.Price,
                                 Vats = qq.OrderDetailVatAnal,
                                 //Total = s.OrderDetail.Total,
                                 TotalAfterDiscount = qq.TotalAfterDiscount,// + qq.OrderDetailIgredients.Sum(sm=> sm.TotalAfterDiscount ?? 0),
                                 Discount = qq.Discount,
                                 TransactionId = qq.TransactionId,
                                 IsPaid = qq.PaidStatus,
                                 TransactionAmount = qq.Transactions != null ? qq.Transactions.Gross : 0,
                                 AccountId = qq.Transactions != null ? qq.Transactions.AccountId : 0,
                                 AccountDescription = qq.Transactions != null ? qq.Transactions.Accounts != null ? qq.Transactions.Accounts.Description : "" : "",
                                 AccountType = qq.Transactions != null ? qq.Transactions.Accounts != null ? qq.Transactions.Accounts.Type : 0 : 0,
                                 Guest = qq.Guest != null ? qq.Guest.LastName : "",// + ' ' + qq.Guest.FirstName : "",
                                 SalesTypeId = qq.SalesTypeId,
                                 SalesTypeDescription = qq.SalesType != null ? qq.SalesType.Description : "",
                                 DepartmentDescription = s.PosInfoDetail.PosInfo.Department != null ? s.PosInfoDetail.PosInfo.Department.Description : "",
                                 PosInfoDescription = s.PosInfoDetail.PosInfo.Description,
                                 StaffName = s.Staff.LastName, //+ " " + s.Staff.FirstName
                                 VatId = (Int64?)qq.OrderDetailVatAnal.FirstOrDefault().VatId,
                                 VatRate = qq.OrderDetailVatAnal.FirstOrDefault().VatRate,
                                 VatAmount = qq.OrderDetailVatAnal.FirstOrDefault().VatAmount,
                                 VatGross = qq.OrderDetailVatAnal.FirstOrDefault().Gross,
                                 VatNet = qq.OrderDetailVatAnal.FirstOrDefault().Net,
                                 Status = qq.Status,
                                 PriceListId = qq.PricelistDetail.PricelistId,
                                 PriceListDescription = qq.PricelistDetail.Pricelist.Description,
                                 PriceLictPrice = qq.PricelistDetail.Price,



                             });//.ToList();
            db.Configuration.LazyLoadingEnabled = false;


            //	  a= s.GroupBy(g=>new {g.PosInfoId, g.InvTypeGroup, g.Inv_Counter})

            return salesFlat;
        }

        private Object GetSalesForReporting(string filters, Int32 eodId = 0, bool isInvoice = true)
        {
            // db.Configuration.LazyLoadingEnabled = false;
            //db.Database.CommandTimeout = 180;
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long?> staffList = new List<long?>();
            List<long?> categoriesList = new List<long?>();
            List<long?> productCategoriesList = new List<long?>();
            List<long?> departmentList = new List<long?>();
            List<long?> invoicesList = new List<long?>();
            // bool useSpecificProducts = false;
            int repno = 0;
            if (String.IsNullOrEmpty(filters))
                eod.Add(eodId);
            else
            {
                var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

                if (flts.UseEod)
                    if (flts.UsePeriod)
                    {
                        eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                                                            EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                    }
                    else
                    {
                        eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                    }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
                staffList.AddRange(flts.StaffList.ToList());
                categoriesList.AddRange(flts.CategoriesList.ToList());
                //productCategoriesList.AddRange(flts.ProductCategoriesList.ToList());
                departmentList.AddRange(flts.DepartmentList.ToList());
                invoicesList.AddRange(flts.InvoiceList.ToList());

                repno = flts.ReportType;
            }

            var flatProducts = db.Product.Select(s => new
            {
                Id = s.Id,
                Description = s.SalesDescription,
                Code = s.Code,
                ProductCategoryId = s.ProductCategoryId,
                ProductCategory = s.ProductCategories.Description,
                CategoryId = s.ProductCategories.CategoryId,
                Category = s.ProductCategories.CategoryId != null ? s.ProductCategories.Categories.Description : "Without Category"
            }).Where(w => categoriesList.Contains(w.CategoryId) || w.CategoryId == null /*&& productCategoriesList.Contains(w.ProductCategoryId)*/);


            var orders = (from q in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false)
                          join pid in db.PosInfoDetail.Where(w => w.IsInvoice == true && posList.Contains(w.PosInfoId) && invoicesList.Contains(w.GroupId)) on q.PosInfoDetailId equals pid.Id
                          join od in db.OrderDetail.Where(w => w.Status != 5 && (w.IsDeleted ?? false) == false) on q.OrderDetailId equals od.Id
                          join o in db.Order.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && (w.IsDeleted ?? false) == false) on od.OrderId equals o.Id
                          join pd in db.PricelistDetail on od.PriceListDetailId equals pd.Id
                          join po in db.PosInfo.Where(w => departmentList.Contains(w.DepartmentId)) on pid.PosInfoId equals po.Id
                          where staffList.Contains(q.StaffId)
                          //              eod.Contains((s.OrderDetail.Order.EndOfDayId ?? 0))
                          //                         && posList.Contains(s.PosInfoDetail.PosInfoId)
                          //                         && staffList.Contains(s.StaffId)
                          //                         && s.PosInfoDetail.IsInvoice == isInvoice

                          select new
                          {
                              Day = q.CreationTS,
                              OrderDetailId = od.Id,
                              PosInfoDetailId = q.PosInfoDetailId,
                              Abbreviation = pid.Abbreviation,
                              Counter = q.Counter,
                              Total = od.TotalAfterDiscount,
                              ProductId = od.ProductId,
                              PriceListId = pd.PricelistId,
                              PriceListPrice = pd.Price,
                              StaffId = q.StaffId,
                              SalesTypeId = od.SalesTypeId,
                              Qty = od.Qty,
                              GroupId = pid.GroupId,
                              Covers = od.Couver,
                              TableId = od.TableId,
                              ProductPrice = od.Price,
                              DepartmentId = po.DepartmentId,
                              PosInfoId = pid.PosInfoId

                          }).Distinct().ToList();

            var distOrders = orders.Select(os => new
            {
                PosInfoDetailId = os.PosInfoDetailId,
                Counter = os.Counter,
                OrderDetailId = os.OrderDetailId,
                DepartmentId = os.DepartmentId,
                //  DepartmentDescription = os.DepartmentDescription,
                ProductId = os.ProductId,
                Total = os.Total
            }).Distinct();
            //	distOrders.Distinct().OrderBy(o=>o.DepartmentId).ThenBy(t=>t.ProductId).Dump();
            var flatByDepartment = from q in distOrders
                                   group q by new { q.DepartmentId, q.ProductId } into g
                                   select new
                                   {
                                       Department = g.FirstOrDefault().DepartmentId,
                                       ProductId = g.FirstOrDefault().ProductId,
                                       // PriceList = g.FirstOrDefault().PriceListId,
                                       Total = g.Distinct().Sum(sm => sm.Total),
                                       // PriceListPrice = g.Average(a => a.PriceListPrice),
                                   };
            var totalsByDepartment = (from q in flatByDepartment
                                      join fp in flatProducts on q.ProductId equals fp.Id
                                      join d in db.Department on q.Department equals d.Id
                                      select new
                                      {
                                          Department = d.Description,
                                          ProductId = q.ProductId,
                                          Category = fp.Category,
                                          ProductCategory = fp.ProductCategory,
                                          //   PriceList = q.PriceList,
                                          Total = q.Total,
                                          //   PriceListPrice = q.PriceListPrice
                                      }).GroupBy(gg => gg.Department).Select(s => new
                                      {
                                          Department = s.FirstOrDefault().Department,
                                          Total = s.Sum(sm => sm.Total),
                                          Categories = s.GroupBy(ggg => ggg.Category).Select(ss => new
                                          {
                                              Category = ss.FirstOrDefault().Category,
                                              Total = ss.Sum(sm => sm.Total),
                                              //										ProductCategories = ss.GroupBy(gggg=>gggg.ProductCategory).Select(sss=>new {
                                              //											ProductCategory = sss.FirstOrDefault().ProductCategory,
                                              //											Total = sss.Sum(sm=>sm.Total)
                                              //										})
                                          })
                                      });

            switch (repno)
            {
                #region case 0
                case 0:
                    var flatByProduct1 = orders.GroupBy(g => new { g.ProductId, g.Abbreviation, g.Counter, g.PosInfoDetailId, g.SalesTypeId, g.PriceListId, })

                         .Select(s => new
                         {
                             PriceListId = s.Key.PriceListId,
                             ProductId = s.Key.ProductId,
                             ProductPrice = s.Max(m => m.PriceListPrice),
                             Qty = s.Sum(ss => (Double)ss.Qty),
                             Gross = s.Sum(ss => (Decimal)ss.Total),
                             ReceiptCount = s.GroupBy(g => new { g.PosInfoDetailId, g.GroupId, g.Counter }).Count(),
                         }).ToList();

                    var usedPriceLists = (from q in flatByProduct1
                                          join p in db.Pricelist on q.PriceListId equals p.Id
                                          select new
                                          {
                                              Id = q.PriceListId,
                                              Description = p.Description
                                          }).Distinct().OrderBy(o => o.Id).ToList();

                    var groupedByProduct = from q in flatByProduct1
                                           group q by q.ProductId into s// 	.GroupBy(g => g.ProductId)
                                           join fp in flatProducts on s.FirstOrDefault().ProductId equals fp.Id
                                           select new
                                           {
                                               ProductId = s.Key,
                                               ProductDescription = fp.Description,
                                               PriceList = (from q in usedPriceLists
                                                            join qq in s.GroupBy(g1 => g1.PriceListId) on q.Id equals qq.Key into ff
                                                            from s1 in ff.DefaultIfEmpty()

                                                            select new
                                                            {
                                                                PriceListId = q.Id,//s1.Key,
                                                                PriceList = q.Description,//s1.FirstOrDefault().PriceList,
                                                                ProductPrice = s.Max(m => m.ProductPrice),
                                                                Qty = s1 != null ? s1.Sum(ss => ss.Qty) : 0,
                                                                Gross = s1 != null ? s1.Sum(ss => ss.Gross) : 0
                                                            }).OrderBy(o => o.PriceListId),
                                               Sr = Decimal.Round((decimal)(s.Sum(s1 => s1.Qty) / s.FirstOrDefault().ReceiptCount * 100), 2)

                                               //).OrderBy(o => o.ProductDescription);
                                           };

                    var totalsPerPriceList = flatByProduct1.GroupBy(g => g.PriceListId).Select(s => new
                    {
                        PriceListId = s.Key,
                        PriceList = s.FirstOrDefault().PriceListId,
                        ProductPrice = s.Max(m => m.ProductPrice),
                        Qty = s.Sum(ss => ss.Qty),
                        Gross = s.Sum(ss => ss.Gross)
                    }).OrderBy(o => o.PriceList);

                    //var totals = new
                    //{
                    //    ItemsSold = flatByProduct1.Sum(s => s.Qty),
                    //    TotalReceipts = flat.GroupBy(g => new { g.PosInfoDetailId, g.GroupId, g.Counter }).Count(),
                    //    TotalSales = flatByProduct.Sum(s => s.Gross),
                    //    TotalPerSalesType = flatByProduct.GroupBy(s1 => s1.SalesTypeId).Select(s => new
                    //    {
                    //        SalesTypeId = s.Key,
                    //        Total = s.Sum(ss => ss.Gross)
                    //    }),
                    //    TotalPerPos = flat.GroupBy(g => g.PosInfoId).Select(s => new
                    //    {
                    //        PosId = s.Key,
                    //        PosDescription = s.FirstOrDefault().PosInfoDescription,
                    //        Total = 0//s.Sum(ss => (double)ss.Gross)
                    //    })

                    //};

                    Object summaryResults = new { usedPriceLists, totalsPerPriceList, totalsByDepartment };
                    return new { processedResults = groupedByProduct, summaryResults };
                //      return new { processedResults = groupedByProduct };
                #endregion
                #region Case 1
                case 1:
                    var flatbyProduct = orders.GroupBy(g => new { g.ProductId, g.Abbreviation, g.Counter, g.PosInfoDetailId })
                           .Select(s => new
                           {
                               ProductId = s.FirstOrDefault().ProductId,
                               Total = s.Sum(sm => sm.Total),
                               StaffId = s.FirstOrDefault().StaffId,
                               Covers = s.FirstOrDefault().Covers,
                               Qty = s.Sum(sm => sm.Qty),
                               Receipts = s.Count(),
                               ReceiptTotal = s.Sum(sm => sm.Total)
                           });
                    var groupedbyProduct = flatbyProduct.GroupBy(g => g.ProductId)
                                                .Select(s => new
                                                {
                                                    ProductId = s.FirstOrDefault().ProductId,
                                                    Total = s.Sum(sm => sm.Total),
                                                    StaffId = s.FirstOrDefault().StaffId,
                                                    Covers = s.FirstOrDefault().Covers,
                                                    ReceiptAverage = s.Average(a => a.Total),
                                                    Receipts = s.FirstOrDefault().Receipts,
                                                    Qty = s.Sum(sm => sm.Qty)
                                                }).ToList();



                    var processedResults = (from q in groupedbyProduct
                                            join s in db.Staff on q.StaffId equals s.Id
                                            join fp in flatProducts on q.ProductId equals fp.Id
                                            select new
                                            {
                                                StaffName = s.LastName,
                                                ProductDescription = fp.Description,
                                                ProductId = fp.Id,
                                                Receipts = q.Receipts,
                                                Qty = q.Qty,
                                                ReceiptAverage = q.ReceiptAverage,
                                                Gross = q.Total,
                                                ProductCategory = fp.ProductCategory

                                            }).GroupBy(g => g.StaffName).Select(ss => new
                                            {
                                                Staff = ss.FirstOrDefault().StaffName,
                                                Receipts = ss.Sum(sm => sm.Receipts),
                                                Qty = ss.Sum(sm => (Double)sm.Qty),
                                                ReceiptAverage = 0,//Decimal.Round(ss.ReceiptAverage(a => a.Total), 2),
                                                Gross = Decimal.Round(ss.Sum(sm => (Decimal)sm.Gross), 2),
                                                Products = ss.GroupBy(gg => gg.ProductId)
                                                                .Select(sss => new
                                                                {
                                                                    Product = sss.FirstOrDefault().ProductDescription,
                                                                    Receipts = sss.Count(),
                                                                    Qty = sss.Sum(sm => (Double)sm.Qty),
                                                                    ReceiptAverage = 0,//Decimal.Round(sss.Average(a => a.ReceiptTotal), 2),
                                                                    Gross = Decimal.Round(sss.Sum(sm => (Decimal)sm.Gross), 2)
                                                                })
                                            });
                    var totals = new
                    {
                        Receipts = processedResults.Sum(s => s.Receipts),
                        Qty = processedResults.Sum(s => s.Qty),
                        TotalSales = processedResults.Sum(s => s.Gross)


                    };

                    var totalsPerWaiter = processedResults.GroupBy(g => g.Staff).Select(s => new { Staff = s.Key, Total = s.Sum(sm => sm.Gross) });
                    summaryResults = new { totals, totalsPerWaiter, totalsByDepartment };

                    return new { processedResults, summaryResults };
                #endregion
                #region case 2
                case 2:
                    var flatbyProduct1 = orders.GroupBy(g => new { g.ProductId, g.Abbreviation, g.Counter, g.PosInfoDetailId })
                           .Select(s => new
                           {
                               ProductId = s.FirstOrDefault().ProductId,
                               Total = s.Sum(sm => sm.Total),
                               StaffId = s.FirstOrDefault().StaffId,
                               Covers = s.FirstOrDefault().Covers,
                               Qty = s.Sum(sm => sm.Qty),
                               Receipts = s.Count(),
                               ReceiptTotal = s.Sum(sm => sm.Total)
                           });
                    var groupedbyProduct1 = flatbyProduct1.GroupBy(g => g.ProductId)
                                                        .Select(s => new
                                                        {
                                                            ProductId = s.FirstOrDefault().ProductId,
                                                            Total = s.Sum(sm => sm.Total),
                                                            StaffId = s.FirstOrDefault().StaffId,
                                                            Covers = s.FirstOrDefault().Covers,
                                                            ReceiptAverage = s.Average(a => a.Total),
                                                            Receipts = s.FirstOrDefault().Receipts,
                                                            Qty = s.Sum(sm => sm.Qty)
                                                        }).ToList();

                    var groupedbyProducts = (from q in groupedbyProduct1
                                             join s in db.Staff on q.StaffId equals s.Id
                                             join fp in flatProducts on q.ProductId equals fp.Id
                                             select new
                                             {
                                                 StaffName = s.LastName,
                                                 ProductDescription = fp.Description,
                                                 ProductId = fp.Id,
                                                 Receipts = q.Receipts,
                                                 Qty = q.Qty,
                                                 ReceiptAverage = q.ReceiptAverage,
                                                 Gross = q.Total,

                                                 ProductCategory = fp.ProductCategory

                                             }).GroupBy(g => g.ProductId)
                           .Select(s => new
                           {
                               Staff = s.FirstOrDefault().ProductDescription,
                               Receipts = s.Sum(sm => sm.Receipts),
                               Qty = s.Sum(sm => (Double)sm.Qty),
                               Persons = 0,//s.Sum(m => m.Covers),
                               ReceiptAverage = 0,
                               Gross = Decimal.Round(s.Sum(sm => (Decimal)sm.Gross), 2)

                           });
                    totals = new
                    {
                        Receipts = groupedbyProducts.Sum(s => s.Receipts),
                        Qty = groupedbyProducts.Sum(s => s.Qty),
                        TotalSales = groupedbyProducts.Sum(s => s.Gross)


                    };
                    return new { processedResults = groupedbyProducts, summaryResults = new { totals, totalsByDepartment } };
                #endregion
                #region case 5
                case 5:
                    var orderdsJoined = from q in orders
                                        join fp in flatProducts on q.ProductId equals fp.Id
                                        join pi in db.PosInfo on q.PosInfoId equals pi.Id
                                        join d in db.Department on pi.DepartmentId equals d.Id
                                        join tbl in db.Table on q.TableId equals tbl.Id into ff
                                        from t in ff.DefaultIfEmpty()

                                        select new
                                        {
                                            OrderDetailId = q.OrderDetailId,
                                            // FODay = q.FODay,
                                            DepartmentId = pi.DepartmentId,
                                            DepartmentDescription = d.Description,
                                            PosInfoId = q.PosInfoId,
                                            Day = q.Day,
                                            PosInfoDetailId = q.PosInfoDetailId,
                                            Abbreviation = q.Abbreviation,
                                            Counter = q.Counter,
                                            Total = q.Total,
                                            ProductId = q.ProductId,
                                            Product = fp.Description,
                                            PriceListId = q.PriceListId,
                                            PriceListPrice = q.PriceListPrice,
                                            StaffId = q.StaffId,
                                            SalesTypeId = q.SalesTypeId,
                                            Qty = q.Qty,
                                            GroupId = q.GroupId,
                                            Covers = q.Covers,
                                            TableId = q.TableId,
                                            PosDescription = pi.Description,
                                            ProductPrice = q.ProductPrice,
                                            TableCode = t != null ? t.Code : ""
                                        };

                    var periodAnal = orderdsJoined.GroupBy(g => g.PosInfoId)
                                   .Select(s => new
                                   {
                                       PosInfoId = s.Key,
                                       PosDescr = s.FirstOrDefault().PosDescription,
                                       PerDay = s.GroupBy(g => g.Day.Value.Date).Select(ss => new
                                       {
                                           Day = ss.Key,
                                           PerReceipt = ss.GroupBy(ggg => new { ggg.Abbreviation, ggg.Counter }).Select(s3 => new
                                           {
                                               Day = s3.FirstOrDefault().Day,
                                           //   Inv_Descr = s3.Key.Inv_Descr,
                                           Inv_Abbr = s3.FirstOrDefault().Abbreviation,
                                               Inv_Counter = s3.Key.Counter,
                                               TableId = s3.FirstOrDefault().TableId,
                                               TableCode = s3.FirstOrDefault().TableCode,
                                           // AccountDescription = s3.FirstOrDefault().AccountDescription,
                                           StaffId = s3.FirstOrDefault().StaffId,
                                               Staff = s3.FirstOrDefault().StaffId,
                                               ReceiptTotal = s3.Sum(sm => sm.Total),
                                               Product = s3.Select(s4 => new
                                               {
                                                   ProductId = s4.ProductId,
                                                   Product = s4.Product,
                                                   Qty = s4.Qty,
                                                   Price = s4.ProductPrice,
                                                   Total = s4.Total
                                               })

                                           })
                                       })

                                   });
                    return new { processedResults = periodAnal, summaryResults = new { totalsByDepartment } };
                #endregion
                #region Case 6,7
                case 6:
                case 7:
                    var qtyTotals = orders.Sum(sm => (Double)sm.Qty);
                    var amountTotals = orders.Sum(sm => (Decimal)sm.Total);
                    var bestSeller = (from q in orders
                                      group q by new { q.ProductId } into s
                                      join fp in flatProducts on s.FirstOrDefault().ProductId equals fp.Id
                                      select new
                                      {
                                          ProductId = s.FirstOrDefault().ProductId,
                                          ProductDescription = fp.Description,
                                          Qty = s.Sum(sm => (Double)sm.Qty),
                                          Total = s.Sum(sm => (Decimal)sm.Total),
                                          QtyPerc = s.Sum(sm => (Double)sm.Qty) * 100 / qtyTotals,
                                          TotalPerc = s.Sum(sm => (Decimal)sm.Total) * 100 / amountTotals


                                      });
                    var result = repno == 6 ? bestSeller.OrderByDescending(o => o.Qty) : bestSeller.OrderByDescending(o => o.Total);
                    summaryResults = new { qtyTotals, amountTotals, totalsByDepartment };
                    return new { processedResults = result, summaryResults };


                #endregion
                #region Case 8
                case 8:
                    var flatQtyTotals = orders.Sum(sm => (Double)sm.Qty);
                    var flatAmountTotals = (orders.Sum(sm => (Decimal)sm.Total));

                    var salesByProductCategory = (from q in orders
                                                  group q by new { q.ProductId } into s
                                                  //join pr in Pricelist on s.FirstOrDefault().PriceListId equals pr.Id
                                                  join fp in flatProducts on s.FirstOrDefault().ProductId equals fp.Id
                                                  select new
                                                  {
                                                      ProductCategory = fp.ProductCategory,
                                                      ProductId = s.Key,
                                                      Category = fp.Category,
                                                      Product = fp.Description,
                                                      Qty = s.Sum(sm => (Double)sm.Qty),
                                                      Total = s.Sum(sm => (Decimal)sm.Total),
                                                      Receipts = s.Select(s4 => new { s4.PosInfoDetailId, s4.GroupId, s4.Counter }).Count()
                                                  })
                        .GroupBy(ggg => ggg.Category).Select(s5 => new
                        {
                            Category = s5.FirstOrDefault().Category,
                            Qty = s5.Sum(sm => (Double)sm.Qty),
                            Total = s5.Sum(sm => (Decimal)sm.Total),
                            TotalPerc = s5.Sum(sm => (Decimal)sm.Total) * 100 / flatAmountTotals,
                            TotalReceipts = s5.Sum(sm => sm.Receipts),
                            ProductCategories = s5.GroupBy(gg => gg.ProductCategory).Select(ss => new
                            {
                                ProductCategory = ss.Key,
                                Qty = ss.Sum(sm => (Double)sm.Qty),
                                Total = ss.Sum(sm => (Decimal)sm.Total),
                                TotalPerc = ss.Sum(sm => (Decimal)sm.Total) * 100 / flatAmountTotals,
                                TotalReceipts = ss.Sum(sm => sm.Receipts),
                                Products = ss.Select(s3 => new
                                {
                                    ProductId = s3.ProductId,
                                    Product = s3.Product,
                                    Qty = s3.Qty,
                                    Total = s3.Total,
                                    Receipts = s3.Receipts
                                })
                            })

                        });
                    return new { processedResults = salesByProductCategory, summaryResults = new { totalsByDepartment } };
                #endregion

                #region case 9
                case 9:
                    flatQtyTotals = orders.Sum(sm => (Double)sm.Qty);
                    flatAmountTotals = orders.Sum(sm => (Decimal)sm.Total);

                    var salesByPricelist = (from q in orders
                                            group q by new { q.ProductId, q.PriceListId } into s
                                            join pr in db.Pricelist on s.FirstOrDefault().PriceListId equals pr.Id
                                            join fp in flatProducts on s.FirstOrDefault().ProductId equals fp.Id
                                            select new
                                            {
                                                PriceList = pr.Description,
                                                ProductId = s.Key.ProductId,
                                                Product = fp.Description,
                                                Qty = s.Sum(sm => (Double)sm.Qty),
                                                Total = s.Sum(sm => (Decimal)sm.Total),
                                                Receipts = s.Select(s4 => new { s4.PosInfoDetailId, s4.GroupId, s4.Counter }).Count()
                                            })
                                      .GroupBy(gg => gg.PriceList).Select(ss => new
                                      {
                                          PriceList = ss.Key,
                                          Qty = ss.Sum(sm => (Double)sm.Qty),
                                          Total = ss.Sum(sm => (Decimal)sm.Total),
                                          TotalPerc = ss.Sum(sm => (Decimal)sm.Total) * 100 / flatAmountTotals,
                                          TotalReceipts = ss.Sum(sm => sm.Receipts),
                                          Products = ss.Select(s3 => new
                                          {
                                              ProductId = s3.ProductId,
                                              Product = s3.Product,
                                              Qty = s3.Qty,
                                              Total = s3.Total,
                                              Receipts = s3.Receipts
                                          })
                                      });

                    return new { processedResults = salesByPricelist, summaryResults = new { totalsByDepartment } };
                    #endregion
            };

            return null;
        }
        private Object GetProductPrices(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var pricelistsList = flts.PricelistsList.ToList();

            var flatProducts = db.Product.Select(s => new
            {
                Id = s.Id,
                Description = s.SalesDescription,
                Code = s.Code,
                ProductCategory = s.ProductCategories.Description,
                Category = s.ProductCategories.Categories.Description
            });

            var query = (from q in flatProducts
                         join pd in db.PricelistDetail.Where(w => pricelistsList.Contains(w.PricelistId)) on q.Id equals pd.ProductId into ff
                         from prices in ff.DefaultIfEmpty()
                         select new
                         {
                             ProductId = q.Id,
                             Code = q.Code,
                             Description = q.Description,
                             ProductCategory = q.ProductCategory,
                             Category = q.Category,
                             Prices = prices.Price,
                             PricelistId = prices.PricelistId,
                             PricelistDescription = prices.Pricelist.Description
                         });

            Object res = query;
            //switch (flts.grouping)
            //{
            //     1 : res = query.Where(w=> w.pri;
            //     2 :break;
            //}
            //.GroupBy(g => g.ProductId).Select(s => new
            //{
            //    //ProductId = s.FirstOrDefault().ProductId,
            //    Code = s.FirstOrDefault().Code,
            //    Description = s.FirstOrDefault().Description,
            //    ProductCategory = s.FirstOrDefault().ProductCategory,
            //    Category = s.FirstOrDefault().Category,
            //    Prices = s.Select(ss => new
            //    {
            //        PricelistId = ss.PricelistId,
            //        PricelistDescription = ss.PricelistDescription,
            //        Price = ss.Prices
            //    }).Distinct()
            //});
            return new { processedResults = res };

        }
        private IEnumerable<dynamic> GetFlatSalesByInvoice(string filters, Int32 eodId = 0)
        {
            //ReportHelper
            var salesFlat = GetFlatSales(filters, eodId);

            var salesByReceipt = salesFlat.GroupBy(g => new { g.PosInfoDetailId, g.Counter, g.GroupId }).Select(s => new
            {
                FODay = s.FirstOrDefault().FODay,
                CloseId = s.FirstOrDefault().CloseId,
                DepartmentId = s.FirstOrDefault().DepartmentId,
                DepartmentDescription = s.FirstOrDefault().DepartmentDescription,
                PosInfoId = s.FirstOrDefault().PosInfoId,
                PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                PosInfoCode = s.FirstOrDefault().PosInfoCode,
                //		PosInfoDetailIds = s.Select(ss=>ss.PosInfoDetailId),
                StaffId = s.FirstOrDefault().StaffId,
                StaffName = s.FirstOrDefault().StaffName,
                Void = s.All(v => v.Void),
                Day = s.Min(m => m.Day),
                Abrreviation = s.FirstOrDefault().Abbreviation,
                Counter = s.FirstOrDefault().Counter,
                Table = s.FirstOrDefault().Table,
                Room = s.FirstOrDefault().Room,
                IsInvoice = s.FirstOrDefault().IsInvoice,
                IsPrinted = s.FirstOrDefault().IsPrinted,
                IsCanceled = s.FirstOrDefault().IsCancel,
                Cover = s.Max(m => m.Cover),
                Total = s.Distinct().Sum(sm => (Decimal?)sm.TotalAfterDiscount),
                Discount = s.Distinct().Sum(sm => (Decimal?)sm.Discount),
                //	TransactionIds = s.Select(ss=>ss.TransactionId),
                TransactionAmount = s.Select(ss => new { TransactionAmount = ss.TransactionAmount, TransactionId = ss.TransactionId }).Distinct().Sum(sm => (Decimal?)sm.TransactionAmount),
                AccountId = s.FirstOrDefault().AccountId,
                AccountDescription = s.FirstOrDefault().AccountDescription,
                AccountType = s.FirstOrDefault().AccountType,
                Guest = s.FirstOrDefault().Guest,
                ItemsCount = s.Distinct().Count(),
                Items = s.Distinct(),


                Vats = s.Select(ss => new { VatId = ss.VatId, VatRate = ss.VatRate, VatAmount = ss.VatAmount, VatGross = ss.VatGross, VatNet = ss.VatNet }).GroupBy(gv => gv.VatId).Select(sv => new VatValuesModel
                {
                    VatId = sv.Key,
                    VatRate = sv.FirstOrDefault().VatRate,
                    VatAmount = sv.Sum(sm => (Decimal?)sm.VatAmount),
                    VatGross = sv.Sum(sm => (Decimal?)sm.VatGross),
                    VatNet = sv.Sum(sm => (Decimal?)sm.VatNet)
                }),
                PosInfoDescription = s.FirstOrDefault().PosInfoDescription
            });
            return salesByReceipt;
        }

        // GET api/EndOfDay
        public Object GetPeriodicSalesPerProduct(string storeid, string fromDate, string toDate, int periodicSalesType)
        {
            DateTime fromD = Convert.ToDateTime(fromDate).Date;
            DateTime toD = Convert.ToDateTime(toDate).Date.AddDays(1);

            //  var receiptCount = db.Order.Where(w=>w.Day >= fromD && w.Day <= toD).Count();
            //var flat = (from q in db.OrderDetail.Include("Order")
            //                                    .Include("PricelistDetail")
            //                                    .Include("PricelistDetail.Pricelist")
            //                                    .Include("SalesType")
            //                                    .Include("Order.Staff")
            //                                    .Where(w => w.Order.Day >= fromD && w.Order.Day <= toD)
            //            select new
            //            {
            //                Day = q.Order.Day,
            //                PriceListId = (q.PricelistDetail != null) ? q.PricelistDetail.Pricelist.Id : 0,
            //                PriceList = (q.PricelistDetail != null) ? q.PricelistDetail.Pricelist.Description : "Χωρίς",
            //                PriceLictPrice = (q.PricelistDetail != null) ? q.PricelistDetail.Price : 0,
            //                Product = q.Product.Description,
            //                ProductId = q.Product.Id,
            //                Price = q.Price,
            //                SalesTypeId = q.SalesType != null ? q.SalesType.Description : "Χωρίς",
            //                Qty = q.Qty,
            //                StaffId = q.Order.StaffId,
            //                Staff = q.Order.Staff.FirstName + " " + q.Order.Staff.LastName,
            //                OrderId = q.Order.Id
            //            }).ToList();


            var flat = (from q in db.OrderDetail.Include("PricelistDetail")
                                                .Include("OrderDetailIgredients")
                                                .Where(w => w.Order.Day >= fromD && w.Order.Day < toD && (w.IsDeleted ?? false) == false)
                        join od in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals od.OrderDetailId
                        join pid in db.PosInfoDetail.Where(w => w.IsInvoice == true && w.IsCancel == false) on od.PosInfoDetailId equals pid.Id
                        join pi in db.PosInfo on pid.PosInfoId equals pi.Id
                        select new
                        {
                            Day = q.Order.Day,
                            PriceListId = (q.PricelistDetail != null) ? q.PricelistDetail.Pricelist.Id : 3,
                            PriceList = (q.PricelistDetail != null) ? q.PricelistDetail.Pricelist.Description : "No Data",
                            PriceLictPrice = (q.PricelistDetail != null) ? q.PricelistDetail.Price : 0,
                            Product = q.Product.Description,
                            ProductId = q.Product.Id,
                            Price = q.Price,
                            IngredientsPrice = q.OrderDetailIgredients.Sum(s => s.Price),// * (decimal?)s.Qty),
                            Total = q.Qty * ((double)q.Price + (q.OrderDetailIgredients.Count() == 0 ? 0 : q.OrderDetailIgredients.Sum(s => (double)s.Price / q.Qty))),
                            InvTypeGroup = pid.GroupId,
                            PosInfoDetailId = pid.PosInfoId,
                            PosInfoId = pi.Id,
                            PosDescr = pid.PosInfo.Description,
                            Inv_Abbr = pid.Abbreviation,
                            Inv_Descr = pid.Description,
                            Inv_Counter = od.Counter,
                            SalesTypeId = q.SalesType != null ? q.SalesType.Id : -1,
                            SalesTypeDesc = q.SalesType != null ? q.SalesType.Description : "Χωρίς",
                            StaffId = q.Order.StaffId,
                            Staff = q.Order.Staff.FirstName + " " + q.Order.Staff.LastName,
                            Qty = q.Qty
                        }).ToList();


            switch (periodicSalesType)
            {
                case 0: return PeriodicStats(flat);
                case 1: return PeriodicStatsPerWaiter(flat);
                case 2: return PeriodicStatsPerProduct(flat);
                case 3: return TotalsPerHour(flat);
                default:
                    break;
            }


            return null;
        }

        public Object GetPeriodicSalesPerProduct(string storeid, string filters)
        {
            // string sampleJson = @"{""FromDate"":""2013-12-31T22:00:00.000Z"",""ToDate"":""2014-07-29T21:00:00.000Z"",""PosList"":[1,4,5],""StaffList"":[2,3],""InvoiceList"":[1,2,3], ""ReportType"": 6}";
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var stats = new StatisticsRepository(db);
            if (flts.ReportType > 100)
            {
                switch (flts.ReportType)
                {
                    case 9999: return GetReportList(filters);
                    case 101: return SalesAnalysis();
                    case 102: return SalesPerDepartmentsChecking(filters);
                    case 103: return SalesForErp(filters);
                    case 105: return PosDailyRevenue(filters, false);
                    case 106: return PosDailyRevenue(filters, true);
                    case 107: return GetXReportV2(filters);
                    case 1081: return GetZReportWrapper(storeid, filters);
                    case 108: return GetZReport(storeid, 512);
                    case 109:
                    case 1091:
                    case 1092:
                    case 1093:
                        return GetCoversForV2(filters);
                    case 110: return GetDailyWaiterReceipts(filters);
                    case 111: return GetReportByWaiter(filters);
                    case 112: return GetReportTicketCount(filters); // 21/8/2014
                    case 113: return ReportsHelper.GetWaiterReportNew(filters, db);
                    case 114: return GetLockerAvailability(filters);

                    case 200: return GetProductPrices(filters);

                    //8000 Saved for Check and repair
                    case 8001: return EndOfDayProductSalesComparer(filters);
                    //
                    case 901: return GetXReportV2(filters);//GetReceiptForReprint(22, 36);
                    case 1002: return SalesPerDepartmentsCheckingFromInvoicesFlat(filters, false);
                    case 1003: return SalesPerDepartmentsCheckingFromInvoicesFlatVoided(filters, true);
                    case 1004: return SalesPerDepartmentsCheckingFromInvoicesFlatWithVoids(filters, false);
                    case 9001: return ExportForGoodys(filters);
                    case 9005: return GetSalesByReceipt(filters);
                    case 9006: return GetSalesByReceiptMaster(filters);
                    //Test
                    case 99005: return GetSalesByReceiptFromStatRepo(filters);
                    case 99006: return GetSalesByReceiptMasterFromStatRepo(filters);

                    case 9007: return stats.GetDataFor9007(filters);
                    case 9008:
                    case 9009: return GetSalesForProductReport(filters);
                    case 9010:
                    case 9011: return GetCoversForV2(filters);//GetInvoicesByVat(filters);
                    case 901111: return stats.GetReceiptsFor9011(filters);

                    case 9012: return GetAllowedCost(filters);
                    case 9013: return GetComplimentaryRoom(filters);
                    case 90130: return stats.GetDataFor90130(filters);


                    //report kleisimatos 4000
                    case 104: return TransferPerPmsDepartment(filters);
                    case 4001: return TransferPmsDepartmentPerInvoice(filters);
                    case 4010: return MealConsumptions(filters);
                    case 5001: return ProductForEodStats(filters);
                    case 7000: return ProductsPerPricelist(filters);
                    default:
                        break;
                }
                //return ReturnAllProductsWithPrices();


            }

            return GetSalesForReporting(filters);


            if (flts.ReportType == 5)
            {
                var flat = GetFlatSales(filters, 0).Distinct().Where(w => w.IsInvoice == true && w.Void == false);
                var periodAnal = flat.GroupBy(g => g.PosInfoId).Select(s => new
                {
                    PosInfoId = s.Key,
                    PosDescr = s.FirstOrDefault().PosDescr,
                    PerDay = s.GroupBy(g => g.Day.Value.Date).Select(ss => new
                    {
                        Day = ss.Key,
                        PerReceipt = ss.GroupBy(ggg => new { ggg.Inv_Descr, ggg.Inv_Counter }).Select(s3 => new
                        {
                            Day = s3.FirstOrDefault().Day,
                            Inv_Descr = s3.Key.Inv_Descr,
                            Inv_Abbr = s3.FirstOrDefault().Inv_Abbr,
                            Inv_Counter = s3.Key.Inv_Counter,
                            TableId = s3.FirstOrDefault().TableId,
                            TableCode = s3.FirstOrDefault().TableCode,
                            AccountDescription = s3.FirstOrDefault().AccountDescription,
                            StaffId = s3.FirstOrDefault().StaffId,
                            Staff = s3.FirstOrDefault().Staff,
                            ReceiptTotal = s3.Sum(sm => sm.Total),
                            Product = s3.Select(s4 => new
                            {
                                ProductId = s4.ProductId,
                                Product = s4.Product,
                                Qty = s4.Qty,
                                Price = s4.Price,
                                Total = s4.Total
                            })

                        })
                    })

                });
                return new { processedResults = periodAnal };
                //return periodAnal;
            }

            return null;
        }

        private object GetZReportWrapper(string storeid, string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            int eodId = (int)flts.EodId.Value;
            return GetZReport(storeid, eodId);
        }

        private object GetXReportV2(string filters)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            var piid = flts.PosList.FirstOrDefault();
            var useEod = flts.UseEod;
            var eodId = 0;
            //Configuration.LazyLoadingEnabled = true;

            var posdata = db.PosInfo.Where(f => f.Id == piid).FirstOrDefault();
            var salesByReceipt = db.Invoices.Where(w => (w.EndOfDayId ?? 0) == eodId && w.PosInfoId == piid
                                                //  && (w.IsVoided ?? false) == false
                                                && (w.IsDeleted ?? false) == false
                                                && w.InvoiceTypeId != null
                                                && w.InvoiceTypes.Type != (int)InvoiceTypesEnum.Order
                                                && w.InvoiceTypes.Type != (int)InvoiceTypesEnum.Void
                                                && w.InvoiceTypes.Type != 10);

            var allInvoices = salesByReceipt.SelectMany(sm => sm.Transactions).Select(s => new
            {
                InvoiceId = s.InvoicesId,
                Description = s.Invoices.InvoiceTypes.Abbreviation,
                InvoiceCounte = s.Invoices.Counter,
                TransAmount = s.Amount,
                InvoiceTotal = s.Invoices.Total,
                Table = s.Invoices.Table.Code,
                Room = s.Invoice_Guests_Trans.FirstOrDefault().Guest.Room,
                StaffCode = s.Staff.Code,
                StaffName = s.Staff.LastName,
                IsVoided = s.Invoices.IsVoided ?? false,
                AccountId = s.AccountId,
                AccountDescription = s.Accounts.Description,
                AccountType = s.Accounts.Type
            });//.ToList();//.Dump();			

            var allInvoicesFinal = from q in salesByReceipt//.Where(w => (w.IsVoided ?? false) == false)
                                   join qq in allInvoices on q.Id equals qq.InvoiceId into f
                                   from temp in f.DefaultIfEmpty()
                                   select new
                                   {
                                       InvoiceId = q.Id,
                                       Description = q.InvoiceTypes.Abbreviation,
                                       InvoiceCounte = q.Counter,
                                       TransAmount = q.Transactions.FirstOrDefault() != null ? temp.TransAmount : 0,
                                       InvoiceTotal = q.Total,
                                       Table = q.Table.Code,
                                       Room = q.Transactions.FirstOrDefault() != null ? temp.Room : "",
                                       StaffCode = q.Staff.Code,
                                       StaffName = q.Staff.LastName,
                                       IsVoided = q.IsVoided ?? false,
                                       AccountId = q.Transactions.FirstOrDefault() != null ? temp.AccountId : 0,
                                       AccountDescription = q.Transactions.FirstOrDefault() != null ? temp.AccountDescription : "Undefined",
                                       AccountType = q.Transactions.FirstOrDefault() != null ? temp.AccountType : 0
                                   };
            var staffCashier = db.Transactions.Where(w => (w.EndOfDayId ?? 0) == eodId && w.PosInfoId == piid && (w.IsDeleted ?? false) == false)
                               .GroupBy(g => new { g.StaffId, g.TransactionType, g.InOut, g.AccountId })
                               .Select(ss => new
                               {
                                   StaffId = ss.FirstOrDefault().StaffId,
                                   AccountId = ss.FirstOrDefault().AccountId,
                                   AccountType = ss.FirstOrDefault().Accounts.Type,
                                   AccountDescription = ss.FirstOrDefault().Accounts.Description,
                                   Amount = ss.Sum(sm => sm.Amount),
                                   InOut = ss.FirstOrDefault().InOut,
                                   Descriptions = ss.FirstOrDefault().Description,
                                   TransactionType = ss.FirstOrDefault().TransactionType

                               }).ToList();//.GroupBy(g.StaffId);
                                           /*
                                                       var odIngVats = (from q in db.OrderDetailIgredientVatAnal
                                                                        join od in db.OrderDetailIgredients on q.OrderDetailIgredientsId equals od.Id
                                                                        join odi in db.OrderDetailInvoices on od.OrderDetailId equals odi.OrderDetailId
                                                                        join inv in salesByReceipt on odi.InvoicesId equals inv.Id//Invoices.Where(w => (eod.Contains(w.EndOfDayId ?? 0))  && posList.Contains(w.PosInfoId.Value) ) on odi.InvoicesId equals inv.Id
                                                                        select new
                                                                        {
                                                                            InvoiceId = inv.Id,
                                                                            Id = q.Id,
                                                                            OrderDetailId = odi.Id,
                                                                            Gross = q.Gross,
                                                                            VatId = q.VatId,
                                                                            Net = q.Net,
                                                                            VatRate = q.VatRate,
                                                                            VatAmount = q.VatAmount
                                                                        }).ToList().Select(s => new
                                                                        {
                                                                            InvoiceId = s.Id,
                                                                            Id = s.Id,
                                                                            OrderDetailId = s.Id,
                                                                            Total = Math.Round((decimal)s.Gross, 2, MidpointRounding.AwayFromZero),
                                                                            VatId = s.VatId,
                                                                            Net = Math.Round((decimal)s.Gross, 2, MidpointRounding.AwayFromZero) - Math.Round((decimal)s.VatAmount, 2, MidpointRounding.AwayFromZero),
                                                                            //Math.Round((decimal)s.Net, 2, MidpointRounding.AwayFromZero),
                                                                            VatRate = s.VatRate,
                                                                            VatAmount = Math.Round((decimal)s.VatAmount, 2, MidpointRounding.AwayFromZero)

                                                                        });

                                                       var odVats = (from q in db.OrderDetailVatAnal
                                                                     join od in db.OrderDetail on q.OrderDetailId equals od.Id
                                                                     join odi in db.OrderDetailInvoices on od.Id equals odi.OrderDetailId
                                                                     join inv in salesByReceipt.Where(f => (f.IsVoided ?? false) == false) on odi.InvoicesId equals inv.Id//Invoices.Where(w => (eod.Contains(w.EndOfDayId ?? 0))  && posList.Contains(w.PosInfoId.Value) ) on odi.InvoicesId equals inv.Id
                                                                     select new
                                                                     {
                                                                         InvoiceId = inv.Id,
                                                                         Id = q.Id,
                                                                         OrderDetailId = odi.Id,
                                                                         Gross = q.Gross,
                                                                         VatId = q.VatId,
                                                                         Net = q.Net,
                                                                         VatRate = q.VatRate,
                                                                         VatAmount = q.VatAmount
                                                                     }).ToList().Select(s => new
                                                                     {
                                                                         InvoiceId = s.Id,
                                                                         Id = s.Id,
                                                                         OrderDetailId = s.Id,
                                                                         Total = Math.Round((decimal)s.Gross, 2, MidpointRounding.AwayFromZero),
                                                                         VatId = s.VatId,
                                                                         Net = Math.Round((decimal)s.Gross, 2, MidpointRounding.AwayFromZero) - Math.Round((decimal)s.VatAmount, 2, MidpointRounding.AwayFromZero),
                                                                         //Math.Round((decimal)s.Net, 2, MidpointRounding.AwayFromZero),
                                                                         VatRate = s.VatRate,
                                                                         VatAmount = Math.Round((decimal)s.VatAmount, 2, MidpointRounding.AwayFromZero)

                                                                     });

                                                       var joinedVats = odVats.Union(odIngVats).ToList().GroupBy(g => g.InvoiceId)
                                                                                                           .Select(s => new
                                                                                                           {
                                                                                                               InvoiceId = s.Key,
                                                                                                               Vats = s.GroupBy(gg => gg.VatId).Select(sss => new
                                                                                                               {
                                                                                                                   VatId = sss.Key,
                                                                                                                   Total = sss.Sum(sm => sm.Total),
                                                                                                                   Net = sss.Sum(sm => sm.Total - sm.VatAmount),
                                                                                                                   VatRate = sss.FirstOrDefault().VatRate,
                                                                                                                   VatAmount = sss.Sum(sm => sm.VatAmount)
                                                                                                               })
                                                                                                           });
                                           */

            var validOds = from q in db.OrderDetail
                           join qq in db.OrderDetailInvoices on q.Id equals qq.OrderDetailId
                           join qqq in salesByReceipt on qq.InvoicesId equals qqq.Id
                           select q;

            //var sumOds = db.OrderDetail.Where(w => w.Order.EndOfDayId == null && w.Order.PosId == piid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5)
            //                           .SelectMany(s => s.OrderDetailVatAnal)
            //                           .GroupBy(g => g.VatId)
            //                           .Select(s => new {
            //    VatId = s.Key,

            //    Gross = s.Sum(sm => sm.Gross),
            //    VatAmount = s.Sum(sm => sm.VatAmount),
            //    Tax = s.Sum(sm => sm.TaxAmount)
            //});
            var sumOds = validOds.Where(w => w.Order.EndOfDayId == null && w.Order.PosId == piid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5)
                                     .SelectMany(s => s.OrderDetailVatAnal)
                                     .GroupBy(g => g.VatId)
                                     .Select(s => new
                                     {
                                         VatId = s.Key,

                                         Gross = s.Sum(sm => sm.Gross),
                                         VatAmount = s.Sum(sm => sm.VatAmount),
                                         Tax = s.Sum(sm => sm.TaxAmount)
                                     });
            //var sumOdis = db.OrderDetail.Where(w => w.Order.EndOfDayId == null && w.Order.PosId == piid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5)
            //                            .SelectMany(ss => ss.OrderDetailIgredients)
            //                            .SelectMany(sm => sm.OrderDetailIgredientVatAnal)
            //                            .GroupBy(g => g.VatId)
            //                            .Select(s => new {
            //    VatId = s.Key,
            //    Gross = s.Sum(sm => sm.Gross),
            //    VatAmount = s.Sum(sm => sm.VatAmount),
            //    Tax = s.Sum(sm => sm.TaxAmount)
            //});
            var sumOdis = validOds.Where(w => w.Order.EndOfDayId == null && w.Order.PosId == piid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5)
                            .SelectMany(ss => ss.OrderDetailIgredients)
                            .SelectMany(sm => sm.OrderDetailIgredientVatAnal)
                            .GroupBy(g => g.VatId)
                            .Select(s => new
                            {
                                VatId = s.Key,
                                Gross = s.Sum(sm => sm.Gross),
                                VatAmount = s.Sum(sm => sm.VatAmount),
                                Tax = s.Sum(sm => sm.TaxAmount)
                            });

            var vatsTemp = sumOds.Union(sumOdis).GroupBy(g => g.VatId).Select(s => new
            {
                VatId = s.Key,
                Gross = s.Sum(sm => sm.Gross),
                VatAmount = s.Sum(sm => sm.VatAmount),
                Tax = s.Sum(sm => sm.Tax)
            }).ToList();

            var vats = (from q in vatsTemp
                        join v in db.Vat.ToList() on q.VatId equals v.Id
                        select new
                        {
                            VatId = q.VatId,
                            VatRate = v.Percentage,
                            VatAmount = (Decimal)Math.Round((Decimal)q.VatAmount, 2, MidpointRounding.AwayFromZero),
                            Net = (Decimal)Math.Round((Decimal)q.Gross, 2, MidpointRounding.AwayFromZero) - (Decimal)Math.Round((Decimal)q.VatAmount, 2, MidpointRounding.AwayFromZero) - (Decimal)Math.Round((Decimal)q.Tax, 2, MidpointRounding.AwayFromZero),
                            Gross = (Decimal)Math.Round((Decimal)q.Gross, 2, MidpointRounding.AwayFromZero),
                            Tax = (Decimal)Math.Round((Decimal)q.Tax, 2, MidpointRounding.AwayFromZero)
                        }).ToList();


            //	joinedVats.Dump();
            //var payments = salesByReceipt.Where(f => f.IsVoided == false).SelectMany(s => s.Transactions).ToList();
            var voided = salesByReceipt.Where(f => f.IsVoided == true).SelectMany(s => s.Transactions).ToList();
            //		Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId.Value)
            //	                                    && (w.IsVoided ?? false) == true
            //	                                    && (w.IsDeleted ?? false) == false
            //	                                    && w.InvoiceTypeId != null && w.InvoiceType.Type != 2 && w.InvoiceType.Type != 3).SelectMany(s=>s.Transactions).ToList();
            //         var vats = joinedVats.SelectMany(sm => sm.Vats).ToList();

            decimal? lockerPrice = 1;

            var totalLockers = db.Transactions.Where(w => w.EndOfDayId == null
                                                                 && w.PosInfoId == piid
                                                                 && (w.TransactionType == 6
                                                                 || w.TransactionType == 7));
            decimal? openLocker = totalLockers.Where(w => w.TransactionType == 6).Sum(sm => sm.Amount) ?? 0;
            decimal? paidLocker = (totalLockers.Where(w => w.TransactionType == 7).Sum(sm => sm.Amount) * -1) ?? 0;




            var barcodes = db.Transactions.Where(w => w.PosInfoId == piid
                                                             && w.EndOfDayId == null
                                                             && w.TransactionType == 9
                                                             && (w.IsDeleted ?? false) == false
                                                             ).Sum(sm => sm.Amount) ?? 0;
            var rlp = db.RegionLockerProduct.FirstOrDefault();
            if (rlp != null)
                lockerPrice = rlp.Price;



            var prods = salesByReceipt.Where(w => w.IsVoided == false)//.AsNoTracking()
                                                                      //                           .Include("OrderDetailInvoices.OrderDetail")
                                                                      //.Where(w => w.EndOfDayId == null && w.PosInfoId == piid && (w.IsDeleted ?? false) == false && (w.IsVoided ?? false) == false)
                                               .SelectMany(w => w.OrderDetailInvoices).Select(s => new
                                               {
                                                   ProductId = s.OrderDetail.ProductId,
                                                   Qty = s.OrderDetail.Qty,
                                                   Total = s.OrderDetail.TotalAfterDiscount
                                               });

            var productsForEODStats = (from q in db.ProductsForEOD
                                       join qq in prods on q.ProductId equals qq.ProductId
                                       select new
                                       {
                                           ProductId = q.ProductId,
                                           Description = q.Description,
                                           Qty = qq.Qty,
                                           Total = qq.Total
                                       }).GroupBy(g => g.ProductId).Select(s => new
                                       {
                                           ProductId = s.FirstOrDefault().ProductId,
                                           Description = s.FirstOrDefault().Description,
                                           Qty = s.Sum(sm => sm.Qty),
                                           Total = s.Sum(sm => sm.Total)
                                       });

            var xDataToPrint = new
            {
                Day = posdata.FODay,
                PosCode = posdata.Id,//Allazei sto webpos
                PosDescription = posdata.Description,
                ReportNo = 0,
                Gross = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => (decimal?)sm.Total), 2),
                VatAmount = Math.Round((decimal)(decimal)vats.Sum(sm => sm.VatAmount), 2),
                Net = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => (decimal?)sm.Total), 2) - Math.Round((decimal)(decimal)vats.Sum(sm => sm.VatAmount), 2),
                //Math.Round((decimal)vats.Sum(sm => sm.Net), 2),
                Discount = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => sm.Discount != null ? (decimal)sm.Discount : 0), 2),
                TicketCount = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Count(),
                ItemsCount = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).SelectMany(s => s.OrderDetailInvoices).Sum(sm => sm.OrderDetail.Qty),

                PaymentAnalysis = allInvoices.Where(a => a.AccountType != 4 && a.IsVoided == false).ToList()
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().AccountDescription,// != null ? w.FirstOrDefault().Account.Description : "",
                                             Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.TransAmount), 2)
                                         }),
                VatAnalysis = vats,// vats.GroupBy(q => q.VatId).ToList()

                //{
                //    VatRate = w.VatRate,
                //    Gross = Math.Round((decimal)w.Total, 2),
                //    VatAmount = Math.Round((decimal)w.VatAmount, 2),
                //    // Net = Math.Round((decimal)w.Sum(r => (decimal?)r.Net), 2)// - (decimal)w.Sum(r => (decimal?)r.Net)
                //    Net = Math.Round((decimal)w.Net, 2)
                //    //VatRate = w.FirstOrDefault().VatRate,
                //    //Gross = Math.Round((decimal)w.Sum(r => (decimal?)r.Total), 2),
                //    //VatAmount = Math.Round((decimal)w.Sum(r => (decimal?)r.VatAmount), 2),
                //    //// Net = Math.Round((decimal)w.Sum(r => (decimal?)r.Net), 2)// - (decimal)w.Sum(r => (decimal?)r.Net)
                //    //Net = Math.Round((decimal)w.Sum(r => (decimal?)r.Total), 2) - Math.Round((decimal)w.Sum(r => (decimal?)r.VatAmount), 2)
                //}),
                VoidAnalysis = voided//Where(f => f.Void == true && f.IsInvoice == true)
                                       .GroupBy(q => q.AccountId)
                                       .Select(w => new
                                       {
                                           Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                           Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Amount), 2, MidpointRounding.AwayFromZero)
                                       }),
                CardAnalysis = allInvoices.Where(a => a.AccountType == 4).ToList()//salesByReceipt.Where(f => f.AccountType == 4 && f.Void == false && f.IsCancel == false && f.IsInvoice == true)
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().AccountDescription,// != null ? w.FirstOrDefault().Account.Description : "",
                                             Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.TransAmount), 2, MidpointRounding.AwayFromZero)
                                         }),
                Barcodes = barcodes,
                Lockers = new
                {
                    HasLockers = rlp != null,
                    TotalLockers = openLocker / lockerPrice,
                    TotalLockersAmount = openLocker,
                    Paidlockers = (paidLocker / lockerPrice) ?? 0,
                    PaidlockersAmount = paidLocker,
                    OccLockers = (openLocker / lockerPrice) - (paidLocker / lockerPrice),
                    OccLockersAmount = openLocker - paidLocker
                },
                ProductsForEODStats = productsForEODStats
            };
            db.Configuration.LazyLoadingEnabled = false;
            return new { xDataToPrint = xDataToPrint, AllInvoicesFinal = allInvoicesFinal, StaffCashier = staffCashier };

        }

        private object GetReportList(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            //return new { processedResults = 
            return db.ReportList.Where(w => w.Id == flts.ReportListId).FirstOrDefault();

        }

        private object GetLockerAvailability(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var posid = flts.PosList.FirstOrDefault();
            var lp = db.RegionLockerProduct.FirstOrDefault();

            if (lp != null && lp.Price > 0)
            {
                var totalLockers = db.Transactions.Where(w => w.TransactionType == (int)TransactionTypesEnum.OpenLocker && w.PosInfoId == posid && w.EndOfDayId == null).Sum(sm => sm.Amount) / lp.Price;
                var paidlockers = ((db.Transactions.Where(w => w.TransactionType == (int)TransactionTypesEnum.CloseLocker && w.PosInfoId == posid && w.EndOfDayId == null).Sum(sm => sm.Amount) * -1) / lp.Price) ?? 0;
                var occLockers = totalLockers - paidlockers;

                return new { totalLockers, paidlockers, occLockers };
            }
            return null;
        }


        private Object GetInvoicesByVat(string filters)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;

            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var posinfos = db.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId) && flts.PosList.Contains(w.Id));
            var eods = db.EndOfDay.Where(w => w.FODay >= flts.FromDate.Date && w.FODay <= flts.ToDate.Date);


            //TODO: Correct that
            var details = from q in db.OrderDetailInvoices
                          join qq in eods on q.EndOfDayId equals qq.Id
                          group q by new { q.Id, VatId = q.VatId } into grp
                          select new
                          {
                              InvoiceId = grp.FirstOrDefault().Id,
                              Description = grp.FirstOrDefault().Description,
                              // IsVoided = grp.FirstOrDefault().IsVoided,
                              Counter = grp.FirstOrDefault().Counter,
                              InvoiceTotal = grp.FirstOrDefault().Total,
                              InvoiceDiscount = grp.FirstOrDefault().Discount,
                              StaffId = grp.FirstOrDefault().StaffId,
                              //InvoiceTypeId = grp.FirstOrDefault().InvoiceTypeId,
                              //Day = grp.FirstOrDefault().Day,
                              //Cover = grp.FirstOrDefault().Cover,
                              EndOfDayId = grp.FirstOrDefault().EndOfDayId,
                              TableId = grp.FirstOrDefault().TableId,

                              VatId = grp.FirstOrDefault().VatId,
                              VatRate = grp.FirstOrDefault().VatRate,
                              VatAmount = grp.Sum(sm => sm.VatAmount)
                          };

            var metadata1 = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
            return new { processedResults = details.Distinct().ToList(), metadata = metadata1 };
            IQueryable<Invoices> invoices = db.Invoices;
            if (flts.UseEod == false)
            {
                invoices = invoices.GetNotAudited().GetInvoiceOnly(db.InvoiceTypes.SalesInvoiceTypes());
            }
            else
            {
                invoices = invoices.GetAudited(eods).GetInvoiceOnly(db.InvoiceTypes.SalesInvoiceTypes());
            }
            //db.Entry(invoices)
            var trans = invoices.GetTransactionsWithAccounts(db.Accounts);
            //  var details = invoices.GetInvoiceDetailsForDisplayWithLet();//.GroupBy(g=>new { g.InvoiceForDetailId, g.VatId }).Select(s=>new
            //{
            //    InvoiceId = s.FirstOrDefault().InvoiceForDetailId,
            //    VatId = s.FirstOrDefault().VatId,
            //    VatAmount = s.Sum(sm => sm.VatAmount),

            //}).ToList();

            //var final = from q in invoices
            //            let l = details.Where(w => w.InvoiceForDetailId == q.Id)
            //            from d in l
            //            let l1 = trans.Where(w => w.InvoiceId == q.Id)
            //            from t in l1.DefaultIfEmpty()
            var final = from q in invoices
                        join d in details on q.Id equals d.InvoiceId
                        join tr in trans on q.Id equals tr.InvoiceId into f
                        from t in f.DefaultIfEmpty()
                        select new
                        {
                            //StoreId = storeid,
                            //                      PosInfoCode = q.PosInfoCode,
                            //DepartmentId = q.DepartmentId,
                            //                      DepartmentDesc = q.DepartmentDesc,
                            //OrderDetailId = d.OrderDetailId,
                            InvoiceId = q.Id,
                            Description = q.Description,
                            IsVoided = q.IsVoided,
                            Counter = q.Counter,
                            InvoiceTotal = q.Total,
                            InvoiceDiscount = q.Discount,
                            StaffId = q.StaffId,
                            InvoiceTypeId = q.InvoiceTypeId,
                            Day = q.Day,
                            Cover = q.Cover,
                            EndOfDayId = q.EndOfDayId,
                            TableId = q.TableId,
                            //RoomId = q.RoomId,

                            //                      ProductCategory = q.ProductCategory,
                            //                      Category = q.Category,
                            VatId = d.VatId,
                            //                      VatRate = q.VatRate,
                            VatAmount = d.VatAmount,
                            //Net = d.TotalAfterDiscount - d.VatAmount,
                            //Gross = d.TotalAfterDiscount,
                            //TotalAfterDiscount = d.TotalAfterDiscount,
                            Discount = q.Discount,
                            //                      PricelistDescription = q.PricelistDescription,
                            //                      PricelistId = q.PricelistId,
                            //                      TimeZone = q.TimeZone,
                            //                      Inhouse = q.Inhouse,
                            Abbreviation = q.Description,
                            //                      PosInfoDescription = q.PosInfoDescription,
                            //                      InvoiceTypeDescription = q.InvoiceTypeDescription,
                            //InvoiceTypeCode = q.InvoiceTypeCode,

                            //TransAmount = (t != null) ? t.Amount : 0,
                            //AccountId = (t != null) ? t.AccountId : -1,
                            //AccountType = (t != null) ? t.AccountType : -1,
                            //AccountDescription = (t != null) ? t.Description : "Not Paid",


                            //                      TransStaffId = (trans != null) ? trans.StaffId : null,
                            //                      TransStaffName = (trans != null) ? trans.StaffLastname : "",
                            //                      Room = (trans != null) ? trans.Room : "",
                            //                      Guest = (trans != null) ? trans.Guest : ""
                        };
            var a = final.Distinct().Count();

            var metadata12 = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
            return new { processedResults = final.Distinct().ToList(), metadata = metadata1 };
        }

        private Object GetCoversForV2(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var posinfos = db.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId) && flts.PosList.Contains(w.Id));
            var eods = db.EndOfDay.Where(w => w.FODay >= flts.FromDate.Date && w.FODay <= flts.ToDate.Date);
            var invTypes = db.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void).Select(s => new
            {
                Id = s.Id,
                Abbr = s.Abbreviation,
                Type = s.Type,
                TypeDescription = s.Type == 1 ? "Sales" :
                                        s.Type == 2 ? "Order" :
                                            s.Type == 3 ? "Voids" :
                                                s.Type == 4 ? "Complimentary" :
                                                    s.Type == 5 ? "Allowance" :
                                                    ""
            });
            var pricelistFlat = from q in db.PricelistDetail
                                join qq in db.Pricelist on q.PricelistId equals qq.Id
                                select new
                                {
                                    PriceListDetailId = q.Id,
                                    PriceListId = q.PricelistId,
                                    Price = q.Price,
                                    Description = qq.Description,
                                    Type = qq.Type
                                };
            //	pricelistFlat.Dump();			

            var filteredOrdersWithEod = db.Order.Where(w => w.EndOfDay == null).Select(q => new
            {
                Id = q.Id,
                EndOfDayId = q.EndOfDayId,
                OrderNo = q.OrderNo,
                StaffId = q.StaffId,
                PdaModuleId = q.PdaModuleId,
                PosId = q.PosId,
                Day = q.Day,
            });
            if (flts.UseEod)
                filteredOrdersWithEod = from q in db.Order
                                        join qq in eods on q.EndOfDayId equals qq.Id
                                        select new
                                        {
                                            Id = q.Id,
                                            EndOfDayId = q.EndOfDayId,
                                            OrderNo = q.OrderNo,
                                            StaffId = q.StaffId,
                                            PdaModuleId = q.PdaModuleId,
                                            PosId = q.PosId,
                                            Day = q.Day,
                                        };

            var filterOrders = from q in filteredOrdersWithEod
                                   //   join qq in eods on q.EndOfDayId equals qq.Id
                               join qqq in posinfos on q.PosId equals qqq.Id
                               select new
                               {
                                   Id = q.Id,
                                   OrderNo = q.OrderNo,
                                   StaffId = q.StaffId,
                                   PdaModuleId = q.PdaModuleId,
                                   PosId = q.PosId,
                                   Day = q.Day,
                                   TimeZone = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? "Breakfast"
                                               : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                               ? "Lunch" : "Dinner",
                               };

            var filteredInvoicesWithEod = db.Invoices.Where(w => (w.IsVoided ?? false == false) && (w.IsDeleted ?? false) == false && w.IsPrinted == true && w.EndOfDayId == null)
                                                      .Select(q => new
                                                      {
                                                          Id = q.Id,
                                                          Description = q.Description,
                                                          IsVoided = q.IsVoided,
                                                          Counter = q.Counter,
                                                          Total = q.Total,
                                                          Discount = q.Discount,
                                                          StaffId = q.StaffId,
                                                          InvoiceTypeId = q.InvoiceTypeId,
                                                          Day = q.Day,
                                                          Cover = q.Cover,
                                                          EndOfDayId = q.EndOfDayId,
                                                          TableId = q.TableId,
                                                          GuestId = q.GuestId,
                                                      });

            if (flts.UseEod)
                filteredInvoicesWithEod = from q in db.Invoices.Where(w => (w.IsVoided ?? false == false) && (w.IsDeleted ?? false) == false && w.IsPrinted == true)
                                          join qq in eods on q.EndOfDayId equals qq.Id
                                          select new
                                          {
                                              Id = q.Id,
                                              Description = q.Description,
                                              IsVoided = q.IsVoided,
                                              Counter = q.Counter,
                                              Total = q.Total,
                                              Discount = q.Discount,
                                              StaffId = q.StaffId,
                                              InvoiceTypeId = q.InvoiceTypeId,
                                              Day = q.Day,
                                              Cover = q.Cover,
                                              EndOfDayId = q.EndOfDayId,
                                              TableId = q.TableId,
                                              GuestId = q.GuestId,
                                          };



            var filteredInvoices = from q in filteredInvoicesWithEod//db.Invoices.Where(w => (w.IsVoided ?? false == false) && (w.IsDeleted ?? false) == false)
                                                                    //join qq in eods on q.EndOfDayId equals qq.Id
                                   join qqq in invTypes on q.InvoiceTypeId equals qqq.Id
                                   select new
                                   {
                                       Id = q.Id,
                                       Description = q.Description,
                                       IsVoided = q.IsVoided,
                                       Counter = q.Counter,
                                       Total = q.Total,
                                       Discount = q.Discount,
                                       StaffId = q.StaffId,
                                       InvoiceTypeId = q.InvoiceTypeId,
                                       InvoiceTypeDescription = qqq.TypeDescription,
                                       Day = q.Day,
                                       Cover = q.Cover,
                                       EndOfDayId = q.EndOfDayId,
                                       TableId = q.TableId,
                                       GuestId = q.GuestId,
                                       Abbr = qqq.Abbr,
                                       InvoiceTypeType = qqq.Type

                                   };
            var posInfoFlat = from q in db.PosInfo//.Where(w => flts.DepartmentList.Contains(w.DepartmentId))
                              join qq in posinfos on q.Id equals qq.Id
                              join qqq in db.Department on q.DepartmentId equals qqq.Id
                              select new
                              {
                                  PosInfoId = q.Id,
                                  PosCode = q.Code,
                                  PosInfoDescription = q.Description,
                                  DepartmentId = q.DepartmentId,
                                  DepartmentDescription = qqq.Description
                              };
            var flatProducts = db.Product.Select(s => new { Id = s.Id, Description = s.SalesDescription, Code = s.Code, ProductCategoryId = s.ProductCategoryId, ProductCategory = s.ProductCategories.Description, Category = s.ProductCategories.Categories.Description, CategoryId = s.ProductCategories.CategoryId });
            var flatIngredients = db.Ingredients.Select(s => new { Id = s.Id, Description = s.SalesDescription, Code = s.Code });



            var flatDetailsOnly = from q in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false /*&& w.Status != 5*/)
                                  join fp in flatProducts on q.ProductId equals fp.Id //into f from fp in flatProducts.FirstOrDefault()
                                  join odva in db.OrderDetailVatAnal on q.Id equals odva.OrderDetailId
                                  join o in filterOrders on q.OrderId equals o.Id
                                  join pl in pricelistFlat on q.PriceListDetailId equals pl.PriceListDetailId
                                  select new
                                  {
                                      OrderDetailId = q.Id,
                                      ProductId = q.ProductId,
                                      ProductCode = fp.Code,
                                      ProductCategory = fp.ProductCategory,
                                      ProductCategoryId = fp.ProductCategoryId,
                                      ProductDescription = fp.Description,
                                      Category = fp.Category,
                                      CategoryId = fp.CategoryId,
                                      VatId = odva.VatId,
                                      VatRate = odva.VatRate,
                                      VatAmount = odva.VatAmount,
                                      Net = odva.Net,
                                      Tax = odva.TaxAmount,
                                      Gross = odva.Gross,
                                      TotalAfterDiscount = q.TotalAfterDiscount,
                                      Discount = q.Discount,
                                      Status = q.Status,
                                      PaidStatus = q.PaidStatus,
                                      PriceListDetailId = q.PriceListDetailId,
                                      OrderId = q.OrderId,
                                      OrderNo = o.OrderNo,
                                      StaffId = o.StaffId,
                                      PdaModuleId = o.PdaModuleId,
                                      PosInfoId = o.PosId,
                                      PricelistDescription = pl.Description,
                                      PricelistId = pl.PriceListId,
                                      TimeZone = ((o.Day.Value.Hour >= 7) && (o.Day.Value.Hour <= 12)) ? "Breakfast"
                                                         : ((o.Day.Value.Hour >= 13) && (o.Day.Value.Hour <= 18))
                                                         ? "Lunch" : "Dinner",
                                      PriceListType = pl.Type ?? 1
                                  };
            //	flatDetails.Where(w=>w.PriceListType == null).Dump();
            var flatDetailsIng = from q in db.OrderDetailIgredients.Where(w => (w.IsDeleted ?? false) == false && w.TotalAfterDiscount != 0)
                                 join ft in flatDetailsOnly on q.OrderDetailId equals ft.OrderDetailId
                                 join fi in flatIngredients on q.IngredientId equals fi.Id
                                 join qq in db.OrderDetailIgredientVatAnal on q.Id equals qq.OrderDetailIgredientsId
                                 select new
                                 {
                                     OrderDetailId = ft.OrderDetailId,
                                     ProductId = q.IngredientId,
                                     ProductCode = fi.Code,
                                     ProductCategory = ft.ProductCategory,
                                     ProductCategoryId = ft.ProductCategoryId,
                                     ProductDescription = fi.Description,
                                     Category = ft.Category,
                                     CategoryId = ft.CategoryId,
                                     VatId = qq.VatId,
                                     VatRate = qq.VatRate,
                                     VatAmount = qq.VatAmount,
                                     Net = qq.Net,
                                     Tax = qq.TaxAmount,
                                     Gross = qq.Gross,
                                     TotalAfterDiscount = q.TotalAfterDiscount,
                                     Discount = q.Discount,
                                     Status = ft.Status,
                                     PaidStatus = ft.PaidStatus,
                                     PriceListDetailId = q.PriceListDetailId,
                                     OrderId = ft.OrderId,
                                     OrderNo = ft.OrderNo,
                                     StaffId = ft.StaffId,
                                     PdaModuleId = ft.PdaModuleId,
                                     PosInfoId = ft.PosInfoId,
                                     PricelistDescription = ft.PricelistDescription,
                                     PricelistId = ft.PricelistId,
                                     TimeZone = ft.TimeZone,
                                     PriceListType = ft.PriceListType

                                 };
            var flatDetails = flatDetailsOnly.Union(flatDetailsIng);
            var orderDetailInvFlat = from q in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false)
                                     join inv in filteredInvoices on q.InvoicesId equals inv.Id
                                     //db.Invoices.Where(w => eods.Contains(w.EndOfDayId) && (w.IsVoided ?? false) == false && (w.IsDeleted ?? false) == false && w.InvoiceTypeId != 3) on 

                                     select new
                                     {
                                         OrderDetailId = q.OrderDetailId,
                                         InvoicesId = q.InvoicesId,
                                         Description = inv.Description,
                                         IsVoided = inv.IsVoided,
                                         Counter = inv.Counter,
                                         Total = inv.Total,
                                         Discount = inv.Discount,
                                         StaffId = inv.StaffId,
                                         InvoiceTypeId = inv.InvoiceTypeId,
                                         Day = inv.Day,
                                         Cover = inv.Cover,
                                         EndOfDayId = inv.EndOfDayId,
                                         TableId = inv.TableId,
                                         RoomId = inv.GuestId,
                                         Abbr = inv.Abbr,
                                         InvoiceTypeType = inv.InvoiceTypeType,
                                         InvoiceTypeDescription = inv.InvoiceTypeDescription
                                     };
            //orderDetailsFlat.Dump();

            var detailsWithMaster = from qq in flatDetails
                                    join q in orderDetailInvFlat on qq.OrderDetailId equals q.OrderDetailId
                                    join qqq in posInfoFlat on qq.PosInfoId equals qqq.PosInfoId
                                    select new
                                    {
                                        PosInfoCode = qqq.PosCode,
                                        DepartmentId = qqq.DepartmentId,
                                        DepartmentDesc = qqq.DepartmentDescription,
                                        OrderDetailId = q.OrderDetailId,
                                        InvoiceId = q.InvoicesId,
                                        Description = q.Description,
                                        IsVoided = q.IsVoided,
                                        Counter = q.Counter,
                                        InvoiceTotal = q.Total,
                                        InvoiceDiscount = q.Discount,
                                        StaffId = q.StaffId,
                                        InvoiceTypeId = q.InvoiceTypeId,
                                        Day = q.Day,
                                        Cover = q.Cover,
                                        EndOfDayId = q.EndOfDayId,
                                        TableId = q.TableId,
                                        RoomId = q.RoomId,



                                        ProductId = qq.ProductId,
                                        ProductCode = qq.ProductCode,
                                        ProductCategory = qq.ProductCategory,
                                        ProductCategoryId = qq.ProductCategoryId,
                                        ProductDescription = qq.ProductDescription,
                                        Category = qq.Category,
                                        CategoryId = qq.CategoryId,
                                        VatId = qq.VatId,
                                        VatRate = qq.VatRate,
                                        VatAmount = qq.VatAmount,
                                        Tax = qq.Tax,
                                        Net = qq.Net,
                                        Gross = qq.Gross,
                                        TotalAfterDiscount = qq.TotalAfterDiscount,
                                        Discount = qq.Discount,
                                        Status = qq.Status,
                                        PaidStatus = qq.PaidStatus,
                                        PriceListDetailId = qq.PriceListDetailId,
                                        OrderId = qq.OrderId,
                                        OrderNo = qq.OrderNo,
                                        DetailStaffId = qq.StaffId,
                                        PdaModuleId = qq.PdaModuleId,
                                        PosInfoId = qq.PosInfoId,
                                        PosInfoDescription = qqq.PosInfoDescription,
                                        PricelistDescription = qq.PricelistDescription,
                                        PricelistId = qq.PricelistId,
                                        TimeZone = qq.TimeZone,
                                        Abbreviation = q.Abbr,
                                        InvoiceType = q.InvoiceTypeType,
                                        Inhouse = q.InvoiceTypeType == (int)InvoiceTypesEnum.Coplimentary,//qq.PriceListType == 2
                                        InvoiceTypeDescription = q.InvoiceTypeDescription
                                    };

            //	detailsWithMaster.Dump();
            switch (flts.ReportType)
            {
                case 9010:
                    var groupedByInv1 = detailsWithMaster.ToList().Distinct().GroupBy(g => new { g.DepartmentId, g.PosInfoId, g.InvoiceId, g.VatId, g.InvoiceType }).Select(s => new
                    {
                        PosInfoCode = s.FirstOrDefault().PosInfoCode,
                        DepartmentId = s.FirstOrDefault().DepartmentId,
                        DepartmentDesc = s.FirstOrDefault().DepartmentDesc,
                        OrderDetailId = s.FirstOrDefault().OrderDetailId,
                        InvoiceId = s.FirstOrDefault().InvoiceId,
                        Description = s.FirstOrDefault().Description,
                        IsVoided = s.FirstOrDefault().IsVoided,
                        Counter = s.FirstOrDefault().Counter,
                        InvoiceTotal = s.FirstOrDefault().InvoiceTotal,
                        InvoiceDiscount = s.FirstOrDefault().Discount,
                        StaffId = s.FirstOrDefault().StaffId,
                        InvoiceTypeId = s.FirstOrDefault().InvoiceTypeId,
                        Day = s.FirstOrDefault().Day,
                        Cover = s.FirstOrDefault().Cover,
                        EndOfDayId = s.FirstOrDefault().EndOfDayId,
                        TableId = s.FirstOrDefault().TableId,
                        RoomId = s.FirstOrDefault().RoomId,

                        ProductCategory = s.FirstOrDefault().ProductCategory,
                        Category = s.FirstOrDefault().Category,
                        VatId = s.FirstOrDefault().VatId,
                        VatRate = s.FirstOrDefault().VatRate,
                        VatAmount = s.Sum(sm => sm.VatAmount),
                        Net = s.Sum(sm => sm.Net),
                        Gross = s.Sum(sm => sm.Gross),
                        TotalAfterDiscount = s.Sum(sm => sm.TotalAfterDiscount),
                        Discount = s.Sum(sm => sm.Discount),
                        PricelistDescription = s.FirstOrDefault().PricelistDescription,
                        PricelistId = s.FirstOrDefault().PricelistId,
                        TimeZone = s.FirstOrDefault().TimeZone,
                        Inhouse = s.FirstOrDefault().Inhouse,
                        Abbreviation = s.FirstOrDefault().Abbreviation,
                        PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
                        InvoiceTypeDescription = s.FirstOrDefault().InvoiceTypeDescription
                        //		PriceListDetailId = qq.PriceListDetailId,
                        //		OrderId = qq.OrderId,	
                        //		OrderNo = qq.OrderNo,
                    });

                    var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
                    return new { processedResults = groupedByInv1.Distinct(), metadata };
                case 9011:
                    var groupedByInv2 = detailsWithMaster.ToList().Distinct().GroupBy(g => new { g.DepartmentId, g.PosInfoId, g.InvoiceId, g.VatId, g.InvoiceType }).Select(s => new
                    {
                        PosInfoCode = s.FirstOrDefault().PosInfoCode,
                        DepartmentId = s.FirstOrDefault().DepartmentId,
                        DepartmentDesc = s.FirstOrDefault().DepartmentDesc,
                        OrderDetailId = s.FirstOrDefault().OrderDetailId,
                        InvoiceId = s.FirstOrDefault().InvoiceId,
                        Description = s.FirstOrDefault().Description,
                        IsVoided = s.FirstOrDefault().IsVoided,
                        Counter = s.FirstOrDefault().Counter,
                        InvoiceTotal = s.FirstOrDefault().InvoiceTotal,
                        InvoiceDiscount = s.FirstOrDefault().Discount,
                        StaffId = s.FirstOrDefault().StaffId,
                        InvoiceTypeId = s.FirstOrDefault().InvoiceTypeId,
                        Day = s.FirstOrDefault().Day,
                        Cover = s.FirstOrDefault().Cover,
                        EndOfDayId = s.FirstOrDefault().EndOfDayId,
                        TableId = s.FirstOrDefault().TableId,
                        RoomId = s.FirstOrDefault().RoomId,

                        ProductCategory = s.FirstOrDefault().ProductCategory,
                        Category = s.FirstOrDefault().Category,
                        VatId = s.FirstOrDefault().VatId,
                        VatRate = s.FirstOrDefault().VatRate,
                        VatAmount = s.Sum(sm => sm.VatAmount),
                        Net = s.Sum(sm => sm.Net),
                        Gross = s.Sum(sm => sm.Gross),
                        TotalAfterDiscount = s.Sum(sm => sm.TotalAfterDiscount),
                        Discount = s.Sum(sm => sm.Discount),
                        PricelistDescription = s.FirstOrDefault().PricelistDescription,
                        PricelistId = s.FirstOrDefault().PricelistId,
                        TimeZone = s.FirstOrDefault().TimeZone,
                        Inhouse = s.FirstOrDefault().Inhouse,
                        Abbreviation = s.FirstOrDefault().Abbreviation,
                        PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
                        InvoiceTypeDescription = s.FirstOrDefault().InvoiceTypeDescription,
                        InvoiceTypeCode = s.FirstOrDefault().InvoiceType
                        //		PriceListDetailId = qq.PriceListDetailId,
                        //		OrderId = qq.OrderId,	
                        //		OrderNo = qq.OrderNo,
                    });

                    var transactionsWitheEod = from q in db.Transactions.Where(w => w.IsDeleted == null && w.EndOfDayId == null)
                                               join qq in db.Invoice_Guests_Trans on q.Id equals qq.TransactionId into f
                                               from igt in f.DefaultIfEmpty()
                                               select new
                                               //.Select(q=> new 
                                               {
                                                   InvoicesId = q.InvoicesId,
                                                   Amount = q.Amount,
                                                   AccountId = q.AccountId,
                                                   StaffId = q.StaffId,
                                                   Room = igt != null ? igt.Guest.Room : "",
                                                   Guest = igt != null ? igt.Guest.LastName : ""
                                               };
                    if (flts.UseEod)
                        transactionsWitheEod = from q in db.Transactions.Where(w => w.IsDeleted == null)
                                               join qq in eods on q.EndOfDayId equals qq.Id
                                               join qqq in db.Invoice_Guests_Trans on q.Id equals qqq.TransactionId into f
                                               from igt in f.DefaultIfEmpty()
                                               select new
                                               {
                                                   InvoicesId = q.InvoicesId,
                                                   Amount = q.Amount,
                                                   AccountId = q.AccountId,
                                                   StaffId = q.StaffId,
                                                   Room = igt != null ? igt.Guest.Room : "",
                                                   Guest = igt != null ? igt.Guest.LastName : ""

                                               };

                    var trtemp = from q in transactionsWitheEod//db.Transactions.Where(w => w.IsDeleted == null)
                                                               //   join qq in eods on q.EndOfDayId equals qq.Id
                                 join qqq in db.Accounts on q.AccountId equals qqq.Id
                                 join qqqq in db.Staff on q.StaffId equals qqqq.Id
                                 select new
                                 {
                                     InvoiceId = q.InvoicesId,
                                     Amount = q.Amount,
                                     AccountId = q.AccountId,
                                     AccountDescription = qqq.Description,
                                     AccountType = qqq.Type,
                                     StaffId = q.StaffId,
                                     StaffLastname = qqqq.LastName,
                                     Room = q.Room,
                                     Guest = q.Guest


                                 };
                    var tr = trtemp.GroupBy(q => new { q.InvoiceId, q.AccountId }).Select(s => new
                    {
                        InvoiceId = s.FirstOrDefault().InvoiceId,
                        Amount = s.Sum(sm => sm.Amount),
                        AccountId = s.FirstOrDefault().AccountId,
                        AccountDescription = s.FirstOrDefault().AccountDescription,
                        AccountType = s.FirstOrDefault().AccountType,
                        StaffId = s.FirstOrDefault().StaffId,
                        StaffLastname = s.FirstOrDefault().StaffLastname,
                        Room = s.FirstOrDefault().Room,
                        Guest = s.FirstOrDefault().Guest
                    });
                    var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
                    var fnal = from q in groupedByInv2
                               join qq in tr on q.InvoiceId equals qq.InvoiceId
                                into f
                               from trans in f.DefaultIfEmpty()
                               select new
                               {
                                   StoreId = storeid,
                                   PosInfoCode = q.PosInfoCode,
                                   DepartmentId = q.DepartmentId,
                                   DepartmentDesc = q.DepartmentDesc,
                                   OrderDetailId = q.OrderDetailId,
                                   InvoiceId = q.InvoiceId,
                                   Description = q.Description,
                                   IsVoided = q.IsVoided,
                                   Counter = q.Counter,
                                   InvoiceTotal = q.InvoiceTotal,
                                   InvoiceDiscount = q.Discount,
                                   StaffId = q.StaffId,
                                   InvoiceTypeId = q.InvoiceTypeId,
                                   Day = q.Day,
                                   Cover = q.Cover,
                                   EndOfDayId = q.EndOfDayId,
                                   TableId = q.TableId,
                                   RoomId = q.RoomId,

                                   ProductCategory = q.ProductCategory,
                                   Category = q.Category,
                                   VatId = q.VatId,
                                   VatRate = q.VatRate,
                                   VatAmount = q.VatAmount,
                                   Net = q.Net,
                                   Gross = q.Gross,
                                   TotalAfterDiscount = q.TotalAfterDiscount,
                                   Discount = q.Discount,
                                   PricelistDescription = q.PricelistDescription,
                                   PricelistId = q.PricelistId,
                                   TimeZone = q.TimeZone,
                                   Inhouse = q.Inhouse,
                                   Abbreviation = q.Abbreviation,
                                   PosInfoDescription = q.PosInfoDescription,
                                   InvoiceTypeDescription = q.InvoiceTypeDescription,
                                   InvoiceTypeCode = q.InvoiceTypeCode,

                                   TransAmount = (trans != null) ? trans.Amount : 0,
                                   AccountId = (trans != null) ? trans.AccountId : -1,
                                   AccountType = (trans != null) ? trans.AccountType : -1,
                                   AccountDescription = (trans != null) ? trans.AccountDescription : "Not Paid",
                                   TransStaffId = (trans != null) ? trans.StaffId : null,
                                   TransStaffName = (trans != null) ? trans.StaffLastname : "",
                                   Room = (trans != null) ? trans.Room : "",
                                   Guest = (trans != null) ? trans.Guest : ""


                               };

                    var metadata1 = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
                    return new { processedResults = fnal.Distinct(), metadata = metadata1 };
            };






            var storeidd = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            var groupedByInv = detailsWithMaster.ToList().GroupBy(g => new { g.InvoiceId, g.ProductCategory, g.Category, g.VatId, g.Inhouse, g.TimeZone }).Select(s => new
            {
                StoreId = storeidd,
                PosInfoCode = s.FirstOrDefault().PosInfoCode,
                DepartmentId = s.FirstOrDefault().DepartmentId,
                DepartmentDesc = s.FirstOrDefault().DepartmentDesc,
                OrderDetailId = s.FirstOrDefault().OrderDetailId,
                InvoiceId = s.FirstOrDefault().InvoiceId,
                Description = s.FirstOrDefault().Description,
                IsVoided = s.FirstOrDefault().IsVoided,
                Counter = s.FirstOrDefault().Counter,
                InvoiceTotal = s.FirstOrDefault().InvoiceTotal,
                InvoiceDiscount = s.FirstOrDefault().Discount,
                StaffId = s.FirstOrDefault().StaffId,
                InvoiceTypeId = s.FirstOrDefault().InvoiceTypeId,
                Day = s.FirstOrDefault().Day,
                Cover = s.FirstOrDefault().Cover,
                EndOfDayId = s.FirstOrDefault().EndOfDayId,
                TableId = s.FirstOrDefault().TableId,
                RoomId = s.FirstOrDefault().RoomId,

                ProductCategory = s.FirstOrDefault().ProductCategory,
                ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                Category = s.FirstOrDefault().Category,
                CategoryId = s.FirstOrDefault().CategoryId,
                VatId = s.FirstOrDefault().VatId,
                VatRate = s.FirstOrDefault().VatRate,
                VatAmount = s.Sum(sm => sm.VatAmount),
                Net = s.Sum(sm => sm.Net),
                Tax = s.Sum(sm => sm.Tax),
                Gross = s.Sum(sm => sm.Gross),
                TotalAfterDiscount = s.Sum(sm => sm.TotalAfterDiscount),
                Discount = s.Sum(sm => sm.Discount),
                PricelistDescription = s.FirstOrDefault().PricelistDescription,
                PricelistId = s.FirstOrDefault().PricelistId,
                TimeZone = s.FirstOrDefault().TimeZone,
                Inhouse = s.FirstOrDefault().Inhouse
                //		PriceListDetailId = qq.PriceListDetailId,
                //		OrderId = qq.OrderId,	
                //		OrderNo = qq.OrderNo,
                //		DetailStaffId = qq.StaffId,
                //		PdaModuleId = qq.PdaModuleId,r
                //		PosInfoId = qq.PosInfoId

            });

            if (flts.ReportType == 1093)
            {
                var metadata93 = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
                return new { processedResults = groupedByInv.Distinct(), metadata = metadata93 };
            }

            var final = groupedByInv.Where(w => w.IsVoided == false || w.IsVoided == null).GroupBy(g => g.DepartmentId).Select(s => new
            {
                DepartmentId = s.FirstOrDefault().DepartmentId,
                DepartmenDescription = s.FirstOrDefault().DepartmentDesc,
                Total = s.Sum(sm => sm.Gross),
                InhouseDetails = s.Where(w => w.Inhouse == false).GroupBy(g => g.Inhouse).Select(s0 => new
                {
                    Inhouse = new
                    {
                        Description = s0.FirstOrDefault().Inhouse,
                        Total = s0.Sum(sm => sm.Gross),
                        Covers = s0.Select(sss => new { inv = sss.InvoiceId, cov = sss.Cover }).Distinct().Sum(sm => sm.cov),
                        NewCovers = s0.Select(sss => new { inv = sss.InvoiceId, cov = sss.Cover, cnt = sss.Counter }).Distinct(),
                        Details = s0.GroupBy(g => g.TimeZone).Select(ss => new
                        {
                            TimeZone = ss.FirstOrDefault().TimeZone,
                            Total = ss.Sum(sm => sm.Gross),
                            Covers = ss.Select(sss => new { inv = sss.InvoiceId, cov = sss.Cover }).Distinct().Sum(sm => sm.cov),
                            Details = ss.GroupBy(g => g.Category).Select(sss => new
                            {
                                CategoryId = sss.FirstOrDefault().Category,
                                Total = sss.Sum(sm => sm.Gross),
                                Details = sss.GroupBy(g => g.ProductCategory).Select(s4 => new
                                {
                                    ProductCategory = s4.FirstOrDefault().ProductCategory,
                                    Total = s4.Sum(sm => sm.Gross),
                                    Vat = s4.Sum(sm => sm.VatAmount),
                                    Tax = s4.Sum(sm => sm.Tax),
                                    Net = s4.Sum(sm => sm.Net),
                                    Discount = s4.Sum(sm => sm.Discount)
                                })
                            })
                        })
                    },
                }),
                CompDetails = s.Where(w => w.Inhouse == true).GroupBy(g => g.Inhouse).Select(s0 => new
                {
                    Inhouse = new
                    {
                        Description = s0.FirstOrDefault().Inhouse,
                        Total = s0.Sum(sm => sm.Gross),
                        Covers = s0.Select(sss => new { inv = sss.InvoiceId, cov = sss.Cover }).Distinct().Sum(sm => sm.cov),
                        Details = s0.GroupBy(g => g.TimeZone).Select(ss => new
                        {
                            TimeZone = ss.FirstOrDefault().TimeZone,
                            Total = ss.Sum(sm => sm.Gross),
                            Covers = ss.Select(sss => new { inv = sss.InvoiceId, cov = sss.Cover }).Distinct().Sum(sm => sm.cov),
                            Details = ss.GroupBy(g => g.Category).Select(sss => new
                            {
                                CategoryId = sss.FirstOrDefault().Category,
                                Total = sss.Sum(sm => sm.Gross),
                                Details = sss.GroupBy(g => g.ProductCategory).Select(s4 => new
                                {
                                    ProductCategory = s4.FirstOrDefault().ProductCategory,
                                    Total = s4.Sum(sm => sm.Gross),
                                    Vat = s4.Sum(sm => sm.VatAmount),
                                    Tax = s4.Sum(sm => sm.Tax),
                                    Net = s4.Sum(sm => sm.Net),
                                    Discount = s4.Sum(sm => sm.Discount)
                                })
                            })
                        })
                    },
                })
            });
            return new { processedResults = final };
        }

        private Object GetAllowedCost(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            //db.Configuration.LazyLoadingEnabled = true;

            var posinfos = db.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId));
            var eods = db.EndOfDay.Where(w => w.FODay >= flts.FromDate.Date && w.FODay <= flts.ToDate.Date);

            var filteredOrdersWithEod = db.Order.Where(w => w.EndOfDay == null).Select(q => new
            {
                EndOfDayId = q.EndOfDayId,
                PosId = q.PosId,
                OrderId = q.Id
            });
            if (flts.UseEod)
                filteredOrdersWithEod = from q in db.Order
                                        join qq in eods on q.EndOfDayId equals qq.Id
                                        select new
                                        {
                                            EndOfDayId = q.EndOfDayId,
                                            PosId = q.PosId,
                                            OrderId = q.Id
                                        };

            var filterOrders = from q in filteredOrdersWithEod
                                   //   join qq in eods on q.EndOfDayId equals qq.Id
                               join qqq in posinfos on q.PosId equals qqq.Id
                               select new
                               {
                                   EndOfDayId = q.EndOfDayId,
                                   PosId = q.PosId,
                                   OrderId = q.OrderId
                               };
            if (flts.DisplayPriceList == null)
            {
                flts.DisplayPriceList = new List<Int64>();
                flts.DisplayPriceList.Add(flts.DisplayPriceListId.Value);
            }


            var catProdCat = from q in db.ProductCategories
                             join qq in db.Categories on q.CategoryId equals qq.Id into f
                             from cat in f.DefaultIfEmpty()
                             join qqq in db.Product on q.Id equals qqq.ProductCategoryId
                             select new
                             {
                                 ProductCategoryId = q.Id,
                                 ProductCategory = q.Description,
                                 CategoryId = q.CategoryId,
                                 Category = q.CategoryId != null ? cat.Description : "No Category",
                                 ProductCode = qqq.Code,
                                 ProductId = qqq.Id,
                                 ProductDescription = qqq.SalesDescription

                             };
            var validpricelistDetailIds = (from q in db.Pricelist
                                           join qq in db.PricelistDetail on q.Id equals qq.PricelistId
                                           join qqq in flts.DisplayPriceList on q.Id equals qqq
                                           select new
                                           {
                                               Id = qq.Id,
                                               PricelistDescription = q.Description
                                           }).Distinct();

            var odis = (from q in db.OrderDetail.Where(w => w.Status != 5 && (w.IsDeleted ?? false) == false)
                        join qq in catProdCat on q.ProductId equals qq.ProductId
                        join qqq in filterOrders on q.OrderId equals qqq.OrderId
                        join qqqq in validpricelistDetailIds on q.PriceListDetailId equals qqqq.Id
                        // join qqq in catProdCat on q.ProductId equals qqq.ProductId
                        select new
                        {
                            OrderDetailId = q.Id,
                            //InvoicesId = qq.InvoicesId,
                            ProductId = qq.ProductId,
                            ProductCode = qq.ProductCode,
                            ProductDescription = qq.ProductDescription,
                            ProductCategoryId = qq.ProductCategoryId,
                            CategoryId = qq.CategoryId,
                            ProductCategory = qq.ProductCategory,
                            Category = qq.Category,
                            DetailTotal = q.TotalAfterDiscount,
                            Qty = q.Qty,
                            PricelistDescription = qqqq.PricelistDescription
                            //	GuestId = qq.GuestId,
                            //	TransactionId = qq.TransactionId
                        })
                       .ToList().GroupBy(g => new { g.ProductId, g.ProductCategoryId, g.CategoryId, g.PricelistDescription }).Select(s => new
                       {
                           OrderDetailId = s.FirstOrDefault().OrderDetailId,
                       //InvoicesId = qq.InvoicesId,
                       ProductId = s.FirstOrDefault().ProductId,
                           ProductCode = s.FirstOrDefault().ProductCode,
                           ProductDescription = s.FirstOrDefault().ProductDescription,
                           ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                           CategoryId = s.FirstOrDefault().CategoryId,
                           ProductCategory = s.FirstOrDefault().ProductCategory,
                           Category = s.FirstOrDefault().Category,
                           DetailTotal = s.Sum(sm => sm.DetailTotal),
                           Qty = s.Sum(sm => sm.Qty),
                           PricelistDescription = s.FirstOrDefault().PricelistDescription
                       });
            var pld = from q in db.PricelistDetail
                      join qq in db.Pricelist.Where(w => flts.CostPriceListId == w.Id) on q.PricelistId equals qq.Id
                      select new
                      {
                          ProductId = q.ProductId,
                          Description = qq.Description,
                          Price = (double)(q.Price != null ? q.Price : 0)
                      };


            var prefinal = (from q in odis
                            join qq in pld on q.ProductId equals qq.ProductId
                            //into f from prl in f.DefaultIfEmpty()
                            select new
                            {
                                Product = q.ProductDescription,
                                Qty = q.Qty,
                                PriceList = q.PricelistDescription,
                                ProductId = q.ProductId,
                                Price = qq.Price,
                                //Price = (double)0,//prl != null ? prl.Price : 0,
                                // Description = prl != null ? prl.Description : "",
                                Description = qq.Description,
                                Cost = qq.Price * q.Qty,
                                //Cost = (double)0,//prl != null ? (Double)q.Qty * (Double)prl.Price : 0,
                                ProductCategoryId = q.ProductCategoryId,
                                ProductCategory = q.ProductCategory,
                                Category = q.Category,
                                CategoryId = q.CategoryId,
                                OrderDetailId = q.OrderDetailId
                            }).ToList().Distinct();


            //var query = from s in db.OrderDetail.Where(w =>  flts.DisplayPriceList.Contains(w.PricelistDetail.PricelistId.Value) /*== */ /*w.Order.PosId == 4 */&& w.Status != 5 && w.IsDeleted == null)
            //            join qq in filterOrders on s.OrderId equals qq.OrderId
            //            select new
            //{
            //    Product = s.Product.SalesDescription,
            //    Qty = (double)(s.Qty != null ? s.Qty : 0),
            //    PriceList = s.PricelistDetail.Pricelist.Description,
            //    ProductId = s.ProductId,
            //    ProductCategory = s.Product.ProductCategoryId != null ? s.Product.ProductCategories.Description : "",
            //    Category = s.Product.ProductCategories != null ?
            //                    s.Product.ProductCategories.Categories != null ?
            //                            s.Product.ProductCategories.Categories.Description
            //                                    : ""
            //                                        : "",
            //};

            //var pld = db.PricelistDetail.Where(w => w.PricelistId == flts.CostPriceListId).Select(s => new
            //{
            //    ProductId = s.ProductId,
            //    Description = s.Pricelist.Description,
            //    Price = (double)(s.Price != null ? s.Price : 0)
            //});

            //var prefinal = (from q in query
            //             join qq in pld on q.ProductId equals qq.ProductId 
            //             //into f from prl in f.DefaultIfEmpty()
            //             select new
            //             {
            //                 Product = q.Product,
            //                 Qty = q.Qty,
            //                 PriceList = q.PriceList,
            //                 ProductId = q.ProductId,
            //                 Price = qq.Price,
            //                 //Price = (double)0,//prl != null ? prl.Price : 0,
            //                // Description = prl != null ? prl.Description : "",
            //                Description = qq.Description,
            //                Cost = qq.Price * q.Qty,
            //                 //Cost = (double)0,//prl != null ? (Double)q.Qty * (Double)prl.Price : 0,
            //                 ProductCategory = q.ProductCategory,
            //                 Category = q.Category
            //             }).ToList();

            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";

            var final = prefinal.GroupBy(g => new { g.PriceList, g.Category, g.ProductCategory, g.ProductId }).Select(s => new
            {
                StoreId = storeid,
                CategoryId = s.FirstOrDefault().CategoryId,
                Category = s.FirstOrDefault().Category,
                ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                ProductCategory = s.FirstOrDefault().ProductCategory,
                Product = s.FirstOrDefault().Product,
                PriceList = s.FirstOrDefault().PriceList,
                ProductId = s.FirstOrDefault().ProductId,
                Description = s.FirstOrDefault().Description,
                Qty = s.Select(sss => new { DetId = sss.OrderDetailId, Qty = s.Max(mx => mx.Qty) }).Distinct().Sum(sm => sm.Qty),
                Price = s.Max(mx1 => mx1.Price),
                Cost = s.Select(sss => new { DetId = sss.OrderDetailId, Qty = s.Max(mx => mx.Qty), Price = s.Max(mx1 => mx1.Price) }).Distinct().Sum(sm => (decimal)(sm.Qty * sm.Price)),


            }).ToList();
            db.Configuration.LazyLoadingEnabled = false;
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);


            return new { processedResults = final, metadata };
        }

        private Object GetComplimentaryRoom(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;


            var posinfos = db.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId));
            var eods = db.EndOfDay.Where(w => w.FODay >= flts.FromDate.Date && w.FODay <= flts.ToDate.Date);


            var filteredInvoicesWithEod = from q in db.Invoices.Where(w => (w.IsVoided ?? false == false) && (w.IsDeleted ?? false) == false && w.IsPrinted == true && w.EndOfDayId == null && (w.InvoiceTypes.Type != 2 || w.InvoiceTypes.Type != 3))
                                          join qq in posinfos on q.PosInfoId equals qq.Id
                                          select new
                                          {
                                              Id = q.Id,
                                              Description = q.Description,
                                              Total = q.Total,
                                              EndOfDayId = q.EndOfDayId,
                                          };

            if (flts.UseEod)
                filteredInvoicesWithEod = from q in db.Invoices.Where(w => (w.IsVoided ?? false == false) && (w.IsDeleted ?? false) == false && w.IsPrinted == true && (w.InvoiceTypes.Type != 2 || w.InvoiceTypes.Type != 3))
                                          join qq in eods on q.EndOfDayId equals qq.Id
                                          join qqq in posinfos on q.PosInfoId equals qqq.Id
                                          select new
                                          {
                                              Id = q.Id,
                                              Description = q.Description,
                                              Total = q.Total,
                                              EndOfDayId = q.EndOfDayId,
                                          };

            var catProdCat = from q in db.ProductCategories
                             join qq in db.Categories on q.CategoryId equals qq.Id into f
                             from cat in f.DefaultIfEmpty()
                             join qqq in db.Product on q.Id equals qqq.ProductCategoryId
                             select new
                             {
                                 ProductCategoryId = q.Id,
                                 ProductCategory = q.Description,
                                 CategoryId = q.CategoryId,
                                 Category = q.CategoryId != null ? cat.Description : "No Category",
                                 //         ProductCode = qqq.Code,
                                 ProductId = qqq.Id,
                                 //         ProductDescription = qqq.SalesDescription

                             };


            var prods = from q in db.OrderDetailInvoices
                        join qq in db.Invoice_Guests_Trans on q.InvoicesId equals qq.InvoiceId
                        join qqq in filteredInvoicesWithEod on q.InvoicesId equals qqq.Id
                        select new
                        {
                            InvoicesId = q.InvoicesId,
                            OrderDetailId = q.OrderDetailId,
                            GuestId = qq.GuestId,
                            TransactionId = qq.TransactionId
                        };
            var odis = from q in db.OrderDetail.Where(w => w.Status != 5 && (w.IsDeleted ?? false) == false)
                       join qq in prods on q.Id equals qq.OrderDetailId
                       join qqq in catProdCat on q.ProductId equals qqq.ProductId
                       select new
                       {
                           InvoicesId = qq.InvoicesId,
                           ProductId = q.ProductId,
                           // ProductCode = qqq.ProductCode,
                           // ProductDescription = qqq.ProductDescription,
                           ProductCategoryId = qqq.ProductCategoryId,
                           CategoryId = qqq.CategoryId,
                           ProductCategory = qqq.ProductCategory,
                           Category = qqq.Category,
                           DetailTotal = q.TotalAfterDiscount,
                           GuestId = qq.GuestId,
                           TransactionId = qq.TransactionId
                       };

            var invByRoom = (from q in odis
                             join qq in db.Transactions on q.TransactionId equals qq.Id
                             join qqq in db.Guest on q.GuestId equals qqq.Id
                             // join qqqq in odis on q.InvoiceId equals qqqq.InvoicesId
                             select new
                             {
                                 //Id = q.Id,
                                 InvoiceId = q.InvoicesId,
                                 Room = qqq.Room,
                                 TransAmount = qq.Amount,
                                 InvoiceAmount = q.DetailTotal,
                                 AccountId = qq.AccountId,
                                 ProductCategoryId = q.ProductCategoryId,
                                 CategoryId = q.CategoryId,
                                 ProductCategory = q.ProductCategory,
                                 //  ProductCode = q.ProductCode,
                                 ProductId = q.ProductId,
                                 //  ProductDescription = q.ProductDescription,
                                 Category = q.Category,
                                 DetailTotal = q.DetailTotal,
                             }).ToList().GroupBy(g => new { g.CategoryId, g.ProductCategoryId, g.ProductId, g.Room, g.AccountId }).Select(s => new
                             {
                                 InvoiceId = s.FirstOrDefault().InvoiceId,
                                 Room = s.FirstOrDefault().Room,
                                 TransAmount = s.Sum(q => q.TransAmount),
                                 InvoiceAmount = s.FirstOrDefault().DetailTotal,
                                 AccountId = s.FirstOrDefault().AccountId,
                                 ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                                 CategoryId = s.FirstOrDefault().CategoryId,
                                 ProductCategory = s.FirstOrDefault().ProductCategory,
                                 // ProductCode = s.FirstOrDefault().ProductCode,
                                 ProductId = s.FirstOrDefault().ProductId,
                                 // ProductDescription = s.FirstOrDefault().ProductDescription,
                                 Category = s.FirstOrDefault().Category,
                                 DetailTotal = s.Sum(q => q.DetailTotal),
                             });
            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            var final = invByRoom.Distinct().ToList().GroupBy(g => new { g.CategoryId, g.ProductCategoryId, g.Room, g.AccountId }).Select(s => new
            {
                StoreId = storeid,
                Room = s.FirstOrDefault().Room,
                AccountId = s.FirstOrDefault().AccountId,
                // AccountDescription = "",//s.FirstOrDefault().AccountDescription,
                CategoryId = s.FirstOrDefault().CategoryId,
                Category = s.FirstOrDefault().Category,
                DetailTotal = s.Distinct().Sum(sm => sm.DetailTotal),
                // ProductCode = s.FirstOrDefault().ProductCode,
                // ProductDescription = s.FirstOrDefault().ProductDescription,
                ProductCategory = s.FirstOrDefault().ProductCategory
            });


            //final.Where(w => w.AccountId == 2).OrderBy(r => r.Room);
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);


            return new { processedResults = final, metadata };

        }



        private Object GetCovers(string filters)
        {
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long?> staffList = new List<long?>();
            int repno = 0;
            if (String.IsNullOrEmpty(filters))
                eod.Add(0);
            else
            {
                var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

                if (flts.UseEod)
                    if (flts.UsePeriod)
                    {
                        eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                                                            EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                    }
                    else
                    {
                        eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                    }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
                staffList.AddRange(flts.StaffList.ToList());
                repno = flts.ReportType;
            }

            var flatProducts = db.Product.Select(s => new { Id = s.Id, Description = s.SalesDescription, Code = s.Code, ProductCategory = s.ProductCategories.Description, Category = s.ProductCategories.Categories.Description });

            var orders = (from q in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false)
                          join pid in db.PosInfoDetail.Where(w => w.IsInvoice == true) on q.PosInfoDetailId equals pid.Id
                          join od in db.OrderDetail.Where(w => w.Status != 5 && (w.IsDeleted ?? false) == false) on q.OrderDetailId equals od.Id
                          join o in db.Order.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && (w.IsDeleted ?? false) == false) on od.OrderId equals o.Id
                          join pd in db.PricelistDetail on od.PriceListDetailId equals pd.Id
                          join eods in db.EndOfDay on o.EndOfDayId equals eods.Id into ff
                          from endofd in ff.DefaultIfEmpty()
                          join po in db.PosInfo.Where(w => posList.Contains(w.Id)) on pid.PosInfoId equals po.Id
                          join dep in db.Department on po.DepartmentId equals dep.Id
                          join tr in db.Transactions on od.TransactionId equals tr.Id into fff
                          from trans in fff.DefaultIfEmpty()
                          join va in db.OrderDetailVatAnal.Where(w => (w.IsDeleted ?? false) == false) on od.Id equals va.OrderDetailId
                          select new
                          {
                              //  FODay = DateTime.Now.Date/*endofd != null ? endofd.FODay : null*/,
                              DepartmentId = dep.Id,
                              DepartmentDescription = dep.Description,
                              Day = q.CreationTS,
                              OrderId = o.Id,
                              OrderDetailId = od.Id,
                              PosInfoDetailId = q.PosInfoDetailId,
                              Abbreviation = pid.Abbreviation,
                              Counter = q.Counter,
                              Total = od.TotalAfterDiscount,
                              ProductId = od.ProductId,
                              PriceListId = pd.PricelistId,
                              PriceListPrice = pd.Price,
                              StaffId = q.StaffId,
                              SalesTypeId = od.SalesTypeId,
                              Qty = od.Qty,
                              GroupId = pid.GroupId,
                              Covers = od.Couver,
                              AccountId = trans.AccountId,
                              Net = od.TotalAfterDiscount == 0 ? 0 : va.Net,
                              Vat = od.TotalAfterDiscount == 0 ? 0 : va.VatAmount,
                              MT = od.TotalAfterDiscount == 0 ? 0 : va.VatAmount != 0 ? va.Net * (Decimal?)0.005 : 0,
                              Discount = od.Discount,
                              IsInhouse = trans.AccountId == 2 || trans.AccountId == 3
                          }).ToList();

            var flat = from s in orders//.OrderBy(o => o.Day)
                       join fp in flatProducts on s.ProductId equals fp.Id

                       select new
                       {
                           FODay = DateTime.Now.Date,//s.FODay,
                           Day = s.Day,
                           PosInfoDetailId = s.PosInfoDetailId,
                           Department = s.DepartmentDescription,
                           GroupId = s.GroupId,
                           OrderId = s.OrderId,
                           OrderDetailId = s.OrderDetailId,
                           Abbreviation = s.Abbreviation,
                           Counter = s.Counter,
                           ProductId = s.ProductId,
                           Cover = s.Covers,
                           Amount = s.Total,
                           Net = s.Total - s.Vat - s.MT,
                           Vat = s.Vat,
                           MT = s.MT,
                           IsInHouse = s.IsInhouse,
                           Discount = s.Vat == 0 ? 0 : (s.PriceListPrice * (Decimal?)s.Qty) - s.Total,
                           Category = fp.Category,
                           Complimentary = s.AccountId == 2,//Account type == 2 to be corrected
                           TimeZoneId = ((s.Day.Value.Hour >= 7) && (s.Day.Value.Hour <= 12)) ? 1
                                       : ((s.Day.Value.Hour >= 13) && (s.Day.Value.Hour <= 18))
                                       ? 2 : ((s.Day.Value.Hour >= 19) && (s.Day.Value.Hour <= 24) || ((s.Day.Value.Hour >= 1) && (s.Day.Value.Hour <= 2))) ? 3 : 4,

                           TimeZone = ((s.Day.Value.Hour >= 7) && (s.Day.Value.Hour <= 12)) ? "Breakfast"
                                       : ((s.Day.Value.Hour >= 13) && (s.Day.Value.Hour <= 18))
                                       ? "Lunch" : "Dinner"
                           //((s.Day.Value.Hour >= 19) && (s.Day.Value.Hour <= 24) || ((s.Day.Value.Hour >= 1) && (s.Day.Value.Hour <= 2))) 
                           // ? "Dinner" : "Overnight",
                       };
            //flat.Dump();
            if (repno == 1092)
            {
                var resInhouse = flat.OrderBy(o => o.FODay).ThenBy(o1 => o1.Department).ThenBy(o2 => o2.TimeZoneId).GroupBy(g => new { g.FODay, g.Department }).Select(s => new
                {
                    Day = s.Key.FODay,
                    DepartmentDescription = s.FirstOrDefault().Department,
                    TotalAmountInHouse = s.Where(w => w.IsInHouse == true && w.Complimentary == false).Distinct().Sum(sm => sm.Amount),
                    TotalAmountInHouseComp = s.Where(w => w.IsInHouse == true && w.Complimentary == true).Distinct().Sum(sm => sm.Amount),
                    TotalAmount = s.Distinct().Sum(sm => sm.Amount),

                    TotalAmountOutHouse = s.Where(w => w.IsInHouse == false).Distinct().Sum(sm => sm.Amount),
                    TotalAmountNoComp = s.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Amount),
                    TotalAmountComp = s.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Amount),
                    //Calculating Cover 
                    CoversComp = s.Where(w => w.Complimentary == true).GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                    CoversNoComp = s.Where(w => w.Complimentary == false).GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                    Covers = s.GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                    InhouseCovers = s.Where(w => w.IsInHouse == true && w.Complimentary == false).GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                    OuthouseCovers = s.Where(w => w.IsInHouse == false).GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),

                    ConsumptionGroupInhouse = s.Where(w => w.Complimentary == false && w.IsInHouse).GroupBy(gg => new { gg.TimeZone }).Select(ss => new
                    {
                        a = s,
                        TimeZone = ss.Key.TimeZone,
                        Covers = ss.GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                        TimeZoneGroup = ss.GroupBy(gg => new { gg.Category }).Select(sss => new
                        {
                            ConsumptionGroup = sss.FirstOrDefault().Category,
                            Amount = sss.Distinct().Sum(sm => sm.Amount),
                            Vat = sss.Distinct().Sum(sm => sm.Vat),
                            MT = sss.Distinct().Sum(sm => sm.MT),
                            Net = sss.Distinct().Sum(sm => sm.Net),
                            Discount = sss.Distinct().Sum(sm => sm.Discount),
                        }),
                        TotalAmount = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Amount),
                        TotalNet = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Net),
                        TotalMT = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.MT),
                        TotalVat = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Vat),
                        TotalDiscount = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Discount)
                    }),
                    ConsumptionGroup = s.Where(w => w.Complimentary == false && !w.IsInHouse).GroupBy(gg => new { gg.TimeZone }).Select(ss => new
                    {
                        TimeZone = ss.Key.TimeZone,
                        Covers = ss.GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                        TimeZoneGroup = ss.GroupBy(gg => new { gg.Category }).Select(sss => new
                        {
                            ConsumptionGroup = sss.FirstOrDefault().Category,
                            Amount = sss.Distinct().Sum(sm => sm.Amount),
                            Vat = sss.Distinct().Sum(sm => sm.Vat),
                            MT = sss.Distinct().Sum(sm => sm.MT),
                            Net = sss.Distinct().Sum(sm => sm.Net),
                            Discount = sss.Distinct().Sum(sm => sm.Discount),
                        }),
                        TotalAmount = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Amount),
                        TotalNet = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Net),
                        TotalMT = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.MT),
                        TotalVat = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Vat),
                        TotalDiscount = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Discount)
                    }),

                    ComplimentaryGroup = s.Where(w => w.Complimentary == true).GroupBy(gg => new { gg.TimeZone }).Select(ss => new
                    {
                        TimeZone = ss.Key.TimeZone,
                        Covers = ss.GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                        TimeZoneGroup = ss.GroupBy(gg => new { gg.Category }).Select(sss => new
                        {
                            ConsumptionGroup = sss.FirstOrDefault().Category,
                            Amount = sss.Distinct().Sum(sm => sm.Amount),
                            Vat = sss.Distinct().Sum(sm => sm.Vat),
                            MT = sss.Distinct().Sum(sm => sm.MT),
                            Net = sss.Distinct().Sum(sm => sm.Net),
                            Discount = sss.Distinct().Sum(sm => sm.Discount),
                        }),
                        TotalAmount = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Amount),
                        TotalNet = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Net),
                        TotalMT = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.MT),
                        TotalVat = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Vat),
                        TotalDiscount = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Discount)
                    }),

                }).GroupBy(gf => gf.Day).Select(sf => new { Day = sf.FirstOrDefault().Day, PerDayGrouping = sf.Select(ssf => ssf), Covers = sf.Sum(sm => sm.Covers), TotalAmount = sf.Sum(sm => sm.TotalAmount) });

                var groupsUsed = flat.Select(s => new { Category = s.Category }).Distinct();

                return new { processedResults = resInhouse, summaryResults = new { groupsUsed }, flatresults = flat };
            }
            else
            {
                var res = flat.OrderBy(o => o.FODay).ThenBy(o1 => o1.Department).ThenBy(o2 => o2.TimeZoneId).GroupBy(g => new { g.FODay, g.Department }).Select(s => new
                {
                    Day = s.Key.FODay,
                    DepartmentDescription = s.FirstOrDefault().Department,
                    TotalAmount = s.Distinct().Sum(sm => sm.Amount),
                    TotalAmountNoComp = s.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Amount),
                    TotalAmountComp = s.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Amount),
                    TotalDiscount = s.Distinct().Sum(sm => sm.Discount),
                    //Calculating Covers
                    CoversComp = s.Where(w => w.Complimentary == true).GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                    CoversNoComp = s.Where(w => w.Complimentary == false).GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                    Covers = s.GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                    ConsumptionGroup = s.Where(w => w.Complimentary == false).GroupBy(gg => new { gg.TimeZone }).Select(ss => new
                    {
                        TimeZone = ss.Key.TimeZone,
                        Covers = ss.GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                        TimeZoneGroup = ss.GroupBy(gg => new { gg.Category }).Select(sss => new
                        {
                            ConsumptionGroup = sss.FirstOrDefault().Category,
                            Amount = sss.Distinct().Sum(sm => sm.Amount),
                            Vat = sss.Distinct().Sum(sm => sm.Vat),
                            MT = sss.Distinct().Sum(sm => sm.MT),
                            Net = sss.Distinct().Sum(sm => sm.Net),
                            Discount = sss.Distinct().Sum(sm => sm.Discount),
                        }),
                        TotalAmount = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Amount),
                        TotalNet = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Net),
                        TotalMT = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.MT),
                        TotalVat = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Vat),
                        TotalDiscount = ss.Distinct().Where(w => w.Complimentary == false).Sum(sm => sm.Discount)
                    }),
                    ComplimentaryGroup = s.Where(w => w.Complimentary == true).GroupBy(gg => new { gg.TimeZone }).Select(ss => new
                    {
                        TimeZone = ss.Key.TimeZone,
                        Covers = ss.GroupBy(gc => new { gc.OrderId }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover),
                        TimeZoneGroup = ss.GroupBy(gg => new { gg.Category }).Select(sss => new
                        {
                            ConsumptionGroup = sss.FirstOrDefault().Category,
                            Amount = sss.Distinct().Sum(sm => sm.Amount),
                            Vat = sss.Distinct().Sum(sm => sm.Vat),
                            MT = sss.Distinct().Sum(sm => sm.MT),
                            Net = sss.Distinct().Sum(sm => sm.Net),
                            Discount = sss.Distinct().Sum(sm => sm.Discount),
                        }),
                        TotalAmount = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Amount),
                        TotalNet = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Net),
                        TotalMT = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.MT),
                        TotalVat = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Vat),
                        TotalDiscount = ss.Distinct().Where(w => w.Complimentary == true).Sum(sm => sm.Discount)
                    }),

                }).GroupBy(gf => gf.Day).Select(sf => new { Day = sf.FirstOrDefault().Day, PerDayGrouping = sf.Select(ssf => ssf), Covers = sf.Sum(sm => sm.Covers), TotalAmount = sf.Sum(sm => sm.TotalAmount) });
                return new { processedResults = res, flatresults = flat.Distinct() };
            }



            //var salesFlat = GetFlatSales(filters);
            ////apo deltia paraggelias
            //var flat = salesFlat.Where(w => w.GroupId == 2 && w.Status != 5).OrderBy(o => o.Day).Select(s => new
            //{
            //    FODay = s.FODay,
            //    Day = s.Day,
            //    PosInfoDetailId = s.PosInfoDetailId,
            //    Department = s.DepartmentDescription,
            //    GroupId = s.GroupId,
            //    OrderDetailId = s.OrderDetailId,
            //    Abbreviation = s.Abbreviation,
            //    Counter = s.Counter,
            //    ProductId = s.ProductId,
            //    ProductDescription = s.ProductDescription,
            //    Cover = s.Cover,
            //    Amount = s.TotalAfterDiscount,
            //    Category = s.CategoryDescription,
            //    //			RelativeInvoices= s.RelativeInvoices,
            //    TimeZoneId = ((s.Day.Hour >= 7) && (s.Day.Hour <= 12)) ? 1
            //                : ((s.Day.Hour >= 13) && (s.Day.Hour <= 18))
            //                ? 2 : ((s.Day.Hour >= 19) && (s.Day.Hour <= 24) || ((s.Day.Hour >= 1) && (s.Day.Hour <= 2))) ? 3 : 4,

            //    TimeZone = ((s.Day.Hour >= 7) && (s.Day.Hour <= 12)) ? "Breakfast"
            //                : ((s.Day.Hour >= 13) && (s.Day.Hour <= 18))
            //                ? "Lunch" : ((s.Day.Hour >= 19) && (s.Day.Hour <= 24) || ((s.Day.Hour >= 1) && (s.Day.Hour <= 2))) ? "Dinner" : "Overnight",
            //}).ToList();



            ////				TimeZoneId = s.FirstOrDefault().TimeZoneId,
            ////				TimeZone = s.FirstOrDefault().TimeZone,
            ////				GroupedTimeZone = s.Select(ss=> ss)


            //var res = flat.OrderBy(o => o.FODay).ThenBy(o1 => o1.Department).ThenBy(o2 => o2.TimeZoneId).GroupBy(g => new { g.FODay, g.Department }).Select(s => new
            //{
            //    Day = s.Key.FODay,
            //    DepartmentDescription = s.FirstOrDefault().Department,
            //    TotalAmount = s.Distinct().Sum(sm => (Decimal?)sm.Amount),
            //    Covers = s.GroupBy(gc => new { gc.Counter, gc.PosInfoDetailId, gc.Abbreviation }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => (Int64?)sm.Cover),
            //    ConsumptionGroup = s.GroupBy(gg => new { gg.TimeZone }).Select(ss => new
            //    {
            //        TimeZone = ss.Key.TimeZone,
            //        Covers = ss.GroupBy(gc => new { gc.Counter, gc.PosInfoDetailId, gc.Abbreviation }).Distinct().Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => (Int64?)sm.Cover),

            //       // Coversa = ss.GroupBy(gc => new { gc.Counter, gc.PosInfoDetailId, gc.Abbreviation }).Select(sc => new { Abbreviation = sc.FirstOrDefault().Abbreviation, Counter = sc.Key.Counter, Cover = (Int64?)sc.FirstOrDefault().Cover }),
            //        //	Coversa = ss.GroupBy(gc => new {gc.Counter, gc.PosInfoDetailId } ).Select(sc=> new {Counter = sc.FirstOrDefault().Counter, Cover = sc.FirstOrDefault().Cover}),
            //        TimeZoneGroup = ss.GroupBy(gg => new { gg.Category }).Select(sss => new
            //        {
            //            //	CategoryId = sss.FirstOrDefault().CategoryId,
            //            ConsumptionGroup = sss.FirstOrDefault().Category,
            //            Amount = sss.Distinct().Sum(sm => (Decimal?)sm.Amount),
            //            //	aa = sss.Distinct()
            //        }),
            //        TotalAmount = ss.Distinct().Sum(sm => (Decimal?)sm.Amount)
            //        //gg.Abbreviation, gg.TimeZone, gg.Counter, gg.PosInfoDetailId,
            //    })
            //}).GroupBy(gf => gf.Day).Select(sf => new { Day = sf.FirstOrDefault().Day, PerDayGrouping = sf.Select(ssf => ssf) });

            //me apl
            //var query = salesByReceipt.Where(w => w.IsInvoice == true).ToList().Select(s => new
            //{
            //    Day = s.FODay.Date != null ? s.FODay : s.Day.Date,
            //    DepartmentId = s.DepartmentId,
            //    Abbreviation = s.Abbreviation,
            //    Counter = s.Counter,
            //    PosInfoDetailId = s.PosInfoDetailId,
            //    DepartmentDescription = s.DepartmentDescription,
            //    //Time = s.Day,
            //    TimeZone = ((s.Day.Hour >= 7) && (s.Day.Hour <= 12)) ? "1. Breakfast"
            //                : ((s.Day.Hour >= 13) && (s.Day.Hour <= 18))
            //                ? "2. Lunch" : "3. Dinner",
            //    Cover = s.Cover,
            //    OrderDetailId = s.OrderDetailId,
            //    Category = s.CategoryDescription,
            //    CategoryId = s.CategoryId,
            //    Amount = s.TotalAfterDiscount
            //});

            //var res = query.OrderBy(o => o.Day).ThenBy(o1 => o1.DepartmentDescription).ThenBy(o2 => o2.TimeZone).GroupBy(g => new { g.Day, g.DepartmentId }).Select(s => new
            //{
            //    Day = s.Key.Day,
            //    DepartmentDescription = s.FirstOrDefault().DepartmentDescription,
            //    ConsumptionGroup = s.GroupBy(gg => new { gg.TimeZone }).Select(ss => new
            //    {
            //        TimeZone = ss.Key.TimeZone,
            //        Covers = ss.GroupBy(gc => new { gc.Counter, gc.PosInfoDetailId }).Select(sc => new { Cover = sc.FirstOrDefault().Cover }).Sum(sm => sm.Cover ?? 0),
            //        //	Coversa = ss.GroupBy(gc => new {gc.Counter, gc.PosInfoDetailId } ).Select(sc=> new {Counter = sc.FirstOrDefault().Counter, Cover = sc.FirstOrDefault().Cover}),
            //        TimeZoneGroup = ss.GroupBy(gg => new { gg.Category }).Select(sss => new
            //        {
            //            CategoryId = sss.FirstOrDefault().CategoryId,
            //            ConsumptionGroup = sss.FirstOrDefault().Category,
            //            Amount = sss.Distinct().Sum(sm => (Decimal?)sm.Amount),
            //            //	aa = sss.Distinct()
            //        })
            //        //gg.Abbreviation, gg.TimeZone, gg.Counter, gg.PosInfoDetailId,
            //    })
            //}).GroupBy(gf => gf.Day).Select(sf => new { Day = sf.FirstOrDefault().Day, PerDayGrouping = sf.Select(ssf => ssf) });
            //var 

            return null;
        }


        private Object EndOfDayProductSalesComparer(string filters)
        {
            // var salesByReceipt = GetFlatSales(filters).Distinct().ToList();
            return HitUtilsHelper.EndOfDayProductSalesComparer(db, filters);

        }
        private Object GetXReport(string filters)
        {
            // var salesByReceipt = GetFlatSales(filters).Distinct().ToList();
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var piid = flts.PosList.FirstOrDefault();
            db.Configuration.LazyLoadingEnabled = true;

            var posdata = db.PosInfo.Where(f => f.Id == piid).FirstOrDefault();
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();

            eod.Add(flts.EodId ?? 0);
            posList.Add(posdata.Id);
            var salesByReceipt = db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId.Value)
                                    && (w.IsVoided ?? false) == false
                                    && (w.IsDeleted ?? false) == false
                                    && w.InvoiceTypeId != null && w.InvoiceTypes.Type != 2 && w.InvoiceTypes.Type != 3);

            var odIngVats = (from q in db.OrderDetailIgredientVatAnal
                             join od in db.OrderDetailIgredients on q.OrderDetailIgredientsId equals od.Id
                             join odi in db.OrderDetailInvoices on od.OrderDetailId equals odi.OrderDetailId
                             join inv in salesByReceipt on odi.InvoicesId equals inv.Id//Invoices.Where(w => (eod.Contains(w.EndOfDayId ?? 0))  && posList.Contains(w.PosInfoId.Value) ) on odi.InvoicesId equals inv.Id
                             select new
                             {
                                 InvoiceId = inv.Id,
                                 Id = q.Id,
                                 OrderDetailId = odi.Id,
                                 Gross = q.Gross,
                                 VatId = q.VatId,
                                 Net = q.Net,
                                 VatRate = q.VatRate,
                                 VatAmount = q.VatAmount
                             }).ToList().Select(s => new
                             {

                                 InvoiceId = s.Id,
                                 Id = s.Id,
                                 OrderDetailId = s.Id,
                                 Total = Math.Round((decimal)s.Gross, 2, MidpointRounding.AwayFromZero),
                                 VatId = s.VatId,
                                 Net = Math.Round((decimal)s.Net, 2, MidpointRounding.AwayFromZero),
                                 VatRate = s.VatRate,
                                 VatAmount = Math.Round((decimal)s.VatAmount, 2, MidpointRounding.AwayFromZero)

                             });

            var odVats = (from q in db.OrderDetailVatAnal
                          join od in db.OrderDetail on q.OrderDetailId equals od.Id
                          join odi in db.OrderDetailInvoices on od.Id equals odi.OrderDetailId
                          join inv in salesByReceipt on odi.InvoicesId equals inv.Id//Invoices.Where(w => (eod.Contains(w.EndOfDayId ?? 0))  && posList.Contains(w.PosInfoId.Value) ) on odi.InvoicesId equals inv.Id
                          select new
                          {
                              InvoiceId = inv.Id,
                              Id = q.Id,
                              OrderDetailId = odi.Id,
                              Gross = q.Gross,
                              VatId = q.VatId,
                              Net = q.Net,
                              VatRate = q.VatRate,
                              VatAmount = q.VatAmount
                          }).ToList().Select(s => new
                          {

                              InvoiceId = s.Id,
                              Id = s.Id,
                              OrderDetailId = s.Id,
                              Total = Math.Round((decimal)s.Gross, 2, MidpointRounding.AwayFromZero),
                              VatId = s.VatId,
                              Net = Math.Round((decimal)s.Net, 2, MidpointRounding.AwayFromZero),
                              VatRate = s.VatRate,
                              VatAmount = Math.Round((decimal)s.VatAmount, 2, MidpointRounding.AwayFromZero)

                          });
            var joinedVats = odVats.Union(odIngVats).ToList().GroupBy(g => g.InvoiceId)
                                                    .Select(s => new
                                                    {
                                                        InvoiceId = s.Key,
                                                        Vats = s.GroupBy(gg => gg.VatId).Select(sss => new
                                                        {
                                                            VatId = sss.Key,
                                                            Total = sss.Sum(sm => sm.Total),
                                                            Net = sss.Sum(sm => sm.Net),
                                                            VatRate = sss.FirstOrDefault().VatRate,
                                                            VatAmount = sss.Sum(sm => sm.VatAmount)
                                                        })
                                                    });

            var payments = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).SelectMany(s => s.Transactions).ToList();
            var voided = db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId.Value)
                                    && (w.IsVoided ?? false) == true
                                    && (w.IsDeleted ?? false) == false
                                    && w.InvoiceTypeId != null && w.InvoiceTypes.Type != 2 && w.InvoiceTypes.Type != 3).SelectMany(s => s.Transactions).ToList();
            var vats = joinedVats.SelectMany(sm => sm.Vats).ToList();

            decimal? lockerPrice = 1;

            var totalLockers = db.Transactions.Where(w => w.EndOfDayId == null
                                                     && w.PosInfoId == piid
                                                     && (w.TransactionType == (int)TransactionTypesEnum.OpenLocker
                                                     || w.TransactionType == (int)TransactionTypesEnum.CloseLocker));
            decimal? openLocker = totalLockers.Where(w => w.TransactionType == (int)TransactionTypesEnum.OpenLocker).Sum(sm => sm.Amount) ?? 0;
            decimal? paidLocker = (totalLockers.Where(w => w.TransactionType == (int)TransactionTypesEnum.CloseLocker).Sum(sm => sm.Amount) * -1) ?? 0;




            var barcodes = db.Transactions.Where(w => w.PosInfoId == piid
                                                 && w.EndOfDayId == null
                                                 && w.TransactionType == (int)TransactionTypesEnum.CreditCode
                                                 && (w.IsDeleted ?? false) == false
                                                 ).Sum(sm => sm.Amount) ?? 0;
            var rlp = db.RegionLockerProduct.FirstOrDefault();
            if (rlp != null)
                lockerPrice = rlp.Price;



            var prods = db.Invoices.AsNoTracking()
                                   .Include("OrderDetailInvoices.OrderDetail")
                                   .Where(w => w.EndOfDayId == null && w.PosInfoId == piid && (w.IsDeleted ?? false) == false && (w.IsVoided ?? false) == false)
                                   .SelectMany(w => w.OrderDetailInvoices).Select(s => new
                                   {
                                       ProductId = s.OrderDetail.ProductId,
                                       Qty = s.OrderDetail.Qty,
                                       Total = s.OrderDetail.TotalAfterDiscount
                                   });

            var productsForEODStats = (from q in db.ProductsForEOD
                                       join qq in prods on q.ProductId equals qq.ProductId
                                       select new
                                       {
                                           ProductId = q.ProductId,
                                           Description = q.Description,
                                           Qty = qq.Qty,
                                           Total = qq.Total
                                       }).GroupBy(g => g.ProductId).Select(s => new
                                       {
                                           ProductId = s.FirstOrDefault().ProductId,
                                           Description = s.FirstOrDefault().Description,
                                           Qty = s.Sum(sm => sm.Qty),
                                           Total = s.Sum(sm => sm.Total)
                                       });

            var xDataToPrint = new
            {
                Day = posdata.FODay,
                PosCode = posdata.Id,//Allazei sto webpos
                PosDescription = posdata.Description,
                ReportNo = 0,
                Gross = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => (decimal?)sm.Total), 2),
                VatAmount = Math.Round((decimal)(decimal)vats.Sum(sm => sm.VatAmount), 2),
                Net = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => (decimal?)sm.Total), 2) - Math.Round((decimal)(decimal)vats.Sum(sm => sm.VatAmount), 2),
                //Math.Round((decimal)vats.Sum(sm => sm.Net), 2),
                Discount = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => sm.Discount != null ? (decimal)sm.Discount : 0), 2),
                TicketCount = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Count(),
                ItemsCount = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).SelectMany(s => s.OrderDetailInvoices).Sum(sm => sm.OrderDetail.Qty),

                PaymentAnalysis = payments.Where(a => a.Accounts.Type != 4).ToList()
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                             Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Amount), 2)
                                         }),
                VatAnalysis = vats.GroupBy(q => q.VatId).ToList()
                                  .Select(w => new
                                  {
                                      VatRate = w.FirstOrDefault().VatRate,
                                      Gross = Math.Round((decimal)w.Sum(r => (decimal?)r.Total), 2),
                                      VatAmount = Math.Round((decimal)w.Sum(r => (decimal?)r.VatAmount), 2),
                                  // Net = Math.Round((decimal)w.Sum(r => (decimal?)r.Net), 2)// - (decimal)w.Sum(r => (decimal?)r.Net)
                                  Net = Math.Round((decimal)w.Sum(r => (decimal?)r.Total), 2) - Math.Round((decimal)w.Sum(r => (decimal?)r.VatAmount), 2)
                                  }),
                VoidAnalysis = voided//Where(f => f.Void == true && f.IsInvoice == true)
                                       .GroupBy(q => q.AccountId)
                                       .Select(w => new
                                       {
                                           Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                           Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Amount), 2, MidpointRounding.AwayFromZero)
                                       }),
                CardAnalysis = payments.Where(a => a.Accounts.Type == 4).ToList()//salesByReceipt.Where(f => f.AccountType == 4 && f.Void == false && f.IsCancel == false && f.IsInvoice == true)
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                             Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Amount), 2, MidpointRounding.AwayFromZero)
                                         }),
                Barcodes = barcodes,
                Lockers = new
                {
                    HasLockers = rlp != null,
                    TotalLockers = openLocker / lockerPrice,
                    TotalLockersAmount = openLocker,
                    Paidlockers = (paidLocker / lockerPrice) ?? 0,
                    PaidlockersAmount = paidLocker,
                    OccLockers = (openLocker / lockerPrice) - (paidLocker / lockerPrice),
                    OccLockersAmount = openLocker - paidLocker
                },
                ProductsForEODStats = productsForEODStats
            };
            //xDataToPrint.Dump();
            //var xDataToPrint = new
            //{
            //    Day = posdata.FODay,
            //    PosCode = posdata.Id,//Allazei sto webpos
            //    PosDescription = posdata.Description,
            //    ReportNo = 0,
            //    Gross = Math.Round((decimal)salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true).Sum(sm => (decimal?)sm.TotalAfterDiscount), 2),
            //    VatAmount = Math.Round((decimal)salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true).Sum(sm => (decimal?)sm.VatAmount), 2),
            //    Net = Math.Round((decimal)salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true).Sum(sm => (decimal?)sm.TotalAfterDiscount)
            //                - (decimal)salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true).Sum(sm => (decimal?)sm.VatAmount), 2),
            //    Discount = Math.Round((decimal)salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true).Sum(sm => sm.Discount != null ? (decimal)sm.Discount : 0), 2),
            //    TicketCount = salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true).GroupBy(f => f.Counter).Count(),
            //    ItemsCount = salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true).Count(),

            //    PaymentAnalysis = salesByReceipt.Where(f => f.AccountType != 4 && f.Void == false && f.IsCancel == false && f.IsInvoice == true)
            //                     .GroupBy(f => f.AccountId)
            //                     .Select(w => new
            //                     {
            //                         Description = w.FirstOrDefault().AccountDescription,
            //                         Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.TotalAfterDiscount), 2)
            //                     }),
            //    VatAnalysis = salesByReceipt.Where(f => f.Void == false && f.IsCancel == false && f.IsInvoice == true)
            //                           .GroupBy(q => q.VatId)
            //                           .Select(w => new
            //                           {
            //                               VatRate = w.FirstOrDefault().VatRate,
            //                               Gross = Math.Round((decimal)w.Sum(r => (decimal?)r.VatGross), 2),
            //                               VatAmount = Math.Round((decimal)w.Sum(r => (decimal?)r.VatAmount), 2),
            //                               Net = Math.Round((decimal)w.Sum(r => (decimal?)r.VatGross), 2) - Math.Round((decimal)w.Sum(r => (decimal?)r.VatAmount), 2)
            //                           }),
            //    VoidAnalysis = salesByReceipt.Where(f => f.Void == true && f.IsInvoice == true)
            //                   .GroupBy(q => q.AccountId)
            //                   .Select(w => new
            //                   {
            //                       Description = w.FirstOrDefault().AccountDescription,
            //                       Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.TotalAfterDiscount), 2)
            //                   }),
            //    CardAnalysis = salesByReceipt.Where(f => f.AccountType == 4 && f.Void == false && f.IsCancel == false && f.IsInvoice == true)
            //                     .GroupBy(f => f.AccountId)
            //                     .Select(w => new
            //                     {
            //                         Description = w.FirstOrDefault().AccountDescription,
            //                         Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.TotalAfterDiscount), 2)
            //                     }),
            //};


            db.Configuration.LazyLoadingEnabled = false;
            return new { xDataToPrint = xDataToPrint };

        }

        private Object GetReportByWaiter(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var piid = flts.PosList.FirstOrDefault();
            db.Configuration.LazyLoadingEnabled = true;

            var posdata = db.PosInfo.Where(f => f.Id == piid).FirstOrDefault();
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long?> staffList = new List<long?>();

            eod.Add(0);
            posList.Add(posdata.Id);
            staffList.AddRange(flts.StaffList);

            var salesByReceipt = db.Invoices.Where(w => staffList.Contains(w.StaffId) && eod.Contains((w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId.Value) && (w.IsVoided ?? false) == false && w.InvoiceTypeId != null && w.InvoiceTypes.Type != 2 && w.InvoiceTypes.Type != 3);



            var payments = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).SelectMany(s => s.Transactions).ToList().Where(w => staffList.Contains(w.StaffId));
            var voidpayments = salesByReceipt.Where(f => (f.IsVoided ?? false) == true).SelectMany(s => s.Transactions).Where(w => staffList.Contains(w.StaffId));




            var xDataToPrint = new
            {
                Day = posdata.FODay,
                PosCode = posdata.Id,//Allazei sto webpos
                PosDescription = posdata.Description,
                ReportNo = 0,
                Gross = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => (decimal?)sm.Total), 2),
                Discount = Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => sm.Discount != null ? (decimal)sm.Discount : 0), 2),
                TicketCount = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Count(),
                ItemsCount = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).SelectMany(s => s.OrderDetailInvoices).Count(),
                VoidsPerAccountWaiter = voidpayments.ToList().GroupBy(gw => gw.StaffId).Select(sss => new
                {
                    StaffId = sss.Key,
                    StaffName = sss.FirstOrDefault().Staff == null ? sss.FirstOrDefault().StaffId.ToString() : sss.FirstOrDefault().Staff.LastName,
                    PerAccount = sss.GroupBy(g => g.AccountId).Select(ss => new
                    {
                        Description = ss.FirstOrDefault().Accounts.Description,
                        Tickets = ss.GroupBy(gg => gg.Id).Count(),
                        Total = Math.Round((decimal)ss.Sum(sm => (Decimal?)sm.Amount), 2, MidpointRounding.AwayFromZero)
                    })
                }),
                TotalsByAccountWaiter = payments.ToList().GroupBy(gw => gw.StaffId).Select(sss => new
                {
                    StaffId = sss.Key,
                    StaffName = sss.FirstOrDefault().Staff == null ? sss.FirstOrDefault().StaffId.ToString() : sss.FirstOrDefault().Staff.LastName,
                    PerAccount = sss.GroupBy(g => g.AccountId).Select(ss => new
                    {
                        Description = ss.FirstOrDefault().Accounts.Description,
                        Tickets = ss.GroupBy(gg => gg.Id).Count(),
                        Total = Math.Round((decimal)ss.Sum(sm => (Decimal?)sm.Amount), 2, MidpointRounding.AwayFromZero)
                    })
                }),
                PaymentAnalysis = payments.Where(a => a.Accounts.Type != 4).ToList()
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                             Amount = w.Sum(r => (decimal?)r.Amount)
                                         }),

                VoidAnalysis = salesByReceipt.Where(f => (f.IsVoided ?? false) == true).SelectMany(s => s.Transactions).ToList()//Where(f => f.Void == true && f.IsInvoice == true)
                                       .GroupBy(q => q.AccountId)
                                       .Select(w => new
                                       {
                                           Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                           Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Amount), 2, MidpointRounding.AwayFromZero)
                                       }),
                CardAnalysis = payments.Where(a => a.Accounts.Type == 4).ToList()//salesByReceipt.Where(f => f.AccountType == 4 && f.Void == false && f.IsCancel == false && f.IsInvoice == true)
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                             Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Amount), 2, MidpointRounding.AwayFromZero)
                                         }),
            };

            #region oldWaiterReport
            /*
            var salesByReceipt = GetFlatSales(filters).Distinct();
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            var piid = flts.PosList.FirstOrDefault();
            var ssds = salesByReceipt.Where(f => f.IsInvoice == true && f.Void == false && f.PosInfoId == piid && f.AccountId == 4);
            var xReport = new
            {
                TotalsSalesPerAccount = salesByReceipt.Where(w => w.IsInvoice == true && w.Void == false && w.PosInfoId == piid).GroupBy(g => g.AccountType).Select(ss => new
                {
                    Description = ss.FirstOrDefault().AccountDescription,
                    Tickets = ss.Count(),
                    Total = ss.Sum(sm => (Decimal?)sm.TotalAfterDiscount)
                }),
                TotalsIncomePerAccount = salesByReceipt.Where(w => w.IsInvoice == true && w.Void == false && w.PosInfoId == piid).GroupBy(g => g.AccountType).Select(ss => new
                {
                    Description = ss.FirstOrDefault().AccountDescription,
                    Tickets = ss.Count(),
                    Total = ss.Sum(sm => (Decimal?)sm.TransactionAmount)
                }),
                TotalsByAccountWaiter = salesByReceipt.Where(w => w.IsInvoice == true && w.Void == false && w.PosInfoId == piid).GroupBy(gw => gw.StaffId).Select(sss => new
                {
                    StaffId = sss.Key,
                    StaffName = sss.FirstOrDefault().StaffName,
                    PerAccount = sss.GroupBy(g => g.AccountType).Select(ss => new
                    {
                        Description = ss.FirstOrDefault().AccountDescription,
                        Tickets = ss.GroupBy(g => g.Counter).Count(),
                        Total = Math.Round((decimal)ss.Sum(sm => (Decimal?)sm.TotalAfterDiscount), 2)
                    })
                }),
                VoidsPerAccount = salesByReceipt.Where(w => w.IsInvoice == true && w.Void == true && w.PosInfoId == piid).GroupBy(g => g.AccountType).Select(ss => new
                {
                    Description = ss.FirstOrDefault().AccountDescription,
                    Tickets = ss.Count(),
                    Total = ss.Sum(sm => (Decimal?)sm.TotalAfterDiscount)
                }),
                VoidsPerAccountWaiter = salesByReceipt.Where(w => w.IsInvoice == true && w.Void == true && w.PosInfoId == piid).GroupBy(gw => gw.StaffId).Select(sss => new
                {
                    StaffId = sss.Key,
                    StaffName = sss.FirstOrDefault().StaffName,
                    PerAccount = sss.GroupBy(g => g.AccountType).Select(ss => new
                    {
                        Description = ss.FirstOrDefault().AccountDescription,
                        Tickets = ss.GroupBy(g => g.Counter).Count(),
                        Total = Math.Round((decimal)ss.Sum(sm => (Decimal?)sm.TotalAfterDiscount), 2)
                    })
                }),
                //VatAnalysis = salesByReceipt.Where(w => w.IsInvoice == true && w.Void == false && w.PosInfoId == piid).SelectMany(sss => sss.Vats)
                //.GroupBy(g => g.VatId).Select(ss => new
                //{
                //    Description = ss.FirstOrDefault().VatRate,
                //    Tickets = ss.Count(),
                //    Total = ss.Sum(sm => sm.VatGross),
                //    VatAmount = ss.Sum(sm => sm.VatGross),
                //    Net = ss.Sum(sm => sm.VatNet),
                //}),
                Cover = salesByReceipt.Where(w => w.IsInvoice == true && w.Void == false && w.PosInfoId == piid).Sum(sm => sm.Cover != null ? sm.Cover : 0)
            };
             * */
            #endregion

            db.Configuration.LazyLoadingEnabled = false;
            return new { processedResults = xDataToPrint };
        }

        /// <summary>
        /// Returns Ticket number per account and void ticket count per account
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        private Object GetReportTicketCount(string filters)
        {
            var salesByReceipt = GetFlatSales(filters).Distinct();
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var piid = flts.PosList.FirstOrDefault();

            var posdata = db.PosInfo.Include("Department").Where(f => f.Id == piid).FirstOrDefault();


            var ticketsPerAccount = salesByReceipt.Where(f => f.Void == false && f.IsInvoice == true)
                                   .GroupBy(g => g.AccountId)
                                   .Select(q => new
                                   {

                                       Description = q.FirstOrDefault().AccountDescription,
                                       TicketsCount = q.GroupBy(w => w.Counter).Count()

                                   });

            var voidTicketsPerAccount = salesByReceipt.Where(f => f.Void == true && f.IsInvoice == true)
                                  .GroupBy(g => g.AccountId)
                                  .Select(q => new
                                  {

                                      Description = q.FirstOrDefault().AccountDescription,
                                      TicketsCount = q.GroupBy(w => w.Counter).Count()

                                  });

            var res = new { Department = posdata.Department.Description, TicketsPerAccount = ticketsPerAccount, VoidTicketsPerAccount = voidTicketsPerAccount };

            return res;
        }


        private Object GetDailyWaiterReceipts(string filters)
        {
            var salesByReceipt = GetFlatSales(filters).Distinct().Where(w => w.IsInvoice == true && w.Void == false);
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var piid = flts.PosList.FirstOrDefault();

            var posdata = db.PosInfo.Include("Department").Where(f => f.Id == piid).FirstOrDefault();

            var DailyWaiterReport = new
            {
                Department = posdata.Department.Description,
                WaitersData = salesByReceipt.GroupBy(f => f.StaffId)
                             .Select(q => new
                             {
                                 WaiterName = q.FirstOrDefault().StaffName,
                                 PerAccountType = q.GroupBy(a => a.AccountId)
                                              .Select(aa => new
                                              {
                                                  AccountType = aa.FirstOrDefault().AccountDescription,
                                                  Receipts = aa.GroupBy(rr => rr.Counter).Select(r => new
                                                  {
                                                      Total = Math.Round(r.Sum(s => (decimal)s.TotalAfterDiscount), 2),
                                                      Room = r.FirstOrDefault().Room,
                                                      ReceiptNo = r.Key,
                                                      Abbr = r.FirstOrDefault().Abbreviation
                                                  }),
                                                  SumTotal = Math.Round(aa.Sum(s => (decimal)s.TotalAfterDiscount), 2)
                                              })
                             })
            };

            return new { processedResults = DailyWaiterReport };


        }

        private Object GetSalesByReceiptMaster(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            if (String.IsNullOrEmpty(filters))
                eod.Add(0);
            else
            {

                if (flts.UseEod)
                {
                    if (flts.EodId == null)
                    {
                        if (flts.UsePeriod)
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                            EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                        }
                        else
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                        }
                    }
                    else
                    {
                        eod.Add((long)flts.EodId);
                    }
                }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
            }

            db.Configuration.LazyLoadingEnabled = true;
            var query = (from q in db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && (w.IsDeleted ?? false) == false)
                         join t in db.Transactions.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals t.InvoicesId into fffff
                         from trans in fffff.DefaultIfEmpty()
                         select new
                         {
                             PosInfoId = q.PosInfoId,
                             PosInfoDescription = q.PosInfo.Description,
                             DepartmentId = q.PosInfo.DepartmentId,
                             DepartmentDescription = q.PosInfo.DepartmentId != null ? q.PosInfo.Department.Description : "",
                             FODay = q.EndOfDayId != null ? q.EndOfDay.FODay : null,
                             //TimeZone = q.
                             TimeZoneId = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? 1
                                                            : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                            ? 2 : ((q.Day.Value.Hour >= 19) && (q.Day.Value.Hour <= 24) || ((q.Day.Value.Hour >= 1) && (q.Day.Value.Hour <= 2))) ? 3 : 4,
                             TimeZone = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? "Breakfast"
                                                            : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                            ? "Lunch" : "Dinner",
                             InvoiceId = q.Id,
                             Abbreviation = q.InvoiceTypes.Abbreviation,
                             InvoiceTypeCode = q.InvoiceTypes.Code,
                             Description = q.Description,
                             Counter = q.Counter,
                             StaffId = q.StaffId,
                             StaffName = q.Staff.LastName,
                             Day = q.Day,
                             Cover = q.Cover,
                             CloseId = q.EndOfDayId != null ? q.EndOfDay.CloseId : 0,
                             TableId = q.TableId,
                             TableCode = q.Table != null ? q.Table.Code : null,
                             Room = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.Room : null,
                             AccountId = trans != null ? trans.AccountId : null,
                             AccountDescription = trans != null ? trans.Accounts.Description : "",
                             Guest = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.LastName : null,
                             InvoiceTotal = q.Total,
                             TransactionTotal = trans != null ? trans.Amount : 0,
                             Discount = q.Discount ?? 0

                         }).ToList();
            db.Configuration.LazyLoadingEnabled = false;
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);


            return new { processedResults = query, metadata };

        }

        private Object GetSalesByReceiptMasterFromStatRepo(string filters)
        {
            var flts = JsonConvert.DeserializeObject<ReceiptFilters>(filters);
            //flts.EodId = 0;
            var query = statRepo.GetReceiptsFor99006(flts.predicate);
            return new { processedResults = query };
        }

        private Object GetSalesByReceiptFromStatRepo(string filters)
        {
            var flts = JsonConvert.DeserializeObject<ReceiptFilters>(filters);
            //  flts.EodId = 0;

            var query = statRepo.GetReceiptsFor99005(flts.predicate);
            return new { processedResults = query };

        }
        private Object GetSalesByReceipt(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            if (String.IsNullOrEmpty(filters))
                eod.Add(0);
            else
            {

                if (flts.UseEod)
                {
                    if (flts.EodId == null)
                    {
                        if (flts.UsePeriod)
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                            EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                        }
                        else
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                        }
                    }
                    else
                    {
                        eod.Add((long)flts.EodId);
                    }
                }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
            }

            db.Configuration.LazyLoadingEnabled = true;
            var validInvTypes = db.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Void);
            if (flts.UseOrderInvoicesType == false)
                validInvTypes = validInvTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order);
            var validInvs = from q in db.Invoices.Where(w => (w.IsDeleted ?? false) == false && eod.Contains((w.EndOfDayId ?? 0)))
                            join qqq in validInvTypes on q.InvoiceTypeId equals qqq.Id
                            join qq in flts.PosList on q.PosInfoId equals qq
                            select q;

            var query = from q in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false)
                        join oid in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals oid.OrderDetailId
                        join inv in validInvs /*db.Invoices.Where(w => (w.IsDeleted ?? false) == false && eod.Contains((w.EndOfDayId ?? 0)))*/ on oid.InvoicesId equals inv.Id
                        join odva in db.OrderDetailVatAnal.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals odva.OrderDetailId
                        select new
                        {
                            InvoiceId = inv.Id,
                            PosInfoId = inv.PosInfoId,
                            PosInfoDescription = inv.PosInfo.Description,
                            //DepartmentDescription = inv.PosInfo.Department.Description
                            IsExtra = 0,
                            Day = inv.Day,
                            StaffId = inv.StaffId,
                            StaffName = inv.Staff.LastName,
                            TableCode = inv.TableId != null ? inv.Table.Code : "",
                            OrderDetailId = q.Id,
                            FODay = inv.EndOfDay.FODay,
                            InvoiceTypeCode = inv.InvoiceTypes.Code,
                            InvoiceType = inv.InvoiceTypes.Abbreviation,
                            InvoiceDescription = inv.Description,
                            InvoiceCounter = inv.Counter,
                            InvoiceTotal = inv.Total,
                            IsVoided = inv.IsVoided,
                            Status = q.Status,
                            OrderId = q.OrderId,
                            ProductCode = q.Product.Code,
                            ProductId = q.ProductId,
                            ProductDescription = q.Product.Description,
                            UnitPrice = q.PricelistDetail.Price,
                            Qty = q.Qty,
                            Total = q.TotalAfterDiscount,
                            Discount = q.Discount,
                            DiscountRemark = inv.DiscountRemark,
                            ProductCategoryId = q.Product.ProductCategoryId,
                            ProductCategoryDescription = q.Product.ProductCategoryId != null ? q.Product.ProductCategories.Description : "",
                            CategoryId = q.Product.ProductCategoryId != null ? q.Product.ProductCategories.CategoryId : null,
                            CategoryDescription = q.Product.ProductCategoryId != null ? q.Product.ProductCategories.CategoryId != null ? q.Product.ProductCategories.Categories.Description : "" : "",
                            PriceListId = q.PricelistDetail.PricelistId,
                            PriceListDescription = q.PricelistDetail.Pricelist.Description,
                            SalesTypeId = q.SalesTypeId,
                            SalesTypeDescription = q.SalesType.Description,
                            VatId = odva.VatId,
                            VatAmount = odva.VatAmount,
                            VatRate = odva.VatRate,
                            Net = odva.Gross - odva.VatAmount - odva.TaxAmount,//odva.Net,
                            Tax = odva.TaxAmount,
                        };

            var queryIng = from q in db.OrderDetailIgredients.Where(w => (w.IsDeleted ?? false) == false)
                               //    join qi in db.Ingredients on q.IngredientId equals qi.Id
                           join od in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false) on q.OrderDetailId equals od.Id
                           join oid in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on od.Id equals oid.OrderDetailId
                           join inv in validInvs /*db.Invoices.Where(w => (w.IsDeleted ?? false) == false && eod.Contains((w.EndOfDayId ?? 0)))*/ on oid.InvoicesId equals inv.Id
                           join odva in db.OrderDetailIgredientVatAnal.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals odva.OrderDetailIgredientsId
                           select new
                           {
                               InvoiceId = inv.Id,
                               PosInfoId = inv.PosInfoId,
                               PosInfoDescription = inv.PosInfo.Description,
                               //DepartmentDescription = inv.PosInfo.Department.Description
                               IsExtra = 1,
                               Day = inv.Day,
                               StaffId = inv.StaffId,
                               StaffName = inv.Staff.LastName,
                               TableCode = inv.TableId != null ? inv.Table.Code : "",
                               OrderDetailId = od.Id,
                               FODay = inv.EndOfDay.FODay,
                               InvoiceTypeCode = inv.InvoiceTypes.Code,
                               InvoiceType = inv.InvoiceTypes.Abbreviation,
                               InvoiceDescription = inv.Description,
                               InvoiceCounter = inv.Counter,
                               InvoiceTotal = inv.Total,
                               IsVoided = inv.IsVoided,
                               Status = od.Status,
                               OrderId = od.OrderId,
                               ProductCode = q.Ingredients.Code,// q.OrderDetail.Product.Code,
                               ProductId = q.IngredientId,
                               ProductDescription = q.Ingredients.Description,
                               UnitPrice = q.PricelistDetail.Price,
                               Qty = q.Qty,
                               Total = q.TotalAfterDiscount,
                               Discount = q.Discount,
                               DiscountRemark = inv.DiscountRemark,
                               ProductCategoryId = od.Product.ProductCategoryId,
                               ProductCategoryDescription = od.Product.ProductCategoryId != null ? od.Product.ProductCategories.Description : "",
                               CategoryId = od.Product.ProductCategoryId != null ? od.Product.ProductCategories.CategoryId : null,
                               CategoryDescription = od.Product.ProductCategoryId != null ? od.Product.ProductCategories.CategoryId != null ? od.Product.ProductCategories.Categories.Description : "" : "",
                               PriceListId = q.PricelistDetail.PricelistId,
                               PriceListDescription = q.PricelistDetail.Pricelist.Description,
                               SalesTypeId = od.SalesTypeId,
                               SalesTypeDescription = od.SalesType.Description,
                               VatId = odva.VatId,
                               VatAmount = odva.VatAmount,
                               VatRate = odva.VatRate,
                               Net = odva.Gross - odva.VatAmount - odva.TaxAmount,//odva.Net,
                               Tax = odva.TaxAmount,
                           };

            var joined = query.Union(queryIng).OrderBy(o => o.InvoiceId).ThenBy(t => t.OrderDetailId);

            var final = from q in joined
                        join tt in db.Transactions on q.InvoiceId equals tt.InvoicesId into f
                        from t in f.DefaultIfEmpty()
                        select new
                        {
                            InvoiceId = q.InvoiceId,
                            PosInfoId = q.PosInfoId,
                            PosInfoDescription = q.PosInfoDescription,
                            FODay = q.FODay,
                            //DepartmentDescription = inv.PosInfo.Department.Description
                            IsExtra = q.IsExtra,
                            Day = q.Day,
                            StaffId = q.StaffId,
                            StaffName = q.StaffName,
                            TableCode = q.TableCode,
                            OrderDetailId = q.OrderDetailId,
                            InvoiceTypeCode = q.InvoiceTypeCode,
                            InvoiceType = q.InvoiceType,
                            InvoiceDescription = q.InvoiceDescription,
                            InvoiceCounter = q.InvoiceCounter,
                            InvoiceTotal = q.InvoiceTotal,
                            IsVoided = q.IsVoided,
                            Status = q.Status,
                            OrderId = q.OrderId,
                            ProductCode = q.ProductCode,
                            ProductId = q.ProductId,
                            ProductDescription = q.ProductDescription,
                            UnitPrice = q.UnitPrice,
                            Qty = q.Qty,
                            Total = q.Total,
                            Discount = q.Discount,
                            DiscountRemark = q.DiscountRemark,
                            ProductCategoryId = q.ProductCategoryId,
                            ProductCategoryDescription = q.ProductCategoryDescription,
                            CategoryId = q.CategoryId,
                            CategoryDescription = q.CategoryDescription,
                            PriceListId = q.PriceListId,
                            PriceListDescription = q.PriceListDescription,
                            SalesTypeId = q.SalesTypeId,
                            SalesTypeDescription = q.SalesTypeDescription,
                            VatId = q.VatId,
                            VatAmount = q.VatAmount,
                            VatRate = q.VatRate,
                            Net = q.Net,
                            Tax = q.Tax,
                            Room = t != null ? t.Invoice_Guests_Trans.FirstOrDefault().Guest.Room : "",
                            TransactionsDescription = t != null ? t.Accounts.Description : "Not Paid",
                            TransactionsAmount = t != null ? t.Amount : 0,
                            TransStaffId = t != null ? t.StaffId : -1,
                            TransStaffName = t != null ? t.Staff.LastName : "",
                            TotalForNoTransaction = t.Amount == 0 ? q.InvoiceTotal : t.Amount


                        };
            //	final.Dump();
            //	return;
            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            var processedResults = final.ToList().GroupBy(g => new { g.InvoiceId, g.OrderDetailId, g.IsExtra, g.ProductId }).Select(s => new
            {
                StoreId = storeid,
                InvoiceId = s.FirstOrDefault().InvoiceId,
                PosInfoId = s.FirstOrDefault().PosInfoId,
                PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
                //DepartmentDescription = inv.PosInfo.Department.Description
                IsExtra = s.FirstOrDefault().IsExtra,
                Day = s.FirstOrDefault().Day,
                StaffId = s.FirstOrDefault().StaffId,
                StaffName = s.FirstOrDefault().StaffName,
                TableCode = s.FirstOrDefault().TableCode,
                OrderDetailId = s.FirstOrDefault().OrderDetailId,
                InvoiceTypeCode = s.FirstOrDefault().InvoiceTypeCode,
                InvoiceType = s.FirstOrDefault().InvoiceType,
                InvoiceDescription = s.FirstOrDefault().InvoiceDescription,
                InvoiceCounter = s.FirstOrDefault().InvoiceCounter,
                InvoiceTotal = s.FirstOrDefault().InvoiceTotal,
                IsVoided = s.FirstOrDefault().IsVoided,
                Status = s.FirstOrDefault().Status,
                OrderId = s.FirstOrDefault().OrderId,
                ProductCode = s.FirstOrDefault().ProductCode,
                ProductId = s.FirstOrDefault().ProductId,
                ProductDescription = s.FirstOrDefault().ProductDescription,
                UnitPrice = s.FirstOrDefault().UnitPrice,
                Qty = s.FirstOrDefault().Qty,//s.Sum(sm => sm.Qty),
                Total = s.FirstOrDefault().Total,//s.Sum(sm => sm.Total),
                Discount = s.FirstOrDefault().Discount,//s.Sum(sm => sm.Discount),
                DiscountRemark = s.FirstOrDefault().DiscountRemark,
                ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                ProductCategoryDescription = s.FirstOrDefault().ProductCategoryDescription,
                CategoryId = s.FirstOrDefault().CategoryId,
                CategoryDescription = s.FirstOrDefault().CategoryDescription,
                PriceListId = s.FirstOrDefault().PriceListId,
                PriceListDescription = s.FirstOrDefault().PriceListDescription,
                SalesTypeId = s.FirstOrDefault().SalesTypeId,
                SalesTypeDescription = s.FirstOrDefault().SalesTypeDescription,
                VatId = s.FirstOrDefault().VatId,
                VatAmount = s.FirstOrDefault().VatAmount,
                VatRate = s.FirstOrDefault().VatRate,
                Net = s.FirstOrDefault().Total - s.FirstOrDefault().VatAmount - s.FirstOrDefault().Tax,//s.FirstOrDefault().Net,
                Tax = s.FirstOrDefault().Tax,
                Transactions = s.Select(ss => new { Description = ss.TransactionsDescription, Room = ss.Room, Amount = ss.TransactionsAmount }).Distinct(),
                TransStaffId = s.FirstOrDefault().TransStaffId,
                TransStaffName = s.FirstOrDefault().TransStaffName,
                TotalForNoTransaction = s.FirstOrDefault().TotalForNoTransaction,
                FODay = s.FirstOrDefault().FODay,



            }).OrderBy(o => o.InvoiceId).ThenBy(t => t.OrderDetailId).ThenBy(t1 => t1.IsExtra);
            db.Configuration.LazyLoadingEnabled = false;
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);

            return new { processedResults, metadata };
        }

        private dynamic ParseFilters(string filters)
        {

            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var poslist = db.PosInfo.Where(w => flts.PosList.Contains(w.Id));
            var itList = db.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void).Select(s => s.Id);

            if (flts.UseEod)
            {
                if (flts.UsePeriod)
                {
                    var eods = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                    EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id);
                    return new { poslist, itList, eods };
                }
                else
                {
                    var eods = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id);
                    return new { poslist, itList, eods };
                }
            }

            return new { poslist, itList };

        }

        private Object GetSalesForProductReport(string filters)
        {

            //    var final = /*db.ProductSalesHistoryPerDay.Where(w => eod.Contains(w.EodId.Value) && posList.Contains(w.PosInfoId) && itList.Contains(w.InvoiceTypeId.Value));
            //        var a = db.ProductSalesHistoryPerDay.Select("new (ProductCode, ProdectDescription, InvoicesId, Total, Qty, StaffId, StaffName)");//.AsQueryable<dynamic>();
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var poslist = db.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId));
            var itList = db.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void).Select(s => s.Id);
            var vitList = itList;//.Where(w => flts.InvoiceList.Contains(w));
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);


            db.Configuration.LazyLoadingEnabled = true;
            var eods = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                    EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id);
            var validInvs = (from q in db.Invoices.Where(w => (w.IsDeleted ?? false) == false)
                             join qqqq in flts.PosList on q.PosInfoId equals qqqq
                             join qqq in vitList on q.InvoiceTypeId equals qqq
                             join ed in eods on q.EndOfDayId equals ed
                             join qq in db.OrderDetailInvoices on q.Id equals qq.InvoicesId
                             join q5 in poslist/*db.PosInfo*/ on q.PosInfoId equals q5.Id
                             select new { InvoicesId = q.Id, OrderDetailId = qq.OrderDetailId, PosInfoId = q.PosInfoId, PosInfoDescription = q5.Description }).Distinct();
            //validInvs.Distinct().Count().Dump();
            var orderDet = (from q in db.OrderDetail
                            join qqq in validInvs on q.Id equals qqq.OrderDetailId
                            join qq in db.OrderDetailVatAnal on q.Id equals qq.OrderDetailId
                            select new
                            {
                                OrderDetailId = q.Id,
                                InvoicesId = qqq.InvoicesId,
                                PriceListDetailId = q.PriceListDetailId,
                                UnitPrice = q.Price,
                                Total = q.TotalAfterDiscount,
                                Qty = q.Qty,
                                VatId = qq.VatId,
                                Net = qq.Net,
                                VatAmount = qq.VatAmount,
                                VatRate = qq.VatRate,
                                SalesTypeId = q.SalesTypeId,
                                ProductId = q.ProductId,
                                IngredientId = (Int64?)-1,
                                IsProduct = true,
                                Price = q.Price,
                                TotalAfterDiscount = q.TotalAfterDiscount,
                                PosInfoId = qqq.PosInfoId,
                                PosInfoDescription = qqq.PosInfoDescription
                            }).Distinct();

            var ingDet = (from q in db.OrderDetailIgredients.Where(w => w.TotalAfterDiscount != 0)
                          join qqq in validInvs on q.OrderDetailId equals qqq.OrderDetailId
                          join qq in db.OrderDetailIgredientVatAnal on q.Id equals qq.OrderDetailIgredientsId
                          join qqqq in db.OrderDetail on q.OrderDetailId equals qqqq.Id
                          select new
                          {
                              OrderDetailId = qqqq.Id,
                              InvoicesId = qqq.InvoicesId,
                              PriceListDetailId = q.PriceListDetailId,
                              UnitPrice = q.Price,
                              Total = q.TotalAfterDiscount,
                              Qty = q.Qty,
                              VatId = qq.VatId,
                              Net = qq.Net,
                              VatAmount = qq.VatAmount,
                              VatRate = qq.VatRate,
                              SalesTypeId = qqqq.SalesTypeId,
                              ProductId = (Int64?)-1,
                              IngredientId = q.IngredientId,
                              IsProduct = false,
                              Price = q.Price,
                              TotalAfterDiscount = q.TotalAfterDiscount,
                              PosInfoId = qqq.PosInfoId,
                              PosInfoDescription = qqq.PosInfoDescription
                          }).Distinct();


            var salesPerProd = orderDet.Union(ingDet);//.Dump();

            var query = (from q in salesPerProd
                         join inv in db.Invoices.Where(w => (w.IsDeleted ?? false) == false) on q.InvoicesId equals inv.Id
                         join pd in db.PricelistDetail on q.PriceListDetailId equals pd.Id
                         join s in db.Staff on inv.StaffId equals s.Id
                         select new
                         {
                             EodId = inv.EndOfDayId ?? 0,
                             InvoicesId = inv.Id,
                             InvoiceTypeId = inv.InvoiceTypeId,
                             OrderDetailId = q.OrderDetailId,
                             Covers = inv.Cover ?? 0,
                             PosInfoId = inv.PosInfoId ?? 0,
                             IsVoided = inv.IsVoided ?? false,
                             ProductId = q.ProductId,
                             IngredientId = q.IngredientId,
                             PriceListDetailId = q.PriceListDetailId,
                             PriceList = pd.PricelistId,
                             PriceListPrice = pd.Price,
                             UnitPrice = q.Price,
                             Total = q.TotalAfterDiscount,
                             Qty = q.Qty,
                             StaffId = inv.StaffId,
                             VatId = q.VatId,
                             Net = q.Net,
                             VatAmount = q.VatAmount,
                             VatRate = q.VatRate,
                             SalesTypeId = q.SalesTypeId,
                             StaffName = s.LastName,
                             IsProduct = q.IsProduct,
                             //   PosInfoId = q.PosInfoId,
                             PosInfoDescription = q.PosInfoDescription
                         }).Distinct();//.ToList();

            //	query.Dump();
            //query.Where(w=>);
            var productsflat = (from q in db.Product
                                join qq in db.ProductCategories on q.ProductCategoryId equals qq.Id into f
                                from pc in f.DefaultIfEmpty()
                                join qqq in db.Categories on pc.CategoryId equals qqq.Id into ff
                                from c in ff.DefaultIfEmpty()
                                select new
                                {
                                    ProductId = q.Id,
                                    ProductCode = q.Code,
                                    ProductDescription = q.SalesDescription,
                                    ProductCategoryId = q.ProductCategoryId,
                                    ProductCategoryDescription = pc != null ? pc.Description : "",
                                    CategoryId = c != null ? c.Id : 0,
                                    CategoryDescription = c != null ? c.Description : "",

                                });//.ToList();
            var ins = db.Ingredients.Select(s => new { Id = s.Id, Description = s.SalesDescription, Code = s.Code });
            var prefinal = (from q in query
                            join qq in productsflat on q.ProductId equals qq.ProductId into ff
                            from prd in ff.DefaultIfEmpty()
                            join qqq in ins on q.IngredientId equals qqq.Id into f
                            from ing in f.DefaultIfEmpty()
                            join qqqq in db.Pricelist on q.PriceList equals qqqq.Id
                            select new
                            {
                                ProductId = q.IsProduct ? prd.ProductId : ing.Id,
                                ProductCode = !q.IsProduct ? ing.Code : prd.ProductCode,
                                ProductDescription = !q.IsProduct ? ing.Description : prd.ProductDescription,
                                ProductCategoryId = q.IsProduct ? prd.ProductCategoryId : -1,
                                ProductCategoryDescription = q.IsProduct ? prd.ProductCategoryDescription : "",
                                CategoryId = q.IsProduct ? prd.CategoryId : -1,
                                CategoryDescription = q.IsProduct ? prd.CategoryDescription : "",
                                EodId = q.EodId,
                                OrderDetailId = q.OrderDetailId,
                                InvoicesId = q.InvoicesId,
                                Covers = q.Covers,
                                PosInfoId = q.PosInfoId,
                                PosInfoDescription = q.PosInfoDescription,
                                IsVoided = q.IsVoided,
                                PriceListDetailId = q.PriceListDetailId,
                                PriceList = q.PriceList,
                                UnitPrice = q.UnitPrice,
                                Total = q.Total,
                                TotalAfterDiscount = q.Total,
                                Qty = q.Qty,
                                StaffId = q.StaffId,
                                VatId = q.VatId,
                                Net = q.Net,
                                VatAmount = q.VatAmount,
                                VatRate = q.VatRate,
                                SalesTypeId = q.SalesTypeId,
                                StaffName = q.StaffName,
                                IsProduct = q.IsProduct,
                                PricelistDescription = qqqq.Description,
                                PriceListPrice = q.PriceListPrice
                            }).Distinct().ToList();
            #region deprecated
            //var query = (from q in db.OrderDetailInvoices
            //            join inv in db.Invoices.Where(w => (w.IsDeleted ?? false) == false && eod.Contains(w.EndOfDayId ?? 0) && posList.Contains(w.PosInfoId) && itList.Contains(w.InvoiceTypeId.Value)) on q.InvoicesId equals inv.Id
            //            join od in db.OrderDetail on q.OrderDetailId equals od.Id
            //            join pd in db.PricelistDetail on od.PriceListDetailId equals pd.Id
            //            join odva in db.OrderDetailVatAnal on q.OrderDetailId equals odva.OrderDetailId
            //            join s in db.Staff on inv.StaffId equals s.Id
            //            select new
            //            {
            //                EodId = inv.EndOfDayId ?? 0,
            //                InvoicesId = inv.Id,
            //                InvoiceTypeId = inv.InvoiceTypeId,
            //                Covers = inv.Cover ?? 0,
            //                PosInfoId = inv.PosInfoId ?? 0,
            //                IsVoided = inv.IsVoided ?? false,
            //                ProductId = od.ProductId,
            //                PriceListDetailId = od.PriceListDetailId,
            //                PriceList = pd.PricelistId,
            //                UnitPrice = od.Price,
            //                Total = od.TotalAfterDiscount,
            //                Qty = od.Qty,
            //                StaffId = inv.StaffId,
            //                VatId = odva.VatId,
            //                Net = odva.Net,
            //                VatAmount = odva.VatAmount,
            //                VatRate = odva.VatRate,
            //                SalesTypeId = od.SalesTypeId,
            //                StaffName = s.LastName
            //            }).ToList();


            ////query.Where(w=>);
            //var productsflat = (from q in db.Product
            //                   join qq in db.ProductCategories on q.ProductCategoryId equals qq.Id into f
            //                   from pc in f.DefaultIfEmpty()
            //                   join qqq in db.Categories on pc.CategoryId equals qqq.Id into ff
            //                   from c in ff.DefaultIfEmpty()
            //                   select new
            //                   {
            //                       ProductId = q.Id,
            //                       ProductCode = q.Code,
            //                       ProductDescription = q.SalesDescription,
            //                       ProductCategoryId = q.ProductCategoryId,
            //                       ProductCategoryDescription = pc != null ? pc.Description : "",
            //                       CategoryId = c != null ? c.Id : 0,
            //                       CategoryDescription = c != null ? c.Description : "",

            //                   }).ToList();

            //var final = (from q in query
            //             join qq in productsflat on q.ProductId equals qq.ProductId
            //             select new
            //             {
            //                 ProductId = qq.ProductId,
            //                 ProductCode = qq.ProductCode,
            //                 ProductDescription = qq.ProductDescription,
            //                 ProductCategoryId = qq.ProductCategoryId,
            //                 ProductCategoryDescription = qq.ProductCategoryDescription,
            //                 CategoryId = qq.CategoryId,
            //                 CategoryDescription = qq.CategoryDescription,
            //                 EodId = q.EodId,
            //                 InvoicesId = q.InvoicesId,
            //                 Covers = q.Covers,
            //                 PosInfoId = q.PosInfoId,
            //                 IsVoided = q.IsVoided,
            //                 PriceListDetailId = q.PriceListDetailId,
            //                 PriceList = q.PriceList,
            //                 UnitPrice = q.UnitPrice,
            //                 Total = q.Total,
            //                 Qty = q.Qty,
            //                 StaffId = q.StaffId,
            //                 VatId = q.VatId,
            //                 Net = q.Net,
            //                 VatAmount = q.VatAmount,
            //                 VatRate = q.VatRate,
            //                 SalesTypeId = q.SalesTypeId,
            //                 StaffName = q.StaffName

            //             }).ToList();
            #endregion
            db.Configuration.LazyLoadingEnabled = false;
            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            //var final = prefinal;
            switch (flts.ReportType)
            {
                case 9009:
                    var fn = prefinal.GroupBy(g => new { g.StaffId, g.IsVoided }).Select(s => new
                    {
                        StoreId = storeid,
                        StaffId = s.FirstOrDefault().StaffId,
                        StaffName = s.FirstOrDefault().StaffName,
                        IsVoided = s.FirstOrDefault().IsVoided,
                        InvoiceCount = s.Distinct().GroupBy(ss => ss.InvoicesId).Count(),
                        Total = s.Sum(sm => sm.TotalAfterDiscount),
                        ProductQty = s.Where(w => w.IsProduct).Sum(sm => sm.Qty),
                        Covers = s.GroupBy(gg => gg.InvoicesId).Sum(sm => sm.FirstOrDefault().Covers)
                    });
                    return new { processedResults = fn.Distinct(), metadata };
                case 9007:
                    var final = prefinal.GroupBy(g => new { g.ProductId, g.StaffId, g.IsVoided, g.PosInfoId }).Select(s => new
                    {
                        StoreId = storeid,
                        PosInfoId = s.FirstOrDefault().PosInfoId,
                        PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
                        ProductId = s.FirstOrDefault().ProductId,
                        ProductCode = s.FirstOrDefault().ProductCode,
                        ProductDescription = s.FirstOrDefault().ProductDescription,
                        ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                        ProductCategoryDescription = s.FirstOrDefault().ProductCategoryDescription,
                        CategoryId = s.FirstOrDefault().CategoryId,
                        CategoryDescription = s.FirstOrDefault().CategoryDescription,
                        PriceListPrice = s.FirstOrDefault().PriceListPrice,
                        IsVoided = s.FirstOrDefault().IsVoided,
                        InvoiceCount = s.Select(ss => ss.InvoicesId).Distinct().Count(),
                        Total = s.Select(ss => new { Id = ss.OrderDetailId, Total = ss.Total }).Distinct().Sum(sm => sm.Total),
                        Qty = s.Select(ss => new { Id = ss.OrderDetailId, Qty = ss.Qty }).Distinct().Sum(sm => sm.Qty),
                        StaffId = s.FirstOrDefault().StaffId,
                        //                                           SalesTypeId = s.FirstOrDefault().SalesTypeId,
                        StaffName = s.FirstOrDefault().StaffName
                    });
                    return new { processedResults = final.Distinct(), metadata };
                // break;
                case 9008:
                    var final1 = prefinal.GroupBy(g => new { g.ProductId, g.PriceList, g.IsVoided }).Select(s => new
                    {
                        StoreId = storeid,
                        ProductId = s.FirstOrDefault().ProductId,
                        ProductCode = s.FirstOrDefault().ProductCode,
                        ProductDescription = s.FirstOrDefault().ProductDescription,
                        ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                        ProductCategoryDescription = s.FirstOrDefault().ProductCategoryDescription,
                        CategoryId = s.FirstOrDefault().CategoryId,
                        CategoryDescription = s.FirstOrDefault().CategoryDescription,
                        PriceListPrice = s.FirstOrDefault().PriceListPrice,
                        IsVoided = s.FirstOrDefault().IsVoided,
                        InvoiceCount = s.Select(ss => ss.InvoicesId).Distinct().Count(),
                        Total = s.Select(ss => new { Id = ss.OrderDetailId, Total = ss.Total }).Distinct().Sum(sm => sm.Total),
                        Qty = s.Select(ss => new { Id = ss.OrderDetailId, Qty = ss.Qty }).Distinct().Sum(sm => sm.Qty),
                        PricelistId = s.FirstOrDefault().PriceList,
                        //                                           SalesTypeId = s.FirstOrDefault().SalesTypeId,
                        PricelistDescription = s.FirstOrDefault().PricelistDescription
                    });


                    //var flatAmountTotals = prefinal.Distinct().Sum(s => s.TotalAfterDiscount);
                    //var final1 = prefinal.GroupBy(g => g.ProductId).Select(s => new
                    //{
                    //    PriceList = s.FirstOrDefault().PriceList,
                    //    ProductId = s.Key,
                    //    Product = s.FirstOrDefault().ProductDescription,
                    //    Qty = s.Sum(sm => (Double)sm.Qty),
                    //    Total = s.Sum(sm => (Decimal)sm.TotalAfterDiscount),
                    //    Receipts = s.Select(s4 => s4.InvoicesId).Distinct().Count()
                    //}).GroupBy(gg => gg.PriceList).Select(ss => new
                    //{
                    //    PriceList = ss.Key,
                    //    Qty = ss.Sum(sm => (Double)sm.Qty),
                    //    Total = ss.Sum(sm => (Decimal)sm.Total),
                    //    TotalPerc = ss.Sum(sm => (Decimal)sm.Total) * 100 / flatAmountTotals,
                    //    TotalReceipts = ss.Sum(sm => sm.Receipts),
                    //    Products = ss.Select(s3 => new
                    //    {
                    //        ProductId = s3.ProductId,
                    //        Product = s3.Product,
                    //        Qty = s3.Qty,
                    //        Total = s3.Total,
                    //        Receipts = s3.Receipts
                    //    })
                    //});
                    return new { processedResults = final1, metadata };
                default:
                    break;
            }
            //    var a = final.AsQueryable().Select("new (ProductCode, ProdectDescription, InvoicesId, Total, Qty, StaffId, StaffName)");


            return new { processedResults = prefinal, metadata/*.Select<dynamic>("new (ProductCode, ProdectDescription, InvoicesId, Total, Qty, StaffId, StaffName)") */};
        }


        [Obsolete]
        private Object GetReceiptForReprint(int posInfoDetailId, int receiptNo)
        {
            db.Configuration.LazyLoadingEnabled = true;
            //var posInfoDetailId = 98;
            //var receiptNo = 71;
            // var guestData = db.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().CustomerId != null;
            //OrderDetail.Where(w=>w.OrderDetailInvoices.Any(a=>a.Counter == 59 && a.PosInfoDetailId == 58)).Dump();
            //var qry = db.OrderDetail.Where(w => w.OrderDetailInvoices.Any(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId));
            var query = db.OrderDetail.Where(w => w.OrderDetailInvoices.Any(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId)).ToList().Select(s => new
            {
                FiscaType = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.FiscalType,
                InvoiceIndex = 0,
                Guest = s.Guest,
                TableNo = s.Table != null ? s.Table.Code : "",
                Waiter = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().Staff.LastName,//+ ' ' + s.OrderDetailInvoices.FirstOrDefault().Staff.FirstName,
                WaiterNo = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().StaffId,
                Pos = "POS-" + s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Code,
                PosDescr = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Description,
                DepartmentDesc = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Department.Description,
                Department = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.DepartmentId,
                //		CustomerName = "Πελάτης Λιανικής",
                //		CustomerAddress ="",
                //		CustomerDeliveryAddress ="",
                //		CustomerPhone = "",
                //		CustomerComments = "",
                //		CustomerAfm = "",
                //		CustomerDoy = "",
                //		CustomerJob = "",
                //	RegNo = "",
                //		SumOfLunches = "",
                //		SumofConsumedLunches = "",
                //		GuestTerm = "",
                //		Adults = 0,
                ////		Kids = 0,
                InvoiceType = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.InvoiceId,//InvoiceTYpe right??
                TotalVat = s.OrderDetailVatAnal.Sum(sm => sm.Gross),
                TotalVat1 = s.OrderDetailVatAnal.Where(w => w.VatId == 1).Sum(sm => sm.Gross),
                TotalVat2 = s.OrderDetailVatAnal.Where(w => w.VatId == 2).Sum(sm => sm.Gross),
                TotalVat3 = s.OrderDetailVatAnal.Where(w => w.VatId == 3).Sum(sm => sm.Gross),
                TotalVat4 = s.OrderDetailVatAnal.Where(w => w.VatId == 4).Sum(sm => sm.Gross),
                TotalVat5 = s.OrderDetailVatAnal.Where(w => w.VatId == 5).Sum(sm => sm.Gross),
                TotalDiscount = s.Discount,
                Bonus = 0,
                PriceList = s.PricelistDetail.PricelistId,
                ReceiptNo = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().Counter,
                Change = 0,
                PaidAmount = s.Transactions.Amount,
                IsVoid = s.Status == 5,
                DetailsId = s.Id,

                PrintKitchen = 0,
                KitchenId = s.Product.KitchenId,
                PaymentType = s.Transactions.AccountId,
                ReceiptTypeDescription = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.Description,
                DepartmentTypeDescription = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Department.Description,
                Price = s.Price,
                IsChangeItem = s.Price < 0,
                ItemCode = s.ProductId,
                ItemDescr = s.Price < 0 ?/* "Αλλαγή " + */s.Product.Description : s.Product.Description,
                ItemQty = s.Qty,
                ItemPrice = s.Price,
                ItemVatRate = s.OrderDetailVatAnal.FirstOrDefault().VatRate,
                ItemGross = s.TotalAfterDiscount,
                ItemDiscount = s.Discount,
                SalesTypeDesc = s.SalesType.Abbreviation,
                Ingredients = s.OrderDetailIgredients



            }).ToList();
            db.Configuration.LazyLoadingEnabled = false;
            var queryGrouped = query.GroupBy(g => g.ReceiptNo).Select(s => new
            {

                FiscaType = s.FirstOrDefault().FiscaType,
                InvoiceIndex = s.FirstOrDefault().InvoiceIndex,
                TableNo = s.FirstOrDefault().TableNo,
                RoomNo = s.FirstOrDefault().Guest == null ? "" : s.FirstOrDefault().Guest.Room,
                Waiter = s.FirstOrDefault().Waiter,
                WaiterNo = s.FirstOrDefault().WaiterNo,
                Pos = s.FirstOrDefault().Pos,
                PosDescr = s.FirstOrDefault().PosDescr,
                DepartmentDesc = s.FirstOrDefault().DepartmentDesc,
                Department = s.FirstOrDefault().Department,
                CustomerName = s.FirstOrDefault().Guest == null ? "Πελάτης Λιανικής" : s.FirstOrDefault().Guest.LastName,// + " " + s.FirstOrDefault().Guest.FirstName,
                CustomerAddress = s.FirstOrDefault().Guest == null ? "" : s.FirstOrDefault().Guest.Address,
                CustomerDeliveryAddress = "",
                CustomerPhone = "",
                CustomerComments = "",
                CustomerAfm = "",
                CustomerDoy = "",
                CustomerJob = "",
                RegNo = s.FirstOrDefault().Guest == null ? 0 : s.FirstOrDefault().Guest.ReservationId,
                SumOfLunches = 0,
                SumofConsumedLunches = 0,
                GuestTerm = s.FirstOrDefault().Guest == null ? "" : s.FirstOrDefault().Guest.BoardCode,
                Adults = 0,
                Kids = 0,
                InvoiceType = s.FirstOrDefault().InvoiceType,
                TotalVat1 = s.Sum(sm => sm.TotalVat1),
                TotalVat2 = s.Sum(sm => sm.TotalVat2),
                TotalVat3 = s.Sum(sm => sm.TotalVat3),
                TotalVat4 = s.Sum(sm => sm.TotalVat4),
                TotalVat5 = s.Sum(sm => sm.TotalVat5),
                TotalDiscount = s.Sum(sm => sm.TotalDiscount),
                Bonus = s.FirstOrDefault().Bonus,
                PriceList = s.FirstOrDefault().PriceList,
                ReceiptNo = s.FirstOrDefault().ReceiptNo,
                Change = s.FirstOrDefault().Change,
                PaidAmount = s.FirstOrDefault().PaidAmount,
                IsVoid = s.FirstOrDefault().IsVoid,
                DetailsId = s.FirstOrDefault().DetailsId,
                //Guest = s.FirstOrDefault().Guest,
                orderdetails = s.Select(ss => new
                {
                    //Id = ss.Id;
                    //od.AA = d.AA;
                    Description = ss.ItemDescr,
                    IsChangeItem = ss.IsChangeItem,
                    KitchenCode = ss.KitchenId,
                    Price = ss.Price,
                    ProductId = ss.ItemCode,
                    Qty = ss.ItemQty,
                    SalesTypeDesc = ss.SalesTypeDesc,
                    OrderDetailIgredients = ss.Ingredients.Select(sss => new
                    {
                        IngredientId = sss.IngredientId,
                        Price = sss.Price,
                        Qty = sss.Qty,
                        VatCode = sss.OrderDetailIgredientVatAnal.FirstOrDefault().VatId,
                        VatDesc = sss.OrderDetailIgredientVatAnal.FirstOrDefault().VatRate,
                    })
                })
            });
            return new { processedResults = queryGrouped };
        }

        private Object PosDailyRevenueNew(string filters, bool isWaiter = false)
        {
            var salesByReceipt = GetFlatSales(filters);
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            List<long> eod = new List<long>();

            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
            else
                eod.Add(0);
            var groupedByDepartment = salesByReceipt.GroupBy(g => g.FODay).Select(s => new
            {
                FODay = s.Key,
                Gross = s.Sum(sm => (Decimal?)sm.Total),
                AllDepartments = s.GroupBy(gg => gg.DepartmentId)
                                                 .Select(ss => new
                                                 {
                                                     DepartmentId = ss.Key,
                                                     Department = ss.FirstOrDefault().DepartmentDescription,
                                                     Gross = ss.Sum(sm => (Decimal?)sm.Total),
                                                     AllPos = ss.GroupBy(g5 => g5.PosInfoId).Select(s9 => new
                                                     {
                                                         PosInfoId = s9.Key,
                                                         PosDescription = s9.FirstOrDefault().PosInfoDescription,
                                                         Gross = s9.Sum(sm => (Decimal?)sm.Total),
                                                         AllAccounts = s9.Select(sss => sss)
                                                                         .GroupBy(ggg => ggg.AccountId).Select(sss => new
                                                                         {
                                                                             AccountId = sss.Key,
                                                                             AccountDescription = sss.FirstOrDefault().AccountDescription,
                                                                             Gross = sss.Sum(sm => (Decimal?)sm.Total),
                                                                             Receipts = sss.GroupBy(g => new { g.Abrreviation, g.Counter, g.Day })
                                                                                             .Select(ssss => new
                                                                                             {
                                                                                                 Day = s.FirstOrDefault().Day,
                                                                                             //                                                  DepartmentId = s.Key.DepartmentId,
                                                                                             //                                                  Deparmtent = s.FirstOrDefault().Department,
                                                                                             ReceiptAbbr = ssss.FirstOrDefault().Abrreviation,
                                                                                                 ReceitCounter = ssss.FirstOrDefault().Counter,
                                                                                                 ReceiptTotal = ssss.Sum(sm => (Decimal?)sm.Total),
                                                                                                 ReceiptTotalAfter = ssss.Sum(sm => (Decimal?)sm.Total),
                                                                                                 Room = ssss.FirstOrDefault().Room,//.Count() > 0 && s.FirstOrDefault().Room.FirstOrDefault() != null ? s.FirstOrDefault().Room.FirstOrDefault().RoomDescription : "Cash",//!= null ? s.FirstOrDefault().TransferToPms1.FirstOrDefault().RoomDescription : "",
                                                                                                 Table = ssss.FirstOrDefault().Table,
                                                                                                 Staff = ssss.FirstOrDefault().StaffId,
                                                                                                 AccountId = ssss.FirstOrDefault().AccountId,
                                                                                                 AccountDescription = ssss.FirstOrDefault().AccountDescription,
                                                                                                 Amount = ssss.Sum(sm => (Decimal?)sm.Total),
                                                                                                 ReceiptNo = ssss.FirstOrDefault().Abrreviation + ' ' + s.FirstOrDefault().Counter,
                                                                                             //ReceiptVatAnal1 = s.GroupBy(gg=>new {gg.VatId}),

                                                                                         })
                                                                         })
                                                     })
                                                 }
                                        )
            }).ToList();
            //  GetFlatSales
            return new { processedResults = groupedByDepartment };
        }

        private Object PosDailyRevenue(string filters, bool isWaiter = false)
        {

            //var a = GetFlatSales(filters);
            var detVat = from q in db.OrderDetailVatAnal.Where(w => (w.IsDeleted ?? false) == false)
                         select new
                         {
                             OrderDetailId = q.OrderDetailId,
                             Gross = q.Gross,
                             Net = q.Net,
                             VatRate = q.VatRate,
                             VatAmount = q.VatAmount,
                             VatId = q.VatId
                         };
            var detVatIng = from q in db.OrderDetailIgredientVatAnal.Where(w => (w.IsDeleted ?? false) == false)
                            select new
                            {
                                OrderDetailId = q.OrderDetailIgredients.OrderDetailId,
                                Gross = q.Gross,
                                Net = q.Net,
                                VatRate = q.VatRate,
                                VatAmount = q.VatAmount,
                                VatId = q.VatId
                            };


            var vatsAnal = detVat.Union(
            detVatIng).GroupBy(g => new { g.OrderDetailId, g.VatId }).Select(s => new
            {
                OrderDetailId = s.Key.OrderDetailId,
                VatId = s.Key.VatId,
                Net = s.Sum(sm => sm.Net),
                VatRate = s.FirstOrDefault().VatRate,
                VatAmount = s.Sum(sm => sm.VatAmount),
                Gross = s.Sum(sm => sm.Gross)

            });
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            List<long> eod = new List<long>();

            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
            else
                eod.Add(0);



            var flatVats = from q in db.OrderDetailInvoices.Include("OrderDetail.Transactions").AsNoTracking()
                                                       .Include("OrderDetail.Transactions.TransferToPms").AsNoTracking()
                                                       .Include("PosInfoDetail.PosInfo.Department").AsNoTracking()
                                                       .Include("OrderDetail.OrderDetailIgredients").AsNoTracking()
                                                       .Include("Staff").AsNoTracking()
                                                       .Include("OrderDetail.Transactions.Accounts").AsNoTracking()
                                                       .Include("OrderDetail.Table").AsNoTracking()
                                                       .Include("OrderDetail.Order").AsNoTracking()
                                                    .Where(w => w.PosInfoDetail.IsInvoice == true && (w.IsDeleted ?? false) == false//&& ((w.OrderDetail.Order.EndOfDayId ?? 0) == 0)
                                                    && eod.Contains((w.OrderDetail.Order.EndOfDayId ?? 0))
                                                    && w.OrderDetail.Status != 5
                                                    && flts.StaffList.Contains(w.StaffId.Value)
                                                    && flts.PosList.Contains(w.PosInfoDetail.PosInfoId)
                                               )
                           join va in vatsAnal on q.OrderDetailId equals va.OrderDetailId
                           select new
                           {
                               FODay = q.OrderDetail.Order.EndOfDay != null ? q.OrderDetail.Order.EndOfDay.FODay : null,
                               DepartmentId = q.PosInfoDetail.PosInfo.DepartmentId,
                               Department = q.PosInfoDetail.PosInfo.Department.Description,

                               PosInfoId = q.PosInfoDetail.PosInfoId,
                               PosInfoDescription = q.PosInfoDetail.PosInfo.Description,
                               StaffName = q.Staff.LastName,
                               InvoiceId = q.Id,
                               InvoiceGroup = q.PosInfoDetail.GroupId,
                               InvoiceAbbreviation = q.PosInfoDetail.Abbreviation,
                               InvoiceCounter = q.Counter,
                               InvoiceDescription = q.PosInfoDetail.Description,
                               Staff = q.StaffId,
                               IsInvoice = q.PosInfoDetail.IsInvoice,
                               VatId = va.VatId,
                               Net = va.Net,
                               VatRate = va.VatRate,
                               VatAmount = va.VatAmount,
                               Gross = va.Gross,
                               AccountId = q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.AccountId : null,
                               AccountDescription = q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.Accounts.Description : "",
                               TransferToPms1 = "9601",//q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.TransferToPms !=null ? q.OrderDetail.Transactions.TransferToPms.FirstOrDefault().RoomDescription : "Cash" : null,
                               Room = q.OrderDetail.Transactions.TransferToPms,
                               Table = q.OrderDetail.TableId != null ? q.OrderDetail.Table.Code : "",
                               TotalAfterDiscount = q.OrderDetail.TotalAfterDiscount,// + q.OrderDetail.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount),
                               Total = (Double)q.OrderDetail.Qty * (Double)q.OrderDetail.Price,
                               Day = q.OrderDetail.Order.Day

                           };
            //		flatVats.Dump();
            if (!isWaiter)
            {
                var groupedByDepartment = flatVats.ToList().GroupBy(g => g.FODay).Select(s => new
                {
                    FODay = s.Key,
                    Gross = s.Sum(sm => sm.Gross),
                    AllDepartments = s.GroupBy(gg => gg.DepartmentId)
                                                     .Select(ss => new
                                                     {
                                                         DepartmentId = ss.Key,
                                                         Department = ss.FirstOrDefault().Department,
                                                         Gross = ss.Sum(sm => sm.Gross),
                                                         AllPos = ss.GroupBy(g5 => g5.PosInfoId).Select(s9 => new
                                                         {
                                                             PosInfoId = s9.Key,
                                                             PosDescription = s9.FirstOrDefault().PosInfoDescription,
                                                             Gross = s9.Sum(sm => sm.Gross),
                                                             AllAccounts = s9.Select(sss => sss)
                                                                             .GroupBy(ggg => ggg.AccountId).Select(sss => new
                                                                             {
                                                                                 AccountId = sss.Key,
                                                                                 AccountDescription = sss.FirstOrDefault().AccountDescription,
                                                                                 Gross = sss.Sum(sm => sm.Gross),
                                                                                 Receipts = sss.GroupBy(g => new { g.DepartmentId, g.PosInfoId, g.InvoiceGroup, g.InvoiceAbbreviation, g.InvoiceCounter, g.Day })
                                                                                                 .Select(ssss => new
                                                                                                 {
                                                                                                     Day = s.FirstOrDefault().Day,
                                                                                                 //                                                  DepartmentId = s.Key.DepartmentId,
                                                                                                 //                                                  Deparmtent = s.FirstOrDefault().Department,
                                                                                                 ReceiptAbbr = ssss.FirstOrDefault().InvoiceAbbreviation,
                                                                                                     ReceitCounter = ssss.FirstOrDefault().InvoiceCounter,
                                                                                                     ReceiptTotal = ssss.Sum(sm => sm.Gross),
                                                                                                     ReceiptTotalAfter = ssss.Sum(sm => sm.TotalAfterDiscount),
                                                                                                     Room = ssss.FirstOrDefault().Room.Count() > 0 && s.FirstOrDefault().Room.FirstOrDefault() != null ? s.FirstOrDefault().Room.FirstOrDefault().RoomDescription : "Cash",//!= null ? s.FirstOrDefault().TransferToPms1.FirstOrDefault().RoomDescription : "",
                                                                                                     Table = ssss.FirstOrDefault().Table,
                                                                                                     Staff = ssss.FirstOrDefault().Staff,
                                                                                                     AccountId = ssss.FirstOrDefault().AccountId,
                                                                                                     AccountDescription = ssss.FirstOrDefault().AccountDescription,
                                                                                                     Amount = ssss.Sum(sm => sm.TotalAfterDiscount),
                                                                                                     ReceiptNo = ssss.FirstOrDefault().InvoiceAbbreviation + ' ' + s.FirstOrDefault().InvoiceCounter,
                                                                                                     ReceiptVatAnal = from v in db.Vat.ToList()
                                                                                                                      join jq in (ssss.GroupBy(gg => new { gg.VatId }).Select(s5 => new
                                                                                                                      {
                                                                                                                          VatId = s5.FirstOrDefault().VatId,
                                                                                                                          Net = s5.Sum(sm => sm.Net),
                                                                                                                          VatRate = s5.FirstOrDefault().VatRate,
                                                                                                                          VatAmount = s5.Sum(sm => sm.VatAmount),
                                                                                                                          Gross = s5.Sum(sm => sm.Gross)

                                                                                                                      })) on v.Id equals jq.VatId into ff
                                                                                                                      from all in ff.DefaultIfEmpty()
                                                                                                                      select new
                                                                                                                      {
                                                                                                                          VatId = v.Id,
                                                                                                                          Net = all != null ? all.Net : 0,
                                                                                                                          VatRate = v.Percentage,
                                                                                                                          VatAmount = all != null ? all.VatAmount : 0,
                                                                                                                          Gross = all != null ? all.Gross : 0
                                                                                                                      }
                                                                                                                  //ReceiptVatAnal1 = s.GroupBy(gg=>new {gg.VatId}),

                                                                                             })
                                                                             })
                                                         })
                                                     }
                                            )
                }).ToList();
                return new { processedResults = groupedByDepartment };
            }
            else
            {
                var groupedByWaiter = flatVats.ToList().GroupBy(g => g.FODay).Select(s => new
                {
                    FODay = s.Key,
                    Gross = s.Sum(sm => sm.Gross),
                    AllDepartments = s.GroupBy(gg => gg.DepartmentId)
                                                     .Select(ss => new
                                                     {
                                                         DepartmentId = ss.Key,
                                                         Department = ss.FirstOrDefault().Department,
                                                         Gross = ss.Sum(sm => sm.Gross),
                                                         AllPos = ss.GroupBy(g5 => g5.PosInfoId).Select(s9 => new
                                                         {
                                                             PosInfoId = s9.Key,
                                                             PosDescription = s9.FirstOrDefault().PosInfoDescription,
                                                             Gross = s9.Sum(sm => sm.Gross),
                                                             AllAccounts = s9.Select(sss => sss)
                                                                             .GroupBy(ggg => ggg.Staff).Select(sss => new
                                                                             {
                                                                                 Staff = sss.Key,
                                                                                 StaffName = sss.FirstOrDefault().StaffName,
                                                                             //AccountDescription = sss.FirstOrDefault().AccountDescription,
                                                                             Gross = sss.Sum(sm => sm.Gross),
                                                                                 Receipts = sss.GroupBy(g => new { g.AccountId })
                                                                                                 .Select(ssss => new
                                                                                                 {

                                                                                                     AccountDescription = ssss.FirstOrDefault().AccountDescription,
                                                                                                     Amount = ssss.Sum(sm => sm.TotalAfterDiscount),
                                                                                                     ReceiptsCount = ssss.GroupBy(g => new { g.PosInfoId, g.InvoiceGroup, g.InvoiceAbbreviation, g.InvoiceCounter, g.Day }).Count(),
                                                                                                     ReceiptVatAnal = from v in db.Vat.ToList()
                                                                                                                      join jq in (ssss.GroupBy(gg => new { gg.VatId }).Select(s5 => new
                                                                                                                      {
                                                                                                                          VatId = s5.FirstOrDefault().VatId,
                                                                                                                          Net = s5.Sum(sm => sm.Net),
                                                                                                                          VatRate = s5.FirstOrDefault().VatRate,
                                                                                                                          VatAmount = s5.Sum(sm => sm.VatAmount),
                                                                                                                          Gross = s5.Sum(sm => sm.Gross)

                                                                                                                      })) on v.Id equals jq.VatId into ff
                                                                                                                      from all in ff.DefaultIfEmpty()
                                                                                                                      select new
                                                                                                                      {
                                                                                                                          VatId = v.Id,
                                                                                                                          Net = all != null ? all.Net : 0,
                                                                                                                          VatRate = v.Percentage,
                                                                                                                          VatAmount = all != null ? all.VatAmount : 0,
                                                                                                                          Gross = all != null ? all.Gross : 0
                                                                                                                      }
                                                                                                                  //ReceiptVatAnal1 = s.GroupBy(gg=>new {gg.VatId}),

                                                                                             })
                                                                             })
                                                         })
                                                     }
                                            )
                });
                //var groupedByDepartment = flatVats.ToList().GroupBy(g => new { g.DepartmentId, g.PosInfoId, g.InvoiceGroup, g.InvoiceAbbreviation, g.InvoiceCounter, g.Day })
                //                                  .Select(s => new
                //                                  {
                //                                      Day = s.FirstOrDefault().Day,
                //                                      DepartmentId = s.Key.DepartmentId,
                //                                      Deparmtent = s.FirstOrDefault().Department,
                //                                      ReceiptAbbr = s.FirstOrDefault().InvoiceAbbreviation,
                //                                      ReceitCounter = s.FirstOrDefault().InvoiceCounter,
                //                                      ReceiptTotal = s.Sum(sm => sm.Gross),
                //                                      ReceiptTotalAfter = s.Sum(sm => sm.TotalAfterDiscount),
                //                                      Room = s.FirstOrDefault().Room.Count() > 0 &&  s.FirstOrDefault().Room.FirstOrDefault() != null?s.FirstOrDefault().Room.FirstOrDefault().RoomDescription:"Cash"  ,//!= null ? s.FirstOrDefault().TransferToPms1.FirstOrDefault().RoomDescription : "",
                //                                      Table = s.FirstOrDefault().Table,
                //                                      Staff = s.FirstOrDefault().Staff,
                //                                      AccountId = s.FirstOrDefault().AccountId,
                //                                      AccountDescription = s.FirstOrDefault().AccountDescription,
                //                                      Amount = s.Sum(sm => sm.TotalAfterDiscount),
                //                                      ReceiptNo = s.FirstOrDefault().InvoiceAbbreviation + ' ' + s.FirstOrDefault().InvoiceCounter,
                //                                      ReceiptVatAnal = from v in db.Vat.ToList()
                //                                                       join jq in (s.GroupBy(gg => new { gg.VatId }).Select(ss => new
                //                                                       {
                //                                                           VatId = ss.FirstOrDefault().VatId,
                //                                                           Net = ss.Sum(sm => sm.Net),
                //                                                           VatRate = ss.FirstOrDefault().VatRate,
                //                                                           VatAmount = ss.Sum(sm => sm.VatAmount),
                //                                                           Gross = ss.Sum(sm => sm.Gross)

                //                                                       })) on v.Id equals jq.VatId into ff
                //                                                       from all in ff.DefaultIfEmpty()
                //                                                       select new
                //                                                       {
                //                                                           VatId = v.Id,
                //                                                           Net = all != null ? all.Net : 0,
                //                                                           VatRate = v.Percentage,
                //                                                           VatAmount = all != null ? all.VatAmount : 0,
                //                                                           Gross = all != null ? all.Gross : 0
                //                                                       }
                //                                      //ReceiptVatAnal1 = s.GroupBy(gg=>new {gg.VatId}),
                //                                  }).ToList();

                //var processedResults = groupedByDepartment;
                //.GroupBy(g => g.AccountId).Select(s => new
                //{
                //    AccountId = s.Key,
                //    AccountDescription = s.FirstOrDefault().AccountDescription,
                //    Receipts = s.Select(ss => ss)
                //});
                return new { processedResults = groupedByWaiter };
            }
            //   return null;
        }

        private Object TransferPerPmsDepartment(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            List<long> eod = new List<long>();

            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
            else
                eod.Add(0);

            var flat = db.TransferToPms.Include("Transactions")
                                       .Include("Transactions.Order")
                                       .Include("Transactions.Staff")
                                       .Include("Transactions.PosInfo")
                                       .Where(w => eod.Contains(w.EndOfDayId ?? 0)).GroupBy(g => g.PmsDepartmentId).Select(s => new
                                       {
                                           PmsDepratmentId = s.Key,
                                           PmsDepartment = s.FirstOrDefault().PmsDepartmentDescription,
                                           Receipts = s.Where(ww => ww.ProfileName != "Cash").Select(ss => new
                                           {
                                               OriginalObj = ss,
                                               Day = ss.Transactions.Day,
                                               AccountType = ss.Transactions.Accounts.Description,
                                               TransferDate = ss.SendToPmsTS,
                                               RegNo = ss.RegNo,
                                               Guest = ss.ProfileName,
                                               Room = ss.RoomDescription,
                                               ReceiptNo = ss.ReceiptNo,
                                               SendToPms = ss.SendToPMS,
                                               Amount = ss.Total,
                                               StaffId = ss.Transactions.Staff.LastName,
                                               PosInfo = ss.Transactions.PosInfo.Description,
                                               ErrorMessage = ss.ErrorMessage
                                           }),
                                           AccountTotal = s.Where(ww => ww.ProfileName != "Cash").Sum(sm => sm.Total),
                                           CashTotal = s.Where(ww => ww.ProfileName == "Cash" && ww.Status == 0).Sum(sm => sm.Total)
                                       });

            return new { processedResults = flat };
        }

        private Object TransferPmsDepartmentPerInvoice(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);


            List<long> eod = new List<long>();

            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
            else
                eod.Add(0);
            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            var invsfromtrans = from q in db.Invoices.Where(w => flts.PosList.Contains(w.PosInfoId) && eod.Contains(w.EndOfDayId ?? 0))
                                join qq in db.InvoiceTypes on q.InvoiceTypeId equals qq.Id
                                join qqq in db.Transactions on q.Id equals qqq.InvoicesId
                                join qqqq in db.PosInfo on q.PosInfoId equals qqqq.Id
                                select new
                                {
                                    Day = q.Day,
                                    DepartmentId = qqqq.DepartmentId,
                                    TranId = qqq.Id,
                                    InvoiceId = q.Id,
                                    InvoiceDesc = q.Description,
                                    InvoiceCounter = q.Counter,
                                    InvoiceAbbr = qq.Abbreviation,
                                    InvoiceTotal = q.Total,
                                    PosInfoId = q.PosInfoId,
                                    PosDescreption = qqqq.Description,
                                    AccountId = qqq.AccountId,
                                    StaffId = q.StaffId

                                };

            var flat = (from q in invsfromtrans
                        join qq in db.TransferToPms.Where(w => (w.Status ?? 0) != 3) on q.TranId equals qq.TransactionId
                        join qqq in db.Accounts on q.AccountId equals qqq.Id
                        join qqqq in db.Department on q.DepartmentId equals qqqq.Id into f
                        from dp in f.DefaultIfEmpty()
                        join qqqqq in db.Staff on q.StaffId equals qqqqq.Id
                        select new
                        {
                            StoreId = storeid,
                            Day = q.Day,
                            InvoiceId = q.InvoiceId,
                            InvoiceDesc = q.InvoiceDesc,
                            InvoiceCounter = q.InvoiceCounter,
                            InvoiceAbbr = q.InvoiceAbbr,
                            InvoiceTotal = q.InvoiceTotal,
                            DepartmentId = q.DepartmentId,
                            DepartmentDescription = dp != null ? dp.Description : "",
                            PosInfoId = q.PosInfoId,
                            PosDescreption = q.PosDescreption,
                            AccountId = q.AccountId,
                            AccountDescription = qqq.Description,
                            PmsDepartmentId = qq.PmsDepartmentId,
                            Description = qq.Description,
                            ProfileName = qq.ProfileName,
                            RoomDescription = qq.RoomDescription,
                            ReceiptNo = qq.ReceiptNo,
                            PMSDepartment = qq.PmsDepartmentDescription,
                            TotalForPMS = qq.Total,
                            StaffId = q.StaffId,
                            StaffCode = qqqqq.Code,
                            StaffName = qqqqq.LastName
                        }).ToList();
            /*.GroupBy(g => g.InvoiceId).Select(ss => new
             {
                 StorId = 1,
                 DepartmentId = ss.FirstOrDefault().DepartmentId,
                 DepartmentDescription = ss.FirstOrDefault().DepartmentDescription,
                 Day = ss.FirstOrDefault().Day,

                 PosInfoId = ss.FirstOrDefault().PosInfoId,
                 PosInfoDescription = ss.FirstOrDefault().PosDescreption,
                 InvoiceId = ss.FirstOrDefault().InvoiceId,
                 Description = ss.FirstOrDefault().InvoiceDesc,
                 Abbreviation = ss.FirstOrDefault().InvoiceAbbr,
                 Counter = ss.FirstOrDefault().InvoiceCounter,
                 InvoiceTotal = ss.FirstOrDefault().InvoiceTotal,
                 Details = ss.Select(sss => new
                 {
                     AccountId = sss.AccountId,
                     AccountDescription = sss.AccountDescription,
                     PmsDepartmentId = sss.PmsDepartmentId,
                     PmsDepartmentDescription = sss.PMSDepartment,
                     Room = sss.RoomDescription,
                     ProfileName = sss.ProfileName,
                     ReceiptNo = sss.ReceiptNo,
                     Total = sss.TotalForPMS
                 })
             });*/
            //var flat = db.TransferToPms.Select(s => new
            //{
            //    PosInfoId = s.Transactions.PosInfoId,
            //    PosDescreption = s.Transactions.PosInfo.Description,
            //    PmsDepartmentId = s.PmsDepartmentId,
            //    Description = s.Description,
            //    ProfileName = s.ProfileName,
            //    RoomDescription = s.RoomDescription,
            //    ReceiptNo = s.ReceiptNo,
            //    PMSDepartment = s.PmsDepartmentDescription,
            //    TotalForPMS = s.Total,
            //    AccountId = s.Transactions.AccountId,
            //    AccountDescription = s.Transactions.Accounts.Description,
            //    InvoiceId = s.Transactions.InvoicesId,
            //    InvoiceDesc = s.Transactions.Invoices.Description,
            //    InvoiceCounter = s.Transactions.Invoices.Counter,
            //    InvoiceAbbr = s.Transactions.Invoices.InvoiceTypes.Abbreviation,
            //    InvoiceTotal = s.Transactions.Invoices.Total
            //}).ToList().GroupBy(g => g.InvoiceId).Select(ss => new
            //{
            //    PosInfoId = ss.FirstOrDefault().PosInfoId,
            //    PosDescreption = ss.FirstOrDefault().PosDescreption,
            //    InvoiceId = ss.FirstOrDefault().InvoiceId,
            //    Descritpion = ss.FirstOrDefault().InvoiceDesc,
            //    Abbreviation = ss.FirstOrDefault().InvoiceAbbr,
            //    Counter = ss.FirstOrDefault().InvoiceCounter,
            //    InvoiceTotal = ss.FirstOrDefault().InvoiceTotal,
            //    Details = ss.Select(sss => new
            //    {
            //        AccountId = sss.AccountId,
            //        AccountDescription = sss.AccountDescription,
            //        PmsDepartmentId = sss.PmsDepartmentId,
            //        PmsDepartmentDescription = sss.PMSDepartment,
            //        ProfileName = sss.ProfileName,
            //        ReceiptNo = sss.ReceiptNo,
            //        Total = sss.TotalForPMS

            //    })
            //});


            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
            return new { processedResults = flat, metadata };
        }


        private Object ProductForEodStats(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            List<long> eod = new List<long>();

            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) >= EntityFunctions.TruncateTime(flts.FromDate) &&
                                            EntityFunctions.TruncateTime(w.FODay) <= EntityFunctions.TruncateTime(flts.ToDate)).Select(s => s.Id).ToList();
            else
                eod.Add(0);

            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            var prods = db.Invoices.Where(w => eod.Contains(w.EndOfDayId ?? 0) && flts.PosList.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false && (w.IsVoided ?? false) == false)
                                  .SelectMany(w => w.OrderDetailInvoices).Select(s => new
                                  {
                                      PosInfoId = s.Invoices.PosInfoId,
                                      PosInfoDescription = s.Invoices.PosInfo.Description,
                                      DepartmentId = s.Invoices.PosInfo.DepartmentId,
                                      DepartmentDescription = s.Invoices.PosInfo.Department.Description,
                                      ProductId = s.OrderDetail.ProductId,
                                      Qty = s.OrderDetail.Qty,
                                      Total = s.OrderDetail.TotalAfterDiscount
                                  });

            var query = (from q in db.ProductsForEOD
                         join qq in prods on q.ProductId equals qq.ProductId
                         select new
                         {
                             DepartmentId = qq.DepartmentId,
                             DepartmentDescription = qq.DepartmentDescription,
                             PosInfoId = qq.PosInfoId,
                             PosInfoDescription = qq.PosInfoDescription,
                             ProductId = q.ProductId,
                             Description = q.Description,
                             Qty = qq.Qty,
                             Total = qq.Total
                         }).ToList().GroupBy(g => new { g.ProductId, g.PosInfoId }).Select(s => new
                         {
                             StoreId = storeid,
                             DepartmentId = s.FirstOrDefault().DepartmentId,
                             DepartmentDescription = s.FirstOrDefault().DepartmentDescription,

                             PosInfoId = s.FirstOrDefault().PosInfoId,
                             PosInfoDescription = s.FirstOrDefault().PosInfoDescription,

                             ProductId = s.FirstOrDefault().ProductId,
                             Description = s.FirstOrDefault().Description,
                             Qty = s.Sum(sm => sm.Qty),
                             Total = s.Sum(sm => sm.Total)
                         });

            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
            return new { processedResults = query, metadata };
        }

        private Object ProductsPerPricelist(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            db.Configuration.LazyLoadingEnabled = true;
            var joinCats = from q in db.ProductCategories
                           join qq in db.Categories on q.CategoryId equals qq.Id
                           into f
                           from cp in f.DefaultIfEmpty()
                           select new
                           {
                               ProductCategoryId = q.Id,
                               ProductCategoryDescription = q.Description,
                               CategoryId = (cp == null) ? -1 : cp.Id,
                               CategoryDescription = (cp == null) ? "No Description" : cp.Description
                           };


            var flatProduct = from q in db.Product
                              join qq in joinCats on q.ProductCategoryId equals qq.ProductCategoryId
                              into f
                              from pc in f.DefaultIfEmpty()
                              select new
                              {
                                  ProductId = q.Id,
                                  ProductDescription = q.Description,
                                  ProductCategoryId = q.ProductCategoryId,
                                  ProductCategoryDescription = q.ProductCategoryId != null ? pc.ProductCategoryDescription : "",
                                  CategoryId = q.ProductCategoryId == null ? -1 : pc.CategoryId,
                                  CategoryDescription = q.ProductCategoryId == null ? "No Description" : pc.CategoryDescription
                              };
            var flatPriceList = from q in db.PricelistDetail
                                join qq in db.Pricelist on q.PricelistId equals qq.Id
                                join qqq in flatProduct on q.ProductId equals qqq.ProductId
                                join qqqq in db.Vat on q.VatId equals qqqq.Id
                                select new
                                {
                                    ProductId = qqq.ProductId,
                                    ProductDescription = qqq.ProductDescription,
                                    ProductCategoryId = qqq.ProductCategoryId,
                                    ProductCategoryDescription = qqq.ProductCategoryDescription,
                                    PriceListId = qq.Id,
                                    PriceListDescrition = qq.Description,
                                    Price = q.Price,
                                    CategoryId = qqq.CategoryId,
                                    CategoryDescription = qqq.CategoryDescription,
                                    VatId = qqqq.Id,
                                    VatCode = qqqq.Code,
                                    VatPercentage = qqqq.Percentage,

                                };
            db.Configuration.LazyLoadingEnabled = false;


            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
            return new { processedResults = flatPriceList, metadata };

        }
        private Object MealConsumptions(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            List<long> eod = new List<long>();
            //            var poslist = db.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId)).Select(s=>s.Id);
            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) >= EntityFunctions.TruncateTime(flts.FromDate) &&
                                        EntityFunctions.TruncateTime(w.FODay) <= EntityFunctions.TruncateTime(flts.ToDate)).Select(s => s.Id).ToList();
            else
                eod.Add(0);

            var query = db.MealConsumption.Where(w => eod.Contains(w.EndOfDayId.Value)).Select(s => new
            {
                EndOfDayId = s.EndOfDayId,
                DepartmentId = s.DepartmentId,
                DepartmentDescription = s.Department.Description,
                PosInfoId = s.PosInfoId,
                PosInfoDescription = s.PosInfo.Description,
                GuestName = s.Guest.LastName,
                GuestId = s.GuestId,
                ReservationNo = s.Guest.ReservationCode,
                Room = s.Guest.Room,
                ConsumedMeals = s.ConsumedMeals,
                ConsumedDay = s.ConsumedTS,
                Board = s.Guest.BoardName,
                BoardId = s.Guest.BoardCode,
                Adults = s.Guest.Adults,
                Children = s.Guest.Children,
                ReservationId = s.ReservationId,
            });

            var flat = from q in query
                       join qq in db.AllowedMealsPerBoard on q.BoardId equals qq.BoardId
                       select new
                       {
                           StoreId = storeid,
                           EndOfDayId = q.EndOfDayId,
                           DepartmentId = q.DepartmentId,
                           DepartmentDescription = q.DepartmentDescription,
                           PosInfoId = q.PosInfoId,
                           PosInfoDescription = q.PosInfoDescription,
                           GuestName = q.GuestName,
                           GuestId = q.GuestId,
                           ReservationNo = q.ReservationNo,
                           Room = q.Room,
                           ConsumedMeals = q.ConsumedMeals,
                           ConsumptionDate = q.ConsumedDay,
                           Board = q.Board,
                           BoardId = q.BoardId,
                           Adults = q.Adults,
                           Children = q.Children,
                           ReservationId = q.ReservationId,
                           AllowedAdultMeals = qq.AllowedMeals,
                           AllowedChildMeals = qq.AllowedMealsChild,
                           AllowedAdultDiscount = qq.AllowedDiscountAmount,
                           AllowedChildDiscount = qq.AllowedDiscountAmountChild,

                       };
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);
            return new { processedResults = flat, metadata };
        }


        private Object SalesForErp(string filters)
        {
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long?> staffList = new List<long?>();
            if (String.IsNullOrEmpty(filters))
                eod.Add(0);
            else
            {
                var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

                if (flts.UseEod)
                {
                    if (flts.EodId == null)
                    {
                        if (flts.UsePeriod)
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                                                                EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                        }
                        else
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                        }
                    }
                    else
                    {
                        eod.Add((long)flts.EodId);
                    }
                }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
                staffList.AddRange(flts.StaffList.ToList());
            }

            var flat = (from q in db.OrderDetailInvoices
                        join od in db.OrderDetail on q.OrderDetailId equals od.Id
                        join o in db.Order on od.OrderId equals o.Id
                        join endofday in db.EndOfDay on o.EndOfDayId equals endofday.Id
                        join pid in db.PosInfoDetail on q.PosInfoDetailId equals pid.Id
                        join pi in db.PosInfo on pid.PosInfoId equals pi.Id
                        join dep in db.Department on pi.DepartmentId equals dep.Id
                        join pld in db.PricelistDetail on od.PriceListDetailId equals pld.Id
                        join pl in db.Pricelist on pld.PricelistId equals pl.Id
                        join tran in db.Transactions on od.TransactionId equals tran.Id
                        join acc in db.Accounts on tran.AccountId equals acc.Id
                        join p in db.Product on od.ProductId equals p.Id
                        join t in db.Table on od.TableId equals t.Id into ff
                        from tbl in ff.DefaultIfEmpty()
                        join gue in db.Guest on od.GuestId equals gue.Id into fff
                        from guest in fff.DefaultIfEmpty()
                        where pid.IsInvoice == true &&
                              eod.Contains((o.EndOfDayId ?? 0))
                                && posList.Contains(pi.Id)
                        //&& staffList.Contains(s.StaffId)

                        select new
                        {
                            DepartmentId = pi.DepartmentId,
                            Department = dep.Description,
                            FODay = endofday.FODay,
                            PosInfoId = pid.PosInfoId,
                            PosInfoDescription = pi.Description,
                            OrderId = od.OrderId,
                            InvoiceId = q.Id,
                            InvoiceGroup = pid.GroupId,
                            InvoiceAbbreviation = pid.Abbreviation,
                            InvoiceCounter = q.Counter,
                            //                           InvoiceDescription = pid.Description,
                            //Cancelation 
                            Staff = q.StaffId,
                            VatId = pld.VatId,
                            ProductCategoryId = p.ProductCategoryId,
                            IsInvoice = pid.IsInvoice,
                            IsVoid = pid.IsCancel,
                            ProductId = p.Code,
                            Product = p.Description,
                            Room = guest != null ? guest.Room : "",
                            Table = tbl != null ? tbl.Code : "",
                            TotalAfterDiscount = od.TotalAfterDiscount, //+ q.OrderDetail.OrderDetailIgredients.Count() > 0 ? q.OrderDetail.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0,
                            Total = od.Qty * (Double?)od.Price,
                            UnitPrice = pld.Price,
                            Qty = od.Qty,
                            PriceList = pld.PricelistId,
                            ItemDiscount = od.Discount,
                            Day = q.OrderDetail.Order.Day,
                            ReservationCode = guest != null ? guest.ReservationCode : "",
                            Guest = guest != null ? guest.LastName : "",
                            AccountId = tran.AccountId,
                            AccountDescription = od.TransactionId != null ? acc.Description : "",
                            //TransferToPms1 = q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.TransferToPms.Count() > 0 ? q.OrderDetail.Transactions.TransferToPms : null : null,
                            //     SendToPMS = od.TransactionId != null ? q.OrderDetail.Transaction.TransactionTransferToPms.Count() > 0 ? q.OrderDetail.Transaction.TransactionTransferToPms.FirstOrDefault().SendToPMS : null : null

                        }).OrderBy(o => o.Day);

            return new { processedResults = flat };
        }

        private Object ExportForGoodys(string filters)
        {

            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long> itList = new List<long>();
            itList.AddRange(db.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void).Select(s => s.Id));
            if (String.IsNullOrEmpty(filters))
                eod.Add(0);
            else
            {
                var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
                if (flts.UseEod)
                {
                    if (flts.EodId == null)
                    {
                        if (flts.UsePeriod)
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                            EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                        }
                        else
                        {
                            eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                        }
                    }
                    else
                    {
                        eod.Add((long)flts.EodId);
                    }
                }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
            }

            db.Configuration.LazyLoadingEnabled = true;
            var odIngVats = from q in db.OrderDetailIgredientVatAnal.Where(w => (w.IsDeleted ?? false) == false)
                            join od in db.OrderDetailIgredients.Where(w => (w.IsDeleted ?? false) == false) on q.OrderDetailIgredientsId equals od.Id
                            join odi in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on od.OrderDetailId equals odi.OrderDetailId
                            join inv in db.Invoices.Where(w => (eod.Contains(w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId.Value) && (w.IsDeleted ?? false) == false && itList.Contains(w.InvoiceTypeId.Value)) on odi.InvoicesId equals inv.Id
                            join v in db.Vat on q.VatId equals v.Id
                            select new
                            {
                                InvoiceId = inv.Id,
                                Id = q.Id,
                                OrderDetailId = odi.Id,
                                Total = q.Gross == null ? 0 : q.Gross,
                                VatId = v.Code,
                                Waiter = inv.StaffId,
                                Amount = od.PricelistDetail.Price,
                                Qty = od.Qty,
                                PosInfoId = inv.PosInfoId,
                                Item_Code = od.Ingredients.Code,
                                Item_Descr = od.Ingredients.SalesDescription,
                                Item_SubGroup = od.OrderDetail.Product.ProductCategoryId,
                                Item_Group = od.OrderDetail.Product.ProductCategories != null ? (Int64?)od.OrderDetail.Product.ProductCategories.CategoryId : null,
                                Table = inv.Table != null ? inv.Table.Code : null,
                                Status = od.OrderDetail.Status,
                                Counter = inv.Counter,
                                Day = inv.Day,
                                DepartmentId = inv.PosInfo.DepartmentId,
                                Listino = od.PricelistDetail.Pricelist.Code == "2" ? 2 : od.OrderDetail.SalesTypeId == 1 ? 1 : 5,
                                CloseId = inv.EndOfDay.CloseId == 0 ? inv.EndOfDayId : inv.EndOfDay.CloseId,
                                FODay = inv.EndOfDay.FODay
                            };

            var odVats = from q in db.OrderDetailVatAnal.Where(w => (w.IsDeleted ?? false) == false)
                         join od in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false) on q.OrderDetailId equals od.Id
                         join odi in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on od.Id equals odi.OrderDetailId
                         join inv in db.Invoices.Where(w => (eod.Contains(w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId.Value) && (w.IsDeleted ?? false) == false && itList.Contains(w.InvoiceTypeId.Value)) on odi.InvoicesId equals inv.Id
                         join v in db.Vat on q.VatId equals v.Id
                         select new
                         {
                             InvoiceId = inv.Id,
                             Id = q.Id,
                             OrderDetailId = odi.Id,
                             Total = q.Gross == null ? 0 : q.Gross,
                             VatId = v.Code,
                             Waiter = inv.StaffId,
                             Amount = od.PricelistDetail.Price,
                             Qty = od.Qty,
                             PosInfoId = inv.PosInfoId,
                             Item_Code = od.Product.Code,
                             Item_Descr = od.Product.SalesDescription,
                             Item_SubGroup = od.Product.ProductCategoryId,
                             Item_Group = od.Product.ProductCategories != null ? (Int64?)od.Product.ProductCategories.CategoryId : null,
                             Table = inv.Table != null ? inv.Table.Code : null,
                             Status = od.Status,
                             Counter = inv.Counter,
                             Day = inv.Day,
                             DepartmentId = inv.PosInfo.DepartmentId,
                             Listino = od.PricelistDetail.Pricelist.Code == "2" ? 2 : od.SalesTypeId == 1 ? 1 : 5,
                             CloseId = inv.EndOfDay.CloseId == 0 ? inv.EndOfDayId : inv.EndOfDay.CloseId,
                             FODay = inv.EndOfDay.FODay
                         };
            var joinedVats = odVats.Union(odIngVats).ToList().GroupBy(g => g.InvoiceId)
                                                    .Select(s => new
                                                    {
                                                        InvoiceId = s.Key,
                                                        Vats = s.GroupBy(gg => gg.VatId).Select(sss => new
                                                        {
                                                            VatId = sss.Key,
                                                            Total = sss.Sum(sm => sm.Total)
                                                        })
                                                    });


            var invs = (from q in db.Invoices.Where(w => (w.IsDeleted ?? false) == false && itList.Contains(w.InvoiceTypeId.Value))//&& (w.IsVoided ?? false) == false)//.ToList()
                                                                                                                                   //join it in db.InvoiceTypes&& itList.Contains(w.InvoiceTypeId.Value)) on q.InvoiceTypeId equals it.Id
                        join ed in db.EndOfDay.Where(w => eod.Contains(w.Id)) on q.EndOfDayId equals ed.Id
                        join pi in db.PosInfo.Where(w => posList.Contains(w.Id)) on q.PosInfoId equals pi.Id
                        select new
                        {
                            InvoiceId = q.Id,
                            Pos = q.PosInfoId,
                            Dept = pi.DepartmentId,
                            REC_DATE = q.Day,
                            REC_TIME = q.Day,//.Time,
                            HOT_DATE = q.Day,
                            CLOSE_ID = ed.CloseId == 0 ? ed.Id : (Int64?)ed.CloseId,
                            FO_DAY = ed.FODay,
                            Room = q.IsVoided == true ? 0 : 9601,//q.Transactions.FirstOrDefault().AccountId == 1?9601:q.Transactions.FirstOrDefault().AccountId == 1
                            WAITER = q.StaffId,
                            TTABLE = q.Table.Code,
                            Total = q.Total * 100,
                            RECEIPT = q.Counter,
                            Listino = q.OrderDetailInvoices.FirstOrDefault().OrderDetail.PricelistDetail.Pricelist.Code == "2" ? 2 : q.OrderDetailInvoices.FirstOrDefault().OrderDetail.SalesTypeId == 1 ? 1 : 5,// == 1 ? 1 : 5,
                        }).ToList();

            //var labPos = (from q in db.Invoices.Where(w => (eod.Contains(w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId.Value) && (w.IsDeleted ?? false) == false && (w.InvoiceTypeId == 1 || w.InvoiceTypeId == 4))
            //              join jv in joinedVats on q.Id equals jv.InvoiceId
            var labPos = (from q in invs

                          join jv in joinedVats on q.InvoiceId equals jv.InvoiceId
                          select new
                          {
                              InvoiceId = q.InvoiceId,
                              Pos = q.Pos,
                              Dept = q.Dept,
                              REC_DATE = q.REC_DATE,
                              REC_TIME = q.REC_TIME,//.Time,
                              HOT_DATE = q.HOT_DATE,
                              CLOSE_ID = q.CLOSE_ID,
                              FO_DAY = q.FO_DAY,
                              Room = q.Room,
                              WAITER = q.WAITER,
                              TTABLE = q.TTABLE,
                              Total = q.Total,

                              VAT4 = jv.Vats.Where(w => w.VatId == 1).Sum(sm => sm.Total) * 100,
                              VAT8 = jv.Vats.Where(w => w.VatId == 2).Sum(sm => sm.Total) * 100,
                              VAT18 = jv.Vats.Where(w => w.VatId == 3).Sum(sm => sm.Total) * 100,
                              VAT36 = 0,//q.OrderDetailInvoices.Where(w=> w.OrderDetail.OrderDetailVatAnal.FirstOrDefault().VatId == 1).Sum(sm=>sm.OrderDetail.OrderDetailVatAnal.FirstOrDefault().Gross ) * 100,
                              VAT0 = jv.Vats.Where(w => w.VatId == 4).Sum(sm => sm.Total) * 100,
                              RECEIPT = q.RECEIPT,
                              //        Listino = q.OrderDetailInvoices.FirstOrDefault().OrderDetail.PriceListDetail.PricelistId == 2?2: q.OrderDetailInvoices.FirstOrDefault().OrderDetail.SalesTypeId == 1 ? 1 : 5,// == 1 ? 1 : 5,
                              Listino = q.Listino
                              //select new
                              //{
                              //Pos = q.PosInfoId,
                              //Dept = q.PosInfo.DepartmentId,
                              //REC_DATE = q.Day,
                              //REC_TIME = q.Day,//.Time,
                              //HOT_DATE = q.Day,
                              //CLOSE_ID = q.EndOfDay.CloseId == 0 ? q.EndOfDayId : q.EndOfDay.CloseId,
                              //FO_DAY = q.EndOfDay.FODay,
                              //Room = q.IsVoided == true ? 0 : 9601,//q.Transactions.FirstOrDefault().AccountId == 1?9601:q.Transactions.FirstOrDefault().AccountId == 1
                              //WAITER = q.StaffId,
                              //TTABLE = q.Table.Code,
                              //Total = q.Total * 100,
                              //VAT4 = jv.Vats.Where(w => w.VatId == 1).Sum(sm => sm.Total) * 100,
                              //VAT8 = jv.Vats.Where(w => w.VatId == 2).Sum(sm => sm.Total) * 100,
                              //VAT18 = jv.Vats.Where(w => w.VatId == 3).Sum(sm => sm.Total) * 100,
                              //VAT36 = 0,//q.OrderDetailInvoices.Where(w=> w.OrderDetail.OrderDetailVatAnal.FirstOrDefault().VatId == 1).Sum(sm=>sm.OrderDetail.OrderDetailVatAnal.FirstOrDefault().Gross ) * 100,
                              //VAT0 = jv.Vats.Where(w => w.VatId == 4).Sum(sm => sm.Total) * 100,
                              //RECEIPT = q.Counter,
                              //Listino = q.OrderDetailInvoices.FirstOrDefault().OrderDetail.PricelistDetail.PricelistId == 2 ? 2 : q.OrderDetailInvoices.FirstOrDefault().OrderDetail.SalesTypeId == 1 ? 1 : 5,// == 1 ? 1 : 5,
                          }).ToList();
            var dets = odVats.Union(odIngVats).Distinct();//.ToList();

            var query = (from q in dets

                         select new LabStore()
                         {
                             Pos = q.PosInfoId,
                             Dept = q.DepartmentId,
                             REC_DATE = q.Day,
                             REC_TIME = q.Day,//.Time,
                             HOT_DATE = q.Day,
                             FO_DAY = q.FODay,
                             CLOSE_ID = q.CloseId,
                             ITEM_GROUP = q.Item_Group,
                             ITEM_SUBGROUP = q.Item_SubGroup,
                             ITEM_DESCR = q.Item_Descr,
                             ITEM_CODE = q.Item_Code,
                             QTY = q.Qty,
                             AMOUNT = q.Amount * 100,
                             Total = q.Total * 100,
                             Waiter = q.Waiter,
                             TTable = q.Table,
                             Room = q.Status == 5 ? 0 : 9601,
                             Vat = q.VatId,
                             Listino = q.Listino,
                             Receipt = q.Counter,
                         }).ToList<LabStore>();

            /* 
            var query = (from q in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false)
                         join odi in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals odi.OrderDetailId
                         join inv in db.Invoices.Where(w => posList.Contains(w.PosInfoId.Value) && eod.Contains(w.EndOfDayId ?? 0) && (w.IsDeleted ?? false) == false && itList.Contains(w.InvoiceTypeId.Value)) on odi.InvoicesId equals inv.Id
                         join pi in db.PosInfo on inv.PosInfoId equals pi.Id
                         join ed in db.EndOfDay on inv.EndOfDayId equals ed.Id
                         join p in db.Product on q.ProductId equals p.Id
                         join odva in db.OrderDetailVatAnal on q.Id equals odva.OrderDetailId
                         join v in db.Vat on odva.VatId equals v.Id
                         select new LabStore()
                         {
                             Odi = odi.Id,
                             Pos = inv.PosInfoId,
                             Dept = pi.DepartmentId,
                             REC_DATE = inv.Day,
                             REC_TIME = inv.Day,//.Time,
                             HOT_DATE = inv.Day,
                             FO_DAY = ed.FODay,
                             CLOSE_ID = ed.CloseId == 0 ? ed.Id : (Int64?)ed.CloseId,
                             ITEM_GROUP = p.ProductCategories != null ? (Int64?)p.ProductCategories.CategoryId : null,
                             ITEM_SUBGROUP = p.ProductCategoryId,
                             ITEM_DESCR = p.SalesDescription,
                             ITEM_CODE = p.Code,
                             QTY = q.Qty,
                             AMOUNT = q.PricelistDetail.Price * 100,
                             Total = q.TotalAfterDiscount.Value * (Decimal)100,//(Math.Round(q.TotalAfterDiscount.Value * (Decimal)100, 1, MidpointRounding.AwayFromZero)),
                             Waiter = inv.StaffId,
                             TTable = q.Table != null ? q.Table.Code : null,
                             Room = q.Status == 5 ? 0 : 9601,
                             Vat = v.Code,// == 1 ? 2 : q.PricelistDetail.Vat.Code == 2 ? 3 : 0 : 0,
                             Listino = q.PricelistDetail.Pricelist.Code == "2" ? 2 : q.SalesTypeId == 1 ? 1 : 5,// == 1 ? 1 : 5,
                             Receipt = inv.Counter,
                         }).ToList<LabStore>();

            var queryIng = (from oding in db.OrderDetailIgredients.Where(w => (w.IsDeleted ?? false) == false)
                            join q in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false) on oding.OrderDetailId equals q.Id
                            join odi in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals odi.OrderDetailId
                            join inv in db.Invoices.Where(w => posList.Contains(w.PosInfoId.Value) && eod.Contains(w.EndOfDayId ?? 0) && (w.IsDeleted ?? false) == false && itList.Contains(w.InvoiceTypeId.Value)) on odi.InvoicesId equals inv.Id
                            join pi in db.PosInfo on inv.PosInfoId equals pi.Id
                            join ed in db.EndOfDay on inv.EndOfDayId equals ed.Id
                            join ing in db.Ingredients on oding.IngredientId equals ing.Id
                            join p in db.Product on q.ProductId equals p.Id
                            join odva in db.OrderDetailIgredientVatAnal on q.Id equals odva.OrderDetailIgredientsId
                            join v in db.Vat on odva.VatId equals v.Id
                            select new LabStore()
                            {
                                Odi = odi.Id,
                                Pos = inv.PosInfoId,
                                Dept = pi.DepartmentId,
                                REC_DATE = inv.Day,
                                REC_TIME = inv.Day,//.Time,
                                HOT_DATE = inv.Day,
                                FO_DAY = ed.FODay,
                                CLOSE_ID = ed.CloseId == 0 ? ed.Id : (Int64?)ed.CloseId,
                                ITEM_GROUP = p.ProductCategories != null ? (Int64?)p.ProductCategories.CategoryId : null,
                                ITEM_SUBGROUP = p.ProductCategoryId,
                                ITEM_DESCR = ing.SalesDescription,
                                ITEM_CODE = ing.Code,
                                QTY = q.Qty,
                                AMOUNT = oding.PricelistDetail.Price * 100,
                                Total = oding.TotalAfterDiscount.Value * (Decimal)100,//Math.Round(oding.TotalAfterDiscount.Value * (Decimal)100, 1, MidpointRounding.AwayFromZero),
                                Waiter = inv.StaffId,
                                TTable = q.Table != null ? q.Table.Code : null,
                                Room = q.Status == 5 ? 0 : 9601,
                                Vat = v.Code,// == 1 ? 2 : oding.PricelistDetail.Vat.Code == 2 ? 3 : 0 : 0,
                                Listino = oding.PricelistDetail.Pricelist.Code == "2" ? 2 : q.SalesTypeId == 1 ? 1 : 5,// == 1 ? 1 : 5,
                                Receipt = inv.Counter,
                            }).ToList<LabStore>();
             */
            db.Configuration.LazyLoadingEnabled = false;

            return new { LabPos = labPos, LabStore = query };

        }
        private Object SalesPerDepartmentsCheckingFromInvoicesFlatWithVoids(string filters, bool voided)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            List<Int64> eod;
            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FromDate)).Select(s => s.Id).ToList();
            else
                eod = new List<Int64>() { 0 };
            List<Int64?> poslist = flts.PosList;
            var itList = db.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void);


            db.Configuration.LazyLoadingEnabled = true;
            var storeid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Description : "";
            var query = from q in db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && flts.StaffList.Contains(w.StaffId)
                                                     && poslist.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false)
                            //&& (w.IsVoided ?? false) == voided)
                        join t in db.Transactions.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals t.InvoicesId into fffff
                        from trans in fffff.DefaultIfEmpty()
                        join qqq in itList on q.InvoiceTypeId equals qqq.Id
                        select new
                        {
                            StoreId = storeid,
                            PosInfoId = q.PosInfoId,
                            PosInfoDescription = q.PosInfo.Description,
                            DepartmentId = q.PosInfo.DepartmentId,
                            DepartmentDescription = q.PosInfo.DepartmentId != null ? q.PosInfo.Department.Description : "",
                            //TimeZone = q.
                            TimeZoneId = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? 1
                                                           : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                           ? 2 : ((q.Day.Value.Hour >= 19) && (q.Day.Value.Hour <= 24) || ((q.Day.Value.Hour >= 1) && (q.Day.Value.Hour <= 2))) ? 3 : 4,
                            TimeZone = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? "Breakfast"
                                                           : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                           ? "Lunch" : "Dinner",
                            Id = q.Id,
                            OrderNos = q.OrderDetailInvoices.Select(s => s.OrderDetail.Order.OrderNo),
                            InvoiceTypeCode = qqq.Code,
                            Abbreviation = qqq.Abbreviation,
                            Description = q.Description,
                            Counter = q.Counter,
                            StaffId = q.StaffId,
                            StaffName = q.Staff.LastName,
                            Day = q.Day,
                            Cover = q.Cover,
                            CloseId = q.EndOfDayId != null ? q.EndOfDay.CloseId : 0,
                            TableId = q.TableId,
                            TableCode = q.Table != null ? q.Table.Code : null,
                            Room = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.Room : null,
                            AccountId = trans != null ? trans.AccountId : null,
                            AccountDescription = trans != null ? trans.Accounts.Description : "Not Paid",
                            Guest = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.LastName : null,
                            InvoiceTotal = q.Total,
                            TransactionTotal = trans != null ? trans.Amount : 0,
                            Discount = q.Discount ?? 0,
                            TransStafId = trans != null ? trans.StaffId : null,
                            TransStafName = trans != null ? trans.Staff.LastName : null,
                            TotalForNoTransaction = trans != null ? trans.Amount : q.Total,
                            IsVoided = q.IsVoided,
                            IsVoidedDescription = q.IsVoided == true ? "Canceled" : "Sales"
                        };
            db.Configuration.LazyLoadingEnabled = false;


            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);

            return new { processedResults = query, metadata };
        }


        private Object SalesPerDepartmentsCheckingFromInvoicesFlatVoided(string filters, bool voided)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            List<Int64> eod;
            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
            else
                eod = new List<Int64>() { 0 };
            List<Int64?> poslist = flts.PosList;



            db.Configuration.LazyLoadingEnabled = true;
            var query = (from q in db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0))
                                                     && poslist.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false
                                                     && (w.IsVoided ?? false) == voided)
                         join t in db.Transactions.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals t.InvoicesId into fffff
                         from trans in fffff.DefaultIfEmpty()
                         select new
                         {
                             PosInfoId = q.PosInfoId,
                             PosInfoDescription = q.PosInfo.Description,
                             DepartmentId = q.PosInfo.DepartmentId,
                             DepartmentDescription = q.PosInfo.DepartmentId != null ? q.PosInfo.Department.Description : "",
                             //TimeZone = q.
                             TimeZoneId = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? 1
                                                            : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                            ? 2 : ((q.Day.Value.Hour >= 19) && (q.Day.Value.Hour <= 24) || ((q.Day.Value.Hour >= 1) && (q.Day.Value.Hour <= 2))) ? 3 : 4,
                             TimeZone = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? "Breakfast"
                                                            : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                            ? "Lunch" : "Dinner",
                             Id = q.Id,
                             InvoiceTypeCode = q.InvoiceTypes.Code,
                             Abbreviation = q.InvoiceTypes.Abbreviation,
                             Description = q.Description,
                             Counter = q.Counter,
                             StaffId = q.StaffId,
                             StaffName = q.Staff.LastName,
                             Day = q.Day,
                             Cover = q.Cover,
                             CloseId = q.EndOfDayId != null ? q.EndOfDay.CloseId : 0,
                             TableId = q.TableId,
                             TableCode = q.Table != null ? q.Table.Code : null,
                             Room = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.Room : null,
                             //  AccountId = trans != null ? trans.AccountId : null,
                             //  AccountDescription = trans != null ? trans.Accounts.Description : "",
                             // Guest = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.LastName : null,
                             InvoiceTotal = q.Total,
                             //  TransactionTotal = trans != null ? trans.Amount : 0,
                             //Discount = q.Discount ?? 0,
                             //TransStafId = trans != null ? trans.StaffId : null,
                             //TransStafName = trans != null ? trans.Staff.LastName : null,
                             //TotalForNoTransaction = trans != null ? trans.Amount : q.Total,

                         }).Distinct();
            db.Configuration.LazyLoadingEnabled = false;
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);

            return new { processedResults = query, metadata };
        }

        private Object SalesPerDepartmentsCheckingFromInvoicesFlat(string filters, bool voided)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            List<Int64> eod;
            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
            else
                eod = new List<Int64>() { 0 };
            List<Int64?> poslist = flts.PosList;



            db.Configuration.LazyLoadingEnabled = true;
            var query = from q in db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0))
                                                     && poslist.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false
                                                     && (w.IsVoided ?? false) == voided)
                        join t in db.Transactions.Where(w => (w.IsDeleted ?? false) == false) on q.Id equals t.InvoicesId into fffff
                        from trans in fffff.DefaultIfEmpty()
                        select new
                        {
                            PosInfoId = q.PosInfoId,
                            PosInfoDescription = q.PosInfo.Description,
                            DepartmentId = q.PosInfo.DepartmentId,
                            DepartmentDescription = q.PosInfo.DepartmentId != null ? q.PosInfo.Department.Description : "",
                            //TimeZone = q.
                            TimeZoneId = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? 1
                                                           : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                           ? 2 : ((q.Day.Value.Hour >= 19) && (q.Day.Value.Hour <= 24) || ((q.Day.Value.Hour >= 1) && (q.Day.Value.Hour <= 2))) ? 3 : 4,
                            TimeZone = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? "Breakfast"
                                                           : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                           ? "Lunch" : "Dinner",
                            Id = q.Id,
                            InvoiceTypeCode = q.InvoiceTypes.Code,
                            Abbreviation = q.InvoiceTypes.Abbreviation,
                            Description = q.Description,
                            Counter = q.Counter,
                            StaffId = q.StaffId,
                            StaffName = q.Staff.LastName,
                            Day = q.Day,
                            Cover = q.Cover,
                            CloseId = q.EndOfDayId != null ? q.EndOfDay.CloseId : 0,
                            TableId = q.TableId,
                            TableCode = q.Table != null ? q.Table.Code : null,
                            Room = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.Room : null,
                            AccountId = trans != null ? trans.AccountId : null,
                            AccountDescription = trans != null ? trans.Accounts.Description : "",
                            Guest = trans != null ? trans.Invoice_Guests_Trans.FirstOrDefault().Guest.LastName : null,
                            InvoiceTotal = q.Total,
                            TransactionTotal = trans != null ? trans.Amount : 0,
                            Discount = q.Discount ?? 0,
                            TransStafId = trans != null ? trans.StaffId : null,
                            TransStafName = trans != null ? trans.Staff.LastName : null,
                            TotalForNoTransaction = trans != null ? trans.Amount : q.Total,

                        };
            db.Configuration.LazyLoadingEnabled = false;

            //Dictionary<String, MetadataModel> metadata = new Dictionary<String, MetadataModel>();
            //metadata.Add("PosInfoId",new MetadataModel(){Description = "Κωδικός POS", FieldType = 1}); 
            //metadata.Add("PosInfoDescription",new MetadataModel(){Description = "Περιγραφή POS"); 
            //metadata.Add("DepartmentId",new MetadataModel(){Description = "Κωδ.Τμήματος"); 
            //metadata.Add("DepartmentDescription",new MetadataModel(){Description = "Τμήμα");
            //metadata.Add("TimeZoneId",new MetadataModel(){Description = "Κωδ. Ζώνης"); 
            //metadata.Add("TimeZone",new MetadataModel(){Description = "Timezone"); 
            //metadata.Add("Id",new MetadataModel(){Description = "Κωδικός Παραστ."); 
            //metadata.Add("InvoiceTypeCode",new MetadataModel(){Description = "Τύπος Παραστ."); 
            //metadata.Add("Abbreviation",new MetadataModel(){Description = "Συντόμευση");
            //metadata.Add("Description",new MetadataModel(){Description = "Περιγραφή"); 
            //metadata.Add("Counter",new MetadataModel(){Description = "Αριθμός"); 
            //metadata.Add("StaffId",new MetadataModel(){Description = "Κωδικός ΄Σερβ."); 
            //metadata.Add("StaffName",new MetadataModel(){Description = "Σερβιτόρος"); 
            //metadata.Add("Day",new MetadataModel(){Description = "Ημέρα"); 
            //metadata.Add("Cover",new MetadataModel(){Description = "Άτομα"); 
            //metadata.Add("CloseId",new MetadataModel(){Description = "Αρ. Z "); 
            //metadata.Add("TableId",new MetadataModel(){Description = "Τραπεζι κλ"); 
            //metadata.Add("TableCode",new MetadataModel(){Description = "Κωδικός Τραπεζ."); 
            //metadata.Add("Room",new MetadataModel(){Description = "Δωμάτιο");
            //metadata.Add("AccountId",new MetadataModel(){Description = "Κωδικός Πληρωμής");
            //metadata.Add("AccountDescription","Πληρωμή"); 
            //metadata.Add("Guest","Πελάτης"); 
            //metadata.Add("InvoiceTotal","Σύνολο Παραστ"); 
            //metadata.Add("TransactionTotal","Σύνολο"); 
            //metadata.Add("Discount","Έκπτωση"); 
            //metadata.Add("TransStafId","Κωδ. Σερβ. Εξοφλ."); 
            //metadata.Add("TransStafName","Σεριτίρος Εξοφλ."); 
            //metadata.Add("TotalForNoTransaction","Σύνολο Εισπρ.");
            var metadata = db.MetadataTable.Where(w => w.ReportType == flts.ReportType);

            return new { processedResults = query, metadata };
        }

        private Object SalesPerDepartmentsChecking(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            List<Int64> eod;
            if (flts.UseEod)
                eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
            else
                eod = new List<Int64>() { 0 };

            //var noeodflat = db.OrderDetailInvoices
            //                            .Include("OrderDetail.Transactions").AsNoTracking()
            //                            .Include("OrderDetail.Transactions.TransferToPms").AsNoTracking()
            //                            .Include("PosInfoDetail.PosInfo.Department").AsNoTracking()
            //                            .Include("OrderDetail.OrderDetailIgredients").AsNoTracking()
            //                            .Include("Staff").AsNoTracking()
            //                            .Include("OrderDetail.Transactions.Accounts").AsNoTracking()
            //                            .Include("OrderDetail.Table").AsNoTracking()
            //                            .Include("OrderDetail.Order").AsNoTracking()
            //                            .Where(w => w.PosInfoDetail.IsInvoice == true
            //                                        && w.OrderDetail.Order.EndOfDayId == null
            //                                        && flts.PosList.Contains(w.PosInfoDetail.PosInfoId)
            //                                        && w.OrderDetail.Order.EndOfDayId == null
            //                                        && w.OrderDetail.Status != 5);
            //var eodflat = db.OrderDetailInvoices
            //                            .Include("OrderDetail.Transactions").AsNoTracking()
            //                            .Include("OrderDetail.Transactions.TransferToPms").AsNoTracking()
            //                            .Include("PosInfoDetail.PosInfo.Department").AsNoTracking()
            //                            .Include("OrderDetail.OrderDetailIgredients").AsNoTracking()
            //                            .Include("Staff").AsNoTracking()
            //                            .Include("OrderDetail.Transactions.Accounts").AsNoTracking()
            //                            .Include("OrderDetail.Table").AsNoTracking()
            //                            .Include("OrderDetail.Order").AsNoTracking()
            //                            .Where(w => w.PosInfoDetail.IsInvoice == true
            //                                   && eod.Contains(w.OrderDetail.Order.EndOfDayId.Value)
            //                                   && flts.PosList.Contains(w.PosInfoDetail.PosInfoId)
            //                                   && w.OrderDetail.Status != 5);

            //var tempflat = flts.UseEod ? eodflat : noeodflat;
            //var flat = (from q in tempflat.ToList()
            //            select new
            //            {
            //                EodId = q.OrderDetail.Order.EndOfDayId,
            //                DepartmentId = q.PosInfoDetail.PosInfo.DepartmentId,
            //                Department = q.PosInfoDetail.PosInfo.Department.Description,

            //                PosInfoId = q.PosInfoDetail.PosInfoId,
            //                PosInfoDescription = q.PosInfoDetail.PosInfo.Description,
            //                PosInfoDetailId = q.PosInfoDetailId,
            //                PosInfoDetailDescription = q.PosInfoDetail.Description,
            //                InvoiceId = q.Id,
            //                InvoiceGroup = q.PosInfoDetail.GroupId,
            //                InvoiceAbbreviation = q.PosInfoDetail.Abbreviation,
            //                InvoiceCounter = q.Counter,
            //                InvoiceDescription = q.PosInfoDetail.Description,
            //                Staff = q.StaffId,
            //                StaffName = q.Staff.LastName,

            //                IsInvoice = q.PosInfoDetail.IsInvoice,

            //                Table = q.OrderDetail.TableId != null ? q.OrderDetail.Table.Code : "",
            //                TotalAfterDiscount = q.OrderDetail.TotalAfterDiscount,//+ q.OrderDetail.OrderDetailIgredients.Count() > 0 ? q.OrderDetail.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0,
            //                Total = q.OrderDetail.Qty * (double)q.OrderDetail.Price,
            //                Day = q.OrderDetail.Order.Day,
            //                AccountId = q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.AccountId : null,
            //                AccountDescription = q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.Accounts.Description : "",
            //                TransferToPms1 = q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.TransferToPms.Count() > 0 ? q.OrderDetail.Transactions.TransferToPms : null : null,
            //                SendToPMS = q.OrderDetail.Transactions != null ? q.OrderDetail.Transactions.TransferToPms.Count() > 0 ? q.OrderDetail.Transactions.TransferToPms.FirstOrDefault().SendToPMS : null : null

            //            }).ToList();

            var flat = (from q in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false)
                            //join it in InvoiceTypes on q.InvoiceTypeId equals it.Id
                        join pid in db.PosInfoDetail.Where(w => w.IsInvoice == true) on q.PosInfoDetailId equals pid.Id
                        join pi in db.PosInfo.Where(w => flts.PosList.Contains(w.Id)) on pid.PosInfoId equals pi.Id
                        join dep in db.Department on pi.DepartmentId equals dep.Id
                        join od in db.OrderDetail.Where(w => w.Status != 5 && (w.IsDeleted ?? false) == false) on q.OrderDetailId equals od.Id
                        join o in db.Order.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && (w.IsDeleted ?? false) == false) on od.OrderId equals o.Id
                        join st in db.Staff on q.StaffId equals st.Id
                        join tb in db.Table on od.TableId equals tb.Id into ff
                        from tbl in ff.DefaultIfEmpty()
                        join tr in db.Transactions.Where(w => (w.IsDeleted ?? false) == false) on od.TransactionId equals tr.Id into fff
                        from trans in fff.DefaultIfEmpty()
                        join ac in db.Accounts on trans.AccountId equals ac.Id into ffff
                        from acc in ffff.DefaultIfEmpty()
                        join g in db.TransferToPms on trans.Id equals g.TransactionId into fffff
                        from gst in fffff.DefaultIfEmpty()
                        select new
                        {
                            EodId = o.EndOfDayId,
                            DepartmentId = pi.DepartmentId,
                            Department = dep.Description,

                            PosInfoId = pid.PosInfoId,
                            PosInfoDescription = pi.Description,
                            PosInfoDetailId = q.PosInfoDetailId,
                            PosInfoDetailDescription = pid.Description,
                            //InvoiceId = q.Id,
                            InvoiceGroup = pid.GroupId,
                            InvoiceAbbreviation = pid.Abbreviation,
                            InvoiceCounter = q.Counter,
                            InvoiceDescription = pid.Description,
                            Staff = q.StaffId,
                            StaffName = st.LastName,

                            IsInvoice = pid.IsInvoice,

                            Table = od.TableId != null ? tbl.Code : "",
                            TotalAfterDiscount = od.TotalAfterDiscount,//+ q.OrderDetail.OrderDetailIgredients.Count() > 0 ? q.OrderDetail.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0,
                            Day = o.Day,
                            AccountId = trans.AccountId,
                            AccountDescription = acc.Description,
                            Room = gst != null ? gst.RoomDescription : ""

                        }).Distinct().ToList();


            var flatByAccountId = flat.GroupBy(g => new { g.PosInfoId }).Select(ss => new
            {
                Department = ss.FirstOrDefault().Department,
                PosInfoId = ss.Key.PosInfoId,

                PosInfoDescription = ss.FirstOrDefault().PosInfoDescription,
                Gross = ss.Sum(sm => sm.TotalAfterDiscount),
                Accounts = ss.GroupBy(gg => gg.AccountId).Select(s3 => new
                {
                    AccountId = s3.Key,
                    AccountDescription = s3.FirstOrDefault().AccountDescription,
                    //GroupByVat = s3.GroupBy(g=>g.VatId), 
                    Invoices = s3.GroupBy(g3 => new { g3.InvoiceGroup, g3.InvoiceAbbreviation, g3.InvoiceCounter })
                                 .Select(s4 => new
                                 {
                                     Day = s4.FirstOrDefault().Day,
                                     InvoiceAbbreviation = s4.FirstOrDefault().InvoiceAbbreviation,
                                     InvoiceCounter = s4.FirstOrDefault().InvoiceCounter,
                                     Total = s4.Sum(sm => sm.TotalAfterDiscount),
                                     TransferToPms = s4.OrderByDescending(o => o.Room).FirstOrDefault().Room,//s4.FirstOrDefault().TransferToPms1 != null ? s4.FirstOrDefault().TransferToPms1.FirstOrDefault().RoomDescription : "",
                                     PosInfoDetailId = s4.FirstOrDefault().PosInfoDetailId,
                                     PosInfoDetailDescription = s4.FirstOrDefault().PosInfoDetailDescription,
                                 //  TransactionId = s4.Select(s5 => s5.TransactionId),
                                 Table = s4.FirstOrDefault().Table,
                                     StaffId = s4.FirstOrDefault().Staff,
                                     StaffName = s4.FirstOrDefault().StaffName,

                                 }),
                    AccountTotal = s3.Sum(sm => sm.TotalAfterDiscount)
                })
            });

            //var flatyByAccountId = flat.GroupBy(g => new { g.PosInfoId }).Select(ss => new
            //{
            //    Department = ss.FirstOrDefault().Department,
            //    PosInfoId = ss.Key.PosInfoId,
            //    PosInfoDescription = ss.FirstOrDefault().PosInfoDescription,
            //    Accounts = ss.GroupBy(gg => gg.AccountId).Select(s3 => new
            //    {
            //        AccountId = s3.Key,
            //        AccountDescription = s3.FirstOrDefault().AccountDescription,
            //        Invoices = s3.GroupBy(g3 => new { g3.InvoiceGroup, g3.InvoiceAbbreviation, g3.InvoiceCounter })
            //                     .Select(s4 => new
            //                     {
            //                         Day = s4.FirstOrDefault().Day,
            //                         InvoiceAbbreviation = s4.FirstOrDefault().InvoiceAbbreviation,
            //                         InvoiceCounter = s4.FirstOrDefault().InvoiceCounter,
            //                         Total = s4.Sum(sm => sm.Total),
            //                         TransferToPms = s4.FirstOrDefault().TransferToPms1,
            //                         RoomCharged = s4.All(a => a.SendToPMS == true)
            //                     })
            //    })
            //});

            return new { processedResults = flatByAccountId };

        }
        private Object SalesAnalysis()
        {
            var detVat = from q in db.OrderDetailVatAnal
                         select new
                         {
                             OrderDetailId = q.OrderDetailId,
                             Gross = q.Gross,
                             Net = q.Net,
                             VatRate = q.VatRate,
                             VatAmount = q.VatAmount,
                             VatId = q.VatId
                         };
            var detVatIng = from q in db.OrderDetailIgredientVatAnal
                            select new
                            {
                                OrderDetailId = q.OrderDetailIgredients.OrderDetailId,
                                Gross = q.Gross,
                                Net = q.Net,
                                VatRate = q.VatRate,
                                VatAmount = q.VatAmount,
                                VatId = q.VatId
                            };


            var vatsAnal = detVat.Union(
            detVatIng).GroupBy(g => new { g.OrderDetailId, g.VatId }).Select(s => new
            {
                OrderDetailId = s.Key.OrderDetailId,
                VatId = s.Key.VatId,
                Net = s.Sum(sm => sm.Net),
                VatRate = s.FirstOrDefault().VatRate,
                VatAmount = s.Sum(sm => sm.VatAmount),
                Gross = s.Sum(sm => sm.Gross)

            });

            var flatVats = from q in db.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true && (w.IsDeleted ?? false) == false)
                           join va in vatsAnal on q.OrderDetailId equals va.OrderDetailId
                           select new
                           {
                               DepartmentId = q.PosInfoDetail.PosInfo.DepartmentId,
                               Department = q.PosInfoDetail.PosInfo.Department.Description,

                               PosInfoId = q.PosInfoDetail.PosInfoId,
                               PosInfoDescription = q.PosInfoDetail.PosInfo.Description,

                               InvoiceId = q.Id,
                               InvoiceGroup = q.PosInfoDetail.GroupId,
                               InvoiceAbbreviation = q.PosInfoDetail.Abbreviation,
                               InvoiceCounter = q.Counter,
                               InvoiceDescription = q.PosInfoDetail.Description,
                               Staff = q.StaffId,
                               IsInvoice = q.PosInfoDetail.IsInvoice,
                               VatId = va.VatId,
                               Net = va.Net,
                               VatRate = va.VatRate,
                               VatAmount = va.VatAmount,
                               Gross = va.Gross,
                               Table = q.OrderDetail.TableId != null ? q.OrderDetail.Table.Code : "",
                               TotalAfterDiscount = q.OrderDetail.TotalAfterDiscount + q.OrderDetail.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount),
                               Total = (Double)q.OrderDetail.Qty * (Double)q.OrderDetail.Price,
                               Day = q.OrderDetail.Order.Day

                           };
            //		flatVats.Dump();

            var groupedByDepartment = flatVats.ToList().GroupBy(g => new { g.DepartmentId, g.PosInfoId, g.InvoiceGroup, g.InvoiceAbbreviation, g.InvoiceCounter, g.Day })
                                              .Select(s => new
                                              {
                                                  Day = s.FirstOrDefault().Day,
                                                  DepartmentId = s.Key.DepartmentId,
                                                  Deparmtent = s.FirstOrDefault().Department,
                                                  ReceiptAbbr = s.FirstOrDefault().InvoiceAbbreviation,
                                                  ReceitCounter = s.FirstOrDefault().InvoiceCounter,
                                                  ReceiptTotal = s.Sum(sm => sm.Gross),
                                                  ReceiptTotalAfter = s.Sum(sm => sm.TotalAfterDiscount),
                                                  Room = 9601,
                                                  Table = s.FirstOrDefault().Table,
                                                  Staff = s.FirstOrDefault().Staff,
                                              //ReceiptVatAnal = s.GroupBy(gg => new { gg.VatId }).Select(ss => new
                                              //{
                                              //    VatId = ss.FirstOrDefault().VatId,
                                              //    Net = ss.Sum(sm => sm.Net),
                                              //    VatRate = ss.FirstOrDefault().VatRate,
                                              //    VatAmount = ss.Sum(sm => sm.VatAmount),
                                              //    Gross = ss.Sum(sm => sm.Gross)

                                              //}),
                                              ReceiptVatAnal = from v in db.Vat.ToList()
                                                                   join jq in (s.GroupBy(gg => new { gg.VatId }).Select(ss => new
                                                                   {
                                                                       VatId = ss.FirstOrDefault().VatId,
                                                                       Net = ss.Sum(sm => sm.Net),
                                                                       VatRate = ss.FirstOrDefault().VatRate,
                                                                       VatAmount = ss.Sum(sm => sm.VatAmount),
                                                                       Gross = ss.Sum(sm => sm.Gross)

                                                                   })) on v.Id equals jq.VatId into ff
                                                                   from all in ff.DefaultIfEmpty()
                                                                   select new
                                                                   {
                                                                       VatId = v.Id,
                                                                       Net = all != null ? all.Net : 0,
                                                                       VatRate = v.Percentage,
                                                                       VatAmount = all != null ? all.VatAmount : 0,
                                                                       Gross = all != null ? all.Gross : 0
                                                                   }
                                                               //ReceiptVatAnal1 = s.GroupBy(gg=>new {gg.VatId}),
                                          }).ToList();

            // groupedByDepartment.Dump();
            var summaryResults = new { Vats = db.Vat.Select(s => new { Percentage = s.Percentage }) };
            return new { processedResults = groupedByDepartment.OrderBy(o => o.Day), summaryResults };
        }
        private Object ReturnAllProductsWithPrices()
        {
            var prods = (from q in db.Product.Include("ProductCategories").Include("Pricelist")
                         select new
                         {
                             Code = q.Code,
                             Description = q.Description,
                             ProductCategory = q.ProductCategories.Id,
                             ProductCategoryDesc = q.ProductCategories.Description,
                             PriceList = q.PricelistDetail.Select(s => new
                             {
                                 PricelistCode = s.PricelistId,
                                 PricelistDescription = s.Pricelist.Description,
                                 Price = s.Price

                             })

                         }).GroupBy(g => g.ProductCategory).Select(ss => new
                         {
                             CategoryId = ss.Key,
                             CategoryDesc = ss.FirstOrDefault().ProductCategoryDesc,
                             Products = ss.Select(s3 => new
                             {
                                 Code = s3.Code,
                                 Description = s3.Description,
                                 PriceLists = s3.PriceList
                             })
                         });
            ;
            return prods;
        }

        [Obsolete]
        private Object BestSeller(IEnumerable<dynamic> flat, bool sortQty)
        {
            var qtyTotals = flat.Sum(sm => (Double)sm.Qty);
            var amountTotals = flat.Sum(sm => (Decimal)sm.TotalAfterDiscount);
            var bestSeller = flat.GroupBy(g => g.ProductId).Select(s => new
            {
                ProductId = s.Key,
                ProductDescription = s.FirstOrDefault().ProductDescription,
                Qty = s.Sum(sm => (Double)sm.Qty),
                Total = s.Sum(sm => (Decimal)sm.TotalAfterDiscount),
                QtyPerc = s.Sum(sm => (Double)sm.Qty) * 100 / qtyTotals,
                TotalPerc = s.Sum(sm => (Decimal)sm.TotalAfterDiscount) * 100 / amountTotals


            });
            var result = sortQty ? bestSeller.OrderByDescending(o => o.Qty) : bestSeller.OrderByDescending(o => o.Total);
            var summaryResults = new { qtyTotals, amountTotals };
            return new { processedResults = result, summaryResults };
            // return new { processedResults = periodAnal };
        }
        [Obsolete]
        private Object PeriodAnal(IEnumerable<dynamic> flat)
        {
            var periodAnal = flat.GroupBy(g => g.PosInfoId).Select(s => new
            {
                PosInfoId = s.Key,
                PosDescr = s.FirstOrDefault().PosDescr,
                PerDay = s.GroupBy(g => EntityFunctions.TruncateTime(g.Day)).Select(ss => new
                {
                    Day = ss.Key,
                    PerReceipt = ss.GroupBy(ggg => new { ggg.Inv_Descr, ggg.Inv_Counter }).Select(s3 => new
                    {
                        Day = s3.FirstOrDefault().Day,
                        Inv_Descr = s3.Key.Inv_Descr,
                        Inv_Abbr = s3.FirstOrDefault().Inv_Abbr,
                        Inv_Counter = s3.Key.Inv_Counter,
                        TableId = s3.FirstOrDefault().TableId,
                        TableCode = s3.FirstOrDefault().TableCode,
                        AccountDescription = s3.FirstOrDefault().AccountDescription,
                        StaffId = s3.FirstOrDefault().StaffId,
                        Staff = s3.FirstOrDefault().Staff,
                        ReceiptTotal = s3.Sum(sm => sm.Total),
                        Product = s3.Select(s4 => new
                        {
                            ProductId = s4.ProductId,
                            Product = s4.Product,
                            Qty = s4.Qty,
                            Price = s4.Price,
                            Total = s4.Total
                        })

                    })
                })

            });
            return periodAnal;
        }
        [Obsolete]
        private Object SalesByProductCategory(IEnumerable<dynamic> flat)
        {
            var flatQtyTotals = flat.Sum(sm => (Double)sm.Qty);
            var flatAmountTotals = flat.Sum(sm => (Decimal)sm.TotalAfterDiscount);

            var salesByProductCategory = flat.GroupBy(g => g.ProductId).Select(s => new
            {
                ProductCategory = s.FirstOrDefault().CategoryDescription,
                ProductId = s.Key,
                Product = s.FirstOrDefault().ProductDescription,
                Qty = s.Sum(sm => (Double)sm.Qty),
                Total = s.Sum(sm => (Decimal)sm.TotalAfterDiscount),
                Receipts = s.Select(s4 => new { s4.PosInfoDetailId, s4.GroupId, s4.Counter }).Count()
            })
                .GroupBy(gg => gg.ProductCategory).Select(ss => new
                {
                    ProductCategory = ss.Key,
                    Qty = ss.Sum(sm => (Double)sm.Qty),
                    Total = ss.Sum(sm => (Decimal)sm.Total),
                    TotalPerc = ss.Sum(sm => (Decimal)sm.Total) * 100 / flatAmountTotals,
                    TotalReceipts = ss.Sum(sm => sm.Receipts),
                    Products = ss.Select(s3 => new
                    {
                        ProductId = s3.ProductId,
                        Product = s3.Product,
                        Qty = s3.Qty,
                        Total = s3.Total,
                        Receipts = s3.Receipts
                    })
                });
            // return new { salesByProductCategory };
            //var summaryResults = new { qtyTotals, amountTotals };
            return new { processedResults = salesByProductCategory };
        }
        [Obsolete]
        private Object SalesByPricelist(IEnumerable<dynamic> flat)
        {
            var flatQtyTotals = flat.Sum(sm => (Double)sm.Qty);
            var flatAmountTotals = flat.Sum(sm => (Decimal)sm.TotalAfterDiscount);

            var salesByPricelist = flat.GroupBy(g => g.ProductId).Select(s => new
            {
                PriceList = s.FirstOrDefault().PriceListDescription,
                ProductId = s.Key,
                Product = s.FirstOrDefault().ProductDescription,
                Qty = s.Sum(sm => (Double)sm.Qty),
                Total = s.Sum(sm => (Decimal)sm.TotalAfterDiscount),
                Receipts = s.Select(s4 => new { s4.PosInfoDetailId, s4.GroupId, s4.Counter }).Count()
            })
                .GroupBy(gg => gg.PriceList).Select(ss => new
                {
                    PriceList = ss.Key,
                    Qty = ss.Sum(sm => (Double)sm.Qty),
                    Total = ss.Sum(sm => (Decimal)sm.Total),
                    TotalPerc = ss.Sum(sm => (Decimal)sm.Total) * 100 / flatAmountTotals,
                    TotalReceipts = ss.Sum(sm => sm.Receipts),
                    Products = ss.Select(s3 => new
                    {
                        ProductId = s3.ProductId,
                        Product = s3.Product,
                        Qty = s3.Qty,
                        Total = s3.Total,
                        Receipts = s3.Receipts
                    })
                });
            //  return new { salesByPricelist };
            //var summaryResults = new { qtyTotals, amountTotals };
            return new { processedResults = salesByPricelist };
        }
        [Obsolete]
        private Object PeriodicStats(IEnumerable<dynamic> flat)
        {

            var flatByProduct = flat.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId })
                              .Select(s => new
                              {
                                  PriceListId = s.Key.PriceListId,
                                  PriceList = s.FirstOrDefault().PriceListDescription,
                                  ProductId = s.Key.ProductId,
                                  Product = s.FirstOrDefault().ProductDescription,
                                  ProductPrice = s.Max(m => m.PriceLictPrice),
                              //PriceList = s.FirstOrDefault().PriceList,
                              Qty = s.Sum(ss => (Double)ss.Qty),
                                  SalesTypeId = s.FirstOrDefault().SalesTypeDescription,
                                  Gross = s.Sum(ss => (Decimal)ss.TotalAfterDiscount),
                                  ReceiptCount = s.GroupBy(g => new { g.PosInfoDetailId, g.GroupId, g.Counter }).Count(),

                              //PriceListId = s.Key.PriceListId,
                              //PriceList = s.FirstOrDefault().PriceListDescription,
                              //ProductId = s.Key.ProductId,
                              //Product = s.FirstOrDefault().Product,
                              //ProductPrice = s.Max(m => m.PriceLictPrice),
                              ////PriceList = s.FirstOrDefault().PriceList,
                              //Qty = s.Sum(ss => (Double)ss.Qty),
                              //SalesTypeId = s.FirstOrDefault().SalesTypeDescription,
                              //Gross = s.Sum(ss => (Decimal)ss.Total),
                              //ReceiptCount = s.GroupBy(g => new { g.PosInfoDetailId, g.GroupId, g.Counter }).Count(),           
                              ////	  a= s.GroupBy(g=>new {g.PosInfoId, g.InvTypeGroup, g.Inv_Counter})

                          }).ToList();
            var usedPriceLists = flatByProduct.Select(s2 => new
            {
                Id = s2.PriceListId,
                Description = s2.PriceList
            }).Distinct().OrderBy(o => o.Id).ToList();
            var groupedByProduct = flatByProduct.GroupBy(g => g.ProductId)
                                  .Select(s => new
                                  {
                                      ProductId = s.Key,
                                      ProductDescription = s.FirstOrDefault().Product,
                                      PriceList = (from q in usedPriceLists
                                                   join qq in s.GroupBy(g1 => g1.PriceListId) on q.Id equals qq.Key into ff
                                                   from s1 in ff.DefaultIfEmpty()

                                                   select new
                                                   {
                                                       PriceListId = q.Id,//s1.Key,
                                                       PriceList = q.Description,//s1.FirstOrDefault().PriceList,
                                                       ProductPrice = s.Max(m => m.ProductPrice),
                                                       Qty = s1 != null ? s1.Sum(ss => ss.Qty) : 0,
                                                       Gross = s1 != null ? s1.Sum(ss => ss.Gross) : 0
                                                   }).OrderBy(o => o.PriceListId),
                                      Sr = Decimal.Round((decimal)(s.Sum(s1 => s1.Qty) / s.FirstOrDefault().ReceiptCount * 100), 2)

                                  }).OrderBy(o => o.ProductDescription);
            var totalsPerPriceList = flatByProduct.GroupBy(g => g.PriceListId).Select(s => new
            {
                PriceListId = s.Key,
                PriceList = s.FirstOrDefault().PriceList,
                ProductPrice = s.Max(m => m.ProductPrice),
                Qty = s.Sum(ss => ss.Qty),
                Gross = s.Sum(ss => ss.Gross)
            }).OrderBy(o => o.PriceList);

            var totals = new
            {
                ItemsSold = flatByProduct.Sum(s => s.Qty),
                TotalReceipts = flat.GroupBy(g => new { g.PosInfoDetailId, g.GroupId, g.Counter }).Count(),
                TotalSales = flatByProduct.Sum(s => s.Gross),
                TotalPerSalesType = flatByProduct.GroupBy(s1 => s1.SalesTypeId).Select(s => new
                {
                    SalesTypeId = s.Key,
                    Total = s.Sum(ss => ss.Gross)
                }),
                TotalPerPos = flat.GroupBy(g => g.PosInfoId).Select(s => new
                {
                    PosId = s.Key,
                    PosDescription = s.FirstOrDefault().PosInfoDescription,
                    Total = 0//s.Sum(ss => (double)ss.Gross)
                })

            };


            //var flatByProduct = flat.GroupBy(g => new { g.PriceListId, g.ProductId })
            //                  .Select(s => new PeriodicSalesFlatModel
            //                  {
            //                      PriceListId = s.Key.PriceListId,
            //                      PriceList = s.FirstOrDefault().PriceList,
            //                      ProductId = s.Key.ProductId,
            //                      Product = s.FirstOrDefault().Product,
            //                      ProductPrice = s.Max(m => m.PriceLictPrice),
            //                      //PriceList = s.FirstOrDefault().PriceList,
            //                      Qty = s.Sum(ss => (Double)ss.Qty),
            //                      SalesTypeId = s.FirstOrDefault().SalesTypeId,
            //                      Gross = s.Sum(ss => (Decimal)ss.Price),
            //                      ReceiptCount = s.Select(ss => ss.OrderId).Distinct().Count()
            //                  }).ToList<PeriodicSalesFlatModel>();
            //var usedPriceLists = flatByProduct.Select(s2 => new
            //{
            //    Id = s2.PriceListId,
            //    Description = s2.PriceList
            //}).Distinct().OrderBy(o => o.Description).ToList();
            //var groupedByProduct = flatByProduct.GroupBy(g => g.ProductId)
            //                      .Select(s => new
            //                      {
            //                          ProductId = s.Key,
            //                          ProductDescription = s.FirstOrDefault().Product,
            //                          PriceList = (from q in usedPriceLists
            //                                       join qq in s.GroupBy(g1 => g1.PriceListId) on q.Id equals qq.Key into ff
            //                                       from s1 in ff.DefaultIfEmpty()

            //                                       select new
            //                                       {
            //                                           PriceListId = q.Id,//s1.Key,
            //                                           PriceList = q.Description,//s1.FirstOrDefault().PriceList,
            //                                           ProductPrice = s.Max(m => m.ProductPrice),
            //                                           Qty = s1 != null ? s1.Sum(ss => ss.Qty) : 0,
            //                                           Gross = s1 != null ? s1.Sum(ss => ss.Gross) : 0
            //                                       }).OrderBy(o => o.PriceList),
            //                          Sr = Decimal.Round((decimal)(s.Sum(s1 => s1.Qty) / s.FirstOrDefault().ReceiptCount * 100), 2)

            //                      }).OrderBy(o => o.ProductDescription);
            //var totalsPerPriceList = flatByProduct.GroupBy(g => g.PriceList).Select(s => new
            //{
            //    PriceListId = s.Key,
            //    PriceList = s.FirstOrDefault().PriceList,
            //    ProductPrice = s.Max(m => m.ProductPrice),
            //    Qty = s.Sum(ss => ss.Qty),
            //    Gross = s.Sum(ss => ss.Gross)
            //}).OrderBy(o => o.PriceList);

            //var totals = new
            //{
            //    ItemsSold = flatByProduct.Sum(s => s.Qty),
            //    TotalReceipts = flatByProduct.FirstOrDefault().ReceiptCount,
            //    TotalSales = flatByProduct.Sum(s => s.Gross),
            //    TotalPerSalesType = flatByProduct.GroupBy(s1 => s1.SalesTypeId).Select(s => new
            //    {
            //        SalesTypeId = s.Key,
            //        Total = s.Sum(ss => ss.Gross)
            //    })

            //};

            var summaryResults = new { usedPriceLists, totalsPerPriceList, totals };
            return new { processedResults = groupedByProduct, summaryResults };

            //return new { usedPriceLists, results = groupedByProduct, totalsPerPriceList, totals };
        }
        [Obsolete]
        private Object PeriodicStatsPerWaiter(IEnumerable<dynamic> flat)
        {

            var flatByWaiter = flat.GroupBy(g => g.StaffName)
                           .Select(s => new
                           {
                               Staff = s.FirstOrDefault().StaffName,
                               Receipts = s.Sum(sm => sm.Receipts),
                               Qty = s.Sum(sm => (Double)sm.Qty),
                               ReceiptAverage = s.Average(a => a.Total),
                               Gross = s.Sum(sm => (Decimal)sm.StaffName),
                               Products = s.GroupBy(gg => gg.ProductId)
                                           .Select(sss => new
                                           {
                                               Product = sss.FirstOrDefault().ProductDescription,
                                               Receipts = sss.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId }).Count(),
                                               Qty = sss.Sum(sm => (Double)sm.Qty),
                                               ReceiptAverage = Decimal.Round(sss.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId }).Select(ss => new { ReceiptTotal = ss.Sum(sm => (Decimal)sm.TotalAfterDiscount) }).Average(a => a.ReceiptTotal), 2),
                                               Gross = Decimal.Round(sss.Sum(sm => (Decimal)sm.TotalAfterDiscount), 2),
                                           })
                           });

            //var flatByWaiter = flat.GroupBy(g => g.StaffId)
            //               .Select(s => new
            //               {
            //                   Staff = s.FirstOrDefault().StaffName,
            //                   Receipts = s.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId }).Count(),
            //                   Qty = s.Sum(sm => (Double)sm.Qty),
            //                   ReceiptAverage = Decimal.Round(s.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId }).Select(ss => new { ReceiptTotal = ss.Sum(sm => (Decimal)sm.Price) }).Average(a => a.ReceiptTotal), 2),
            //                   Gross = Decimal.Round(s.Sum(sm => (Decimal)sm.TotalAfterDiscount), 2),
            //                   Products = s.GroupBy(gg => gg.ProductId)
            //                               .Select(sss => new
            //                               {
            //                                   Product = sss.FirstOrDefault().ProductDescription,
            //                                   Receipts = sss.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId }).Count(),
            //                                   Qty = sss.Sum(sm => (Double)sm.Qty),
            //                                   ReceiptAverage = Decimal.Round(sss.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId }).Select(ss => new { ReceiptTotal = ss.Sum(sm => (Decimal)sm.TotalAfterDiscount) }).Average(a => a.ReceiptTotal), 2),
            //                                   Gross = Decimal.Round(sss.Sum(sm => (Decimal)sm.TotalAfterDiscount), 2),
            //                               })
            //               });

            var totals = new
            {
                Receipts = flatByWaiter.Sum(s => s.Receipts),
                Qty = flatByWaiter.Sum(s => s.Qty),
                TotalSales = flatByWaiter.Sum(s => s.Gross)


            };

            var totalsPerWaiter = flatByWaiter.GroupBy(g => g.Staff).Select(s => new { Staff = s.Key, Total = s.Sum(sm => sm.Gross) });
            var summaryResults = new { totals, totalsPerWaiter };

            return new { processedResults = flatByWaiter, summaryResults };
            // return new { flatByWaiter, totals, totalsPerWaiter };
        }

        private Object PeriodicStatsPerProduct(IEnumerable<dynamic> flat)
        {


            // art
            var flatByWaiter = flat.GroupBy(g => g.ProductId)
                           .Select(s => new
                           {
                               Staff = s.FirstOrDefault().ProductDescription,
                               Receipts = s.GroupBy(g => new { g.PriceListId, g.ProductId, g.SalesTypeId }).Count(),
                               Qty = s.Sum(sm => (Double)sm.Qty),
                               Persons = s.Sum(m => m.Cover),
                               ReceiptAverage = Decimal.Round(s.GroupBy(g => new { g.PosInfoDetailId, g.GroupId, g.Counter }).Select(ss => new { ReceiptTotal = ss.Sum(sm => (Decimal)sm.TotalAfterDiscount) }).Average(a => a.ReceiptTotal), 2),
                               Gross = Decimal.Round(s.Sum(sm => (Decimal)sm.TotalAfterDiscount), 2),

                           });

            var totals = new
            {
                Receipts = 0,//flatByWaiter.Sum(s => s.Receipts),
                Qty = 0,//flatByWaiter.Sum(s => s.Qty),
                TotalSales = 0,//flatByWaiter.Sum(s => s.Gross)


            };

            var summaryResults = new { totals };

            return new { processedResults = flatByWaiter, summaryResults };
            // return new { flatByWaiter, totals };
        }
        private Object TotalsPerHour(IEnumerable<dynamic> flat)
        {
            var totalsPerHour = flat.Select(s => new
            {
                Day = s.Day.Date,
                Hour = s.Day.Hour,
                Price = s.Price,
                OrderId = s.OrderId
            }).GroupBy(g => g.Day)
                          .Select(ss => new
                          {
                              Day = ss.Key,
                              TotalsPerDay = ss.GroupBy(gg => gg.Hour).Select(sss => new
                              {
                                  Hour = sss.Key,
                                  Total = sss.Sum(sm => (Decimal)sm.Price),
                                  Tickets = sss.Select(ssss => ssss.OrderId).Distinct().Count()
                              })

                          });
            return totalsPerHour;
        }

        public Object GetZReport(string storeid, Int32 eodId)
        {
            db.Configuration.LazyLoadingEnabled = false;

            EndOfDay eod = db.EndOfDay.Where(w => w.Id == (Int64)eodId).SingleOrDefault();
            if (eod == null)
                return null;
            var piid = eod.PosInfoId;
            //Configuration.LazyLoadingEnabled = true;

            var posdata = db.PosInfo.Where(f => f.Id == piid).FirstOrDefault();
            var salesByReceipt = db.Invoices.Where(w => w.EndOfDayId == eodId && w.PosInfoId == piid
                                                //  && (w.IsVoided ?? false) == false
                                                && (w.IsDeleted ?? false) == false
                                                && w.InvoiceTypeId != null && w.InvoiceTypes.Type != (int)InvoiceTypesEnum.Order && w.InvoiceTypes.Type != (int)InvoiceTypesEnum.Void && w.InvoiceTypes.Type != 10);

            //added where clause to remove transactions marked as IsDeleted
            var allInvoices = salesByReceipt.SelectMany(sm => sm.Transactions).Where(w => (w.IsDeleted ?? false) == false).Select(s => new
            {
                InvoiceId = s.InvoicesId,
                Description = s.Invoices.InvoiceTypes.Abbreviation,
                InvoiceCounte = s.Invoices.Counter,
                TransAmount = s.Amount,
                InvoiceTotal = s.Invoices.Total,
                Table = s.Invoices.Table.Code,
                Room = s.Invoice_Guests_Trans.FirstOrDefault().Guest.Room,
                StaffCode = s.Staff.Code,
                StaffName = s.Staff.LastName,
                IsVoided = s.Invoices.IsVoided ?? false,
                AccountId = s.AccountId,
                AccountDescription = s.Accounts.Description,
                AccountType = s.Accounts.Type
            });//.ToList();//.Dump();			

            var allInvoicesFinal = from q in salesByReceipt//.Where(w => (w.IsVoided ?? false) == false)
                                   join qq in allInvoices on q.Id equals qq.InvoiceId into f
                                   from temp in f.DefaultIfEmpty()
                                   select new
                                   {
                                       InvoiceId = q.Id,
                                       Description = q.InvoiceTypes.Abbreviation,
                                       InvoiceCounte = q.Counter,
                                       TransAmount = q.Transactions.FirstOrDefault() != null ? temp.TransAmount : 0,
                                       InvoiceTotal = q.Total,
                                       Table = q.Table.Code,
                                       Room = q.Transactions.FirstOrDefault() != null ? temp.Room : "",
                                       StaffCode = q.Staff.Code,
                                       StaffName = q.Staff.LastName,
                                       IsVoided = q.IsVoided ?? false,
                                       AccountId = q.Transactions.FirstOrDefault() != null ? temp.AccountId : 0,
                                       AccountDescription = q.Transactions.FirstOrDefault() != null ? temp.AccountDescription : "Undefined",
                                       AccountType = q.Transactions.FirstOrDefault() != null ? temp.AccountType : 0
                                   };
            var staffCashier = db.Transactions.Where(w => w.EndOfDayId == eodId && (w.IsDeleted ?? false) == false)
                               .GroupBy(g => new { g.StaffId, g.TransactionType, g.InOut, g.AccountId })
                               .Select(ss => new
                               {
                                   StaffId = ss.FirstOrDefault().StaffId,
                                   AccountId = ss.FirstOrDefault().AccountId,
                                   AccountType = ss.FirstOrDefault().Accounts.Type,
                                   AccountDescription = ss.FirstOrDefault().Accounts.Description,
                                   Amount = ss.Sum(sm => sm.Amount),
                                   InOut = ss.FirstOrDefault().InOut,
                                   Descriptions = ss.FirstOrDefault().Description,
                                   TransactionType = ss.FirstOrDefault().TransactionType

                               }).ToList();//.GroupBy(g.StaffId);

            var validOds = from q in db.OrderDetail
                           join qq in db.OrderDetailInvoices on q.Id equals qq.OrderDetailId
                           join qqq in salesByReceipt on qq.InvoicesId equals qqq.Id
                           select q;
            var sumOds = validOds.Where(w => w.Order.EndOfDayId == eodId && w.Order.PosId == piid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5).SelectMany(s => s.OrderDetailVatAnal).GroupBy(g => g.VatId).Select(s => new
            {
                VatId = s.Key,
                Gross = s.Sum(sm => sm.Gross),
                VatAmount = s.Sum(sm => sm.VatAmount),
                Tax = s.Sum(sm => sm.TaxAmount)
            });
            var sumOdis = validOds.Where(w => w.Order.EndOfDayId == eodId && w.Order.PosId == piid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5).SelectMany(ss => ss.OrderDetailIgredients).SelectMany(sm => sm.OrderDetailIgredientVatAnal).GroupBy(g => g.VatId).Select(s => new
            {
                VatId = s.Key,
                Gross = s.Sum(sm => sm.Gross),
                VatAmount = s.Sum(sm => sm.VatAmount),
                Tax = s.Sum(sm => sm.TaxAmount)
            });

            var vatsTemp = sumOds.Union(sumOdis).GroupBy(g => g.VatId).Select(s => new
            {
                VatId = s.Key,
                Gross = s.Sum(sm => sm.Gross),
                VatAmount = s.Sum(sm => sm.VatAmount),
                Tax = s.Sum(sm => sm.Tax)
            }).ToList();

            var vats = (from q in vatsTemp
                        join v in db.Vat.ToList() on q.VatId equals v.Id
                        select new
                        {
                            VatId = q.VatId,
                            VatRate = v.Percentage,
                            VatAmount = (Decimal)Math.Round((Decimal)q.VatAmount, 2, MidpointRounding.AwayFromZero),
                            Net = (Decimal)Math.Round((Decimal)q.Gross, 2, MidpointRounding.AwayFromZero) - (Decimal)Math.Round((Decimal)q.VatAmount, 2, MidpointRounding.AwayFromZero) - (Decimal)Math.Round((Decimal)q.Tax, 2, MidpointRounding.AwayFromZero),
                            Gross = (Decimal)Math.Round((Decimal)q.Gross, 2, MidpointRounding.AwayFromZero),
                            Tax = (Decimal)Math.Round((Decimal)q.Tax, 2, MidpointRounding.AwayFromZero)
                        }).ToList();


            var voided = salesByReceipt.Where(f => f.IsVoided == true).SelectMany(s => s.Transactions).ToList();

            decimal? lockerPrice = 1;

            var totalLockers = db.Transactions.Where(w => w.EndOfDayId == null
                                                                 && w.PosInfoId == piid
                                                                 && (w.IsDeleted ?? false) == false
                                                                 && (w.TransactionType == 6
                                                                 || w.TransactionType == 7));
            decimal? openLocker = totalLockers.Where(w => w.TransactionType == 6).Sum(sm => sm.Amount) ?? 0;
            decimal? paidLocker = (totalLockers.Where(w => w.TransactionType == 7).Sum(sm => sm.Amount) * -1) ?? 0;




            var barcodes = db.Transactions.Where(w => w.PosInfoId == piid
                                                             && w.EndOfDayId == null
                                                             && w.TransactionType == 9
                                                             && (w.IsDeleted ?? false) == false
                                                             ).Sum(sm => sm.Amount) ?? 0;
            var rlp = db.RegionLockerProduct.FirstOrDefault();
            if (rlp != null)
                lockerPrice = rlp.Price;



            var prods = salesByReceipt.Where(w => w.IsVoided == false)//.AsNoTracking()
                                                                      //                           .Include("OrderDetailInvoices.OrderDetail")
                                                                      //.Where(w => w.EndOfDayId == null && w.PosInfoId == piid && (w.IsDeleted ?? false) == false && (w.IsVoided ?? false) == false)
                                               .SelectMany(w => w.OrderDetailInvoices).Select(s => new
                                               {
                                                   ProductId = s.OrderDetail.ProductId,
                                                   Qty = s.OrderDetail.Qty,
                                                   Total = s.OrderDetail.TotalAfterDiscount
                                               });

            var productsForEODStats = (from q in db.ProductsForEOD
                                       join qq in prods on q.ProductId equals qq.ProductId
                                       select new
                                       {
                                           ProductId = q.ProductId,
                                           Description = q.Description,
                                           Qty = qq.Qty,
                                           Total = qq.Total
                                       }).GroupBy(g => g.ProductId).Select(s => new
                                       {
                                           ProductId = s.FirstOrDefault().ProductId,
                                           Description = s.FirstOrDefault().Description,
                                           Qty = s.Sum(sm => sm.Qty),
                                           Total = s.Sum(sm => sm.Total)
                                       });
            var nodata = allInvoices.Count() == 0;
            var nosalesData = salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Count() == 0;


            var xDataToPrint = new
            {
                Day = eod.FODay,
                PosCode = posdata.Id,//Allazei sto webpos
                PosDescription = posdata.Description,
                ReportNo = eod.CloseId,
                Gross = nosalesData ? 0 : Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => (decimal?)sm.Total), 2),
                VatAmount = nodata ? 0 : vats.Sum(sm => Math.Round((decimal)sm.VatAmount, 2)),
                Net = nodata ? 0 : vats.Sum(sm => Math.Round((decimal)sm.Net, 2)),
                //                Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => (decimal?)sm.Total), 2) - Math.Round((decimal)(decimal)vats.Sum(sm => sm.VatAmount), 2),
                //Math.Round((decimal)vats.Sum(sm => sm.Net), 2),
                Discount = nosalesData ? 0 : Math.Round((decimal)salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Sum(sm => sm.Discount != null ? (decimal)sm.Discount : 0), 2),
                TicketCount = nosalesData ? 0 : salesByReceipt.Where(f => (f.IsVoided ?? false) == false).Count(),
                ItemsCount = nosalesData ? 0 : salesByReceipt.Where(f => (f.IsVoided ?? false) == false).SelectMany(s => s.OrderDetailInvoices).Sum(sm => sm.OrderDetail.Qty),

                PaymentAnalysis = allInvoices.Where(a => a.AccountType != 4 && a.IsVoided == false).ToList()
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().AccountDescription,// != null ? w.FirstOrDefault().Account.Description : "",
                                             Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.TransAmount), 2)
                                         }),
                VatAnalysis = vats,// vats.GroupBy(q => q.VatId).ToList()


                VoidAnalysis = voided//Where(f => f.Void == true && f.IsInvoice == true)
                                       .GroupBy(q => q.AccountId)
                                       .Select(w => new
                                       {
                                           Description = w.FirstOrDefault().Accounts != null ? w.FirstOrDefault().Accounts.Description : "",
                                           Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Amount), 2, MidpointRounding.AwayFromZero)
                                       }),
                CardAnalysis = allInvoices.Where(a => a.AccountType == 4).ToList()//salesByReceipt.Where(f => f.AccountType == 4 && f.Void == false && f.IsCancel == false && f.IsInvoice == true)
                                         .GroupBy(f => f.AccountId)
                                         .Select(w => new
                                         {
                                             Description = w.FirstOrDefault().AccountDescription,// != null ? w.FirstOrDefault().Account.Description : "",
                                             Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.TransAmount), 2, MidpointRounding.AwayFromZero)
                                         }),
                Barcodes = barcodes,
                Lockers = new
                {
                    HasLockers = rlp != null,
                    TotalLockers = openLocker / lockerPrice,
                    TotalLockersAmount = openLocker,
                    Paidlockers = (paidLocker / lockerPrice) ?? 0,
                    PaidlockersAmount = paidLocker,
                    OccLockers = (openLocker / lockerPrice) - (paidLocker / lockerPrice),
                    OccLockersAmount = openLocker - paidLocker
                },
                ProductsForEODStats = productsForEODStats
            };
            db.Configuration.LazyLoadingEnabled = false;
            return xDataToPrint;

            /*

            decimal? lockerPrice = 1;

            var totalLockers = db.Transactions.Where(w => w.EndOfDayId == eodId
                // && w.PosInfoId == piid
                                                     && (w.TransactionType == (int)TransactionType.OpenLocker
                                                     || w.TransactionType == (int)TransactionType.CloseLocker));
            decimal? openLocker = totalLockers.Where(w => w.TransactionType == (int)TransactionType.OpenLocker).Sum(sm => sm.Amount) ?? 0;
            decimal? paidLocker = (totalLockers.Where(w => w.TransactionType == (int)TransactionType.CloseLocker).Sum(sm => sm.Amount) * -1) ?? 0;




            var barcodes = db.Transactions.Where(w => //w.PosInfoId == piid &&
                                                  w.EndOfDayId == eodId
                                                 && w.TransactionType == (int)TransactionType.CreditCode
                                                 && (w.IsDeleted ?? false) == false
                                                 ).Sum(sm => sm.Amount) ?? 0;
            var rlp = db.RegionLockerProduct.FirstOrDefault();
            if (rlp != null)
                lockerPrice = rlp.Price;



            var prods = db.Invoices.AsNoTracking()
                                   .Include("OrderDetailInvoices.OrderDetail")
                                   .Where(w => w.EndOfDayId == eodId
                                       //    &&  w.PosInfoId == piid 
                                       && (w.IsDeleted ?? false) == false
                                       && (w.IsVoided ?? false) == false)
                                   .SelectMany(w => w.OrderDetailInvoices).Select(s => new
                                   {
                                       ProductId = s.OrderDetail.ProductId,
                                       Qty = s.OrderDetail.Qty,
                                       Total = s.OrderDetail.TotalAfterDiscount
                                   });

            var productsForEODStats = (from q in db.ProductsForEOD
                                       join qq in prods on q.ProductId equals qq.ProductId
                                       select new
                                       {
                                           ProductId = q.ProductId,
                                           Description = q.Description,
                                           Qty = qq.Qty,
                                           Total = qq.Total
                                       }).GroupBy(g => g.ProductId).Select(s => new
                                       {
                                           ProductId = s.FirstOrDefault().ProductId,
                                           Description = s.FirstOrDefault().Description,
                                           Qty = s.Sum(sm => sm.Qty),
                                           Total = s.Sum(sm => sm.Total)
                                       });

            var result = (from q in db.EndOfDay.Include("EndOfDayDetail")
                              //.Include("EndOfDayPaymentAnalysis")
                                        .Include("EndOfDayPaymentAnalysis.Accounts")
                              //.Include("EndOfDayVoidsAnalysis")
                                        .Include("EndOfDayVoidsAnalysis.Accounts")
                                        .Include("PosInfo")
                                        .Where(w => w.Id == eodId)
                          select new
                          {
                              Day = q.FODay,
                              PosCode = q.PosInfo.Code,//Allazei sto webpos
                              PosDescription = q.PosInfo.Description,
                              ReportNo = q.CloseId,
                              Gross = q.Gross,
                              VatAmount = q.EndOfDayDetail.Sum(s => s.VatAmount),
                              Net = q.Net,
                              Discount = q.Discount,
                              TicketCount = q.TicketsCount,
                              ItemsCount = q.ItemCount,

                              PaymentAnalysis = q.EndOfDayPaymentAnalysis.Where(f => f.Accounts.Type != 4).Select(s => new { Description = s.Accounts.Description, Amount = Math.Round((decimal)s.Total, 2) }),
                              VatAnalysis = q.EndOfDayDetail.Select(s => new { VatRate = s.VatRate, Gross = Math.Round((decimal)s.Gross, 2), Net = Math.Round((decimal)s.Net, 2), VatAmount = Math.Round((decimal)s.VatAmount, 2) }),
                              VoidAnalysis = q.EndOfDayVoidsAnalysis.Select(s => new { Description = s.Accounts.Description, Amount = Math.Round((decimal)s.Total, 2) }),
                              CardAnalysis = q.EndOfDayPaymentAnalysis.Where(f => f.Accounts.Type == 4).Select(s => new { Description = s.Accounts.Description, Amount = Math.Round((decimal)s.Total, 2) }),

                          }).Single();


            var newResults = new
            {
                PosCode = result.PosCode,//Allazei sto webpos
                PosDescription = result.PosDescription,
                ReportNo = result.ReportNo,
                Gross = result.Gross,
                VatAmount = result.VatAmount,
                Net = result.Net,
                Discount = result.Discount,
                TicketCount = result.TicketCount,
                ItemsCount = result.ItemsCount,

                PaymentAnalysis = result.PaymentAnalysis,
                VatAnalysis = result.VatAnalysis,
                VoidAnalysis = result.VoidAnalysis,
                CardAnalysis = result.CardAnalysis,


                Barcodes = barcodes,
                Lockers = new
                {
                    HasLockers = rlp != null,
                    TotalLockers = openLocker / lockerPrice,
                    TotalLockersAmount = openLocker,
                    Paidlockers = (paidLocker / lockerPrice) ?? 0,
                    PaidlockersAmount = paidLocker,
                    OccLockers = (openLocker / lockerPrice) - (paidLocker / lockerPrice),
                    OccLockersAmount = openLocker - paidLocker
                },
                ProductsForEODStats = productsForEODStats

            };
            return newResults;

    */
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


    }





    public class StatisticFilters
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Int64?> PosList { get; set; }
        public List<Int64?> StaffList { get; set; }
        public List<Int64?> InvoiceList { get; set; }
        public int ReportType { get; set; }
        public DateTime? FODay { get; set; }
        public long? EodId { get; set; } // 1-9-2014
        public bool UseEod { get; set; }
        public bool UsePeriod { get; set; }
        public int fromProductCode { get; set; }
        public int toProductCode { get; set; }
        public int grouping { get; set; }
        public List<Int64?> PricelistsList { get; set; }
        public List<Int64?> CategoriesList { get; set; }
        public List<Int64?> ProductCategoriesList { get; set; }
        public List<Int64?> DepartmentList { get; set; }
        public string FromProductCode { get; set; }
        public string ToProductCode { get; set; }
        public Int64? ReportListId { get; set; }
        public Int64? CostPriceListId { get; set; }
        public Int64? DisplayPriceListId { get; set; }
        public List<Int64> DisplayPriceList { get; set; }
        public bool UseOrderInvoicesType { get; set; }
    }

    public class VatValuesModel
    {
        public Int64? VatId { get; set; }
        public Decimal? VatRate { get; set; }
        public Decimal? VatAmount { get; set; }
        public Decimal? VatGross { get; set; }
        public Decimal? VatNet { get; set; }
    }

    public class LabStore
    {

        public Int64? Odi { get; set; }

        public Int64? Pos { get; set; }

        public Int64? Dept { get; set; }

        public DateTime? REC_DATE { get; set; }

        public DateTime? REC_TIME { get; set; }

        public DateTime? HOT_DATE { get; set; }

        public DateTime? FO_DAY { get; set; }

        public Int64? CLOSE_ID { get; set; }

        public Int64? ITEM_GROUP { get; set; }

        public Int64? ITEM_SUBGROUP { get; set; }

        public String ITEM_DESCR { get; set; }

        public String ITEM_CODE { get; set; }

        public Double? QTY { get; set; }

        public Decimal? AMOUNT { get; set; }

        public Decimal? Total { get; set; }

        public Int64? Waiter { get; set; }

        public String TTable { get; set; }

        public Int64? Room { get; set; }

        public Int64? Vat { get; set; }

        public Int64? Listino { get; set; }

        public Int32? Receipt { get; set; }

    }
}
