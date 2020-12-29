using System.Collections.Generic;

namespace Pos_WebApi.Models.DTOModels
{
    public class AllowedMealsPerBoardDetailsDTO : IDTOModel<AllowedMealsPerBoardDetails>
    {
        public long Id { get; set; }
        public long? ProductCategoryId { get; set; }
        public long? AllowedMealsPerBoardId { get; set; }
        public string ProductCategoryDescription { get; set; }
        public bool IsDeleted { get; set; }

        public AllowedMealsPerBoardDetails ToModel()
        {
            var model = new AllowedMealsPerBoardDetails()
            {
                ProductCategoryId = this.ProductCategoryId,
                AllowedMealsPerBoardId = this.AllowedMealsPerBoardId,
            };
            return model;
        }

        public AllowedMealsPerBoardDetails UpdateModel(AllowedMealsPerBoardDetails model)
        {
            model.ProductCategoryId = this.ProductCategoryId;
            model.AllowedMealsPerBoardId = this.AllowedMealsPerBoardId;

            return model;
        }


     
    }
}
