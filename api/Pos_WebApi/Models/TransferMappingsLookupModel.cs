namespace Pos_WebApi.Models
{
    public class TransferMappingsLookupModel
    {
        public long? TransferMappingsId { get; set; }
        public long? PosDepartmentId { get; set; }
        public string PosDepartmentDescription { get; set; }
        public long? PricelistId { get; set; }
        public string PricelistDescription { get; set; }
        public int? VatCode { get; set; }
        public string VatDescription { get; set; }
        public string PmsDepartmentId { get; set; }
        public string PmsDepartmentDescription { get; set; }
    }

    public class TransferMappingUsedDetailsLookUps
    {
        public int? VatCode { get; set; }
        public long? TransferMappingsId { get; set; }
        public long? ProductCategoryId { get; set; }
        public bool IsInCurrentVat { get; set; }
        public long? TransferMappingsDetailsId { get; set; }
    }
}
