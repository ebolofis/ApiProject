using System.Collections.Generic;
using System.Linq;

namespace Pos_WebApi.Models.DTOModels {
    public class AccountsDTO : IDTOModel<Accounts> {
        public AccountsDTO() {
            PmsTransferMappings = new HashSet<EODAccountToPmsTransferDTO>();
        }


        public long Id { get; set; }
        public string Description { get; set; }
        public short? Type { get; set; }
        public bool? IsDefault { get; set; }
        public bool? SendsTransfer { get; set; }
        public bool IsDeleted { get; set; }
        public bool? CardOnly { get; set; }

        public ICollection<EODAccountToPmsTransferDTO> PmsTransferMappings { get; set; }

        public Accounts ToModel() {
            var model = new Accounts() {
                Id = this.Id,
                Description = this.Description,
                Type = this.Type,
                SendsTransfer = this.SendsTransfer,
                CardOnly = this.CardOnly
            };


            foreach (var ap in this.PmsTransferMappings) {
                model.EODAccountToPmsTransfer.Add(ap.ToModel());
            }
            return model;
        }

        public Accounts UpdateModel(Accounts model) {
            model.Description = this.Description;
            model.Type = this.Type;
            model.SendsTransfer = this.SendsTransfer;
            model.CardOnly = this.CardOnly;

            foreach (var pg in this.PmsTransferMappings) {
                if (pg.Id == 0)
                    model.EODAccountToPmsTransfer.Add(pg.ToModel());
                else {
                    var cur = model.EODAccountToPmsTransfer.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null) {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }
            return model;
        }
    }
}
