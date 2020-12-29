using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.OldGoodysModel
{
    public class OldWebGoodysOrderModel
    { 
        /// <summary>
        /// orderno
        /// </summary>
        public long orderno { get; set; }

        /// <summary>
        /// shopId e.x "1-F6JT"
        /// </summary>
        public string shopId { get; set; }

        /// <summary>
        /// shop e.x "ΝΕΑ ΣΜΥΡΝΗ"
        /// </summary>
        public string shop { get; set; }

        /// <summary>
        /// software e.x "OMNIREST"
        /// </summary>
        public string software { get; set; }

        /// <summary>
        /// shop e.x "N"
        /// </summary>
        public string takeAway { get; set; }

        /// <summary>
        /// shop e.x "1-3DI4PD"
        /// </summary>
        public string srId { get; set; }

        /// <summary>
        /// shop e.x "Jun 13, 2019 9:20:00 PM"
        /// </summary>
        public DateTime statusTime { get; set; }

        /// <summary>
        /// shop e.x "1-3DI4N9"
        /// </summary>
        public string room { get; set; }

        /// <summary>
        /// payment e.x "AP"
        /// </summary>
        public string payment { get; set; }

        /// <summary>
        /// comments e.x "ΧΤΥΠΗΣΤΕ ΤΟ ΚΟΥΔΟΥΝΙ"
        /// </summary>
        public string comments { get; set; }

        /// <summary>
        /// mqty e.x 2
        /// </summary>
        public int mqty { get; set; }

        /// <summary>
        /// deliveryTime e.x "Jun 13, 2019 10:05:00 PM"
        /// </summary>
        public DateTime deliveryTime { get; set; }

        public List<OldWebGoodysProductsModel> products { get; set; }

        public OldWebGoodysCustomerModel customer { get; set; }
    }

    public class OldWebGoodysProductsModel
    {
        /// <summary>
        /// item_code e.x  "3030"
        /// </summary>
        public long item_code { get; set; }

        /// <summary>
        /// item_descr e.x "Cheddar Bacon Extreme Deluxe"
        /// </summary>
        public string item_descr { get; set; }

        /// <summary>
        /// item_vat e.x "2"
        /// </summary>
        public string item_vat { get; set; }

        /// <summary>
        /// qty e.x "1"
        /// </summary>
        public string qty { get; set; }

        /// <summary>
        /// amount e.x "710"
        /// </summary>
        public string amount { get; set; }

        /// <summary>
        /// total e.x "710"
        /// </summary>
        public string total { get; set; }

        /// <summary>
        /// isCombined e.x 0
        /// </summary>
        public int isCombined { get; set; }

        /// <summary>
        /// discount_per e.x "0"
        /// </summary>
        public string discount_per { get; set; }

        /// <summary>
        /// discount_amount e.x "0"
        /// </summary>
        public string discount_amount { get; set; }

        /// <summary>
        /// total_disc e.x "0"
        /// </summary>
        public string total_disc { get; set; }
    }

    public class OldWebGoodysCustomerModel
    {
        /// <summary>
        /// customerid e.x  "1-3DI4N9"
        /// </summary>
        public string customerid { get; set; }

        /// <summary>
        /// accountId e.x "1-3DI4N5"
        /// </summary>
        public string accountId { get; set; }

        /// <summary>
        /// addressId e.x "1-3DI4N8"
        /// </summary>
        public string addressId { get; set; }

        /// <summary>
        /// name e.x "ΧΡΥΣΑΥΓΗ"
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// fname e.x "ΤΑΣΤΣΙΔΟΥ"
        /// </summary>
        public string fname { get; set; }

        /// <summary>
        /// tel1 e.x "+306936641666"
        /// </summary>
        public string tel1 { get; set; }

        /// <summary>
        /// shop e.x "1-3DI4N9"
        /// </summary>
        public string room { get; set; }

        /// <summary>
        /// address1 e.x "ΔΟΙΡΑΝΗΣ"
        /// </summary>
        public string address1 { get; set; }

        /// <summary>
        /// addressNo e.x "46"
        /// </summary>
        public string addressNo { get; set; }

        /// <summary>
        /// orofos1 e.x "4"
        /// </summary>
        public string orofos1 { get; set; }

        /// <summary>
        /// city e.x "ΚΑΛΛΙΘΕΑ"
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// zipcode e.x "17672"
        /// </summary>
        public string zipcode { get; set; }

        /// <summary>
        /// email e.x "chrysavgitastsidou@gmail.com"
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// store e.x "1-F6JT"
        /// </summary>
        public string store { get; set; }
    }

}
