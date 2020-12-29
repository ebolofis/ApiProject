using log4net;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Helpers
{
    public partial class SendTransferRepository : IDisposable
    {
        protected PosEntities db;
        protected long posInfoId;
        protected long posDepartment;
        protected long defaultHotelId = 1;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SendTransferRepository(PosEntities dbcontext)
        {
            db = dbcontext;
        }

        public SendTransferRepository(PosEntities dbcontext, long PosInfoId, long posDepartmentId)
        {
            db = dbcontext;
            posInfoId = PosInfoId;
            posDepartment = posDepartmentId;

            var hi = db.HotelInfo.FirstOrDefault();
            if (hi != null)
                defaultHotelId = hi.HotelId ?? 1;
        }

        private TransferToPms WriteTransfer(long receiptCounter, string pmsDeparmentId, string pmsDepartment, long posInfoDetailId, string pmsRoom, decimal amount,short? transferType, long? pmsPaymentId, long? pmsInvoiceId)
        {
            TransferToPms ttp = new TransferToPms();
            ttp.Description = "Rec: " + receiptCounter + ", Pos: " + posInfoId + ", " + posDepartment;
            ttp.PmsDepartmentId = pmsDeparmentId;
            ttp.PosInfoDetailId = posInfoDetailId;
            ttp.ReceiptNo = receiptCounter.ToString();
            ttp.RoomDescription = pmsRoom;
            ////Set Status Flag (0: Cash, 1: Not Cash)
            ttp.PosInfoId = posInfoId;
            ttp.PmsDepartmentDescription = pmsDepartment;
            //tpms.TransactionId = tr.Id;
            ttp.TransferType = 0;//Xrewstiko
            ttp.SendToPmsTS = DateTime.Now;
            ttp.Total = amount;
            var identifier = Guid.NewGuid();
            ttp.TransferIdentifier = identifier;
            ttp.PMSPaymentId = pmsPaymentId;
            ttp.PMSInvoiceId = pmsInvoiceId;
            return ttp;
        }

        public TransferToPms WriteCashToTransfer(long receiptCounter, string pmsDeparmentId, string pmsDepartment, long posInfoDetailId, string accountDescription, string pmsRoom, decimal amount, bool isFiscal, bool isImmediateTransfer, long? pmsPaymentId, long? pmsInvoiceId, bool sendtopms = false, long? hotelId = null)
        {
            TransferToPms ttp = WriteTransfer(receiptCounter, pmsDeparmentId, pmsDepartment, posInfoDetailId, pmsRoom, amount, 0, pmsPaymentId, pmsInvoiceId);
            ttp.ProfileName = accountDescription;
            ttp.RegNo = "0";
            ttp.RoomId = "";
            ttp.SendToPMS = (sendtopms && !isFiscal) || (isImmediateTransfer && sendtopms && isFiscal) ;
            ////Set Status Flag (0: Cash, 1: Not Cash)
            ttp.Status = (short)0;
            //tpms.TransactionId = tr.Id;
            ttp.TransferType = 0;//Xrewstiko
            ttp.Total = amount;
            ttp.HotelId = hotelId ?? defaultHotelId;
            if (!isImmediateTransfer && isFiscal && sendtopms)
                ttp.Status = 2;
            return ttp;

        }
        public TransferToPms WriteRoomChargeToTransfer(long receiptCounter, string pmsDeparmentId, string pmsDepartment, long posInfoDetailId, string profileId, string profileName,
                                                        string regNo, string pmsRoomId, string pmsRoom, decimal amount, bool sendToPMS, long? pmsPaymentId, long? pmsInvoiceId, long? hotelId = null)
        {
            

            TransferToPms ttp = WriteTransfer(receiptCounter, pmsDeparmentId, pmsDepartment, posInfoDetailId, pmsRoom, amount, 0, pmsPaymentId, pmsInvoiceId);
            ttp.ProfileId = profileId;
            ttp.ProfileName = profileName;
            ttp.RoomId = pmsRoomId;
            ttp.RoomDescription = pmsRoom;
            ttp.RegNo = regNo;
            ttp.SendToPMS = sendToPMS;
            ttp.HotelId = hotelId ?? defaultHotelId;
            if (!sendToPMS)
                ttp.Status = 2;

            return ttp;

        }

        public TransferToPms WriteEODToTransfer(long posInfoId, string posName, string pmsDepartmentId, string pmsDepDescription, string accountDescription, string room,decimal? total, long? hotelId)
        {
            // Create insert to local transfertopms table
            TransferToPms tpms = new TransferToPms();
            tpms.Description = "Pos:" + posInfoId + " PosName: " + posName + " Descr:" + accountDescription + " Ημέρας  EOD";
            tpms.PmsDepartmentId = pmsDepartmentId;
            tpms.PosInfoId = posInfoId;
            tpms.PmsDepartmentDescription = pmsDepDescription;
            tpms.SendToPmsTS = DateTime.Now;
            //  tpms.PosInfoDetailId = piDet.Id;
            // tpms.ProfileId = g.CustomerId;
            tpms.ProfileName = accountDescription;
            //  tpms.ReceiptNo = newCounter.ToString();
            tpms.RegNo = "0";//order.RegNo;
            tpms.RoomDescription = room;
            //  tpms.RoomId = order.RoomId;
            tpms.SendToPMS = true;
            //  tpms.TransactionId = order.Transactions.FirstOrDefault().Id;
            // tpms.TransferType = 0;//Xrewstiko
            tpms.Total = total;
            tpms.HotelId = hotelId ?? defaultHotelId;
            var identifier = Guid.NewGuid();
            tpms.TransferIdentifier = identifier;
            db.TransferToPms.Add(tpms);
            db.Entry(tpms).State = System.Data.Entity.EntityState.Added;
            return tpms;

        }

        public void SendTransferToPMS(IEnumerable<TransferToPms> objTosendList, string storeid)
        {
            var hi = db.HotelInfo.FirstOrDefault();

            foreach (var to in objTosendList)
            {
                int resid = 0;
                var toint = int.TryParse(to.RegNo, out resid);
                int PmsDepartmentId = 0;
                var topmsint = int.TryParse(to.PmsDepartmentId, out PmsDepartmentId);

                TransferObject trObj = new TransferObject();

                trObj.TransferIdentifier = to.TransferIdentifier;
                trObj.HotelId = (int)hi.HotelId;
                trObj.amount = (decimal)to.Total;
                trObj.departmentId = PmsDepartmentId;
                trObj.description = to.Description;
                trObj.profileName = to.ProfileName;
                trObj.resId = resid;
                trObj.TransferIdentifier = to.TransferIdentifier;
                trObj.HotelUri = hi.HotelUri;
                trObj.RoomName = to.RoomDescription;
                trObj.HotelId = hi.HotelId ?? 1;

                if (to.SendToPMS == true)
                {
                    SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                    //sendserv.BeginInvoke(trObj, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                }
            }
        }


        private void SendTransferCallback(IAsyncResult result)
        {
            try
            {
                // db = new PosEntities(false);
                SendTransfer del = (SendTransfer)result.AsyncState;

                TranferResultModel res = (TranferResultModel)del.EndInvoke(result);

                Guid storeid;

                if (Guid.TryParse(res.StoreId, out storeid))
                {

                    using (var ctx = new PosEntities(false, storeid))
                    {
                        var originalTransfer = ctx.TransferToPms.FirstOrDefault(f => f.TransferIdentifier == res.TransferObj.TransferIdentifier);

                        if (originalTransfer != null)
                        {
                            //originalTransfer.SendToPmsTS = DateTime.Now;
                            originalTransfer.ErrorMessage = res.TransferErrorMessage;
                            originalTransfer.PmsResponseId = res.TransferResponseId;

                        }

                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                logger.Error(ex.ToString());
            }

        }

        public void Dispose()
        {

        }


        private delegate TranferResultModel SendTransfer(TransferObject tpms, string storeid);

    }

}