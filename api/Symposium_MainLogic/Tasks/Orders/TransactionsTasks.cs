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
    public class TransactionsTasks : ITransactionsTasks
    {
        ITransactionsDT dt;
        IInvoice_Guests_TransDT invGuestDT;
        IPosInfoDT posInfoDT;
        IHotelInfoDT hotelInfoDT;
        IOrderDetailInvoicesDT orderDetInvDT;
        ITransferMappingsDT transfMappDT;
        IGuestDT guestDT;

        public TransactionsTasks(ITransactionsDT dt, IInvoice_Guests_TransDT invGuestDT, IPosInfoDT posInfoDT, IHotelInfoDT hotelInfoDT,
            IOrderDetailInvoicesDT orderDetInvDT, ITransferMappingsDT transfMappDT, IGuestDT guestDT)
        {
            this.dt = dt;
            this.invGuestDT = invGuestDT;
            this.posInfoDT = posInfoDT;
            this.hotelInfoDT = hotelInfoDT;
            this.orderDetInvDT = orderDetInvDT;
            this.transfMappDT = transfMappDT;
            this.guestDT = guestDT;
        }


        private short GetTransactionType(InvoiceTypesEnum invoiceType)
        {
            switch (invoiceType)
            {
                case InvoiceTypesEnum.Allowance:
                case InvoiceTypesEnum.Coplimentary:
                case InvoiceTypesEnum.Receipt:
                case InvoiceTypesEnum.Timologio:
                    return 3;
                case InvoiceTypesEnum.Void:
                    return 4;
                case InvoiceTypesEnum.PaymentReceipt:
                    return 7;
                case InvoiceTypesEnum.RefundReceipt:
                    return 7;
            }
            return 3;
        }

        private short GetTransactionInOut(InvoiceTypesEnum invoiceType)
        {
            switch (invoiceType)
            {
                case InvoiceTypesEnum.PaymentReceipt:
                case InvoiceTypesEnum.Allowance:
                case InvoiceTypesEnum.Coplimentary:
                case InvoiceTypesEnum.Receipt:
                case InvoiceTypesEnum.Timologio:
                    return 0;
                case InvoiceTypesEnum.Void:
                    return 1;
                case InvoiceTypesEnum.RefundReceipt:
                    return 1;
            }
            return 0;
        }


        private CreditTransactionsModel CreateCreditTransaction(Receipts r, ReceiptPayments rp)
        {
            var descr = "";
            var type = (byte)CreditTransactionType.RemoveCredit;
            var amount = rp.Amount;
            switch ((InvoiceTypesEnum)r.InvoiceTypeType)
            {
                case InvoiceTypesEnum.PaymentReceipt:
                    if (rp.CreditTransactionAction == (short)CreditTransactionType.AddCredit)
                    {
                        descr = "Add amount to Barcode Credit Account";
                        type = (byte)CreditTransactionType.AddCredit;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.RemoveCredit)
                    {
                        descr = "Remove amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.RemoveCredit;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.ReturnCredit)
                    {
                        descr = "Return amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.ReturnCredit;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.ReturnAllCredits)
                    {
                        descr = "Return amount from Barcode Credit Account and close account";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.PayLocker)
                    {
                        descr = "Remove amount from Barcode Credit Account due to Locker opening";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        amount = rp.Amount * -1;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.ReturnLocker)
                    {
                        descr = "Return amount to Barcode Credit Account due to Locker closing";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        amount = rp.Amount;
                    }
                    break;
                case InvoiceTypesEnum.RefundReceipt:
                    if (rp.CreditTransactionAction == (short)CreditTransactionType.AddCredit)
                    {
                        descr = "Add amount to Barcode Credit Account";
                        type = (byte)CreditTransactionType.AddCredit;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.RemoveCredit)
                    {
                        descr = "Remove amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.RemoveCredit;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.ReturnCredit)
                    {
                        descr = "Return amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.ReturnCredit;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.ReturnAllCredits)
                    {
                        descr = "Return amount from Barcode Credit Account and close account";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        amount = rp.Amount;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.PayLocker)
                    {
                        descr = "Remove amount from Barcode Credit Account due to Locker opening";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        amount = rp.Amount * -1;
                    }
                    else if (rp.CreditTransactionAction == (short)CreditTransactionType.ReturnLocker)
                    {
                        descr = "Return amount to Barcode Credit Account due to Locker closing";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        amount = rp.Amount;
                    }
                    break;
                case InvoiceTypesEnum.Allowance:
                case InvoiceTypesEnum.Coplimentary:
                case InvoiceTypesEnum.Receipt:
                case InvoiceTypesEnum.Timologio:
                    descr = "Payment for Receipt " + r.ReceiptNo.ToString();
                    type = (byte)CreditTransactionType.RemoveCredit;
                    amount = rp.Amount * -1;
                    break;
                case InvoiceTypesEnum.Void:
                    descr = "Cancel Payment for Receipt " + r.ReceiptNo.ToString();
                    type = (byte)CreditTransactionType.AddCredit;
                    amount = rp.Amount * -1;
                    break;
            }

            CreditTransactionsModel ct = new CreditTransactionsModel()
            {
                CreditCodeId = rp.CreditCodeId ?? 0,
                CreditAccountId = rp.CreditAccountId ?? 0,
                CreationTS = DateTime.Now,
                Type = type,
                Description = descr,
                Amount = amount ?? 0,
                StaffId = r.StaffId ?? 0,
                PosInfoId = rp.PosInfoId ?? 0
            };
            return ct;
        }


        /// <summary>
        /// Create's a list of TransferToPms for new Transaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="payment"></param>
        /// <param name="r"></param>
        /// <param name="tr"></param>
        /// <param name="i"></param>
        /// <param name="isFiscal"></param>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        private List<TransferToPmsModel> CreateTransferToPMS(DBInfoModel Store, ReceiptPayments payment, Receipts r, long? InvoiceId, bool isFiscal, long? hotelId)
        {
            //No Payment exists to create TransferToPms
            if (payment == null)
                return null;

            bool SendToPms;
            bool ImmediateTransfer = false;

            int totPays = r.ReceiptPayments.Count();
            decimal Pst = 100;
            if (totPays != 1 && ((r.Total ?? 0) != 0))
            {
                Pst = (100 * payment.Amount ?? 0) / (r.Total ?? 0);
            }

            List<TransferToPmsModel> tpms = new List<TransferToPmsModel>();
            try
            {
                if (r.ModifyOrderDetails == ModifyOrderDetailsEnum.PayOffOnly && r.Total < r.ReceiptDetails.Sum(s => s.ItemGross) && (InvoiceId ?? 0) > 0)
                {
                    List<OrderDetailInvoicesModel> detailsFromDB = orderDetInvDT.GetOrderDetailInvoicesOfSelectedInvoice(Store, InvoiceId ?? 0).OrderBy(o => o.ItemSort).ToList();
                    foreach (OrderDetailInvoicesModel item in detailsFromDB)
                    {
                        r.ReceiptDetails.FirstOrDefault(w => w.ItemSort == item.ItemSort && w.OrderDetailId == item.OrderDetailId).ItemGross = item.Total;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            List<TransferToPmsModel> transfCheck = new List<TransferToPmsModel>();
            GuestModel guest = null;
            PmsRoomModel pmsRoom = null;

            foreach (ReceiptDetails item in r.ReceiptDetails)
            {
                TransferToPmsModel tmpObj = new TransferToPmsModel();
                TransferMappingsModel transfMapp = transfMappDT.GetTransferMappingForNewTransaction(Store, r.DepartmentId ?? 0, item.ProductCategoryId ?? 0,
                    item.PricelistId ?? 0, (int)(hotelId ?? 1));
                if (transfMapp != null)
                {
                    var fld = transfCheck.Find(f => f.PmsDepartmentId == transfMapp.PmsDepartmentId);
                    if (fld != null)
                        fld.Total += Math.Round(((item.ItemGross ?? 0) / Pst) * 100, 2);
                    else
                    {
                        SendToPms = (ModifyOrderDetailsEnum.ChangePaymentType == r.ModifyOrderDetails) ||
                            (!isFiscal) ||
                            (isFiscal && ModifyOrderDetailsEnum.PayOffOnly == r.ModifyOrderDetails && r.IsPrinted);

                        tmpObj.Description = "Rec : " + r.ReceiptNo.ToString() + ", Pos: " + r.PosInfoId.ToString() + ", " + r.DepartmentId.ToString();
                        tmpObj.EndOfDayId = r.EndOfDayId ?? 0;
                        tmpObj.HotelId = hotelId ?? 1;
                        tmpObj.IsDeleted = false;
                        tmpObj.PmsDepartmentDescription = transfMapp.PmsDepDescription;
                        tmpObj.PmsDepartmentId = transfMapp.PmsDepartmentId;
                        tmpObj.Points = r.Points;
                        tmpObj.PosInfoDetailId = item.PosInfoDetailId ?? 0;
                        tmpObj.PosInfoId = r.PosInfoId ?? 0;
                        tmpObj.ReceiptNo = r.ReceiptNo.ToString();
                        tmpObj.Total = item.ItemGross ?? 0;
                        tmpObj.TransferIdentifier = Guid.NewGuid().ToString();
                        tmpObj.TransferType = 0;
                        tmpObj.SendToPmsTS = DateTime.Now;
                        tmpObj.PMSPaymentId = payment.PMSPaymentId;
                        tmpObj.PMSInvoiceId = r.PMSInvoiceId ?? 0;

                        tmpObj.Total += Math.Round(((item.ItemGross ?? 0) / Pst) * 100, 2);


                        switch ((AccountType)payment.AccountType)
                        {
                            case AccountType.Charge:
                                tmpObj.SendToPMS = SendToPms;
                                tmpObj.Status = SendToPms ? 0 : 2;
                                if (payment.GuestId != null && payment.GuestId != 0)
                                {
                                    guest = guestDT.GetGuestById(Store, payment.GuestId ?? 0);
                                    if (guest != null)
                                    {
                                        tmpObj.ProfileId = guest.ProfileNo.ToString();
                                        tmpObj.ProfileName = guest.LastName + " " + guest.FirstName;
                                        tmpObj.RegNo = guest.ReservationId.ToString();
                                        tmpObj.RoomId = guest.RoomId.ToString();
                                        tmpObj.RoomDescription = guest.Room;

                                    }
                                }
                                break;
                            case AccountType.Barcode:
                            case AccountType.Voucher:
                            case AccountType.CreditCard:
                            case AccountType.TicketCompliment:
                            case AccountType.Cash:
                                pmsRoom = dt.GetPmsRoomForCashForTransferToPMS(Store, payment.AccountId ?? 0, r.PosInfoId ?? 0);
                                if (pmsRoom != null)
                                {
                                    SendToPms = pmsRoom.SendsTransfer ?? false;
                                    ImmediateTransfer = (r.ModifyOrderDetails == ModifyOrderDetailsEnum.ChangePaymentType ||
                                                        (isFiscal && ModifyOrderDetailsEnum.PayOffOnly == r.ModifyOrderDetails && r.IsPrinted));

                                    tmpObj.ProfileId = "";
                                    tmpObj.ProfileName = payment.AccountDescription;
                                    tmpObj.RegNo = "";
                                    tmpObj.RoomId = "";
                                    tmpObj.RoomDescription = (pmsRoom.PmsRoom ?? 0).ToString();
                                    tmpObj.SendToPMS = (SendToPms && !isFiscal) || (ImmediateTransfer && SendToPms && isFiscal);
                                    if (!ImmediateTransfer && isFiscal && SendToPms)
                                        tmpObj.Status = 2;
                                }
                                else tmpObj = null;
                                break;
                            case AccountType.Coplimentary:
                            case AccountType.Allowence:
                                guest = guestDT.GetGuestById(Store, payment.GuestId ?? 0);
                                if (guest != null)
                                {
                                    tmpObj.ProfileId = guest.ProfileNo.ToString();
                                    tmpObj.ProfileName = guest.LastName + " " + guest.FirstName;
                                    tmpObj.RegNo = guest.ReservationId.ToString();
                                    tmpObj.RoomId = guest.RoomId.ToString();
                                    tmpObj.RoomDescription = guest.Room;
                                }
                                else
                                {
                                    pmsRoom = dt.GetPmsRoomForCashForTransferToPMS(Store, payment.AccountId ?? 0, r.PosInfoId ?? 0);
                                    SendToPms = pmsRoom.SendsTransfer ?? false;
                                    ImmediateTransfer = r.ModifyOrderDetails == ModifyOrderDetailsEnum.ChangePaymentType;

                                    if (pmsRoom == null)
                                        tmpObj = null;
                                    else
                                    {
                                        tmpObj.ProfileId = "";
                                        tmpObj.ProfileName = payment.AccountDescription;
                                        tmpObj.RegNo = "";
                                        tmpObj.RoomId = "";
                                        tmpObj.RoomDescription = (pmsRoom.PmsRoom ?? 0).ToString();
                                        tmpObj.SendToPMS = (SendToPms && !isFiscal) || (ImmediateTransfer && SendToPms && isFiscal);
                                        if (!ImmediateTransfer && isFiscal && SendToPms)
                                            tmpObj.Status = 2;
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    throw new Exception("Tranfer Mapping not found PosDepartmentId : " + r.DepartmentId.ToString() + " ProductCategoryId : " + item.ProductCategoryId.ToString() +
                                        ", PriceListId : " + item.PricelistId.ToString() + ", HotelId : " + hotelId.ToString());
                }
                if (tmpObj != null)
                    tpms.Add(tmpObj);
            }

            return tpms;
        }


        /// <summary>
        /// Create's List Of Transactions with all connected tables for each list items. Tables are
        /// Invoice Guest Transaction
        /// Credits
        /// Transfer To Pms
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="receipt"></param>
        /// <param name="inv"></param>
        /// <param name="HotelInfoId"></param>
        /// <returns></returns>
        public List<TransactionsExtraModel> ReturnTransactionFromReceipt(DBInfoModel Store, Receipts receipt, InvoiceModel inv, int HotelInfoId = -1)
        {
            List<TransactionsExtraModel> ret = new List<TransactionsExtraModel>();
            GuestModel guest = guestDT.GetGuestById(Store, receipt.CustomerID ?? 0);
            foreach (ReceiptPayments item in receipt.ReceiptPayments)
            {
                TransactionsExtraModel tr = new TransactionsExtraModel();
                tr.AccountId = (int)item.AccountId;
                tr.Amount = item.Amount ?? 0;
                tr.DepartmentId = receipt.DepartmentId;
                tr.Description = "Pay Off";
                tr.ExtDescription = "CreateTransactionsFromReceiptPayments";
                tr.StaffId = receipt.StaffId ?? 0;
                tr.TransactionType = (int)(item.TransactionType != null ? item.TransactionType : GetTransactionType((InvoiceTypesEnum)receipt.InvoiceTypeType));
                tr.PosInfoId = item.PosInfoId ?? 0;
                tr.InOut = GetTransactionInOut((InvoiceTypesEnum)receipt.InvoiceTypeType);
                tr.Day = inv == null ? DateTime.Now : (inv.Day ?? DateTime.Now);
                tr.EndOfDayId = receipt.EndOfDayId;
                //List Of Invoice Guest Transaction For each Transaction
                tr.InvoiceGuest = new List<Invoice_Guests_TransModel>();

                //List Of Credits for each Transaction
                tr.CreditTransaction = new List<CreditTransactionsModel>();

                //List Of Transfer To Pms for each Transaction
                tr.TransferToPms = new List<TransferToPmsModel>();

                if (guest != null)
                {
                    Invoice_Guests_TransModel invGT = new Invoice_Guests_TransModel();
                    invGT.GuestId = guest.Id;
                    invGT.DeliveryCustomerId = guest.ProfileNo;
                    tr.InvoiceGuest.Add(invGT);
                }

                //Collect Invoice Guests
                //{TODO: Check It More for charge. Changed to All types}
                //switch ((AccountType)item.AccountType)
                //{
                //    case AccountType.Cash: break;
                //    case AccountType.CreditCard: break;
                //    case AccountType.Barcode:
                //        //WaterParkLogic
                //        break;
                //    case AccountType.Voucher:
                //        break;
                //    case AccountType.Allowence:
                //    case AccountType.Coplimentary:
                //    case AccountType.Charge:
                //        if (item.GuestId != null)
                //        {
                //            if (receipt.ModifyOrderDetails == (int)ModifyOrderDetailsEnum.PayOffOnly)
                //            {
                //                Invoice_Guests_TransModel igt = invGuestDT.GetInvoiceGuestByInvoiceId(Store, inv == null ? 0 : (inv.Id ?? 0));

                //                if (igt == null)
                //                {
                //                    igt = new Invoice_Guests_TransModel();
                //                    igt.GuestId = item.GuestId;
                //                    igt.InvoiceId = inv == null ? (long?)null : inv.Id;
                //                    tr.InvoiceGuest.Add(igt);
                //                }
                //                else
                //                    tr.InvoiceGuest.Add(igt);
                //            }
                //            else
                //            {
                //                Invoice_Guests_TransModel igt = new Invoice_Guests_TransModel();
                //                igt.GuestId = item.GuestId;
                //                if (receipt.CreateTransactions == true)
                //                    igt.InvoiceId = inv == null ? (long?)null : inv.Id;
                //                tr.InvoiceGuest.Add(igt);
                //            }
                //        }
                //        break;
                //    default:
                //        break;
                //}

                //Collect Creadits
                if ((item.CreditAccountId ?? 0) != 0)
                {
                    if (item.CreditTransactionAction != (short)CreditTransactionType.None)
                        tr.CreditTransaction.Add(CreateCreditTransaction(receipt, item));
                }


                //Collect TransferToPms
                if (HotelInfoId >= 0)
                {

                    PosInfoModel pi = posInfoDT.GetSinglePosInfo(Store, receipt.PosInfoId ?? 0);
                    bool isFiscal = pi != null ? (pi.FiscalType != (int)FiscalTypeEnum.Generic) : false;
                    HotelsInfoModel HotelInfo;
                    if (HotelInfoId == 0)
                        HotelInfo = hotelInfoDT.SelectFirstHotelInfo(Store);
                    else
                        HotelInfo = hotelInfoDT.selectHotelInfoByHotelId(Store, HotelInfoId);

                    if (HotelInfo != null)
                    {
                        if (HotelInfo != null && HotelInfo.Type == 0 || HotelInfo.Type == 10 || HotelInfo.Type == 4)
                        {
                            List<TransferToPmsModel> lstTransToPms = CreateTransferToPMS(Store, item, receipt, inv == null ? (long?)null : inv.Id, isFiscal, HotelInfo.HotelId);
                            foreach (TransferToPmsModel trf in lstTransToPms)
                                tr.TransferToPms.Add(trf);
                        }
                    }
                }
                
                ret.Add(tr);
            }

            return ret;
        }


        /// <summary>
        /// Return's a list of transaction based on invoiceId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public List<TransactionsModel> GetTransactionsByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            return dt.GetTransactionsByInvoiceId(Store, InvoiceId);
        }

        /// <summary>
        /// add's new transacion to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewTransaction(DBInfoModel Store, TransactionsModel item)
        {
            return dt.AddNewTransaction(Store, item);
        }

        /// <summary>
        /// Return a list of Transaction Extra Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public List<TransactionsExtraModel> GetTransactionsByInvoiceId(DBInfoModel Store, List<long> InvoiceId)
        {
            List<TransactionsExtraModel> Transactions = new List<TransactionsExtraModel>();
            foreach (long item in InvoiceId)
                Transactions.Add(AutoMapper.Mapper.Map<TransactionsExtraModel>(dt.GetTransactionsByInvoiceId(Store, item)));

            return Transactions;
        }

        /// <summary>
        /// Return's PMS Room For TransferToPms for Cash, CC, etc....
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="AccountId"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        public PmsRoomModel GetPmsRoomForCashForTransferToPMS(DBInfoModel Store, long AccountId, long PosInfoId)
        {
            return dt.GetPmsRoomForCashForTransferToPMS(Store, AccountId, PosInfoId);
        }
    }
}
