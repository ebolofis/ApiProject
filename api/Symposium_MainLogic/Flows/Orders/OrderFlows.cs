using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using Symposium.Models.Models.Orders;
using Symposium.Models.Models.Plugins;
using Symposium.Plugins;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class OrderFlows : IOrderFlows
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IDA_ClientJobsFlows daFlows;
        IPosInfoDetailFlows posInfoDetFlow;
        IPosInfoDetailTasks posInfoDetTask;
        IGuestTasks guestTasks;
        IPosInfoFlows posInfoFlow;
        IOrderTask orderTask;
        IInvoicesFlows invFlow;
        IInvoice_Guest_TransFlows invGuestTransFlow;
        ICreditTransactionsFlows credTransactFlow;
        ITransferToPmsFlows transfToPmsFlow;
        IOrderDetailInvoicesFlows ordDetInvFlow;
        ITransactionsFlows transactFlows;
        IInvoiceTypeTasks invTypeTask;
        IAccountTasks accTask;
        IOrderDetailTasks ordDetTask;
        IOrderDetailIngredientsTasks ordDetIngTask;
        IOrderStatusFlows orderStatusFlows;

        /*To Remove It*/
        ITransferMappingsDT transfMappDT;


        public OrderFlows(IDA_ClientJobsFlows daFlows, IPosInfoDetailFlows posInfoDetFlow, IPosInfoDetailTasks posInfoDetTask,
            IGuestTasks guestTasks, IPosInfoFlows posInfoFlow, IOrderTask orderTask,
            IInvoicesFlows invFlow, IInvoice_Guest_TransFlows invGuestTransFlow,
            ICreditTransactionsFlows credTransactFlow, ITransferToPmsFlows transfToPmsFlow,
            ITransactionsFlows transactFlows, IOrderDetailInvoicesFlows ordDetInvFlow,
            IInvoiceTypeTasks invTypeTask, IAccountTasks accTask, ITransferMappingsDT transfMappDT,
            IOrderDetailTasks ordDetTask, IOrderDetailIngredientsTasks ordDetIngTask, IOrderStatusFlows orderStatusFlows)
        {
            this.daFlows = daFlows;
            this.posInfoDetFlow = posInfoDetFlow;
            this.posInfoDetTask = posInfoDetTask;
            this.guestTasks = guestTasks;
            this.posInfoFlow = posInfoFlow;
            this.orderTask = orderTask;
            this.invFlow = invFlow;
            this.invGuestTransFlow = invGuestTransFlow;
            this.credTransactFlow = credTransactFlow;
            this.transfToPmsFlow = transfToPmsFlow;
            this.ordDetInvFlow = ordDetInvFlow;
            this.transactFlows = transactFlows;
            this.invTypeTask = invTypeTask;
            this.accTask = accTask;
            this.transfMappDT = transfMappDT;
            this.ordDetTask = ordDetTask;
            this.ordDetIngTask = ordDetIngTask;
            this.orderStatusFlows = orderStatusFlows;
        }

        /// <summary>
        /// Return's Order Status for DA_Order
        /// </summary>
        /// <param name="IsDelay"></param>
        /// <param name="IsNew"></param>
        /// <param name="EstTakeoutDate"></param>
        /// <param name="OrderStatus"></param>
        /// <param name="OldStatus"></param>
        /// <returns></returns>
        private OrderStatusEnum ReturnOrderStatus(bool IsDelay, bool IsNew, DateTime EstTakeoutDate,
            OrderStatusEnum OrderStatus, OrderStatusEnum OldStatus, OrderTypeStatus orderType, short accountType, bool IsPaid, out bool AddNew, DBInfoModel dbInfo, long OrderId)
        {
            AddNew = true;
            if (IsNew && IsDelay)
            {
                if (orderType == OrderTypeStatus.DineIn && (accountType == (short)AccountType.CreditCard || accountType == (short)AccountType.TicketCompliment) && IsPaid)
                    return OrderStatusEnum.Complete;
                else
                    return OrderStatusEnum.Received;
            }
            else if (IsNew && !IsDelay)
            {
                if (orderType == OrderTypeStatus.DineIn && (accountType == (short)AccountType.CreditCard || accountType == (short)AccountType.TicketCompliment) && IsPaid)
                    return OrderStatusEnum.Complete;
                else 
                    return OrderStatusEnum.Preparing;
            }
            else
            {
                if (!IsDelay)
                {
                    if (OldStatus > OrderStatus)
                    {
                        AddNew = false;
                        return OldStatus;
                    }
                    else
                        return OrderStatusEnum.Preparing;
                }
                else
                {
                    OrderStatusModel orderStatusModel = orderStatusFlows.GetLastOrderStatusForOrderId(dbInfo, OrderId);

                    if (orderStatusModel == null || orderStatusModel.Status != OrderStatusEnum.Received)
                        return OrderStatusEnum.Received;
                    else
                    {
                        AddNew = false;
                        return OrderStatusEnum.Received;
                    }
                    //TimeSpan minDiff = EstTakeoutDate.Subtract(DateTime.Now);// DateTime.Now.Subtract(EstTakeoutDate);
                    //if (minDiff.TotalMinutes < 10)
                    //{
                    //    if (OldStatus > OrderStatus)
                    //    {
                    //        AddNew = false;
                    //        return OldStatus;
                    //    }
                    //    else
                    //        return OrderStatus;
                    //}
                    //else
                    //{
                    //    AddNew = true;
                    //    return OrderStatusEnum.Received;
                    //}
                }
            }
        }

        /// <summary>
        /// Convert's a Da_OrderModel to new OrderModel for Insert
        /// </summary>
        /// <param name="daOrder"></param>
        /// <param name="StaffId"></param>
        /// <param name="PdaModule"></param>
        /// <param name="ClientPos"></param>
        /// <param name="ExtKey"></param>
        /// <param name="ExtObj"></param>
        /// <param name="OrderOrigin"></param>
        /// <returns></returns>
        public OrderModel ConvertDA_OrderToOrder(DA_OrderModel daOrder, DA_StoreModel StoreModel, DACustomerModel Customer, long StaffId,
            int? PdaModule, long? ClientPos, string ExtKey, string ExtObj, OrderOriginEnum OrderOrigin, ExternalSystemOrderEnum? ExtType)
        {
            OrderModel order = new OrderModel();
            order.Day = DateTime.Now;
            order.Total = daOrder.Total;
            order.PosId = daOrder.PosId;
            order.StaffId = StaffId;
            order.Discount = daOrder.Discount;
            order.TotalBeforeDiscount = daOrder.Total + daOrder.Discount;
            order.PdaModuleId = PdaModule;
            order.ClientPosId = ClientPos;
            order.IsDelay = false;
            order.ExtType = ExtType == null || ExtType == ExternalSystemOrderEnum.Default ? (int?)null : daOrder.ExtType;
            order.ExtObj = ExtType == null || ExtType == ExternalSystemOrderEnum.Default ? (string)null : ExtObj;
            order.ExtKey = ExtType == null || ExtType == ExternalSystemOrderEnum.Default ? (string)null : ExtKey;
            order.OrderOrigin = (int)OrderOrigin;
            order.Couver = daOrder.Cover ?? 0;
            order.isDAModified = false;
            order.DA_IsPaid = daOrder.IsPaid;
            order.EstTakeoutDate = daOrder.EstTakeoutDate;
            order.IsDelay = daOrder.IsDelay;
            order.OrderNotes = daOrder.Remarks;
            order.StoreNotes = StoreModel != null ? StoreModel.Notes : "";
            order.CustomerNotes = Customer != null ? Customer.Notes : "";
            order.CustomerSecretNotes = Customer != null ? Customer.SecretNotes : "";
            order.CustomerLastOrderNotes = Customer != null ? Customer.LastOrderNote : "";
            order.LogicErrors = daOrder.LogicErrors;
            order.ItemsChanged = daOrder.ItemsChanged;
            order.DA_Origin = daOrder.Origin;
            return order;
        }


        /// <summary>
        /// Return's a new Invoice with all associated tables model
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="invoiceType"></param>
        /// <param name="GuestId"></param>
        /// <param name="PosId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="StaffId"></param>
        /// <param name="PaymentDescr"></param>
        /// <param name="OrderDetailInvoices"></param>
        /// <returns></returns>
        public InvoiceWithTablesModel ReturnNewInvoice(DA_OrderModel Order, 
            InvoiceTypeModel invoiceType, long? GuestId, long PosId, long PosInfoDetailId,
            long StaffId, string PaymentDescr, List<OrderDetailInvoicesModel> OrderDetailInvoices, long? ClientPosId)
        {
            InvoiceWithTablesModel tmpInv = new InvoiceWithTablesModel();
            tmpInv.BuzzerNumber = "";
            tmpInv.CashAmount = "0";
            tmpInv.ClientPosId = ClientPosId; //Order.ClientStoreId;
            tmpInv.Cover = Order.Cover;
            tmpInv.Day = DateTime.Now;
            tmpInv.Description = invoiceType.Description;
            tmpInv.Discount = Order.Discount;
            tmpInv.DiscountRemark = Order.DiscountRemark;
            tmpInv.GuestId = GuestId;

            StringBuilder hashsting = new StringBuilder(tmpInv.Day.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            hashsting.Append(PosId);
            hashsting.Append(PosInfoDetailId);
            hashsting.Append(2);
            hashsting.Append(Order.Total.ToString("000000000.00", CultureInfo.GetCultureInfo("en-US")));
            hashsting.Append(StaffId);
            hashsting.Append(OrderDetailInvoices.Count);
            if (OrderDetailInvoices != null)
            {
                foreach (var item in OrderDetailInvoices)
                {
                    hashsting.Append(item.Description);
                    hashsting.Append(item.Qty);
                }
            }
            tmpInv.Hashcode = MD5Helper.GetMD5Hash(hashsting.ToString()) + tmpInv.Day.Value.ToString("-yyyyMMdd");

            tmpInv.InvoiceTypeId = invoiceType.Id;
            tmpInv.IsDeleted = false;
            tmpInv.IsInvoiced = false;
            tmpInv.IsPaid = (int)IsPaidEnum.None;
            tmpInv.IsPrinted = false;
            tmpInv.IsVoided = false;
            tmpInv.LoyaltyDiscount = 0;
            tmpInv.Net = Order.NetAmount;
            tmpInv.PaidTotal = 0;// Order.Total;
            tmpInv.PaymentsDesc = PaymentDescr;
            tmpInv.PdaModuleId = null;
            tmpInv.PosInfoDetailId = PosInfoDetailId;
            tmpInv.PosInfoId = PosId;
            tmpInv.Rooms = "";
            tmpInv.StaffId = StaffId;
            tmpInv.TableId = Order.TableId;
            tmpInv.TableSum = null;
            tmpInv.Tax = Order.TotalTax;
            tmpInv.Total = Order.Total;
            tmpInv.Vat = Order.TotalVat;

            return tmpInv;
        }

        /// <summary>
        /// Return's an Invoice Shipping Detail Model
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="ship"></param>
        /// <param name="phone"></param>
        /// <param name="Customer"></param>
        /// <param name="StoreRemarks"></param>
        /// <param name="GuestId"></param>
        /// <returns></returns>
        public InvoiceShippingDetailsModel ReturnInvoiceShippingDetail(DeliveryCustomersBillingAddressModel bill,
            DeliveryCustomersShippingAddressModel ship, DeliveryCustomersPhonesModel phone,
            DACustomerModel Customer, string StoreRemarks, long? GuestId)
        {
            InvoiceShippingDetailsModel invShp = new InvoiceShippingDetailsModel()
            {
                ShippingAddressId = ship == null ? (long?)null : ship.ID,
                ShippingAddress = ship == null ? (string)null : ship.AddressStreet + " " + ship.AddressNo,
                ShippingCity = ship == null ? (string)null : ship.City,
                ShippingZipCode = ship == null ? (string)null : ship.Zipcode,
                BillingAddressId = bill == null ? (long?)null : bill.ID,
                BillingAddress = bill == null ? (string)null : bill.AddressStreet + " " + bill.AddressNo,
                BillingCity = bill == null ? (string)null : bill.City,
                BillingZipCode = bill == null ? (string)null : bill.Zipcode,
                Floor = ship == null ? (string)null : ship.Floor,
                CustomerRemarks = Customer.Notes,
                StoreRemarks = StoreRemarks,
                CustomerName = Customer.LastName + " " + Customer.FirstName,
                Longtitude = ship == null ? (double?)null : double.Parse(string.IsNullOrEmpty(ship.Longtitude) ? "0" : ship.Longtitude),
                Latitude = ship == null ? (double?)null : double.Parse(string.IsNullOrEmpty(ship.Latitude) ? "0" : ship.Latitude),
                Phone = phone.PhoneNumber,
                BillingName = bill == null ? (string)null : Customer.JobName,
                BillingVatNo = bill == null ? (string)null : Customer.VatNo,
                BillingDOY = bill == null ? (string)null : Customer.Doy,
                BillingJob = bill == null ? (string)null : Customer.Proffesion,
                CustomerID = GuestId
            };
            return invShp;
        }

        /// <summary>
        /// Return's a new transaction
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="StaffId"></param>
        /// <param name="TransactType"></param>
        /// <param name="Total"></param>
        /// <param name="AccountId"></param>
        /// <param name="InOut"></param>
        /// <param name="Description"></param>
        /// <param name="ExtDescr"></param>
        /// <returns></returns>
        public TransactionsExtraModel ReturnTransaction(PosInfoModel PosInfo, long StaffId, TransactionTypesEnum TransactType,
            decimal Total, int AccountId, int InOut, string Description, string ExtDescr)
        {
            TransactionsExtraModel tmp = new TransactionsExtraModel()
            {
                Day = DateTime.Now,
                PosInfoId = PosInfo.Id,
                StaffId = StaffId,
                TransactionType = (int)TransactType,
                Amount = Total,
                DepartmentId = PosInfo.DepartmentId,
                Description = Description,
                AccountId = AccountId,
                InOut = InOut,
                Gross = Total,
                ExtDescription = ExtDescr,
                IsDeleted = false
            };
            return tmp;
        }

        /// <summary>
        /// Create's a new Credit Transactions 
        /// </summary>
        /// <param name="invType"></param>
        /// <param name="credTransactType"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="StaffId"></param>
        /// <param name="Amount"></param>
        /// <param name="CreditCodeId"></param>
        /// <param name="CreditAccountId"></param>
        /// <returns></returns>
        public CreditTransactionsModel CreateCreditTransaction(InvoiceTypesEnum invType,
           CreditTransactionType credTransactType, long PosInfoId, long StaffId, decimal Amount,
           int CreditCodeId, int CreditAccountId, long ReceiptNo)
        {
            var descr = "";
            var type = (byte)CreditTransactionType.RemoveCredit;
            switch (invType)
            {
                case InvoiceTypesEnum.PaymentReceipt:
                    if (credTransactType == CreditTransactionType.AddCredit)
                    {
                        descr = "Add amount to Barcode Credit Account";
                        type = (byte)CreditTransactionType.AddCredit;
                    }
                    else if (credTransactType == CreditTransactionType.RemoveCredit)
                    {
                        descr = "Remove amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.RemoveCredit;
                    }
                    else if (credTransactType == CreditTransactionType.ReturnCredit)
                    {
                        descr = "Return amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.ReturnCredit;
                    }
                    else if (credTransactType == CreditTransactionType.ReturnAllCredits)
                    {
                        descr = "Return amount from Barcode Credit Account and close account";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                    }
                    else if (credTransactType == CreditTransactionType.PayLocker)
                    {
                        descr = "Remove amount from Barcode Credit Account due to Locker opening";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        Amount = Amount * -1;
                    }
                    else if (credTransactType == CreditTransactionType.ReturnLocker)
                    {
                        descr = "Return amount to Barcode Credit Account due to Locker closing";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                    }
                    break;
                case InvoiceTypesEnum.RefundReceipt:
                    if (credTransactType == CreditTransactionType.AddCredit)
                    {
                        descr = "Add amount to Barcode Credit Account";
                        type = (byte)CreditTransactionType.AddCredit;
                    }
                    else if (credTransactType == CreditTransactionType.RemoveCredit)
                    {
                        descr = "Remove amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.RemoveCredit;
                    }
                    else if (credTransactType == CreditTransactionType.ReturnCredit)
                    {
                        descr = "Return amount from Barcode Credit Account";
                        type = (byte)CreditTransactionType.ReturnCredit;
                    }
                    else if (credTransactType == CreditTransactionType.ReturnAllCredits)
                    {
                        descr = "Return amount from Barcode Credit Account and close account";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                    }
                    else if (credTransactType == CreditTransactionType.PayLocker)
                    {
                        descr = "Remove amount from Barcode Credit Account due to Locker opening";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        Amount = Amount * -1;
                    }
                    else if (credTransactType == CreditTransactionType.ReturnLocker)
                    {
                        descr = "Return amount to Barcode Credit Account due to Locker closing";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                    }
                    break;
                case InvoiceTypesEnum.Allowance:
                case InvoiceTypesEnum.Coplimentary:
                case InvoiceTypesEnum.Receipt:
                case InvoiceTypesEnum.Timologio:
                    descr = "Payment for Receipt " + ReceiptNo.ToString();
                    type = (byte)CreditTransactionType.RemoveCredit;
                    Amount = Amount * -1;
                    break;
                case InvoiceTypesEnum.Void:
                    descr = "Cancel Payment for Receipt " + ReceiptNo.ToString();
                    type = (byte)CreditTransactionType.AddCredit;
                    Amount = Amount * -1;
                    break;
            }

            CreditTransactionsModel ct = new CreditTransactionsModel()
            {
                CreditCodeId = CreditCodeId,
                CreditAccountId = CreditAccountId,
                CreationTS = DateTime.Now,
                Type = type,
                Description = descr,
                Amount = Amount,
                StaffId = StaffId,
                PosInfoId = PosInfoId
            };
            return ct;
        }

        /// <summary>
        /// Return's a transfer mapping model based on product category, pos department and pricelist id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosDepartmentId"></param>
        /// <param name="ProductCategoryId"></param>
        /// <param name="PriceListId"></param>
        /// <param name="HotelId"></param>
        /// <returns></returns>
        public TransferMappingsModel GetTransferMappingForNewTransaction(DBInfoModel Store, long PosDepartmentId, long ProductCategoryId,
            long PriceListId, int HotelId)
        {
            return transfMappDT.GetTransferMappingForNewTransaction(Store, PosDepartmentId, ProductCategoryId, PriceListId, HotelId);
        }

        /// <summary>
        /// Return's a List of TransferToPms Record
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="PayTotal"></param>
        /// <param name="TotPayRecord"></param>
        /// <param name="HotelId"></param>
        /// <param name="DepartmentId"></param>
        /// <param name="Account"></param>
        /// <param name="isFiscal"></param>
        /// <param name="Inv"></param>
        /// <param name="OrderDetailInvoice"></param>
        /// <param name="ModifyOrder"></param>
        /// <param name="AccType"></param>
        /// <param name="PMSPaymentId"></param>
        /// <param name="PMSInvoiceId"></param>
        /// <param name="Guest"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public List<TransferToPmsModel> ReturnTransferToPms(DBInfoModel dbInfo, decimal PayTotal,
            int TotPayRecord, int? HotelId, int? DepartmentId, AccountModel Account, bool isFiscal, InvoiceModel Inv,
            List<OrderDetailInvoicesModel> OrderDetailInvoice, ModifyOrderDetailsEnum ModifyOrder, AccountType AccType,
            long? PMSPaymentId, long? PMSInvoiceId, GuestModel Guest, int Points, out string Error)
        {
            List<OrderDetailInvoicesModel> detailsFromDB = new List<OrderDetailInvoicesModel>();

            Error = "";

            bool SendToPms;
            bool ImmediateTransfer = false;

            //Find Percentance for each record to create.
            decimal Pst = 100;
            if (TotPayRecord != 1 && ((Inv.Total ?? 0) != 0))
            {
                Pst = (100 * PayTotal) / (Inv.Total ?? 0);
            }

            List<TransferToPmsModel> tpms = new List<TransferToPmsModel>();
            try
            {
                if (ModifyOrder == ModifyOrderDetailsEnum.PayOffOnly && Inv.Total < OrderDetailInvoice.Sum(s => s.Total) && (Inv.Id ?? 0) > 0)
                {
                    detailsFromDB = ordDetInvFlow.GetOrderDetailInvoicesOfSelectedInvoice(dbInfo, Inv.Id ?? 0).OrderBy(o => o.ItemSort).ToList();
                    foreach (OrderDetailInvoicesModel item in detailsFromDB)
                    {
                        /*What I Have To DO*/
                        //r.ReceiptDetails.FirstOrDefault(w => w.ItemSort == item.ItemSort && w.OrderDetailId == item.OrderDetailId).ItemGross = item.Total;
                    }
                }
                else
                    detailsFromDB = OrderDetailInvoice;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return null;
            }

            List<TransferToPmsModel> transfCheck = new List<TransferToPmsModel>();
            PmsRoomModel pmsRoom = null;

            foreach (OrderDetailInvoicesModel item in detailsFromDB)
            {
                TransferToPmsModel tmpObj = new TransferToPmsModel();
                TransferMappingsModel transfMapp = transfMappDT.GetTransferMappingForNewTransaction(dbInfo, DepartmentId ?? 0, item.ProductCategoryId ?? 0,
                    item.PricelistId ?? 0, (int)(HotelId ?? 1));

                if (transfMapp != null)
                {
                    var fld = transfCheck.Find(f => f.PmsDepartmentId == transfMapp.PmsDepartmentId);
                    if (fld != null)
                        fld.Total += Math.Round(((item.Total ?? 0) / Pst) * 100, 2);
                    else
                    {
                        SendToPms = (ModifyOrderDetailsEnum.ChangePaymentType == ModifyOrder) ||
                            (!isFiscal) ||
                            (isFiscal && ModifyOrderDetailsEnum.PayOffOnly == ModifyOrder && (Inv.IsPrinted ?? false));

                        tmpObj.Description = "Rec : " + Inv.Counter.ToString() + ", Pos: " + Inv.PosInfoId.ToString() + ", " + DepartmentId.ToString();
                        tmpObj.EndOfDayId = Inv.EndOfDayId ?? 0;
                        tmpObj.HotelId = HotelId ?? 1;
                        tmpObj.IsDeleted = false;
                        tmpObj.PmsDepartmentDescription = transfMapp.PmsDepDescription;
                        tmpObj.PmsDepartmentId = transfMapp.PmsDepartmentId;
                        tmpObj.Points = Points;
                        tmpObj.PosInfoDetailId = item.PosInfoDetailId ?? 0;
                        tmpObj.PosInfoId = Inv.PosInfoId ?? 0;
                        tmpObj.ReceiptNo = Inv.Counter.ToString();
                        tmpObj.Total = item.Total ?? 0;
                        tmpObj.TransferIdentifier = Guid.NewGuid().ToString();
                        tmpObj.TransferType = 0;
                        tmpObj.SendToPmsTS = DateTime.Now;
                        tmpObj.PMSPaymentId = PMSPaymentId;
                        tmpObj.PMSInvoiceId = PMSInvoiceId;

                        tmpObj.Total += Math.Round(((item.Total ?? 0) / Pst) * 100, 2);


                        switch (AccType)
                        {
                            case AccountType.Charge:
                                tmpObj.SendToPMS = SendToPms;
                                tmpObj.Status = SendToPms ? 0 : 2;
                                if (Guest != null)
                                {
                                    tmpObj.ProfileId = Guest.ProfileNo.ToString();
                                    tmpObj.ProfileName = Guest.LastName + " " + Guest.FirstName;
                                    tmpObj.RegNo = Guest.ReservationId.ToString();
                                    tmpObj.RoomId = Guest.RoomId.ToString();
                                    tmpObj.RoomDescription = Guest.Room;
                                }
                                break;
                            case AccountType.Barcode:
                            case AccountType.Voucher:
                            case AccountType.CreditCard:
                            case AccountType.TicketCompliment:
                            case AccountType.Cash:
                                pmsRoom = transactFlows.GetPmsRoomForCashForTransferToPMS(dbInfo, (int)Account.Id, Inv.PosInfoId ?? 0);
                                if (pmsRoom != null)
                                {
                                    SendToPms = pmsRoom.SendsTransfer ?? false;
                                    ImmediateTransfer = (ModifyOrder == ModifyOrderDetailsEnum.ChangePaymentType ||
                                                        (isFiscal && ModifyOrderDetailsEnum.PayOffOnly == ModifyOrder && (Inv.IsPrinted ?? false)));

                                    tmpObj.ProfileId = "";
                                    tmpObj.ProfileName = Account.Description;
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
                                if (Guest != null)
                                {
                                    tmpObj.ProfileId = Guest.ProfileNo.ToString();
                                    tmpObj.ProfileName = Guest.LastName + " " + Guest.FirstName;
                                    tmpObj.RegNo = Guest.ReservationId.ToString();
                                    tmpObj.RoomId = Guest.RoomId.ToString();
                                    tmpObj.RoomDescription = Guest.Room;
                                }
                                else
                                {
                                    pmsRoom = transactFlows.GetPmsRoomForCashForTransferToPMS(dbInfo, Account.Id, Inv.PosInfoId ?? 0);
                                    SendToPms = pmsRoom.SendsTransfer ?? false;
                                    ImmediateTransfer = ModifyOrder == ModifyOrderDetailsEnum.ChangePaymentType;

                                    if (pmsRoom == null)
                                        tmpObj = null;
                                    else
                                    {
                                        tmpObj.ProfileId = "";
                                        tmpObj.ProfileName = Account.Description;
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
                    Error = "Tranfer Mapping not found PosDepartmentId : " + DepartmentId.ToString() + " ProductCategoryId : " + item.ProductCategoryId.ToString() +
                                        ", PriceListId : " + item.PricelistId.ToString() + ", HotelId : " + HotelId.ToString();
                    return null;
                }
                if (tmpObj != null)
                    tpms.Add(tmpObj);
            }
            return tpms;
        }


        /// <summary>
        /// Convert's a OrderFromDAToClientForWebCallModel model to FullOrderWithTablesModel model for post new order
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <param name="guestModel"></param>
        /// <param name="deliveryCustomer"></param>
        /// <param name="invType"></param>
        /// <param name="invTypeForDP"></param>
        /// <param name="accModel"></param>
        /// <param name="posInfo"></param>
        /// <param name="posInfoDetForDP"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        private FullOrderWithTablesModel ConvertDA_OrderToStoreOrder(DBInfoModel dbInfo, OrderFromDAToClientForWebCallModel model,
            GuestModel guestModel, DeliveryCustomerModel deliveryCustomer, InvoiceTypeModel invType, InvoiceTypeModel invTypeForDP,
            AccountModel accModel, PosInfoModel posInfo, PosInfoDetailModel posInfoDetForDP, 
            out string Error)
        {
            FullOrderWithTablesModel StoreOrder = new FullOrderWithTablesModel();
            Error = "";


            string Company = !string.IsNullOrEmpty(deliveryCustomer.BillingName) ? deliveryCustomer.BillingName : deliveryCustomer.LastName + " " + deliveryCustomer.FirstName;
            string Bell;
            var fld = model.Addresses.Find(f => !string.IsNullOrEmpty(f.Bell));
            if (fld != null)
                Bell = fld.Bell;
            else
                Bell = "";

            ExternalDA_ObjectModel nextobj = new ExternalDA_ObjectModel
            {
                OrderNo = model.Order.Id.ToString(),
                Status = model.Order.Status,
                InvoiceCode = invType.Code,
                InvoiceType = model.Order.InvoiceType,
                AccountType = model.Order.AccountType,
                bell = Bell,
                with_couvert = null,
                payment_method = accModel.Description,
                company_name = Company,
                rendezvous = model.Order.IsDelay,// (order.timeslot != null),
                start_time = model.Order.OrderDate.TimeOfDay,
                end_time = model.Order.IsDelay ? (model.Order.EstTakeoutDate ?? DateTime.Now).TimeOfDay : (TimeSpan?)null,
                isPrinted = false,
                EstBillingDate = model.Order.EstBillingDate,
                EstTakeoutDate = model.Order.EstTakeoutDate,
                AgentNo = model.Order.AgentNo,
                PointsGain = model.Order.PointsGain,
                PointsRedeem = model.Order.PointsRedeem,
                DA_ExternalModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ExternalObjectModel>(string.IsNullOrEmpty(model.Order.ExtObj) ? "" : model.Order.ExtObj)
            };
            string ExtObj = Newtonsoft.Json.JsonConvert.SerializeObject(nextobj);

            //1. Convert DA_OrderModel To Order Model(FullOrderWithTablesModel inherited from OrderModel)
            StoreOrder = AutoMapper.Mapper.Map<FullOrderWithTablesModel>(
                                        ConvertDA_OrderToOrder(model.Order, model.StoreModel, model.Customer, (long)(model.StoreModel.PosStaffId ?? 0), null, null,
                                        model.Order.Id.ToString(), ExtObj, OrderOriginEnum.Delivery, ExternalSystemOrderEnum.DeliveryAgent));

            StoreOrder.OrderDetails = ordDetInvFlow.ConvertDA_OrderDetailsToOrderDetails(dbInfo,
                model.Order, model.OrderDetails, model.OrderDetailExtras, ModifyOrderDetailsEnum.FromScratch,
                guestModel.Id, true, (int) model.Order.OrderType, model.Order.TableId, deliveryCustomer, model.StoreModel.PosStaffId, out Error);
            if (!string.IsNullOrEmpty(Error))
                return null;
            foreach (OrderDetailWithExtrasModel ordDet in StoreOrder.OrderDetails)
                ordDet.OrderDetailInvoices.ForEach(f => f.IsPrinted = model.Order.IsDelay ? false : ((FiscalTypeEnum)posInfoDetForDP.FiscalType == FiscalTypeEnum.Generic));

            if (StoreOrder.OrderDetails == null)
                return null;

            DeliveryCustomersBillingAddressModel bill = deliveryCustomer.BillingAddresses.Find(f => f.IsSelected == true);
            DeliveryCustomersShippingAddressModel ship = deliveryCustomer.ShippingAddresses.Find(f => f.IsSelected == true);
            DeliveryCustomersPhonesModel phone = deliveryCustomer.Phones.Find(f => f.IsSelected == true);



            //List Of All Order Detail Invoices....
            List<OrderDetailInvoicesModel> tmpDetInvoice = new List<OrderDetailInvoicesModel>();
            foreach (OrderDetailWithExtrasModel item in StoreOrder.OrderDetails)
            {
                foreach (OrderDetailInvoicesModel tmp in item.OrderDetailInvoices)
                    tmpDetInvoice.Add(tmp);
            }

            /*New Invoice for Δ.Π.*/
            InvoiceWithTablesModel tmpInv = ReturnNewInvoice(model.Order, 
                invTypeForDP, guestModel == null ? (long?)null : guestModel.Id, model.StoreModel.PosId ?? 0,
                posInfoDetForDP.Id, model.StoreModel.PosStaffId ?? 0, accModel.Description,
                tmpDetInvoice, null);
            tmpInv.IsPrinted = model.Order.IsDelay ? false : ((FiscalTypeEnum)posInfoDetForDP.FiscalType == FiscalTypeEnum.Generic);
            /*Invoice Shipping Detail*/
            tmpInv.InvoiceShippings = new List<InvoiceShippingDetailsModel>();
            tmpInv.InvoiceShippings.Add(ReturnInvoiceShippingDetail(bill, ship, phone, model.Customer,
                model.Order.Remarks, deliveryCustomer.ID));

            /*Transaction With associated tables*/
            tmpInv.Transactions = new List<TransactionsExtraModel>();
            /*Το αρχικό παραστατικό είναι το invType. Αν θέλουμε να εκδόσουμε Δελτίο Παραγγελίας τοτε invTypeForDP ΔΕΝ ΕΙΝΑΙ ΚΕΝΟ αλλά έχει τα στοιχεία του Δελτίου Παραγγελίας*/
            if (invType.Type != 2 && invTypeForDP == null)
            {
                int InOut = GetTransactionInOut((InvoiceTypesEnum)invType.Type);

                TransactionsExtraModel transact = ReturnTransaction(posInfo, model.StoreModel.PosStaffId ?? 0, TransactionTypesEnum.Sale,
                    model.Order.Total, (int)accModel.Id, InOut, "Pay Off", "CreateTransactionsFromReceiptPayments");

                /*Invoice Guest Transaction*/
                Invoice_Guests_TransModel invGuest = new Invoice_Guests_TransModel()
                {
                    GuestId = guestModel == null ? (long?)null : guestModel.Id,
                    DeliveryCustomerId = deliveryCustomer.ID
                };
                transact.InvoiceGuest = new List<Invoice_Guests_TransModel>();
                transact.InvoiceGuest.Add(invGuest);

                //Credit Transaction Not Needed
                transact.CreditTransaction = new List<CreditTransactionsModel>();
                //CreditTransactionsModel crtModel = CreateCreditTransaction()

                /*Transfer To Pms*/
                bool isFiscal = posInfo != null ? (posInfo.FiscalType != (int)FiscalTypeEnum.Generic) : false;

                transact.TransferToPms = ReturnTransferToPms(dbInfo, tmpInv.Total ?? 0, 1, 1, (int)(posInfo.DepartmentId ?? 0),
                    accModel, isFiscal, tmpInv, tmpDetInvoice, ModifyOrderDetailsEnum.FromScratch, AccountType.Charge, null, null,
                    guestModel, 0, out Error);

                tmpInv.Transactions.Add(transact);
            }

            //Implementation for Goodys. If TableId on invoice < 1 then null. Error on Foreign Keys
            if ((tmpInv.TableId ?? 0) < 1)
                tmpInv.TableId = null;

            StoreOrder.Invoice = new List<InvoiceWithTablesModel>();
            StoreOrder.Invoice.Add(tmpInv);

            return StoreOrder;
        }

        /// <summary>
        /// Insert's new order from Delivery Agent to Store. Return's New Order Id
        /// </summary>
        /// <param name="model"></param>
        public List<ResultsAfterDA_OrderActionsModel> InsertDeliveryOrders(DBInfoModel dbInfo, OrderFromDAToClientForWebCallModel model,
            string StatusCanCanceld, bool CreateDP, out SignalRAfterInvoiceModel signal)
        {
             signal = null;

            List<ResultsAfterDA_OrderActionsModel> resutls = new List<ResultsAfterDA_OrderActionsModel>();
            if (model.Order == null)
            {
                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                tmpRes.DA_Order_Id = -1;
                tmpRes.Store_Order_Id = -1;
                tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                tmpRes.Store_Order_Status_DT = DateTime.Now;
                tmpRes.Succeded = false;
                tmpRes.Errors = "No Order Exists...";
                resutls.Add(tmpRes);
                return resutls;
            }

            if (model.OrderDetails == null)
            {
                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                tmpRes.DA_Order_Id = model.Order.Id;
                tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                tmpRes.Store_Order_Status_DT = DateTime.Now;
                tmpRes.Succeded = false;
                tmpRes.Errors = "No Order Details Exists...";
                resutls.Add(tmpRes);
                return resutls;
            }



            string Error = "";
            string ExtEcrName = posInfoFlow.GetExtEcrName(dbInfo, model.StoreModel.PosId ?? 0);
            //foreach (DA_OrderModel item in model.Orders)
            //{
            try
            {
                //Κατάσταση παραγγελίας (Received = 0,Pending = 1,Preparing = 2,Ready = 3,Onroad = 4,Canceled = 5,Complete = 6)

                bool AddNewOrderStatus;

                OrderStatusEnum lastStatus = OrderStatusEnum.Received;

                //Updates or Insert Customer to Delivery Customer Tables. Not for status Cancel
                GuestModel guestModel = new GuestModel();
                DeliveryCustomerModel deliveryCustomer = new DeliveryCustomerModel();
                InvoiceTypeModel invType = new InvoiceTypeModel();
                AccountModel accModel = new AccountModel();
                PosInfoModel posInfo = new PosInfoModel();

                //Δελτιο Παραγγελίας
                InvoiceTypeModel invoiceType = new InvoiceTypeModel();
                PosInfoDetailModel posInfoDetDP = new PosInfoDetailModel();

                if (model.Order.Status != OrderStatusEnum.Canceled)
                {
                    model.Customer.ExtType = (ExternalSystemOrderEnum)model.Order.ExtType;
                    deliveryCustomer = daFlows.UpsertCustomer(dbInfo, model.Customer, model.Addresses, (int) model.Order.OrderType, out Error, ref guestModel);

                    invType = invTypeTask.GetInvoiceTypeByType(dbInfo, model.Order.InvoiceType);
                    accModel = accTask.GetAccountByType(dbInfo, model.Order.AccountType);

                    //Αν πρέπει να δημιουργηθεί Δελτιο Παραγγελίας CreateDP = true τότε μαζεύει δεδομένα για το Δελτιο Παραγγελίας αλλιώς για το παραστατικό που έρχεται...
                    if (CreateDP)
                    {
                        if (model.Order.OrderType == OrderTypeStatus.DineIn)
                        {
                            long dineInPosInfoDetailId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DineInPosInfoDetailIdForOrder");
                            posInfoDetDP = posInfoDetTask.GetSinglePosInfoDetail(dbInfo, dineInPosInfoDetailId, null, null);
                            if (posInfoDetDP == null)
                            {
                                posInfoDetDP = posInfoDetFlow.GetSinglePosInfoDetail(dbInfo, (long)(model.StoreModel.PosId ?? 0), 2);
                            }
                        }
                        else
                        {
                            posInfoDetDP = posInfoDetFlow.GetSinglePosInfoDetail(dbInfo, (long)(model.StoreModel.PosId ?? 0), 2);
                        }
                        invoiceType = invTypeTask.GetInvoiceTypeByType(dbInfo, 2);
                    }
                    else
                    {
                        if (model.Order.OrderType == OrderTypeStatus.DineIn)
                        {
                            long dineInPosInfoDetailId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DineInPosInfoDetailIdForInvoice");
                            posInfoDetDP = posInfoDetTask.GetSinglePosInfoDetail(dbInfo, dineInPosInfoDetailId, null, null);
                            if (posInfoDetDP == null)
                            {
                                posInfoDetDP = posInfoDetFlow.GetSinglePosInfoDetail(dbInfo, (long)(model.StoreModel.PosId ?? 0), 2);
                            }
                        }
                        else {
                            posInfoDetDP = posInfoDetFlow.GetSinglePosInfoDetail(dbInfo, (long)(model.StoreModel.PosId), invType.Type ?? 0);
                        }
                        invoiceType = invTypeTask.GetInvoiceTypeByType(dbInfo, invType.Type ?? 0);
                    }
                    posInfo = posInfoFlow.GetSinglePosInfo(dbInfo, model.StoreModel.PosId ?? 0);
                }

                if (model.Order.Status < OrderStatusEnum.Ready)
                {
                    bool isNew = false;
                    if (model.Order.StoreOrderId > 0)
                    {
                        isNew = false;
                        daFlows.CheckIfDA_OrderExists(dbInfo, model.Order, model.StoreModel, out lastStatus);

                        model.Order.Status = ReturnOrderStatus(model.Order.IsDelay, isNew,
                            model.Order.EstTakeoutDate ?? DateTime.Now, model.Order.Status, lastStatus,
                            model.Order.OrderType, model.Order.AccountType, model.Order.IsPaid,
                            out AddNewOrderStatus, dbInfo, model.Order.StoreOrderId);
                    }
                    else
                    {
                        //If Order Exists return's last order status. If last is bigger than current then current equals to last.
                        isNew = !daFlows.CheckIfDA_OrderExists(dbInfo, model.Order, model.StoreModel, out lastStatus);
                        model.Order.Status = ReturnOrderStatus(model.Order.IsDelay, isNew, model.Order.EstTakeoutDate ?? DateTime.Now,
                            model.Order.Status, lastStatus, model.Order.OrderType, model.Order.AccountType, model.Order.IsPaid, out AddNewOrderStatus, dbInfo, 0);
                    }

                    if (isNew)
                    {
                        //Insert
                        model.Order.StorePosInfoDetail = posInfoDetDP.Id;

                        if (deliveryCustomer == null)
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = false;
                            tmpRes.Errors = Error;
                            resutls.Add(tmpRes);
                            return resutls;
                        }

                        FullOrderWithTablesModel OrderToPost = ConvertDA_OrderToStoreOrder(dbInfo, model, guestModel,
                            deliveryCustomer, invType, invoiceType, accModel, posInfo, posInfoDetDP, out Error);

                        /*No model for insert created*/
                        if (OrderToPost == null)
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = false;
                            tmpRes.Errors = Error;
                            resutls.Add(tmpRes);
                            return resutls;
                        }

                        /*Order Status */
                        if (model.Order.OrderType == OrderTypeStatus.DineIn && 
                                                     (model.Order.AccountType == (short)AccountTypeEnum.CreditCard || model.Order.AccountType == (short)AccountTypeEnum.TicketCompliment)&& 
                                                     model.Order.IsPaid)
                        {
                            OrderToPost.OrderStatus = new OrderStatusModel()
                            {
                                Status = OrderStatusEnum.Complete,
                                TimeChanged = DateTime.Now,
                                StaffId = model.StoreModel.PosStaffId,
                                IsSend = false,
                                ExtState = model.Order.ExtType,
                                DAOrderId = model.Order.Id
                            };
                        }
                        else
                        {
                            OrderToPost.OrderStatus = new OrderStatusModel()
                            {
                                Status = model.Order.IsDelay ? OrderStatusEnum.Received : OrderStatusEnum.Preparing,
                                TimeChanged = DateTime.Now,
                                StaffId = model.StoreModel.PosStaffId,
                                IsSend = false,
                                ExtState = model.Order.ExtType,
                                DAOrderId = model.Order.Id
                            };
                        }


                        //Validate Receipt Model
                        if (!daFlows.ValidateFullOrder(dbInfo, OrderToPost, model.Order, model.Customer.Id,
                            ExternalSystemOrderEnum.DeliveryAgent, ModifyOrderDetailsEnum.FromScratch, out Error, false))
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = false;
                            tmpRes.Errors = Error;
                            resutls.Add(tmpRes);
                            return resutls;
                        }
                        //InsertNewInvoiceModel ResultsAfterOrderModelCreation;
                        //bool externalCommunication;
                        //FullOrderWithTablesModel FullOrderModel = daFlows.CreateFullOrderModel(Store, nrec, guestModel,
                        //    out ResultsAfterOrderModelCreation, out externalCommunication,
                        //    out Error, item.Id, 0);

                        /*Insert's new data to database*/
                        OrderStatusEnum currentStatus = model.Order.OrderType == OrderTypeStatus.DineIn && 
                                                                                 (model.Order.AccountType == (short)AccountTypeEnum.CreditCard || model.Order.AccountType == (short)AccountTypeEnum.TicketCompliment)  && 
                                                                                 model.Order.IsPaid ? OrderStatusEnum.Complete : model.Order.IsDelay ? OrderStatusEnum.Received : OrderStatusEnum.Preparing;
                        ResultsAfterDA_OrderActionsModel tmpResult = UpsertNewOrder(dbInfo, OrderToPost, PrintTypeEnum.PrintWhole,
                            ExtEcrName, model.Order.Id, currentStatus);
                        tmpResult.Old_Store_Order_Status = -100;
                        resutls.Add(tmpResult);

                        if (model.Order.OrderType == OrderTypeStatus.DineIn)
                        {
                            signal = orderTask.CreateSignalForDeliveryOrder(model.Order);
                        }
                    }
                    else
                    {
                        //Update
                        /*Check's last ordr status. Must be less than 3 (Ready)*/
                        OrderStatusModel lstStatus = orderStatusFlows.GetLastOrderStatusForOrderId(dbInfo, model.Order.StoreOrderId);
                        if (lstStatus != null && lstStatus.Status > OrderStatusEnum.Ready)
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = false;
                            tmpRes.Errors = string.Format(Resources.Messages.CANNOTCHANGEAGENTORDERCOMPLETED, model.Order.Id.ToString());
                            resutls.Add(tmpRes);
                            return resutls;
                        }
                        /*Check's the number of printed invoices. Must be less than 2 */
                        List<InvoiceModel> allInvoices = invFlow.GetInvoicesByOrderId(dbInfo, model.Order.StoreOrderId);
                        if (allInvoices != null && allInvoices.Count > 1)
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = false;
                            tmpRes.Errors = string.Format(Resources.Messages.CANNOTCHANGEAGENTORDERCOMPLETED, model.Order.Id.ToString());
                            resutls.Add(tmpRes);
                            return resutls;
                        }


                        if (deliveryCustomer == null)
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = false;
                            tmpRes.Errors = Error;
                            resutls.Add(tmpRes);
                            return resutls;
                        }
                        OrderStatusModel tmpOrdStat = orderStatusFlows.GetLastOrderStatusForOrderId(dbInfo, model.Order.StoreOrderId);
                        int oldOrderStatus = 0;
                        if (tmpOrdStat != null)
                            oldOrderStatus = (int)tmpOrdStat.Status;
                        bool NeedToUpdate = false;

                        //Set's posInfoDetail for Δελτίο Παραγγελίας
                        model.Order.StorePosInfoDetail = posInfoDetDP.Id;

                        //Get's full object with all tables from DB
                        FullOrderWithTablesModel StoreOrder = orderTask.GetFullOrderModel(dbInfo, model.Order.StoreOrderId,
                            model.Order.Id, ExternalSystemOrderEnum.DeliveryAgent);

                        //Convert's DA Order to Store Order model with all tables
                        FullOrderWithTablesModel orderToUpdate = ConvertDA_OrderToStoreOrder(dbInfo,
                            model, guestModel, deliveryCustomer, invType, invoiceType, accModel, posInfo, posInfoDetDP,
                            out Error);
                        if(orderToUpdate == null)
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = false;
                            tmpRes.Errors = "Cannot convert Delivery Order to local order \r\n" + Error;
                            resutls.Add(tmpRes);
                            return resutls;
                        }
                        /*Sets old Order No and Receipt No from Local Database*/
                        orderToUpdate.ReceiptNo = StoreOrder.ReceiptNo;
                        orderToUpdate.OrderNo = StoreOrder.OrderNo;

                        /*Items are the same so get old items from DB (StoreOrder) having all Id's needed */
                        long OldInvoiceId = 0;

                        string sConnect = @"data source=" + dbInfo.DataSource + 
                                          @";initial catalog=" + dbInfo.DataBase + 
                                          @";persist security info=True;user id=" + dbInfo.DataBaseUsername + ";password=" + 
                                          dbInfo.DataBasePassword + ";MultipleActiveResultSets=True;App=HIT.SymPOSium";
                        using (IDbConnection db = new SqlConnection(sConnect))
                        {
                            db.Open();
                            using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                            {
                                if (!model.Order.ItemsChanged)
                                {
                                    orderToUpdate.OrderDetails = new List<OrderDetailWithExtrasModel>();
                                    foreach (OrderDetailWithExtrasModel item in StoreOrder.OrderDetails)
                                        orderToUpdate.OrderDetails.Add(item);
                                }
                                else
                                {
                                    if ((StoreOrder.Id ?? 0) > 0)
                                    {
                                        List<InvoiceModel> tmpInvoices = invFlow.GetInvoicesByOrderId(db, StoreOrder.Id ?? 0, transact);
                                        if (tmpInvoices != null && tmpInvoices.Count > 0)
                                            OldInvoiceId = tmpInvoices[0].Id ?? 0;
                                    }
                                }
                                List<long> InvoicesIds = new List<long>();

                                if (model.Order.ItemsChanged || StoreOrder.Discount!= model.Order.Discount)
                                {
                                    NeedToUpdate = true;

                                    if (!orderTask.DeleteOrderItemsForUpdate(db, transact, model.Order.StoreOrderId, ref InvoicesIds, out Error))
                                    {
                                        ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                        tmpRes.DA_Order_Id = model.Order.Id;
                                        tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                                        tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                                        tmpRes.Store_Order_Status_DT = DateTime.Now;
                                        tmpRes.Succeded = false;
                                        tmpRes.Errors = Error;
                                        resutls.Add(tmpRes);
                                        transact.Rollback();
                                        return resutls;
                                    }
                                    orderToUpdate.OrderDetails = ordDetInvFlow.ConvertDA_OrderDetailsToOrderDetails(dbInfo,
                                        model.Order, model.OrderDetails, model.OrderDetailExtras, ModifyOrderDetailsEnum.FromScratch,
                                        guestModel.Id, true, (int) model.Order.OrderType, model.Order.TableId, deliveryCustomer, model.StoreModel.PosStaffId, out Error,
                                        db, transact);
                                    //ordDetInvFlow.ConvertDA_OrderDetailsToOrderDetails(
                                    //model.Order, model.OrderDetails, model.OrderDetailExtras, ModifyOrderDetailsEnum.FromScratch,
                                    //guestModel.Id, true, model.Order.OrderType, null, model.StoreModel.PosStaffId, out Error, db, transact);
                                }
                                else
                                {
                                    /*Initialize empty list to null for check if two model are equals*/
                                    foreach (OrderDetailWithExtrasModel item in orderToUpdate.OrderDetails)
                                    {
                                        item.Price = Math.Round((decimal)(item.Price ?? 0), 2);
                                        item.Discount = Math.Round((decimal)(item.Discount ?? 0), 2);
                                        item.TotalAfterDiscount = Math.Round((decimal)(item.TotalAfterDiscount ?? 0), 2);
                                        item.SalesTypeId = (int) model.Order.OrderType;

                                        if (item.OrderDetIngrendients == null || item.OrderDetIngrendients.Count < 1)
                                            item.OrderDetIngrendients = null;
                                        else
                                        {
                                            foreach (OrderDetailIngredientsModel ing in item.OrderDetIngrendients)
                                            {
                                                ing.Price = Math.Round((decimal)(ing.Price ?? 0), 2);
                                                ing.Discount = Math.Round((decimal)(ing.Discount ?? 0), 2);
                                                ing.TotalAfterDiscount = Math.Round((decimal)(ing.TotalAfterDiscount ?? 0), 2);
                                            }
                                        }

                                        foreach (OrderDetailInvoicesModel inv in item.OrderDetailInvoices)
                                        {
                                            inv.Price = Math.Round((decimal)(inv.Price ?? 0), 2);
                                            inv.Discount = Math.Round((decimal)(inv.Discount ?? 0), 2);
                                            inv.Net = Math.Round((decimal)(inv.Net ?? 0), 2);
                                            inv.Price = Math.Round((decimal)(inv.Price ?? 0), 2);
                                            inv.ReceiptSplitedDiscount = Math.Round((decimal)(inv.ReceiptSplitedDiscount ?? 0), 2);
                                            inv.TaxAmount = Math.Round((decimal)(inv.TaxAmount ?? 0), 2);
                                            inv.Total = Math.Round((decimal)(inv.Total ?? 0), 2);
                                            inv.TotalAfterDiscount = Math.Round((decimal)(inv.TotalAfterDiscount ?? 0), 2);
                                            inv.TotalBeforeDiscount = Math.Round((decimal)(inv.TotalBeforeDiscount ?? 0), 2);
                                            inv.VatAmount = Math.Round((decimal)(inv.VatAmount ?? 0), 2);
                                            inv.SalesTypeId = (int) model.Order.OrderType;
                                        }

                                    }

                                    if (orderToUpdate.Invoice == null || orderToUpdate.Invoice.Count < 1)
                                        orderToUpdate.Invoice = null;
                                    else if (orderToUpdate.Invoice[0].Transactions == null || orderToUpdate.Invoice[0].Transactions.Count < 1)
                                        orderToUpdate.Invoice[0].Transactions = null;
                                    else
                                    {
                                        if (orderToUpdate.Invoice[0].Transactions[0].CreditTransaction == null || orderToUpdate.Invoice[0].Transactions[0].CreditTransaction.Count < 1)
                                            orderToUpdate.Invoice[0].Transactions[0].CreditTransaction = null;
                                        if (orderToUpdate.Invoice[0].Transactions[0].InvoiceGuest == null || orderToUpdate.Invoice[0].Transactions[0].InvoiceGuest.Count < 1)
                                            orderToUpdate.Invoice[0].Transactions[0].InvoiceGuest = null;
                                        if (orderToUpdate.Invoice[0].Transactions[0].TransferToPms == null || orderToUpdate.Invoice[0].Transactions[0].TransferToPms.Count < 1)
                                            orderToUpdate.Invoice[0].Transactions[0].TransferToPms = null;
                                    }

                                    foreach (OrderDetailWithExtrasModel item in StoreOrder.OrderDetails)
                                    {
                                        item.Price = Math.Round((decimal)(item.Price ?? 0), 2);
                                        item.Discount = Math.Round((decimal)(item.Discount ?? 0), 2);
                                        item.TotalAfterDiscount = Math.Round((decimal)(item.TotalAfterDiscount ?? 0), 2);


                                        if (item.OrderDetIngrendients == null || item.OrderDetIngrendients.Count < 1)
                                            item.OrderDetIngrendients = null;
                                        else
                                        {
                                            foreach (OrderDetailIngredientsModel ing in item.OrderDetIngrendients)
                                            {
                                                ing.Price = Math.Round((decimal)(ing.Price ?? 0), 2);
                                                ing.Discount = Math.Round((decimal)(ing.Discount ?? 0), 2);
                                                ing.TotalAfterDiscount = Math.Round((decimal)(ing.TotalAfterDiscount ?? 0), 2);
                                            }
                                        }

                                        foreach (OrderDetailInvoicesModel inv in item.OrderDetailInvoices)
                                        {
                                            inv.Price = Math.Round((decimal)(inv.Price ?? 0), 2);
                                            inv.Discount = Math.Round((decimal)(inv.Discount ?? 0), 2);
                                            inv.Net = Math.Round((decimal)(inv.Net ?? 0), 2);
                                            inv.Price = Math.Round((decimal)(inv.Price ?? 0), 2);
                                            inv.ReceiptSplitedDiscount = Math.Round((decimal)(inv.ReceiptSplitedDiscount ?? 0), 2);
                                            inv.TaxAmount = Math.Round((decimal)(inv.TaxAmount ?? 0), 2);
                                            inv.Total = Math.Round((decimal)(inv.Total ?? 0), 2);
                                            inv.TotalAfterDiscount = Math.Round((decimal)(inv.TotalAfterDiscount ?? 0), 2);
                                            inv.TotalBeforeDiscount = Math.Round((decimal)(inv.TotalBeforeDiscount ?? 0), 2);
                                            inv.VatAmount = Math.Round((decimal)(inv.VatAmount ?? 0), 2);
                                        }
                                    }

                                    if (StoreOrder.Invoice == null || StoreOrder.Invoice.Count < 1)
                                        StoreOrder.Invoice = null;
                                    else if (StoreOrder.Invoice[0].Transactions == null || StoreOrder.Invoice[0].Transactions.Count < 1)
                                        StoreOrder.Invoice[0].Transactions = null;
                                    else
                                    {
                                        if (StoreOrder.Invoice[0].Transactions[0].CreditTransaction == null || StoreOrder.Invoice[0].Transactions[0].CreditTransaction.Count < 1)
                                            StoreOrder.Invoice[0].Transactions[0].CreditTransaction = null;
                                        if (StoreOrder.Invoice[0].Transactions[0].InvoiceGuest == null || StoreOrder.Invoice[0].Transactions[0].InvoiceGuest.Count < 1)
                                            StoreOrder.Invoice[0].Transactions[0].InvoiceGuest = null;
                                        if (StoreOrder.Invoice[0].Transactions[0].TransferToPms == null || StoreOrder.Invoice[0].Transactions[0].TransferToPms.Count < 1)
                                            StoreOrder.Invoice[0].Transactions[0].TransferToPms = null;
                                    }
                                    /*Ends Initialization for empty list to null to check if two model are equals*/

                                    //Compare's two models. One from Delivery Agent Converted to Store Order Model and One from Database, Excluded same fields as Id, OrderId etc....
                                    CompareTwoObjects compare = new CompareTwoObjects();
                                    string[] IgnoreFields = { "Id", "OrderId", "OrderDetailId", "InvoicesId", "OrderDetailIgredientsId", "CreationTS",
                                                              "InvoiceId","TransactionId", "TransferIdentifier", "Guid", "isDAModified", "Day", "Hashcode", "Invoice",
                                                              "Counter", "IngredientId", "StatusTS" };
                                    NeedToUpdate = !compare.AreObjectsEqual(orderToUpdate, StoreOrder, out Error, IgnoreFields);
                                }

                                if (NeedToUpdate)
                                {
                                    if (StoreOrder.Invoice != null && StoreOrder.Invoice.Count == 1)
                                    {
                                        if (StoreOrder.Invoice[0].Transactions != null && StoreOrder.Invoice[0].Transactions.Count == 1)
                                        {
                                            TransactionsExtraModel item = StoreOrder.Invoice[0].Transactions[0];

                                            //Delete's old Invoice Guest Trans Model
                                            if (item.InvoiceGuest != null && item.InvoiceGuest.Count == 1)
                                                invGuestTransFlow.DeleteInvoiceGuestTransaction(db, transact, item.InvoiceGuest[0]);

                                            //Delete's old Credit Transaction by adding same item with amount equal to (-1)*Amount
                                            if (item.CreditTransaction != null && item.CreditTransaction.Count == 1)
                                            {
                                                CreditTransactionsModel crTr = item.CreditTransaction[0];
                                                crTr.Amount = (-1) * crTr.Amount;
                                                crTr.Id = null;
                                                credTransactFlow.AddNewCreditTransaction(db, crTr, transact, out Error);
                                                if (!string.IsNullOrEmpty(Error))
                                                {
                                                    logger.Error("Add CreditTransaction : " + Error);

                                                    ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                                    tmpRes.DA_Order_Id = model.Order.Id;
                                                    tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                                                    tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                                                    tmpRes.Store_Order_Status_DT = DateTime.Now;
                                                    tmpRes.Succeded = false;
                                                    tmpRes.Errors = Error;
                                                    resutls.Add(tmpRes);

                                                    transact.Rollback();
                                                    return resutls;
                                                }
                                            }

                                            //Delete's old Transfer To PMS by adding same record with Total equal to (-1)*Total
                                            if (item.TransferToPms != null && item.TransferToPms.Count == 1)
                                            {
                                                TransferToPmsModel transfToPms = item.TransferToPms[0];
                                                transfToPms.Id = null;
                                                transfToPms.Points = (-1) * transfToPms.Points;
                                                transfToPms.Total = (-1) * transfToPms.Total;
                                                transfToPms.PmsResponseId = null;
                                                transfToPmsFlow.AddNewTransferToPms(db, transfToPms, transact, out Error);
                                                if (!string.IsNullOrEmpty(Error))
                                                {
                                                    logger.Error("Add Transaction : " + Error);

                                                    ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                                    tmpRes.DA_Order_Id = model.Order.Id;
                                                    tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                                                    tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                                                    tmpRes.Store_Order_Status_DT = DateTime.Now;
                                                    tmpRes.Succeded = false;
                                                    tmpRes.Errors = Error;
                                                    resutls.Add(tmpRes);

                                                    transact.Rollback();
                                                    return resutls;
                                                }
                                            }

                                        }
                                    }
                                    //Set's Old Order Id To Model for Update
                                    orderToUpdate.Id = StoreOrder.Id;

                                    //Set's Old Invoice Id to Model For Update
                                    long counter = 0;
                                    long OrderDetId = 0;

                                    if (OldInvoiceId > 0)
                                    {
                                        InvoiceWithTablesModel tmpInv = invFlow.GetInvoiceFromOldId(db, transact, OldInvoiceId);
                                        if (tmpInv != null)
                                        {
                                            foreach (InvoiceWithTablesModel item in orderToUpdate.Invoice)
                                            {
                                                item.Id = tmpInv.Id;

                                                //Set's old Ids for InvoiceShippingDetail
                                                if (item.InvoiceShippings != null && StoreOrder.Invoice != null && StoreOrder.Invoice[0].InvoiceShippings != null)
                                                {
                                                    long oldInvShpid = StoreOrder.Invoice[0].InvoiceShippings.Find(f => f.InvoicesId == tmpInv.Id).Id;
                                                    item.InvoiceShippings.ForEach(f => f.Id = oldInvShpid);
                                                }
                                            }
                                        }
                                    }
                                    else if(model.Order.Discount == StoreOrder.Discount)
                                    {
                                        if (orderToUpdate.OrderDetails != null && orderToUpdate.OrderDetails.Count > 0)
                                            OrderDetId = orderToUpdate.OrderDetails[0].Id ?? 0;
                                        if (orderToUpdate.OrderDetails != null && orderToUpdate.OrderDetails.Count > 0 && orderToUpdate.OrderDetails[0].OrderDetailInvoices != null)
                                            counter = StoreOrder.OrderDetails[0].OrderDetailInvoices.Find(f => f.OrderDetailId == OrderDetId).Counter ?? 0;
                                        if (orderToUpdate.Invoice != null && StoreOrder.Invoice != null)
                                        {
                                            foreach (InvoiceWithTablesModel item in orderToUpdate.Invoice)
                                            {
                                                var fld = StoreOrder.Invoice.Find(f => f.Counter == counter && f.InvoiceTypeId == item.InvoiceTypeId);
                                                if (fld != null)
                                                    item.Id = fld.Id;

                                                //Set's old Ids for InvoiceShippingDetail
                                                if (item.InvoiceShippings != null && StoreOrder.Invoice != null && StoreOrder.Invoice[0].InvoiceShippings != null)
                                                {
                                                    long oldInvShpid = StoreOrder.Invoice[0].InvoiceShippings.Find(f => f.InvoicesId == fld.Id).Id;
                                                    item.InvoiceShippings.ForEach(f => f.Id = oldInvShpid);
                                                }
                                            }
                                        }
                                    }
                                    if (AddNewOrderStatus)
                                    {
                                        orderToUpdate.OrderStatus = new OrderStatusModel()
                                        {
                                            Status = model.Order.IsDelay ? OrderStatusEnum.Received : model.Order.Status,
                                            TimeChanged = DateTime.Now,
                                            StaffId = model.StoreModel.PosStaffId,
                                            IsSend = false,
                                            ExtState = model.Order.ExtType,
                                            DAOrderId = model.Order.Id
                                        };
                                    }

                                    OrderStatusEnum currentStatus = model.Order.OrderType == OrderTypeStatus.DineIn && 
                                                                    (model.Order.AccountType == (short)AccountTypeEnum.CreditCard || model.Order.AccountType == (short)AccountTypeEnum.TicketCompliment) && 
                                                                    model.Order.IsPaid ? OrderStatusEnum.Complete : model.Order.IsDelay ? OrderStatusEnum.Received : OrderStatusEnum.Preparing;
                                    ResultsAfterDA_OrderActionsModel tmpResult = UpsertNewOrder(dbInfo, orderToUpdate, PrintTypeEnum.PrintWhole,
                                    ExtEcrName, model.Order.Id, currentStatus, db, transact);
                                    tmpResult.Old_Store_Order_Status = oldOrderStatus;
                                    resutls.Add(tmpResult);

                                    //orderToUpdate.OrderNo = StoreOrder.OrderNo;
                                    //orderToUpdate.ReceiptNo = StoreOrder.ReceiptNo;
                                    //if (orderToUpdate.Invoice != null && orderToUpdate.Invoice.Count > 0)
                                    //    orderToUpdate.Invoice.ForEach(f => f.Counter = (int)orderToUpdate.ReceiptNo);

                                }
                                else
                                {
                                    //Record not needed to update but return true to reduce IsSend field to Delivery Agent
                                    ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                    tmpRes.DA_Order_Id = model.Order.Id;
                                    tmpRes.Store_Order_Id = orderToUpdate.Id ?? 0;
                                    tmpRes.Store_Order_Status = model.Order.IsDelay ? OrderStatusEnum.Received : OrderStatusEnum.Preparing;
                                    tmpRes.Store_Order_Status_DT = DateTime.Now;
                                    tmpRes.Succeded = true;
                                    tmpRes.Old_Store_Order_Status = oldOrderStatus;
                                    tmpRes.Errors = "";
                                    resutls.Add(tmpRes);
                                    return resutls;
                                }
                                try
                                {
                                    if (resutls[0].Succeded)
                                        transact.Commit();
                                    else
                                        transact.Rollback();
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
                else if (model.Order.Status == OrderStatusEnum.Canceled)  //(daOrder.Status == OrderStatusEnum.Canceled)
                {
                    if (orderTask.CheckIfOrderIsCanceled(dbInfo, model.Order.StoreOrderId, model.Order.Id, ExternalSystemOrderEnum.DeliveryAgent, out Error))
                    {
                        ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                        tmpRes.DA_Order_Id = model.Order.Id;
                        tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                        tmpRes.Store_Order_Status = OrderStatusEnum.Canceled;
                        tmpRes.Store_Order_Status_DT = DateTime.Now;
                        tmpRes.Succeded = true;
                        tmpRes.Errors = Error;
                        resutls.Add(tmpRes);
                        return resutls;
                    }
                    int oldOrderStatus = (int)orderStatusFlows.GetLastOrderStatusForOrderId(dbInfo, model.Order.StoreOrderId).Status;
                    //bool CanDelete = false;
                    //if (model.Order.StoreOrderId > 0)
                    //    CanDelete = CanCancelOrDeleteState(dbInfo, model.Order.StoreOrderId) == 0;
                    //else
                    //    CanDelete = CanCancelOrDeleteState(dbInfo, model.Order.Id, ExternalSystemOrderEnum.DeliveryAgent) == 0;

                    FullOrderWithTablesModel orderToCancel = orderTask.GetFullOrderModel(dbInfo, model.Order.StoreOrderId, model.Order.Id, ExternalSystemOrderEnum.DeliveryAgent);
                    if (orderToCancel != null)
                    {
                        //InvoiceTypeModel invModel = invTypeTask.GetSingleInvoiceType(dbInfo, orderToCancel.Invoice[0].InvoiceTypeId ?? 0);

                        InvoiceTypeModel invTypeModel = null;
                        InvoiceModel invModel = null;
                        List<InvoiceModel> getInvoices = invFlow.GetInvoicesByOrderId(dbInfo, orderToCancel.Id ?? 0);

                        if (getInvoices == null || getInvoices.Count < 1)
                        {
                            ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                            tmpRes.DA_Order_Id = model.Order.Id;
                            tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                            tmpRes.Store_Order_Status = OrderStatusEnum.Canceled;
                            tmpRes.Store_Order_Status_DT = DateTime.Now;
                            tmpRes.Succeded = true;
                            tmpRes.Errors = "";
                            resutls.Add(tmpRes);
                            return resutls;
                        }
                        else
                        {
                            foreach (InvoiceModel item in getInvoices)
                            {
                                invModel = item;
                                invTypeModel = invTypeTask.GetSingleInvoiceType(dbInfo, invModel.InvoiceTypeId ?? 0);
                                if (invTypeModel.Type != 2)
                                    break;
                            }
                        }

                        if (invTypeModel.Type == 2) /*Δελτίο Παραγγελίας. Λιγοτέροι έλεγχοι και κλήση της procedure*/
                        {
                            if ((orderToCancel.IsDeleted ?? false) == true || (invModel.IsDeleted ?? false) == true || (invModel.IsVoided ?? false) == true)
                            {
                                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                tmpRes.DA_Order_Id = model.Order.Id;
                                tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                                tmpRes.Store_Order_Status = OrderStatusEnum.Canceled;
                                tmpRes.Store_Order_Status_DT = DateTime.Now;
                                tmpRes.Succeded = true;
                                tmpRes.Old_Store_Order_Status = oldOrderStatus;
                                tmpRes.Errors = "Invoice Id " + invModel.Id.ToString() + " for order id = " + orderToCancel.Id.ToString() + " already canceled";
                                resutls.Add(tmpRes);
                                return resutls;
                            }

                            signal = invFlow.Cancel_DP_Receipt(dbInfo, invModel.Id ?? 0, orderToCancel.PosId, orderToCancel.StaffId, out Error);
                            if (signal == null)
                            {
                                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                tmpRes.DA_Order_Id = model.Order.Id;
                                tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                                tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                                tmpRes.Store_Order_Status_DT = DateTime.Now;
                                tmpRes.Succeded = false;
                                tmpRes.Errors = Error;
                                resutls.Add(tmpRes);
                                return resutls;
                            }
                            else
                            {
                                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                tmpRes.DA_Order_Id = model.Order.Id;
                                tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                                tmpRes.Store_Order_Status = OrderStatusEnum.Canceled;
                                tmpRes.Store_Order_Status_DT = DateTime.Now;
                                tmpRes.Succeded = true;
                                tmpRes.Old_Store_Order_Status = oldOrderStatus;
                                tmpRes.Errors = "";
                                resutls.Add(tmpRes);
                                return resutls;
                            }

                        }
                        else
                        {


                            signal = invFlow.CancelReceipt(dbInfo, invModel.Id ?? 0, orderToCancel.PosId, orderToCancel.StaffId, false, out Error);
                            if (signal == null)
                            {
                                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                tmpRes.DA_Order_Id = model.Order.Id;
                                tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                                tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                                tmpRes.Store_Order_Status_DT = DateTime.Now;
                                tmpRes.Succeded = false;
                                tmpRes.Errors = Error;
                                resutls.Add(tmpRes);
                                return resutls;
                            }
                            else
                            {
                                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                                tmpRes.DA_Order_Id = model.Order.Id;
                                tmpRes.Store_Order_Id = orderToCancel.Id ?? 0;
                                tmpRes.Store_Order_Status = OrderStatusEnum.Canceled;
                                tmpRes.Store_Order_Status_DT = DateTime.Now;
                                tmpRes.Succeded = true;
                                tmpRes.Old_Store_Order_Status = oldOrderStatus;
                                tmpRes.Errors = "";
                                resutls.Add(tmpRes);
                                return resutls;
                            }
                        }
                    }
                    else
                    {
                        ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                        tmpRes.DA_Order_Id = model.Order.Id;
                        tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                        tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                        tmpRes.Store_Order_Status_DT = DateTime.Now;
                        tmpRes.Succeded = false;
                        tmpRes.Errors = "Order Not Found";
                        resutls.Add(tmpRes);
                        return resutls;
                    }


                }
                else
                {
                    ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                    tmpRes.DA_Order_Id = model.Order.Id;
                    tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                    tmpRes.Store_Order_Status = model.Order.Status;
                    tmpRes.Store_Order_Status_DT = DateTime.Now;
                    tmpRes.Succeded = true;
                    tmpRes.Errors = "";
                    resutls.Add(tmpRes);
                    return resutls;

                    //logger.Info("Delivery Order with Not Pending or Canceled status:" + daOrder.Status + " id:" + daOrder.Id);
                    //logger.Info(daOrder);
                }

            }
            catch (Exception ex)
            {
                logger.Info(ex.ToString());
                ResultsAfterDA_OrderActionsModel tmpRes = new ResultsAfterDA_OrderActionsModel();
                tmpRes.DA_Order_Id = model.Order.Id;
                tmpRes.Store_Order_Id = model.Order.StoreOrderId;
                tmpRes.Store_Order_Status = OrderStatusEnum.Open;
                tmpRes.Store_Order_Status_DT = DateTime.Now;
                tmpRes.Succeded = false;
                tmpRes.Errors = ex.ToString();//ex.Message + " " + (string.IsNullOrEmpty(ex.InnerException.Message) ? "" : ex.InnerException.Message);
                resutls.Add(tmpRes);
            }

            //} //For-each Old
            return resutls;
        }

        /// <summary>
        /// Check's if Invoice Shipping Detail Need Update
        /// </summary>
        /// <param name="shipAdr"></param>
        /// <param name="billAdr"></param>
        /// <param name="phone"></param>
        /// <param name="deliveryCustomer"></param>
        /// <param name="StoreRemarks"></param>
        /// <param name="invShip"></param>
        /// <returns></returns>
        private bool CheckForAddressesChange(DeliveryCustomersShippingAddressModel shipAdr, DeliveryCustomersBillingAddressModel billAdr,
            DeliveryCustomersPhonesModel phone, DeliveryCustomerModel deliveryCustomer, string StoreRemarks, InvoiceShippingDetailsModel invShip)
        {
            if (billAdr == null && ((invShip.BillingAddressId ?? 0) != 0))
                return true;
            if (billAdr != null && ((invShip.BillingAddressId ?? 0) == 0))
                return true;
            if (shipAdr == null && ((invShip.ShippingAddressId ?? 0) != 0))
                return true;
            if (shipAdr != null && ((invShip.ShippingAddressId ?? 0) == 0))
                return true;
            if (phone == null && !string.IsNullOrEmpty(invShip.Phone))
                return true;
            if (phone != null && string.IsNullOrEmpty(invShip.Phone))
                return true;

            return invShip.BillingAddress != billAdr.AddressStreet ||
                invShip.BillingAddressId != billAdr.ID ||
                invShip.BillingCity != billAdr.City ||
                invShip.Latitude != double.Parse(shipAdr.Latitude) ||
                invShip.Longtitude != double.Parse(shipAdr.Longtitude) ||
                invShip.ShippingAddress != shipAdr.AddressStreet ||
                invShip.ShippingAddressId != shipAdr.ID ||
                invShip.ShippingCity != shipAdr.City ||
                invShip.ShippingZipCode != shipAdr.Zipcode ||
                invShip.Phone != phone.PhoneNumber ||
                invShip.BillingDOY != deliveryCustomer.DOY ||
                invShip.BillingJob != deliveryCustomer.BillingJob ||
                invShip.BillingName != deliveryCustomer.BillingName ||
                invShip.BillingVatNo != deliveryCustomer.BillingVatNo ||
                invShip.BillingZipCode != billAdr.Zipcode ||
                invShip.CustomerID != deliveryCustomer.ID ||
                invShip.CustomerName != deliveryCustomer.LastName + " " + deliveryCustomer.FirstName ||
                invShip.CustomerRemarks != deliveryCustomer.Comments ||
                invShip.Floor != deliveryCustomer.Floor ||
                invShip.StoreRemarks != StoreRemarks;
        }

        // <summary>
        /// Check Client Store Status and Order Type Status.
        /// Return true if Store can accept order type status
        /// </summary>
        /// <param name="StoreStatus"></param>
        /// <param name="OrderType"></param>
        /// <returns></returns>
        public bool CheckClientStoreOrderStatus(DBInfoModel dbInfo, long StoreId, OrderTypeStatus OrderType)
        {
            return daFlows.CheckClientStoreOrderStatus(dbInfo, StoreId, OrderType);
        }

        /// <summary>
        /// Add's new order to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewOrder(DBInfoModel Store, OrderModel item)
        {
            return orderTask.AddNewOrder(Store, item);
        }

        /// <summary>
        /// Insert an order to db with all 
        /// association tables such as OrderDetail, invoices, ....
        /// Return's a model with result
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <param name="PrinterType"></param>
        /// <param name="ExtEcrName"></param>
        /// <param name="DA_OrderId"></param>
        /// <param name="OrderStatus"></param>
        /// <param name="dbExists"></param>
        /// <param name="dbTransactExist"></param>
        /// <returns></returns>
        public ResultsAfterDA_OrderActionsModel UpsertNewOrder(DBInfoModel dbInfo, FullOrderWithTablesModel order, PrintTypeEnum PrinterType,
            string ExtEcrName, long? DA_OrderId = 0, OrderStatusEnum OrderStatus = 0, IDbConnection dbExists = null, IDbTransaction dbTransactExist = null)
        {
            ResultsAfterDA_OrderActionsModel results = new ResultsAfterDA_OrderActionsModel();
            string Error = "";

            try
            {
                long OrderId = 0;
                long OrderNo = 0;
                bool bAllOK = orderTask.UpSertFullOrderModel(dbInfo, order, out OrderId, out OrderNo, out Error, dbExists, dbTransactExist);
                if (!bAllOK)
                {
                    results.Succeded = false;
                    results.DA_Order_Id = DA_OrderId ?? 0;
                    results.Errors = Error;
                    results.Store_Order_Id = OrderId;
                    results.Store_Order_No = OrderNo;
                    results.Store_Order_Status = OrderStatusEnum.Open;
                    results.Store_Order_Status_DT = DateTime.Now;
                }
                else
                {
                    OrderModel retOrder = orderTask.GetOrderByDAId(dbInfo, long.Parse(order.ExtKey), (ExternalSystemOrderEnum)order.ExtType);

                    results.Succeded = true;
                    results.DA_Order_Id = DA_OrderId ?? 0;
                    results.Errors = "";
                    results.Store_Order_Id = retOrder.Id ?? 0;
                    results.Store_Order_No = retOrder.OrderNo;
                    results.Store_Order_Status = OrderStatus;
                    results.Store_Order_Status_DT = DateTime.Now;
                    results.InvoiceId = order.Invoice[0].Id ?? 0;
                    results.PrintType = PrinterType;
                    results.ItemAdditionalInfo = null;
                    results.TempPrint = false;
                    results.ExtEcrName = ExtEcrName;
                }
            }
            catch (Exception ex)
            {
                results.Succeded = false;
                results.DA_Order_Id = DA_OrderId ?? 0;
                results.Errors = ex.ToString();
                results.Store_Order_Id = -1;
                results.Store_Order_Status = OrderStatusEnum.Open;
                results.Store_Order_Status_DT = DateTime.Now;
            }
            return results;
        }

        /// <summary>
        /// Return's InvoiceIs using OrderId and Tables OrderDetail and OrderDetailInvoices
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public long GetInvoiceIdByOrderId(DBInfoModel dbInfo, long OrderId)
        {
            return orderTask.GetInvoiceIdByOrderId(dbInfo, OrderId);
        }

        /// <summary>
        /// Return's all is-delayed orders to send to kitchen if EstTakeoutDate if 10 min less 
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<OrderModel> GetDelayedOrders(DBInfoModel Store)
        {
            return orderTask.GetDelayedOrders(Store);
        }

        /// <summary>
        /// Delete's all records for specific Order (OrderDetail,OrderDetailIgredients,OrderDetailInvoices)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="OrderId"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool DeleteOrderItemsForUpdate(IDbConnection db, IDbTransaction dbTransact, long OrderId, ref List<long> InvoiceId, out string Error)
        {
            return orderTask.DeleteOrderItemsForUpdate(db, dbTransact, OrderId, ref InvoiceId, out Error);
        }

        /// <summary>
        /// Return's a list of invoice ids for specific order
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<long> GetInvoiceIdsForOrderId(DBInfoModel dbInfo, long OrderId)
        {
            return orderTask.GetInvoiceIdsForOrderId(dbInfo, OrderId);
        }

        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderModel GetOrderById(DBInfoModel dbInfo, long OrderId)
        {
            return orderTask.GetOrderById(dbInfo, OrderId);
        }

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public int CanCancelOrDeleteState(DBInfoModel dbInfo, long OrderId)
        {
            return orderTask.CanCancelOrDeleteState(dbInfo, OrderId);
        }

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public int CanCancelOrDeleteState(DBInfoModel dbInfo, long DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            return orderTask.CanCancelOrDeleteState(dbInfo, DAOrderId, ExtType);
        }

        /// <summary>
        /// Return's an Order with all associated tables such as OrderDetail, OrderIngredients, OrderDetailInvoices......
        /// The result is an FullOrderWithTablesModel same to Post new Order Model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public FullOrderWithTablesModel GetFullOrderModel(DBInfoModel dbInfo, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            return orderTask.GetFullOrderModel(dbInfo, OrderId, DAOrderId, ExtType);
        }


        /// <summary>
        /// Convert's a Receipt Detail Model to OrderDetailInvoices Model
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isFiscal"></param>
        /// <param name="IsPrinted"></param>
        /// <param name="ReceiptCounter"></param>
        /// <param name="OrderNo"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="EndOfDayId"></param>
        /// <param name="Abbreviation"></param>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        private OrderDetailInvoicesModel ConvertFromReceiptModel(ReceiptDetails item,
            bool isFiscal, bool IsPrinted, long ReceiptCounter, long OrderNo, long PosInfoId,
            long? EndOfDayId, string Abbreviation, string CustomerID)
        {
            OrderDetailInvoicesModel ordInv = new OrderDetailInvoicesModel();
            ordInv.StaffId = item.StaffId;
            ordInv.PosInfoDetailId = item.PosInfoDetailId;
            ordInv.Counter = ReceiptCounter;
            ordInv.CreationTS = DateTime.Now;
            ordInv.IsPrinted = isFiscal ? false : IsPrinted;
            ordInv.CustomerId = CustomerID == null ? "" : CustomerID.ToString();
            ordInv.IsDeleted = false;
            ordInv.OrderDetailIgredientsId = item.IsExtra == 1 ? item.OrderDetailIgredientsId : null;
            ordInv.Price = item.Price;
            ordInv.Discount = item.ItemDiscount;
            ordInv.Net = item.ItemNet;
            ordInv.VatRate = item.ItemVatRate;
            ordInv.VatAmount = item.ItemVatValue;
            ordInv.VatId = item.VatId;
            ordInv.TaxId = item.TaxId;
            ordInv.VatCode = (int?)item.VatCode;
            ordInv.TaxAmount = item.ItemTaxAmount;
            ordInv.Qty = item.ItemQty;
            ordInv.Total = item.ItemGross ?? 0;
            ordInv.PricelistId = item.PricelistId;
            ordInv.ProductId = item.IsExtra == 1 ? null : item.ProductId;
            ordInv.Description = item.ItemDescr;
            ordInv.ItemCode = item.ItemCode;
            ordInv.ItemRemark = item.ItemRemark;
            ordInv.InvoiceType = item.InvoiceType;
            ordInv.TableId = item.TableId;
            ordInv.TableCode = item.TableCode;
            ordInv.RegionId = item.RegionId;
            ordInv.OrderNo = OrderNo;
            ordInv.IsExtra = item.IsExtra == 1;
            ordInv.PosInfoId = PosInfoId;
            ordInv.Abbreviation = Abbreviation;
            ordInv.DiscountId = null;                                       //For check
            ordInv.SalesTypeId = item.SalesTypeId;
            ordInv.ProductCategoryId = item.ProductCategoryId;
            ordInv.CategoryId = item.CategoryId;
            ordInv.ItemPosition = item.ItemPosition;
            ordInv.ItemSort = item.ItemSort;
            ordInv.ItemRegion = item.ItemRegion;
            ordInv.RegionPosition = item.RegionPosition;
            ordInv.ItemBarcode = item.ItemBarcode;
            ordInv.TotalBeforeDiscount = item.TotalBeforeDiscount;
            ordInv.TotalAfterDiscount = item.TotalBeforeDiscount - item.ItemDiscount;
            ordInv.ReceiptSplitedDiscount = item.ReceiptSplitedDiscount;
            ordInv.TableLabel = item.TableLabel;
            ordInv.EndOfDayId = EndOfDayId;

            return ordInv;
        }

        private FullOrderWithTablesModel ConvertReceiptModelToFullOrderModel(DBInfoModel dbInfo, Receipts receipt,
            ModifyOrderDetailsEnum orderStatus)
        {
            string Error = "";
            FullOrderWithTablesModel order = null;

            PosInfoModel posInfo = posInfoFlow.GetSinglePosInfo(dbInfo, receipt.PosInfoId ?? 0);

            GuestModel guest = null;
            if (receipt.ReceiptPayments != null)
                guest = guestTasks.GetGuestById(dbInfo, receipt.ReceiptPayments.FirstOrDefault().GuestId ?? 0);

            long ReceiptCounter = 0;
            bool isFiscal = posInfo != null ? (FiscalTypeEnum)posInfo.FiscalType == FiscalTypeEnum.Opos : false;

            if (receipt.InvoiceTypeId != null && receipt.InvoiceTypeId > 0)
                ReceiptCounter = posInfoDetFlow.GetNextCounter(dbInfo, posInfo.Id, receipt.PosInfoDetailId ?? 0, (int)(receipt.InvoiceTypeId ?? 0));
            else
                ReceiptCounter = posInfoDetFlow.GetNextCounter(dbInfo, posInfo.Id, receipt.PosInfoDetailId ?? 0);

            /*New Order*/  //For check
            if (orderStatus != ModifyOrderDetailsEnum.FromScratch)
                order = AutoMapper.Mapper.Map<FullOrderWithTablesModel>(orderTask.GetOrderById(dbInfo, receipt.ReceiptDetails.FirstOrDefault().OrderId ?? 0));

            if (order == null)
            {
                order = new FullOrderWithTablesModel();

                order.OrderNo = receipt.ReceiptDetails.FirstOrDefault().OrderNo ?? 0;
                order.Day = receipt.EndOfDayId == null ? DateTime.Now : (receipt.Day ?? DateTime.Now);
                order.Total = receipt.Total ?? 0;
                order.PosId = receipt.PosInfoId ?? 0;
                order.StaffId = receipt.StaffId ?? 0;
                order.EndOfDayId = null;
                order.Discount = receipt.Discount;
                order.ReceiptNo = ReceiptCounter;
                order.TotalBeforeDiscount = (receipt.Total + (receipt.Discount ?? 0)) ?? 0;
                order.PdaModuleId = (int?)receipt.PdaModuleId;
                order.ClientPosId = receipt.ClientPosId;
                order.IsDeleted = false;
                order.ExtType = receipt.ExtType;
                order.ExtKey = receipt.ExtKey;
                order.ExtObj = receipt.ExtObj;
                order.OrderOrigin = receipt.OrderOrigin ?? 0;
                order.Couver = receipt.Cover ?? 0;
                //newOrder.PosInfoDetailId = newInvoice.PosInfoDetailId;   //For check
            }
            


            /*Order Details with OrderDetailIngrendients and OrderDetailInvoices*/
            order.OrderDetails = new List<OrderDetailWithExtrasModel>();
            OrderDetailWithExtrasModel lastOrdDet = new OrderDetailWithExtrasModel();
            foreach (ReceiptDetails item in receipt.ReceiptDetails)
            {
                if (item.IsExtra != 1)
                {
                    /*OrderDetail  (startOd Old Code)*/
                    OrderDetailWithExtrasModel ordDet = null;
                    if (orderStatus != ModifyOrderDetailsEnum.FromScratch)
                    {
                        ordDet = AutoMapper.Mapper.Map<OrderDetailWithExtrasModel>(ordDetTask.GetOrderDetailById(dbInfo, item.OrderDetailId ?? 0));
                        if(ordDet != null && item.SelectedQuantity != null )
                        {
                            OrderDetailWithExtrasModel od = AutoMapper.Mapper.Map<OrderDetailWithExtrasModel>(ordDet);
                            OrderDetailInvoicesModel odi = od.OrderDetailInvoices.Find(f => f.IsExtra = false && f.ProductId == item.ProductId && f.PosInfoId == item.PosInfoId && f.PosInfoDetailId == item.PosInfoDetailId);
                            OrderDetailInvoicesModel startOdi = ordDetInvFlow.GetOrderDtailByOrderDetailId(dbInfo, item.OrderDetailId ?? 0, item.ProductId ?? 0,
                                null, item.PosInfoId, item.PosInfoDetailId, true, out Error);
                            if(startOdi == null || odi == null)
                            {
                                if (odi == null)
                                    logger.Error("No Order detail invoices found for OrderDetailId " + od.Id.ToString() +
                                                 " and ProductId = " + item.ProductId.ToString() +
                                                 " and PosInfoId = " + item.PosInfoId.ToString() +
                                                 " and PosInfoDetailId = " + item.PosInfoDetailId.ToString());
                                if (startOdi == null)
                                    logger.Error(Error);

                                return null;
                            }

                        }
                    }
                    if (ordDet == null)
                    {
                        ordDet = new OrderDetailWithExtrasModel();
                        ordDet.OrderId = item.OrderId;
                        ordDet.ProductId = item.ProductId;
                        ordDet.KitchenId = item.KitchenId;
                        ordDet.KdsId = item.KdsId;
                        ordDet.PreparationTime = item.PreparationTime;
                        ordDet.TableId = item.TableId;
                        ordDet.Status = item.Status;
                        ordDet.StatusTS = item.Status == null ? (DateTime?)null : DateTime.Now;
                        ordDet.Price = item.Price;
                        ordDet.PriceListDetailId = item.PriceListDetailId;
                        ordDet.Qty = item.ItemQty;
                        ordDet.SalesTypeId = item.SalesTypeId;
                        ordDet.Discount = item.ItemDiscount;
                        ordDet.PaidStatus = item.PaidStatus;
                        ordDet.TransactionId = null;                            //For check
                        ordDet.TotalAfterDiscount = item.TotalBeforeDiscount - item.ItemDiscount;
                        ordDet.GuestId = null;                                  //For check
                        ordDet.Couver = null;                                   //For check
                        ordDet.Guid = Guid.NewGuid();
                        ordDet.IsDeleted = false;
                        ordDet.PendingQty = null;                               //For check
                        ordDet.OrderDetailInvoices = new List<OrderDetailInvoicesModel>();
                        ordDet.OrderDetIngrendients = new List<OrderDetailIngredientsModel>();
                        order.OrderDetails.Add(ordDet);
                    }
                    
                    lastOrdDet = ordDet;
                }
                else
                {   /*OrderDetailIngredients*/

                    OrderDetailIngredientsModel ordIng = null;

                    if (orderStatus != ModifyOrderDetailsEnum.FromScratch)
                        ordIng = ordDetIngTask.GetOrderDetailIngredientsById(dbInfo, item.OrderDetailIgredientsId ?? 0);
                    if (ordIng == null)
                    {
                        ordIng = new OrderDetailIngredientsModel();
                        ordIng.IngredientId = item.OrderDetailIgredientsId;
                        ordIng.Qty = item.ItemQty;
                        ordIng.UnitId = null;                                   //For check
                        ordIng.Price = item.Price;
                        ordIng.PriceListDetailId = item.PriceListDetailId == null ? lastOrdDet.PriceListDetailId : item.PriceListDetailId;
                        ordIng.Discount = item.ItemDiscount;
                        ordIng.TotalAfterDiscount = item.TotalBeforeDiscount - item.ItemDiscount;
                        ordIng.IsDeleted = false;
                    }
                    lastOrdDet.OrderDetIngrendients.Add(ordIng);
                }

                /*Order Detail Invoices*/
                if (item.SelectedQuantity == null || item.SelectedQuantity == 0)
                    lastOrdDet.OrderDetailInvoices.Add(ConvertFromReceiptModel(item, isFiscal, receipt.IsPrinted,
                        ReceiptCounter, order.OrderNo, posInfo.Id, receipt.EndOfDayId, receipt.Abbreviation, receipt.CustomerID.ToString()));
            }


            /*Invoices*/
            order.Invoice = new List<InvoiceWithTablesModel>();
            InvoiceWithTablesModel inv = new InvoiceWithTablesModel();
            inv.Day = receipt.EndOfDayId == null ? DateTime.Now : receipt.Day;
            inv.Description = receipt.InvoiceDescription;
            inv.Cover = receipt.Cover;
            inv.Counter = (int)ReceiptCounter;
            inv.PosInfoDetailId = receipt.PosInfoDetailId;
            inv.StaffId = receipt.StaffId;
            inv.TableId = receipt.TableId;
            inv.Total = receipt.Total;
            inv.Discount = receipt.Discount;
            inv.DiscountRemark = receipt.DiscountRemark;
            inv.ClientPosId = receipt.ClientPosId;
            inv.PdaModuleId = receipt.PdaModuleId;
            inv.InvoiceTypeId = receipt.InvoiceTypeId;
            inv.Net = receipt.Net;
            inv.Vat = receipt.Vat;
            inv.TableSum = receipt.TableSum;
            inv.CashAmount = receipt.CashAmount;
            inv.BuzzerNumber = receipt.BuzzerNumber;
            inv.EndOfDayId = receipt.EndOfDayId;
            inv.LoyaltyDiscount = receipt.LoyaltyDiscount;
            inv.IsPrinted = isFiscal ? false : receipt.IsPrinted;
            inv.IsVoided = receipt.InvoiceTypeType == 3 || receipt.InvoiceTypeType == 8;
            inv.ForeignExchangeCurrency = receipt.ForeignExchangeCurrencyTo;
            inv.ForeignExchangeTotal = receipt.ForeignExchangeTotal;
            inv.ForeignExchangeDiscount = receipt.ForeignExchangeDiscount;
            if (receipt.InvoiceTypeType == 2)
            {
                inv.IsPaid = 0;
                inv.PaidTotal = 0;
            }
            else
            {
                if (receipt.CreateTransactions == false)
                {
                    inv.IsPaid = 0;
                    inv.PaidTotal = 0;
                }
                else
                {
                    inv.PaidTotal = receipt.ReceiptPayments != null ? receipt.ReceiptPayments.Sum(sm => sm.Amount) : 0;
                    if (inv.Total - inv.PaidTotal > 0) inv.IsPaid = 1; else inv.IsPaid = 2;
                }
            }
            inv.PaymentsDesc = receipt.ReceiptPayments != null && receipt.ReceiptPayments.Count() > 0 ?
                                    receipt.ReceiptPayments.Select(s => s.AccountDescription).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) :
                                    "";
            inv.Rooms = receipt.ReceiptPayments != null && receipt.ReceiptPayments.Count() > 0 ?
                            receipt.ReceiptPayments.Select(s => s.Room).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) :
                            "";

            inv.PosInfoId = posInfo.Id; //For check

            StringBuilder hashsting = new StringBuilder(receipt.Day.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            hashsting.Append(receipt.PosInfoId.Value);
            hashsting.Append(receipt.PosInfoDetailId.Value);
            hashsting.Append(receipt.InvoiceTypeType.Value);
            hashsting.Append(receipt.Total.Value.ToString("000000000.00", CultureInfo.GetCultureInfo("en-US")));
            hashsting.Append(receipt.StaffId);
            hashsting.Append(receipt.ReceiptDetails.Count());
            if (receipt.ReceiptDetails != null)
            {
                foreach (var item in receipt.ReceiptDetails)
                {
                    hashsting.Append(item.ItemDescr);
                    hashsting.Append(item.ItemQty);
                }
            }
            inv.Hashcode = MD5Helper.GetMD5Hash(hashsting.ToString() + inv.Day.Value.ToString("-yyyyMMdd"));

            // if (detailStatus == ModifyOrderDetailsEnum.FromScratch)    //For check  
            inv.OrderNo = receipt.ReceiptDetails.Select(s => s.OrderNo).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Trim().Remove(0, 1);
            //else
            //    newInvoice.OrderNo = receipt.OrderNo.Trim();


            //inv.ExtECRCode            //For check
            //inv.GuestId               //For check
            //inv.Hashcode              //For check
            //inv.Id                    //For check
            //inv.IsDeleted             //For check
            //inv.IsInvoiced            //For check
            //inv.OrderNo               //For check
            //inv.Tax                   //For check

            inv.InvoiceShippings = new List<InvoiceShippingDetailsModel>();
            inv.InvoiceShippings.Add(new InvoiceShippingDetailsModel
            {
                BillingAddress = receipt.BillingAddress,
                BillingAddressId = receipt.BillingAddressId,
                BillingCity = receipt.BillingCity,
                BillingZipCode = receipt.BillingZipCode,
                BillingName = receipt.BillingName,
                BillingVatNo = receipt.BillingVatNo,
                BillingDOY = receipt.BillingDOY,
                BillingJob = receipt.BillingJob,
                CustomerID = receipt.CustomerID,
                CustomerRemarks = receipt.CustomerRemarks,
                Floor = receipt.Floor,
                Latitude = receipt.Latitude,
                Longtitude = receipt.Longtitude,
                Phone = receipt.Phone,
                ShippingAddress = receipt.ShippingAddress,
                ShippingAddressId = receipt.ShippingAddressId,
                ShippingCity = receipt.ShippingCity,
                ShippingZipCode = receipt.ShippingZipCode,
                CustomerName = receipt.CreateTransactions ? receipt.CustomerName : (guest != null ? guest.LastName + " " + guest.FirstName : receipt.CustomerName)
            });

            /*Transaction*/
            inv.Transactions = new List<TransactionsExtraModel>();
            foreach (ReceiptPayments item in receipt.ReceiptPayments)
            {
                TransactionsExtraModel tr = new TransactionsExtraModel();
                tr.AccountId = (int?)item.AccountId ?? 0;
                tr.Amount = item.Amount ?? 0;
                tr.DepartmentId = receipt.DepartmentId;
                tr.Description = "Pay Off";
                tr.ExtDescription = "CreateTransactionsFromReceiptPayments";
                tr.StaffId = receipt.StaffId ?? 0;
                tr.TransactionType = (int)(item.TransactionType != null ? item.TransactionType : GetTransactionType((InvoiceTypesEnum)receipt.InvoiceTypeType));
                tr.PosInfoId = item.PosInfoId ?? 0;
                tr.InOut = GetTransactionInOut((InvoiceTypesEnum)receipt.InvoiceTypeType);
                tr.Day = inv.Day ?? DateTime.Now;
                tr.EndOfDayId = receipt.EndOfDayId == 0 ? null : receipt.EndOfDayId;

                tr.InvoiceGuest = new List<Invoice_Guests_TransModel>();

                /*Invoice Guest Trans*/
                if ((AccountType)item.AccountType == AccountType.Allowence || 
                    (AccountType)item.AccountType == AccountType.Coplimentary || 
                    (AccountType)item.AccountType == AccountType.Charge)
                {
                    Invoice_Guests_TransModel ingt = null;

                    if (item.GuestId != null)
                    {
                        if(receipt.ModifyOrderDetails == ModifyOrderDetailsEnum.PayOffOnly)
                        {
                            ingt = invGuestTransFlow.GetInvoiceGuestByInvoiceId(dbInfo, inv.Id ?? 0);
                            if(ingt == null)
                            {
                                ingt = new Invoice_Guests_TransModel();
                                ingt.GuestId = item.GuestId;
                            }
                        }
                        else
                        {
                            ingt = new Invoice_Guests_TransModel();
                            ingt.GuestId = item.GuestId;
                        }
                        tr.InvoiceGuest.Add(ingt);
                    }
                }

                /*Credit Transaction*/
                tr.CreditTransaction = new List<CreditTransactionsModel>();
                if((item.CreditAccountId??0) != 0)
                {
                    if (item.CreditTransactionAction != (short)CreditTransactionType.None)
                        tr.CreditTransaction.Add(CreateCreditTransaction(item.Amount ?? 0, (InvoiceTypesEnum)(receipt.InvoiceTypeType ?? -1), item.CreditTransactionAction,
                            receipt.ReceiptNo ?? 0, item.CreditCodeId ?? 0, item.CreditAccountId ?? 0, receipt.StaffId ?? 0, item.PosInfoId ?? 0));
                }

                //ReturnTransferToPms(dbInfo, )



                inv.Transactions.Add(tr);

            }



            //inv.InvoiceShippings

            return order;
        }

        /// <summary>
        /// Get's the transaction type
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get's transactions InOut status
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create's new CreditTransaction Model
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="invTypeType"></param>
        /// <param name="CreditTransactionAction"></param>
        /// <param name="ReceiptNo"></param>
        /// <param name="CreditCodeId"></param>
        /// <param name="CreditAccountId"></param>
        /// <param name="StaffId"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        private CreditTransactionsModel CreateCreditTransaction(decimal Amount, InvoiceTypesEnum invTypeType,
            short CreditTransactionAction, int ReceiptNo, long CreditCodeId,
            long CreditAccountId, long StaffId, long PosInfoId)
        {
            string descr = "";
            byte type = (byte)CreditTransactionAction;
            switch (invTypeType)
            {
                case InvoiceTypesEnum.PaymentReceipt:
                    if (CreditTransactionAction == (short)CreditTransactionType.AddCredit)
                        descr = "Add amount to Barcode Credit Account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.RemoveCredit)
                        descr = "Remove amount from Barcode Credit Account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.ReturnCredit)
                        descr = "Return amount from Barcode Credit Account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.ReturnAllCredits)
                        descr = "Return amount from Barcode Credit Account and close account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.PayLocker)
                    {
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        descr = "Remove amount from Barcode Credit Account due to Locker opening";
                        Amount = Amount * (-1);
                    }
                    else if (CreditTransactionAction == (short)CreditTransactionType.ReturnLocker)
                    {
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        descr = "Return amount to Barcode Credit Account due to Locker closing";
                    }
                    break;
                case InvoiceTypesEnum.RefundReceipt:
                    if (CreditTransactionAction == (short)CreditTransactionType.AddCredit)
                        descr = "Add amount to Barcode Credit Account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.RemoveCredit)
                        descr = "Remove amount from Barcode Credit Account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.ReturnCredit)
                        descr = "Return amount from Barcode Credit Account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.ReturnAllCredits)
                        descr = "Return amount from Barcode Credit Account and close account";
                    else if (CreditTransactionAction == (short)CreditTransactionType.PayLocker)
                    {
                        descr = "Remove amount from Barcode Credit Account due to Locker opening";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                        Amount = Amount * (-1);
                    }
                    else if (CreditTransactionAction == (short)CreditTransactionType.ReturnLocker)
                    {
                        descr = "Return amount to Barcode Credit Account due to Locker closing";
                        type = (byte)CreditTransactionType.ReturnAllCredits;
                    }
                    break;
                case InvoiceTypesEnum.Allowance:
                case InvoiceTypesEnum.Coplimentary:
                case InvoiceTypesEnum.Receipt:
                case InvoiceTypesEnum.Timologio:
                    descr = "Payment for Receipt " + ReceiptNo.ToString();
                    type = (byte)CreditTransactionType.RemoveCredit;
                    Amount = Amount * (-1);
                    break;
                case InvoiceTypesEnum.Void:
                    descr = "Cancel Payment for Receipt " + ReceiptNo.ToString();
                    type = (byte)CreditTransactionType.AddCredit;
                    Amount = Amount * (-1);
                    break;
            }
            return new CreditTransactionsModel()
            {
                CreditCodeId = CreditCodeId,
                CreditAccountId = CreditAccountId,
                CreationTS = DateTime.Now,
                Type = type,
                Description = descr,
                StaffId = StaffId,
                PosInfoId = PosInfoId,
                Amount = Amount,
                TransactionId = -1,
                InvoiceId = -1,
                EndOfDayId = 0
            };
        }

        private List<TransferToPmsModel> CreateTransferToPMS(DBInfoModel dbInfo, ReceiptPayments payment, Receipts r, TransactionsExtraModel tr, 
            InvoiceWithTablesModel i, bool isFiscal, long? hotelId, List<OrderDetailInvoicesModel> detailsFromDB)
        {
            List<TransferToPmsModel> tpms = new List<TransferToPmsModel>();
            try
            {
                if (r.ModifyOrderDetails == ModifyOrderDetailsEnum.PayOffOnly && r.Total < r.ReceiptDetails.Sum(s => s.ItemGross))
                {
                    logger.Info("Fixing Payoff Details for receipt id {0}" + r.Id);
                    foreach (OrderDetailInvoicesModel item in detailsFromDB)
                    {
                        r.ReceiptDetails.FirstOrDefault(w => w.ItemSort == item.ItemSort && w.OrderDetailId == item.OrderDetailId).ItemGross = item.Total;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            List<TransferMappingsModel> transferMapping = transfMappDT.GetTransferMappingsByHotelAndDepartment(dbInfo, r.DepartmentId ?? 0, (int)(hotelId ?? 0));

            var mappedDepartmentsQuery = (from q in r.ReceiptDetails
                                          join qqq in transferMapping
                                              on new { ProductCategoryId = q.ProductCategoryId, PriceListId = q.PricelistId, }
                                              equals new { ProductCategoryId = qqq.ProductCategoryId, PriceListId = qqq.PriceListId }
                                          select new
                                          {
                                              OrderDetailId = q.OrderDetailId,
                                              ItemSort = q.ItemSort,
                                              ProductId = q.ProductId,
                                              PosInfoId = q.PosInfoId,
                                              PosInfoDetailId = q.PosInfoDetailId,
                                              PosDepartmentId = r.DepartmentId,
                                              PosDepartment = r.DepartmentDescription,
                                              PmsDepartmentId = qqq.PmsDepartmentId,
                                              PmsDepartment = qqq.PmsDepDescription,
                                              ReceiptId = r.Id,
                                              ReceiptCounter = r.ReceiptNo,
                                              ReceiptAbbr = r.Abbreviation,
                                              Total = q.ItemGross,

                                          }).Distinct().ToList().GroupBy(g => g.PmsDepartmentId).Select(s => new
                                          {
                                              PosInfoId = s.FirstOrDefault().PosInfoId,
                                              PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                                              PosDepartmentId = s.FirstOrDefault().PosDepartmentId,
                                              PosDepartment = s.FirstOrDefault().PosDepartment,
                                              PmsDepartmentId = s.FirstOrDefault().PmsDepartmentId,
                                              PmsDepartment = s.FirstOrDefault().PmsDepartment,
                                              ReceiptId = s.FirstOrDefault().ReceiptId,
                                              ReceiptCounter = s.FirstOrDefault().ReceiptCounter,
                                              ReceiptAbbr = s.FirstOrDefault().ReceiptAbbr,
                                              Total = s.Select(ss => new { ItemSort = ss.ItemSort, Total = ss.Total }).Distinct().Sum(sm => sm.Total)
                                          }).Distinct().ToList();
            logger.Info("Transfer Mapping Total {0}, " + mappedDepartmentsQuery.Sum(s => s.Total) + "Receipt Total { 1}" + r.Total);
            if (mappedDepartmentsQuery.Sum(s => s.Total) > r.Total)
            {
                logger.Error("Transfer Mappings Total Error : mappedDepartmentsQuery.Sum(s => s.Total) > r.Total");
            }
            GuestModel guest = guestTasks.GetGuestById(dbInfo, payment.GuestId ?? 0);
            EODAccountAndAccountModel accExtra = accTask.GetEodAccountAndAccount(dbInfo, i.PosInfoId ?? 0, tr.AccountId);
            var IsNotSendingTransfer = (accExtra != null && (accExtra.SendsTransfer ?? false));
            var IsCreditCard = payment.AccountType == 4;
            var points = r.Points;
            if (tr.Amount == 0)
                return null;
            //If is payoff only mark as true else check if is fiscal and mark as false for extec
            var markRoomCharge = ModifyOrderDetailsEnum.ChangePaymentType == r.ModifyOrderDetails || isFiscal == false || (isFiscal == true && ModifyOrderDetailsEnum.PayOffOnly == r.ModifyOrderDetails && r.IsPrinted);
            var perc = tr.Amount == 0 ? 0 : Math.Round((Decimal)((i.Total / tr.Amount) * 100), 2);
            if (i.IsVoided == false)
            {
                if (r.ReceiptPayments.Count() == 1)
                    perc = 100;
                else if (r.ReceiptPayments.Sum(s => s.Amount) < i.Total)
                {
                    perc = Math.Round((Decimal)((r.ReceiptPayments.Sum(s => s.Amount) / tr.Amount) * 100), 2);
                }
            }
            if (mappedDepartmentsQuery.Count() > 0)
            {
                /*
                SendTransferRepository stRepository = new SendTransferRepository(db, payment.PosInfoId.Value, mappedDepartmentsQuery.FirstOrDefault().PosDepartmentId.Value);

                foreach (var mdq in mappedDepartmentsQuery)
                {

                    TransferToPmsModel ttp = new TransferToPmsModel();
                    switch ((AccountType)payment.AccountType)
                    {
                        case AccountType.Charge:
                            ttp = CreateTransferToPms(AccountType.Charge,mdq.ReceiptCounter.Value.ToString(),
                                posi)
                            ttp = stRepository.WriteRoomChargeToTransfer(mdq.ReceiptCounter.Value, mdq.PmsDepartmentId, mdq.PmsDepartment, mdq.PosInfoDetailId.Value,
                                                                             guest.ProfileNo.Value.ToString(), guest.FirstName + " " + guest.LastName, guest.ReservationId.ToString(),
                                                                             guest.RoomId.ToString(), guest.Room, Math.Round((Decimal)((mdq.Total / perc) * 100), 2), markRoomCharge, payment.PMSPaymentId, r.PMSInvoiceId, hotelId);
                            tr.TransferToPms.Add(ttp);
                            tpms.Add(ttp);
                            break;
                        case AccountType.Barcode:
                        case AccountType.Voucher:
                        case AccountType.CreditCard:
                        case AccountType.Cash:
                            if (accExtra != null)
                            {
                                Decimal amount = perc == (decimal)0.0 ? mdq.Total ?? 0 : Math.Round((Decimal)((mdq.Total / perc) * 100), 2);
                                ttp = stRepository.WriteCashToTransfer(mdq.ReceiptCounter.Value, mdq.PmsDepartmentId, mdq.PmsDepartment, mdq.PosInfoDetailId.Value,
                                                                       payment.AccountDescription, accExtra.PmsRoom.ToString(), amount, isFiscal, (r.ModifyOrderDetails == Symposium.Models.Enums.ModifyOrderDetailsEnum.ChangePaymentType || (isFiscal == true && Symposium.Models.Enums.ModifyOrderDetailsEnum.PayOffOnly == r.ModifyOrderDetails && r.IsPrinted)),
                                                                       payment.PMSPaymentId, r.PMSInvoiceId, IsNotSendingTransfer, hotelId);
                                tr.TransferToPms.Add(ttp);
                                tpms.Add(ttp);
                            }
                            break;
                        case AccountType.Coplimentary:
                        case AccountType.Allowence:
                            if (guest != null)
                            {
                                ttp = stRepository.WriteRoomChargeToTransfer(mdq.ReceiptCounter.Value, mdq.PmsDepartmentId, mdq.PmsDepartment, mdq.PosInfoDetailId.Value,
                                                                           guest.ProfileNo.Value.ToString(), guest.FirstName + " " + guest.LastName, guest.ReservationId.ToString(),
                                                                           guest.RoomId.ToString(), guest.Room, Math.Round((Decimal)((mdq.Total / perc) * 100), 2), markRoomCharge, payment.PMSPaymentId, r.PMSInvoiceId, hotelId);
                                tr.TransferToPms.Add(ttp);
                                tpms.Add(ttp);
                            }
                            else
                            {
                                Decimal amount = perc == (decimal)0.0 ? mdq.Total ?? 0 : Math.Round((Decimal)((mdq.Total / perc) * 100), 2);
                                try
                                {
                                    if (accExtra == null)
                                    {
                                    }

                                    ttp = stRepository.WriteCashToTransfer(mdq.ReceiptCounter.Value, mdq.PmsDepartmentId, mdq.PmsDepartment, mdq.PosInfoDetailId.Value,
                                                                           payment.AccountDescription, accExtra.PmsRoom.ToString(), amount, isFiscal,
                                                                           r.ModifyOrderDetails == ModifyOrderDetailsEnum.ChangePaymentType, payment.PMSPaymentId, r.PMSInvoiceId, IsNotSendingTransfer, hotelId);
                                    tr.TransferToPms.Add(ttp);
                                    tpms.Add(ttp);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    
                }*/
            }
            else
            {
                //TODO: Notify User
                string msg = "No department found for products in Receipt {0}" + r.ReceiptNo;
            }
            
            return tpms;
        }

        private TransferToPmsModel CreateTransferToPms(AccountType accType, string receiptCounter, long posInfoId, string posDepartment,
            string pmsDeparmentId, string pmsDepartment, long posInfoDetailId, string pmsRoom, decimal total,
            long pmsPaymentId, long pmsInvoiceId, string profileId, string profileName, string roomId, string regNo,
            bool sendToPMS, bool isFiscal, bool isImmediateTransfer, long hotelId)
        {
            TransferToPmsModel ret = new TransferToPmsModel()
            {
                Description = "Rec: " + receiptCounter + ", Pos: " + posInfoId.ToString() + ", " + posDepartment,
                PmsDepartmentId = pmsDeparmentId,
                PosInfoDetailId = posInfoDetailId,
                ReceiptNo = receiptCounter,
                RoomDescription = pmsRoom,
                PosInfoId = posInfoId,
                PmsDepartmentDescription = pmsDepartment,
                TransferType = 0, //Xreostiko
                SendToPmsTS = DateTime.Now,
                Total = total,
                TransferIdentifier = Guid.NewGuid().ToString(),
                PMSPaymentId = pmsPaymentId,
                PMSInvoiceId = pmsInvoiceId
            };
            switch (accType) {
                case AccountType.Charge:
                ret.ProfileId = profileId;
                ret.ProfileName = profileName;
                ret.RoomId = roomId;
                ret.RegNo = regNo;
                ret.SendToPMS = sendToPMS;
                ret.HotelId = hotelId;
                if (!sendToPMS)
                    ret.Status = 2;
                    break;
                case AccountType.Barcode:
                case AccountType.Voucher:
                case AccountType.CreditCard:
                case AccountType.TicketCompliment:
                case AccountType.Cash:
                    ret.ProfileName = profileName; /*Account Description*/
                    ret.RegNo = "0";
                    ret.RoomId = "";
                    ret.SendToPMS = (sendToPMS && !isFiscal) || (isImmediateTransfer && sendToPMS && isFiscal);
                    ret.Status = 0;
                    ret.HotelId = hotelId;
                    if (!isImmediateTransfer && isFiscal && sendToPMS)
                        ret.Status = 2;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Invoke plugin for card payment
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbInfo"></param>
        /// <returns>bool</returns>
        public bool OrderCardPayment(PluginCardPaymentModel model, DBInfoModel dbInfo)
        {
            PluginHelper pluginHelper = new PluginHelper();
            bool paymentCompleted = false;
            try
            {
                object ImplementedClassInstance = pluginHelper.InstanciatePlugin(typeof(CardPayment));
                if (ImplementedClassInstance == null)
                {
                    paymentCompleted = true;
                }
                else
                {
                    object[] InvokedMethodParameters = { model, dbInfo, logger };
                    PluginCardPaymentResultModel res = pluginHelper.InvokePluginMethod<PluginCardPaymentResultModel>(ImplementedClassInstance, "InvokePluginTransaction", new[] { typeof(PluginCardPaymentModel), typeof(DBInfoModel), typeof(ILog) }, InvokedMethodParameters);
                    if (res.ReturnCode == "")
                    {
                        paymentCompleted = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Error calling CardPayment plugin: " + e.ToString());
                return paymentCompleted;
            }
            return paymentCompleted;
        }

        /// <summary>
        /// Update OrderDetail set status for a specific Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool SetStatusToOrderDetails(DBInfoModel Store, long OrderId, OrderStatusEnum Status)
        {
            return ordDetTask.SetStatusToOrderDetails(Store, OrderId, Status);
        }
    }
}
