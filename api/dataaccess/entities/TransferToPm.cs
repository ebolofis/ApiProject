namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TransferToPm
    {
        public long Id { get; set; }

        [StringLength(150)]
        public string RegNo { get; set; }

        [StringLength(100)]
        public string PmsDepartmentId { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(150)]
        public string ProfileId { get; set; }

        [StringLength(150)]
        public string ProfileName { get; set; }

        public long? TransactionId { get; set; }

        public short? TransferType { get; set; }

        [StringLength(50)]
        public string RoomId { get; set; }

        [StringLength(150)]
        public string RoomDescription { get; set; }

        [StringLength(50)]
        public string ReceiptNo { get; set; }

        public long? PosInfoDetailId { get; set; }

        public bool? SendToPMS { get; set; }

        public decimal? Total { get; set; }

        public DateTime? SendToPmsTS { get; set; }

        [StringLength(1000)]
        public string ErrorMessage { get; set; }

        [StringLength(150)]
        public string PmsResponseId { get; set; }

        public Guid? TransferIdentifier { get; set; }

        [StringLength(300)]
        public string PmsDepartmentDescription { get; set; }

        public short? Status { get; set; }

        public long? PosInfoId { get; set; }

        public long? EndOfDayId { get; set; }

        public long? HotelId { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
