using log4net;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class NFCcardTasks : INFCcardTasks
    {
        INFCcardDT cardDT;
        /// <summary>
        /// Constuctor of nfc tasks initiallizing DT usage of BI
        /// </summary>
        /// <param name="nfccardDT">instance of DT interface of nfc card functionality </param>

        public NFCcardTasks(INFCcardDT nfccardDT)
        {
            this.cardDT = nfccardDT;
        }
        /// <summary>
        /// Select the First Record from table NFCcard
        /// </summary>
        /// <param name="store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <returns></returns>
        public NFCcardModel selectFirstNFCcardRecord(DBInfoModel store)
        {
            return cardDT.GetFirstNFCInfo(store);
        }

        /// <summary>
        /// Function for checking the validity of the NFCcard Model , depending on the type of the card. Throws business exception if type is invalid.
        /// </summary>
        /// <param name="modeltoEval"></param>
        /// <returns></returns>
        public bool checkValidityOfNFCcardModel(NFCcardModel modeltoEval)
        {
            bool res = false;
            if (modeltoEval.Type == (int)NFCCardsEnum.MifareClassic)
            {
                res = mifareClassiceval(modeltoEval);
            }
            else if (modeltoEval.Type == (int)NFCCardsEnum.MifareUltralight)
            {
                res = mifareUltralighteval(modeltoEval);
            }
            else
            {
                throw new BusinessException("Invalid card type!", "Use mifare classic or mifare ultra light!");
            }
            return res;
        }

        /// <summary>
        /// Update First Record from table NFCcard
        /// </summary>
        /// <param name="store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="nfcMod">Nfc Model provided for update</param>
        /// <returns>true if opperation end successfully</returns>
        public long updateFirstNFCcardRecord(DBInfoModel store, NFCcardModel nfcMod, NFCcardModel oldNFCCardRecord)
        {
            oldNFCCardRecord.Type = nfcMod.Type;
            oldNFCCardRecord.RoomSector = nfcMod.RoomSector;
            oldNFCCardRecord.FirstDateSector = nfcMod.FirstDateSector;
            oldNFCCardRecord.SecondDateSector = nfcMod.SecondDateSector;
            oldNFCCardRecord.ValidateDate = nfcMod.ValidateDate;
            return cardDT.UpdateNFCcard(store, oldNFCCardRecord);
        }

        /// <summary>
        /// Checks mifare classic cards. The card has 64 sectors. If the model has values over 63 or under 0 throw business exception.
        /// </summary>
        /// <param name="classicModelEval"></param>
        /// <returns></returns>
        private bool mifareClassiceval(NFCcardModel classicModelEval)
        {
            bool result = false;
            if (classicModelEval.RoomSector < 0 || classicModelEval.RoomSector > 63)
            {
                result = false;
                throw new BusinessException(Symposium.Resources.Errors.INVALIDCARDMODEL, string.Format(Symposium.Resources.Errors.INVALIDROOMSECTOR, classicModelEval.RoomSector));
            }
            else if (classicModelEval.ValidateDate && (classicModelEval.FirstDateSector < 0 || classicModelEval.FirstDateSector > 63 || classicModelEval.SecondDateSector < 0 || classicModelEval.SecondDateSector > 63))
            {
                result = false;
                throw new BusinessException(Symposium.Resources.Errors.INVALIDCARDMODEL, string.Format(Symposium.Resources.Errors.INVALIDDATESECTOR, classicModelEval.FirstDateSector + " " + classicModelEval.SecondDateSector));
            }
            else {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Checks mifare ultralight cards. The card has 16 pages. If the model has values over 15 or under 0 throw business exception.
        /// </summary>
        /// <param name="ultralightModelEval"></param>
        /// <returns></returns>
        private bool mifareUltralighteval(NFCcardModel ultralightModelEval)
        {
            bool result = false;
            if (ultralightModelEval.RoomSector < 0 || ultralightModelEval.RoomSector > 15)
            {
                result = false;
                throw new BusinessException(Symposium.Resources.Errors.INVALIDCARDMODEL, string.Format(Symposium.Resources.Errors.INVALIDROOMSECTOR, ultralightModelEval.RoomSector));
            }
            else if (ultralightModelEval.ValidateDate && (ultralightModelEval.FirstDateSector < 0 || ultralightModelEval.FirstDateSector > 15 || ultralightModelEval.SecondDateSector < 0 || ultralightModelEval.SecondDateSector > 15))
            {
                result = false;
                throw new BusinessException(Symposium.Resources.Errors.INVALIDCARDMODEL, string.Format(Symposium.Resources.Errors.INVALIDDATESECTOR, ultralightModelEval.FirstDateSector, ultralightModelEval.SecondDateSector));
            }
            else
            {
                result = true;
            }
            return result;
        }
    }

}
