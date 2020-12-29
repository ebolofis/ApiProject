using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    /// <summary>
    /// Class that collects from tasks NFC Logic 
    /// </summary>
    public class NFCcardFlows : INFCcardFlows
    {

        INFCcardTasks nfcTasks;
        /// <summary>
        /// Constuctor initiallizing tasks needed for NFC flows functionality and usage
        /// </summary>
        /// <param name="nfcTasks">Interface of nfc Tasks implementing BI on nfc device</param>
        public NFCcardFlows(INFCcardTasks nfcTasks)
        {
            this.nfcTasks = nfcTasks;
        }

        /// <summary>
        /// Gets the first and only row of table NFC card
        /// </summary>
        /// <param name="store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <returns>Tasks selected NFC card</returns>
        public NFCcardModel SelectNFCcardTable(DBInfoModel store)
        {
            return nfcTasks.selectFirstNFCcardRecord(store);
        }
        /// <summary>
        /// Update the selected NFC row via tasks update functionality
        /// </summary>
        /// <param name="dbInfo">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="nfc">NFC Row to Edit</param>
        /// <returns>nfc loaded</returns>
        public bool UpdateNFCcardTable(DBInfoModel dbInfo, NFCcardModel nfcModel)
        {
            bool checkValidity = nfcTasks.checkValidityOfNFCcardModel(nfcModel);
            if (checkValidity)
            {
                NFCcardModel oldNFCCardRecord = nfcTasks.selectFirstNFCcardRecord(dbInfo);
                long Id = nfcTasks.updateFirstNFCcardRecord(dbInfo, nfcModel, oldNFCCardRecord);
            }
            return checkValidity;
        }
    }
}
