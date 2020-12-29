using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotelizer
{
    public class HotelizerResponcePostOrderModel
    {
        /// <summary>
        /// Responce result
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// model with data
        /// </summary>
        public List<HotelizerResponcePostOrderDataModel> data { get; set; }

        /// <summary>
        /// model with updates ???? no more info from Hotelizer
        /// </summary>
        public HotelizerResponcePostOrderUpdatesModel updates { get; set; }

    }

    public class HotelizerResponcePostOrderDataModel
    {
        public int id { get; set; }

        public string order_no { get; set; }

        public string guid { get; set; }

        public DateTime due_date { get; set; }

        public int active { get; set; }

        public int? reservation_room_id { get; set; }

        public int integration_id { get; set; }

        public int? pos_table_id { get; set; }

        public decimal net_total { get; set; }

        public decimal vat_net_total { get; set; }

        public decimal taxes_total { get; set; }

        public decimal vat_taxes_total { get; set; }

        public decimal grand_total { get; set; }

        public decimal paid_total { get; set; }

        public decimal balance { get; set; }

        public int? night_audit_id { get; set; }

        public DateTime created { get; set; }

        public DateTime modified { get; set; }

        public int property_id { get; set; }

        public HotelizerResponceInvoiceTypesModel invoice_types { get; set; }

        public HotelizerResponcePostOrderIntegrationsModel integrations { get; set; }

        public HotelizerResponcePostOrderReservationRoomsModel reservation_rooms { get; set; }
    }

    public class HotelizerResponcePostOrderIntegrationsModel
    {
        public int id { get; set; }

        public string name { get; set; }
    }


    public class HotelizerResponcePostOrderReservationRoomsModel
    {
        public int id { get; set; }

        public int reservation_id { get; set; }

        public DateTime fromd { get; set; }

        public DateTime tod { get; set; }

        public int room_id { get; set; }

        public int room_type_id { get; set; }

        public int reservation_guest_id { get; set; }

        public HotelizerResponcePostOrderReservationGuestsModel reservation_guests { get; set; }

        public HotelizerResponcePostOrderReservationsModel reservations { get; set; }

        public HotelizerResponcePostOrderRoomTypesModel room_types { get; set; }

        public HotelizerResponcePostOrderRoomsModel rooms { get; set; }
    }

    public class HotelizerResponceInvoiceTypesModel
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class HotelizerResponcePostOrderReservationGuestsModel
    {
        public int id { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }
    }

    public class HotelizerResponcePostOrderReservationsModel
    {
        public int id { get; set; }

        public string title { get; set; }
    }

    public class HotelizerResponcePostOrderRoomTypesModel
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class HotelizerResponcePostOrderRoomsModel
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class HotelizerResponcePostOrderUpdatesModel
    {

    }
}
