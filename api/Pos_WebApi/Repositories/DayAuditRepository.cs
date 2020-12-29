using log4net;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models.FilterModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Pos_WebApi.Repositories {
    public class DayAuditRepository :IDisposable{
        protected PosEntities DbContext;
        BussinessRepository br;
        SendTransferRepository str;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DayAuditRepository( PosEntities db ) {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            br = new BussinessRepository(this.DbContext);
            str = new SendTransferRepository(this.DbContext);
        }

        /// <summary>
        /// Return invoices for specific EndOfDayId
        /// </summary>
        /// <param name="endOfDayId"></param>
        /// <param name="predicate"></param>
        /// <param name="printedOnly"></param>
        /// <returns></returns>
        private IQueryable<AuditFlatInvoicesBaseClass> GetAuditFlatInvoicesBase( long endOfDayId, Expression<Func<AuditFlatInvoicesBaseClass, bool>> predicate = null, bool printedOnly = true ) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("            select ");
            sb.AppendLine("     Day = i.Day ");
            sb.AppendLine("    , DepartmentId = IsNull(pi.DepartmentId,0) ");
            sb.AppendLine("    , PosInfoDescription = pi.Description ");
            sb.AppendLine("    , InvoiceId = i.Id ");
            sb.AppendLine("    , OrderNo = Cast (IsNull(od.OrderNo, 0) as bigInt) ");
            sb.AppendLine("    , Counter =  Cast (IsNull(i.Counter,0 ) as bigInt)  ");
            sb.AppendLine("    , Covers = Cast (IsNull(i.Cover, 0) as bigInt) ");
            sb.AppendLine("    , InvoiceAbbreviation = od.Abbreviation ");
            sb.AppendLine("    , InvoiceType = od.InvoiceType ");
            sb.AppendLine("    , InvoiceTotal = i.Total ");
            sb.AppendLine("    , InvoiceDiscount = i.Discount ");
            sb.AppendLine("    , InvoicePaidAmount = i.PaidTotal ");
            sb.AppendLine("    , IsPaid = i.IsPaid ");
            sb.AppendLine("    , IsInvoiced = cast(case od.InvoiceType when 2 then i.IsInvoiced else 1 end  as bit)");
            sb.AppendLine("    , IsPrinted = IsNull(i.IsPrinted, 0)");
            sb.AppendLine("	   , IsVoided = isNull(i.IsVoided,0) ");
            //sb.AppendLine("    -- IsDeleted = i.IsDeleted ?? false, ");
            sb.AppendLine("    , VatId = od.VatId ");
            sb.AppendLine("    , VatCode = v.Code ");
            sb.AppendLine("    , Total = od.Total ");
            sb.AppendLine("    , VatAmount = od.VatAmount ");
            sb.AppendLine("	   , VatRate = od.VatRate ");
            sb.AppendLine("    , Net = od.Net ");
            sb.AppendLine("    , AccountId = IsNull(tr.AccountId, 0) ");
            sb.AppendLine("    , AccountDesc = IsNull(a.Description, '') ");
            sb.AppendLine("	   , TableCode = od.TableCode ");
            sb.AppendLine("    , AccountType = a.Type ");
            sb.AppendLine("	   , TransAmount = IsNull(tr.Amount, 0) ");
            sb.AppendLine("	   , TransctionId = tr.Id ");
            sb.AppendLine("	   , TransStaff = tr.StaffId ");
            sb.AppendLine("	   , StaffId = i.StaffId ");
            sb.AppendLine("    , OrderDetailId = od.OrderDetailId ");
            sb.AppendLine("	   , FiscalType = pi.FiscalType ");
            //sb.AppendLine("                            --//FODay = od.FOdd ..eod == null ? DateTime.Now : eod.FODay, ");
            sb.AppendLine("	   , EndOfDayId = IsNull(od.EndOfDayId,0) ");
            sb.AppendLine("    , PosInfoId = od.PosInfoId ");
            sb.AppendLine("     , Room =  i.Rooms ");
            sb.AppendLine("from OrderDetailInvoices od ");
            sb.AppendLine("join Invoices i on od.InvoicesId = i.Id and IsNull(i.IsDeleted,0) = 0 ");
            sb.AppendLine("join posInfo pi on pi.Id = i.PosInfoId ");
            sb.AppendLine("join Department d on d.Id = pi.DepartmentId ");
            sb.AppendLine("left outer join Transactions tr  on tr.InvoicesId = i.Id and IsNull(tr.IsDeleted,0) = 0 ");
            sb.AppendLine("left outer join Accounts a on a.Id = tr.AccountId ");
            sb.AppendLine("join Staff s on s.id = i.StaffId ");
            sb.AppendLine("left outer join EndOfDay eod on eod.id = i.EndOfDayId ");
            sb.AppendLine("join Vat v on v.Id = od.VatId ");
            sb.AppendLine(string.Format("Where IsNull(i.endOfDayId, 0) = {0} and  i.IsPrinted = {1} and  IsNull(od.IsDeleted,0) = 0 and od.InvoiceType != 8", endOfDayId, printedOnly ? 1 : 0));

            var query = DbContext.Database.SqlQuery<AuditFlatInvoicesBaseClass>(sb.ToString()).AsQueryable<AuditFlatInvoicesBaseClass>();
            if ( predicate != null ) {
                return query = query.Where(predicate);
            } else
                return query;

        }

        private IQueryable<AuditFlatInvoicesBaseClass> GetAuditFlatInvoicesBaseOld( long endOfDayId, Expression<Func<AuditFlatInvoicesBaseClass, bool>> predicate = null, bool printedOnly = true ) {
            //var a = br.ReceiptDetailsBO(x => x.EndOfDayId == null).ToList();
            //var b = br.ReceiptsBO(x => x.EndOfDayId == 0).ToList();
            //var c = br.ReceiptPaymentsFlat(x => x.EndOfDayId == 0).ToList();
            var query = from q in br.ReceiptDetailsBO(x => x.EndOfDayId == null)
                        join qq in br.ReceiptsBO(x => x.EndOfDayId == 0 && x.IsPrinted == printedOnly) on q.ReceiptsId equals qq.Id
                        join qqq in br.ReceiptPaymentsFlat(x => x.EndOfDayId == 0) on q.ReceiptsId equals qqq.InvoicesId into f
                        from tr in f.DefaultIfEmpty()
                        join pi in DbContext.PosInfo on qq.PosInfoId equals pi.Id
                        //var query = from q in DbContext.OrderDetailInvoices//.Where(w => (w.EndOfDayId ?? 0) == endOfDayId && w.PosInfoId == posInfoId)
                        //            join qq in DbContext.Invoices on q.InvoicesId equals qq.Id
                        //            join qqq in DbContext.Transactions on q.InvoicesId equals qqq.InvoicesId into f
                        //            from tr in f.DefaultIfEmpty()
                        //                        join qqqq in DbContext.PosInfoDetail on q.PosInfoDetailId equals qqqq.Id
                        //                        join qqqqq in DbContext.EndOfDay on q.EndOfDayId equals qqqqq.Id into ff
                        //                        from eod in ff.DefaultIfEmpty()
                        where // (qq.IsDeleted == false || qq.IsDeleted == null)
                              //                           && (q.IsDeleted == false || q.IsDeleted == null)
                              //&& 
                           q.InvoiceType != 8 && q.InvoiceType != 11 && q.InvoiceType != 12
                           && !( q.InvoiceType == 2 && qq.IsVoided == true )
                        //&& qq.EndOfDayId == null
                        select new AuditFlatInvoicesBaseClass {
                            Day = qq.Day,
                            DepartmentId = qq.DepartmentId,
                            PosInfoDescription = pi.Description,
                            InvoiceId = qq.Id,
                            OrderNo = q.OrderNo,
                            Counter = qq.ReceiptNo,
                            Covers = qq.Cover,
                            InvoiceAbbreviation = q.Abbreviation,
                            InvoiceType = q.InvoiceType,
                            InvoiceTotal = qq.Total,
                            InvoiceDiscount = qq.Discount,
                            InvoicePaidAmount = qq.PaidTotal,
                            IsPaid = qq.IsPaid,
                            IsInvoiced = q.InvoiceType == 2 ? qq.IsInvoiced : true,
                            IsVoided = qq.IsVoided,
                            // IsDeleted = qq.IsDeleted ?? false,
                            VatId = q.VatId,
                            Total = q.ItemGross,
                            VatAmount = q.ItemVatValue,
                            VatRate = q.ItemVatRate,
                            Net = q.ItemNet,
                            AccountId = tr != null ? tr.AccountId : 0,
                            //AccountDesc = tr != null? tr.AccountDescription:"",
                            TableCode = q.TableCode,
                            AccountType = tr != null ? tr.AccountType : 0,
                            TransAmount = tr != null ? tr.Amount : 0,
                            TransctionId = tr != null ? (long?) tr.Id : null,
                            TransStaff = tr.StaffId,
                            StaffId = qq.StaffId,
                            OrderDetailId = q.OrderDetailId,
                            //FiscalType = qq.FiscalType,
                            //FODay = q.FOdd ..eod == null ? DateTime.Now : eod.FODay,
                            EndOfDayId = q.EndOfDayId,
                            PosInfoId = q.PosInfoId,
                            Room = qq.Room
                            //PaidStatus = q.Paid
                        };
            if ( predicate != null )
                query = query.Where(predicate);
            return query;
        }

        private IEnumerable<AuditGroupByInvoiceBaseClass> GetAuditGroupByInvoice( IQueryable<AuditFlatInvoicesBaseClass> flatInvoices, Expression<Func<AuditGroupByInvoiceBaseClass, bool>> predicate = null, int? byAccount = null ) {
            var query = flatInvoices.ToList().GroupBy(g => g.InvoiceId);
            if ( byAccount != null )
                query = query.Where(a => a.Any(aa => aa.AccountType == byAccount));
            var qroupedByInvoice = query
                                      .Select(s => new AuditGroupByInvoiceBaseClass {
                                          Day = s.FirstOrDefault().Day,
                                          DepartmentId = s.FirstOrDefault().DepartmentId,
                                          PosInfoId = s.FirstOrDefault().PosInfoId,
                                          PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
                                          InvoiceId = s.FirstOrDefault().InvoiceId,
                                          OrderNo = s.FirstOrDefault().OrderNo,
                                          Abbreviation = s.FirstOrDefault().InvoiceAbbreviation,
                                          ReceiptNo = s.FirstOrDefault().Counter,
                                          InvoiceType = s.FirstOrDefault().InvoiceType,
                                          IsPaid = s.FirstOrDefault().IsPaid,
                                          InvoiceTotal = s.FirstOrDefault().InvoiceType == 3 ? s.FirstOrDefault().InvoiceTotal * -1 : s.FirstOrDefault().InvoiceTotal,
                                          InvoiceDiscount = s.FirstOrDefault().InvoiceDiscount,
                                          InvoicePaidAmount = s.FirstOrDefault().InvoicePaidAmount ?? 0,
                                          IsInvoiced = s.FirstOrDefault().IsInvoiced,
                                          IsVoided = s.FirstOrDefault().IsVoided,
                                          Vat1 = s.Where(w => w.VatCode == 1).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
                                          Vat2 = s.Where(w => w.VatCode == 2).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
                                          Vat3 = s.Where(w => w.VatCode == 3).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
                                          Vat4 = s.Where(w => w.VatCode == 4).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
                                          Vat5 = s.Where(w => w.VatCode == 5).Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
                                          Vat1Perc = s.Where(w => w.VatCode == 1).Min(mn => mn.VatRate),
                                          Vat2Perc = s.Where(w => w.VatCode == 2).Min(mn => mn.VatRate),
                                          Vat3Perc = s.Where(w => w.VatCode == 3).Min(mn => mn.VatRate),
                                          Vat4Perc = s.Where(w => w.VatCode == 4).Min(mn => mn.VatRate),
                                          Vat5Perc = s.Where(w => w.VatCode == 5).Min(mn => mn.VatRate),
                                          AccountType1 = s.Where(w => w.AccountType == 1).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType2 = s.Where(w => w.AccountType == 2).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType3 = s.Where(w => w.AccountType == 3).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType4 = s.Where(w => w.AccountType == 4).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType5 = s.Where(w => w.AccountType == 5).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType6 = s.Where(w => w.AccountType == 6).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType7 = s.Where(w => w.AccountType == 7).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType8 = s.Where(w => w.AccountType == 8).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType9 = s.Where(w => w.AccountType > 8).Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId }).Distinct().Sum(sm => sm.TransAmount),
                                          AccountType1ReceiptCount = s.Where(w => w.AccountType == 1).Count() > 0 ? 1 : 0,
                                          AccountType2ReceiptCount = s.Where(w => w.AccountType == 2).Count() > 0 ? 1 : 0,
                                          AccountType3ReceiptCount = s.Where(w => w.AccountType == 3).Count() > 0 ? 1 : 0,
                                          AccountType4ReceiptCount = s.Where(w => w.AccountType == 4).Count() > 0 ? 1 : 0,
                                          AccountType5ReceiptCount = s.Where(w => w.AccountType == 5).Count() > 0 ? 1 : 0,
                                          AccountType6ReceiptCount = s.Where(w => w.AccountType == 6).Count() > 0 ? 1 : 0,
                                          AccountType7ReceiptCount = s.Where(w => w.AccountType == 7).Count() > 0 ? 1 : 0,
                                          AccountType8ReceiptCount = s.Where(w => w.AccountType == 8).Count() > 0 ? 1 : 0,
                                          AccountType9ReceiptCount = s.Where(w => w.AccountType > 8).Count() > 0 ? 1 : 0,
                                          TransAmount = s.Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount }).Distinct().Sum(sm => sm.TransAmount),
                                          CardAnalysis = s.Where(w => w.AccountType == 4).Select(ss => new {
                                              AccountId = ss.AccountId,
                                              TransAmount = ss.TransAmount
                                          })
                                                    .Distinct()
                                                    .GroupBy(g => g.AccountId).Select(ssss => new AuditCreditCardsAmounts {
                                                        AccountId = ssss.Key,
                                                        TransAmount = ssss.Sum(sm => sm.TransAmount)
                                                    }).ToList(),
                                          ItemsCount = s.Count(),
                                          StaffId = s.FirstOrDefault().StaffId,
                                          FiscalType = s.FirstOrDefault().FiscalType,
                                          FODay = s.FirstOrDefault().FODay,
                                          EndOfDayId = s.FirstOrDefault().EndOfDayId,
                                          Covers = s.FirstOrDefault().Covers,
                                          Check = s.FirstOrDefault().InvoiceTotal - s.Select(ss => new { VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId }).Distinct().Sum(sm => sm.Total),
                                          Room = s.FirstOrDefault().Room,
                                          TableCode = s.FirstOrDefault().TableCode
                                      }).AsQueryable<AuditGroupByInvoiceBaseClass>();
            if ( predicate != null )
                return qroupedByInvoice = qroupedByInvoice.Where(predicate);
            else
                return qroupedByInvoice;
        }

        /// <summary>
        /// Returns Not Audited sales (Ανάλυση Πληρωμών) by PosId for a day
        /// </summary>
        /// <param name="endOfDayId">EndOfDay.Id for a previous day, or 0 for the current day</param>
        /// <param name="posInfoId">PosInfo.Id</param>
        /// <returns>
        /// i.	Τα αθροίσματα (όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό) των παραστατικών ανά τρόπο πληρωμής (Account Type) 
        /// ii. Τα αθροίσματα των μη εξοφλημένων αποδείξεων(όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό). Με εξοφλημένες είναι οι αποδείξεις που δεν υπάρχει για αυτέs εγγραφή στον transactions ή υπάρχει εγγραφή και είναι μερικώς εξοφλημένη.
        /// iii.Το άθροισμα των παραπάνω  (σύνολο Αποδείξεων, συνολικό ποσό)
        ///  iv.Τη συνολική έκπτωση
        ///   v.Τα αθροίσματα των ακυρωμένων αποδείξεων (όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό)
        ///  vi.Το άθροισμα των μη τιμολογημένων αποδείξεων(όνομα πληρωμής, σύνολο Αποδείξεων, συνολικό ποσό)
        /// vii.Ανάλυση couver(συνολικό ποσό)
        /// </returns>
        public IEnumerable<dynamic> GetPosSales( long endOfDayId, long? posInfoId ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0 && w.PosInfoId == posInfoId && !( w.InvoiceType == 2 && w.IsInvoiced ) /*&& (w.InvoiceType != 3 && w.IsVoided == false) || w.InvoiceType == 3 && w.IsVoided == true*/);//.ToList();
            var qroupedByInvoice = GetAuditGroupByInvoice(query).ToList();

            var dayAudit = qroupedByInvoice.Where(w => (w.InvoiceType != 2 && w.InvoiceType != 11 && w.InvoiceType != 12) || ( w.InvoiceType == 2 && w.IsInvoiced == false )).ToList()
                                         .GroupBy(g => g.FODay)
                                         .Select(s => new {
                                             Day = s.FirstOrDefault().Day,
                                             FODay = s.Key,
                                             Total = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.InvoiceTotal),
                                             //check = s.Where(w=> w.InvoiceType != 2).Distinct(),
                                             ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Count(),
                                             NotInvoicedAmount = s.Where(w => w.InvoiceType == 2 && w.IsVoided == false).Distinct().Sum(sm => sm.InvoiceTotal),
                                             NotInvoicedReceiptsCount = s.Where(w => w.InvoiceType == 2 && w.IsVoided == false).Distinct().Count(),
                                             //	check = s.Where(w=>w.InvoiceType == 2).Distinct(),
                                             NotPaidAmount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsPaid < 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ?
                                                       s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsPaid < 2 && w.IsVoided == false)
                                                             //  .GroupBy(g => g.InvoiceId).Select(sss => sss.FirstOrDefault().InvoiceTotal - sss.FirstOrDefault().InvoicePaidAmount).FirstOrDefault() : (decimal?)0,
                                                             .GroupBy(g => g.InvoiceId).Sum(sss => sss.FirstOrDefault().InvoiceTotal - sss.FirstOrDefault().InvoicePaidAmount) : (decimal?) 0,

                                             //  NotPaidAmount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false && w.IsPaid < 2).Distinct().Sum(sm => sm.InvoiceTotal) - s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.InvoicePaidAmount),
                                             NotPaidReceiptsCount = s.Where(w => w.InvoiceType != 2 && w.IsPaid < 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Count(),

                                             VoidsReceiptsAmount = s.Where(w => w.InvoiceType == 3).Distinct().Sum(sm => sm.InvoiceTotal),
                                             VoidsReceiptsCount = s.Where(w => w.InvoiceType == 3).Distinct().Count(),

                                             Discount = s.Where(w => w.InvoiceType != 2).Distinct().Sum(sm => sm.InvoiceDiscount),
                                             Vat1 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.Vat1),
                                             Vat2 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.Vat2),
                                             Vat3 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.Vat3),
                                             Vat4 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.Vat4),
                                             Vat5 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.Vat5),

                                             AccountType1 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType1),
                                             AccountType2 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType2),
                                             AccountType3 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType3),
                                             AccountType4 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType4),
                                             AccountType5 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType5),
                                             AccountType6 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType6),
                                             AccountType7 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType7),
                                             AccountType8 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType8),
                                             AccountType9 = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Distinct().Sum(sm => sm.AccountType9),

                                             AccountType1ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType1ReceiptCount),
                                             AccountType2ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType2ReceiptCount),
                                             AccountType3ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType3ReceiptCount),
                                             AccountType4ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType4ReceiptCount),
                                             AccountType5ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType5ReceiptCount),
                                             AccountType6ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType6ReceiptCount),
                                             AccountType7ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType7ReceiptCount),
                                             AccountType8ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType8ReceiptCount),
                                             AccountType9ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.AccountType9ReceiptCount),

                                             CardAnalysis = s.SelectMany(sss => sss.CardAnalysis).GroupBy(g => g.AccountId).Select(ss1 => new AuditCreditCardsAmounts {
                                                 AccountId = ss1.Key,
                                                 TransAmount = ss1.Sum(sm => sm.TransAmount),
                                             }).ToList(),
                                             //Covers = s.Where(w => w.InvoiceType != 2).Distinct().Sum(sm => sm.Covers),
                                             Covers = DbContext.Order.Where(o => (o.EndOfDayId ?? 0) == 0 && o.PosId == posInfoId).Sum(c => c.Couver),
                                             WholeSalesTotal = s.Where(w => w.InvoiceType != 2 && w.FiscalType == 1).Distinct().Sum(sm => sm.InvoiceTotal),
                                             ReatailSalesTotal = s.Where(w => w.InvoiceType != 2 && w.FiscalType == 0).Distinct().Sum(sm => sm.InvoiceTotal),
                                             Room = s.FirstOrDefault().Room,
                                             TableCode = s.FirstOrDefault().TableCode
                                         });

            return dayAudit;
        }


        /// <summary>
        /// return not printed orders (sales) for aspecific day [InvoiceType = 2 (Δελτίο παραγγελίας) and IsInvoiced = false]
        /// </summary>
        /// <param name="endOfDayId">endOfDayId. For the current/not closed day: endOfDayId=0 </param>
        /// <param name="posInfoId">Current Pos</param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetNotPrintedPosSales( long endOfDayId, long? posInfoId ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0 && w.PosInfoId == posInfoId && !( w.InvoiceType == 2 && w.IsInvoiced ), false);
            var qroupedByInvoice = GetAuditGroupByInvoice(query).ToList();

            return qroupedByInvoice.Where(w => !( w.InvoiceType == 2 && w.IsInvoiced == false ));
        }


        /// <summary>
        ///  Return receipts (sales) for specific invoice type, PosId and day
        /// </summary>
        /// <param name="endOfDayId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="invoiceType"></param>
        /// <param name="paidOnly"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetPosSalesByInvoiceType( long endOfDayId, long? posInfoId, int invoiceType, bool paidOnly = false ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0 && w.PosInfoId == posInfoId /*&& (w.InvoiceType != 3 && w.IsVoided == false) || w.InvoiceType == 3 && w.IsVoided == false*/);//.ToList();
            var qroupedByInvoice = GetAuditGroupByInvoice(query).ToList();

            if ( invoiceType == 2 )
                return qroupedByInvoice.Where(w => w.IsInvoiced == false && w.IsVoided == false);
            return qroupedByInvoice.Where(w => w.InvoiceType == invoiceType);
        }

        public IEnumerable<dynamic> GetDepartmentSalesByInvoiceType( long endOfDayId, long? departmentId, bool paidOnly = false ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0 && w.DepartmentId == departmentId /*&& (w.InvoiceType != 3 && w.IsVoided == false) || w.InvoiceType == 3 && w.IsVoided == false*/);//.ToList();
            var qroupedByInvoice = GetAuditGroupByInvoice(query).ToList();


            return qroupedByInvoice.Where(w => !( w.InvoiceType == 2 && w.IsInvoiced == false ));
        }

        public IEnumerable<dynamic> GetAllPosSalesByInvoice( long endOfDayId, long? posInfoId, bool paidOnly = false ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0 && w.PosInfoId == posInfoId /*&& (w.InvoiceType != 3 && w.IsVoided == false) || w.InvoiceType == 3 && w.IsVoided == false*/);//.ToList();
            var qroupedByInvoice = GetAuditGroupByInvoice(query).ToList();


            return qroupedByInvoice.Where(w => !( w.InvoiceType == 2 && w.IsInvoiced == false ));
        }

        public IEnumerable<dynamic> GetAllPosSalesByStaff( long endOfDayId, long? posInfoId, long? staffId, bool paidOnly = false ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0 && w.PosInfoId == posInfoId /*&& (w.InvoiceType != 3 && w.IsVoided == false) || w.InvoiceType == 3 && w.IsVoided == false*/);//.ToList();

            var qroupedByInvoice = GetAuditGroupByInvoice(query).ToList();


            return qroupedByInvoice.Where(w => !( w.InvoiceType == 2 && w.IsInvoiced == false ) && w.StaffId == staffId);
        }

        /// <summary>
        /// Return not paid receipts (sales) for specific account type, PosId and  day
        /// </summary>
        /// <param name="endOfDayId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetPosSalesByNotPaid( long endOfDayId, long? posInfoId, long? staffId = null ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0
                                                    && w.PosInfoId == posInfoId
                                                    && w.InvoiceType != 2 && w.IsPaid < 2
                                                    && ( w.InvoiceType != 3 && w.IsVoided == false )
                                                    && w.InvoiceType != 8//  && w.IsVoided == false
                                                    && w.IsVoided == false
                                                    );//.ToList();
            var qroupedByInvoice = GetAuditGroupByInvoice(query).ToList();

            if ( staffId != null )
                return qroupedByInvoice.Where(w => w.StaffId == staffId);
            else
                return qroupedByInvoice;
        }

        /// <summary>
        /// Return receipts (sales) for specific account type, PosId and day
        /// </summary>
        /// <param name="endOfDayId"></param>
        /// <param name="posInfoId"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetPosSalesByAccountType( long endOfDayId, long? posInfoId, int accountType ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId, w => ( w.EndOfDayId ?? 0 ) == 0 && w.PosInfoId == posInfoId && w.InvoiceType != 2 && w.IsVoided == false && w.InvoiceType != 3 && w.InvoiceType != 8 && w.InvoiceType != 11 && w.InvoiceType != 12);
            var qroupedByInvoice = GetAuditGroupByInvoice(query, null, accountType).ToList();

            return qroupedByInvoice;
        }

        /// <summary>
        /// Return not audited sales for specific staff by PosId and day
        /// </summary>
        /// <param name="endOfDayId"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetPosSalesPerStaff( long? endOfDayId, long? posInfoId ) {
            var query = GetAuditFlatInvoicesBase(endOfDayId ?? 0, w => ( w.EndOfDayId ?? 0 ) == 0 && w.PosInfoId == posInfoId && !( w.InvoiceType == 2 && w.IsInvoiced ) && w.InvoiceType != 8 && w.InvoiceType != 11 && w.InvoiceType != 12 /*&& (w.InvoiceType != 3 && w.IsVoided == false) || w.InvoiceType == 3 && w.IsVoided == false*/);//.ToList();

            var qroupedByInvoice = query//.Where(w => (w.InvoiceType != 3 && w.IsVoided == false) || w.InvoiceType == 3 && w.IsVoided == false) 
                            .ToList()
                            .GroupBy(g => g.InvoiceId)
                            //.Where(w=>w.Any(a=>a.AccountType == 2))
                            .Select(s => new {
                                Day = s.FirstOrDefault().Day,
                                InvoiceId = s.FirstOrDefault().InvoiceId,
                                OrderNo = s.FirstOrDefault().OrderNo,
                                Abbreviation = s.FirstOrDefault().InvoiceAbbreviation,
                                ReceiptNo = s.FirstOrDefault().Counter,
                                InvoiceType = s.FirstOrDefault().InvoiceType,
                                IsPaid = s.FirstOrDefault().IsPaid,
                                InvoiceTotal = s.FirstOrDefault().InvoiceType == 3 ? s.FirstOrDefault().InvoiceTotal * -1 : s.FirstOrDefault().InvoiceTotal,
                                InvoiceDiscount = s.FirstOrDefault().InvoiceDiscount,
                                InvoicePaidAmount = s.FirstOrDefault().InvoicePaidAmount ?? 0,
                                IsInvoiced = s.FirstOrDefault().IsInvoiced,
                                IsVoided = s.FirstOrDefault().IsVoided,
                                Vat1 = s.Where(w => w.VatId == 1).Select(ss => new {
                                    VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId
                                }).Distinct().Sum(sm => sm.Total),
                                Vat2 = s.Where(w => w.VatId == 2).Select(ss => new {
                                    VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId
                                }).Distinct().Sum(sm => sm.Total),
                                Vat3 = s.Where(w => w.VatId == 3).Select(ss => new {
                                    VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId
                                }).Distinct().Sum(sm => sm.Total),
                                Vat4 = s.Where(w => w.VatId == 4).Select(ss => new {
                                    VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId
                                }).Distinct().Sum(sm => sm.Total),
                                Vat5 = s.Where(w => w.VatId == 5).Select(ss => new {
                                    VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId
                                }).Distinct().Sum(sm => sm.Total),
                                AccountType1 = s.Where(w => w.AccountType == 1 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType2 = s.Where(w => w.AccountType == 2 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType3 = s.Where(w => w.AccountType == 3 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType4 = s.Where(w => w.AccountType == 4 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType5 = s.Where(w => w.AccountType == 5 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType6 = s.Where(w => w.AccountType == 6 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType7 = s.Where(w => w.AccountType == 7 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType8 = s.Where(w => w.AccountType == 8 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType9 = s.Where(w => w.AccountType > 8 && w.InvoiceType != 3 && w.IsVoided == false).Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount, TransctionId = ss.TransctionId
                                }).Distinct().Sum(sm => sm.TransAmount),
                                AccountType1ReceiptCount = s.Where(w => w.AccountType == 1 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType2ReceiptCount = s.Where(w => w.AccountType == 2 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType3ReceiptCount = s.Where(w => w.AccountType == 3 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType4ReceiptCount = s.Where(w => w.AccountType == 4 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType5ReceiptCount = s.Where(w => w.AccountType == 5 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType6ReceiptCount = s.Where(w => w.AccountType == 6 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType7ReceiptCount = s.Where(w => w.AccountType == 7 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType8ReceiptCount = s.Where(w => w.AccountType == 8 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                AccountType9ReceiptCount = s.Where(w => w.AccountType > 8 && w.InvoiceType != 3 && w.IsVoided == false).Count() > 0 ? 1 : 0,
                                TransAmount = s.Select(ss => new {
                                    AccountId = ss.AccountId, TransAmount = ss.TransAmount
                                }).Distinct().Sum(sm => sm.TransAmount),

                                ItemsCount = s.Count(),
                                //  TransAmount = s.Select(ss => new { AccountId = ss.AccountId, TransAmount = ss.TransAmount }).Distinct().Sum(sm => sm.TransAmount),
                                TransStaffId = s.Where(w => w.TransStaff != w.StaffId).Select(ss => ss.TransStaff).Distinct(),
                                StaffId = s.FirstOrDefault().StaffId,
                                FiscalType = s.FirstOrDefault().FiscalType,
                                FODay = s.FirstOrDefault().FODay,
                                EndOfDayId = s.FirstOrDefault().EndOfDayId,
                                //Covers = s.FirstOrDefault().Covers,
                                PaidFromOtherStaffAmount = s.Where(w => w.TransStaff != w.StaffId).Sum(ss => ss.TransAmount),
                                Check = s.FirstOrDefault().InvoiceTotal - s.Select(ss => new {
                                    VatId = ss.VatId, Total = ss.Total, OrderDetailId = ss.OrderDetailId
                                }).Distinct().Sum(sm => sm.Total),
                                CardAnalysis = s.Where(w => w.AccountType == 4).Select(ss => new {
                                    AccountId = ss.AccountId,
                                    TransAmount = ss.TransAmount
                                })
                                                    .Distinct()
                                                    .GroupBy(g => g.AccountId).Select(ssss => new AuditCreditCardsAmounts {
                                                        AccountId = ssss.Key,
                                                        TransAmount = ssss.Sum(sm => sm.TransAmount)
                                                    }).ToList(),
                                Room = s.FirstOrDefault().Room,
                                Tablecode = s.FirstOrDefault().TableCode
                            });


            var staffOtherTrans = ( from q in DbContext.Transactions.Where(w => w.EndOfDayId == null && w.PosInfoId == posInfoId && ( w.TransactionType != 3 && w.TransactionType != 4 && w.TransactionType != 7 ))
                                    join qq in DbContext.Accounts on q.AccountId equals qq.Id
                                    select new {
                                        AccountType = qq.Type,
                                        Amount = q.Amount,
                                        InOut = q.InOut,
                                        StaffId = q.StaffId,
                                        AccoutId = q.AccountId
                                    } )
                                   .GroupBy(g => g.StaffId)
                                   .Select(s => new {
                                       StaffId = s.Key,
                                       CashDrawerIn = s.Where(w => w.InOut == 0).Sum(sm => sm.Amount),
                                       CashDrawerOut = s.Where(w => w.InOut == 1).Sum(sm => sm.Amount),
                                       AccountType1 = s.Where(w => w.AccountType == 1).Sum(sm => sm.Amount),
                                       AccountType2 = s.Where(w => w.AccountType == 2).Sum(sm => sm.Amount),
                                       AccountType3 = s.Where(w => w.AccountType == 3).Sum(sm => sm.Amount),
                                       AccountType4 = s.Where(w => w.AccountType == 4).Sum(sm => sm.Amount),
                                       AccountType5 = s.Where(w => w.AccountType == 5).Sum(sm => sm.Amount),
                                       AccountType6 = s.Where(w => w.AccountType == 7).Sum(sm => sm.Amount),
                                       AccountType7 = s.Where(w => w.AccountType == 7).Sum(sm => sm.Amount),
                                       AccountType8 = s.Where(w => w.AccountType == 8).Sum(sm => sm.Amount),
                                       AccountType9 = s.Where(w => w.AccountType > 8).Sum(sm => sm.Amount),
                                       CreditCards = s.Where(w => w.AccountType == 4).Select(ss => new {
                                           AccountId = ss.AccoutId,
                                           Amount = ss.Amount
                                       }).Distinct()
                                       .GroupBy(g => g.AccountId).Select(ssss => new AuditCreditCardsAmounts {
                                           AccountId = ssss.Key,
                                           TransAmount = ssss.Sum(sm => sm.Amount)
                                       }).ToList(),
                                   });

            var staffCovers = (from q in DbContext.Order.Where(w => w.EndOfDayId == null && w.PosId == posInfoId)
                                select new
                                {
                                    StaffId = q.StaffId,
                                    Couver = q.Couver
                                })
                               .GroupBy(g => g.StaffId)
                               .Select(s => new {
                                   StaffId = s.Key,
                                   Cover = s.Sum(sm => sm.Couver)
                               });
                               


           var dayAudit = ( from q in qroupedByInvoice.ToList().Where(w => w.InvoiceType != 2 || ( w.InvoiceType == 2 && w.IsInvoiced == false ))
                            join t in staffOtherTrans on q.StaffId equals t.StaffId into f
                            from tr in f.DefaultIfEmpty()
                            //join sc in staffCovers on q.StaffId equals sc.StaffId into ff
                            //from scv in ff.DefaultIfEmpty()
                            select new {
                                 Day = q.Day,
                                 InvoiceId = q.InvoiceId,
                                 OrderNo = q.OrderNo,
                                 Abbreviation = q.Abbreviation,
                                 ReceiptNo = q.ReceiptNo,
                                 InvoiceType = q.InvoiceType,
                                 IsPaid = q.IsPaid,
                                 InvoiceTotal = q.InvoiceTotal,
                                 InvoiceDiscount = q.InvoiceDiscount,
                                 InvoicePaidAmount = q.InvoicePaidAmount,
                                 //Covers = scv != null ? scv.Cover : 0,
                                 IsInvoiced = q.IsInvoiced,
                                 IsVoided = q.IsVoided,
                                 Vat1 = q.Vat1,
                                 Vat2 = q.Vat2,
                                 Vat3 = q.Vat3,
                                 Vat4 = q.Vat4,
                                 Vat5 = q.Vat5,


                                 AccountType1 = q.AccountType1,
                                 AccountType2 = q.AccountType2,
                                 AccountType3 = q.AccountType3,
                                 AccountType4 = q.AccountType4,
                                 AccountType5 = q.AccountType5,
                                 AccountType6 = q.AccountType6,
                                 AccountType7 = q.AccountType7,
                                 AccountType8 = q.AccountType8,
                                 AccountType9 = q.AccountType9,
                                 AccountType1ReceiptCount = q.AccountType1ReceiptCount,
                                 AccountType2ReceiptCount = q.AccountType2ReceiptCount,
                                 AccountType3ReceiptCount = q.AccountType3ReceiptCount,
                                 AccountType4ReceiptCount = q.AccountType4ReceiptCount,
                                 AccountType5ReceiptCount = q.AccountType4ReceiptCount,
                                 AccountType6ReceiptCount = q.AccountType5ReceiptCount,
                                 AccountType7ReceiptCount = q.AccountType5ReceiptCount,
                                 AccountType8ReceiptCount = q.AccountType8ReceiptCount,
                                 AccountType9ReceiptCount = q.AccountType9ReceiptCount,

                                 ReturnedAccountType1 = tr != null ? tr.AccountType1 : 0,
                                 ReturnedAccountType2 = tr != null ? tr.AccountType2 : 0,
                                 ReturnedAccountType3 = tr != null ? tr.AccountType3 : 0,
                                 ReturnedAccountType4 = tr != null ? tr.AccountType4 : 0,
                                 ReturnedAccountType5 = tr != null ? tr.AccountType5 : 0,
                                 ReturnedAccountType6 = tr != null ? tr.AccountType6 : 0,
                                 ReturnedAccountType7 = tr != null ? tr.AccountType7 : 0,
                                 ReturnedAccountType8 = tr != null ? tr.AccountType8 : 0,
                                 ReturnedAccountType9 = tr != null ? tr.AccountType9 : 0,
                                 ReturnedCreditCards = tr != null ? tr.CreditCards : null,

                                 TransAmount = q.TransAmount,
                                 TransStaffId = q.TransStaffId,
                                 ItemsCount = q.ItemsCount,
                                 StaffId = q.StaffId,
                                 FiscalType = q.FiscalType,
                                 FODay = q.FODay,
                                 EndOfDayId = q.EndOfDayId,
                                 PaidFromOtherStaffAmount = q.PaidFromOtherStaffAmount,
                                 Check = q.Check,
                                 CardAnalysis = q.CardAnalysis,
                                 CashDrawerIn = tr != null ? tr.CashDrawerIn : 0,
                                 CashDrawerOut = tr != null ? tr.CashDrawerOut : 0,
                                 Room = q.Room,
                                 TableCode = q.Tablecode
                             } )
                            .GroupBy(g => new {
                                g.StaffId, g.InvoiceId
                            })
                                   .Select(s => new {
                                       Day = s.FirstOrDefault().Day,
                                       StaffId = s.Key.StaffId,
                                       Total = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().InvoiceTotal : 0,
                                       ReceiptCount = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? 1 : 0,//s.Where(w=>w.InvoiceType != 2).Distinct().Count(),
                                       NotInvoicedAmount = s.Where(w => w.InvoiceType == 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType == 2).Distinct().FirstOrDefault().InvoiceTotal : 0,
                                       NotInvoicedReceiptsCount = s.Where(w => w.InvoiceType == 2).Distinct().Count(),
                                       NotPaidAmount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsPaid < 2).Distinct().FirstOrDefault() != null ?
                                                       s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsPaid < 2)
                                                        .GroupBy(g => g.InvoiceId).Sum(sss => sss.FirstOrDefault().InvoiceTotal - sss.FirstOrDefault().InvoicePaidAmount) : (decimal?) 0,


                                       // s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsPaid < 2).Distinct().FirstOrDefault().InvoiceTotal
                                       //- s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsPaid < 2).Distinct().FirstOrDefault().InvoicePaidAmount : 0,
                                       NotPaidReceiptsCount = s.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsPaid < 2).Distinct().Count(),
                                       Voids = s.Where(w => w.InvoiceType == 3).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType == 3).Distinct().FirstOrDefault().InvoiceTotal : 0,
                                       VoidsReceiptsCount = s.Where(w => w.InvoiceType == 3).Distinct().Count(),

                                       Discount = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().InvoiceDiscount : 0,
                                       //Covers = s.Sum(sm => sm.Covers),
                                       //Covers = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().Covers : 0,

                                       Vat1 = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().Vat1 : 0,
                                       Vat2 = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().Vat2 : 0,
                                       Vat3 = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().Vat3 : 0,
                                       Vat4 = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().Vat4 : 0,
                                       Vat5 = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().Vat5 : 0,

                                       AccountType1 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType1 : 0,
                                       AccountType2 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType2 : 0,
                                       AccountType3 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType3 : 0,
                                       AccountType4 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType4 : 0,
                                       AccountType5 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType5 : 0,
                                       AccountType6 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType6 : 0,
                                       AccountType7 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType7 : 0,
                                       AccountType8 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType8 : 0,
                                       AccountType9 = s.Where(w => w.InvoiceType != 2 && w.IsVoided == false).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().AccountType9 : 0,

                                       AccountType1ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType1ReceiptCount : 0,
                                       AccountType2ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType2ReceiptCount : 0,
                                       AccountType3ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType3ReceiptCount : 0,
                                       AccountType4ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType4ReceiptCount : 0,
                                       AccountType5ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType5ReceiptCount : 0,
                                       AccountType6ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType6ReceiptCount : 0,
                                       AccountType7ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType7ReceiptCount : 0,
                                       AccountType8ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType8ReceiptCount : 0,
                                       AccountType9ReceiptCount = s.FirstOrDefault() != null ? s.FirstOrDefault().AccountType9ReceiptCount : 0,

                                       ReturnedAccountType1 = s.Sum(sm => sm.ReturnedAccountType1),
                                       ReturnedAccountType2 = s.Sum(sm => sm.ReturnedAccountType2),
                                       ReturnedAccountType3 = s.Sum(sm => sm.ReturnedAccountType3),
                                       ReturnedAccountType4 = s.Sum(sm => sm.ReturnedAccountType4),
                                       ReturnedAccountType5 = s.Sum(sm => sm.ReturnedAccountType5),
                                       ReturnedAccountType6 = s.Sum(sm => sm.ReturnedAccountType6),
                                       ReturnedAccountType7 = s.Sum(sm => sm.ReturnedAccountType7),
                                       ReturnedAccountType8 = s.Sum(sm => sm.ReturnedAccountType8),
                                       ReturnedAccountType9 = s.Sum(sm => sm.ReturnedAccountType9),
                                       // ReturnedCreditCards = s.SelectMany(sm => sm.ReturnedCreditCards),
                                       ReturnedCreditCards = s.Where(w => w.ReturnedCreditCards != null).SelectMany(sm => sm.ReturnedCreditCards),


                                       //CardAnalysis = s.FirstOrDefault().CardAnalysis,
                                       CardAnalysis = s.SelectMany(sm => sm.CardAnalysis).GroupBy(gg => gg.AccountId).Select(ss1 => new AuditCreditCardsAmounts() { AccountId = ss1.Key, TransAmount = ss1.Sum(sm => sm.TransAmount) }),
                                       PaidFromOtherStaff = s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2).Distinct().FirstOrDefault().PaidFromOtherStaffAmount : 0,

                                       WholeSalesTotal = s.Where(w => w.InvoiceType != 2 && w.FiscalType == 1).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2 && w.FiscalType == 1).Distinct().FirstOrDefault().InvoiceTotal : 0,
                                       ReatailSalesTotal = s.Where(w => w.InvoiceType != 2 && w.FiscalType == 0).Distinct().FirstOrDefault() != null ? s.Where(w => w.InvoiceType != 2 && w.FiscalType == 0).Distinct().FirstOrDefault().InvoiceTotal : 0,
                                       CashDrawerIn = s.FirstOrDefault() != null ? s.FirstOrDefault().CashDrawerIn : 0,
                                       CashDrawerOut = s.FirstOrDefault() != null ? s.FirstOrDefault().CashDrawerOut : 0,
                                       Room = s.FirstOrDefault().Room,
                                       TableCode = s.FirstOrDefault().TableCode
                                   })
                                  .GroupBy(g => g.StaffId)
                                  .Select(s => new {
                                      StaffId = s.Key,
                                      Total = s.Sum(sm => sm.Total),
                                      ReceiptCount = s.Sum(sm => sm.ReceiptCount),
                                      NotInvoicedAmount = s.Sum(sm => sm.NotInvoicedAmount),
                                      NotInvoicedReceiptsCount = s.Sum(sm => sm.NotInvoicedReceiptsCount),
                                      NotPaidAmount = s.Sum(sm => sm.NotPaidAmount),
                                      NotPaidReceiptsCount = s.Sum(sm => sm.NotPaidReceiptsCount),
                                      Voids = s.Sum(sm => sm.Voids),
                                      VoidsReceiptsCount = s.Sum(sm => sm.VoidsReceiptsCount),

                                      Discount = s.Sum(sm => sm.Discount),
                                      //Covers = s.Sum(sm => sm.Covers),

                                      Vat1 = s.Sum(sm => sm.Vat1),
                                      Vat2 = s.Sum(sm => sm.Vat2),
                                      Vat3 = s.Sum(sm => sm.Vat3),
                                      Vat4 = s.Sum(sm => sm.Vat4),
                                      Vat5 = s.Sum(sm => sm.Vat5),

                                      AccountType1 = s.Sum(sm => sm.AccountType1),
                                      AccountType2 = s.Sum(sm => sm.AccountType2),
                                      AccountType3 = s.Sum(sm => sm.AccountType3),
                                      AccountType4 = s.Sum(sm => sm.AccountType4),
                                      AccountType5 = s.Sum(sm => sm.AccountType5),
                                      AccountType6 = s.Sum(sm => sm.AccountType6),
                                      AccountType7 = s.Sum(sm => sm.AccountType7),
                                      AccountType8 = s.Sum(sm => sm.AccountType8),
                                      AccountType9 = s.Sum(sm => sm.AccountType9),

                                      AccountType1ReceiptCount = s.Sum(sm => sm.AccountType1ReceiptCount),
                                      AccountType2ReceiptCount = s.Sum(sm => sm.AccountType2ReceiptCount),
                                      AccountType3ReceiptCount = s.Sum(sm => sm.AccountType3ReceiptCount),
                                      AccountType4ReceiptCount = s.Sum(sm => sm.AccountType4ReceiptCount),
                                      AccountType5ReceiptCount = s.Sum(sm => sm.AccountType5ReceiptCount),
                                      AccountType6ReceiptCount = s.Sum(sm => sm.AccountType6ReceiptCount),
                                      AccountType7ReceiptCount = s.Sum(sm => sm.AccountType7ReceiptCount),
                                      AccountType8ReceiptCount = s.Sum(sm => sm.AccountType8ReceiptCount),
                                      AccountType9ReceiptCount = s.Sum(sm => sm.AccountType9ReceiptCount),

                                      ReturnedAccountType1 = s.FirstOrDefault().ReturnedAccountType1,
                                      ReturnedAccountType2 = s.FirstOrDefault().ReturnedAccountType2,
                                      ReturnedAccountType3 = s.FirstOrDefault().ReturnedAccountType3,
                                      ReturnedAccountType4 = s.FirstOrDefault().ReturnedAccountType4,
                                      ReturnedAccountType5 = s.FirstOrDefault().ReturnedAccountType5,
                                      ReturnedAccountType6 = s.FirstOrDefault().ReturnedAccountType6,
                                      ReturnedAccountType7 = s.FirstOrDefault().ReturnedAccountType7,
                                      ReturnedAccountType8 = s.FirstOrDefault().ReturnedAccountType8,
                                      ReturnedAccountType9 = s.FirstOrDefault().ReturnedAccountType9,
                                      ReturnedCreditCards = s.FirstOrDefault().ReturnedCreditCards,

                                      CardAnalysis = s.SelectMany(sm => sm.CardAnalysis).GroupBy(g => g.AccountId).Select(ss1 => new AuditCreditCardsAmounts() {
                                          AccountId = ss1.Key,
                                          TransAmount = ss1.Sum(sm => sm.TransAmount),
                                          TransCount = ss1.Count()
                                      }),
                                      PaidFromOtherStaff = s.Sum(sm => sm.PaidFromOtherStaff),

                                      WholeSalesTotal = s.Sum(sm => sm.WholeSalesTotal),
                                      ReatailSalesTotal = s.Sum(sm => sm.ReatailSalesTotal),
                                      CashDrawerIn = s.Sum(sm => sm.AccountType1) + ( s.FirstOrDefault().CashDrawerIn ?? 0 ),
                                      CashDrawerOut = s.FirstOrDefault().CashDrawerOut,
                                      Room = s.FirstOrDefault().Room,
                                      TableCode = s.FirstOrDefault().TableCode
                                  });

            var staffData = (from q in dayAudit.ToList()
                             join t in staffCovers on q.StaffId equals t.StaffId into f
                             from tr in f.DefaultIfEmpty()
                             select new
                             {
                                 StaffId = q.StaffId,
                                 Total = q.Total,
                                 ReceiptCount = q.ReceiptCount,
                                 NotInvoicedAmount = q.NotInvoicedAmount,
                                 NotInvoicedReceiptsCount = q.NotInvoicedReceiptsCount,
                                 NotPaidAmount = q.NotPaidAmount,
                                 NotPaidReceiptsCount = q.NotPaidReceiptsCount,
                                 Voids = q.Voids,
                                 VoidsReceiptsCount = q.VoidsReceiptsCount,

                                 Discount = q.Discount,
                                 Covers = tr != null ? tr.Cover : 0,

                                 Vat1 = q.Vat1,
                                 Vat2 = q.Vat2,
                                 Vat3 = q.Vat3,
                                 Vat4 = q.Vat4,
                                 Vat5 = q.Vat5,

                                 AccountType1 = q.AccountType1,
                                 AccountType2 = q.AccountType2,
                                 AccountType3 = q.AccountType3,
                                 AccountType4 = q.AccountType4,
                                 AccountType5 = q.AccountType5,
                                 AccountType6 = q.AccountType6,
                                 AccountType7 = q.AccountType7,
                                 AccountType8 = q.AccountType8,
                                 AccountType9 = q.AccountType9,

                                 AccountType1ReceiptCount = q.AccountType1ReceiptCount,
                                 AccountType2ReceiptCount = q.AccountType2ReceiptCount,
                                 AccountType3ReceiptCount = q.AccountType3ReceiptCount,
                                 AccountType4ReceiptCount = q.AccountType4ReceiptCount,
                                 AccountType5ReceiptCount = q.AccountType5ReceiptCount,
                                 AccountType6ReceiptCount = q.AccountType6ReceiptCount,
                                 AccountType7ReceiptCount = q.AccountType7ReceiptCount,
                                 AccountType8ReceiptCount = q.AccountType8ReceiptCount,
                                 AccountType9ReceiptCount = q.AccountType9ReceiptCount,

                                 ReturnedAccountType1 = q.ReturnedAccountType1,
                                 ReturnedAccountType2 = q.ReturnedAccountType2,
                                 ReturnedAccountType3 = q.ReturnedAccountType3,
                                 ReturnedAccountType4 = q.ReturnedAccountType4,
                                 ReturnedAccountType5 = q.ReturnedAccountType5,
                                 ReturnedAccountType6 = q.ReturnedAccountType6,
                                 ReturnedAccountType7 = q.ReturnedAccountType7,
                                 ReturnedAccountType8 = q.ReturnedAccountType8,
                                 ReturnedAccountType9 = q.ReturnedAccountType9,
                                 ReturnedCreditCards = q.ReturnedCreditCards,

                                 CardAnalysis = q.CardAnalysis,
                                 PaidFromOtherStaff = q.PaidFromOtherStaff,

                                 WholeSalesTotal = q.WholeSalesTotal,
                                 ReatailSalesTotal = q.ReatailSalesTotal,
                                 CashDrawerIn = q.CashDrawerIn,
                                 CashDrawerOut = q.CashDrawerOut,
                                 Room = q.Room,
                                 TableCode = q.TableCode
                             });

            return staffData;

        }

        public IEnumerable<dynamic> GetConsumedMeals( long? endOfDayId, long? departmentId, long? posInfoId ) {
            var query = from q in DbContext.MealConsumption
                        join qq in DbContext.Guest on q.GuestId equals qq.Id
                        join qqq in DbContext.Department on q.DepartmentId equals qqq.Id
                        join qqqq in DbContext.PosInfo on q.PosInfoId equals qqqq.Id
                        where ( q.EndOfDayId ?? 0 ) == 0 && q.DepartmentId == departmentId
                        select new {
                            EndOfDayId = q.EndOfDayId,
                            DepartmentId = q.DepartmentId,
                            DepartmentDescription = qqq.Description,
                            PosInfoId = q.PosInfoId,
                            PosInfoDescription = qqqq.Description,
                            GuestName = qq.FirstName,
                            GuestLastname = qq.LastName,
                            BoardCode = qq.BoardCode,
                            Reservation = qq.ReservationCode,
                            ConsumedMeals = q.ConsumedMeals,
                        };

            if ( posInfoId != null )
                query = query.Where(w => w.PosInfoId == posInfoId);
            return query;
        }


        private IEnumerable<AuditSalesPerProductBase> GetSalesPerProductBase( long endOfDayId, long? departmentId, long? posInfoId ) {
            var query = from q in br.ReceiptDetailsBO(x => x.EndOfDayId == null)
                        join qq in br.ReceiptsBO(x => x.EndOfDayId == 0) on q.ReceiptsId equals qq.Id
                        join qqq in DbContext.Pricelist on q.PricelistId equals qqq.Id
                        join pc in DbContext.ProductCategories on q.ProductCategoryId equals pc.Id
                        join c in DbContext.Categories on pc.CategoryId equals c.Id
                        join st in DbContext.SalesType on q.SalesTypeId equals st.Id
                        //join st in Staff on qqq.StaffId equals st.Id
                        where q.EndOfDayId == null && q.InvoiceType != 2 //&& (qqq.IsVoided ?? false) == false
                        select new AuditSalesPerProductBase {
                            Day = qq.Day,
                            PosInfoId = q.PosInfoId,
                            //					DepartmentId = qqq.
                            ProductId = q.ProductId,
                            ProductCode = q.ItemCode,
                            Description = q.ItemDescr,
                            InvoiceId = q.ReceiptsId,
                            ProductCategory = pc.Description,
                            Pricelist = qqq.Description,
                            Category = c.Description,
                            IsVoided = qq.IsVoided,
                            StaffName = qq.StaffName,
                            StaffCode = qq.StaffCode,
                            Qty = q.ItemQty,
                            Price = q.Price,
                            Gross = q.TotalBeforeDiscount,
                            Discount = q.ItemDiscount,
                            IsExtra = q.IsExtra,
                            SalesTypeId = st.Id,
                            SalesTypeDescription = st.Description
                        };
            if ( departmentId != null ) {
                query = query.Where(w => w.DepartmentId == departmentId);
            }
            if ( posInfoId != null ) {
                query = query.Where(w => w.PosInfoId == posInfoId);
            }
            //query.Dump();
            return query;
        }

        public IEnumerable<Object> GetSalesPerProductByCategories( long endOfDayId, long? departmentId, long? posInfoId ) {
            var baseQuery = GetSalesPerProductBase(0, departmentId, posInfoId).ToList();

            var query = baseQuery.GroupBy(g => new {
                g.ProductId, g.Category, g.ProductCategory
            }).Select(s => new {
                Category = s.Key.Category,
                ProductCategory = s.Key.ProductCategory,
                ProductCode = s.FirstOrDefault().ProductCode,
                Description = s.FirstOrDefault().Description,
                ReceiptCount = s.GroupBy(gg => gg.InvoiceId).Count(),
                Qty = s.Sum(sm => sm.Qty),
                MinPrice = s.Min(ss => ss.Price),
                MaxPrice = s.Max(ss => ss.Price),
                Discount = s.Sum(sm => sm.Discount),
                Total = s.Sum(sm => sm.Gross)
            });

            return query;
        }

        public IEnumerable<Object> GetSalesPerProductByPricelists( long endOfDayId, long? departmentId, long? posInfoId ) {
            var baseQuery = GetSalesPerProductBase(0, departmentId, posInfoId).ToList();

            var query = baseQuery.ToList().GroupBy(g => new {
                g.ProductId, g.Pricelist
            }).Select(s => new {
                Pricelist = s.Key.Pricelist,
                ProductCode = s.FirstOrDefault().ProductCode,
                Description = s.FirstOrDefault().Description,
                ReceiptCount = s.GroupBy(gg => gg.InvoiceId).Count(),
                Qty = s.Sum(sm => sm.Qty),
                MinPrice = s.Min(ss => ss.Price),
                MaxPrice = s.Max(ss => ss.Price),
                Discount = s.Sum(sm => sm.Discount),
                Total = s.Sum(sm => sm.Gross),

            });

            return query;
        }

        public IEnumerable<Object> GetHourlySalesPerProduct( long endOfDayId, long? departmentId, long? posInfoId ) {
            var baseQuery = GetSalesPerProductBase(0, departmentId, posInfoId).ToList();

            var query = baseQuery.ToList().GroupBy(g => new {
                g.Day.Value.Date, g.Day.Value.Hour
            }).Select(s => new {
                Day = s.Key.Date.ToShortDateString(),
                Hour = s.Key.Hour,
                //ProductCategory = s.Key.ProductCategory,
                ProductCode = s.FirstOrDefault().ProductCode,
                Description = s.FirstOrDefault().Description,
                ReceiptCount = s.GroupBy(gg => gg.InvoiceId).Count(),
                Qty = s.Sum(sm => sm.Qty),
                MinPrice = s.Min(ss => ss.Price),
                MaxPrice = s.Max(ss => ss.Price),
                Discount = s.Sum(sm => sm.Discount),
                Total = s.Sum(sm => sm.Gross),
                IsExtra = s.FirstOrDefault().IsExtra,
            });

            return query;
        }

        public IEnumerable<Object> GetHourlySalesPerProductSalesType( long endOfDayId, long? departmentId, long? posInfoId ) {
            var baseQuery = GetSalesPerProductBase(0, departmentId, posInfoId).ToList();

            var query = baseQuery.ToList().GroupBy(g => new {
                g.Day.Value.Date, g.Day.Value.Hour, g.SalesTypeId
            }).Select(s => new {
                Day = s.Key.Date.ToShortDateString(),
                Hour = s.Key.Hour,
                SalesTypeDescription = s.FirstOrDefault().SalesTypeDescription,
                //ProductCategory = s.Key.ProductCategory,
                ProductCode = s.FirstOrDefault().ProductCode,
                Description = s.FirstOrDefault().Description,
                ReceiptCount = s.GroupBy(gg => gg.InvoiceId).Count(),
                Qty = s.Sum(sm => sm.Qty),
                MinPrice = s.Min(ss => ss.Price),
                MaxPrice = s.Max(ss => ss.Price),
                Discount = s.Sum(sm => sm.Discount),
                Total = s.Sum(sm => sm.Gross),
                IsExtra = s.FirstOrDefault().IsExtra,
            });

            return query;
        }

        private EndOfDay GetEODData( long posInfoId ) {
            var query = from q in br.ReceiptDetailsBO(x => x.PosInfoId == posInfoId && x.EndOfDayId == null)
                        join qq in br.ReceiptsBO(x => x.PosInfoId == posInfoId && x.EndOfDayId == 0) on q.ReceiptsId equals qq.Id
                        join qqq in br.ReceiptPaymentsFlat(x => x.PosInfoId == posInfoId && x.EndOfDayId == 0) on q.ReceiptsId equals qqq.InvoicesId into f
                        from tr in f.DefaultIfEmpty()
                        select new {
                            InvoiceId = q.ReceiptsId,
                            OrderNo = q.OrderNo,
                            Counter = qq.ReceiptNo,
                            Covers = qq.Cover,
                            InvoiceAbbreviation = q.Abbreviation,
                            InvoiceType = q.InvoiceType,
                            InvoiceTotal = qq.Total,
                            InvoiceDiscount = qq.Discount,
                            //InvoicePaidAmount = qq.PaidTotal,
                            IsPaid = qq.IsPaid,
                            IsInvoiced = qq.IsInvoiced,
                            IsVoided = qq.IsVoided,
                            VatId = q.VatId,
                            Total = q.ItemGross,
                            VatAmount = q.ItemVatValue,
                            VatRate = q.ItemVatRate,
                            Net = q.ItemNet,
                            AccountId = tr != null ? tr.AccountId : 0,
                            AccountDesc = tr != null ? tr.AccountDescription : "",
                            AccountType = tr != null ? tr.AccountType : 0,
                            TransAmount = tr != null ? tr.Amount : 0,
                            TransStaff = tr.StaffId,
                            TransactionType = tr != null ? tr.TransactionType : null,
                            StaffId = qq.StaffId,
                            OrderDetailId = q.OrderDetailId,
                            //FiscalType = qq.FiscalType,
                            //FODay =qq.FODay,
                            EndOfDayId = q.EndOfDayId,
                            PosInfoId = q.PosInfoId,
                            ItemQty = q.ItemQty,
                            TaxId = q.TaxId,
                            TaxAmount = q.ItemTaxAmount,
                            Discount = q.ItemDiscount,
                            IsExtra = q.IsExtra
                        };
            // var akyr = query.Where(w => w.InvoiceType == 3).ToList();
            //var a = query.ToList().GroupBy(g => g.AccountId).Select(s => new { AccountId = s.Key, Tot = s.Sum(ss => ss.Total) }).ToList();
            var eodData = from q in query.ToList()
                          group q by q.InvoiceId into g
                          select new {
                              PosInfoId = g.FirstOrDefault().PosInfoId,
                              Gross = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false)
                                       .Select(ss => new {
                                           InvoiceId = ss.IsInvoiced, InvoiceTotal = ss.InvoiceTotal
                                       }).Distinct().Sum(sm => sm.InvoiceTotal),
                              Ticketscount = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false)
                                              .Select(ss => new {
                                                  InvoiceId = ss.IsInvoiced, InvoiceTotal = ss.InvoiceTotal
                                              })
                                              .Distinct().Count(),
                              ItemCount = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.ItemQty),
                              TicketAverage = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false).Average(avg => avg.Total),
                              Discount = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false)
                                          .Select(ss => new {
                                              InvoiceId = ss.IsInvoiced, InvoiceDiscount = ss.InvoiceDiscount
                                          })
                                          .Distinct().Sum(sm => sm.InvoiceDiscount),
                              EndOfDayDetail = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsVoided == false)
                                                .Select(sss => new {
                                                    InvoiceId = sss.InvoiceId,
                                                    VatId = sss.VatId,
                                                    VatRate = sss.VatRate,
                                                    VatAmount = sss.VatAmount,
                                                    TaxId = sss.TaxId,
                                                    TaxAmount = sss.TaxAmount,
                                                    Total = sss.Total,
                                                    OrderDetailId = sss.OrderDetailId,
                                                    IsExtra = sss.IsExtra,
                                                    Net = sss.Net,
                                                    Discount = sss.Discount,
                                                    InvoiceTotal = sss.InvoiceTotal
                                                }).Distinct()
                                                .GroupBy(gg => gg.VatId)
                                                .Select(ss => new {
                                                    InvoiceId = ss.FirstOrDefault().InvoiceId,
                                                    VatId = ss.Key,
                                                    VatRate = ss.FirstOrDefault().VatRate,
                                                    VatAmount = ss.Sum(sm => sm.VatAmount ?? 0),
                                                    TaxId = ss.FirstOrDefault().TaxId,
                                                    TaxAmount = ss.Sum(sm => sm.TaxAmount ?? 0),
                                                    Gross = ss.Sum(sm => sm.Total ?? 0),
                                                    Net = ss.Sum(sm => sm.Net ?? 0),
                                                    Discount = ss.Sum(sm => sm.Discount ?? 0),
                                                    InvoiceTotal = ss.FirstOrDefault().InvoiceTotal
                                                }),
                              PaymentAnalysis = g.Where(w => w.InvoiceType != 8 && w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false)
                                                  .Select(ss => new {
                                                      InvoiceId = ss.InvoiceId,
                                                      AccountId = ss.AccountId,
                                                      TransAmount = ss.TransAmount,
                                                      Description = ss.AccountDesc,
                                                      AccountType = ss.AccountType
                                                  }).ToList().Distinct()
                                                  .GroupBy(gg => gg.AccountId).Select(ss => new {
                                                      AccountId = ss.Key,
                                                      Description = ss.FirstOrDefault().Description,
                                                      AccountType = ss.FirstOrDefault().AccountType,
                                                      TransAmount = ss.Distinct().Sum(sm => sm.TransAmount)
                                                  }).Where(w => w.TransAmount != 0),
                              // a = g.Where(w => w.InvoiceType == 3).Distinct().GroupBy(gg => gg.AccountId),
                              VoidsAnalysis = g.Where(w => w.InvoiceType == 3).Select(ss => new {
                                  InvoiceId = ss.InvoiceId,
                                  AccountId = ss.AccountId,
                                  TransAmount = ss.TransAmount,
                                  Description = ss.AccountDesc,
                                  AccountType = ss.AccountType
                              }).ToList().Distinct().Distinct().GroupBy(gg => gg.AccountId).Select(ss => new {
                                  AccountId = ss.Key,
                                  Description = ss.FirstOrDefault().Description,
                                  AccountType = ss.FirstOrDefault().AccountType,
                                  TransAmount = ss.Sum(sm => sm.TransAmount)
                              }),

                              DatTime = DateTime.Now
                          };

            if ( eodData.Count() == 0 ) {

                var pid = DbContext.PosInfo.FirstOrDefault(x => x.Id == posInfoId);
                var eod = new EndOfDay() {
                    PosInfoId = pid.Id,
                    Gross = 0,
                    Net = 0,
                    TicketAverage = 0,
                    ItemCount = 0,
                };
                return eod;
            }

            var barcodes = br.ReceiptPaymentsFlat(w => w.PosInfoId == posInfoId
                                                     && w.EndOfDayId == null
                                                     && w.TransactionType == 7
                                                     ).Sum(sm => sm.Amount) ?? 0;



            var totalLockers = br.ReceiptPaymentsFlat(w => w.EndOfDayId == null
                                                        && w.PosInfoId == posInfoId
                                                        && ( w.TransactionType == 6
                                                        || w.TransactionType == 7 ));
            decimal? openLocker = totalLockers.Where(w => w.TransactionType == 6).Sum(sm => sm.Amount) ?? 0;
            decimal? paidLocker = ( totalLockers.Where(w => w.TransactionType == 7).Sum(sm => sm.Amount) * -1 ) ?? 0;

            var temp = eodData.GroupBy(g => g.PosInfoId).Select(s => new {
                EndOfDayDetail = s.SelectMany(ss => ss.EndOfDayDetail).Sum(sm => sm.Gross)
                //.GroupBy(gg => gg.VatId).Select(s2 => new EndOfDayDetail
                //{
                //    VatId = s2.Key,
                //    VatRate = s2.FirstOrDefault().VatRate,
                //    VatAmount = s2.Sum(sm => sm.VatAmount),
                //    TaxId = s2.FirstOrDefault().TaxId,
                //    TaxAmount = s2.FirstOrDefault().TaxAmount,
                //    Gross = s2.Sum(sm => sm.Gross),
                //    Net = s2.Sum(sm => sm.Net),
                //    Discount = s2.Sum(sm => sm.Discount),
                //}).ToList(),
            });


            var final = eodData.GroupBy(g => g.PosInfoId).Select(s => new EndOfDay {
                PosInfoId = s.Key,
                Net = s.FirstOrDefault().EndOfDayDetail.Sum(sm => sm.Net),
                Gross = s.Sum(sm => sm.Gross),
                TicketsCount = s.Sum(sm => sm.Ticketscount),
                ItemCount = (int) s.Sum(sm => sm.ItemCount),
                TicketAverage = s.Sum(sm => sm.Ticketscount) > 0 ? s.Sum(sm => sm.Gross) / s.Sum(sm => sm.Ticketscount) : 0, //Math.Round(s.Average(sm=>(decimal)sm.TicketAverage),2, MidpointRounding.AwayFromZero),
                EndOfDayDetail = s.SelectMany(ss => ss.EndOfDayDetail).GroupBy(gg => gg.VatId).Select(s2 => new EndOfDayDetail {
                    VatId = s2.Key,
                    VatRate = s2.FirstOrDefault().VatRate,
                    VatAmount = s2.Sum(sm => sm.VatAmount),
                    TaxId = s2.FirstOrDefault().TaxId,
                    TaxAmount = s2.FirstOrDefault().TaxAmount,
                    Gross = s2.Sum(sm => sm.Gross),
                    Net = s2.Sum(sm => sm.Net),
                    Discount = s2.Sum(sm => sm.Discount),
                }).ToList(),
                //a= s.Select(sl=>sl.a),
                EndOfDayPaymentAnalysis = s.SelectMany(ss => ss.PaymentAnalysis).GroupBy(gg => gg.AccountId).Select(ss => new EndOfDayPaymentAnalysis {
                    AccountId = ss.Key,
                    Description = ss.FirstOrDefault().Description,
                    AccountType = ss.FirstOrDefault().AccountType,
                    Total = ss.Sum(sm => sm.TransAmount)
                }).ToList(),
                EndOfDayVoidsAnalysis = s.SelectMany(ss => ss.VoidsAnalysis).Where(w => w.AccountId != 0).GroupBy(gg => gg.AccountId).Select(ss => new EndOfDayVoidsAnalysis {
                    AccountId = ss.Key,
                    Description = ss.FirstOrDefault().Description,
                    AccountType = ss.FirstOrDefault().AccountType,
                    Total = ss.Sum(sm => sm.TransAmount)
                }).ToList(),
                Lockers = DbContext.Lockers.Where(x => x.EndOfDayId == null).ToList(),
                Barcodes = barcodes,
                Discount = s.Sum(sm => sm.Discount),
            }).SingleOrDefault();
            var unpaid = final.Gross - final.EndOfDayPaymentAnalysis.Sum(sm => sm.Total);
            if ( unpaid > 0 ) {
                final.EndOfDayPaymentAnalysis.Add(new EndOfDayPaymentAnalysis() { Id = 0, Total = unpaid, AccountType = 1, AccountId = 1, Description = "Cash" });
            }

            return final;
        }

        private dynamic ConvertEodDataToZModel( EndOfDay eodData, long posInfoId, bool isZ, bool isReprint = false ) {
            var pi = DbContext.PosInfo.FirstOrDefault(x => x.Id == posInfoId);
            DateTime FODayDate = DateTime.Now;
            int eodCounterForReprint = 0;
            if ( isZ == true ) {
                if ( pi.CloseId == null ) {
                    pi.CloseId = 1;
                } else {
                    if ( !isReprint ) {
                        pi.CloseId++;
                        var lastEODId = DbContext.EndOfDay.Max(eod => (long?)eod.Id) ?? 0;
                        eodData.Id = lastEODId + 1;
                    }
                    else
                    {
                        var pastEOD = DbContext.EndOfDay.FirstOrDefault(x => x.Id == eodData.Id);
                        if (pastEOD != null)
                        {
                            eodCounterForReprint = (int) pastEOD.CloseId;
                            if (pastEOD.dtDateTime != null)
                            {
                                FODayDate = (DateTime) pastEOD.dtDateTime;
                            }
                            else
                            {
                                FODayDate = (DateTime) pastEOD.FODay;
                            }
                        }
                    }
                }
                eodData.CloseId = !isReprint ? (int) pi.CloseId : eodCounterForReprint;

                if ( pi.ResetsReceiptCounter == true ) {
                    pi.ReceiptCount = 0;
                }
                pi.IsOpen = false;
                DbContext.Entry(pi).State = EntityState.Modified;
            }
            if ( eodData == null )
                return null;//Return Empty Model

            var vatAnalysis = eodData.EndOfDayDetail
                                      .Select(q => new {
                                          VatId = q.VatId,
                                          VatRate = q.VatRate,
                                          //VatAmount = (decimal)Math.Round((decimal)q.VatAmount, 2, MidpointRounding.AwayFromZero),
                                          //Net = (decimal)Math.Round((decimal)q.Net, 2, MidpointRounding.AwayFromZero),//(decimal)Math.round((decimal)q.Gross, 2, MidpointRounding.AwayFromZero) - (decimal)math.round((decimal)q.vatamount, 2, midpointrounding.awayfromzero) - (decimal)math.round((decimal)q.taxamount, 2, midpointrounding.awayfromzero),
                                          Tax = q.TaxId != null ? (decimal) Math.Round((decimal) q.TaxId, 2, MidpointRounding.AwayFromZero) : 0,
                                          Gross = (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero),
                                          VatAmount = (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero)
                                                    - EconomicHelper.DeVat(q.VatRate ?? 0, (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero)),
                                          Net = EconomicHelper.DeVat(q.VatRate ?? 0, (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero)),

                                      }).ToList();

            var xDataToPrint = new {
                EndOfDayId = eodData.Id,
                PosInfoId = posInfoId,
                Day = (isReprint == false) ? (Convert.ToDateTime(pi.FODay)).ToString("yyyy-MM-dd HH:mm:ss.sss") : FODayDate.ToString("yyyy-MM-dd HH:mm:ss.sss"),
                //Time = FODayDate.ToString("yyyy-MM-dd HH:mm:ss.sss"),
                dtDateTime = FODayDate.ToString("yyyy-MM-dd HH:mm:ss.sss"),
                //Day = (false == isReprint) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.sss") : FODayDate.ToString("yyyy-MM-dd HH:mm:ss.sss"),
                //dtDateTime = (false == isReprint) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.sss") : FODayDate.ToString("yyyy-MM-dd HH:mm:ss.sss"),
                PosCode = pi.Code,//Allazei sto webpos
                PosDescription = pi.Description,
                ReportNo = eodData.CloseId,//pi.CloseId,
                Gross = eodData.Gross,
                //VatAmount = eodData.EndOfDayDetail.Sum(sm => sm.VatAmount),
                //Net = eodData.EndOfDayDetail.Sum(sm => sm.Net),
                VatAmount = vatAnalysis.Sum(sm => sm.VatAmount),
                Net = vatAnalysis.Sum(sm => sm.Net),

                Discount = eodData.Discount,
                TicketCount = eodData.TicketsCount,
                ItemsCount = eodData.ItemCount,
                PaymentAnalysis = eodData.EndOfDayPaymentAnalysis.Where(a => a.AccountType != 4)
                                       .Select(w => new {
                                           Description = w.Description,
                                           Amount = Math.Round((decimal) w.Total, 2)
                                       }),
                vatanalysis = eodData.EndOfDayDetail
                                      .Select(q => new {
                                          VatId = q.VatId,
                                          VatRate = q.VatRate,
                                          //VatAmount = (decimal)Math.Round((decimal)q.VatAmount, 2, MidpointRounding.AwayFromZero),
                                          //Net = (decimal)Math.Round((decimal)q.Net, 2, MidpointRounding.AwayFromZero),//(decimal)Math.round((decimal)q.Gross, 2, MidpointRounding.AwayFromZero) - (decimal)math.round((decimal)q.vatamount, 2, midpointrounding.awayfromzero) - (decimal)math.round((decimal)q.taxamount, 2, midpointrounding.awayfromzero),
                                          Tax = q.TaxId != null ? (decimal) Math.Round((decimal) q.TaxId, 2, MidpointRounding.AwayFromZero) : 0,
                                          Gross = (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero),
                                          VatAmount = (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero)
                                                    - EconomicHelper.DeVat(q.VatRate ?? 0, (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero)),
                                          Net = EconomicHelper.DeVat(q.VatRate ?? 0, (decimal) Math.Round((decimal) q.Gross, 2, MidpointRounding.AwayFromZero)),

                                      }).ToList(),
                VoidAnalysis = eodData.EndOfDayVoidsAnalysis
                                     .GroupBy(q => q.AccountId)
                                     .Select(w => new
                                     {
                                         Description = w.FirstOrDefault().Description,
                                         Amount = Math.Round((decimal)w.Sum(r => (decimal?)r.Total), 2, MidpointRounding.AwayFromZero)
                                     }),
                CardAnalysis = eodData.EndOfDayPaymentAnalysis.Where(a => a.AccountType == 4)
                                       .GroupBy(f => f.AccountId)
                                       .Select(w => new {
                                           Description = w.FirstOrDefault().Description,
                                           Amount = Math.Round((decimal) w.Sum(r => (decimal?) r.Total), 2, MidpointRounding.AwayFromZero)
                                       }),
                Barcodes = eodData.Barcodes,
                Lockers = eodData.Lockers.FirstOrDefault()
                //ProductsForEODStats = productsForEODStats
            };



            return xDataToPrint;
        }

        public bool StartNewAuditedDay( long posInfoId, string foday ) {
            PosInfo posinfo = DbContext.PosInfo.FirstOrDefault(w => w.Id == posInfoId);
            if ( posinfo == null ) {
                return false;
            }
            bool isOpen = posinfo.IsOpen ?? false;
            if (isOpen)
            {
                DbContext.Entry(posinfo).State = EntityState.Modified;
                return false;
            }
            posinfo.FODay = DateTime.Parse(foday);
            posinfo.IsOpen = true;
            if ( posinfo.ResetsReceiptCounter == true ) {
                posinfo.ReceiptCount = 0;
            }
            DbContext.Entry(posinfo).State = EntityState.Modified;
            //foreach (var p in posinfo.PosInfoDetail)
            //{
            //    if (p.ResetsAfterEod == true)
            //    {
            //        p.Counter = 0;
            //    }
            //}
            return true;
        }


        /// <summary>
        /// For a given day return the list of z-reports
        /// </summary>
        /// <param name="foDay"></param>
        /// <returns></returns>
        public IEnumerable<Object> AvailableZReports( DateTime? foDay ) {

            return DbContext.EndOfDay.Include(i => i.PosInfo).Where(w => DbFunctions.TruncateTime(w.FODay) == DbFunctions.TruncateTime(foDay)).Select(s => new {
                EndOfDayId = s.Id,
                PosInfoId = s.PosInfoId,
                PosInfoDescription = s.PosInfo.Description,
                FODay = s.FODay,
                CloseId = s.CloseId,
                Total = s.Gross
            });
        }


        /// <summary>
        /// create a Z-report to (re)print
        /// </summary>
        /// <param name="posInfoId"></param>
        /// <param name="staffId"></param>
        /// <param name="storeId"></param>
        /// <param name="eodId"></param>
        /// <returns>Z-report Model</returns>
        public Object ReprintZ( long posInfoId, long staffId, string storeId, long eodId ) {
            var query = from q in br.ReceiptDetailsBO(x => x.PosInfoId == posInfoId && x.EndOfDayId == eodId)
                        join qq in br.ReceiptsBO(x => x.PosInfoId == posInfoId && x.EndOfDayId == eodId) on q.ReceiptsId equals qq.Id
                        join qqq in br.ReceiptPaymentsFlat(x => x.PosInfoId == posInfoId && x.EndOfDayId == eodId) on q.ReceiptsId equals qqq.InvoicesId into f
                        from tr in f.DefaultIfEmpty()
                        select new {
                            InvoiceId = q.ReceiptsId,
                            OrderNo = q.OrderNo,
                            Counter = qq.ReceiptNo,
                            Covers = qq.Cover,
                            InvoiceAbbreviation = q.Abbreviation,
                            InvoiceType = q.InvoiceType,
                            InvoiceTotal = qq.Total,
                            InvoiceDiscount = qq.Discount,
                            //InvoicePaidAmount = qq.PaidTotal,
                            IsPaid = qq.IsPaid,
                            IsInvoiced = qq.IsInvoiced,
                            IsVoided = qq.IsVoided,
                            VatId = q.VatId,
                            Total = q.ItemGross,
                            VatAmount = q.ItemVatValue,
                            VatRate = q.ItemVatRate,
                            Net = q.ItemNet,
                            AccountId = tr != null ? tr.AccountId : 0,
                            AccountDesc = tr != null ? tr.AccountDescription : "",
                            AccountType = tr != null ? tr.AccountType : 0,
                            TransAmount = tr != null ? tr.Amount : 0,
                            TransStaff = tr.StaffId,
                            TransactionType = tr != null ? tr.TransactionType : null,
                            StaffId = qq.StaffId,
                            OrderDetailId = q.OrderDetailId,
                            //FiscalType = qq.FiscalType,
                            //FODay =qq.FODay,
                            EndOfDayId = q.EndOfDayId,
                            PosInfoId = q.PosInfoId,
                            ItemQty = q.ItemQty,
                            TaxId = q.TaxId,
                            TaxAmount = q.ItemTaxAmount,
                            Discount = q.ItemDiscount,
                            IsExtra = q.IsExtra
                        };
            // var akyr = query.Where(w => w.InvoiceType == 3).ToList();
            //var a = query.ToList().GroupBy(g => g.AccountId).Select(s => new { AccountId = s.Key, Tot = s.Sum(ss => ss.Total) }).ToList();
            var tempeodData = from q in query.ToList()
                              group q by q.InvoiceId into g
                              select new {
                                  PosInfoId = g.FirstOrDefault().PosInfoId,
                                  Gross = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false)
                                           .Select(ss => new {
                                               InvoiceId = ss.IsInvoiced, InvoiceTotal = ss.InvoiceTotal
                                           }).Distinct().Sum(sm => sm.InvoiceTotal),
                                  Ticketscount = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false)
                                                  .Select(ss => new {
                                                      InvoiceId = ss.IsInvoiced, InvoiceTotal = ss.InvoiceTotal
                                                  })
                                                  .Distinct().Count(),
                                  ItemCount = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false).Sum(sm => sm.ItemQty),
                                  TicketAverage = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false).Average(avg => avg.Total),
                                  Discount = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 8 && w.InvoiceType != 3 && w.IsVoided == false)
                                              .Select(ss => new {
                                                  InvoiceId = ss.IsInvoiced, InvoiceDiscount = ss.InvoiceDiscount
                                              })
                                              .Distinct().Sum(sm => sm.InvoiceDiscount),
                                  EndOfDayDetail = g.Where(w => w.InvoiceType != 2 && w.InvoiceType != 3 && w.InvoiceType != 8 && w.IsVoided == false)
                                                    .Select(sss => new {
                                                        InvoiceId = sss.InvoiceId,
                                                        VatId = sss.VatId,
                                                        VatRate = sss.VatRate,
                                                        VatAmount = sss.VatAmount,
                                                        TaxId = sss.TaxId,
                                                        TaxAmount = sss.TaxAmount,
                                                        Total = sss.Total,
                                                        OrderDetailId = sss.OrderDetailId,
                                                        IsExtra = sss.IsExtra,
                                                        Net = sss.Net,
                                                        Discount = sss.Discount,
                                                        InvoiceTotal = sss.InvoiceTotal
                                                    }).Distinct()
                                                    .GroupBy(gg => gg.VatId)
                                                    .Select(ss => new {
                                                        InvoiceId = ss.FirstOrDefault().InvoiceId,
                                                        VatId = ss.Key,
                                                        VatRate = ss.FirstOrDefault().VatRate,
                                                        VatAmount = ss.Sum(sm => sm.VatAmount ?? 0),
                                                        TaxId = ss.FirstOrDefault().TaxId,
                                                        TaxAmount = ss.Sum(sm => sm.TaxAmount ?? 0),
                                                        Gross = ss.Sum(sm => sm.Total ?? 0),
                                                        Net = ss.Sum(sm => sm.Net ?? 0),
                                                        Discount = ss.Sum(sm => sm.Discount ?? 0),
                                                        InvoiceTotal = ss.FirstOrDefault().InvoiceTotal
                                                    }),
                                  PaymentAnalysis = g.Where(w => w.InvoiceType != 8 && w.InvoiceType != 2 && w.InvoiceType != 3 && w.IsVoided == false)
                                                      .Select(ss => new {
                                                          InvoiceId = ss.InvoiceId,
                                                          AccountId = ss.AccountId,
                                                          TransAmount = ss.TransAmount,
                                                          Description = ss.AccountDesc,
                                                          AccountType = ss.AccountType
                                                      }).ToList().Distinct()
                                                      .GroupBy(gg => gg.AccountId).Select(ss => new {
                                                          AccountId = ss.Key,
                                                          Description = ss.FirstOrDefault().Description,
                                                          AccountType = ss.FirstOrDefault().AccountType,
                                                          TransAmount = ss.Distinct().Sum(sm => sm.TransAmount)
                                                      }).Where(w => w.TransAmount != 0),
                                  // a = g.Where(w => w.InvoiceType == 3).Distinct().GroupBy(gg => gg.AccountId),
                                  VoidsAnalysis = g.Where(w => w.InvoiceType == 3).Select(ss => new {
                                      InvoiceId = ss.InvoiceId,
                                      AccountId = ss.AccountId,
                                      TransAmount = ss.TransAmount,
                                      Description = ss.AccountDesc,
                                      AccountType = ss.AccountType
                                  }).ToList().Distinct().Distinct().GroupBy(gg => gg.AccountId).Select(ss => new {
                                      AccountId = ss.Key,
                                      Description = ss.FirstOrDefault().Description,
                                      AccountType = ss.FirstOrDefault().AccountType,
                                      TransAmount = ss.Sum(sm => sm.TransAmount)
                                  }),

                                  DatTime = DateTime.Now
                              };

            if ( tempeodData.Count() == 0)
            {
                logger.Warn("No Z-Report data found for posInfoId:" + posInfoId.ToString() + " and eodId:" + eodId.ToString());
                return null;
            }
                

            var barcodes = br.ReceiptPaymentsFlat(w => w.PosInfoId == posInfoId
                                                     && w.EndOfDayId == null
                                                     && w.TransactionType == 9
                                                     ).Sum(sm => sm.Amount) ?? 0;



            var totalLockers = br.ReceiptPaymentsFlat(w => w.EndOfDayId == null
                                                        && w.PosInfoId == posInfoId
                                                        && ( w.TransactionType == 6
                                                        || w.TransactionType == 7 ));
            decimal? openLocker = totalLockers.Where(w => w.TransactionType == 6).Sum(sm => sm.Amount) ?? 0;
            decimal? paidLocker = ( totalLockers.Where(w => w.TransactionType == 7).Sum(sm => sm.Amount) * -1 ) ?? 0;

            var final = tempeodData.GroupBy(g => g.PosInfoId).Select(s => new EndOfDay {
                PosInfoId = s.Key,
                Net = s.FirstOrDefault().EndOfDayDetail.Sum(sm => sm.Net),
                Gross = s.Sum(sm => sm.Gross),
                TicketsCount = s.Sum(sm => sm.Ticketscount),
                ItemCount = (int) s.Sum(sm => sm.ItemCount),
                TicketAverage = s.Sum(sm => sm.Ticketscount) > 0 ? s.Sum(sm => sm.Gross) / s.Sum(sm => sm.Ticketscount) : 0, //Math.Round(s.Average(sm=>(decimal)sm.TicketAverage),2, MidpointRounding.AwayFromZero),
                EndOfDayDetail = s.SelectMany(ss => ss.EndOfDayDetail).GroupBy(gg => gg.VatId).Select(s2 => new EndOfDayDetail {
                    VatId = s2.Key,
                    VatRate = s2.FirstOrDefault().VatRate,
                    VatAmount = s2.Sum(sm => sm.VatAmount),
                    TaxId = s2.FirstOrDefault().TaxId,
                    TaxAmount = s2.FirstOrDefault().TaxAmount,
                    Gross = s2.Sum(sm => sm.Gross),
                    Net = s2.Sum(sm => sm.Net),
                    Discount = s2.Sum(sm => sm.Discount),
                }).ToList(),
                //a= s.Select(sl=>sl.a),
                EndOfDayPaymentAnalysis = s.SelectMany(ss => ss.PaymentAnalysis).GroupBy(gg => gg.AccountId).Select(ss => new EndOfDayPaymentAnalysis {
                    AccountId = ss.Key,
                    Description = ss.FirstOrDefault().Description,
                    AccountType = ss.FirstOrDefault().AccountType,
                    Total = ss.Sum(sm => sm.TransAmount)
                }).ToList(),
                EndOfDayVoidsAnalysis = s.SelectMany(ss => ss.VoidsAnalysis).Where(w => w.AccountId != 0).GroupBy(gg => gg.AccountId).Select(ss => new EndOfDayVoidsAnalysis {
                    AccountId = ss.Key,
                    Description = ss.FirstOrDefault().Description,
                    AccountType = ss.FirstOrDefault().AccountType,
                    Total = ss.Sum(sm => sm.TransAmount)
                }).ToList(),
                Lockers = DbContext.Lockers.Where(x => x.EndOfDayId == null).ToList(),
                Barcodes = barcodes,
                Discount = s.Sum(sm => sm.Discount),
            }).SingleOrDefault();
            var unpaid = final.Gross - final.EndOfDayPaymentAnalysis.Sum(sm => sm.Total);
            if ( unpaid > 0 ) {
                final.EndOfDayPaymentAnalysis.Add(new EndOfDayPaymentAnalysis() { Id = 0, Total = unpaid, AccountType = 1, AccountId = 1, Description = "Cash" });
            };



            var eodData = final;
            if ( eodData != null ) {
                eodData.StaffId = staffId;
                eodData.Id = eodId;
                //if (eodData)
                var xDataToPrint = ConvertEodDataToZModel(eodData, posInfoId, true, true);
                //   self.ws.send('SendMessage:' + fiscalName + '|ZReport:' + JSON.stringify(model));
                //eodData.FODay = xDataToPrint.Day;
                //var transferToPmsList = SendTransfer(posInfoId);
                //CreateEndOFDay(eodData);

                //DbContext.SaveChanges();
                //var eodTTps = DbContext.TransferToPms.Where(w => w.PosInfoId == posInfoId && w.EndOfDayId == null && w.Description.Contains("EOD"));
                //foreach (var ttp in eodTTps)
                //{
                //    ttp.EndOfDayId = eodData.Id;
                //    DbContext.Entry(ttp).State = EntityState.Modified;
                //}
                //DbContext.SaveChanges();
                //try
                //{
                //    SendTransferRepository str = new SendTransferRepository(DbContext);
                //    str.SendTransferToPMS(transferToPmsList, storeId);
                //}
                //catch (Exception ex)
                //{
                //    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error SendTransferToPMS | " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                //}
                return xDataToPrint;
            }
            else
            {
                logger.Warn("No Z-Report data found for posInfoId:" + posInfoId.ToString() + " and eodId:" + eodId.ToString()+ " because eodData = null.");
                return null;
            }
        }

        public Object GetZTotals( long posInfoId, long staffId, string storeId ) {
            var eodData = GetEODData(posInfoId);
            if ( eodData != null ) {
                eodData.StaffId = staffId;
                var xDataToPrint = ConvertEodDataToZModel(eodData, posInfoId, true);
                if (xDataToPrint.Lockers != null) {
                    xDataToPrint.Lockers.EndOfDay = null;
                }
                //   self.ws.send('SendMessage:' + fiscalName + '|ZReport:' + JSON.stringify(model));
                //eodData.FODay = xDataToPrint.Day;
                eodData.FODay = Convert.ToDateTime(xDataToPrint.Day);
                eodData.dtDateTime = Convert.ToDateTime(xDataToPrint.dtDateTime);
                var transferToPmsList = SendTransfer(posInfoId);
                CreateEndOFDay(eodData);

                DbContext.SaveChanges();
                var eodTTps = DbContext.TransferToPms.Where(w => w.PosInfoId == posInfoId && w.EndOfDayId == null && w.Description.Contains("EOD"));
                foreach ( var ttp in eodTTps ) {
                    ttp.EndOfDayId = eodData.Id;
                    DbContext.Entry(ttp).State = EntityState.Modified;
                }
                DbContext.SaveChanges();
                try {
                    SendTransferRepository str = new SendTransferRepository(DbContext);
                    //str.SendTransferToPMS(transferToPmsList, storeId);
                } catch ( Exception ex ) {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error SendTransferToPMS | " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                    logger.Error(ex.ToString());
                }
                return xDataToPrint;
            } else
                return null;
        }

        /// <summary>
        ///  Return XReport Data for not audited sales by posId
        /// </summary>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public Object GetXTotals( long posInfoId ) {
            var eodData = GetEODData(posInfoId);
            if ( eodData != null ) {
                var xDataToPrint = ConvertEodDataToZModel(eodData, posInfoId, false);
                return xDataToPrint;
            } else
                return null;
        }

        private IEnumerable<TransferToPms> SendTransfer( long posInfoId ) {
            var res = new HashSet<TransferToPms>();

            var pi = DbContext.PosInfo.FirstOrDefault(x => x.Id == posInfoId);
            var query = from q in DbContext.EODAccountToPmsTransfer
                        join qq in DbContext.Accounts.Where(w => w.SendsTransfer == false) on q.AccountId equals qq.Id
                        where q.PosInfoId == posInfoId
                        select new {
                            AccountId = q.AccountId,
                            Room = q.PmsRoom.ToString(),
                            Description = q.Description,
                            PosInfoId = q.PosInfoId,
                            AccountDescription = qq.Description
                        };

            var ttpms = ( from q in DbContext.TransferToPms.Where(w => w.EndOfDayId == null && w.PosInfoId == posInfoId && w.Status == 0 && !w.Description.Contains("EOD") && w.SendToPMS == false)
                          join qq in query on q.RoomDescription equals qq.Room
                          join qqq in DbContext.Transactions.Where(w => ( w.IsDeleted ?? false ) == false && w.TransactionType != 4) on q.TransactionId equals qqq.Id
                          join qqqq in DbContext.Invoices.Where(w => ( w.IsDeleted ?? false ) == false && ( w.IsVoided ?? false ) == false && ( w.IsPrinted ?? false ) == true) on qqq.InvoicesId equals qqqq.Id
                          select new {
                              RegNo = q.RegNo,
                              PmsDepartmentId = q.PmsDepartmentId,
                              PmsDepartmentDescription = q.PmsDepartmentDescription,
                              RoomDescription = q.RoomDescription,
                              AccountDescription = qq.AccountDescription,
                              Total = q.Total,
                              Id = q.Id,
                              q.Points,
                              HotelId = q.HotelId
                          } ).Distinct()
                        .GroupBy(g => new {
                            RoomDescription = g.RoomDescription, PmsDepartmentId = g.PmsDepartmentId, HotelId = g.HotelId
                        })
                        .Select(ss => new {
                            RegNo = ss.FirstOrDefault().RegNo,
                            PmsDepartmentId = ss.Key.PmsDepartmentId,
                            PmsDepartmentDescription = ss.FirstOrDefault().PmsDepartmentDescription,
                            Room = ss.Key.RoomDescription,
                            Points = ss.FirstOrDefault().Points,
                            ProfileNo = ss.FirstOrDefault().AccountDescription,
                            AccountDescription = ss.FirstOrDefault().AccountDescription,
                            RoomDescription = ss.Key.RoomDescription,
                            Total = ss.Sum(sm => sm.Total),
                            HotelId = ss.FirstOrDefault().HotelId
                        });
            foreach ( var t in ttpms.Where(w => w.Total != 0) ) {
                res.Add(str.WriteEODToTransfer(pi.Id, pi.Description, t.PmsDepartmentId, t.PmsDepartmentDescription, t.AccountDescription, t.Room, t.Total, t.HotelId));
            }
            return res;
        }

        public void CreateEndOFDay( EndOfDay eod ) {
            var invoices = DbContext.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            foreach ( var q in invoices ) {
                eod.Invoices.Add(q);
            }
            var odi = DbContext.OrderDetailInvoices.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            foreach ( var q in odi ) {
                eod.OrderDetailInvoices.Add(q);
            }
            var transactions = DbContext.Transactions.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            foreach ( var q in transactions ) {
                eod.Transactions.Add(q);
            }
            var orders = DbContext.Order.Where(w => w.EndOfDayId == null && w.PosId == eod.PosInfoId);
            foreach ( var q in orders ) {
                eod.Order.Add(q);
            }
            var transfers = DbContext.TransferToPms.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            foreach ( var q in transfers ) {
                eod.TransferToPms.Add(q);
            }
            var mealConsumption = DbContext.MealConsumption.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            foreach ( var q in mealConsumption ) {
                eod.MealConsumption.Add(q);
            }
            var creditTransactions = DbContext.CreditTransactions.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            foreach ( var q in creditTransactions ) {
                eod.CreditTransactions.Add(q);
            }

            var kitchenInstructionLogger = DbContext.KitchenInstructionLogger.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            foreach ( var q in kitchenInstructionLogger ) {
                eod.KitchenInstructionLogger.Add(q);
            }
            ///Different Way
            //var creditAccounts = DbContext.CreditAccounts.Where(w => w.EndOfDayId == null && w.PosInfoId == eod.PosInfoId);
            //foreach (var q in creditAccounts)
            //{
            //    q.EndOfDay = eod;
            //    DbContext.Entry(q).State = EntityState.Modified;
            //}
            var lockers = DbContext.Lockers.Where(w => w.EndOfDayId == null);
            foreach ( var q in lockers ) {
                eod.Lockers.Add(q);
            }


            DbContext.EndOfDay.Add(eod);

        }
        public int SaveChanges( ) {
            return DbContext.SaveChanges();
        }

        public void Dispose()
        {
            br.Dispose();
            str.Dispose();
            br = null;
            str = null;
        }
    }
  
   

}

