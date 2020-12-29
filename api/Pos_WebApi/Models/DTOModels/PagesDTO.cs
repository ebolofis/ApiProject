using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos_WebApi.Models.DTOModels {
    public class PagesDTO : IDTOModel<Pages> {
        public PagesDTO() {
            PageButtons = new HashSet<PageButtonDTO>();
        }

        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public long Id { get; set; }
        public string Description { get; set; }
        public string ExtendedDesc { get; set; }
        public long? DefaultPriceListId { get; set; }
        public long? PageSetId { get; set; }
        public short? Sort { get; set; }
        public bool? Status { get; set; }

        public bool? IsDeleted { get; set; }


        public ICollection<PageButtonDTO> PageButtons { get; set; }

        public Pages ToModel() {
            try {
                Pages model = new Pages {
                    Id = this.Id,
                    Description = this.Description,
                    DefaultPriceList = this.DefaultPriceListId,
                    ExtendedDesc = this.ExtendedDesc,
                    PageSetId = this.PageSetId,
                    Sort = this.Sort,
                    Status = this.Status
                };

                foreach (var pb in PageButtons.Where(w => w.IsDeleted == false)) {
                    PageButton ex = new PageButton() {
                        Id = pb.Id,
                        ProductId = pb.ProductId,
                        SetDefaultPriceListId = pb.SetDefaultPriceListId,
                        Background = pb.Background,
                        Color = pb.Color,
                        Description = pb.Description,
                        SetDefaultSalesType = pb.SetDefaultSalesType,
                        NavigateToPage = pb.NavigateToPage,
                        Sort = pb.Sort,
                        Type = pb.Type,
                        PageId = pb.PageId,
                    };
                    model.PageButton.Add(ex);
                }

                return model;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
               // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("PagesDTO ToModel() : " + Id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
            return null;
        }

        public Pages UpdateModel(Pages model) {
            try {
                model.Description = this.Description;
                model.DefaultPriceList = this.DefaultPriceListId;
                model.ExtendedDesc = this.ExtendedDesc;
                model.PageSetId = this.PageSetId;
                model.Sort = this.Sort;
                model.Status = this.Status;

                foreach (var pb in model.PageButton.Where(w => w.Id != 0)) {
                    var cb = this.PageButtons.FirstOrDefault(w => w.Id == pb.Id);
                    if (cb != null) {
                        pb.ProductId = cb.ProductId;
                        pb.SetDefaultPriceListId = cb.SetDefaultPriceListId;
                        pb.Background = cb.Background;
                        pb.Color = cb.Color;
                        pb.Description = cb.Description;
                        pb.SetDefaultSalesType = cb.SetDefaultSalesType;
                        pb.NavigateToPage = cb.NavigateToPage;
                        pb.Sort = cb.Sort;
                        pb.Type = cb.Type;
                        pb.PageId = cb.PageId;
                    }
                    //  model.PageButton.Add(pb);
                }

                foreach (var pb in this.PageButtons.Where(w => w.Id == 0)) {

                    PageButton ex = new PageButton() {
                        //Id = pb.Id,
                        ProductId = pb.ProductId,
                        SetDefaultPriceListId = pb.SetDefaultPriceListId,
                        Background = pb.Background,
                        Color = pb.Color,
                        Description = pb.Description,
                        SetDefaultSalesType = pb.SetDefaultSalesType,
                        NavigateToPage = pb.NavigateToPage,
                        Sort = pb.Sort,
                        Type = pb.Type,
                        PageId = pb.PageId,
                    };
                    model.PageButton.Add(ex);
                }

                return model;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Updating PagesDTO ToModel() : " + Id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
            return model;
        }

        public object[] GetKeys() {
            throw new NotImplementedException();
        }
    }
}