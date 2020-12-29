using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Model that describes an Account
    /// </summary>
    public class PosInfoModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        //imerominia teleuteas enarjis imeras
        public Nullable<System.DateTime> FODay { get; set; }

        //poses fores exei ginei klisimo imeras
        public Nullable<long> CloseId { get; set; }

        //string gia POS registration
        public string IPAddress { get; set; }

        //1 = POS, 2 = KDS
        public Nullable<byte> Type { get; set; }

        //tmima sto opoio anikei to POS
        public Nullable<long> DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }
        public string FiscalName { get; set; }

        //1 = generic, 2 = OPOS
        public Nullable<Int16> FiscalType { get; set; }

        //0 = exei ginei klisimo imeras, 1 = exei ginei anigma imeras
        public Nullable<bool> IsOpen { get; set; }
        public Nullable<long> ReceiptCount { get; set; }

        //0 = o ReceiptCount den midenizete, 1 = o ReceiptCount midenizete se kathe klisimo imeras
        public Nullable<bool> ResetsReceiptCounter { get; set; }

        //tropos pliromis
        public Nullable<long> AccountId { get; set; }

        //1 = o servitoros ginete logout meta tin paragelia
        public Nullable<bool> LogInToOrder { get; set; }

        //sto trapzei diatirounte kai ta antikimena pou exoun ejoflithei
        public Nullable<bool> ClearTableManually { get; set; }

        //1 = apenergopoihsh POS
        public Nullable<bool> IsDeleted { get; set; }

        //periexei ta athroistika posa ton paragelion
        public Nullable<int> InvoiceSumType { get; set; }

        //an exei klidothei i othoni tou POS tote kata to login: 0 = zitaei password, 1 = den zita password
        public Nullable<short> LoginToOrderMode { get; set; }

        //tipos eisagogis gia anazitisi pelati: 0 = arithmitiko pliktrologio, 1 = alfarithmitiko pliktrologio, 2 = apo karta nfc, 3 = arithmitiko pliktrologio 'h karta nfc, 4 = alfarithmitiko pliktrologio 'h apo karta nfc
        public Nullable<short> KeyboardType { get; set; }
        public string CustomerDisplayGif { get; set; }

        //i default timi tou pms gia to pos
        public Nullable<int> DefaultHotelId { get; set; }

        //to signalR group gia epikinonia tou pos me ton WebPosNfcDriver
        public string NfcDevice { get; set; }

        // Configuration file name for specific pos
        public string Configuration { get; set; }

        public List<PosInfoDetail> PosInfoDetail { get; set; }

    }

    public class PosInfoDetail {
        public long Id { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<long> Counter { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public Nullable<bool> ResetsAfterEod { get; set; }
        public Nullable<short> InvoiceId { get; set; }
        public string ButtonDescription { get; set; }
        public Nullable<short> Status { get; set; }
        public Nullable<bool> CreateTransaction { get; set; }
        public Nullable<byte> FiscalType { get; set; }
        public Nullable<bool> IsInvoice { get; set; }
        public Nullable<bool> IsCancel { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<long> InvoicesTypeId { get; set; }
        public Nullable<bool> IsPdaHidden { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<short> SendsVoidToKitchen { get; set; }
        public virtual ICollection<PosInfoDetail_Excluded_AccountsModel> PosInfoDetail_Excluded_Accounts { get; set; }
        public List<PosInfoDetail_Pricelist_Assoc_Model> PosInfoDetail_Pricelist_Assoc { get; set; }
    }

    public class PosInfoDetail_Pricelist_Assoc_Model
    {
        public long Id { get; set; }
        public Nullable<long> PosInfoDetailId { get; set; }
        public Nullable<long> PricelistId { get; set; }
    }

}