public class AuditFlatInvoicesBaseClass {
    public DateTime? Day {
        get; set;
    }
    public long? InvoiceId {
        get; set;
    }

    public long? OrderNo {
        get; set;
    }

    public long? Counter {
        get; set;
    }

    public long? Covers {
        get; set;
    }
    public string InvoiceAbbreviation {
        get; set;
    }
    public int? InvoiceType {
        get; set;
    }
    public decimal? InvoiceTotal {
        get; set;
    }

    public decimal? InvoiceDiscount {
        get; set;
    }

    public decimal? InvoicePaidAmount {
        get; set;
    }
    public short IsPaid {
        get; set;
    }
    public bool IsInvoiced {
        get; set;
    }
    public bool IsDeleted {
        get; set;
    }
    public bool IsVoided {
        get; set;
    }
    public long? VatId {
        get; set;
    }

    public int? VatCode
    {
        get; set;
    }

    public decimal? Total {
        get; set;
    }

    public decimal? VatAmount {
        get; set;
    }

    public decimal? VatRate {
        get; set;
    }

    public decimal? Net {
        get; set;
    }
    public long? AccountId {
        get; set;
    }
    //			AccountDesc = tr != null? tr.Account.Description:"",
    public short? AccountType {
        get; set;
    }

    public decimal? TransAmount {
        get; set;
    }
    public long? TransStaff {
        get; set;
    }
    public long? StaffId {
        get; set;
    }
    public long? OrderDetailId {
        get; set;
    }
    public byte? FiscalType {
        get; set;
    }
    public DateTime? FODay {
        get; set;
    }
    public long? EndOfDayId {
        get; set;
    }
    public long? PosInfoId {
        get; set;
    }
    public string Room {
        get; set;
    }
    public long? TransctionId {
        get; set;
    }
    public string TableCode {
        get; set;
    }
    public long? DepartmentId {
        get; set;
    }
    public string PosInfoDescription {
        get; set;
    }
}

