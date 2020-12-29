using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models.DTOModels {
    public class PayroleDTO : IDTOModel<Payrole> {
        public long Id { get; set; }
        public string Identification { get; set; }
        public System.DateTime ActionDate { get; set; }
        public int Type { get; set; }
        public long PosInfoId { get; set; }
        public long StaffId { get; set; }
        public string ShopId { get; set; }

        public Payrole ToModel() {
            var model = new Payrole() {
                Id = this.Id,
                Identification = this.Identification,
                ActionDate = this.ActionDate,
                Type = this.Type,
                PosInfoId = this.PosInfoId,
                StaffId = this.StaffId,
                ShopId = this.ShopId,
            };
            //foreach (var det in this.ProductRecipe) { model.ProductRecipe.Add(det.ToModel());}
            //foreach (var det in this.ProductExtras) { model.ProductExtras.Add(det.ToModel()); }
            //foreach (var det in this.ProductBarcodes) { model.ProductBarcodes.Add(det.ToModel());}
            //foreach (var det in this.ProductPrices) { model.PricelistDetail.Add(det.ToModel()); }
            return model;
        }
        public Payrole UpdateModel(Payrole model) {
            model.Id = this.Id;
            model.Identification = this.Identification;
            model.ActionDate = this.ActionDate;
            model.Type = this.Type;
            model.PosInfoId = this.PosInfoId;
            model.StaffId = this.StaffId;
            model.ShopId = this.ShopId;

            return model;
        }
    }
}
