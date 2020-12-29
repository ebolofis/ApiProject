using LinqKit;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Linq.Expressions;

namespace Pos_WebApi.Models.FilterModels
{
    public class PageSetFilter
    {
        public PageSetFilter()
        {

        }
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? DeactivationDate { get; set; }
        public long? PosInfoId { get; set; }
        public long? PdaModuleId { get; set; }

        private Expression<Func<PageSetDTO, bool>> _predicate;

        public Expression<Func<PageSetDTO, bool>> predicate
        {
            get
            {
                var _predicate = PredicateBuilder.True<PageSetDTO>();
                if (this.Id != null)
                    _predicate = _predicate.And(p => p.Id == this.Id);
                if (this.Description != null)
                    _predicate = _predicate.And(p => this.Description.Contains(this.Description));
                if (this.ActivationDate != null)
                    _predicate = _predicate.And(p => p.ActivationDate == this.ActivationDate);
                if (this.DeactivationDate != null)
                    _predicate = _predicate.And(p => p.DeactivationDate == this.DeactivationDate);
                if (this.PosInfoId != null)
                    _predicate = _predicate.And(p => p.PosInfoId == this.PosInfoId);
                if (this.PdaModuleId != null)
                    _predicate = _predicate.And(p => p.PdaModuleId == this.PdaModuleId);

                return _predicate;
            }
        }
    }
}