public class AuditGroupByInvoiceBaseClass {
    public AuditGroupByInvoiceBaseClass( ) {
        CardAnalysis = new HashSet<AuditCreditCardsAmounts>();
    }
    public long? InvoiceId {
        get; set;
    }
    public long? OrderNo {
        get; set;
    }
    public string Abbreviation {
        get; set;
    }
    public long? ReceiptNo {
        get; set;
    }
    public int? InvoiceType {
        get; set;
    }
    public short? IsPaid {
        get; set;
    }
    public decimal? InvoiceTotal {
        get; set;
    }
    public decimal? InvoiceDiscount {
        get; set;
    }
    public decimal? InvoicePaidAmount {
        get; set;
    }
    public bool IsInvoiced {
        get; set;
    }
    public bool IsVoided {
        get; set;
    }
    public decimal? Vat1 {
        get; set;
    }
    public decimal? Vat2 {
        get; set;
    }
    public decimal? Vat3 {
        get; set;
    }
    public decimal? Vat4 {
        get; set;
    }
    public decimal? Vat5 {
        get; set;
    }
    public decimal? Vat1Perc {
        get; set;
    }
    public decimal? Vat2Perc {
        get; set;
    }
    public decimal? Vat3Perc {
        get; set;
    }
    public decimal? Vat4Perc {
        get; set;
    }
    public decimal? Vat5Perc {
        get; set;
    }
    public decimal? AccountType1 {
        get; set;
    }
    public decimal? AccountType2 {
        get; set;
    }
    public decimal? AccountType3 {
        get; set;
    }
    public decimal? AccountType4 {
        get; set;
    }
    public decimal? AccountType5 {
        get; set;
    }
    public decimal? AccountType6 {
        get; set;
    }
    public decimal? AccountType7 {
        get; set;
    }
    public decimal? AccountType8 {
        get; set;
    }
    public decimal? AccountType9 {
        get; set;
    }
    public int? AccountType1ReceiptCount {
        get; set;
    }
    public int? AccountType2ReceiptCount {
        get; set;
    }
    public int? AccountType3ReceiptCount {
        get; set;
    }
    public int? AccountType4ReceiptCount {
        get; set;
    }
    public int? AccountType5ReceiptCount {
        get; set;
    }
    public int? AccountType6ReceiptCount {
        get; set;
    }
    public int? AccountType7ReceiptCount {
        get; set;
    }
    public int? AccountType8ReceiptCount {
        get; set;
    }
    public int? AccountType9ReceiptCount {
        get; set;
    }
    public decimal? TransAmount {
        get; set;
    }
    public double? ItemsCount {
        get; set;
    }
    public long? StaffId {
        get; set;
    }
    public short? FiscalType {
        get; set;
    }
    public DateTime? FODay {
        get; set;
    }
    public long? EndOfDayId {
        get; set;
    }

