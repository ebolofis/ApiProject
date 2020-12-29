namespace Pos_WebApi.Models.DTOModels
{
    public class TableDTO : IDTOModel<Table>
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

        public Table ToModel()
        {
            var model = new Table()
            {
                Id = this.Id,
                Code = this.Code,
                SalesDescription = this.SalesDescription,
                Description = this.Description,
                MinCapacity = this.MinCapacity,
                MaxCapacity = this.MaxCapacity,
                RegionId = this.RegionId,
                Status = this.Status,
                YPos = this.YPos,
                XPos = this.XPos,
                IsOnline = this.IsOnline,
                ReservationStatus = this.ReservationStatus,
                Shape = this.Shape,
                TurnoverTime = this.TurnoverTime,
                ImageUri = this.ImageUri,
                Width = this.Width,
                Height = this.Height,
                Angle = this.Angle,
                IsDeleted = this.IsDeleted
            };
            return model;

        }

        public Table UpdateModel(Table model)
        {
            model.Code = this.Code;
            model.SalesDescription = this.SalesDescription;
            model.Description = this.Description;
            model.MinCapacity = this.MinCapacity;
            model.MaxCapacity = this.MaxCapacity;
            model.RegionId = this.RegionId;
            model.Status = this.Status;
            model.YPos = this.YPos;
            model.XPos = this.XPos;
            model.IsOnline = this.IsOnline;
            model.ReservationStatus = this.ReservationStatus;
            model.Shape = this.Shape;
            model.TurnoverTime = this.TurnoverTime;
            model.ImageUri = this.ImageUri;
            model.Width = this.Width;
            model.Height = this.Height;
            model.Angle = this.Angle;
            model.IsDeleted = this.IsDeleted;
            return model;
        }
    }
}