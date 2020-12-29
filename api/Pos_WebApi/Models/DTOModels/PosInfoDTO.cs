using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos_WebApi.Models.DTOModels {
    public class PosInfoDTO : IDTOModel<PosInfo> {
        public PosInfoDTO() {
            AssosiatedPricelists = new HashSet<PosInfo_Pricelist_AssocDTO>();
            AssosiatedKDS = new HashSet<PosInfoKdsAssocDTO>();
            AssosiatedRegions = new HashSet<PosInfo_Region_AssocDTO>();
            AssosiatedKitchenInstructions = new HashSet<PosInfo_KitchenInstruction_AssocDTO>();
            AssosiatedStaffPositions = new HashSet<PosInfo_StaffPosition_AssocDTO>();
            PosInfoDetailTemplate = new HashSet<PosInfoDetailTemplateDTO>();

            //Code = this.Code
            ClearTableManually = false;
            CloseId = 0;
            FiscalType = this.FiscalType;
            IsOpen = false;
            LogInToOrder = false;
            Theme = "Light";
            ReceiptCount = 0;
            ViewOnly = false;
            wsIP = this.wsIP;
            wsPort = "4502";
            LoginToOrderMode = (short)LoginToOrderModeEnum.None;
            KeyboardType = (short)KeyboardTypeEnum.Numeric;
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? FODay { get; set; }
        public long? CloseId { get; set; }
        public string IPAddress { get; set; }
        public byte? Type { get; set; }
        public string wsIP { get; set; }
        public string wsPort { get; set; }
        public long? DepartmentId { get; set; }
        public string FiscalName { get; set; }
        public byte? FiscalType { get; set; }
        public bool? IsOpen { get; set; }
        public long? ReceiptCount { get; set; }
        public bool? ResetsReceiptCounter { get; set; }
        public string Theme { get; set; }
        public long? AccountId { get; set; }
        public bool? LogInToOrder { get; set; }
        public bool? ClearTableManually { get; set; }
        public bool? ViewOnly { get; set; }
        public bool? IsDeleted { get; set; }
        public int? InvoiceSumType { get; set; }
        public short? LoginToOrderMode { get; set; }
        public short? KeyboardType { get; set; }
        public Nullable<int> DefaultHotelId { get; set; }
        public string CustomerDisplayGif { get; set; }
        public string NfcDevice { get; set; }
        public string Configuration { get; set; }





        public virtual AccountsDTO Accounts { get; set; }

        public virtual ICollection<Kds> Kds { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<Notifications> Notifications { get; set; }
        public virtual ICollection<PosInfoDetail> PosInfoDetail { get; set; }
        public virtual ICollection<PdaModule> PdaModule { get; set; }
        public virtual ICollection<EODAccountToPmsTransfer> EODAccountToPmsTransfer { get; set; }
        public virtual ICollection<Region> Region { get; set; }
        public virtual ICollection<ClientPos> ClientPos { get; set; }

        //Many To Many
        public virtual ICollection<PosInfo_Pricelist_AssocDTO> AssosiatedPricelists { get; set; }
        public virtual ICollection<PosInfoKdsAssocDTO> AssosiatedKDS { get; set; }
        public virtual ICollection<PosInfo_Region_AssocDTO> AssosiatedRegions { get; set; }
        public virtual ICollection<PosInfo_KitchenInstruction_AssocDTO> AssosiatedKitchenInstructions { get; set; }
        public virtual ICollection<PosInfo_StaffPosition_AssocDTO> AssosiatedStaffPositions { get; set; }
        public virtual ICollection<PosInfoDetailTemplateDTO> PosInfoDetailTemplate { get; set; }


        //  public virtual ICollection<PagePosAssoc> PagePosAssoc { get; set; } Included in pageset
        //   public virtual ICollection<PosInfo_StaffPositin_Assoc> PosInfo_StaffPositin_Assoc { get; set; }


        //
        //public virtual ICollection<EndOfDay> EndOfDay { get; set; }
        //public virtual ICollection<Transactions> Transactions { get; set; }
        //public virtual ICollection<Invoices> Invoices { get; set; }
        //public virtual ICollection<MealConsumption> MealConsumption { get; set; }
        //public virtual ICollection<CreditTransactions> CreditTransactions { get; set; }




        public PosInfo ToModel() {
            var model = new PosInfo() {
                Id = this.Id,
                Code = this.Code,
                Description = this.Description,
                DepartmentId = this.DepartmentId,
                AccountId = this.AccountId,
                ClearTableManually = this.ClearTableManually,
                ClientPos = this.ClientPos,
                CloseId = this.CloseId,
                FiscalName = this.FiscalName,
                FiscalType = this.FiscalType,
                IPAddress = this.IPAddress,
                IsOpen = this.IsOpen,
                FODay = this.FODay,
                LogInToOrder = this.LogInToOrder,
                ResetsReceiptCounter = this.ResetsReceiptCounter,
                Theme = this.Theme,
                ReceiptCount = this.ReceiptCount,
                Type = this.Type,
                ViewOnly = this.ViewOnly,
                wsIP = this.wsIP,
                wsPort = this.wsPort,
                LoginToOrderMode = this.LoginToOrderMode,
                KeyboardType = this.KeyboardType,
                DefaultHotelId = this.DefaultHotelId,
                CustomerDisplayGif = this.CustomerDisplayGif,
                NfcDevice = this.NfcDevice,
                Configuration = this.Configuration,
            };

            foreach (var ap in this.AssosiatedPricelists) {
                model.PosInfo_Pricelist_Assoc.Add(ap.ToModel());
            }

            foreach (var ap in this.AssosiatedKDS) {
                model.PosInfoKdsAssoc.Add(ap.ToModel());
            }

            foreach (var ap in this.AssosiatedRegions) {
                model.PosInfo_Region_Assoc.Add(ap.ToModel());
            }

            foreach (var ap in this.AssosiatedKitchenInstructions) {
                model.PosInfo_KitchenInstruction_Assoc.Add(ap.ToModel());
            }
            foreach (var ap in this.AssosiatedStaffPositions) {
                model.PosInfo_StaffPositin_Assoc.Add(ap.ToModel());
            }

            if (this.Accounts != null) {
                model.Accounts = this.Accounts.ToModel();
            }

            if (this.Type == (int)ModuleType.Pos) {
                foreach (var it in PosInfoDetailTemplate) {
                    var pids = it.CreateDetails();
                    foreach (var pid in pids) {
                        model.PosInfoDetail.Add(pid);
                    }
                }
            }
            return model;
        }

        public PosInfo UpdateModel(PosInfo model) {
            model.Code = this.Code;
            model.Description = this.Description;
            model.DepartmentId = this.DepartmentId;
            model.AccountId = this.AccountId;
            model.ClearTableManually = this.ClearTableManually;
            model.ClientPos = this.ClientPos;
            model.CloseId = this.CloseId;
            model.FiscalName = this.FiscalName;
            model.FiscalType = this.FiscalType;
            model.IPAddress = this.IPAddress;
            model.IsOpen = this.IsOpen;
            model.FODay = this.FODay;
            model.LogInToOrder = this.LogInToOrder;
            model.ResetsReceiptCounter = this.ResetsReceiptCounter;
            model.Theme = this.Theme;
            model.ReceiptCount = this.ReceiptCount;
            model.Type = this.Type;
            model.ViewOnly = this.ViewOnly;
            model.wsIP = this.wsIP;
            model.wsPort = this.wsPort;
            model.LoginToOrderMode = this.LoginToOrderMode;
            model.KeyboardType = this.KeyboardType;
            model.ResetsReceiptCounter = this.ResetsReceiptCounter;
            model.DefaultHotelId = this.DefaultHotelId;
            model.CustomerDisplayGif = this.CustomerDisplayGif;
            model.NfcDevice = this.NfcDevice;
            model.Configuration = this.Configuration;

            foreach (var pg in this.AssosiatedPricelists.Where(w => w.IsDeleted == false)) {
                if (pg.Id == 0)
                    model.PosInfo_Pricelist_Assoc.Add(pg.ToModel());
                else {
                    var cur = model.PosInfo_Pricelist_Assoc.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null) {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }

            foreach (var pg in this.AssosiatedKDS.Where(w => w.IsDeleted == false)) {
                if (pg.Id == 0)
                    model.PosInfoKdsAssoc.Add(pg.ToModel());
                else {
                    var cur = model.PosInfoKdsAssoc.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null) {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }
            foreach (var pg in this.AssosiatedRegions.Where(w => w.IsDeleted == false)) {
                if (pg.Id == 0)
                    model.PosInfo_Region_Assoc.Add(pg.ToModel());
                else {
                    var cur = model.PosInfo_Region_Assoc.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null) {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }

            foreach (var pg in this.AssosiatedStaffPositions.Where(w => w.IsDeleted == false)) {
                if (pg.Id == 0)
                    model.PosInfo_StaffPositin_Assoc.Add(pg.ToModel());
                else {
                    var cur = model.PosInfo_StaffPositin_Assoc.FirstOrDefault(x => x.Id == pg.Id);
                    if (cur != null) {
                        pg.UpdateModel(pg.ToModel());
                    }

                }
            }

            if (this.Accounts != null) {
                model.Accounts = this.Accounts.ToModel();
            }

            return model;
        }
    }

}
