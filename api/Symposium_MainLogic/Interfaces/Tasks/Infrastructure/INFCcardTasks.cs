using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface INFCcardTasks
    {
        /// <summary>
        /// Select the First Record from table NFCcard
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        NFCcardModel selectFirstNFCcardRecord(DBInfoModel store);

        /// <summary>
        /// Function for checking the validity of the NFCcard Model , depending on the type of the card. Throws business exception if type is invalid.
        /// </summary>
        /// <param name="modeltoEval"></param>
        /// <returns></returns>
        bool checkValidityOfNFCcardModel(NFCcardModel modeltoEval);

        /// <summary>
        /// Update First Record from table NFCcard
        /// </summary>
        /// <param name="store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="nfcMod">Nfc Model provided for update</param>
        /// <returns>true if opperation end successfully</returns>
        long updateFirstNFCcardRecord(DBInfoModel store, NFCcardModel nfcMod, NFCcardModel oldNFCCardRecord);
    }
}
