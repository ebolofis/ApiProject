using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos_WebApi.Models.DTOModels
{

    public class RegionDTO : IDTOModel<Region>
    {
        public RegionDTO()
        {
            AssociatedPos = new HashSet<PosInfo_Region_AssocDTO>();
            AssociatedTables = new HashSet<TableDTO>();
        }

        public long? Id { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string BluePrintPath { get; set; }
        public int? MaxCapacity { get; set; }
        public long? PosInfoId { get; set; }
        public bool? IsLocker { get; set; }

        public ICollection<PosInfo_Region_AssocDTO> AssociatedPos { get; set; }
        public ICollection<TableDTO> AssociatedTables { get; set; }


        public Region ToModel()
        {
            var model = new Region()
            {
                Description = this.Description,
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                BluePrintPath = this.BluePrintPath,
                MaxCapacity = this.MaxCapacity,
                IsLocker = this.IsLocker

            };

            foreach (var ap in this.AssociatedTables)
            {
                model.Table.Add(ap.ToModel());
            }

            foreach (var ap in this.AssociatedPos)
            {
                model.PosInfo_Region_Assoc.Add(ap.ToModel());
            }


            return model;
        }

        public Region UpdateModel(Region model)
        {

            model.Description = this.Description;
            model.StartTime = this.StartTime;
            model.EndTime = this.EndTime;
            model.BluePrintPath = this.BluePrintPath;
            model.MaxCapacity = this.MaxCapacity;
            model.IsLocker = this.IsLocker;

            foreach (var pg in this.AssociatedTables.Where(w => (w.IsDeleted ?? false) == false))
            {
                if (pg.Id == 0)
                    model.Table.Add(pg.ToModel());
                else
                {
                    var cur = model.Table.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null)
                    {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }
            foreach (var pg in this.AssociatedPos.Where(w => w.IsDeleted == false))
            {
                if (pg.Id == 0)
                    model.PosInfo_Region_Assoc.Add(pg.ToModel());
                else
                {
                    var cur = model.PosInfo_Region_Assoc.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null)
                    {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }

            return model;
        }
    }
}