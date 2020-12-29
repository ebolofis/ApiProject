using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using Pos_WebApi.Controllers;
using Newtonsoft.Json;
using System.Data.Entity.Core.Objects;

namespace Pos_WebApi.Helpers
{
    public class ReportsHelper
    {
        public static dynamic GetWaiterReport(string filters, PosEntities db)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var piid = flts.PosList.FirstOrDefault();
            var transactions = db.Transactions.Include(i => i.Order).Include(i => i.Accounts).Include(e => e.OrderDetail.Select(f => f.OrderDetailInvoices))
                .Where(w => w.PosInfoId == piid && w.EndOfDayId == null && ((w.IsDeleted ?? false) == false)).AsNoTracking().AsEnumerable();
            var invoices = db.Invoices.Include("Transactions").Include("InvoiceTypes").Where(w => w.EndOfDayId == null && w.PosInfoId == piid && (w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Coplimentary)
                && (w.IsVoided ?? false) == false && (w.IsDeleted ?? false) == false && w.Transactions.Count == 0);

            var group = transactions.Where(w => w.StaffId != null).ToList().GroupBy(g => g.StaffId).Select(s => new
            {
                Id = s.Key,
                Code = db.Staff.Find(s.Key).Code,
                FirstName = db.Staff.Find(s.Key).FirstName,
                ImageUri = db.Staff.Find(s.Key).ImageUri,
                LastName = db.Staff.Find(s.Key).LastName,
                Transactions = s.GroupBy(gg => gg.TransactionType).Select(ss => new
                {
                    Description = ss.FirstOrDefault().Description,
                    Amount = ss.Sum(sum => sum.Amount),
                    Count = ss.Count(),
                    TransType = ss.Key,
                    TranAnalysis = ss.GroupBy(f => new { AccId = f.AccountId, AccDesc = f.Accounts.Description }).Select(ff => new
                    {
                        AccountDescr = ff.Key.AccDesc,
                        AccId = ff.Key.AccId,
                        Amount = ff.Sum(f => f.Amount),
                        TicketsCount = ff.Select(f => f.OrderDetail.Select(fff => fff.OrderDetailInvoices.Where(g => g.OrderDetail.Status != 5))).Count(),//ff.GroupBy(fff => fff.ord).Count(),
                        //Voids = ff.Select(f => f.OrderDetail.Select(fff => fff.OrderDetailInvoices.Where(g => g.OrderDetail.Status == 5)
                        //    .GroupBy(e => new { invgroup = e.PosInfoDetail.GroupId, invposinfid = e.PosInfoDetailId, invcounter = e.Counter  })))


                        //  Voids = ff.Select(o => o.Order.OrderDetail.Where(g => g.Status == 5).Where(gg => gg.))
                        //Void = ff.Select(o => o.OrderDetail.Select(oo => oo.OrderDetailInvoices
                        //    .Where(gg => gg.OrderDetail.Status == 5 && gg.PosInfoDetail.IsInvoice == true)
                        //    .Select(q => new
                        //    {
                        //        InvoiceGroup = q.PosInfoDetail.GroupId,
                        //        InvoiceAbbreviation = q.PosInfoDetail.Abbreviation,
                        //        InvoiceCounter = q.Counter
                        //    })))

                    })
                }).AsEnumerable(),
                Balance = s.Sum(sum => sum.Amount),
                TotalTransactions = s.Count(),
                UnPaidInvoices = new
                {
                    Count = invoices.Where(ww => ww.StaffId == s.Key).Count(),
                    Amount = invoices.Where(ww => ww.StaffId == s.Key).Sum(sum => sum.Total)
                }
            });
            return group;
        }

        public static dynamic GetWaiterReportNew_old(string filters, PosEntities db)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var piid = flts.PosList.FirstOrDefault();
            Int64? eodid = 0;
            var validInvoiceTypesIds = db.InvoiceTypes.Where(w => w.Code != "2" && w.Code != "3").Select(s => new { Id = s.Id, Abbreviation = s.Abbreviation });
            var dpInvTypes = db.InvoiceTypes.Where(w => w.Code == "2").Select(w => w.Id);

