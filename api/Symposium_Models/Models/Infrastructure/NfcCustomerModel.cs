using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Model the keeps Data read/write from/to NFC card 
    /// </summary>
  public  class NfcCustomerModel 
    {
        /// <summary>
        /// Room Number
        /// </summary>
       public string RoomNo { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// In case of Error, error description returns here
        /// </summary>
        public string ErrorMessage { get; set; } = "";
    }
}
