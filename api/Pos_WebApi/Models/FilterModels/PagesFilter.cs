using LinqKit;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Linq.Expressions;

namespace Pos_WebApi.Models.FilterModels
{
    public class PagesFilter
    {

        public PagesFilter()
        {

        }
        public long? Id { get; set; }
        public string Description { get; set; }
        public string ExtendedDesc { get; set; }
        public long? DefaultPriceListId { get; set; }
        public long? PageSetId { get; set; }
        public short? Sort { get; set; }
        public bool? Status { get; set; }

        private Expression<Func<PagesDTO, bool>> _predicate;

        public Expression<Func<PagesDTO, bool>> predicate
        {
            get
            {
                var _predicate = PredicateBuilder.True<PagesDTO>();
                if (this.Id != null)
                    _predicate = _predicate.And(p => p.Id == this.Id);
                if (this.Description != null)
                    _predicate = _predicate.And(p => this.Description.Contains(this.Description));
                if (this.ExtendedDesc != null)
                    _predicate = _predicate.And(p => this.ExtendedDesc.Contains(this.ExtendedDesc));
                if (this.DefaultPriceListId != null)
                    _predicate = _predicate.And(p => p.DefaultPriceListId == this.DefaultPriceListId);
                if (this.PageSetId != null)
                    _predicate = _predicate.And(p => p.PageSetId == this.PageSetId);
                if (this.Sort != null)
                    _predicate = _predicate.And(p => p.Sort == this.Sort);
                if (this.Status != null)
                    _predicate = _predicate.And(p => p.Status == this.Status);

                return _predicate;
            }
        }
    }
}