            var getInvoicesFromDetails = (from q in db.OrderDetailInvoices.Where(w => w.Invoices.EndOfDayId == null)
                                          join qq in db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == piid) on q.InvoicesId equals qq.Id
                                          join qqq in db.OrderDetail on q.OrderDetailId equals qqq.Id
                                          select new
                                          {
                                              InvoicesId = q.InvoicesId,
                                              OrderDetailId = q.OrderDetailId,
                                              Total = qqq.TotalAfterDiscount,
                                              Status = qqq.Status,
                                              PaidStatus = qqq.PaidStatus,
                                              InvoiceTypeId = qq.InvoiceTypeId,
                                              Qty = qqq.Qty
                                          }).Where(w => w.Status != 5 && w.PaidStatus != 2).ToList()
                          .GroupBy(g => g.OrderDetailId).Select(s => new
                          {
                              InvoicesId = s.FirstOrDefault().InvoicesId,
                              Total = s.FirstOrDefault().Total,
                              HasDP = s.Any(a => a.InvoiceTypeId == 2),
                              InvCount = s.Count(),
                              Qty = s.FirstOrDefault().Qty
                          }).Where(w => w.HasDP).GroupBy(g => g.InvoicesId).Select(ss => new
                          {
                              InvoicesId = ss.FirstOrDefault().InvoicesId,
                              Total = ss.Sum(sm => sm.Total),
                              Qty = ss.Sum(sm => sm.Qty)
                          });
            //getInvoicesFromDetails.Dump();

            var validInvoices = db.Invoices.Where(w => (w.EndOfDayId ?? 0) == eodid && (w.IsDeleted ?? false == false)).Select(s => s.Id).Distinct();
            var itemsCount = (from q in db.OrderDetailInvoices
                              join qq in db.OrderDetail on q.OrderDetailId equals qq.Id
                              join qqq in validInvoices on q.InvoicesId equals qqq
                              select new
                              {
                                  InvoiceId = q.InvoicesId,
                                  ItemsCount = qq.Qty

                              }).GroupBy(g => g.InvoiceId).Select(s => new
                              {
                                  InvoiceId = s.Key,
                                  ItemsCount = s.Sum(sm => sm.ItemsCount)
                              });

            var invsTemp = (from s in db.Invoices.Where(w => (w.EndOfDayId ?? 0) == eodid && (w.IsDeleted ?? false == false))
                            join qq in validInvoiceTypesIds on s.InvoiceTypeId equals qq.Id
                            join st in db.Staff on s.StaffId equals st.Id
                            join qqq in itemsCount on s.Id equals qqq.InvoiceId
                            select new
                            {
                                StaffId = s.StaffId,
                                LastName = st.LastName,
                                FirstName = st.FirstName,
                                InvoicesId = s.Id,
                                PosInfoId = s.PosInfoId,
                                Discount = s.Discount ?? 0,
                                Total = s.Total,
                                IsPaid = s.Transactions.Count() > 0,
                                IsVoided = s.IsVoided ?? false,
                                InvoiceTypeId = s.InvoiceTypeId,
                                IsPrinted = s.IsPrinted ?? false,
                                Abbreviation = qq.Abbreviation,
                                Counter = s.Counter,
                                ItemsCount = qqq.ItemsCount,
                                PaidTotal = 0//s.Transactions.Count() == 0? 0:s.Transactions.Sum(sm=>sm.Amount)
                            }).ToList();

            var transWithStaff = from q in db.Transactions.Where(w => w.EndOfDayId == null && w.PosInfoId == piid)
                                 join st in db.Staff on q.StaffId equals st.Id
                                 select new
                                 {
                                     InvoicesId = q.InvoicesId,
                                     Amount = q.Amount ?? 0,
                                     TransStaffId = q.StaffId,
                                     TransLastName = st.LastName,
                                     TransFirstName = st.FirstName,
                                 };


            var invs = (from q in invsTemp
                        join qq in transWithStaff on q.InvoicesId equals qq.InvoicesId into f
                        from it in f.DefaultIfEmpty()
                        select new
                        {
                            InvoicesId = q.InvoicesId,
                            PosInfoId = q.PosInfoId,
                            Discount = q.Discount,
                            Total = q.Total,
                            IsPaid = q.IsPaid,
                            IsVoided = q.IsVoided,
                            InvoiceTypeId = q.InvoiceTypeId,
                            IsPrinted = q.IsPrinted,
                            TransAmount = it.Amount,
                            InvoiceStaffId = q.StaffId,
                            InvoiceLastName = q.LastName,
                            InvoiceFirstName = q.FirstName,
                            TransStaffId = it.TransStaffId,
                            TransLastName = it.TransLastName,
                            TransFirstName = it.TransFirstName,
                            Abbreviation = q.Abbreviation,
                            Counter = q.Counter,
                            ItemsCount = q.ItemsCount
                        }).GroupBy(g => g.InvoicesId).Select(ss => new
                            {
                                InvoicesId = ss.FirstOrDefault().InvoicesId,
                                PosInfoId = ss.FirstOrDefault().PosInfoId,
                                Discount = ss.FirstOrDefault().Discount,
                                Total = ss.FirstOrDefault().Total,
                                IsPaid = ss.FirstOrDefault().IsPaid,
                                IsVoided = ss.FirstOrDefault().IsVoided,
                                InvoiceTypeId = ss.FirstOrDefault().InvoiceTypeId,
                                IsPrinted = ss.FirstOrDefault().IsPrinted,
                                PaidTotal = ss.Sum(sm => sm.TransAmount),
                                InvoiceStaffId = ss.FirstOrDefault().InvoiceStaffId,
                                TransInvoiceStaffId = ss.FirstOrDefault().TransStaffId,
                                Abbreviation = ss.FirstOrDefault().Abbreviation,
                                Counter = ss.FirstOrDefault().Counter,
                                ItemsCount = ss.FirstOrDefault().ItemsCount
                            });
            //	invs.Dump();
            var paidinvs = invs.Where(w => w.IsPaid && w.IsVoided == false);
            var voidinvs = invs.Where(w => w.IsPaid && w.IsVoided == true);
            var unpaidinvs = invs.Where(w => w.PaidTotal < w.Total);

            var validTransInvoices = db.Invoices.Where(w => (w.EndOfDayId ?? 0) == eodid && (w.IsDeleted ?? false == false) && (w.IsVoided ?? false) == false).Select(s => s.Id).Distinct();
            var validTransitemsCount = (from q in db.OrderDetailInvoices
                                        join qq in db.OrderDetail on q.OrderDetailId equals qq.Id
                                        join qqq in validTransInvoices on q.InvoicesId equals qqq
                                        select new
                                        {
                                            InvoiceId = q.InvoicesId,
                                            ItemsCount = qq.PaidStatus == 1 ? qq.Qty : 0,
                                            CancelCount = qq.Status == 5 ? qq.Qty : 0

                                        }).GroupBy(g => g.InvoiceId).Select(s => new
                                        {
                                            InvoiceId = s.Key,
                                            ItemsCount = s.Sum(sm => sm.ItemsCount),
                                            CancelCount = s.Sum(sm => sm.CancelCount)
                                        });
            var trans = (from s in db.Invoices.Where(w => (w.EndOfDayId ?? 0) == eodid && (w.IsDeleted ?? false == false) && (w.IsVoided ?? false) == false).SelectMany(sm => sm.Transactions)
                         join qq in validTransitemsCount on s.Id equals qq.InvoiceId
                         select new
                         {
                             AccountId = s.AccountId,
                             AccountDescription = s.Accounts.Description,
                             Amount = s.TransactionType == 4 ? s.Amount * -1 : s.Amount,
                             AccountType = s.Accounts.Type,
                             TransactionType = s.TransactionType,
                             StaffId = s.StaffId,
                             ItemsCount = qq.ItemsCount,
                             CancelCount = qq.CancelCount
                         }).ToList().GroupBy(g => new { g.AccountId, g.TransactionType, g.StaffId }).Select(ss => new
                          {
                              TransactionType = ss.FirstOrDefault().TransactionType,
                              AccountId = ss.FirstOrDefault().AccountId,
                              AccountDescription = ss.FirstOrDefault().AccountDescription,
                              AccountType = ss.FirstOrDefault().AccountType,
                              Amount = ss.Sum(sm => sm.Amount),
                              StaffId = ss.FirstOrDefault().StaffId,
                              ItemsCount = ss.FirstOrDefault().ItemsCount,
                              CancelCount = ss.FirstOrDefault().CancelCount
                          });



            var final = (from q in db.Staff.ToList()
                         join qq in trans on q.Id equals qq.StaffId into f
                         from tr in f.DefaultIfEmpty()
                         join qqq in invs on q.Id equals qqq.InvoiceStaffId into ff
                         from i in ff.DefaultIfEmpty()
                         join q4 in paidinvs on q.Id equals q4.InvoiceStaffId into f4
                         from pi in f4.DefaultIfEmpty()
                         join q5 in unpaidinvs on q.Id equals q5.InvoiceStaffId into f5
                         from ui in f5.DefaultIfEmpty()
                         select new
                         {
                             StaffId = q.Id,
                             FirstName = q.FirstName,
                             LastName = q.LastName,
                             Code = q.Code,
                             ImageUri = q.ImageUri,
                             PerTrType = tr,
                             PaidInvoices = pi,
                             UnPaidTickets = ui
                         }).GroupBy(g => g.StaffId).Select(s => new
                        {
                            StaffId = s.FirstOrDefault().StaffId,
                            FirstName = s.FirstOrDefault().FirstName,
                            LastName = s.FirstOrDefault().LastName,
                            Code = s.FirstOrDefault().Code,
                            ImageUri = s.FirstOrDefault().ImageUri,
                            PerTrType = s.Select(ss => ss.PerTrType).Where(w => w.TransactionType == 3).Distinct(),
                            PaidTickets = s.Select(ss => ss.PaidInvoices).Where(w => w.IsVoided == false && w.Total == w.PaidTotal).Distinct().Select(s3 => new
                                   {
                                       StaffId = 0,// s3 != null ? s3.StaffId : null,
                                       StaffName = "",//s3 != null ? s3.StaffName : null,
                                       InvoiceStaffId = 0,//s3 != null ? s3.InvoiceStaffId : null,
                                       InvoiceStaffName = "",//s3 != null ? s3.InvoiceStaffName : null,
                                       InvoiceId = s3.InvoicesId,
                                       Abbreviation = s3.Abbreviation,
                                       Counter = s3.Counter,
                                       InvoiceTotal = s3.Total,
                                   }).Distinct(),
                            UnPaidTickets = s.FirstOrDefault().UnPaidTickets != null ? s.Select(ss => ss.UnPaidTickets).Distinct() : null,
                            VoidTickets = s.FirstOrDefault().PaidInvoices != null ? s.Select(sss => sss.PaidInvoices).Where(w => w.IsVoided == true).Distinct() : null,
                            VoidsTotal = s.FirstOrDefault().PaidInvoices != null ? s.Select(sss => sss.PaidInvoices).Where(w => w.IsVoided == true).Distinct().Select(ss => ss.PaidTotal).Sum(sm => sm) : 0,
                            PartialPaid = s.FirstOrDefault().PaidInvoices != null ? s.Select(sss => sss.PaidInvoices).Where(w => w.IsVoided == false && w.Total > w.PaidTotal).Distinct() : null,
                            PartialRemaining = 0,//s.FirstOrDefault().PaidInvoices != null?s.Select(sss=>sss.PaidInvoices).Where(w=>w.IsVoided == false).Distinct().Select(ss=>ss.PaidTotal).Sum(sm=>sm):0,
                            UnPaidRemaining = s.FirstOrDefault().UnPaidTickets != null ? s.Select(sss => sss.UnPaidTickets).Distinct().Sum(sm => sm.Total - sm.PaidTotal) : 0,
                            TotalPaid = s.FirstOrDefault().PaidInvoices != null ? s.Select(sss => sss.PaidInvoices).Where(w => w.IsVoided == false).Distinct().Select(ss => ss.PaidTotal).Sum(sm => sm) : 0,

                        });


            return final.Where(w => w.VoidsTotal != 0 || w.UnPaidRemaining != 0 || w.TotalPaid != 0);
        }
        public static dynamic GetWaiterReportNew(string filters, PosEntities db)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            var piid = flts.PosList.FirstOrDefault();
            db.Configuration.LazyLoadingEnabled = true;
            db.Configuration.ProxyCreationEnabled = true;

            //var posdata = db.PosInfo.Where(f => f.Id == piid).FirstOrDefault();
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long> staffList = new List<long>();
            List<long?> validInvoiceTypes = new List<long?>();

            validInvoiceTypes.Add((Int64)InvoiceTypesEnum.Receipt);
            validInvoiceTypes.Add((Int64)InvoiceTypesEnum.Timologio);
            validInvoiceTypes.Add((Int64)InvoiceTypesEnum.Coplimentary);
            validInvoiceTypes.Add((Int64)InvoiceTypesEnum.Allowance);

            eod.Add(0);
            posList.Add(piid);

            var st1 = db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false).Select(s => s.StaffId.Value).Distinct();
            var st2 = db.Transactions.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && posList.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false).Select(s => s.StaffId.Value).Distinct();


            staffList.AddRange(st2.Union(st1).Distinct());

            var trans = (from q in db.Transactions.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && staffList.Contains(w.StaffId.Value) && posList.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false)
                         join a in db.Accounts on q.AccountId equals a.Id
                         join s in db.Staff on q.StaffId equals s.Id
                         select new
                         {
                             TransactionId = q.Id,
                             TransDescription = q.Description,
                             AccountId = a.Id,
                             AccountDescription = a.Description,
                             AccountType = a.Type,
                             StaffId = s.Id,
                             StaffName = s.LastName,
                             Amount = q.Amount,
                             TransactionType = q.TransactionType
                         }).ToList();

            //Fix End of day on Credit Accounts
            var dtnow = DateTime.Now.Date;
            var creditTrans = (from q in db.CreditTransactions.Where(w => staffList.Contains(w.StaffId.Value) && posList.Contains(w.PosInfoId) && EntityFunctions.TruncateTime(w.CreationTS) == dtnow)
                               join s in db.Staff on q.StaffId equals s.Id
                               select new {
                                   Description = q.Description,
                                   Amount = q.Amount,
                                   CreateTS = q.CreationTS,
                                   StaffId = s.Id,
                                   StaffName = s.LastName,
                                   Type = q.Type
                               }).GroupBy(g => g.Type).Select(s => new
                               {
                                   Description = s.FirstOrDefault().Description,
                                   Type = s.FirstOrDefault().Type,
                                   Amount = s.Sum(sm => sm.Amount),
                                   StaffId = s.FirstOrDefault().StaffId,
                                   StaffName = s.FirstOrDefault().StaffName,
                               }).ToList();

            //var creditTrans = (from q in db.Transactions.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && staffList.Contains(w.StaffId.Value) && posList.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false)
            //                   join qq in db.CreditTransactions on q.InvoicesId equals qq.InvoiceId
            //                   join a in db.Accounts on q.AccountId equals a.Id
            //                   join s in db.Staff on q.StaffId equals s.Id
            //                   select new
            //                   {
            //                       TransactionId = q.Id,
            //                       TransDescription = qq.Description,
            //                       AccountId = a.Id,
            //                       AccountDescription = a.Description,
            //                       AccountType = a.Type,
            //                       StaffId = s.Id,
            //                       StaffName = s.LastName,
            //                       Amount = q.Amount,
            //                       TransactionType = qq.Type
            //                   }).GroupBy(g => g.TransactionType).Select(s => new
            //                   {
            //                       Description  = s.FirstOrDefault().TransDescription,
            //                       Amount = s.Sum(sm=>sm.Amount),
            //                       StaffId = s.FirstOrDefault().StaffId,
            //                       StaffName = s.FirstOrDefault().StaffName,
            //                   }).ToList();




            var invs = db.Invoices.Where(w => eod.Contains((w.EndOfDayId ?? 0)) && staffList.Contains(w.StaffId.Value) && posList.Contains(w.PosInfoId) && (w.IsDeleted ?? false) == false);

            var TotalsByTrType = trans.GroupBy(gw => gw.StaffId).Select(s => new
            {
                StaffId = s.FirstOrDefault().StaffId,
                StaffName = s.FirstOrDefault().StaffName,
                Total = Math.Round((decimal)s.Sum(sm => (Decimal?)sm.Amount), 2, MidpointRounding.AwayFromZero),
                PerTrType = s.GroupBy(g => g.TransactionType).Select(ss => new
                {
                    Amount = Math.Round((decimal)ss.Sum(sm => (Decimal?)sm.Amount), 2, MidpointRounding.AwayFromZero),
                    Description = ss.FirstOrDefault().TransDescription,
                    TransactionType = ss.FirstOrDefault().TransactionType,
                    TranAnalysis = ss.GroupBy(g => g.AccountId).Select(fss => new
                    {
                        AccountDescr = fss.FirstOrDefault().AccountDescription,
                        AccountType = fss.FirstOrDefault().AccountType,
                        Amount = Math.Round((decimal)fss.Sum(sm => (Decimal?)sm.Amount), 2, MidpointRounding.AwayFromZero)
                    }),
                }),
            });

            //var TotalsByAccountWaiter = trans.GroupBy(gw => gw.StaffId).Select(sss => new
            //{
            //    StaffId = sss.FirstOrDefault().StaffId,
            //    StaffName = sss.FirstOrDefault().StaffName,
            //    PerAccount = sss.GroupBy(g => g.AccountId).Select(ss => new
            //    {
            //        Description = ss.FirstOrDefault().AccountDescription,
            //        AccountType = ss.FirstOrDefault().AccountType,
            //        Total = Math.Round((decimal)ss.Sum(sm => (Decimal?)sm.Amount), 2, MidpointRounding.AwayFromZero)
            //    }),

            //});
            var PaidTickets = invs.Where(w => (w.IsVoided ?? false) == false && w.Transactions.Count() > 0 && validInvoiceTypes.Contains(w.InvoiceTypes.Type))

                                    .SelectMany(sm => sm.Transactions).Select(s => new
                                    {

                                        StaffId = s.StaffId,
                                        StaffName = s.Staff.LastName,
                                        InvoiceStaffId = s.Invoices.StaffId,
                                        InvoiceStaffName = s.Invoices.Staff.LastName,
                                        InvoiceId = s.InvoicesId,
                                        Abbreviation = s.Invoices.InvoiceTypes.Abbreviation,
                                        Counter = s.Invoices.Counter,
                                        InvoiceTotal = s.Invoices.Total,
                                        TransAmount = s.Amount,
                                        IsVoided = s.Invoices.IsVoided



                                    }).ToList();
            var UnPaidTickets = invs.Where(w => (w.IsVoided ?? false) == false && w.Transactions.Count() == 0 && validInvoiceTypes.Contains(w.InvoiceTypes.Type)).ToList().Select(s => new
            {
                StaffId = s.StaffId,
                StaffName = s.Staff.LastName,
                InvoiceId = s.Id,
                Abbreviation = s.InvoiceTypes.Abbreviation,
                Counter = s.Counter,
                InvoiceTotal = s.Total,
            }).ToList();

            var partialPaid = PaidTickets.GroupBy(g => g.InvoiceId).Select(s => new
            {
                StaffId = s.FirstOrDefault().InvoiceStaffId,
                StaffName = s.FirstOrDefault().InvoiceStaffName,
                Abbreviation = s.FirstOrDefault().Abbreviation,
                Counter = s.FirstOrDefault().Counter,
                IsPartialPaid = s.Sum(sm => sm.TransAmount) != s.FirstOrDefault().InvoiceTotal,
                PaidAmount = s.Sum(sm => sm.TransAmount),
                Remaining = s.FirstOrDefault().InvoiceTotal - s.Sum(sm => sm.TransAmount)
            }).ToList();
            var VoidTickets = invs.Where(w => (w.IsVoided ?? false) == true).Select(s => new
            {
                StaffId = s.StaffId,
                StaffName = s.Staff.LastName,
                InvoiceId = s.Id,
                Abbreviation = s.InvoiceTypes.Abbreviation,
                Counter = s.Counter,
                InvoiceTotal = s.Total,
            }).ToList(); ;


            var jon = (from q in db.Staff.Where(w => staffList.Contains(w.Id)).ToList()
                       join ppa in TotalsByTrType on q.Id equals ppa.StaffId into f
                       from pa in f.DefaultIfEmpty()
                       join paidt in PaidTickets on q.Id equals paidt.StaffId into ff
                       from pt in ff.DefaultIfEmpty()
                       join uut in UnPaidTickets on q.Id equals uut.StaffId into fff
                       from ut in fff.DefaultIfEmpty()
                       join vvt in VoidTickets on q.Id equals vvt.StaffId into ffff
                       from vt in ffff.DefaultIfEmpty()
                       join ppaidt in partialPaid on q.Id equals ppaidt.StaffId into fffff
                       from ppt in fffff.DefaultIfEmpty()
                       join cct in creditTrans on q.Id equals cct.StaffId into f5
                       from cctrans in f5.DefaultIfEmpty()
                       select new
                       {
                           StaffId = q.Id,
                           FirstName = q.FirstName,
                           LastName = q.LastName,
                           Code = q.Code,
                           ImageUri = q.ImageUri,
                           PerTrType = pa != null ? pa.PerTrType : null,
                           PaidTickets = pt,
                           UnPaidTickets = ut,
                           VoidTickets = vt,
                           PartialPaid = ppt,
                           creditTrans = cctrans
                       }).ToList().GroupBy(g => g.StaffId).Select(s => new
                       {
                           StaffId = s.FirstOrDefault().StaffId,
                           FirstName = s.FirstOrDefault().FirstName,
                           LastName = s.FirstOrDefault().LastName,
                           Code = s.FirstOrDefault().Code,
                           ImageUri = s.FirstOrDefault().ImageUri,
                           PerTrType = s.FirstOrDefault().PerTrType,
                           PaidTickets = s.Select(ss => ss.PaidTickets).Select(s3 => new
                           {
                               StaffId = s3 != null ? s3.StaffId : null,
                               StaffName = s3 != null ? s3.StaffName : null,
                               InvoiceStaffId = s3 != null ? s3.InvoiceStaffId : null,
                               InvoiceStaffName = s3 != null ? s3.InvoiceStaffName : null,
                               InvoiceId = s3 != null ? s3.InvoiceId : null,
                               Abbreviation = s3 != null ? s3.Abbreviation : null,
                               Counter = s3 != null ? s3.Counter : null,
                               InvoiceTotal = s3 != null ? s3.InvoiceTotal : null,
                           }).Distinct(),
                           UnPaidTickets = s.Select(ss => ss.UnPaidTickets).Distinct(),
                           VoidTickets = s.Select(ss => ss.VoidTickets).Distinct(),
                           VoidsTotal = s.Select(ss => ss.VoidTickets).FirstOrDefault() != null ? s.Select(ss => ss.VoidTickets).Distinct().Sum(sm => sm.InvoiceTotal) : 0,
                           PartialPaid = s.Select(ss => ss.PartialPaid).Distinct(),
                           PartialRemaining = s.Select(ss => ss.PartialPaid).FirstOrDefault() != null ? s.Select(ss => ss.PartialPaid).Distinct().Sum(sm => sm.Remaining) : 0,
                           UnPaidRemaining = s.Select(ss => ss.UnPaidTickets).FirstOrDefault() != null ? s.Select(ss => ss.UnPaidTickets).Distinct().Sum(sm => sm.InvoiceTotal) : 0,
                           TotalPaid = s.FirstOrDefault().PerTrType != null ? s.FirstOrDefault().PerTrType.Sum(sm => sm.Amount) : 0,
                           CreditTrans = s.Select(ss=>ss.creditTrans).Distinct()
                       });

            db.Configuration.LazyLoadingEnabled = false;

            return jon;
        }
    }


}