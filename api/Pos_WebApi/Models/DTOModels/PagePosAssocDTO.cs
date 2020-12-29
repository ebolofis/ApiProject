namespace Pos_WebApi.Models.DTOModels
{
    public class PagePosAssocDTO : IDTOModel<PagePosAssoc>
    {
        public long Id { get; set; }
        public long? PageSetId { get; set; }
        public long? PosInfoId { get; set; }
        public string PosInfoDescription { get; set; }
        public bool IsDeleted { get; set; }

        public PagePosAssoc ToModel()
        {
            var model = new PagePosAssoc()
            {
                PageSetId = this.PageSetId,
                PosInfoId = this.PosInfoId
            };
            return model;
        }

        public PagePosAssoc UpdateModel(PagePosAssoc model)
        {
            model.PageSetId = this.PageSetId;
            model.PosInfoId = this.PosInfoId;

            return model;
        }
    }
}
