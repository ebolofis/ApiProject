using LinqKit;
using Pos_WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Pos_WebApi.Models.FilterModels
{
    public class ProductsWithCategoriesFlatFilters
    {

        public ProductsWithCategoriesFlatFilters()
        {

        }
        public long? ProductId { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategoryCode { get; set; }
        public long? ProductCategoryId { get; set; }
        public string ProductCategoryDesc { get; set; }
        public long? CategoryId { get; set; }
        public string CategoryDesc { get; set; }
        public List<long?> ProductCategoriesList { get; set; }
        public List<long?> CategoriesList { get; set; }

        private Expression<Func<TempProductsWithCategoriesFlat, bool>> _predicate;

        public Expression<Func<TempProductsWithCategoriesFlat, bool>> predicate
        {
            get
            {
                var _predicate = PredicateBuilder.True<TempProductsWithCategoriesFlat>();
                if (this.ProductId != null)
                    _predicate = _predicate.And(p => p.ProductId == this.ProductId);
                if (!String.IsNullOrEmpty(this.Description))
                    _predicate = _predicate.And(p => p.Description.ToUpper().Contains(this.Description.ToUpper()));
                if (!String.IsNullOrEmpty(this.ProductCode))
                    _predicate = _predicate.And(p => p.ProductCode.StartsWith(this.ProductCode));
                if (!String.IsNullOrEmpty(this.ProductCategoryCode))
                    _predicate = _predicate.And(p => p.ProductCategoryCode == this.ProductCategoryCode);
                if (this.ProductCategoryId != null)
                    _predicate = _predicate.And(p => p.ProductCategoryId == this.ProductCategoryId);
                if (this.CategoryId != null)
                    _predicate = _predicate.And(p => p.CategoryId == this.CategoryId);
                if (!String.IsNullOrEmpty(this.ProductCategoryDesc))
                    _predicate = _predicate.And(p => p.ProductCategoryDesc.Contains(this.ProductCategoryDesc));
                if (!String.IsNullOrEmpty(this.CategoryDesc))
                    _predicate = _predicate.And(p => p.CategoryDesc.Contains(this.CategoryDesc));
                if (this.ProductCategoriesList != null)
                    _predicate = _predicate.And(p => this.ProductCategoriesList.Contains(p.ProductCategoryId));
                if (this.CategoriesList != null)
                    _predicate = _predicate.And(p => this.CategoriesList.Contains(p.ProductCategoryId));
                return _predicate;
            }
        }
    }
}
