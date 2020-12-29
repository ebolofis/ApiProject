using Newtonsoft.Json;
using Pos_WebApi.Controllers;
using Pos_WebApi.Models.FilterModels;
using Pos_WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Pos_WebApi.Helpers
{
    public partial class StatisticsRepository
    {
        protected PosEntities dbContext;
        private BussinessRepository boRepo;

        public StatisticsRepository(PosEntities db)
        {
            dbContext = db;
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.AutoDetectChangesEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;


        }


        public IEnumerable<Object> GetReceiptsFor99006(Expression<Func<TempReceiptBOFull, bool>> predicate = null)
        {
            boRepo = new BussinessRepository(dbContext);
            var receipts = boRepo.ReceiptsBO(predicate);
            if (receipts.Count() > 0)
            {
                var query = (from q in receipts.ToList()
                             join qq in dbContext.PosInfo on q.PosInfoId equals qq.Id
                             join qqq in dbContext.Department on qq.DepartmentId equals qqq.Id into f
                             from d in f.DefaultIfEmpty()
                             join qqqq in dbContext.EndOfDay on q.EndOfDayId equals qqqq.Id into ff
                             from e in ff.DefaultIfEmpty()
                             join qqqqq in boRepo.ReceiptPaymentsFlat() on q.Id equals qqqqq.InvoicesId into fff
                             from trans in fff.DefaultIfEmpty()
                             join it in dbContext.InvoiceTypes on q.InvoiceTypeId equals it.Id

                             select new
                             {
                                 PosInfoId = q.PosInfoId,
                                 PosInfoDescription = qq.Description,
                                 DepartmentId = qq.DepartmentId,
                                 DepartmentDescription = qq.DepartmentId != null ? d.Description : "",
                                 FODay = q.EndOfDayId != null ? e.FODay : null,
                                 //TimeZone = q.
                                 TimeZoneId = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? 1
                                                                 : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                                 ? 2 : ((q.Day.Value.Hour >= 19) && (q.Day.Value.Hour <= 24) || ((q.Day.Value.Hour >= 1) && (q.Day.Value.Hour <= 2))) ? 3 : 4,
                                 TimeZone = ((q.Day.Value.Hour >= 7) && (q.Day.Value.Hour <= 12)) ? "Breakfast"
                                                                 : ((q.Day.Value.Hour >= 13) && (q.Day.Value.Hour <= 18))
                                                                 ? "Lunch" : "Dinner",
                                 InvoiceId = q.Id,
                                 Abbreviation = q.Abbreviation,
                                 InvoiceTypeCode = q.InvoiceTypeType,
                                 Description = it.Description,
                                 Counter = q.ReceiptNo,
                                 StaffId = q.StaffId,
                                 StaffName = q.StaffFullName,
                                 Day = q.Day,
                                 Cover = q.Cover,
                                 CloseId = q.EndOfDayId != null ? e.CloseId : 0,
                                 TableId = q.TableId,
                                 TableCode = q.TableCode,
                                 Room = q.Room,
                                 AccountId = trans != null ? trans.AccountId : null,
                                 AccountDescription = trans != null ? trans.AccountDescription : "",
                                 Guest = trans != null ? trans.LastName : null,
                                 InvoiceTotal = q.Total,
                                 TransactionTotal = trans != null ? trans.Amount : 0,
                                 Discount = q.Discount ?? 0

                             }).ToList();
                return query;
            }
            else
                return receipts;
        }


        public IEnumerable<Object> GetReceiptsFor99005(Expression<Func<TempReceiptBOFull, bool>> predicate = null)
        {
            boRepo = new BussinessRepository(dbContext);
            var receipts = boRepo.ReceiptsBO(predicate).ToList();
            var eods = receipts.Select(s => s.EndOfDayId).Distinct();
            if (receipts.Count() > 0)
            {
                var query = from q in boRepo.ReceiptDetailsBO(x => eods.Contains(x.EndOfDayId)).ToList()
                            join qq in receipts on q.ReceiptsId equals qq.Id
                            select new
                            {
                                InvoiceId = q.IsInvoiced,
                                PosInfoId = q.PosInfoId,
                                PosInfoDescription = "",//q.PosInfoDescription,
                                IsExtra = q.IsExtra,
                                Day = qq.Day,
                                StaffId = q.StaffId,
                                StaffName = qq.StaffName,
                                TableCode = q.TableCode,
                                OrderDetailId = q.OrderDetailId,
                                InvoiceTypeCode = qq.InvoiceTypeType,
                                InvoiceType = q.InvoiceType,
                                InvoiceDescription = qq.InvoiceDescription,
                                InvoiceCounter = q.ReceiptNo,
                                InvoiceTotal = qq.Total,
                                IsVoided = qq.IsVoided,
                                Status = q.Status,
                                OrderId = q.OrderId,
                                ProductCode = q.ItemCode,
                                ProductId = q.ProductId,
                                ProductDescription = q.ItemDescr,
                                UnitPrice = q.Price,
                                Qty = q.ItemQty,
                                Total = q.ItemGross,//s.Sum(sm => sm.Total),
                                Discount = q.ItemDiscount,//s.Sum(sm => sm.Discount),
                                DiscountRemark = qq.DiscountRemark,
                                ProductCategoryId = q.ProductCategoryId,
                                ProductCategoryDescription = "",//q.ProductCategoryDescripti,
                                CategoryId = 0,//q.CategoryId,
                                CategoryDescription = "",//q.CategoryDescription,
                                PriceListId = q.PricelistId,
                                PriceListDescription = "",//q.PriceListDescription,
                                SalesTypeId = q.SalesTypeId,
                                SalesTypeDescription = "",//q.SalesTypeDescription,
                                VatId = q.VatId,
                                VatAmount = q.ItemVatValue,
                                VatRate = q.ItemVatRate,
                                Net = q.ItemNet,
                                Tax = q.TaxId,
                                //Transactions = s.Select(ss => new { Description = ss.TransactionsDescription, Room = ss.Room, Amount = ss.TransactionsAmount }).Distinct(),
                                //                            TransStaffId = q.TransStaffId,
                                //                          TransStaffName = q.TransStaffName,
                                //                          TotalForNoTransaction = q.TotalForNoTransaction,
                                //      FODay = qq.FODay,
                            };

                return query;
            }
            else
                return receipts;

        }



        public Object GetReceiptsFor9011(string filters)
        {
            //var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            //var results = dbContext.ReceiptsNoFunctions//.Where(w => w.DepartmentId == 3)
            //    .Select(s => new
            //    {
            //        PosInfoCode = s.PosInfoId,
            //        DepartmentId = s.DepartmentId,
            //        DepartmentDesc = s.DepartmentDescription,
            //        //OrderDetailId = s.FirstOrDefault().OrderDetailId,
            //        InvoiceId = s.Id,
            //        Description = s.InvoiceDescription,
            //        IsVoided = s.IsVoided,
            //        Counter = s.ReceiptNo,
            //        InvoiceTotal = s.Total,
            //        InvoiceDiscount = s.Discount,
            //        StaffId = s.StaffId,
            //        InvoiceTypeId = s.InvoiceTypeId,
            //        Day = s.Day,
            //        Cover = s.Cover,
            //        EndOfDayId = s.EndOfDayId,
            //        TableId = s.TableId,
            //        RoomId = 0,
            //        ProductCategory = "",
            //        Vat1 = s.Vat1,
            //        Vat2 = s.Vat2,
            //        Vat3 = s.vat3,
            //        Vat4 = s.Vat4,
            //        Vat5 = s.Vat5,
            //        Cash = s.Cash,
            //        RoomCharge = s.RoomCharge,

            //        Category = "",
            //        //                VatId = s.VatId,
            //        ////                VatRate = s.FirstOrDefault().VatRate,
            //        //                VatAmount = s.Sum(sm => sm.VatAmount),
            //        //                Net = s.Sum(sm => sm.Net),
            //        //                Gross = s.Sum(sm => sm.Gross),
            //        //                TotalAfterDiscount = s.Sum(sm => sm.TotalAfterDiscount),
            //        //               Discount = s.Sum(sm => sm.Discount),
            //        //               PricelistDescription = s.FirstOrDefault().PricelistDescription,
            //        //               PricelistId = s.FirstOrDefault().PricelistId,
            //        //               TimeZone = s.FirstOrDefault().TimeZone,
            //        //               Inhouse = s.FirstOrDefault().Inhouse,
            //        Abbreviation = s.Abbreviation,
            //        PosInfoDescription = s.PosInfoDescription,
            //        //                InvoiceTypeDescription = s.InvoiceTypeDescription,
            //        InvoiceTypeCode = s.InvoiceTypeType
            //    });
            //var a = results.ToList().GroupBy(g => new { g.PosInfoCode, g.DepartmentId, g.InvoiceId, g.InvoiceTypeCode }).Select(s => new
            //{
            //    PosInfoCode = s.Key.PosInfoCode,
            //    PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
            //    DepartmentId = s.Key.DepartmentId,
            //    DepartmentDescription = s.FirstOrDefault().DepartmentDesc,
            //    InvoiceTypeCode = s.Key.InvoiceTypeCode,
            //    InvoiceAbb = s.FirstOrDefault().Description,
            //    CountInvs = s.Count(),
            //    Total = s.Sum(sm => sm.InvoiceTotal)
            //}).Distinct();
            //return new { a };
            return null;
        }

        public Object GetDataFor90130(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);

            var posinfos = dbContext.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId));

            var eods = dbContext.EndOfDay.Where(w => w.FODay >= flts.FromDate.Date && w.FODay <= flts.ToDate.Date);

            var filteredInvoicesWithEod = from q in dbContext.Invoices.Where(w => (w.IsVoided ?? false == false)
                                                                               && (w.IsDeleted ?? false) == false
                                                                               && w.IsPrinted == true
                                                                               && w.EndOfDayId == null
                                                                               && (w.InvoiceTypes.Type != 2 || w.InvoiceTypes.Type != 3))
                                          join qq in posinfos on q.PosInfoId equals qq.Id
                                          select new
                                          {
                                              Id = q.Id,
                                              Description = q.Description,
                                              Total = q.Total,
                                              EndOfDayId = q.EndOfDayId,
                                          };

            if (flts.UseEod)
                filteredInvoicesWithEod = from q in dbContext.Invoices.Where(w => (w.IsVoided ?? false == false)
                                                                               && (w.IsDeleted ?? false) == false
                                                                               && w.IsPrinted == true
                                                                               && (w.InvoiceTypes.Type != 2 || w.InvoiceTypes.Type != 3))
                                          join qq in eods on q.EndOfDayId equals qq.Id
                                          join qqq in posinfos on q.PosInfoId equals qqq.Id
                                          select new
                                          {
                                              Id = q.Id,
                                              Description = q.Description,
                                              Total = q.Total,
                                              EndOfDayId = q.EndOfDayId,
                                          };

            var catProdCat = from q in dbContext.ProductCategories
                             join qq in dbContext.Categories on q.CategoryId equals qq.Id into f
                             from cat in f.DefaultIfEmpty()
                             join qqq in dbContext.Product on q.Id equals qqq.ProductCategoryId
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


            var prods = from q in dbContext.OrderDetailInvoices
                        join qq in dbContext.Invoice_Guests_Trans on q.InvoicesId equals qq.InvoiceId
                        join qqq in filteredInvoicesWithEod on q.InvoicesId equals qqq.Id
                        select new
                        {
                            InvoicesId = q.InvoicesId,
                            OrderDetailId = q.OrderDetailId,
                            GuestId = qq.GuestId,
                            TransactionId = qq.TransactionId
                        };
            var odis = from q in dbContext.OrderDetail.Where(w => w.Status != 5 && (w.IsDeleted ?? false) == false)
                       join qq in prods on q.Id equals qq.OrderDetailId
                       join qqq in catProdCat on q.ProductId equals qqq.ProductId
                       select new
                       {
                           InvoicesId = qq.InvoicesId,
                           ProductId = q.ProductId,
                           ProductCode = qqq.ProductCode,
                           ProductDescription = qqq.ProductDescription,
                           ProductCategoryId = qqq.ProductCategoryId,
                           CategoryId = qqq.CategoryId,
                           ProductCategory = qqq.ProductCategory,
                           Category = qqq.Category,
                           DetailTotal = q.TotalAfterDiscount,
                           GuestId = qq.GuestId,
                           TransactionId = qq.TransactionId
                       };

            var invByRoom = (from q in odis
                             join qq in dbContext.Transactions on q.TransactionId equals qq.Id
                             join qqq in dbContext.Guest on q.GuestId equals qqq.Id
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
                                 ProductCode = q.ProductCode,
                                 ProductId = q.ProductId,
                                 ProductDescription = q.ProductDescription,
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
                                 ProductCode = s.FirstOrDefault().ProductCode,
                                 ProductId = s.FirstOrDefault().ProductId,
                                 ProductDescription = s.FirstOrDefault().ProductDescription,
                                 Category = s.FirstOrDefault().Category,
                                 DetailTotal = s.Sum(q => q.DetailTotal),

                             });
            var storeid = dbContext.Store.FirstOrDefault() != null ? dbContext.Store.FirstOrDefault().Description : "";
            var final = invByRoom.Distinct().ToList().GroupBy(g => new { g.CategoryId, g.ProductCategoryId, g.Room, g.AccountId }).Select(s => new
            {
                StoreId = storeid,
                Room = s.FirstOrDefault().Room,
                AccountId = s.FirstOrDefault().AccountId,
                //AccountDescription = s.FirstOrDefault().AccountDescription,
                CategoryId = s.FirstOrDefault().CategoryId,
                Category = s.FirstOrDefault().Category,
                DetailTotal = s.Distinct().Sum(sm => sm.DetailTotal),
                ProductCode = s.FirstOrDefault().ProductCode,
                ProductDescription = s.FirstOrDefault().ProductDescription,
                ProductCategory = s.FirstOrDefault().ProductCategory
            });


            //final.Where(w => w.AccountId == 2).OrderBy(r => r.Room);
            var metadata = dbContext.MetadataTable.Where(w => w.ReportType == flts.ReportType);


            return new { processedResults = final, metadata };

        }

        public Object GetDataFor9007(string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var poslist = dbContext.PosInfo.Where(w => flts.DepartmentList.Contains(w.DepartmentId));
            var itList = dbContext.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void).Select(s => s.Id);
            var vitList = itList;//.Where(w => flts.InvoiceList.Contains(w));
            var metadata = dbContext.MetadataTable.Where(w => w.ReportType == flts.ReportType);


            dbContext.Configuration.LazyLoadingEnabled = true;
            var eods = dbContext.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                    EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id);

            var validInvs = (from q in dbContext.Invoices.Where(w => (w.IsDeleted ?? false) == false)
                             join qqqq in flts.PosList on q.PosInfoId equals qqqq
                             join qqq in vitList on q.InvoiceTypeId equals qqq
                             //join ed in eods on q.EndOfDayId  equals ed
                             join qq in dbContext.OrderDetailInvoices on q.Id equals qq.InvoicesId
                             join q5 in poslist/*db.PosInfo*/ on q.PosInfoId equals q5.Id
                             select new { EndOfDayId = q.EndOfDayId, InvoicesId = q.Id, OrderDetailId = qq.OrderDetailId, PosInfoId = q.PosInfoId, PosInfoDescription = q5.Description }).Distinct();
            if (flts.UseEod)
            {
                validInvs = from q in validInvs
                            join qq in eods on q.EndOfDayId equals qq
                            select q;
            }
            else
            {
                validInvs = validInvs.Where(w => w.EndOfDayId == null);
            }
            var storeid = dbContext.Store.FirstOrDefault() != null ? dbContext.Store.FirstOrDefault().Description : "";
            //validInvs.Distinct().Count().Dump();
            var orderDet = (from q in dbContext.OrderDetail
                            join qqq in validInvs on q.Id equals qqq.OrderDetailId
                            join qq in dbContext.OrderDetailVatAnal on q.Id equals qq.OrderDetailId
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

            var ingDet = (from q in dbContext.OrderDetailIgredients.Where(w => w.TotalAfterDiscount != 0)
                          join qqq in validInvs on q.OrderDetailId equals qqq.OrderDetailId
                          join qq in dbContext.OrderDetailIgredientVatAnal on q.Id equals qq.OrderDetailIgredientsId
                          join qqqq in dbContext.OrderDetail on q.OrderDetailId equals qqqq.Id
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
                         join inv in dbContext.Invoices.Where(w => (w.IsDeleted ?? false) == false) on q.InvoicesId equals inv.Id
                         join pd in dbContext.PricelistDetail on q.PriceListDetailId equals pd.Id
                         join s in dbContext.Staff on inv.StaffId equals s.Id
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
            var productsflat = (from q in dbContext.Product
                                join qq in dbContext.ProductCategories on q.ProductCategoryId equals qq.Id into f
                                from pc in f.DefaultIfEmpty()
                                join qqq in dbContext.Categories on pc.CategoryId equals qqq.Id into ff
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
            var ins = dbContext.Ingredients.Select(s => new { Id = s.Id, Description = s.SalesDescription, Code = s.Code });
            var prefinal = (from q in query
                            join qq in productsflat on q.ProductId equals qq.ProductId into ff
                            from prd in ff.DefaultIfEmpty()
                            join qqq in ins on q.IngredientId equals qqq.Id into f
                            from ing in f.DefaultIfEmpty()
                            join qqqq in dbContext.Pricelist on q.PriceList equals qqqq.Id
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
            var final = prefinal.GroupBy(g => new { g.ProductId, g.StaffId, g.IsVoided, g.PosInfoId, g.IsProduct }).Select(s => new
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
        }


        //public IEnumerable<dynamic> GetDaillySalesWithVat(long departmentId, long? posInfoId)
        //{

        //}

        //private IEnumerable<AuditGroupByInvoiceBaseClass> GetAuditGroupByInvoice(IQueryable<AuditFlatInvoicesBaseClass> flatInvoices, Expression<Func<AuditGroupByInvoiceBaseClass, bool>> predicate = null, int? byAccount = null)
        //{
        //    var query = flatInvoices.ToList().GroupBy(g => g.InvoiceId);
        //    if (byAccount != null)
        //        query = query.Where(a => a.Any(aa => aa.AccountType == byAccount));
        //    var qroupedByInvoice = query
        //                              .Select(s => new AuditGroupByInvoiceBaseClass
        //                              {
        //                                  InvoiceId = s.FirstOrDefault().InvoiceId,
        //                                  OrderNo = s.FirstOrDefault().OrderNo,
        //                                  Abbreviation = s.FirstOrDefault().InvoiceAbbreviation,
        //                                  ReceiptNo = s.FirstOrDefault().Counter,
        //                                  InvoiceType = s.FirstOrDefault().InvoiceType,
        //                                  IsPaid = s.FirstOrDefault().IsPaid,
        //                                  InvoiceTotal = s.FirstOrDefault().InvoiceType == 3 ? s.FirstOrDefault().InvoiceTotal * -1 : s.FirstOrDefault().InvoiceTotal,
        //                                  InvoiceDiscount = s.FirstOrDefault().InvoiceDiscount,
        //                                  InvoicePaidAmount = s.FirstOrDefault().InvoicePaidAmount ?? 0,
        //                                  IsInvoiced = s.FirstOrDefault().IsInvoiced,
        //                                  IsVoided = s.FirstOrDefault().IsVoided,
        //                                  Vat1 = s.Where(w => w.VatId == 1).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
        //                                  Vat2 = s.Where(w => w.VatId == 2).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
        //                                  Vat3 = s.Where(w => w.VatId == 3).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
        //                                  Vat4 = s.Where(w => w.VatId == 4).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
        //                                  Vat5 = s.Where(w => w.VatId == 5).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
        //                                  AccountType1 = s.Where(w => w.AccountType == 1).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType2 = s.Where(w => w.AccountType == 2).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType3 = s.Where(w => w.AccountType == 3).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType4 = s.Where(w => w.AccountType == 4).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType5 = s.Where(w => w.AccountType == 5).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType6 = s.Where(w => w.AccountType == 6).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType7 = s.Where(w => w.AccountType == 7).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType8 = s.Where(w => w.AccountType == 8).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType9 = s.Where(w => w.AccountType > 8).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
        //                                  AccountType1ReceiptCount = s.Where(w => w.AccountType == 1).Count() > 0 ? 1 : 0,
        //                                  AccountType2ReceiptCount = s.Where(w => w.AccountType == 2).Count() > 0 ? 1 : 0,
        //                                  AccountType3ReceiptCount = s.Where(w => w.AccountType == 3).Count() > 0 ? 1 : 0,
        //                                  AccountType4ReceiptCount = s.Where(w => w.AccountType == 4).Count() > 0 ? 1 : 0,
        //                                  AccountType5ReceiptCount = s.Where(w => w.AccountType == 5).Count() > 0 ? 1 : 0,
        //                                  AccountType6ReceiptCount = s.Where(w => w.AccountType == 6).Count() > 0 ? 1 : 0,
        //                                  AccountType7ReceiptCount = s.Where(w => w.AccountType == 7).Count() > 0 ? 1 : 0,
        //                                  AccountType8ReceiptCount = s.Where(w => w.AccountType == 8).Count() > 0 ? 1 : 0,
        //                                  AccountType9ReceiptCount = s.Where(w => w.AccountType > 8).Count() > 0 ? 1 : 0,
        //                                  TransAmount = s.Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount }).Distinct().Sum(sm => sm.TransAmount),
        //                                  CardAnalysis = s.Where(w => w.AccountType == 4).Select(ss => new
        //                                  {
        //                                      AccountId = ss.AccountId,
        //                                      TransAmount = ss.TransAmount
        //                                  })
        //                                            .Distinct()
        //                                            .GroupBy(g => g.AccountId).Select(ssss => new AuditCreditCardsAmounts
        //                                            {
        //                                                AccountId = ssss.Key,
        //                                                TransAmount = ssss.Sum(sm => sm.TransAmount)
        //                                            }).ToList(),
        //                                  ItemsCount = s.Count(),
        //                                  StaffId = s.FirstOrDefault().StaffId,
        //                                  FiscalType = s.FirstOrDefault().FiscalType,
        //                                  FODay = s.FirstOrDefault().FODay,
        //                                  EndOfDayId = s.FirstOrDefault().EndOfDayId,
        //                                  Covers = s.FirstOrDefault().Covers,
        //                                  Check = s.FirstOrDefault().InvoiceTotal - s.Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
        //                                  Room = s.FirstOrDefault().Room,
        //                                  TableCode = s.FirstOrDefault().TableCode
        //                              }).AsQueryable<AuditGroupByInvoiceBaseClass>();
        //    if (predicate != null)
        //        return qroupedByInvoice = qroupedByInvoice.Where(predicate);
        //    else
        //        return qroupedByInvoice;
        //}


        //private IQueryable<AuditFlatInvoicesBaseClass> GetAuditFlatInvoicesBase(Expression<Func<AuditFlatInvoicesBaseClass, bool>> predicate = null)
        //{
        //    var query = from q in boRepo.ReceiptDetailsBO(x => x.EndOfDayId == null)
        //                join qq in boRepo.ReceiptsBO(x => x.EndOfDayId == 0) on q.ReceiptsId equals qq.Id
        //                join qqq in boRepo.ReceiptPaymentsFlat(x => x.EndOfDayId == 0) on q.ReceiptsId equals qqq.InvoicesId into f
        //                from tr in f.DefaultIfEmpty()
        //                where q.InvoiceType != 8 && q.InvoiceType != 11 && q.InvoiceType != 12
        //                   && !(q.InvoiceType == 2 && qq.IsVoided == true)
        //                //&& qq.EndOfDayId == null
        //                select new AuditFlatInvoicesBaseClass
        //                {
        //                    InvoiceId = qq.Id,
        //                    OrderNo = q.OrderNo,
        //                    Counter = qq.ReceiptNo,
        //                    Covers = qq.Cover,
        //                    InvoiceAbbreviation = q.Abbreviation,
        //                    InvoiceType = q.InvoiceType,
        //                    InvoiceTotal = qq.Total,
        //                    InvoiceDiscount = qq.Discount,
        //                    InvoicePaidAmount = qq.PaidTotal,
        //                    IsPaid = qq.IsPaid,
        //                    IsInvoiced = q.InvoiceType == 2 ? qq.IsInvoiced : true,
        //                    IsVoided = qq.IsVoided,
        //                    // IsDeleted = qq.IsDeleted ?? false,
        //                    VatId = q.VatId,
        //                    Total = q.ItemGross,
        //                    VatAmount = q.ItemVatValue,
        //                    VatRate = q.ItemVatRate,
        //                    Net = q.ItemNet,
        //                    AccountId = tr != null ? tr.AccountId : 0,
        //                    //AccountDesc = tr != null? tr.AccountDescription:"",
        //                    TableCode = q.TableCode,
        //                    AccountType = tr != null ? tr.AccountType : 0,
        //                    TransAmount = tr != null ? tr.Amount : 0,
        //                    TransctionId = tr != null ? (long?)tr.Id : null,
        //                    TransStaff = tr.StaffId,
        //                    StaffId = qq.StaffId,
        //                    OrderDetailId = q.OrderDetailId,
        //                    //FiscalType = qq.FiscalType,
        //                    //FODay = q.FOdd ..eod == null ? DateTime.Now : eod.FODay,
        //                    EndOfDayId = q.EndOfDayId,
        //                    PosInfoId = q.PosInfoId,
        //                    Room = qq.Room
        //                    //PaidStatus = q.Paid
        //                };
        //    if (predicate != null)
        //        query = query.Where(predicate);
        //    return query;
        //}


    }

}
