using LinqKit;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Linq.Expressions;

namespace Pos_WebApi.Models.FilterModels
{
    public class ProductFilter
    {
        public ProductFilter()
        {

        }
        public long? ProductId { get; set; }
        public string Description { get; set; }
        public string ExtraDescription { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<long> UnitId { get; set; }
        public string SalesDescription { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<long> KdsId { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public string ImageUri { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public string Code { get; set; }
        public Nullable<bool> IsCustom { get; set; }
        public Nullable<long> KitchenRegionId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }


        private Expression<Func<ProductDTO, bool>> _predicate;
        private Expression<Func<ProductDTO, bool>> _orpredicate;

        public Expression<Func<ProductDTO, bool>> predicate
        {
            get
            {
                var _predicate = PredicateBuilder.True<ProductDTO>();
                if (this.ProductId != null)
                    _predicate = _predicate.And(p => p.Id == this.ProductId);
                if (!String.IsNullOrEmpty(this.Description))
                    _predicate = _predicate.And(p => p.Description.ToUpper().Contains(this.Description.Trim().ToUpper())
                                                                             || this.SalesDescription.ToUpper().Contains(this.Description.Trim().ToUpper())
                                                                             || this.ExtraDescription.ToUpper().Contains(this.Description.ToUpper()));
                if (!String.IsNullOrEmpty(this.Code))
                    _predicate = _predicate.And(p => p.Code.Contains(this.Code));
                if (this.ProductCategoryId != null)
                    _predicate = _predicate.And(p => p.ProductCategoryId == this.ProductCategoryId);
                if (this.IsDeleted != null)
                    _predicate = _predicate.And(p => p.IsDeleted == this.IsDeleted);
                return _predicate;
            }
        }
        public Expression<Func<ProductDTO, bool>> orpredicate {
            get {
                var _orpredicate = PredicateBuilder.False<ProductDTO>();
                if (this.ProductId != null)
                    _orpredicate = _orpredicate.Or(p => p.Id == this.ProductId);
                if (!String.IsNullOrEmpty(this.Description))
                    _orpredicate = _orpredicate.Or(p => p.Description.ToUpper().Contains(this.Description.Trim().ToUpper())
                                                                             || this.SalesDescription.ToUpper().Contains(this.Description.Trim().ToUpper())
                                                                             || this.ExtraDescription.ToUpper().Contains(this.Description.ToUpper()));
                if (!String.IsNullOrEmpty(this.Code))
                    _orpredicate = _orpredicate.Or(p => p.Code.Contains(this.Code));
                if (this.ProductCategoryId != null)
                    _orpredicate = _orpredicate.Or(p => p.ProductCategoryId == this.ProductCategoryId);
                if (this.IsDeleted != null)
                    _orpredicate = _orpredicate.Or(p => p.IsDeleted == this.IsDeleted);
                return _orpredicate;
            }
        }

        
    }
}
