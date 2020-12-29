using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class RegionModelsPreview
    {
        public List<RegionModel> RegionModel { get; set; }
    }
    public class RegionModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string BluePrintPath { get; set; }
        public Nullable<int> MaxCapacity { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<bool> IsLocker { get; set; }
        public List<PosInfo_Region_AssocModels> PosInfo_Region_Assoc { get; set; }
        public List<TablesModel> Table { get; set; }
    }

    public class TablesModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string SalesDescription { get; set; }
        public string Description { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public long? RegionId { get; set; }
        public byte? Status { get; set; }
        public string YPos { get; set; }
        public string XPos { get; set; }
        public bool? IsOnline { get; set; }
        public short? ReservationStatus { get; set; }
        public long? Shape { get; set; }
        public int? TurnoverTime { get; set; }
        public string ImageUri { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public string Angle { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class PosInfo_Region_AssocModels
    {
        public long Id { get; set; }
        public long? PosInfoId { get; set; }
        public long? RegionId { get; set; }
        public string PosInfoDescription { get; set; }
        public string RegionDescription { get; set; }
        public bool IsDeleted { get; set; }
    }
}
