using System.Collections.Generic;
using System.Linq;

namespace Pos_WebApi.Models.DTOModels
{
    public class AllowedMealsPerBoardDTO : IDTOModel<AllowedMealsPerBoard>
    {
        public AllowedMealsPerBoardDTO()
        {
            Details = new HashSet<AllowedMealsPerBoardDetailsDTO>();
        }

        public long Id { get; set; }
        public string BoardId { get; set; }
        public string BoardDescription { get; set; }
        public int? AllowedMeals { get; set; }
        public decimal? AllowedDiscountAmount { get; set; }
        public bool? IsDeleted { get; set; }
        public long? PriceListId { get; set; }
        public int? AllowedMealsChild { get; set; }
        public decimal? AllowedDiscountAmountChild { get; set; }
        public long? PricelistId { get; set; }
        public string PricelistDescription { get; set; }

        public ICollection<AllowedMealsPerBoardDetailsDTO> Details { get; set; }

        public AllowedMealsPerBoard ToModel()
        {
            var model = new AllowedMealsPerBoard()
            {
                Id = this.Id,
                BoardId = this.BoardId,
                BoardDescription = this.BoardDescription,
                AllowedMeals = this.AllowedMeals,
                AllowedDiscountAmount = this.AllowedDiscountAmount,
                AllowedMealsChild = this.AllowedMealsChild,
                AllowedDiscountAmountChild = this.AllowedDiscountAmountChild,
                PriceListId = this.PriceListId
            };

            foreach (var det in this.Details)
            {
                model.AllowedMealsPerBoardDetails.Add(det.ToModel());
            }
            return model;
        }

        public AllowedMealsPerBoard UpdateModel(AllowedMealsPerBoard model)
        {
            model.BoardId = this.BoardId;
            model.BoardDescription = this.BoardDescription;
            model.AllowedMeals = this.AllowedMeals;
            model.AllowedDiscountAmount = this.AllowedDiscountAmount;
            model.AllowedMealsChild = this.AllowedMealsChild;
            model.AllowedDiscountAmountChild = this.AllowedDiscountAmountChild;
            model.PriceListId = this.PriceListId;

            foreach (var det in this.Details.Where(w=>w.IsDeleted == false))
            {
                if (det.Id == 0)
                    model.AllowedMealsPerBoardDetails.Add(det.ToModel());
                else
                {
                    var cur = model.AllowedMealsPerBoardDetails.FirstOrDefault(x => x.Id == det.Id);
                    if (cur != null)
                    {
                        det.UpdateModel(det.ToModel());
                    }
                }
            }
            return model;
        }
    }
}
