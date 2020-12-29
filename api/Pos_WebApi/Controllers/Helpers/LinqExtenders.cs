using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Helpers
{
    public static class LinqExtenders
    {
        public static IQueryable<InvoiceTypes> SalesInvoiceTypes(this IQueryable<InvoiceTypes> source)
        {
            return source.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void);
        }
        public static IQueryable<InvoiceTypes> OrdersInvoiceTypes(this IQueryable<InvoiceTypes> source)
        {
            return source.Where(w => w.Type == (int)InvoiceTypesEnum.Order);
        }

        public static IQueryable<InvoiceTypes> AllowedsInvoiceTypes(this IQueryable<InvoiceTypes> source)
        {
            return source.Where(w => w.Type == (int)InvoiceTypesEnum.Allowance);
        }

        public static IQueryable<InvoiceTypes> ComplimentaryInvoiceTypes(this IQueryable<InvoiceTypes> source)
        {
            return source.Where(w => w.Type == (int)InvoiceTypesEnum.Coplimentary);
        }


        public static IQueryable<Invoices> GetDeleted(this IQueryable<Invoices> source)
        {
            return source.Where(w => w.IsDeleted == true);
        }

        public static IQueryable<Invoices> GetNotAudited(this IQueryable<Invoices> source)
        {
            return source.Where(w => (w.IsDeleted == false || w.IsDeleted == null) && w.EndOfDayId == null);
        }
        public static IQueryable<Invoices> GetAudited(this IQueryable<Invoices> source, IQueryable<EndOfDay> eod)
        {
            return from q in source.Where(w => w.IsDeleted == false || w.IsDeleted == null)
                   join qq in eod on q.EndOfDayId equals qq.Id
                   select q;
        }

        public static IQueryable<Invoices> GetInvoiceOnly(this IQueryable<Invoices> source, IQueryable<InvoiceTypes> validInvoiceTypes)
        {
            return from q in source
                   join qq in validInvoiceTypes on q.InvoiceTypeId equals qq.Id
                   select q;

        }

        public static IQueryable<Invoices> GetInvoiceOrders(this IQueryable<Invoices> source)
        {
            return source.Where(w => w.InvoiceTypeId == (int)InvoiceTypesEnum.Order);
        }

        public static IQueryable<TransactionsWithAccounts> GetTransactionsWithAccounts(this IQueryable<Invoices> source, IQueryable<Accounts> accounts)
        {
            var query = source.SelectMany(s => s.Transactions);
            var query1 = accounts;// source.SelectMany(s => s.Transactions).Select(s => s.Accounts);
            var query2 = source.SelectMany(s => s.Transactions).SelectMany(s => s.Invoice_Guests_Trans);
            var query3 = source.SelectMany(s => s.Transactions).SelectMany(s => s.Invoice_Guests_Trans).Select(s => s.Guest);

            var final = from q in query
                        let l = source.Where(w => w.Id == q.InvoicesId)
                        from i in l
                        let l1 = query1.Where(w => w.Id == q.AccountId)
                        from a in l1
                        select new TransactionsWithAccounts
                        {
                            Id = q.Id,
                            AccountId = q.AccountId ?? 0,
                            Amount = q.Amount ?? 0,
                            Description = a.Description,
                            AccountType = a.Type ?? 0,
                            InvoiceId = i.Id,

                        };
            return final;
        }

        public static IQueryable<InvoiceForDisplayDetailsTemp> GetInvoiceDetailsForDisplay(this IQueryable<Invoices> source)
        {
            var query = source.SelectMany(s => s.OrderDetailInvoices).Select(s => s.OrderDetail).Where(w => w.IsDeleted == false || w.IsDeleted == null);
            var query1 = source.SelectMany(s => s.OrderDetailInvoices).Where(w => w.IsDeleted == false || w.IsDeleted == null);
            var query2 = source.SelectMany(s => s.OrderDetailInvoices).Select(s => s.OrderDetail).SelectMany(s => s.OrderDetailVatAnal).Where(w => w.IsDeleted == false || w.IsDeleted == null);

            var final = (from q in query1
                         join qq in query on q.OrderDetailId equals qq.Id
                         join qqq in query2 on q.OrderDetailId equals qqq.OrderDetailId
                         select new InvoiceForDisplayDetailsTemp
                         {
                             Id = qq.Guid.Value,
                             Discount = qq.Discount ?? 0,
                             OrderDetailId = qq.Id,
                             InvoiceForDetailId = q.InvoicesId.Value,
                             TableId = qq.TableId,
                             PaidStatus = qq.PaidStatus ?? 0,
                             Status = qq.Status ?? 0,
                             Price = qq.Price ?? 0,
                             Qty = (qq.Qty ?? 0.0),
                             TotalAfterDiscount = qq.TotalAfterDiscount ?? 0,
                             VatId = qqq.VatId.Value,
                             VatAmount = qqq.VatAmount ?? 0,
                             VatRate = qqq.VatRate ?? 0,
                             TaxAmount = qqq.TaxAmount ?? 0,
                             TaxId = qqq.TaxId ?? 0,
                             ProductId = qq.ProductId ?? 0,
                             IsExtra = false,

                         }).Distinct();
            return final;
        }
        public static IQueryable<InvoiceForDisplayDetailsTemp> GetInvoiceDetailsForDisplayWithLet(this IQueryable<Invoices> source)
        {
            var query = source.SelectMany(s => s.OrderDetailInvoices).Select(s => s.OrderDetail);
            var query1 = source.SelectMany(s => s.OrderDetailInvoices);
            var query2 = source.SelectMany(s => s.OrderDetailInvoices).Select(s => s.OrderDetail).SelectMany(s => s.OrderDetailVatAnal);

            var final = (from q in query1
                         let l = query.Where(w => w.Id == q.OrderDetailId)
                         from qq in l
                         let l1 = query2.Where(w => w.OrderDetailId == q.OrderDetailId)
                         from qqq in l1
                         select new InvoiceForDisplayDetailsTemp
                         {
                             Id = qq.Guid.Value,
                             Discount = qq.Discount,
                             OrderDetailId = qq.Id,
                             InvoiceForDetailId = q.InvoicesId.Value,
                             TableId = qq.TableId,
                             PaidStatus = qq.PaidStatus,
                             Status = qq.Status,
                             Price = qq.Price,
                             Qty = (qq.Qty ?? 0.0),
                             TotalAfterDiscount = qq.TotalAfterDiscount,
                             VatId = qqq.VatId.Value,
                             VatAmount = qqq.VatAmount,
                             VatRate = qqq.VatRate,
                             TaxAmount = qqq.TaxAmount,
                             TaxId = qqq.TaxId,
                             ProductId = qq.ProductId,
                             IsExtra = false,

                         }).Distinct();
            var query3 = source.SelectMany(s => s.OrderDetailInvoices).Select(s => s.OrderDetail).SelectMany(s => s.OrderDetailIgredients);
            var query4 = source.SelectMany(s => s.OrderDetailInvoices).Select(s => s.OrderDetail).SelectMany(s => s.OrderDetailIgredients).SelectMany(s => s.OrderDetailIgredientVatAnal);

            var final1 = (from q in query3
                          let l = query.Where(w => w.Id == q.OrderDetailId)
                          from qq in l
                          let l1 = query4.Where(w => w.OrderDetailIgredientsId == q.Id)
                          from qqq in l1
                          let l2 = query1.Where(w => w.OrderDetailId == q.OrderDetailId)
                          from qqqq in l2
                          select new InvoiceForDisplayDetailsTemp
                          {
                              Id = qq.Guid.Value,
                              Discount = q.Discount,
                              OrderDetailId = qq.Id,
                              InvoiceForDetailId = qqqq.InvoicesId.Value,
                              TableId = qq.TableId,
                              PaidStatus = qq.PaidStatus,
                              Status = qq.Status,
                              Price = q.Price,
                              Qty = q.Qty ,
                              TotalAfterDiscount = q.TotalAfterDiscount,
                              VatId = qqq.VatId.Value,
                              VatAmount = qqq.VatAmount,
                              VatRate = qqq.VatRate,
                              TaxAmount = qqq.TaxAmount,
                              TaxId = qqq.TaxId,
                              ProductId = qq.ProductId,
                              IsExtra = true,

                          }).Distinct();
            return final.Union(final1);

        }

    }

}
