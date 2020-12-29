using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    /// <summary>
    /// Handles NFC card activities
    /// </summary>
    public interface INFCcardFlows
    {
        /// <summary>
        /// Gets the first and only row of table NFC card
        /// </summary>
        /// <param name="store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <returns>Tasks selected NFC card</returns>
        NFCcardModel SelectNFCcardTable(DBInfoModel store);

        /// <summary>
        /// Update the selected NFC row via tasks update functionality
        /// </summary>
        /// <param name="store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="nfc">NFC Row to Edit</param>
        /// <returns>nfc loaded</returns>
        bool UpdateNFCcardTable(DBInfoModel store, NFCcardModel nfcModel);
    }
}
