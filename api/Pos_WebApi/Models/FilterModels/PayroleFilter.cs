using LinqKit;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;

namespace Pos_WebApi.Models.FilterModels {
    public class PayroleFilter {

        public PayroleFilter() {

        }
        //public long Id { get; set; }
        public string Identification { get; set; }
        public System.DateTime ActionDateFrom { get; set; }
        public System.DateTime ActionDateTo { get; set; }
        public int? Type { get; set; }
        public List<long> PosInfoId { get; set; }
        public List<long> StaffId { get; set; }
        public string ShopId { get; set; }
        public bool usedate { get; set; }


        private Expression<Func<PayroleDTO, bool>> _predicate;

        public Expression<Func<PayroleDTO, bool>> predicate {
            get {
                var _predicate = PredicateBuilder.True<PayroleDTO>();

                if (!String.IsNullOrEmpty(this.Identification))
                    _predicate = _predicate.And(p => p.Identification.ToUpper().Contains(this.Identification.Trim().ToUpper()));

                if (!String.IsNullOrEmpty(this.ShopId))
                    _predicate = _predicate.And(p => p.ShopId.ToUpper().Contains(this.ShopId.Trim().ToUpper()));

                if (this.PosInfoId != null)
                    _predicate = _predicate.And(p => this.PosInfoId.Contains(p.PosInfoId));
                if (this.StaffId != null)
                    _predicate = _predicate.And(p => this.StaffId.Contains(p.StaffId));

                if (this.usedate == true)
                    _predicate = _predicate.And(p => EntityFunctions.TruncateTime(p.ActionDate) >= EntityFunctions.TruncateTime(this.ActionDateFrom));
                if (this.usedate == true)
                    _predicate = _predicate.And(p => EntityFunctions.TruncateTime(p.ActionDate) <= EntityFunctions.TruncateTime(this.ActionDateTo)); 

                if (this.Type != null)
                    _predicate = _predicate.And(p => p.Type == this.Type);

                return _predicate;
            }
        }
    }


}