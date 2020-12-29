using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Models.DTOModels
{
    public class PageButtonDTO : IDTOModel<PageButton>
    {
        public long Id { get; set; }
        public short? Sort { get; set; }
        public long? PageId { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }
        public long? ProductId { get; set; }
        public string Description { get; set; }
        public string SalesDescritpion { get; set; }
        public byte? Type { get; set; }
        public long? SetDefaultPriceListId { get; set; }
        public long? SetDefaultSalesType { get; set; }
        public long? NavigateToPage { get; set; }
        public long? PriceListId { get; set; }

        public bool IsDeleted { get; set; }

        public PageButton ToModel()
        {
            var model = new PageButton()
            {
                Id = this.Id,
                Sort = this.Sort,
                PageId = this.PageId,
                Color = this.Color,
                Background = this.Background,
                ProductId = this.ProductId,
                Description = this.Description,
                Type = this.Type,
                SetDefaultPriceListId = this.SetDefaultPriceListId,
                SetDefaultSalesType = this.SetDefaultSalesType,
                NavigateToPage = this.NavigateToPage,
                PriceListId = this.PriceListId

            };
            return model;
        }

        public PageButton UpdateModel(PageButton model)
        {

            model.Id = this.Id;
            model.Sort = this.Sort;
            model.PageId = this.PageId;
            model.Color = this.Color;
            model.Background = this.Background;
            model.ProductId = this.ProductId;
            model.Description = this.Description;
            model.Type = this.Type;
            model.SetDefaultPriceListId = this.SetDefaultPriceListId;
            model.SetDefaultSalesType = this.SetDefaultSalesType;
            model.NavigateToPage = this.NavigateToPage;
            model.PriceListId = this.PriceListId;


            return model;

        }
    }
}
