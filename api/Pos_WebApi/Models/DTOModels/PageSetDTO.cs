using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos_WebApi.Models.DTOModels
{
    public class PageSetDTO : IDTOModel<PageSet>
    {
        public PageSetDTO()
        {
            Pages = new HashSet<PagesDTO>();
        }
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? DeactivationDate { get; set; }
        public long? PosInfoId { get; set; }
        public long? PdaModuleId { get; set; }

        public ICollection<PagesDTO> Pages { get; set; }
        public ICollection<PagePosAssocDTO> AssosiatedPos { get; set; }

        public PageSet ToModel()
        {
            PageSet model = new PageSet();
            model.Id = this.Id;
            model.Description = this.Description;
            model.ActivationDate = this.ActivationDate;
            model.DeactivationDate = this.DeactivationDate;

            foreach (var pg in this.Pages)
            {
                model.Pages.Add(pg.ToModel());
            }
            foreach (var ap in this.AssosiatedPos)
            {
                model.PagePosAssoc.Add(ap.ToModel());
            }
            return model;
        }


        public PageSet UpdateModel(PageSet model)
        {
            model.Description = this.Description;
            model.ActivationDate = this.ActivationDate;
            model.DeactivationDate = this.DeactivationDate;

            foreach (var pg in this.Pages)
            {
                if (pg.Id == 0)
                    model.Pages.Add(pg.ToModel());
                else
                {
                    var cur = model.Pages.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null)
                    {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }

            foreach (var ap in this.AssosiatedPos)
            {
                if (ap.Id == 0)
                    model.PagePosAssoc.Add(ap.ToModel());
                else
                {
                    var cur = model.PagePosAssoc.FirstOrDefault(x => x.Id == ap.Id);
                    if (cur != null)
                    {
                        ap.UpdateModel(ap.ToModel());
                    }
                }
            }

            return model;
        }
    }

    //public class PageSetDTOSingle

    //public IEnumerable<long> PosInfoId { get; set; }
    //public IEnumerable<long> PdaModuleId { get; set; }


    public class PageSetGroupedDTO : PageSetDTO
    {
        public IEnumerable<long?> PosInfoIds { get; set; }
        public IEnumerable<long?> PdaModuleIds { get; set; }
    }
}