    public decimal? Check {
        get; set;
    }
    public ICollection<AuditCreditCardsAmounts> CardAnalysis {
        get; set;
    }
    public string Room {
        get; set;
    }
    public long? Covers {
        get; set;
    }
    public string TableCode {
        get; set;
    }
    public long? DepartmentId {
        get; set;
    }
    public long? PosInfoId {
        get; set;
    }
    public string PosInfoDescription {
        get; set;
    }
    public DateTime? Day {
        get; set;
    }
}

public class AuditCreditCardsAmounts {
    public long? AccountId {
        get; set;
    }
    public decimal? TransAmount {
        get; set;
    }
    public decimal? TransCount
    {
        get; set;
    }

}

public class AuditSalesPerProductBase {
    public DateTime? Day {
        get; set;
    }
    public long? PosInfoId {
        get; set;
    }
    public long? DepartmentId {
        get; set;
    }
    public long? ProductId {
        get; set;
    }
    public string ProductCode {
        get; set;
    }
    public string Description {
        get; set;
    }
    public long? InvoiceId {
        get; set;
    }
    public string ProductCategory {
        get; set;
    }
    public string Pricelist {
        get; set;
    }
    public string Category {
        get; set;
    }
    public bool? IsVoided {
        get; set;
    }
    public string StaffName {
        get; set;
    }
    public string StaffCode {
        get; set;
    }
    public double? Qty {
        get; set;
    }
    public decimal? Price {
        get; set;
    }
    public decimal? Gross {
        get; set;
    }
    public decimal? Discount {
        get; set;
    }
    public bool? IsExtra {
        get; set;
    }
    public long? SalesTypeId {
        get; set;
    }
    public string SalesTypeDescription {
        get; set;
    }
}

public static class EconomicHelper {
    public static decimal DeVat( decimal perc, decimal tempnetbyvat ) {
        if ( perc == 0 )
            return tempnetbyvat;
        else
            return (decimal) ( tempnetbyvat / (decimal) ( 1 + (decimal) ( perc / 100 ) ) );
    }
}