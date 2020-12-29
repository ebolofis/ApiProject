using System.Collections.Generic;

namespace Pos_WebApi.Models.DTOModels
{
    public class PricelistMasterDTO : IDTOModel<PricelistMaster>
    {
        public PricelistMasterDTO()
        {
            AssociatedSalesTypes = new HashSet<SalesType_PricelistMaster_AssocDTO>();
        }

        public long Id { get; set; }
        public string Description { get; set; }
        public byte? Status { get; set; }
        public bool? Active { get; set; }

        public ICollection<SalesType_PricelistMaster_AssocDTO> AssociatedSalesTypes { get; set; }

        public PricelistMaster ToModel()
        {
            var model = new PricelistMaster()
            {
                Description = this.Description,
                Status = this.Status,
                Active = this.Active
            };

            foreach (var sa in AssociatedSalesTypes)
            {
                model.SalesType_PricelistMaster_Assoc.Add(sa.ToModel());
            }
            return model;

        }

        public PricelistMaster UpdateModel(PricelistMaster model)
        {
            model.Description = this.Description;
            model.Status = this.Status;
            model.Active = this.Active;

            return model;
        }
    }
}