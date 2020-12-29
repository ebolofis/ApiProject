using LinqKit;
using Pos_WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;

namespace Pos_WebApi.Models.FilterModels
{
    public class ReceiptFilters
    {
        public ReceiptFilters()
        {

        }
        private Expression<Func<TempReceiptBOFull, bool>> _predicate;

        public DateTime? FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Int64?> PosList { get; set; }
        public List<Int64?> StaffList { get; set; }
        public List<short?> InvoiceTypeList { get; set; }
        public DateTime? FODay { get; set; }
        public long? EodId { get; set; } // 1-9-2014
        public string TableCode { get; set; }
        public List<Int64?> TableCodes { get; set; }
        public string Room { get; set; }
        public bool? IsInvoiced { get; set; }
        public string OrderNo { get; set; }
        public int? ReceiptNo { get; set; }

        public bool UseEod { get; set; }
        public bool UsePeriod { get; set; }
        public List<Int64?> PricelistsList { get; set; }
        public long? PosInfoId { get; set; }
        public bool? IsPaid { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public bool? IsPrinted { get; set; }


        public Expression<Func<TempReceiptBOFull, bool>> predicate
        {
            get
            {
                var _predicate = PredicateBuilder.True<TempReceiptBOFull>();
                if (this.FromDate != null)
                    _predicate = _predicate.And(p => EntityFunctions.TruncateTime(p.Day) == EntityFunctions.TruncateTime(this.FromDate));
                //if (this.FODay != null)
                //    _predicate = _predicate.And(p => p.FODay == this.FODay);
                if (this.PosList != null)
                    _predicate = _predicate.And(p => this.PosList.Contains(p.PosInfoId));
                if (this.TableCode != null)
                    _predicate = _predicate.And(p => p.TableCode.Contains(this.TableCode));
                if (this.Room != null)
                    _predicate = _predicate.And(p => p.Room.Contains(this.Room));
                if (this.PosInfoId != null)
                    _predicate = _predicate.And(p => p.PosInfoId == this.PosInfoId);
                if (this.EodId != null)
                    _predicate = _predicate.And(p => p.EndOfDayId == this.EodId);
                if (this.IsInvoiced != null && this.IsInvoiced == false)
                    _predicate = _predicate.And(p => p.IsInvoiced == this.IsInvoiced && p.InvoiceTypeType == 2);
                if (!string.IsNullOrEmpty(this.OrderNo))
                    _predicate = _predicate.And(p => p.OrderNo.Contains(this.OrderNo));
        /*afto*/if (this.ReceiptNo != null)
                    _predicate = _predicate.And(p => p.ReceiptNo == this.ReceiptNo);
                if (this.IsPrinted != null && this.IsPrinted == false)
                    _predicate = _predicate.And(p => p.IsPrinted == false);

                if (InvoiceTypeList != null && (this.IsInvoiced == null || this.IsInvoiced == true))
                    _predicate = _predicate.And(p => this.InvoiceTypeList.Contains(p.InvoiceTypeType));
                if (this.IsPaid != null)
                {
                    _predicate = _predicate.And(p => p.InvoiceTypeType != 8 && p.InvoiceTypeType != 3 && p.IsVoided == false);
                    if (this.IsPaid == true)
                    {
                        _predicate = _predicate.And(p => p.IsPaid == 2);

                    }
                    else
                    {
                        _predicate = _predicate.And(p => p.IsPaid < 1);
                    }
                }


                return _predicate;
            }
        }
        //        public List<Int64?> CategoriesList { get; set; }
        //        public List<Int64?> ProductCategoriesList { get; set; }
        //        public List<Int64?> DepartmentList { get; set; }
        //        public string FromProductCode { get; set; }
        //        public string ToProductCode { get; set; }
        //        public Int64? CostPriceListId { get; set; }
        //        public Int64? DisplayPriceListId { get; set; }
        //        public List<Int64> DisplayPriceList { get; set; }
        //        public bool UseOrderInvoicesType { get; set; }
    }
}
