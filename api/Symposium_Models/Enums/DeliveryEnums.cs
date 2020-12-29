using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    public enum DeliveryCustomerSignalCause
    {
        New, Edit, Delete
    }

    public enum ExternalSystemOrderEnum
    {
        Default = 0,
        Vivardia = 1,
        VivardiaNoKitchen = 2,
        Forkey = 3,
        ICoupon = 4,
        DeliveryAgent = 5,
        GoodysOldProject = 6,
        Hotelizer = 7
    }

    public enum DeliveryForkeyPaymentEnum
    {
        CASH = 0,
        MPOS = 1, 
        OTHER = 2
    }
    public enum DeliveryForkeyStatusEnum
    {
        downloaded =0 , printed =1 , assigned=2, delivered =3, cancelled =4 
    }

    public enum DeliveryForkeyErrorEnum
    {
        /// <summary>
        /// Κανένα σφαλμα 
        /// </summary>
        NoError = 0,
        /// <summary>
        /// Η παραγγελία δεν έχει όνομα πελάτη
        /// </summary>
        MISSING_USER_NAME = 1,
        /// <summary>
        /// Η παραγγελία δεν έχει διεύθυνση παράδωσης
        /// </summary>
        MISSING_ADDRESS = 2,
        /// <summary>
        /// Η παραγγελία δεν έχει τηλέφωνο
        /// </summary>
        MISSING_TEL = 3,
        /// <summary>
        /// Παραγγελία τυπου πληρομής τιμολόγιο χωρίς στοιχεία παραστατισκού
        /// </summary>
        MISSING_INVOICE_INFO = 4,
        /// <summary>
        /// Σφάλμα κατά την αναζήτηση προιόντος στην webPos με  product.Code == order.dishes.dish.recipe.external_id
        /// </summary>
        MISSING_DISH_ID = 5,
        /// <summary>
        /// Tο άθροισμα των cost για κάθε dish δεν είναι ίσο με το συνολικό ποσό.
        /// </summary>
        COST_MISMATCH = 6,
        /// <summary>
        /// Tο ποσοστό του φορολογικού συντελεστή δεν υπάρχει στη βάση.
        /// </summary>
        VAT_MISMATCH = 7,

        NO_DISHES = 9,
        ALLREADY_PROCESSED = 10,
        MORE_THAN_ONE_SAMEID = 11
    }
}
