using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class InvoiceModel {
        public Nullable<long> Id { get; set; }
        public string Description { get; set; }

        //tipos tou deltiou
        public Nullable<long> InvoiceTypeId { get; set; }

        //arithmos parastatikou apo PosInfoDetail.Counter
        public Nullable<int> Counter { get; set; }

        //imerominia dimourgias eggrafis
        public Nullable<System.DateTime> Day { get; set; }

        //1 printed, 0 = not printed
        public Nullable<bool> IsPrinted { get; set; }

        //pelatis
        public Nullable<long> GuestId { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Net { get; set; }
        public Nullable<long> StaffId { get; set; }
        public Nullable<int> Cover { get; set; }
        public Nullable<long> TableId { get; set; }
        public Nullable<long> PosInfoId { get; set; }

        //PDA gia to opoio egine paragelia
        public Nullable<long> PdaModuleId { get; set; }

        //client POS gia to poio egine paragelia
        public Nullable<long> ClientPosId { get; set; }
        public Nullable<long> EndOfDayId { get; set; }
        public Nullable<long> PosInfoDetailId { get; set; }

        //1 = to parastatiko ine akiromeno
        public Nullable<bool> IsVoided { get; set; }

        //1 = deleted
        public Nullable<bool> IsDeleted { get; set; }

        //aitiologia ekptosis
        public string DiscountRemark { get; set; }

        //perigrafi tropou pliromis
        public string PaymentsDesc { get; set; }

        //0 = den exei ejoflithei kanena antikimeno, 1 = merika antikimena exou nejoflithei, 2 = ola ta antikimena exoun ejoflithei
        public short IsPaid { get; set; }
        public Nullable<decimal> PaidTotal { get; set; }
        public string Rooms { get; set; }
        public string OrderNo { get; set; }

        //0 = den ine deltio paragelias 'h deltio paragelias ki den exei vgei antistixi apdijei, 1 = deltio paragelias kai exei vgei antistixei apodijei
        public bool IsInvoiced { get; set; }
        public string Hashcode { get; set; }

        //athroisma posou antikimenon sto trapezi
        public Nullable<decimal> TableSum { get; set; }

        //poso me to opoio plirose o pelatis
        public string CashAmount { get; set; }
        public string BuzzerNumber { get; set; }

        public decimal? LoyaltyDiscount { get; set; }

        public string ForeignExchangeCurrency { get; set; }

        public decimal? ForeignExchangeTotal { get; set; }

        public decimal? ForeignExchangeDiscount { get; set; }

        public string ExtECRCode { get; set; }
    }
}
