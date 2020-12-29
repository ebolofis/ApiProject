using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotel
{
    public class Hotel_CustomMessagesModel
    {
        /// <summary>
        ///  Table Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// το ονομα του πεδίου όπως έρχεται από τη stored procedure και το νέο μοντέλο πελάτη
        /// </summary>
        public string Param { get; set; }
        /// <summary>
        /// η τιμή του πεδίου
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// το κειμενο το μηνύματος.
        /// πχ "Ο πελάτης είναι <value>.", όπου το <value> θα αντικατασταθεί από την τιμή του Value 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// προτεραιώτητα. Τιμές: 1-1000
        /// </summary>
        public int Priority { get; set; }
    }

}
