using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface INFCcardDT
    {
        /// <summary>
        /// Get NFC card
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <returns>List of all nfc card registers</returns>
        List<NFCcardModel> GetNFCInfo(DBInfoModel Store);
        
        /// <summary>
        /// Get first NFC card
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <returns>First or default of NFC model on db context</returns>
        NFCcardModel GetFirstNFCInfo(DBInfoModel Store);

        /// <summary>
        /// Upsert NFC card
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="nfcCard">Model to update register</param>
        /// <returns>Id of model updated</returns>
        long UpdateNFCcard(DBInfoModel Store, NFCcardModel nfcCard);
    }
}
