using LinqKit;
using log4net;
using Pos_WebApi.Models.FilterModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace Pos_WebApi.Repositories
{
    public class BussinessRepository : IDisposable
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BussinessRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }


        public IQueryable<TempInvoiceRefForDetails> InvoiceRefForDetailsRaw(Expression<Func<TempInvoiceRefForDetails, bool>> invoicePredicate)
        {

            var queryBuilder = new StringBuilder();

            queryBuilder.AppendLine(@"select");
            queryBuilder.AppendLine(@"    InvoicesId = inv.Id,");
            queryBuilder.AppendLine(@"InvoiceTypesId = inv.InvoiceTypeId,");
            queryBuilder.AppendLine(@"InvoiceTypeCode = it.Code,");
            queryBuilder.AppendLine(@"InvoiceTypeType = it.Type,");
            queryBuilder.AppendLine(@"PosInfoId = inv.PosInfoId,");
            queryBuilder.AppendLine(@"OrderDetailId = od.Id,");
            queryBuilder.AppendLine(@"Abbreviation = it.Abbreviation,");
            queryBuilder.AppendLine(@"PosInfoDetailId = inv.PosInfoDetailId,");
            queryBuilder.AppendLine(@"OrderNo = o.OrderNo,");
            queryBuilder.AppendLine(@"ReceiptNo = inv.Counter,");
            queryBuilder.AppendLine(@"Guid = od.Guid,");
            queryBuilder.AppendLine(@"    StaffId = inv.StaffId,");
            queryBuilder.AppendLine(@"PaidStatus = od.PaidStatus");
            queryBuilder.AppendLine(@"                from OrderDetailInvoices odi");
            queryBuilder.AppendLine(@"join OrderDetail od on od.Id = odi.OrderDetailId and IsNull(od.IsDeleted, 0) = 0");
            queryBuilder.AppendLine(@"join[Table] tbl on tbl.Id = od.TableId");
            queryBuilder.AppendLine(@"join[Order] o on od.OrderId = o.Id");
            queryBuilder.AppendLine(@"join Invoices inv on inv.Id = odi.InvoicesId");
            queryBuilder.AppendLine(@"join InvoiceTypes it on  it.Id = inv.InvoiceTypeId");
            queryBuilder.AppendLine(@"where  (od.Status < 2) and ((it.Type = 2 and od.PaidStatus = 0) or (it.Type != 2 and od.PaidStatus > 0 ))");
            var query = DbContext.Database.SqlQuery<TempInvoiceRefForDetails>(queryBuilder.ToString());




            return query.AsQueryable<TempInvoiceRefForDetails>();

        }
        public IQueryable<TempInvoiceRefForDetails> InvoiceRefForDetails(Expression<Func<Invoices, bool>> invoicePredicate,
                                                        Expression<Func<InvoiceTypes, bool>> invoiceTypePredicate,
                                                        Expression<Func<OrderDetail, bool>> orderDetailPredicate,
                                                        Expression<Func<Order, bool>> orderPredicate)
        {
            var query = from i in DbContext.Invoices.Where(invoicePredicate)
                        join q in DbContext.OrderDetailInvoices on i.Id equals q.InvoicesId
                        join it in DbContext.InvoiceTypes.Where(invoiceTypePredicate) on i.InvoiceTypeId equals it.Id
                        join od in DbContext.OrderDetail.Where(orderDetailPredicate) on q.OrderDetailId equals od.Id
                        join o in DbContext.Order.Where(orderPredicate) on od.OrderId equals o.Id
                        where (i.IsDeleted ?? false) == false && (od.IsDeleted ?? false) == false
                        select new TempInvoiceRefForDetails
                        {
                            InvoicesId = q.InvoicesId,
                            EndOfDayId = i.EndOfDayId,
                            InvoiceTypesId = i.InvoiceTypeId,
                            InvoiceTypeCode = it.Code,
                            PosInfoId = i.PosInfoId,
                            InvoiceTypeType = it.Type,
                            OrderDetailId = od.Id,
                            Abbreviation = it.Abbreviation,
                            PosInfoDetailId = i.PosInfoDetailId,
                            OrderNo = o.OrderNo,
                            ReceiptNo = i.Counter,
                            Guid = od.Guid,
                            StaffId = i.StaffId,
                            PaidStatus = od.PaidStatus,
                            TableId = od.TableId,
                            Cover = i.Cover

                        };
            return query.AsQueryable<TempInvoiceRefForDetails>();

        }


        public IQueryable<TempInvoiceRefForDetails> InvoiceRefForDetailsExp(Expression<Func<TempInvoiceRefForDetails, bool>> predicate = null)
        {
            var query = from i in DbContext.Invoices
                        join q in DbContext.OrderDetailInvoices on i.Id equals q.InvoicesId
                        join it in DbContext.InvoiceTypes on i.InvoiceTypeId equals it.Id
                        join od in DbContext.OrderDetail on q.OrderDetailId equals od.Id
                        join o in DbContext.Order on od.OrderId equals o.Id
                        where (i.IsDeleted ?? false) == false && (od.IsDeleted ?? false) == false
                        select new TempInvoiceRefForDetails
                        {
                            InvoicesId = q.InvoicesId,
                            EndOfDayId = i.EndOfDayId,
                            InvoiceTypesId = i.InvoiceTypeId,
                            InvoiceTypeCode = it.Code,
                            PosInfoId = i.PosInfoId,
                            InvoiceTypeType = it.Type,
                            OrderDetailId = od.Id,
                            Abbreviation = it.Abbreviation,
                            PosInfoDetailId = i.PosInfoDetailId,
                            OrderNo = o.OrderNo,
                            ReceiptNo = i.Counter,
                            Guid = od.Guid,
                            StaffId = i.StaffId,
                            PaidStatus = od.PaidStatus,
                            TableId = od.TableId,
                            Cover = i.Cover,
                            PriceListDetailId = od.PriceListDetailId,
                            TotalAfterDiscount = od.TotalAfterDiscount,
                            Discount = od.Discount,
                            ProductId = od.ProductId,
                            Status = od.Status,
                            Qty = od.Qty,
                            OrderId = od.OrderId

                        };
            if (predicate == null)
                return query;
            else
                return query.Where(predicate);

        }



        public IQueryable<TempOrderDetailVatFlat> OrderDetailIgredientVatFlatExp(Expression<Func<TempOrderDetailVatFlat, bool>> predicate = null)
        {
            var query = from q in DbContext.OrderDetailIgredientVatAnal
                        join qqq in DbContext.Vat on q.VatId equals qqq.Id
                        join qqqq in DbContext.Tax on q.TaxId equals qqqq.Id into f
                        from tx in f.DefaultIfEmpty()
                        where q.IsDeleted == null || q.IsDeleted == false
                        select new TempOrderDetailVatFlat
                        {
                            //       ReceiptsId = irfd.InvoicesId,
                            Id = q.OrderDetailIgredientsId,
                            OrderDetailId = q.OrderDetailIgredientsId,
                            Gross = q.Gross,
                            Net = q.Net,
                            VatRate = q.VatRate,
                            VatId = q.VatId,
                            VatAmount = q.VatAmount,
                            VatCode = qqq.Code,
                            CurrentVatPercentage = qqq.Percentage,
                            TaxId = q.TaxId,
                            TaxAmount = q.TaxAmount,
                            TaxRate = q.TaxId != null ? tx.Percentage : null

                        };
            if (predicate == null)
                return query.AsQueryable();
            else
                return query.Where(predicate);
        }


        public IQueryable<TempOrderDetailVatFlat> OrderDetailIgredientVatFlat(Expression<Func<Invoices, bool>> invoicePredicate,
                                                                     Expression<Func<InvoiceTypes, bool>> invoiceTypePredicate,
                                                                     Expression<Func<OrderDetail, bool>> orderDetailPredicate,
                                                                     Expression<Func<Order, bool>> orderPredicate)
        {
            var query = from irfd in InvoiceRefForDetails(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate)
                        join q in DbContext.OrderDetailIgredients on irfd.OrderDetailId equals q.OrderDetailId
                        join qq in DbContext.OrderDetailIgredientVatAnal on q.Id equals qq.OrderDetailIgredientsId
                        join qqq in DbContext.Vat on qq.VatId equals qqq.Id
                        join qqqq in DbContext.Tax on qq.TaxId equals qqqq.Id into f
                        from tx in f.DefaultIfEmpty()
                        where q.IsDeleted == null || q.IsDeleted == false
                        select new TempOrderDetailVatFlat
                        {
                            ReceiptsId = irfd.InvoicesId,
                            Id = q.Id,
                            OrderDetailId = q.OrderDetailId,
                            Gross = q.TotalAfterDiscount,
                            Net = qq.Net,
                            VatRate = qq.VatRate,
                            VatId = qq.VatId,
                            VatAmount = qq.VatAmount,
                            VatCode = qqq.Code,
                            CurrentVatPercentage = qqq.Percentage,
                            TaxId = qq.TaxId,
                            TaxAmount = qq.TaxAmount,
                            TaxRate = qq.TaxId != null ? tx.Percentage : null

                        };
            return query.AsQueryable();
        }

        public IQueryable<TempOrderDetailVatFlat> OrderDetailVatFlat(Expression<Func<Invoices, bool>> invoicePredicate,
                                                                     Expression<Func<InvoiceTypes, bool>> invoiceTypePredicate,
                                                                     Expression<Func<OrderDetail, bool>> orderDetailPredicate,
                                                                     Expression<Func<Order, bool>> orderPredicate)
        {
            var query = from irfd in InvoiceRefForDetails(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate)
                        join q in DbContext.OrderDetail.Where(orderDetailPredicate) on irfd.OrderDetailId equals q.Id
                        join qq in DbContext.OrderDetailVatAnal on q.Id equals qq.OrderDetailId
                        join qqq in DbContext.Vat on qq.VatId equals qqq.Id
                        join qqqq in DbContext.Tax on qq.TaxId equals qqqq.Id into f
                        from tx in f.DefaultIfEmpty()
                        where q.IsDeleted == null || q.IsDeleted == false
                        select new TempOrderDetailVatFlat
                        {
                            ReceiptsId = irfd.InvoicesId,
                            Id = q.Id,
                            OrderDetailId = q.Id,
                            Gross = q.TotalAfterDiscount,
                            Net = qq.Net,
                            VatRate = qq.VatRate,
                            VatId = qq.VatId,
                            VatAmount = qq.VatAmount,
                            VatCode = qqq.Code,
                            CurrentVatPercentage = qqq.Percentage,
                            TaxId = qq.TaxId,
                            TaxAmount = qq.TaxAmount,
                            TaxRate = qq.TaxId != null ? tx.Percentage : null
                        };
            return query.AsQueryable();
        }
        public IQueryable<TempOrderDetailVatFlat> OrderDetailVatFlatExp(Expression<Func<TempOrderDetailVatFlat, bool>> predicate = null)
        {
            var query = from q in DbContext.OrderDetailVatAnal
                        join qqq in DbContext.Vat on q.VatId equals qqq.Id
                        join qqqq in DbContext.Tax on q.TaxId equals qqqq.Id into f
                        from tx in f.DefaultIfEmpty()
                        select new TempOrderDetailVatFlat
                        {
                            //  ReceiptsId = irfd.InvoicesId,
                            Id = q.Id,
                            OrderDetailId = q.OrderDetailId,
                            Gross = q.Gross,
                            Net = q.Net,
                            VatRate = q.VatRate,
                            VatId = q.VatId,
                            VatAmount = q.VatAmount,
                            VatCode = qqq.Code,
                            CurrentVatPercentage = qqq.Percentage,
                            TaxId = q.TaxId,
                            TaxAmount = q.TaxAmount,
                            TaxRate = q.TaxId != null ? tx.Percentage : null
                        };
            if (predicate == null)
                return query.AsQueryable();
            else
                return query.Where(predicate);
        }
        public IQueryable<TempProductsWithCategoriesFlat> ProductsWithCategoriesFlat(Expression<Func<TempProductsWithCategoriesFlat, bool>> predicate = null)
        {
            var query = from q in DbContext.Product
                        join qq in DbContext.ProductCategories on q.ProductCategoryId equals qq.Id into f
                        from pc in f.DefaultIfEmpty()
                        join qqq in DbContext.Categories on pc.CategoryId equals qqq.Id into ff
                        from c in f.DefaultIfEmpty()
                        where q.IsDeleted == null || q.IsDeleted == false
                        select new TempProductsWithCategoriesFlat
                        {
                            ProductId = q.Id,
                            Description = q.Description,
                            ProductCode = q.Code,
                            ProductCategoryCode = q.ProductCategoryId != null ? pc.Code : "",
                            ProductCategoryId = q.ProductCategoryId,
                            ProductCategoryDesc = q.ProductCategoryId != null ? pc.Description : "",
                            CategoryId = c != null ? c.Id : 0,
                            CategoryDesc = c != null ? c.Description : ""
                        };
            if (predicate != null)
                query = query.AsExpandable().Where(predicate);

            return query.AsQueryable();
        }
        public IQueryable<TempProductPricelistsFlat> ProductPricelistsFlat()
        {
            var query = from q in DbContext.Pricelist
                        join qq in DbContext.PricelistDetail on q.Id equals qq.PricelistId
                        select new TempProductPricelistsFlat
                        {
                            PricelistId = q.Id,
                            PricelistCode = q.Code,
                            PricelistDescription = q.Description,
                            ActivationDate = q.ActivationDate,
                            DeactivationDat = q.DeactivationDate,
                            PriceListDetailId = qq.Id,
                            Price = qq.Price,
                            ProductId = qq.ProductId,
                            IngredientId = qq.IngredientId,
                            VatId = qq.VatId,
                            TaxId = qq.TaxId,
                            PriceWithout = qq.PriceWithout
                        };
            return query;
        }

        public IQueryable<TempGuestPaymentsFlat> GuestPaymentsFlat(Expression<Func<TempGuestPaymentsFlat, bool>> predicate = null)
        {
            var query = from q in DbContext.Invoice_Guests_Trans
                        join qq in DbContext.Guest on q.GuestId equals qq.Id
                        select new TempGuestPaymentsFlat
                        {
                            GuestId = qq.Id,
                            Room = qq.Room,
                            RoomId = qq.RoomId,
                            ProfileNo = qq.ProfileNo,
                            ReservationCode = qq.ReservationCode,
                            FirstName = qq.FirstName,
                            LastName = qq.LastName,
                            InvoicesId = q.InvoiceId,
                            TransactionId = q.TransactionId,
                            HotelId = qq.HotelId,
                            ClassId = qq.ClassId,
                            ClassName = qq.ClassName,
                            AvailablePoints = qq.AvailablePoints,
                            fnbdiscount = qq.fnbdiscount,
                            ratebuy = qq.ratebuy
                        };
            if (predicate != null)
                return query.Where(predicate);
            else

                return query.AsQueryable();
        }

        public IQueryable<TempOrderDetail> OrderDetailingredientsTemp(Expression<Func<Invoices, bool>> invoicePredicate,
                                                        Expression<Func<InvoiceTypes, bool>> invoiceTypePredicate,
                                                        Expression<Func<OrderDetail, bool>> orderDetailPredicate,
                                                        Expression<Func<Order, bool>> orderPredicate)
        {
            var query = from irfd in InvoiceRefForDetails(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate)
                        join q in DbContext.OrderDetailIgredients on irfd.OrderDetailId equals q.OrderDetailId
                        join qq in OrderDetailIgredientVatFlat(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate) on q.Id equals qq.Id
                        join qqq in ProductPricelistsFlat() on q.PriceListDetailId equals qqq.PriceListDetailId
                        join qqqq in DbContext.OrderDetail.Where(orderDetailPredicate) on q.OrderDetailId equals qqqq.Id
                        join qqqqq in DbContext.Ingredients on q.IngredientId equals qqqqq.Id
                        select new TempOrderDetail
                        {
                            OrderDetailId = q.OrderDetailId,
                            TotalAfterDiscount = q.TotalAfterDiscount,
                            Net = qq.Net,
                            VatAmount = qq.VatAmount,
                            VatRate = qq.VatRate,
                            VatCode = qq.VatCode,
                            TaxId = qq.TaxId,
                            TaxAmount = qq.TaxAmount,
                            VatId = qq.VatId,
                            ProductId = q.IngredientId,
                            Qty = q.Qty,
                            Discount = q.Discount,
                            PricelistDescription = qqq.PricelistDescription,
                            Price = qqq.Price,
                            PricelistDetailId = qqq.PriceListDetailId,
                            KitchenId = qqqq.KitchenId,
                            KdsId = qqqq.KdsId,
                            PreparationTime = qqqq.PreparationTime,
                            TableId = qqqq.TableId,
                            OrderId = qqqq.OrderId,
                            Description = qqqqq.Description,
                            Code = qqqqq.Code,
                            Status = qqqq.Status,
                            PaidStatus = qqqq.PaidStatus,
                            IsExtra = true,
                            InvoiceTypeType = irfd.InvoiceTypeType,
                            Abbreviation = irfd.Abbreviation,
                            OrderNo = irfd.OrderNo,
                            PosInfoId = irfd.PosInfoId,
                            Guid = irfd.Guid,
                            StaffId = irfd.StaffId
                        };

            return query.AsQueryable();
        }

        public IQueryable<TempOrderDetail> OrderDetailingredientsTempExp(Expression<Func<TempOrderDetail, bool>> predicate = null)
        {
            var query = from q in DbContext.OrderDetailIgredients
                        join qq in OrderDetailIgredientVatFlatExp() on q.Id equals qq.Id
                        join qqq in ProductPricelistsFlat() on q.PriceListDetailId equals qqq.PriceListDetailId
                        join qqqqq in DbContext.Ingredients on q.IngredientId equals qqqqq.Id
                        select new TempOrderDetail
                        {
                            EndOfDayId = null,
                            ReceiptId = 0,
                            OrderDetailId = q.OrderDetailId,
                            TotalAfterDiscount = q.TotalAfterDiscount,
                            Net = qq.Net,
                            VatAmount = qq.VatAmount,
                            VatRate = qq.VatRate,
                            VatCode = qq.VatCode,
                            TaxId = qq.TaxId,
                            TaxAmount = qq.TaxAmount,
                            VatId = qq.VatId,
                            ProductId = q.IngredientId,
                            Qty = q.Qty,
                            Discount = q.Discount,
                            PriceListId = qqq.PricelistId,
                            PricelistDescription = qqq.PricelistDescription,
                            Price = qqq.Price,
                            PricelistDetailId = qqq.PriceListDetailId,
                            KitchenId = 0,
                            KdsId = 0,
                            PreparationTime = 0,
                            TableId = null,
                            OrderId = null,
                            Description = qqqqq.Description,
                            Code = qqqqq.Code,
                            Status = null,
                            PaidStatus = null,
                            IsExtra = true,
                            InvoiceTypeType = null,
                            Abbreviation = null,
                            OrderNo = null,
                            PosInfoId = null,
                            Guid = null,
                            StaffId = null
                        };

            return query.AsQueryable();
        }

        public IQueryable<TempOrderDetail> OrderDetailTemp(Expression<Func<Invoices, bool>> invoicePredicate,
                                                        Expression<Func<InvoiceTypes, bool>> invoiceTypePredicate,
                                                        Expression<Func<OrderDetail, bool>> orderDetailPredicate,
                                                        Expression<Func<Order, bool>> orderPredicate)
        {
            var query = from q in InvoiceRefForDetailsExp()
                        join qq in OrderDetailVatFlat(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate) on q.OrderDetailId equals qq.OrderDetailId
                        //  join od in DbContext.OrderDetail.Where(orderDetailPredicate) on q.OrderDetailId equals od.Id
                        join qqq in ProductPricelistsFlat() on q.PriceListDetailId equals qqq.PriceListDetailId
                        join qqqqq in ProductsWithCategoriesFlat() on q.ProductId equals qqqqq.ProductId

                        select new TempOrderDetail
                        {
                            EndOfDayId = q.EndOfDayId,

                            OrderDetailId = q.OrderDetailId,
                            TotalAfterDiscount = q.TotalAfterDiscount,
                            Net = qq.Net,
                            VatAmount = qq.VatAmount,
                            VatRate = qq.VatRate,
                            VatCode = qq.VatCode,
                            TaxId = qq.TaxId,
                            TaxAmount = qq.TaxAmount,
                            VatId = qq.VatId,
                            ProductId = q.ProductId,
                            Qty = q.Qty,
                            Discount = q.Discount,
                            PricelistDescription = qqq.PricelistDescription,
                            Price = qqq.Price,
                            PricelistDetailId = qqq.PriceListDetailId,
                            //KitchenId = q.KitchenId,
                            //KdsId = q.KdsId,
                            //PreparationTime = qqqq.PreparationTime,
                            TableId = q.TableId,
                            OrderId = q.OrderId,
                            Description = qqqqq.Description,
                            Code = qqqqq.ProductCode,
                            Status = q.Status,
                            PaidStatus = q.PaidStatus,
                            IsExtra = false,
                            InvoiceTypeType = q.InvoiceTypeType,
                            Abbreviation = q.Abbreviation,
                            OrderNo = q.OrderNo,
                            PosInfoId = q.PosInfoId,
                            Guid = q.Guid,
                            StaffId = q.StaffId
                        };

            return query.AsQueryable();
        }


        public IQueryable<TempOrderDetail> OrderDetailTempExp(Expression<Func<TempOrderDetail, bool>> predicate = null)
        {
            var query = from q in InvoiceRefForDetailsExp()
                        join qq in OrderDetailVatFlatExp() on q.OrderDetailId equals qq.OrderDetailId
                        //  join od in DbContext.OrderDetail.Where(orderDetailPredicate) on q.OrderDetailId equals od.Id
                        join qqq in ProductPricelistsFlat() on q.PriceListDetailId equals qqq.PriceListDetailId
                        join qqqqq in ProductsWithCategoriesFlat() on q.ProductId equals qqqqq.ProductId

                        select new TempOrderDetail
                        {
                            EndOfDayId = q.EndOfDayId,
                            ReceiptId = q.InvoicesId,
                            OrderDetailId = q.OrderDetailId,
                            TotalAfterDiscount = q.TotalAfterDiscount,
                            Net = qq.Net,
                            VatAmount = qq.VatAmount,
                            VatRate = qq.VatRate,
                            VatCode = qq.VatCode,
                            TaxId = qq.TaxId,
                            TaxAmount = qq.TaxAmount,
                            VatId = qq.VatId,
                            ProductId = q.ProductId,
                            Qty = q.Qty,
                            Discount = q.Discount,
                            PriceListId = qqq.PricelistId,
                            PricelistDescription = qqq.PricelistDescription,
                            Price = qqq.Price,
                            PricelistDetailId = qqq.PriceListDetailId,
                            KitchenId = 0,
                            KdsId = 0,
                            PreparationTime = 0,
                            TableId = q.TableId,
                            OrderId = q.OrderId,
                            Description = qqqqq.Description,
                            Code = qqqqq.ProductCode,
                            Status = q.Status,
                            PaidStatus = q.PaidStatus,
                            IsExtra = false,
                            InvoiceTypeType = q.InvoiceTypeType,
                            Abbreviation = q.Abbreviation,
                            OrderNo = q.OrderNo,
                            PosInfoId = q.PosInfoId,
                            Guid = q.Guid,
                            StaffId = q.StaffId

                        };
            if (predicate == null)
                return query.AsQueryable();
            else
                return query.Where(predicate);
        }
        public IQueryable<TempOrderDetail> AllDetailsTempExp(Expression<Func<TempOrderDetail, bool>> predicate = null)
        {
            var dets = OrderDetailTempExp();
            var tempIng = from q in dets
                          join qq in OrderDetailingredientsTempExp() on q.OrderDetailId equals qq.OrderDetailId
                          select new TempOrderDetail
                          {
                              EndOfDayId = q.EndOfDayId,
                              ReceiptId = q.ReceiptId,

                              OrderDetailId = q.OrderDetailId,
                              TotalAfterDiscount = qq.TotalAfterDiscount,
                              Net = qq.Net,
                              VatAmount = qq.VatAmount,
                              VatRate = qq.VatRate,
                              VatCode = qq.VatCode,
                              TaxId = qq.TaxId,
                              TaxAmount = qq.TaxAmount,
                              VatId = qq.VatId,
                              ProductId = q.ProductId,
                              Qty = qq.Qty,
                              Discount = qq.Discount,
                              PriceListId = qq.PriceListId,
                              PricelistDescription = qq.PricelistDescription,
                              Price = qq.Price,
                              PricelistDetailId = q.PricelistDetailId,
                              KitchenId = q.KitchenId,
                              KdsId = q.KdsId,
                              PreparationTime = q.PreparationTime,
                              TableId = q.TableId,
                              OrderId = q.OrderId,
                              Description = qq.Description,
                              Code = qq.Code,
                              Status = q.Status,
                              PaidStatus = q.PaidStatus,
                              IsExtra = true,
                              InvoiceTypeType = q.InvoiceTypeType,
                              Abbreviation = q.Abbreviation,
                              OrderNo = q.OrderNo,
                              PosInfoId = q.PosInfoId,
                              Guid = q.Guid,
                              StaffId = q.StaffId
                          };
            var query = dets.Union(tempIng);
            if (predicate == null)
                return query;
            else
                return query.Where(predicate);
        }

        public IQueryable<TempOrderDetail> AllDetailsTemp(Expression<Func<Invoices, bool>> invoicePredicate,
                                                        Expression<Func<InvoiceTypes, bool>> invoiceTypePredicate,
                                                        Expression<Func<OrderDetail, bool>> orderDetailPredicate,
                                                        Expression<Func<Order, bool>> orderPredicate)
        {
            return OrderDetailTemp(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate)
               .Union(OrderDetailingredientsTemp(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate));
        }

        public IQueryable<TempOrderDetailVatFlat> AllDetailsVatsTemp(Expression<Func<Invoices, bool>> invoicePredicate,
                                                      Expression<Func<InvoiceTypes, bool>> invoiceTypePredicate,
                                                      Expression<Func<OrderDetail, bool>> orderDetailPredicate,
                                                      Expression<Func<Order, bool>> orderPredicate)
        {
            return OrderDetailVatFlat(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate)
               .Union(OrderDetailIgredientVatFlat(invoicePredicate, invoiceTypePredicate, orderDetailPredicate, orderPredicate));
        }

        public IQueryable<TempInvoicesFlat> InvoicesFlat(Expression<Func<Invoices, bool>> invoicePredicate,
                                                         Expression<Func<InvoiceTypes, bool>> invoicetypePredicate
                                                          )
        {
            var query = from q in DbContext.Invoices.Where(invoicePredicate)
                        join it in DbContext.InvoiceTypes on q.InvoiceTypeId equals it.Id
                        join pi in DbContext.PosInfo on q.PosInfoId equals pi.Id
                        join qqqq in DbContext.Department on pi.DepartmentId equals qqqq.Id into f
                        from d in f.DefaultIfEmpty()
                        where (q.IsDeleted == null || q.IsDeleted == false)
                        select new TempInvoicesFlat
                        {
                            InvoicesId = q.Id,
                            EndOfDayId = q.EndOfDayId,
                            Abbreviation = it.Abbreviation,
                            Code = it.Code,
                            Type = it.Type,
                            ReceiptNo = q.Counter,
                            PosInfoId = q.PosInfoId,
                            PosInfoDetailId = q.PosInfoDetailId,
                            IsPrinted = q.IsPrinted,
                            IsVoided = q.IsVoided,
                            Total = q.Total,
                            Discount = q.Discount,
                            DiscountRemark = q.DiscountRemark,
                            //OrderDetailId = q.OrderDetailId,
                            PosInfoDescription = pi.Description,
                            DepartmentId = pi.DepartmentId,
                            DepartmentDescription = d != null ? d.Description : "",
                        };
            return query.AsQueryable();
        }
        [Obsolete("Use ReceiptPaymentsFlat instead")]
        public IQueryable<TempReceiptPaymentsFlat> ReceiptPaymentsFlatOld(Expression<Func<Invoices, bool>> invoicePredicate,
                                                                       Expression<Func<Transactions, bool>> transactionPredicate)
        {
            var query = (from q in DbContext.Transactions.Where(transactionPredicate)
                         join i in DbContext.Invoices.Where(invoicePredicate) on q.InvoicesId equals i.Id
                         join qq in DbContext.Accounts on q.AccountId equals qq.Id
                         join qqq in GuestPaymentsFlat() on q.Id equals qqq.TransactionId into f
                         from gst in f.DefaultIfEmpty()
                         join qqqqq in DbContext.EODAccountToPmsTransfer on new { AccountId = q.AccountId, PosInfoId = q.PosInfoId }
                                                           equals new { AccountId = qqqqq.AccountId, PosInfoId = qqqqq.PosInfoId }
                                                           into fff
                         from eodtpt in fff.DefaultIfEmpty()
                         where q.InvoicesId != null && (q.IsDeleted == null || q.IsDeleted == false)
                         select new TempReceiptPaymentsFlat
                         {
                             Id = q.Id,
                             EndOfDayId = q.EndOfDayId ?? 0,
                             PosInfoId = q.InvoicesId,
                             AccountId = q.AccountId,
                             AccountDescription = qq.Description,
                             AccountType = qq.Type,
                             AccountEODRoom = eodtpt != null ? eodtpt.PmsRoom : null,
                             SendsTransfer = qq.SendsTransfer ?? false,
                             Amount = q.Amount,
                             TransactionType = q.TransactionType,
                             GuestId = gst.GuestId != null ? gst.GuestId : 0,
                             Room = gst.Room != null ? gst.Room : "",
                             RoomId = gst.RoomId != null ? gst.RoomId : null,
                             ProfileNo = gst.ProfileNo != null ? gst.ProfileNo : null,
                             ReservationCode = gst.ReservationCode != null ? gst.ReservationCode : "",
                             FirstName = gst.FirstName != null ? gst.FirstName : "",
                             LastName = gst.LastName != null ? gst.LastName : "",
                             InvoicesId = q.InvoicesId ?? 0,
                             TransactionId = gst.TransactionId != null ? gst.TransactionId : null
                         }).Distinct();
            return query.AsQueryable();
        }

        public IQueryable<TempReceiptPaymentsFlat> ReceiptPaymentsFlat(Expression<Func<TempReceiptPaymentsFlat, bool>> predicate = null)
        {
            var query = (from q in DbContext.Transactions
                         join i in DbContext.Invoices on q.InvoicesId equals i.Id
                         join it in DbContext.InvoiceTypes on i.InvoiceTypeId equals it.Id
                         join qq in DbContext.Accounts on q.AccountId equals qq.Id
                         join qqq in GuestPaymentsFlat() on q.Id equals qqq.TransactionId into f
                         from gst in f.DefaultIfEmpty()
                         join qqqqq in DbContext.EODAccountToPmsTransfer on new { AccountId = q.AccountId, PosInfoId = q.PosInfoId }
                                                           equals new { AccountId = qqqqq.AccountId, PosInfoId = qqqqq.PosInfoId }
                                                           into fff
                         from eodtpt in fff.DefaultIfEmpty()
                         where q.InvoicesId != null && (q.IsDeleted == null || q.IsDeleted == false)
                         select new TempReceiptPaymentsFlat
                         {
                             Id = q.Id,
                             EndOfDayId = q.EndOfDayId ?? 0,
                             PosInfoId = q.PosInfoId,
                             AccountId = q.AccountId,
                             AccountDescription = qq.Description,
                             AccountType = qq.Type,
                             AccountEODRoom = eodtpt != null ? eodtpt.PmsRoom : null,
                             SendsTransfer = qq.SendsTransfer ?? false,
                             Amount = q.Amount,
                             TransactionType = q.TransactionType,
                             GuestId = gst.GuestId != null ? gst.GuestId : 0,
                             Room = gst.Room != null ? gst.Room : "",
                             RoomId = gst.RoomId != null ? gst.RoomId : null,
                             ProfileNo = gst.ProfileNo != null ? gst.ProfileNo : null,
                             ReservationCode = gst.ReservationCode != null ? gst.ReservationCode : "",
                             FirstName = gst.FirstName != null ? gst.FirstName : "",
                             LastName = gst.LastName != null ? gst.LastName : "",
                             InvoicesId = q.InvoicesId ?? 0,
                             TransactionId = gst.TransactionId != null ? gst.TransactionId : null,
                             InvoiceType = it.Type,
                             StaffId = q.StaffId,
                             HotelId = q.TransferToPms.FirstOrDefault().HotelId
                         }).Distinct();
            if (predicate == null)
                return query.AsQueryable();
            else
                return query.Where(predicate);
        }


        public IQueryable<TempRelatedInvoices> ReceiptRelatedReceipts(IEnumerable<TempReceiptDetails> rds)
        {
            var ods = rds.Select(s => s.OrderDetailId).AsEnumerable();
            var odisUpdatesQuery = from q in DbContext.OrderDetailInvoices.Where(w => ods.Contains(w.OrderDetailId))// && w.EndOfDayId == null && w.PosInfoId == 3 && w.IsExtra == false)
                                   join qq in DbContext.OrderDetail on q.OrderDetailId equals qq.Id
                                   where (q.IsDeleted ?? false) == false
                                   select new
                                   {
                                       InvoicesId = q.InvoicesId,
                                       Abbreviation = q.Abbreviation,
                                       Counter = q.Counter,
                                       InvoiceType = q.InvoiceType,
                                       PaidStatus = qq.PaidStatus,
                                       OrderDetailId = q.OrderDetailId,
                                       Status = qq.Status,
                                       Total = q.Total,
                                       Discount = q.Discount,
                                       ReceiptSplitedDiscount = q.ReceiptSplitedDiscount
                                   };
            long receiptId = rds.Select(s => s.ReceiptsId).FirstOrDefault() ?? 0;
            var result = odisUpdatesQuery.Where(w => w.InvoicesId != receiptId).ToList().GroupBy(g => g.InvoicesId).Select(s => new TempRelatedInvoices
            {
                InvoicesId = s.Key,
                InvoiceTypeCode = s.FirstOrDefault().InvoiceType,
                IsInvoiced = s.All(a => a.PaidStatus > 0),
                Abbreviation = s.FirstOrDefault().Abbreviation,
                Counter = s.FirstOrDefault().Counter,
                IsVoided = s.All(a => a.Status == 5),
                Details = s.Select(ss => new TempRelatedInvoicesDetails
                {
                    OrderDetailId = ss.OrderDetailId,
                    Total = ss.Total,
                    Discount = ss.Discount,
                    ReceiptSplitedDiscount = ss.ReceiptSplitedDiscount
                }).ToList()

            });
            return result.AsQueryable();

        }

        public IQueryable<TempTableOrdersMin> ReceiptDetailsForTablesBO(Expression<Func<TempTableOrdersMin, bool>> predicate)
        {
            var query = from q in ReceiptDetailsBO(x => x.EndOfDayId == null && x.Status != 5 && ((x.PaidStatus == 0 && x.InvoiceType == 2) || (x.PaidStatus == 1 && x.InvoiceType != 2)))
                        join qqqq in DbContext.Staff on q.StaffId equals qqqq.Id
                       
                        select new TempTableOrdersMin
                        {
                            EndOfDayId = q.EndOfDayId,
                            Status = q.Status,
                            RegionId = q.RegionId,
                            PosInfoId = q.PosInfoId,
                            TableId = q.TableId,
                            StaffFirstName = qqqq.FirstName,
                            PaidStatus = q.PaidStatus,
                            Cover = 0,
                            InvoiceType = q.InvoiceType,
                            StaffId = qqqq.Id,
                            ReceiptId = q.ReceiptsId
                        };

            if (predicate == null)
                return query.Distinct();
            else
                return query.Where(predicate);
        }

        public IQueryable<TempReceiptDetails> ReceiptDetailsBO(Expression<Func<TempReceiptDetails, bool>> predicate = null)
        {
            var query = from q in DbContext.OrderDetailInvoices//.Where(w=>w.ProductId == null)
                        join pr in DbContext.Pricelist on q.PricelistId equals pr.Id
                        join od in DbContext.OrderDetail on q.OrderDetailId equals od.Id
                        join p in DbContext.Product on q.ProductId equals p.Id into pp
                        from ppp in pp.DefaultIfEmpty()
                        join qq in DbContext.Kitchen on od.KitchenId equals qq.Id into f
                        from k in f.DefaultIfEmpty()
                        join stq in DbContext.SalesType on q.SalesTypeId equals stq.Id into fst
                        from slsTp in fst.DefaultIfEmpty()
                        where (q.IsDeleted ?? false) == false
                        select new TempReceiptDetails
                        {
                            EndOfDayId = q.EndOfDayId,
                            ReceiptsId = q.InvoicesId,
                            Abbreviation = q.Abbreviation,
                            ReceiptNo = q.Counter,
                            InvoiceType = (short)q.InvoiceType,
                            OrderDetailId = q.OrderDetailId,
                            ProductId = q.ProductId,
                            ItemCode = q.ItemCode,
                            ItemDescr = q.Description,
                            ExtraDescription = ppp != null ? ppp.ExtraDescription : "",
                            SalesDescription = ppp != null ? ppp.SalesDescription : "",
                            Price = q.Price,
                            ItemDiscount = q.Discount,
                            ItemQty = q.Qty,
                            ItemGross = q.Total,
                            Status = od.Status,
                            StatusTS = od.StatusTS,
                            PaidStatus = od.PaidStatus,
                            KitchenId = od.KitchenId,
                            KdsId = od.KdsId,
                            Guid = od.Guid,
                            TableId = q.TableId,
                            TableCode = q.TableCode,
                            TableLabel = q.TableLabel,
                            RegionId = q.RegionId,
                            OrderNo = q.OrderNo,
                            OrderId = od.OrderId ?? 0,
                            PriceListDetailId = od.PriceListDetailId,
                            PricelistId = q.PricelistId,
                            PricelistDescr = pr.Description,
                            PosInfoId = q.PosInfoId,
                            PosInfoDetailId = q.PosInfoDetailId,
                            ItemVatRate = q.VatRate,
                            VatId = q.VatId.Value,
                            ItemVatValue = q.VatAmount,
                            TaxId = q.TaxId,
                            ItemTaxAmount = q.TaxAmount,
                            ItemNet = q.Net,
                            VatCode = q.VatCode,
                            StaffId = q.StaffId,
                            IsExtra = q.IsExtra,
                            OrderDetailIgredientsId = q.OrderDetailIgredientsId,
                            SalesTypeId = q.SalesTypeId,
                            ProductCategoryId = q.ProductCategoryId,
                            //TODO: Activate After DBUpdate
                            ItemPosition = q.ItemPosition,
                            ItemSort = q.ItemSort,
                            ItemRegion = q.ItemRegion,
                            RegionPosition = q.RegionPosition,
                            ItemBarcode = q.ItemBarcode,
                            KitchenCode = k != null ? k.Code : "",

                            //TODO: Add new Field to the Database
                            TotalBeforeDiscount = q.TotalBeforeDiscount,
                            ItemRemark = q.ItemRemark,
                            SalesTypeDescription = slsTp != null ? slsTp.Description : "",
                            ReceiptSplitedDiscount = q.ReceiptSplitedDiscount
                        };

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query.Distinct().OrderBy(o => o.ReceiptsId).ThenBy(o1 => o1.OrderDetailId).ThenBy(o2 => o2.IsExtra);
        }

        public IQueryable<TempReceiptBOFull> ReceiptsBO(Expression<Func<TempReceiptBOFull, bool>> predicate, ReceiptFilters filters = null)
        {

            IQueryable<TempReceiptBOFull> query;
            try
            {
                if (filters == null || filters.FromDate == null)
                {
                    query = from q in DbContext.Invoices
                            join pi in DbContext.PosInfo on q.PosInfoId equals pi.Id
                            join s in DbContext.Staff on q.StaffId equals s.Id
                            join pid in DbContext.PosInfoDetail on q.PosInfoDetailId equals pid.Id
                            join t in DbContext.Table on q.TableId equals t.Id into f
                            from tbl in f.DefaultIfEmpty()
                            join shdet in DbContext.InvoiceShippingDetails on q.Id equals shdet.InvoicesId into ff
                            from sd in ff.DefaultIfEmpty()
                            join it in DbContext.InvoiceTypes on q.InvoiceTypeId equals it.Id
                            where (q.IsDeleted == null || q.IsDeleted == false)
                            select new TempReceiptBOFull
                            {
                                Id = q.Id,
                                EndOfDayId = q.EndOfDayId ?? 0,
                                Day = q.Day,
                                OrderNo = q.OrderNo,
                                //Description = q.Description,
                                DepartmentId = pi.DepartmentId,
                                ReceiptNo = q.Counter,
                                Cover = q.Cover,
                                Discount = q.Discount,
                                Total = q.Total,
                                InvoiceTypeId = q.InvoiceTypeId,
                                InvoiceIndex = pid.InvoiceId,
                                IsPrinted = q.IsPrinted ?? false,
                                IsVoided = q.IsVoided ?? false,
                                IsPaid = q.IsPaid,
                                IsInvoiced = q.IsInvoiced,
                                TableId = q.TableId,
                                TableCode = q.TableId != null ? tbl.Code : "",
                                PaidTotal = q.PaidTotal,
                                PaymentsDesc = q.PaymentsDesc,
                                Room = q.Rooms,
                                StaffId = q.StaffId,
                                PosInfoId = q.PosInfoId,
                                PosInfoDetailId = q.PosInfoDetailId,
                                StaffCode = s.Code,
                                StaffName = s.FirstName,
                                StaffLastName = s.LastName,
                                StaffFullName = "" + s.FirstName + " " + s.LastName + "",
                                InvoiceTypeType = it.Type,
                                Abbreviation = it.Abbreviation,
                                ShippingAddress = sd.ShippingAddress,
                                ShippingAddressId = sd.ShippingAddressId,
                                ShippingCity = sd.ShippingCity,
                                ShippingZipCode = sd.ShippingZipCode,
                                BillingAddress = sd.BillingAddress,
                                BillingAddressId = sd.BillingAddressId,
                                BillingCity = sd.BillingCity,
                                BillingZipCode = sd.BillingZipCode,
                                BillingName = sd.BillingName,
                                BillingVatNo = sd.BillingVatNo,
                                BillingDOY = sd.BillingDOY,
                                BillingJob = sd.BillingJob,

                                CustomerRemarks = sd.CustomerRemarks,
                                CustomerName = sd.CustomerName,
                                CustomerID = sd.CustomerID ?? -1,
                                Floor = sd.Floor,
                                Latitude = sd.Latitude,
                                Longtitude = sd.Longtitude,
                                Phone = sd.Phone,
                                StoreRemarks = sd.StoreRemarks,
                                TableSum = q.TableSum,
                                CashAmount = q.CashAmount,
                                BuzzerNumber = q.BuzzerNumber,
                                Vat = q.Vat,
                                Net = q.Net,
                                PdaModuleId = q.PdaModuleId,
                                ExtECRCode = q.ExtECRCode
                            };
                }
                else
                {
                    query = (from q in DbContext.Invoices.Where(w => ((filters.FromDate != null) ? EntityFunctions.TruncateTime(w.Day) == EntityFunctions.TruncateTime(filters.FromDate) : true) &&
                                                                    (filters.PosInfoId != null ? w.PosInfoId == filters.PosInfoId : true) &&
                                                                    ////(filters.PosList != null && filters.PosList.Count > 0 ? filters.PosList.Contains(w.PosInfoId) : true) &&
                                                                    ////(((filters.PosList != null && filters.PosList.Count > 0) ? filters.PosList.Contains(w.PosInfoId) : true)) &&
                                                                    (filters.Room != null ? filters.Room == w.Rooms : true) &&
                                                                    (filters.EodId != null ? filters.EodId == w.EndOfDayId : true) &&
                                                                    (filters.IsInvoiced == null ? true : filters.IsInvoiced != w.IsInvoiced) &&
                                                                    ////(!string.IsNullOrEmpty(filters.OrderNo) ? filters.OrderNo.Contains(w.OrderNo) : true) &&
                                                                    (!string.IsNullOrEmpty(filters.OrderNo) ? w.OrderNo.IndexOf(filters.OrderNo ?? "") > 0 : true) &&
                                                                    (filters.IsPrinted == null || filters.IsPrinted == false ? true : filters.IsPrinted == w.IsPrinted) &&
                                                                    (filters.IsPaid != null ? (filters.IsPaid == true ? w.IsPaid == 2 : w.IsPaid < 1) : true) &&
                                                                    (filters.ReceiptNo != null ? filters.ReceiptNo == w.Counter : true)
                                                                    )
                             join pi in DbContext.PosInfo on q.PosInfoId equals pi.Id
                             join s in DbContext.Staff on q.StaffId equals s.Id
                             join pid in DbContext.PosInfoDetail on q.PosInfoDetailId equals pid.Id
                             join t in DbContext.Table.Where(y => filters.TableCode != null ? filters.TableCode == y.Code : true) on q.TableId equals t.Id into f
                             from tbl in f.DefaultIfEmpty()
                             join shdet in DbContext.InvoiceShippingDetails on q.Id equals shdet.InvoicesId into ff
                             from sd in ff.DefaultIfEmpty()
                             join it in DbContext.InvoiceTypes//.Where(z => (filters.InvoiceTypeList != null && filters.InvoiceTypeList.Count() > 0 ? filters.InvoiceTypeList.Contains(z.Type.Value) : true)) 
                                                                          on q.InvoiceTypeId.Value equals it.Id
                             where (q.IsDeleted == null || q.IsDeleted == false)
                             select new TempReceiptBOFull
                             {
                                 Id = q.Id,
                                 EndOfDayId = q.EndOfDayId ?? 0,
                                 Day = q.Day ?? DateTime.Now,
                                 OrderNo = q.OrderNo ?? "",
                                 //FODay = pi.FODay,
                                 //Description = q.Description,
                                 DepartmentId = pi.DepartmentId ?? -1,
                                 ReceiptNo = q.Counter ?? -1,
                                 Cover = q.Cover ?? -1,
                                 Discount = q.Discount ?? 0,
                                 Total = q.Total ?? 0,
                                 InvoiceTypeId = q.InvoiceTypeId ?? -1,
                                 Counter = q.Counter ?? -1,
                                 InvoiceIndex = pid.InvoiceId ?? -1,
                                 IsPrinted = q.IsPrinted ?? false,
                                 IsVoided = q.IsVoided ?? false,
                                 IsPaid = q.IsPaid,
                                 IsInvoiced = q.IsInvoiced,
                                 TableId = q.TableId ?? -1,
                                 TableCode = q.TableId != null ? tbl.Code : "",
                                 PaidTotal = q.PaidTotal ?? -1,
                                 PaymentsDesc = q.PaymentsDesc ?? "",
                                 Room = q.Rooms ?? "",
                                 StaffId = q.StaffId ?? -1,
                                 PosInfoId = q.PosInfoId ?? -1,
                                 PosInfoDetailId = q.PosInfoDetailId ?? -1,
                                 StaffCode = s.Code ?? "",
                                 StaffName = s.FirstName ?? "",
                                 InvoiceTypeType = it.Type ?? -1,
                                 Abbreviation = it.Abbreviation ?? "",
                                 ShippingAddress = sd.ShippingAddress ?? "",
                                 ShippingAddressId = sd.ShippingAddressId ?? -1,
                                 ShippingCity = sd.ShippingCity ?? "",
                                 ShippingZipCode = sd.ShippingZipCode ?? "",
                                 BillingAddress = sd.BillingAddress ?? "",
                                 BillingAddressId = sd.BillingAddressId ?? -1,
                                 BillingCity = sd.BillingCity ?? "",
                                 BillingZipCode = sd.BillingZipCode ?? "",
                                 BillingName = sd.BillingName ?? "",
                                 BillingVatNo = sd.BillingVatNo ?? "",
                                 BillingDOY = sd.BillingDOY ?? "",
                                 BillingJob = sd.BillingJob ?? "",

                                 CustomerRemarks = sd.CustomerRemarks ?? "",
                                 CustomerName = sd.CustomerName ?? "",
                                 CustomerID = sd.CustomerID ?? -1,
                                 Floor = sd.Floor ?? "",
                                 Latitude = sd.Latitude ?? 0,
                                 Longtitude = sd.Longtitude ?? 0,
                                 Phone = sd.Phone ?? "",
                                 StoreRemarks = sd.StoreRemarks ?? "",
                                 TableSum = q.TableSum ?? 0,
                                 CashAmount = q.CashAmount ?? "0",
                                 BuzzerNumber = q.BuzzerNumber ?? "",
                                 Vat = q.Vat,
                                 Net = q.Net,
                                 PdaModuleId = q.PdaModuleId,
                                 ExtECRCode = q.ExtECRCode
                             });

                    if (filters.InvoiceTypeList != null && filters.InvoiceTypeList.Count() > 0)
                        query = query.Where(w => filters.InvoiceTypeList.Contains(w.InvoiceTypeType));
                }

                if (predicate != null)
                {
                    query = query.AsExpandable().Where(predicate);
                }
                return query;

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return null;
            }

        }

        public IQueryable<TableCovers> OrderCovers(Expression<Func<Order, bool>> orderPredicate)
        {
            var query = from o in DbContext.Order.Where(orderPredicate)
                        select new TableCovers
                        {
                            OrderId = o.Id,
                            OrderNo = o.OrderNo,
                            Cover = o.Couver
                        };

            return query.AsQueryable();
        }

        public IQueryable<TableCounter> OrderCounter(Expression<Func<Invoices, bool>> InvoicesPredicate)
        {
            var query = from i in DbContext.Invoices.Where(InvoicesPredicate)
                        select new TableCounter
                        {
                            Counter = i.Counter
                        };

            return query.AsQueryable();
        }

        public IQueryable<TempDeliveryCustomers> GetDeliveryCustomersForSelectedInvoices(IEnumerable<long?> invoiceIds)
        {
            var query = from isd in DbContext.InvoiceShippingDetails.Where(x => invoiceIds.Contains(x.InvoicesId))
                        join dc in DbContext.Delivery_Customers on isd.CustomerID equals dc.ID
                        //select dc;
                        select new TempDeliveryCustomers
                        {
                            Id = dc.ID,
                            FirstName = dc.FirstName,
                            LastName = dc.LastName,
                            Comments = dc.Comments,
                            StoreRemark = isd.StoreRemarks,
                            BillingName = dc.BillingName,
                            BillingVatNo = dc.BillingVatNo,
                            BillingDOY = dc.BillingDOY,
                            BillingJob = dc.BillingJob,
                            CustomerType = dc.CustomerType,
                            DOY = dc.DOY,
                            Floor = dc.Floor,
                            ShippingAddress = isd.ShippingAddress,
                            selectedBillingAddresses = new TempDeliveryCustomersBillingAddress
                            {
                                AddressStreet = isd.BillingAddress,
                                AddressNo = "",
                                ID = isd.BillingAddressId,
                                City = isd.BillingCity,
                                ZipCode = isd.BillingZipCode
                            },
                            selectedShippingAddresses = new TempDeliveryCustomersShippingAddress
                            {
                                AddressStreet = isd.ShippingAddress,
                                AddressNo = "",
                                ID = isd.ShippingAddressId,
                                City = isd.ShippingCity,
                                ZipCode = isd.ShippingZipCode,
                                Floor = isd.Floor,
                                Latitude = isd.Latitude,
                                Longtitude = isd.Longtitude
                            },
                            selectedPhone = new TempDeliveryCustomersPhone
                            {
                                PhoneNumber = isd.Phone
                            }

                        };

            return query.AsQueryable();
        }

        public IQueryable<OrderExternalInfo> GetOrderExternalInfo(IEnumerable<long> orderIds)
        {
            var query = from o in DbContext.Order.Where(x => orderIds.Contains(x.Id) && x.ExtType != null)
                        select new OrderExternalInfo
                        {
                            OrderId = o.Id,
                            ExtKey = o.ExtKey,
                            ExtType = o.ExtType,
                            ExtObj = o.ExtObj
                        };
            return query.AsQueryable();
        }

        public void Dispose()
        {

        }
    }


    #region Extract To models Directory When done

    public class TempInvoiceRooms
    {
        public long? ReceiptsId { get; set; }
        public string Rooms { get; set; }
        public string AccountDescriptions { get; set; }
        public decimal? Total { get; set; }
    }


    public class TempOrderDetailVatFlat
    {
        public long? ReceiptsId { get; set; }
        public long? Id { get; set; }
        public long? OrderDetailId { get; set; }
        public decimal? Gross { get; set; }
        public decimal? Net { get; set; }
        public decimal? VatRate { get; set; }
        public long? VatId { get; set; }
        public decimal? VatAmount { get; set; }
        public long? VatCode { get; set; }
        public decimal? CurrentVatPercentage { get; set; }
        public long? TaxId { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TaxRate { get; set; }
    }


    public class TempProductsWithCategoriesFlat
    {
        public long? ProductId { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategoryCode { get; set; }
        public long? ProductCategoryId { get; set; }
        public string ProductCategoryDesc { get; set; }
        public long? CategoryId { get; set; }
        public string CategoryDesc { get; set; }
    }
    public class TempProductPricelistsFlat
    {
        public long? PricelistId { get; set; }
        public string PricelistCode { get; set; }
        public string PricelistDescription { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? DeactivationDat { get; set; }
        public long? PriceListDetailId { get; set; }
        public decimal? Price { get; set; }
        public long? ProductId { get; set; }
        public long? IngredientId { get; set; }
        public long? VatId { get; set; }
        public long? TaxId { get; set; }
        public decimal? PriceWithout { get; set; }
    }
    public class TempOrderDetail
    {
        public long? ReceiptId { get; set; }
        public long? EndOfDayId { get; set; }
        public long? OrderDetailId { get; set; }
        public decimal? TotalAfterDiscount { get; set; }
        public decimal? Net { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatRate { get; set; }
        public long? VatCode { get; set; }
        public long? TaxId { get; set; }
        public decimal? TaxAmount { get; set; }
        public long? VatId { get; set; }
        public long? ProductId { get; set; }
        public double? Qty { get; set; }
        public decimal? Discount { get; set; }
        public string PricelistDescription { get; set; }
        public decimal? Price { get; set; }
        public long? PricelistDetailId { get; set; }
        public long? KitchenId { get; set; }
        public long? KdsId { get; set; }
        public int? PreparationTime { get; set; }
        public long? TableId { get; set; }
        public long? OrderId { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public byte? Status { get; set; }
        public byte? PaidStatus { get; set; }
        public bool IsExtra { get; set; }
        public short? InvoiceTypeType { get; set; }
        public string Abbreviation { get; set; }
        public long? OrderNo { get; set; }
        public long? PosInfoId { get; set; }
        public long? StaffId { get; set; }
        public string StaffName { get; set; }
        public long? PriceListId { get; set; }
        public Guid? Guid { get; set; }


    }
    public class TempInvoicesFlat
    {
        public long? InvoicesId { get; set; }
        public long? EndOfDayId { get; set; }
        public string Abbreviation { get; set; }
        public string Code { get; set; }
        public short? Type { get; set; }
        public long? ReceiptNo { get; set; }
        public long? PosInfoId { get; set; }
        public long? PosInfoDetailId { get; set; }
        public bool? IsPrinted { get; set; }
        public bool? IsVoided { get; set; }
        public decimal? Total { get; set; }
        public decimal? Discount { get; set; }
        public string DiscountRemark { get; set; }
        public long? OrderDetailId { get; set; }

        public string PosInfoDescription { get; set; }
        public long? DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }
    }
    public class TempReceiptDetails
    {
        //public System.Guid Id { get; set; }
        public Nullable<long> ReceiptsId { get; set; }
        public long? EndOfDayId { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<long> StaffId { get; set; }
        public string Abbreviation { get; set; }
        public long? ReceiptNo { get; set; }
        public Nullable<short> InvoiceType { get; set; }
        public Nullable<long> OrderDetailId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescr { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<double> ItemQty { get; set; }
        public Nullable<decimal> ItemGross { get; set; }
        public Nullable<decimal> ItemDiscount { get; set; }
        public Nullable<decimal> ItemVatRate { get; set; }
        public Nullable<decimal> ItemVatValue { get; set; }
        public Nullable<long> TaxId { get; set; }
        public Nullable<decimal> ItemTaxAmount { get; set; }
        public Nullable<decimal> ItemNet { get; set; }
        public Nullable<long> VatCode { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<DateTime> StatusTS { get; set; }
        public Nullable<byte> PaidStatus { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<long> KdsId { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public bool IsExtra { get; set; }
        public string TableCode { get; set; }
        public Nullable<long> TableId { get; set; }
        public string TableLabel { get; set; }
        public Nullable<long> OrderNo { get; set; }
        public long OrderId { get; set; }
        public Nullable<long> PosInfoDetailId { get; set; }
        public Nullable<long> PriceListDetailId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public long? VatId { get; set; }
        public Nullable<long> PricelistId { get; set; }
        public string PricelistDescr { get; set; }
        public Nullable<long> RegionId { get; set; }
        public string ItemRemark { get; set; }
        public long? OrderDetailIgredientsId { get; set; }
        public bool IsInvoiced { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public Int32 ItemPosition { get; set; }
        public Int32 ItemSort { get; set; }
        public String ItemRegion { get; set; }
        public int? RegionPosition { get; set; }
        public Int32? ItemBarcode { get; set; }
        public string KitchenCode { get; set; }
        public Nullable<decimal> TotalBeforeDiscount { get; set; }
        public string SalesTypeDescription { get; set; }
        public decimal? ReceiptSplitedDiscount { get; set; }
        public string ExtraDescription { get; set; }
        public string SalesDescription { get; set; }
    }
    public class TempGuestPaymentsFlat
    {
        public long? GuestId { get; set; }
        public string Room { get; set; }
        public int? RoomId { get; set; }
        public int? ProfileNo { get; set; }
        public string ReservationCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? InvoicesId { get; set; }
        public long? TransactionId { get; set; }
        public long? HotelId { get; set; }

        //Απαραίτητη πληροφορία loyalty του πελάτη
        public Nullable<int> ClassId { get; set; }
        public string ClassName { get; set; }
        public Nullable<int> AvailablePoints { get; set; }
        public Nullable<int> fnbdiscount { get; set; }
        public Nullable<int> ratebuy { get; set; }
    }

    public class TempReceiptPaymentsFlat : TempGuestPaymentsFlat
    {
        public long? Id { get; set; }
        public long? EndOfDayId { get; set; }
        public long? PosInfoId { get; set; }
        public long? AccountId { get; set; }
        public string AccountDescription { get; set; }
        public short? AccountType { get; set; }
        public long? AccountEODRoom { get; set; }
        public bool SendsTransfer { get; set; }
        public decimal? Amount { get; set; }
        public short? TransactionType { get; set; }
        public short? InvoiceType { get; set; }
        public long? StaffId { get; set; }
        public long? CreditAccountId { get; set; }
        public long? CreditCodeId { get; set; }
        public string CreditAccountDescription { get; set; }
        public long? HotelId { get; set; }
    }
    public class TempReceiptBOFull
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> Day { get; set; }
        public string Abbreviation { get; set; }
        public string InvoiceDescription { get; set; }
        public Nullable<int> ReceiptNo { get; set; }
        public Nullable<long> InvoiceTypeId { get; set; }
        public Nullable<int> Counter { get; set; }
        public long? EndOfDayId { get; set; }
        public Nullable<System.DateTime> FODay { get; set; }
        public Nullable<short> InvoiceTypeType { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public string PosInfoDescription { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }
        public Nullable<int> Cover { get; set; }
        public Nullable<long> TableId { get; set; }
        public string TableCode { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Net { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<long> StaffId { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public bool IsVoided { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsInvoiced { get; set; }
        public Nullable<long> PosInfoDetailId { get; set; }
        public Nullable<long> ClientPosId { get; set; }
        public Nullable<long> PdaModuleId { get; set; }
        public string DiscountRemark { get; set; }
        public string BillingAddress { get; set; }
        public Nullable<long> BillingAddressId { get; set; }
        public string BillingCity { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingName { get; set; }
        public string BillingVatNo { get; set; }
        public string BillingDOY { get; set; }
        public string BillingJob { get; set; }
        public string CustomerName { get; set; }
        public long CustomerID { get; set; }
        public string CustomerRemarks { get; set; }
        public string Floor { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longtitude { get; set; }
        public string Phone { get; set; }
        public string ShippingAddress { get; set; }
        public Nullable<long> ShippingAddressId { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingZipCode { get; set; }
        public string StoreRemarks { get; set; }
        public string OrderNo { get; set; }
        public string Room { get; set; }
        public Nullable<short> InvoiceIndex { get; set; }
        public short ModifyOrderDetails { get; set; }

        public decimal? Vat1 { get; set; }
        public decimal? Vat2 { get; set; }
        public decimal? Vat3 { get; set; }
        public decimal? Vat4 { get; set; }
        public decimal? Vat5 { get; set; }
        public string PaymentsDesc { get; set; }
        //        public short IsPaid { get { return this.PaidTotal != 0 ? (this.Total ?? 0 - this.PaidTotal ?? 0) < (decimal)0.50 ? (short)2 : (short)1 : (short)0; } }
        public short IsPaid { get; set; }
        public decimal? PaidTotal { get; set; }
        public string StaffLastName { get; set; }
        public string StaffFullName { get; set; }

        public IEnumerable<TempReceiptDetails> ReceiptDetails { get; set; }
        public IEnumerable<TempReceiptPaymentsFlat> ReceiptPayments { get; set; }
        public decimal? TableSum { get; set; }
        public string CashAmount { get; set; }
        public string BuzzerNumber { get; set; }
        public string ExtECRCode { get; set; }
    }


    public class TempRelatedInvoices
    {
        public TempRelatedInvoices()
        {
            Details = new HashSet<TempRelatedInvoicesDetails>();
        }
        public long? InvoicesId { get; set; }
        public int? InvoiceTypeCode { get; set; }
        public bool IsInvoiced { get; set; }
        public string Abbreviation { get; set; }
        public long? Counter { get; set; }
        public bool IsVoided { get; set; }
        public virtual ICollection<TempRelatedInvoicesDetails> Details { get; set; }
    }
    public class TempRelatedInvoicesDetails
    {
        public long? OrderDetailId { get; set; }
        public decimal? Total { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ReceiptSplitedDiscount { get; set; }
    }

    public class TempSumms
    {
        public long? ReceiptsId { get; set; }
        public decimal? Total { get; set; }
    }

    public class TempInvoiceRefForDetails
    {
        public long? InvoicesId { get; set; }
        public long? EndOfDayId { get; set; }
        public long? InvoiceTypesId { get; set; }
        public string InvoiceTypeCode { get; set; }
        public long? PosInfoId { get; set; }
        public short? InvoiceTypeType { get; set; }
        public long? OrderDetailId { get; set; }
        public long? PosInfoDetailId { get; set; }
        public string Abbreviation { get; set; }
        public long? OrderNo { get; set; }
        public int? ReceiptNo { get; set; }
        public Guid? Guid { get; set; }
        public long? StaffId { get; set; }
        public byte? PaidStatus { get; set; }
        public long? TableId { get; set; }
        public int? Cover { get; set; }
        public long? PriceListDetailId { get; set; }
        public decimal? TotalAfterDiscount { get; set; }
        public decimal? Discount { get; set; }
        public long? ProductId { get; set; }
        public double? Qty { get; set; }
        public long? OrderId { get; set; }
        public byte? Status { get; set; }
    }

    public class TempTableOrdersMin
    {
        public long? ReceiptId { get; set; }
        public long? RegionId { get; set; }
        public long? PosInfoId { get; set; }
        public long? TableId { get; set; }
        public string TableCode { get; set; }
        public long? OrderNo { get; set; }
        public double? Qty { get; set; }
        public decimal? Total { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffLastName { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get { return this.StaffLastName ?? "" + " " + this.StaffFirstName ?? ""; } }
        public byte? PaidStatus { get; set; }
        public int? Cover { get; set; }
        public long? EndOfDayId { get; set; }
        public byte? Status { get; set; }
        public short? InvoiceType { get; set; }
        public long? StaffId { get; set; }
    }

    public class TableCovers
    {
        public long? OrderId { get; set; }
        public long? OrderNo { get; set; }
        public int? Cover { get; set; }
    }

    public class TableCounter
    {
        public int? Counter { get; set; }
    }

    public class TempDeliveryCustomers
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Comments { get; set; }
        public string StoreRemark { get; set; }
        public string BillingName { get; set; }
        public string BillingVatNo { get; set; }
        public string BillingDOY { get; set; }
        public string BillingJob { get; set; }
        public TempDeliveryCustomersBillingAddress selectedBillingAddresses { get; set; }
        public TempDeliveryCustomersShippingAddress selectedShippingAddresses { get; set; }
        public TempDeliveryCustomersPhone selectedPhone { get; set; }
        public int? CustomerType { get; set; }
        public string DOY { get; set; }
        public string Floor { get; set; }
        public string ShippingAddress { get; set; }

    }

    public class TempDeliveryCustomersBillingAddress
    {
        public string AddressStreet { get; set; }
        public string AddressNo { get; set; }
        public long? ID { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

    }

    public class TempDeliveryCustomersShippingAddress
    {
        public string AddressStreet { get; set; }
        public string AddressNo { get; set; }
        public long? ID { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Floor { get; set; }
        public double? Latitude { get; set; }
        public double? Longtitude { get; set; }
    }

    public class TempDeliveryCustomersPhone
    {
        public string PhoneNumber { get; set; }
    }

    public class OrderExternalInfo
    {
        public long OrderId { get; set; }
        public int? ExtType { get; set; }
        public string ExtObj { get; set; }
        public string ExtKey { get; set; }
    }

    #endregion
}