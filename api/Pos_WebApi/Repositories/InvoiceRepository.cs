using log4net;
using Microsoft.AspNet.SignalR;
using PMSConnectionLib;
using Pos_WebApi.Helpers.V3;
using Pos_WebApi.Hubs;
using Pos_WebApi.Models;
using Pos_WebApi.Models.ExtecrModels;
using Pos_WebApi.Models.FilterModels;
using Pos_WebApi.Repositories;
using Pos_WebApi.Repositories.PMSRepositories;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.ExternalSystems;
using Symposium.Models.Models.Helper;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.MealBoards;
using Symposium.WebApi.MainLogic;
using Symposium.WebApi.MainLogic.Flows;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Hotel;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;

namespace Pos_WebApi.Helpers
{
    public partial class InvoiceRepository : IoCSupported<MainLogicDIModule>
    {
        protected string _storeid;
        protected PosEntities db;
        BussinessRepository br;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        IiCouponFlows icflow;
        IHotelFlows iHotelFlows;
        IGoodysFlow goodys;
        IOrderDetailFlows orderDetailsFlows;
        IDA_OrderStatusFlows daOrderStatusFlows;
        protected Symposium.Models.Models.DBInfoModel DBInfo;

        private ITransferToPmsTasks ttpmsTask;

        public InvoiceRepository(PosEntities dbContext)
        {
            br = new BussinessRepository(dbContext);
            db = dbContext;
            HttpRequestUtil httpUtil = new HttpRequestUtil();
            DBInfo = httpUtil.GetStore();

            var config = System.Web.Http.GlobalConfiguration.Configuration;
            System.Web.Http.Dependencies.IDependencyResolver autofac;
            autofac = config.DependencyResolver;
            ttpmsTask = (ITransferToPmsTasks)autofac.GetService(typeof(ITransferToPmsTasks));
        }

        ~InvoiceRepository()
        {
            br = null;
        }


        public Receipts ConvertTmpReceiptBOToReceiptForCancelled(TempReceiptBOFull rec)
        {
            Receipts newRec = new Receipts();
            newRec.Id = 0;
            newRec.Day = rec.Day;
            newRec.Abbreviation = rec.Abbreviation;
            newRec.InvoiceDescription = rec.InvoiceDescription;
            newRec.ReceiptNo = rec.ReceiptNo;
            newRec.InvoiceTypeId = rec.InvoiceTypeId;
            //newRec.EndOfDayId = rec.EndOfDayId;
            //newRec.FODay = rec.FODay;
            newRec.InvoiceTypeType = rec.InvoiceTypeType;
            newRec.PosInfoId = rec.PosInfoId;
            newRec.PosInfoDetailId = rec.PosInfoDetailId;
            //newRec.ClientPosId = rec.ClientPosId;
            //newRec.PdaModuleId = rec.PdaModuleId;
            //newRec.PosInfoDescription = rec.PosInfoDescription;
            newRec.DepartmentId = rec.DepartmentId;
            newRec.DepartmentDescription = rec.DepartmentDescription;
            newRec.Cover = rec.Cover;
            newRec.InvoiceIndex = rec.InvoiceIndex;
            //newRec.TableId = rec.TableId;
            //newRec.TableCode = rec.TableCode;
            newRec.Total = rec.Total;
            newRec.Discount = rec.Discount;
            //newRec.DiscountRemark = rec.DiscountRemark;
            newRec.Net = rec.Net;
            newRec.Vat = rec.Vat;
            newRec.StaffId = rec.StaffId;
            //newRec.StaffCode = rec.StaffCode;
            //newRec.StaffName = rec.StaffName;
            newRec.IsVoided = rec.IsVoided;
            newRec.IsPrinted = rec.IsPrinted;
            //newRec.BillingAddress = rec.BillingAddress;
            //newRec.BillingAddressId = rec.BillingAddressId;
            //newRec.BillingCity = rec.BillingCity;
            //newRec.BillingZipCode = rec.BillingZipCode;
            //newRec.BillingName = rec.BillingName;
            //newRec.BillingVatNo = rec.BillingVatNo;
            //newRec.BillingDOY = rec.BillingDOY;
            //newRec.BillingJob = rec.BillingJob;
            //newRec.CustomerName = rec.CustomerName;
            //newRec.CustomerID = rec.CustomerID;
            //newRec.CustomerRemarks = rec.CustomerRemarks;
            //newRec.Floor = rec.Floor;
            //newRec.Latitude = rec.Latitude;
            //newRec.Longtitude = rec.Longtitude;
            //newRec.Phone = rec.Phone;
            //newRec.ShippingAddress = rec.ShippingAddress;
            //newRec.ShippingCity = rec.ShippingCity;
            //newRec.ShippingAddressId = rec.ShippingAddressId;
            //newRec.ShippingZipCode = rec.ShippingZipCode;
            newRec.OrderNo = rec.OrderNo;
            //newRec.Room = rec.Room;
            //newRec.PaymentsDesc = rec.PaymentsDesc;
            //newRec.IsPaid = rec.IsPaid;
            //newRec.PaidTotal = rec.PaidTotal;
            newRec.StaffLastName = rec.StaffLastName;
            newRec.CreateTransactions = false;
            newRec.ModifyOrderDetails = Symposium.Models.Enums.ModifyOrderDetailsEnum.FromOtherUnmodified;
            newRec.Points = 0;
            //newRec.LoyaltyDiscount = 0;
            //newRec.DigitalSignatureImage = null;
            //newRec.TableSum = rec.TableSum;
            //newRec.CashAmount = rec.CashAmount;
            //newRec.BuzzerNumber = rec.BuzzerNumber;
            //newRec.OrderOrigin = null;
            newRec.PrintType = PrintTypeEnum.CancelCurrentReceipt;
            newRec.ItemAdditionalInfo = null;
            newRec.TempPrint = false;
            //newRec.IsAssignedForDelivery = false;
            //newRec.AssignedToDeliverStaffId = null;
            //newRec.AssignedToDeliverStatus = null;

            foreach (TempReceiptDetails item in rec.ReceiptDetails)
            {
                ReceiptDetails recDet = new ReceiptDetails();
                //recDet.ReceiptsId = item.ReceiptsId;
                //recDet.EndOfDayId = item.EndOfDayId;
                recDet.PosInfoId = item.PosInfoId;
                recDet.StaffId = item.StaffId;
                recDet.Abbreviation = "ΕΑΣ";
                recDet.InvoiceType = 3;
                recDet.OrderDetailId = item.OrderDetailId;
                recDet.PosInfoDetailId = item.PosInfoDetailId;
                recDet.ItemCode = item.ItemCode;
                recDet.ItemDescr = item.ItemDescr;
                recDet.Price = item.Price;
                recDet.ItemQty = -item.ItemQty;
                recDet.ItemGross = item.ItemGross;
                recDet.ItemDiscount = item.ItemDiscount;
                recDet.ItemVatRate = item.ItemVatRate;
                recDet.ItemVatValue = item.ItemVatValue;
                //recDet.TaxId = item.TaxId;
                //recDet.ItemTaxAmount = item.ItemTaxAmount;
                recDet.ItemNet = item.ItemNet;
                recDet.VatCode = item.VatCode;
                recDet.Status = item.Status;
                recDet.PaidStatus = item.PaidStatus;
                //recDet.KitchenId = item.KitchenId;
                //recDet.PreparationTime = item.PreparationTime;
                //recDet.KdsId = item.KdsId;
                //recDet.Guid = item.Guid;
                //recDet.TableCode = item.TableCode;
                //recDet.TableId = item.TableId;
                //recDet.RegionId = item.RegionId;
                recDet.OrderNo = item.OrderNo;
                recDet.OrderId = item.OrderId;
                //recDet.PriceListDetailId = item.PriceListDetailId;
                recDet.PricelistId = item.PricelistId;
                recDet.ProductId = item.ProductId;
                recDet.VatId = item.VatId;
                recDet.IsExtra = 0;
                //recDet.ItemRemark = item.ItemRemark;
                //recDet.OrderDetailIgredientsId = item.OrderDetailIgredientsId;
                recDet.IsInvoiced = item.IsInvoiced;
                recDet.SalesTypeId = item.SalesTypeId;
                recDet.ProductCategoryId = item.ProductCategoryId;
                //recDet.CategoryId = null;
                recDet.ItemPosition = item.ItemPosition;
                recDet.ItemSort = item.ItemSort;
                //recDet.ItemRegion = item.ItemRegion;
                //recDet.RegionPosition = item.RegionPosition;
                //recDet.ItemBarcode = 0;
                recDet.TotalBeforeDiscount = item.TotalBeforeDiscount;
                //recDet.ReceiptSplitedDiscount = item.ReceiptSplitedDiscount;
                //recDet.Receipts = null;
                newRec.ReceiptDetails.Add(recDet);
            }

            foreach (TempReceiptPaymentsFlat item in rec.ReceiptPayments)
            {
                ReceiptPayments recPaym = new ReceiptPayments();
                //recPaym.ReceiptsId = item.Id;
                //recPaym.EndOfDayId = item.EndOfDayId;
                //recPaym.Abbreviation = null;
                //recPaym.ReceiptNo = null;
                //recPaym.InvoiceType = item.InvoiceType;
                recPaym.PosInfoId = item.PosInfoId;
                recPaym.AccountId = item.AccountId;
                recPaym.AccountDescription = item.AccountDescription;
                recPaym.AccountType = item.AccountType;
                //recPaym.AccountEODRoom = item.AccountEODRoom;
                recPaym.SendsTransfer = item.SendsTransfer;
                recPaym.Amount = item.Amount;
                //recPaym.TransactionType = item.TransactionType;
                //recPaym.Room = item.Room;
                //recPaym.RoomId = item.RoomId;
                //recPaym.GuestId = item.GuestId;
                //recPaym.ProfileNo = item.ProfileNo;
                //recPaym.Guest = null;
                //recPaym.ReservationCode = item.ReservationCode;
                //recPaym.CreditAccountId = item.CreditAccountId;
                //recPaym.CreditCodeId = item.CreditCodeId;
                //recPaym.CreditAccountDescription = item.CreditAccountDescription;
                recPaym.CreditTransactionAction = 0;
                //recPaym.HotelId = item.HotelId;
                newRec.ReceiptPayments.Add(recPaym);
            }


            return newRec;
        }

        /// <summary>
        /// InvoiceForDisplayController Controler POST new Receipts function
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="receipts"></param>
        /// <returns></returns>
        public HttpResponseMessage MainPostReceiptsAction(string storeid, IEnumerable<Receipts> receipts)
        {
            bool sendKdsMessage = false; bool externalCommunication; string extecr = "";
            //Loyalty - hotel
            var hi = db.HotelInfo.FirstOrDefault();
            HotelInfo(hi);

            foreach (var receipt in receipts)
            {
                try
                {
                    bool t = CheckConnectedExtecr(receipt, out extecr);
                    if (!t)
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent(string.Format(Symposium.Resources.Errors.EXTECRNOTFOUND, extecr ?? "<null>"))
                        };
                    }
                    externalCommunication = true;
                    //check If there is selected customer for Invoice and all Invoice's Required fields have value.
                    if (receipt.InvoiceTypeType == 3 || receipt.InvoiceTypeType == 8)
                    {
                        string customerClassType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
                        if (customerClassType == "Pos_WebApi.Customer_Modules.Goodies")
                        {
                            goodys = Resolve<IGoodysFlow>();
                            long invoiceId = goodys.GetInvoiceid(DBInfo, Convert.ToInt64(receipt.OrderNo));
                           
                            goodys.UpdateGoodysApi(invoiceId, DBInfo);
                        }
                           
                    }
                        if (receipt.InvoiceTypeType == 7)
                    {
                        if (receipt.CustomerID == 0 || receipt.CustomerID == null)
                        {
                            if ((ModifyOrderDetailsEnum)receipt.ModifyOrderDetails != ModifyOrderDetailsEnum.PayOffOnly)
                            {

                                logger.Info("There is no Selected Customer");
                                throw new Exception("There is No Selected Customer!");
                            }
                        }
                        else if (string.IsNullOrEmpty(receipt.BillingName) || string.IsNullOrEmpty(receipt.BillingVatNo) || string.IsNullOrEmpty(receipt.BillingDOY) || string.IsNullOrEmpty(receipt.BillingJob))
                        {
                            if ((ModifyOrderDetailsEnum)receipt.ModifyOrderDetails != ModifyOrderDetailsEnum.PayOffOnly)
                            {

                                logger.Info("Some Invoice Fields are Empty");
                                throw new Exception("Some Invoice Fields are Empty!");

                            }
                        }
                    }

                    //ICoupon functionallity when oreder is type of icoupon on exttype and coupon is not empty
                    if (receipt.ExtType == (int)ExternalSystemOrderEnum.ICoupon)
                    {
                        if (receipt.ICouponUsed == null)
                        {
                            logger.Info("No coupon on Invoice");
                            throw new Exception("No coupon entity on Invoice with ExtType ICoupon");
                        }
                        icflow = Resolve<IiCouponFlows>();//IoC;
                        List<ICoupon> tempIcoupon = new List<ICoupon>();

                        string _locationRef, _serviceProviderRef, _tillRef, _tradingOutletRef, _tradingOutletName;
                        foreach (ICoupon temp in receipt.ICouponUsed)
                        {
                            _tillRef = MainConfigurationHelper.GetSubConfiguration("api", "tillRef");
                            _locationRef = MainConfigurationHelper.GetSubConfiguration("api", "locationRef");
                            _serviceProviderRef = MainConfigurationHelper.GetSubConfiguration("api", "serviceProviderRef");
                            _tradingOutletRef = MainConfigurationHelper.GetSubConfiguration("api", "tradingOutletRef");
                            _tradingOutletName = MainConfigurationHelper.GetSubConfiguration("api", "tradingOutletName");
                            tempIcoupon.Add(icflow.UpdateCoupon(temp, "redeem", _tillRef, _locationRef, _serviceProviderRef, _tradingOutletRef, _tradingOutletName));
                        }
                        receipt.ICouponUsed = tempIcoupon;

                        //Initialize InsertDataRequestModel according to receipt
                        decimal totalToPay = 0;
                        totalToPay = (decimal)receipt.Total + (decimal)receipt.Discount;
                        foreach (ICoupon coupon in receipt.ICouponUsed)
                        {
                            IcouponInsertReceiptsDataModel insertRec = new IcouponInsertReceiptsDataModel();
                            insertRec.receiptLines = new List<ReceiptLines>();
                            insertRec.glLines = new List<GlLines>();
                            insertRec.serviceProviderRef = MainConfigurationHelper.GetSubConfiguration("api", "serviceProviderRef");
                            insertRec.tradingOutletRef = MainConfigurationHelper.GetSubConfiguration("api", "tradingOutletRef");
                            insertRec.tradingOutletName = MainConfigurationHelper.GetSubConfiguration("api", "tradingOutletName");
                            insertRec.couponRef = coupon.couponRef;
                            insertRec.receiptNumber = receipt.OrderNo;
                            insertRec.receiptDate = DateTime.Now;
                            if (receipt.ICouponUsed.Count == 1)
                            {
                                if (coupon.value <= totalToPay)
                                {
                                    insertRec.redeemedValue = coupon.value;
                                }
                                else
                                {
                                    insertRec.redeemedValue = totalToPay;
                                }
                            }
                            else
                            {
                                decimal totalCoupons = 0;
                                receipt.ICouponUsed.ToList().ForEach(q => { totalCoupons += q.value; });
                                if (totalCoupons <= receipt.Total)
                                {
                                    insertRec.redeemedValue = coupon.value;
                                }
                                else
                                {
                                    if (coupon.value <= totalToPay)
                                    {
                                        insertRec.redeemedValue = coupon.value;
                                    }
                                    else
                                    {
                                        insertRec.redeemedValue = totalToPay;
                                    }
                                }
                            }
                            // ReceiptLines
                            if (coupon.type == 1)
                            {
                                // type 1
                                foreach (ReceiptDetails rec in receipt.ReceiptDetails)
                                {
                                    ReceiptLines recLines = new ReceiptLines();
                                    if (!string.IsNullOrEmpty(rec.ItemCode))
                                    {
                                        recLines.type = 1; // type
                                        recLines.text = rec.ItemDescr; //item Description
                                        int qty = (int)rec.ItemQty; // Item Quantity
                                        recLines.quantity = qty;
                                        recLines.value = (decimal)rec.Price; // Item Value
                                        recLines.isCoupon = false; // has Coupon
                                        insertRec.receiptLines.Add(recLines);
                                    }
                                }

                                // type 2
                                if (receipt.Discount > 0)
                                {
                                    if (receipt.ICouponUsed.Count > 1)
                                    {
                                        decimal restTotalToPay = 0;
                                        restTotalToPay = totalToPay;
                                        foreach (ICoupon item in receipt.ICouponUsed)
                                        {
                                            if (totalToPay >= coupon.value)
                                            {
                                                ReceiptLines recLines = new ReceiptLines();
                                                recLines.type = 2; // type
                                                recLines.text = "iCoupon DISC"; //item Description
                                                int qty = 1; // Item Quantity
                                                recLines.quantity = qty;
                                                if (insertRec.couponRef == item.couponRef)
                                                {
                                                    recLines.isCoupon = true; // has Coupon
                                                    recLines.value = -coupon.value; // Item Value
                                                    restTotalToPay = restTotalToPay - coupon.value;
                                                }
                                                else
                                                {
                                                    recLines.isCoupon = false;
                                                    if (coupon.value >= restTotalToPay)
                                                    {
                                                        recLines.value = -restTotalToPay; // Item Value
                                                    }
                                                    else
                                                    {
                                                        recLines.value = -coupon.value;
                                                        restTotalToPay = restTotalToPay - coupon.value;
                                                    }
                                                }
                                                insertRec.receiptLines.Add(recLines);
                                            }
                                            else
                                            {
                                                ReceiptLines recLines = new ReceiptLines();
                                                recLines.type = 2; // type
                                                recLines.text = "iCoupon DISC"; //item Description
                                                int qty = 1; // Item Quantity
                                                recLines.quantity = qty;
                                                if (insertRec.couponRef == item.couponRef)
                                                {
                                                    recLines.isCoupon = true; // has Coupon
                                                    recLines.value = -totalToPay; // Item Value
                                                }
                                                else
                                                {
                                                    recLines.isCoupon = false;
                                                    recLines.value = -coupon.value; // Item Value
                                                }
                                                insertRec.receiptLines.Add(recLines);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (totalToPay >= coupon.value)
                                        {
                                            ReceiptLines recLines = new ReceiptLines();
                                            recLines.type = 2; // type
                                            recLines.text = "iCoupon DISC"; //item Description
                                            int qty = 1; // Item Quantity
                                            recLines.quantity = qty;
                                            recLines.value = -coupon.value; // Item Value
                                            recLines.isCoupon = true; // has Coupon
                                            insertRec.receiptLines.Add(recLines);
                                        }
                                        else
                                        {
                                            ReceiptLines recLines = new ReceiptLines();
                                            recLines.type = 2; // type
                                            recLines.text = "iCoupon DISC"; //item Description
                                            int qty = 1; // Item Quantity
                                            recLines.quantity = qty;
                                            recLines.value = -totalToPay; // Item Value
                                            recLines.isCoupon = true; // has Coupon
                                            insertRec.receiptLines.Add(recLines);
                                        }
                                    }

                                }

                                // type 4
                                foreach (ReceiptPayments recPay in receipt.ReceiptPayments)
                                {
                                    ReceiptLines recLines = new ReceiptLines();
                                    if (recPay.AccountId >= 0 && recPay.Amount > 0)
                                    {
                                        recLines.type = 4; // type
                                        recLines.text = recPay.AccountDescription; //item Description
                                        int qty = 1; // Item Quantity
                                        recLines.quantity = qty;
                                        recLines.value = (decimal)recPay.Amount; // Item Value
                                        recLines.isCoupon = false; // has Coupon
                                        insertRec.receiptLines.Add(recLines);
                                    }
                                }
                            }
                            else if (coupon.type == 2)
                            {
                                // type 1
                                foreach (ReceiptDetails rec in receipt.ReceiptDetails)
                                {
                                    ReceiptLines recLines0 = new ReceiptLines();
                                    if (!string.IsNullOrEmpty(rec.ItemCode))
                                    {
                                        recLines0.type = 1; // type
                                        recLines0.text = rec.ItemDescr; //item Description
                                        int qty0 = (int)rec.ItemQty; // Item Quantity
                                        recLines0.quantity = qty0;
                                        recLines0.value = (decimal)rec.ItemGross; // Item Value
                                        recLines0.isCoupon = false; // has Coupon
                                        insertRec.receiptLines.Add(recLines0);
                                    }
                                }

                                // type 4
                                foreach (ReceiptPayments recPay in receipt.ReceiptPayments)
                                {
                                    ReceiptLines recLines1 = new ReceiptLines();
                                    if (recPay.AccountId >= 0 && recPay.AccountType != 10)
                                    {
                                        if (recPay.AccountType != 6)
                                        {
                                            if (totalToPay > coupon.value)
                                            {
                                                recLines1.type = 4; // type
                                                recLines1.text = recPay.AccountDescription; //item Description
                                                int qty1 = 1; // Item Quantity
                                                recLines1.quantity = qty1;
                                                recLines1.value = (decimal)recPay.Amount; // Item Value
                                                recLines1.isCoupon = false; // has Coupon
                                                insertRec.receiptLines.Add(recLines1);
                                            }
                                        }
                                        else
                                        {
                                            if (receipt.ICouponUsed.Count > 1)
                                            {
                                                decimal restTotalToPay = 0;
                                                restTotalToPay = (decimal)receipt.Total;
                                                foreach (ICoupon item in receipt.ICouponUsed)
                                                {
                                                    if (totalToPay >= coupon.value)
                                                    {
                                                        ReceiptLines recLines = new ReceiptLines();
                                                        recLines.type = 4; // type
                                                        recLines.text = "iCOUPON"; //item Description
                                                        int qty = 1; // Item Quantity
                                                        recLines.quantity = qty;
                                                        if (insertRec.couponRef == item.couponRef)
                                                        {
                                                            recLines.isCoupon = true; // has Coupon
                                                            recLines.value = coupon.value; // Item Value
                                                            restTotalToPay = restTotalToPay - coupon.value;
                                                        }
                                                        else
                                                        {
                                                            recLines.isCoupon = false;
                                                            if (coupon.value >= restTotalToPay)
                                                            {
                                                                recLines.value = restTotalToPay; // Item Value
                                                            }
                                                            else
                                                            {
                                                                recLines.value = coupon.value;
                                                                restTotalToPay = restTotalToPay - coupon.value;
                                                            }
                                                        }
                                                        insertRec.receiptLines.Add(recLines);
                                                    }
                                                    else
                                                    {
                                                        ReceiptLines recLines = new ReceiptLines();
                                                        recLines.type = 4; // type
                                                        recLines.text = "iCOUPON"; //item Description
                                                        int qty = 1; // Item Quantity
                                                        recLines.quantity = qty;
                                                        if (insertRec.couponRef == item.couponRef)
                                                        {
                                                            recLines.isCoupon = true; // has Coupon
                                                            recLines.value = totalToPay; // Item Value
                                                        }
                                                        else
                                                        {
                                                            recLines.isCoupon = false;
                                                            recLines.value = coupon.value; // Item Value
                                                        }
                                                        insertRec.receiptLines.Add(recLines);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (receipt.Total >= coupon.value)
                                                {
                                                    ReceiptLines recLines = new ReceiptLines();
                                                    recLines.type = 4; // type
                                                    recLines.text = "iCOUPON"; //item Description
                                                    int qty = 1; // Item Quantity
                                                    recLines.quantity = qty;
                                                    recLines.value = coupon.value; // Item Value
                                                    recLines.isCoupon = true; // has Coupon
                                                    insertRec.receiptLines.Add(recLines);
                                                }
                                                else
                                                {
                                                    ReceiptLines recLines = new ReceiptLines();
                                                    recLines.type = 4; // type
                                                    recLines.text = "iCOUPON"; //item Description
                                                    int qty = 1; // Item Quantity
                                                    recLines.quantity = qty;
                                                    recLines.value = (decimal)receipt.Total; // Item Value
                                                    recLines.isCoupon = true; // has Coupon
                                                    insertRec.receiptLines.Add(recLines);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // glLines
                            if (coupon.type == 1)
                            {
                                int counter = 0;
                                foreach (ReceiptDetails rec1 in receipt.ReceiptDetails)
                                {
                                    foreach (ICoupon item in receipt.ICouponUsed)
                                    {
                                        if (insertRec.couponRef == item.couponRef)
                                        {
                                            decimal valueAfterDiscount = 0;
                                            decimal discountRate = 0;
                                            decimal newCouponValue = 0;
                                            decimal checkValue = 0;
                                            decimal roundingValue = 0;

                                            if (totalToPay >= coupon.value)
                                            {
                                                newCouponValue = coupon.value;
                                            }
                                            else
                                            {
                                                newCouponValue = totalToPay;
                                            }
                                            discountRate = newCouponValue / (decimal)(receipt.Total + receipt.Discount);
                                            valueAfterDiscount = Math.Round((decimal)rec1.Price * discountRate, 2);
                                            receipt.ReceiptDetails.ToList().ForEach(q => { checkValue += Math.Round((decimal)q.Price * discountRate, 2); });
                                            roundingValue = checkValue - newCouponValue;
                                            GlLines gLines = new GlLines();
                                            if (!string.IsNullOrEmpty(rec1.ItemCode))
                                            {
                                                counter++;
                                                gLines.gl = rec1.ItemCode;
                                                gLines.text = rec1.ItemDescr;
                                                if (receipt.ReceiptDetails.Count == counter)
                                                {
                                                    gLines.value = valueAfterDiscount - roundingValue;
                                                }
                                                else
                                                {
                                                    gLines.value = valueAfterDiscount;
                                                }
                                                gLines.taxValue = Math.Round(valueAfterDiscount * (decimal)(rec1.ItemVatRate * (decimal)0.01), 6);
                                                gLines.taxRate = Math.Round((decimal)rec1.ItemVatRate, 2);
                                                insertRec.glLines.Add(gLines);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (coupon.type == 2)
                            {
                                GlLines gLines1 = new GlLines();
                                gLines1.gl = "TENDER";
                                gLines1.text = "iCOUPON";
                                if (totalToPay >= coupon.value)
                                {
                                    gLines1.value = coupon.value;
                                }
                                else
                                {
                                    gLines1.value = totalToPay;
                                }
                                gLines1.taxValue = 0;
                                gLines1.taxRate = 0;
                                insertRec.glLines.Add(gLines1);
                            }

                            totalToPay = totalToPay - coupon.value;

                            //InsertDataRequest
                            icflow.InsertSalesData(insertRec);
                        }
                    }

                    var res = InsertInvoice(receipt, out externalCommunication);
                    if (receipt.DigitalSignatureImage != null)
                    {
                        // Get DigitalSignatureImage and stored as an Array of bytes 
                        DigitalSignature digSignature = new DigitalSignature
                        {
                            InvoiceId = res.InvoiceId,
                            Images = Convert.FromBase64String(receipt.DigitalSignatureImage)
                        };
                        db.DigitalSignature.Add(digSignature);
                        db.SaveChanges();
                    }
                    //Loyalty insert Function
                    InsertLoyaltyActions(receipt, hi);
                    if (!externalCommunication)
                    {
                        InsertNotPosted(receipt, "Hashcode exists!");
                        continue;
                    }
                    if (receipt.ModifyOrderDetails != Symposium.Models.Enums.ModifyOrderDetailsEnum.PayOffOnly)
                    {
                        var getpid = db.PosInfoDetail.Find(receipt.PosInfoDetailId);
                        var receiptSendsVoidToKitchen = true;
                        if (getpid != null)
                            receiptSendsVoidToKitchen = (getpid.SendsVoidToKitchen ?? 0) > 0;

                        var sentKitchen = (receipt.ModifyOrderDetails == (Symposium.Models.Enums.ModifyOrderDetailsEnum.FromOtherUnmodified) && receipt.InvoiceTypeType == 3 && receiptSendsVoidToKitchen)
                                        || (receipt.ModifyOrderDetails == Symposium.Models.Enums.ModifyOrderDetailsEnum.FromScratch)
                                        || (receipt.ModifyOrderDetails == Symposium.Models.Enums.ModifyOrderDetailsEnum.FromOtherUnmodified && receipt.InvoiceTypeType == 8 && receiptSendsVoidToKitchen);
                        if (receipt.ExtType == null || receipt.ExtType == (int)ExternalSystemOrderEnum.Vivardia || receipt.ExtType == (int)ExternalSystemOrderEnum.ICoupon)
                        {
                            hub.Clients.Group(storeid).newReceipt(storeid + "|" + res.ExtecrName, res.InvoiceId, true, sentKitchen, receipt.PrintType, receipt.ItemAdditionalInfo, receipt.TempPrint);
                            string cType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
                            if (cType == "Pos_WebApi.Customer_Modules.Goodies")
                            {
                                hub.Clients.All.NewInvoice(receipt.PosInfoId, res.InvoiceId);
                                hub.Clients.All.OpenOrders("Updated New Orders through Invoice For Display Flow");
                            }



                                // Code handled onyl by pda
                                if (receipt.PdaModuleId != null)
                            {
                                PdaModule pdaClient = db.PdaModule.Where(p => p.Id == receipt.PdaModuleId).FirstOrDefault();
                                if (pdaClient != null)
                                {
                                    string pdaClientName = storeid + "|PDA-" + pdaClient.Code;
                                    string pdaClientId = WebPosHub._connections.GetConnections(pdaClientName);
                                    if (pdaClientId != null)
                                    {
                                        hub.Clients.Client(pdaClientId).receiptId(res.InvoiceId);
                                    }
                                }
                            }
                        }
                        else if (receipt.ExtType == (int)ExternalSystemOrderEnum.VivardiaNoKitchen || receipt.ExtType == (int)ExternalSystemOrderEnum.Forkey)
                        {
                            hub.Clients.Group(storeid).refreshTable(storeid, null);
                        }

                        if (receipt.TableId != null) { hub.Clients.Group(storeid).refreshTable(storeid, receipt.TableId); }
                        sendKdsMessage = true;
                    }
                    else hub.Clients.Group(storeid).refreshTable(storeid, receipt.TableId);
                    if (
                        receipt.ModifyOrderDetails == (int)ModifyOrderDetailsEnum.FromScratch &&
                        (receipt.ExtType == (int)ExternalSystemOrderEnum.Vivardia || receipt.ExtType == (int)ExternalSystemOrderEnum.VivardiaNoKitchen || receipt.ExtType == (int)ExternalSystemOrderEnum.Forkey)
                    )
                    {

                        try
                        {
                            List<long?> del_sel_SalesTypes = receipt.ReceiptDetails.Select(q => q.SalesTypeId).Distinct().ToList();// db.OrderDetail.Where(od => od.OrderId == o.Id).;
                            hub.Clients.Group(storeid).deliveryMessage(storeid, -1, del_sel_SalesTypes);
                        }
                        catch (Exception ex) { logger.Error(ex.ToString()); }
                    }
                    // try patch forkey order
                    if (receipt.InvoiceTypeType == 3 || receipt.InvoiceTypeType == 8)
                    {
                        string strod = receipt.OrderNo.Trim();
                        Order fo = db.Order.Where(q => q.EndOfDayId == null && q.ExtType == (int)ExternalSystemOrderEnum.Forkey && q.OrderNo.ToString().Equals(strod)).FirstOrDefault();
                        //if res == -1 order not found escape goew to task lvl
                        if (fo != null) { var patchres = PatchOrder(fo.ExtKey, DeliveryForkeyStatusEnum.cancelled); }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    InsertNotPosted(receipt, message);
                    throw;
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.ExpectationFailed };
                }
            }
            if (sendKdsMessage)
            {
                var receiptId = 0L;
                hub.Clients.Group(storeid).kdsMessage(storeid, receiptId);
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
        }
        public long PatchOrder(string forkey_orderId, DeliveryForkeyStatusEnum forkey_status)
        {
            try
            {
                ////https://devel.forky.gr/api/backend/orders/
                string pathtopatchRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "forkeyPatch");
                string pathtopatch = pathtopatchRaw.Trim();
                string forkeyAuthRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "forkeyAuth");
                string forkeyAuth = forkeyAuthRaw.Trim();
                pathtopatch += forkey_orderId;
                int returnCode = 0; string ErrorMess = "-"; dynamic MyDynamic = new System.Dynamic.ExpandoObject();
                MyDynamic.status = forkey_status.ToString(); WebApiClientHelper wapich = new WebApiClientHelper();
                string result = wapich.PatchRequest<dynamic>(MyDynamic, pathtopatch, forkeyAuth, null, out returnCode, out ErrorMess, "application/json", "OAuth2");
            }
            catch (Exception ex) { return -1; }
            return 0;
        }


        /// <summary>
        /// Paged Receipts by PosinfoId on PArameter
        /// Model returned 
        /// </summary>
        /// <param name="posInfoId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="eodId"></param>
        /// <returns></returns>
        public PagedResult<TempReceiptBOFull> GetPagedReceiptsByPos(long posInfoId, int page = 0, int pageSize = 10, long? eodId = 0)
        {
            var query = br.ReceiptsBO(x => x.EndOfDayId == 0 && x.PosInfoId == posInfoId).OrderByDescending(o => o.Id);
            var extras = query.GroupBy(g => g.InvoiceTypeType).Select(s => new
            {
                InvoiceType = s.Key,
                Abbr = s.FirstOrDefault().Abbreviation,
                Amount = s.Sum(sm => sm.Total),
                Count = s.Count()
            }).ToList();

            var result = new PagedResult<TempReceiptBOFull>
            {
                Extras = extras,
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// Paged result of Receipts on predecate and filters 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filts"></param>
        /// <returns></returns>
        public PagedResult<TempReceiptBOFull> GetPagedReceiptsByPos(Expression<Func<TempReceiptBOFull, bool>> predicate, int page = 0, int pageSize = 10, ReceiptFilters filts = null)
        {
            var query = br.ReceiptsBO(predicate, filts).OrderByDescending(o => o.Id);
            var extras = query.GroupBy(g => g.InvoiceTypeType).Select(s => new { InvoiceType = s.Key, Abbr = s.FirstOrDefault().Abbreviation, Amount = s.Sum(sm => sm.Total), Count = s.Count() }).ToList();
            var result = new PagedResult<TempReceiptBOFull>
            {
                Extras = extras,
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            result.Results = query.Skip(skip).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// Selected Invoices and entities to Return Receipt as a callback
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dynamic GetReceiptDetailsById(long id)
        {
            var receipt = br.ReceiptsBO(x => x.Id == id).FirstOrDefault();
            var details = br.ReceiptDetailsBO(x => x.ReceiptsId == id).ToList();
            var payments = br.ReceiptPaymentsFlat(x => x.InvoicesId == id).ToList();
            var externalInfo = from q in db.Invoices.Where(w => w.Id == id)
                               join odi in db.OrderDetailInvoices on q.Id equals odi.InvoicesId
                               join od in db.OrderDetail on odi.OrderDetailId equals od.Id
                               join o in db.Order on od.OrderId equals o.Id
                               select new
                               {
                                   ExtKey = o.ExtKey,
                                   ExtType = o.ExtType,
                                   ExtObj = o.ExtObj
                               };
            var related = br.ReceiptRelatedReceipts(details).ToList();
            foreach (var p in payments)
            {
                if (p.AccountType == (short)AccountType.Barcode)
                {
                    var data = from t in db.Transactions.Where(w => w.Id == p.Id)
                               join ct in db.CreditTransactions on t.Id equals ct.TransactionId
                               join ca in db.CreditAccounts on ct.CreditAccountId equals ca.Id
                               join cc in db.CreditCodes on ct.CreditCodeId equals cc.Id
                               select new
                               {
                                   CreditAccountId = ca.Id,
                                   CreditCodeId = cc.Id,
                                   CreditAccountDescription = ca.Description
                               };
                    var creditInfo = data.FirstOrDefault();
                    p.CreditAccountId = creditInfo.CreditAccountId;
                    p.CreditCodeId = creditInfo.CreditCodeId;
                    p.CreditAccountDescription = creditInfo.CreditAccountDescription;
                }
            }
            receipt.ReceiptDetails = details;
            receipt.ReceiptPayments = payments;

            //  var a = br.RelatedReceiptsBO(x => x.Id == id, xx => true);

            //            var a = db.fnGetRelatedReceipts(id).ToList();
            //var query = db.Receipts.Include("ReceiptDetails")
            //                              .Include("ReceiptPayments")
            //                              .Where(w => w.Id == id);
            return new { Receipt = receipt, RelatedReceipts = related, ExternalInfo = externalInfo.Distinct().ToList() };
        }


        /// <summary>
        /// Return receipt retails (items included)
        /// </summary>
        /// <param name="id">receipt id</param>
        /// <returns>EXTECR_ReceiptModel</returns>
        public dynamic GetReceiptDetailsForExtecr(long id)
        {

            var receipt = br.ReceiptsBO(x => x.Id == id).FirstOrDefault();
            var details = br.ReceiptDetailsBO(x => x.ReceiptsId == id).ToList();
            var payments = br.ReceiptPaymentsFlat(x => x.InvoicesId == id).ToList();
            var receiptDescription = db.PosInfoDetail.Where(w => w.Id == receipt.PosInfoDetailId).FirstOrDefault();
            var departmentDescr = db.Department.FirstOrDefault(x => x.Id == receipt.DepartmentId);
            var pdaDescr = db.PdaModule.FirstOrDefault(x => x.Id == receipt.PdaModuleId);
            var related = br.ReceiptRelatedReceipts(details).ToList();

            EXTECR_ReceiptModel res = new EXTECR_ReceiptModel
            {
                PosDescr = receipt.PosInfoDescription,
                Pos = receipt.PosInfoId.ToString(),
                Total = receipt.Total != null ? receipt.Total.Value : 0,
                TotalDiscount = receipt.Discount > 0 ? receipt.Discount - (details.Sum(sm => sm.ItemDiscount) ?? 0) : 0,
                Waiter = receipt.StaffFullName,
                WaiterNo = receipt.StaffCode,
                TableNo = receipt.TableCode,
                ReceiptNo = receipt.ReceiptNo.ToString(),
                OrderNo = receipt.OrderNo,
                OrderId = receipt.OrderNo,

                CustomerAfm = "",
                CustomerAddress = receipt.BillingAddress,
                CustomerComments = receipt.CustomerRemarks,
                CustomerDeliveryAddress = receipt.ShippingAddress,
                CustomerDoy = "",
                CustomerJob = "",
                CustomerPhone = receipt.Phone,

                BillingName = receipt.BillingName,
                BillingJob = receipt.BillingJob,
                BillingVatNo = receipt.BillingVatNo,
                BillingDOY = receipt.BillingDOY,
                BillingAddress = receipt.BillingAddress,
                BillingCity = receipt.BillingCity,
                BillingZipCode = receipt.BillingZipCode,

                Department = receipt.DepartmentId.ToString(),
                DepartmentDesc = receipt.DepartmentDescription,
                Couver = receipt.Cover,
                City = receipt.BillingCity,
                Floor = receipt.Floor,
                DepartmentTypeDescription = departmentDescr != null ? departmentDescr.Description : "",
                ReceiptTypeDescription = receiptDescription != null ? receiptDescription.Description : details.FirstOrDefault().Abbreviation,
                IsVoid = receipt.IsVoided,
                InvoiceIndex = receipt.InvoiceIndex.ToString(),
                InvoiceType = receipt.InvoiceTypeType != null ? receipt.InvoiceTypeType.Value : (short)0,
                // !!!!! this dublicate had the same cast and table sum was later
                //TableTotal = receipt.Total, 
                TableTotal = receipt.TableSum,
                CashAmount = receipt.CashAmount,
                BuzzerNumber = receipt.BuzzerNumber,
                SalesTypeDescription = details.FirstOrDefault().SalesTypeDescription,
                PdaId = receipt.PdaModuleId,
                PdaDescription = pdaDescr != null ? pdaDescr.Description : "",
                TotalVat = receipt.Vat,
                RelatedReceipts = related.ToList().Select(s => string.Join(s.Abbreviation, " ", s.Counter)).ToList(),
                TotalNet = receipt.Net ?? 0,
                ExtECRCode = receipt.ExtECRCode
            };

            if (payments.Count() > 0)
            {
                res.PaymentType = payments.FirstOrDefault().AccountDescription;
                res.RoomNo = payments.FirstOrDefault().Room;
                res.CustomerName = String.IsNullOrEmpty(receipt.CustomerName) || payments.FirstOrDefault().GuestId == null
                                            ? "Πελάτης Λιανικής"
                                            : String.IsNullOrEmpty(receipt.CustomerName) ? receipt.CustomerName : payments.FirstOrDefault().FirstName + " " + payments.FirstOrDefault().LastName;
                if (!String.IsNullOrEmpty(receipt.CustomerName))
                {
                    try
                    {
                        var guestData = db.Guest.Find(payments.FirstOrDefault().GuestId);
                        res.RoomNo = string.IsNullOrEmpty(payments.FirstOrDefault().Room) ? guestData.Room : payments.FirstOrDefault().Room;
                        res.GuestTerm = guestData.BoardCode;
                        res.Kids = guestData.Children.ToString();
                        res.Adults = guestData.Adults.ToString();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("GuestId : " + payments.FirstOrDefault().GuestId + " not found| " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                    }
                }

            }
            else if (!String.IsNullOrEmpty(receipt.Room))
            {
                res.PaymentType = receipt.PaymentsDesc;
                res.RoomNo = receipt.Room;
                res.CustomerName = String.IsNullOrEmpty(receipt.CustomerName) ? "Πελάτης Λιανικής" : receipt.CustomerName;

                EXTECR_PaymentTypeModel p1 = new EXTECR_PaymentTypeModel();
                p1.AccountType = 1;
                p1.Amount = receipt.Total;
                p1.Description = receipt.PaymentsDesc;

                Models.ExtecrModels.EXTECR_Guest g = new Models.ExtecrModels.EXTECR_Guest();
                g.FirstName = "";
                g.LastName = receipt.CustomerName;
                g.Room = res.RoomNo;
                Models.ExtecrModels.EXTECR_Guest g1 = new Models.ExtecrModels.EXTECR_Guest();
                g1.FirstName = "";
                g1.LastName = receipt.CustomerName;
                g1.Room = receipt.Room;
                p1.Guest = g1;
                res.PaymentsList.Add(p1);
            }
            else
            {
                res.PaymentType = "";
                res.RoomNo = "";
                res.CustomerName = String.IsNullOrEmpty(receipt.CustomerName) ? "Πελάτης Λιανικής" : receipt.CustomerName;
            }
            res.Department = departmentDescr != null ? departmentDescr.Id.ToString() : "";
            var groupeddetails = details.GroupBy(g => g.OrderDetailId)
                                       .Select(s => new
                                       {
                                           Product = s.Where(w => w.IsExtra == false).FirstOrDefault(),
                                           Extras = s.Where(w => w.IsExtra == true)
                                       });
            foreach (var det in groupeddetails.OrderBy(o => o.Product.ItemSort))
            {
                Models.ExtecrModels.EXTECR_ReceiptItemsModel prod = new Models.ExtecrModels.EXTECR_ReceiptItemsModel();
                prod.Date = receipt.Day != null ? receipt.Day.Value : DateTime.Now;
                prod.InvoiceNo = receipt.ReceiptNo.ToString();
                prod.IsChangeItem = false;
                prod.ItemCode = det.Product.ItemCode;
                prod.ItemCustomRemark = det.Product.ItemRemark;
                decimal ItemGrossTemp = det.Product.ItemGross != null ? det.Product.ItemGross.Value : 0;
                if (ItemGrossTemp < 0 && ((receipt.InvoiceTypeType != 8) && (receipt.InvoiceTypeType != 3)))
                {
                    prod.ItemDescr = "ΑΛΛΑΓΗ " + det.Product.ItemDescr;
                    prod.ExtraDescription = "ΑΛΛΑΓΗ " + det.Product.ExtraDescription;
                    prod.SalesDescription = "ΑΛΛΑΓΗ " + det.Product.SalesDescription;
                    prod.IsChangeItem = true;
                }
                else
                {
                    prod.ItemDescr = det.Product.ItemDescr;
                    prod.ExtraDescription = det.Product.ExtraDescription;
                    prod.SalesDescription = det.Product.SalesDescription;
                }

                prod.ItemDiscount = det.Product.ItemDiscount;
                prod.ItemGross = (det.Product.TotalBeforeDiscount ?? 0) == 0 ? (det.Product.ItemGross != null ? det.Product.ItemGross.Value : 0) : (det.Product.TotalBeforeDiscount != null ? det.Product.TotalBeforeDiscount.Value : 0);
                prod.ItemNet = det.Product.ItemNet != null ? det.Product.ItemNet.Value : 0;
                prod.ItemPrice = det.Product.Price != null ? det.Product.Price.Value : 0;
                prod.ItemQty = det.Product.ItemQty != null ? (decimal)det.Product.ItemQty.Value : 0;
                prod.ItemTotal = det.Product.ItemGross != null ? det.Product.ItemGross.Value : 0;
                prod.ItemNet = det.Product.ItemNet != null ? det.Product.ItemNet.Value : 0;
                prod.ItemVatDesc = det.Product.ItemVatRate != null ? det.Product.ItemVatRate.Value.ToString("#.##") : "0.00";
                prod.ItemVatRate = (int)det.Product.VatCode;
                prod.ItemVatValue = det.Product.ItemVatValue != null ? det.Product.ItemVatValue.Value : 0;
                prod.SalesTypeExtDesc = det.Product.SalesTypeDescription;

                if (!string.IsNullOrEmpty(det.Product.KitchenCode))
                {
                    int kcode = 0;
                    int.TryParse(det.Product.KitchenCode, out kcode);
                    prod.KitchenId = kcode;
                }
                prod.Time = receipt.Day != null ? receipt.Day.Value : DateTime.Now;
                //TODO: Apply 
                prod.RegionPosition = det.Product.RegionPosition;
                prod.ItemRegion = det.Product.ItemRegion;
                prod.ItemSort = det.Product.ItemSort;
                prod.ItemPosition = det.Product.ItemPosition;
                prod.ItemBarcode = prod.ItemBarcode;
                foreach (var ex in det.Extras.OrderBy(e => e.ItemSort))
                {
                    Models.ExtecrModels.EXTECR_ReceiptExtrasModel extra = new Models.ExtecrModels.EXTECR_ReceiptExtrasModel
                    {
                        IsChangeItem = false,
                        ItemCode = ex.ItemCode,//Convert.ToInt32(ex.ItemCode);
                        ItemDescr = ex.ItemDescr,
                        ItemDiscount = ex.ItemDiscount,
                        ItemGross = ex.ItemGross,
                        ItemNet = ex.ItemNet != null ? ex.ItemNet.Value : 0,
                        ItemPrice = ex.Price,
                        ItemQty = ex.ItemQty != null ? (decimal)ex.ItemQty.Value : 0,
                        ItemVatDesc = ex.ItemVatRate != null ? ex.ItemVatRate.Value.ToString("#.##") : "0.00",
                        ItemVatRate = (int)ex.VatCode,
                        ItemVatValue = ex.ItemVatValue != null ? ex.ItemVatValue.Value : 0
                    };
                    prod.Extras.Add(extra);

                }
                res.Details.Add(prod);
            }

            foreach (var pay in payments)
            {
                EXTECR_PaymentTypeModel p = new EXTECR_PaymentTypeModel();
                p.AccountType = pay.AccountType;
                p.Amount = pay.Amount;
                p.Description = pay.AccountDescription;
                if (pay.GuestId != null)
                {
                    Models.ExtecrModels.EXTECR_Guest g = new Models.ExtecrModels.EXTECR_Guest();
                    g.FirstName = pay.FirstName;
                    g.LastName = pay.LastName;
                    g.Room = pay.Room;
                    p.Guest = g;

                    if (pay.GuestId != null && pay.GuestId > 0)
                    {
                        try
                        {
                            var guestData = db.Guest.Find(pay.GuestId);

                            g.FirstName = string.IsNullOrEmpty(pay.FirstName) ? guestData.FirstName : pay.FirstName;
                            g.LastName = string.IsNullOrEmpty(pay.LastName) ? guestData.LastName : pay.LastName;
                            g.Room = string.IsNullOrEmpty(pay.Room) ? guestData.Room : pay.Room;
                            res.GuestTerm = guestData.BoardCode;
                            res.Kids = guestData.Children.ToString();
                            res.Adults = guestData.Adults.ToString();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
                res.PaymentsList.Add(p);
            }


            if (receipt.IsVoided == true && res.InvoiceType != 3 && res.InvoiceType != 8) { res.IsVoid = false; }

            string customerClassType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
            if (customerClassType == "Pos_WebApi.Customer_Modules.Waterpark")
            {
                foreach (var crPaym in payments)
                {
                    if (crPaym.AccountType == (short)AccountType.Barcode)
                    {
                        IQueryable<Models.ExtecrModels.EXTECR_CreditTransaction> query;
                        query = from ct in db.CreditTransactions
                                join ca in db.CreditAccounts on ct.CreditAccountId equals ca.Id
                                join ct2 in db.CreditTransactions on ca.Id equals ct2.CreditAccountId
                                where ct2.TransactionId == crPaym.Id
                                group ct by ct.CreditAccountId into q
                                select new Models.ExtecrModels.EXTECR_CreditTransaction
                                {
                                    CreditAccountId = q.Key,
                                    Amount = q.Sum(f => f.Amount),
                                };
                        Models.ExtecrModels.EXTECR_CreditTransaction tempCreditPayment = query.FirstOrDefault();
                        if (tempCreditPayment != null)
                        {
                            res.CreditTransactions.Add(tempCreditPayment);
                        }
                    }
                }

            }
            return res;
        }

        /// <summary>
        /// decide if client is running in multiple hotels mode (has hotelinfo record with type=4)
        /// </summary>
        /// <returns>bool: true if hotelinfo has type=4 (multiple hotel records), false otherwise</returns>
        public bool hasMultipleHotels()
        {
            bool hasMultipleHotels = false;

            List<HotelInfo> hotelList = new List<HotelInfo>();

            hotelList = db.HotelInfo.ToList();

            foreach (HotelInfo hotel in hotelList)
            {
                if (hotel.Type == 4)
                {
                    hasMultipleHotels = true;

                    break;
                }
            }

            return hasMultipleHotels;
        }

        public bool ChangePaymentType(string storeid, Receipts r, long Id)
        {
             long? prevHotelId = 1;
            var transferToPmsList = new List<TransferToPms>();
            var inv = db.Invoices.FirstOrDefault(x => x.Id == r.Id);
            var pi = db.PosInfo.Include(i => i.Department).FirstOrDefault(f => f.Id == inv.PosInfoId);
            var isFiscal = pi != null ? pi.FiscalType == 2 : false;
            if (pi != null)
            {
                r.DepartmentId = pi.DepartmentId;
                r.DepartmentDescription = pi.Department != null ? pi.Department.Description : "";
            }

            List<Transactions> currentTransactions = db.Transactions.Include(i => i.TransferToPms).Where(w => w.InvoicesId == r.Id && (w.IsDeleted ?? false) == false).ToList();
            bool checkForPaymentChangeMethod = false;
            long hotelId;
            if (currentTransactions[0].TransferToPms == null || currentTransactions[0].TransferToPms.ToList().Count() == 0)
                hotelId = Id;
            else
                hotelId = (currentTransactions[0].TransferToPms.ToList()[0].HotelId) ?? Id;

            bool hasMultipleHotels = this.hasMultipleHotels();

            HotelInfo hi;

            if (hasMultipleHotels)
            {
                try
                {
                    hi = db.HotelInfo.First(h => h.HotelId == hotelId);
                }
                catch (Exception e)
                {
                    logger.Error("No matching HotelInfo found!");

                    logger.Error(e.ToString());

                    return false;
                }
            }
            else
            {
                hi = db.HotelInfo.FirstOrDefault(h => h.HotelId == hotelId);
            }

            checkForPaymentChangeMethod = this.CheckHotelTypeAndAccountId(currentTransactions, hi);
            if (!checkForPaymentChangeMethod)
            {
                return false;
            }
            foreach (var t in currentTransactions)
            {
                t.IsDeleted = true;
                db.Entry(t).State = EntityState.Modified;

                foreach (var mdq in t.TransferToPms.ToList())
                {
                    prevHotelId = mdq.HotelId;
                    TransferToPms ttp = new TransferToPms();

                    SendTransferRepository stRepository = new SendTransferRepository(db, t.PosInfoId.Value, t.DepartmentId.Value); 
                    if (mdq.ProfileId == null)
                    {
                        ttp = stRepository.WriteCashToTransfer(Convert.ToInt64(mdq.ReceiptNo),
                                                               mdq.PmsDepartmentId,
                                                               mdq.PmsDepartmentDescription,
                                                               mdq.PosInfoDetailId.Value,
                                                               mdq.ProfileName,
                                                               mdq.RoomDescription,
                                                               (decimal)mdq.Total * -1,
                                                               isFiscal,
                                                               true,
                                                               mdq.PMSPaymentId,
                                                               mdq.PMSInvoiceId,
                                                               mdq.SendToPMS ?? false,
                                                               mdq.HotelId);
                    }
                    else
                    {
                        ttp = stRepository.WriteRoomChargeToTransfer(Convert.ToInt64(mdq.ReceiptNo),
                                                                     mdq.PmsDepartmentId,
                                                                     mdq.PmsDepartmentDescription,
                                                                     mdq.PosInfoDetailId.Value,
                                                                     mdq.ProfileId,
                                                                     mdq.ProfileName,
                                                                     mdq.RegNo,
                                                                     mdq.RoomId,
                                                                     mdq.RoomDescription,
                                                                     (decimal)mdq.Total * -1,
                                                                     true,
                                                                     mdq.PMSPaymentId,
                                                                     mdq.PMSInvoiceId,
                                                                     mdq.HotelId);
                    }
                    ttp.Transactions = t;
                    db.TransferToPms.Add(ttp);
                    transferToPmsList.Add(ttp);
                }
            }
            r.ModifyOrderDetails = Symposium.Models.Enums.ModifyOrderDetailsEnum.ChangePaymentType;

            foreach (var rp in r.ReceiptPayments)
            {

                rp.HotelId = rp.HotelId == null ? prevHotelId : rp.HotelId;
                Transactions newtrs = CreateTransactionsFromReceiptPayments(r, rp, inv);
                newtrs.InvoicesId = r.Id;
                db.Transactions.Add(newtrs);
                transferToPmsList.AddRange(newtrs.TransferToPms);
            }
            //Payment From
            inv.PaymentsDesc = r.ReceiptPayments != null && r.ReceiptPayments.Count() > 0 ? r.ReceiptPayments.Select(s => s.AccountDescription).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) : "";

            inv.Rooms = r.ReceiptPayments != null && r.ReceiptPayments.Count() > 0 ? r.ReceiptPayments.Select(s => s.Room).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) : "";
            db.Entry(inv).State = EntityState.Modified;
            db.SaveChanges();
            SendTransferRepository str = new SendTransferRepository(db);
            //str.SendTransferToPMS(transferToPmsList, _storeid);
          
            return true;
        }

        public bool CheckHotelTypeAndAccountId(List<Transactions> transactions, HotelInfo hi)
        {
            List<Accounts> acc = db.Accounts.ToList();
            short? accountType = 0;
            foreach (Transactions tr in transactions)
            {
                Accounts result = acc.Where(x => x.Id == tr.AccountId).FirstOrDefault();
                accountType = result.Type;
                if (accountType == 2 || accountType == 3 || accountType == 9)
                {
                    PMSConnection pmsconn = new PMSConnection();
                    if (hi != null)
                    {
                        if (hi.Type == 0 || hi.Type == 4 || hi.Type == 10)
                        {
                            string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName + ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                            pmsconn.initConn(connStr);
                            foreach (TransferToPms ttpms in tr.TransferToPms)
                            {
                                bool res = pmsconn.checkForPaymentChange(ttpms.RegNo, hi.HotelType);
                                if (!res)
                                    return false;
                            }
                        }
                    }
                    else
                        return false;
                }
            }
            return true;
        }



        /// <summary>
        ///Insert new receipt to DB
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="externalCommunication"></param>
        /// <returns></returns>
        public dynamic InsertInvoice(Receipts receipt, out bool externalCommunication)
        {
            externalCommunication = true;
            HotelInfo hi = db.HotelInfo.FirstOrDefault(); // I have to make a function in order to choose hotel depending on Type

            if (receipt.ModifyOrderDetails != Symposium.Models.Enums.ModifyOrderDetailsEnum.PayOffOnly)
            {
                //newCounter contains OrderNo and ReceiptNo
                var newCounter = UpdateInvoiceCounters(receipt);
                if (newCounter.ReceiptNo == 0)
                    return null;

                receipt.ReceiptNo = (int)newCounter.ReceiptNo;
                if (receipt.ModifyOrderDetails == (int)ModifyOrderDetailsEnum.FromScratch)
                {
                    receipt.OrderNo = newCounter.OrderNo.ToString();

                    //TO BE REVIEWDED!!!!!!!!!!!!!!!!!!!
                    foreach (ReceiptDetails rec in receipt.ReceiptDetails)
                        rec.OrderNo = long.Parse(receipt.OrderNo.Trim());
                }
            }

            var transferToPmsList = new List<TransferToPms>();
            Invoices inv = CreateInvoiceFromReceipt(receipt, (ModifyOrderDetailsEnum)receipt.ModifyOrderDetails, out externalCommunication);
            if (externalCommunication == false)
            {
                return null;
            }
            if ((ModifyOrderDetailsEnum)receipt.ModifyOrderDetails != ModifyOrderDetailsEnum.PayOffOnly)
            {
                db.Invoices.Add(inv);
            }
     
            transferToPmsList.AddRange(inv.Transactions.SelectMany(sm => sm.TransferToPms));
            db.SaveChanges();

            string customerClassType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
            if (customerClassType == "Pos_WebApi.Customer_Modules.Goodies")
            {
                orderDetailsFlows = Resolve<IOrderDetailFlows>();
            var fld = db.Order.Local.ToList().Find(x => x.ExtKey == receipt.ExtKey);
           
                if (fld != null && fld.ExtType == 1 && (fld.IsDelay ?? false) == false)
                    orderDetailsFlows.SetStatusToOrderDetails(DBInfo, db.Order.Local[0].Id, Symposium.Models.Enums.OrderStatusEnum.Preparing);
            }
            //checks if need to call Api Url for Hotelizer or other external systems and sends post method
            bool extApi = false;
            extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
            if (extApi)
            {
                //Gets a list of transfer to pms model from invoice transaction
                List<TransferToPmsModel> transferToPms = new List<TransferToPmsModel>();
                if (inv.Transactions != null)
                {
                    foreach (Transactions item in inv.Transactions)
                    {
                        if (item.TransferToPms != null)
                        {
                            foreach (TransferToPms ttp in item.TransferToPms)
                                transferToPms.Add(ttpmsTask.GetModelById(DBInfo, ttp.Id));
                        }
                    }
                }
                //post receipt using Hotelizer Api Url
                HotelizerFlows hotelizer = new HotelizerFlows();
                hotelizer.PostNewChargeToHotelizer(receipt, transferToPms, inv.Id, DBInfo);
            }

            if (receipt.MacroGuidId != null)
            {
                UpdateConsumptionModel consumptionData = new UpdateConsumptionModel();
                consumptionData.macroGuid = (Guid)receipt.MacroGuidId;
                consumptionData.couver = (int)receipt.Cover;
                iHotelFlows = Resolve<IHotelFlows>();
                iHotelFlows.UpdateMacroRemainingConsumption(DBInfo, consumptionData);
            }

            if ((ModifyOrderDetailsEnum)receipt.ModifyOrderDetails == ModifyOrderDetailsEnum.FromOtherUnmodified || (ModifyOrderDetailsEnum)receipt.ModifyOrderDetails == ModifyOrderDetailsEnum.FromOtherUpated)
            {
                daOrderStatusFlows = Resolve<IDA_OrderStatusFlows>();
                daOrderStatusFlows.InsertOrderStatusFromInvoiceId(DBInfo, inv.Id);
            }

           
            SendTransferRepository str = new SendTransferRepository(db);
            //str.SendTransferToPMS(transferToPmsList, _storeid);
            var pi = db.PosInfo.Find(inv.PosInfoId);

            return new { ExtecrName = pi != null ? pi.FiscalName : "Invalid", InvoiceId = inv.Id };
        }

        public bool InsertNotPosted(Receipts receipt, string message)
        {
            var insertedOrder = db.ExternalLostOrders.Where(s => s.ExtType == receipt.ExtType && s.OrderNo == receipt.ExtKey).FirstOrDefault();
            if (insertedOrder == null)
            {
                ExternalLostOrders lostOrder = new ExternalLostOrders();
                lostOrder.ExtType = receipt.ExtType ?? 0;
                lostOrder.OrderNo = receipt.ExtKey;
                if (lostOrder.ExtType != 0 && lostOrder.OrderNo != null)
                {
                    db.ExternalLostOrders.Add(lostOrder);
                    db.Entry(lostOrder).State = EntityState.Added;
                    db.SaveChanges();
                }
            }
            var logger = log4net.LogManager.GetLogger(this.GetType());
            logger.Error(message);
            return true;
        }



        private OrderDetailIgredients CreateOrderDetailIngredient(ReceiptDetails rd, OrderDetail od, ModifyOrderDetailsEnum detailStatus)
        {
            OrderDetailIgredients odi = new OrderDetailIgredients
            {
                IngredientId = rd.ProductId,
                PriceListDetailId = rd.PriceListDetailId,
                Price = rd.Price,
                Qty = rd.ItemQty,
                TotalAfterDiscount = rd.ItemGross,
                Discount = rd.ItemDiscount,
                PendingQty = rd.ItemQty
            };
            if (!(detailStatus == ModifyOrderDetailsEnum.FromScratch))
                odi.OrderDetailId = od.Id;
            return odi;
        }

        private OrderDetailInvoices CreateOrderDetailInvoicesFromReceiptDetails(ReceiptDetails receiptDetails, Invoices invoice, string abbreviation, ModifyOrderDetailsEnum detailStatus, OrderDetail previousOd, Order newOrder, bool hasPayments, int modifiedCounter)
        {
            OrderDetailInvoices odi = new OrderDetailInvoices
            {
                CreationTS = DateTime.Now,
                Counter = invoice.Counter,
                PosInfoDetailId = invoice.PosInfoDetailId,
                OrderDetailId = receiptDetails.OrderDetailId,
                StaffId = receiptDetails.StaffId,
                IsPrinted = invoice.IsPrinted,
                //New Fields for OrderDetailInvoices
                Abbreviation = abbreviation,
                IsExtra = receiptDetails.IsExtra == 0 ? false : true,
                ItemCode = receiptDetails.ItemCode,
                ItemRemark = receiptDetails.ItemRemark,
                ProductId = receiptDetails.ProductId,
                Qty = receiptDetails.ItemQty,
                VatAmount = receiptDetails.ItemVatValue,
                VatCode = (int)receiptDetails.VatCode,
                VatId = receiptDetails.VatId,
                VatRate = receiptDetails.ItemVatRate,
                TaxId = receiptDetails.TaxId,
                TaxAmount = receiptDetails.ItemTaxAmount,
                Total = receiptDetails.ItemGross,
                Net = receiptDetails.ItemNet,
                Price = receiptDetails.Price,
                Discount = receiptDetails.ItemDiscount,
                PricelistId = receiptDetails.PricelistId,
                InvoiceType = receiptDetails.InvoiceType,
                Description = receiptDetails.ItemDescr,
                TableId = receiptDetails.TableId,
                TableCode = receiptDetails.TableCode,
                RegionId = receiptDetails.RegionId,
                OrderNo = receiptDetails.OrderNo,
                PosInfoId = receiptDetails.PosInfoId,
                EndOfDayId = invoice.EndOfDayId,
                //New Fields In OrderDetailInvoices
                ProductCategoryId = receiptDetails.ProductCategoryId,
                CategoryId = receiptDetails.CategoryId,
                SalesTypeId = receiptDetails.SalesTypeId,
                //New Fields for 
                //TODO:Activate after update db
                ItemPosition = receiptDetails.ItemPosition,
                ItemSort = receiptDetails.ItemSort,
                ItemRegion = receiptDetails.ItemRegion,
                RegionPosition = receiptDetails.RegionPosition,
                ItemBarcode = receiptDetails.ItemBarcode,
                TotalBeforeDiscount = receiptDetails.TotalBeforeDiscount,
                ReceiptSplitedDiscount = receiptDetails.ReceiptSplitedDiscount,
                TableLabel = receiptDetails.TableLabel
            };

            switch (detailStatus)
            {
                case ModifyOrderDetailsEnum.FromScratch:
                    if (receiptDetails.IsExtra == 0)
                    {
                        OrderDetail od = CreateOrderDetail(receiptDetails);
                        od.Order = newOrder;
                        newOrder.OrderDetail.Add(od);
                        odi.OrderDetail = od;
                        od.OrderDetailInvoices.Add(odi);
                    }
                    else
                    {
                        var ing = CreateOrderDetailIngredient(receiptDetails, previousOd, detailStatus);
                        ing.OrderDetail = previousOd;
                        odi.OrderDetailIgredients = ing;
                        odi.OrderDetail = previousOd;
                    }
                    break;
                case ModifyOrderDetailsEnum.FromOtherUnmodified:
                    odi.OrderDetailId = receiptDetails.OrderDetailId;
                    if (receiptDetails.IsExtra == 1)
                        odi.OrderDetailIgredientsId = receiptDetails.OrderDetailIgredientsId;
                    break;
                case ModifyOrderDetailsEnum.FromOtherUpated: break;
                default: break;
            }

            if (receiptDetails.SelectedQuantity != null)
            {
                // SelectedQuantity and ItemQty have been reversed in agent. ItemQty is target quantity
                UpdateOrderDetailWithSplitQuantity(odi, receiptDetails.ItemQty ?? 0, receiptDetails.SelectedQuantity ?? 0, previousOd, hasPayments, modifiedCounter, receiptDetails);
            }

            return odi;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="isPaid"></param>
        /// <param name="staffId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private bool InsertOrderStatus(Order o, bool isPaid, long staffId, short invoiceType)
        {
            bool newDeliveryMask = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "NewDeliveryMask");
            if (newDeliveryMask)
            {
                bool res = true;
                var status = 1;
                switch (invoiceType)
                {
                    case 3:
                    case 8:
                        status = 5;
                        break;
                    default:
                        status = 1;
                        break;
                }
                if (o.Id == 0 || status == 5)
                {
                    try
                    {
                        long daOrderId = 0;
                        long.TryParse(o.ExtKey, out daOrderId);
                        OrderStatus os = new OrderStatus
                        {
                            Order = o,
                            StaffId = staffId,
                            Status = status,
                            TimeChanged = DateTime.Now,
                            ExtState = o.ExtType,
                            DAOrderId = daOrderId,
                            IsSend = false
                        };
                        if (o.ExtType != null && status != 5)
                        {
                            os.ExtState = 1;
                        }
                        db.OrderStatus.Add(os);

                        if (o.ExtType == 1 && status == 1)
                        {
                            OrderStatus os2 = new OrderStatus
                            {
                                Order = o,
                                StaffId = staffId,
                                Status = 1,
                                TimeChanged = DateTime.Now
                            };
                            db.OrderStatus.Add(os2);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        res = false;
                    }
                }
                return res;
            }
            else
            {
                bool res = true;
                var status = 0;
                if (o.ExtType == 1 && o.IsDelay == true)
                    status = 0;
                else if (o.ExtType == 1 && (o.IsDelay == false || o.IsDelay ==null))
                    status = 1;
                else{ 
                    switch (invoiceType)
                    {
                        case 3:
                        case 8:
                            status = 5;
                            break;
                        default:
                            status = 0;
                            break;
                    }
                }
                if (o.Id == 0 || status == 5)
                {
                    try
                    {
                        OrderStatus os = new OrderStatus
                        {
                            Order = o,
                            StaffId = staffId,
                            Status = status,
                            TimeChanged = DateTime.Now
                        };
                        if (o.ExtType != null && status != 5)
                        {
                            os.ExtState = o.ExtType;
                        }
                        db.OrderStatus.Add(os);

                        //if (o.ExtType == 1 && status == 0 && o.IsDelay == false)
                        //{
                        //    OrderStatus os2 = new OrderStatus
                        //    {
                        //        Order = o,
                        //        StaffId = staffId,
                        //        Status = 1,
                        //        TimeChanged = DateTime.Now
                        //    };
                        //    db.OrderStatus.Add(os2);
                        //}
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        res = false;
                    }
                }
                return res;
            }
        }

        public bool UpdateOrderDetailStatus(IEnumerable<OrderDetail> ods, byte status, byte paidStatus)
        {
            bool res = true;
            try
            {
                foreach (var item in ods)
                {
                    item.Status = status == 5 ? status : item.Status;
                    item.PaidStatus = paidStatus;
                    db.Entry(item).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                res = false;
            }
            return res;
        }

        /// <summary>
        /// 0 - invoiced, 1 - void
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="">0 - invoiced, 1 - void</param>
        /// <returns></returns>
        public bool UpdateRelativeInvoiceStatus(Invoices inv, byte status)
        {
            bool res = true;
            try
            {
                switch (status)
                {
                    case 0: inv.IsInvoiced = true; break;
                    case 1: inv.IsVoided = true; break;
                }
                db.Entry(inv).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                res = false;
            }
            return res;
        }

        private IEnumerable<InvoiceForStatusModel> GetAllOrderDetailsFromDB(List<long> ods)
        {
            return (from q in db.OrderDetailInvoices.Where(w => ods.Contains(w.OrderDetailId ?? 0))
                    join qq in db.OrderDetail on q.OrderDetailId equals qq.Id
                    select new
                    {
                        InvoicesId = q.InvoicesId,
                        Abbreviation = q.Abbreviation,
                        Counter = q.Counter,
                        InvoiceType = q.InvoiceType,
                        PaidStatus = qq.PaidStatus,
                        OrderDetailId = q.OrderDetailId,
                        Status = qq.Status
                    }).ToList().GroupBy(g => g.InvoicesId).Select(s => new InvoiceForStatusModel
                    {
                        InvoicesId = s.Key.Value,
                        IsInvoiced = s.All(a => a.PaidStatus > 0),
                        IsVoided = s.All(a => a.Status == 5),
                        InvoiceType = s.FirstOrDefault().InvoiceType
                    });
        }


        private bool InvoiceStatusUpdate(Receipts r, int status, ModifyOrderDetailsEnum detailStatus, Order currentOrder)
        {
            var ods = r.ReceiptDetails.Select(s => s.OrderDetailId);
            var odUpdates = db.OrderDetail.Where(w => ods.Contains(w.Id)).ToList();

            switch (detailStatus)
            {
                case ModifyOrderDetailsEnum.FromScratch:
                    UpdateOrderDetailStatus(odUpdates, (byte)r.IsPaid, 0);
                    break;
                case ModifyOrderDetailsEnum.FromOtherUnmodified:
                case ModifyOrderDetailsEnum.FromOtherUpated:
                    switch (r.InvoiceTypeType)
                    {
                        case 2://Deltia Paraggelias
                            UpdateOrderDetailStatus(odUpdates, 0, 0);
                            //
                            break;
                        case 3://Akyrwtika
                            UpdateOrderDetailStatus(odUpdates, 5, 2);
                            var invsToVoid = GetAllOrderDetailsFromDB(odUpdates.Select(s => s.Id).ToList()).Where(w => w.InvoiceType != 8 && w.InvoiceType != 3).Select(s => s.InvoicesId).Distinct();
                            var invs = db.Invoices.Where(w => invsToVoid.Contains(w.Id)).ToList();
                            foreach (var itv in invs)
                            {
                                UpdateRelativeInvoiceStatus(itv, 1);
                            }
                            break;
                        case 8://akyrwtika deltiwn
                            UpdateOrderDetailStatus(odUpdates, 5, 2);
                            var odsUpdates = GetAllOrderDetailsFromDB(odUpdates.Select(s => s.Id).ToList()).Where(w => w.InvoiceType != 8 && w.InvoiceType != 3).Select(s => s.InvoicesId).Distinct();
                            var ordersToVoid = db.Invoices.Where(w => odsUpdates.Contains(w.Id)).ToList();
                            foreach (var rel in ordersToVoid)
                            {
                                var isAllVoided = db.OrderDetailInvoices.Where(w => w.InvoicesId == rel.Id && w.IsExtra == false).ToList();
                                var ct = isAllVoided.Select(s => new { OrderDetailId = s.OrderDetailId }).Intersect(r.ReceiptDetails.Select(s => new { OrderDetailId = s.OrderDetailId }));
                                var previousDetails = db.OrderDetailInvoices.Include(i => i.OrderDetail).Where(w => w.InvoicesId == rel.Id && w.IsExtra == false);
                                var allreadyCanceled = previousDetails.Where(w => w.OrderDetail.Status == 5).Count();


                                if (ct.Count() + allreadyCanceled == isAllVoided.Count())
                                {
                                    UpdateRelativeInvoiceStatus(rel, 1);
                                    InsertOrderStatus(currentOrder, true, r.StaffId.Value, 5);
                                }
                                else if (ct.Count() + previousDetails.Where(w => w.OrderDetail.PaidStatus > 0).Count() == previousDetails.Count())
                                {
                                    UpdateRelativeInvoiceStatus(rel, 0);
                                }
                            }
                            break;
                        default:
                            UpdateOrderDetailStatus(odUpdates, 0, (byte)(r.IsPaid > 1 ? 2 : 1));
                            MarkRelatedInvoicesAsInvoiced(r, odUpdates.Select(s => s.Id).ToList());
                            break;
                    }
                    break;
                case ModifyOrderDetailsEnum.PayOffOnly:
                    //Marks OrderDetails As Paid Or Partial Paid
                    UpdateOrderDetailStatus(odUpdates, 0, (byte)(r.IsPaid > 1 ? 2 : r.InvoiceTypeType == 2 ? 1 : 2));
                    MarkRelatedInvoicesAsInvoiced(r, odUpdates.Select(s => s.Id).ToList());
                    break;
                default:
                    break;
            }

            InsertOrderStatus(currentOrder, r.IsPaid == 2, r.StaffId.Value, r.InvoiceTypeType.Value);
            return true;
        }

        private void MarkRelatedInvoicesAsInvoiced(Receipts r, List<long> odUpdates)
        {
            //Retrieves all related OrderDetails
            var odisUpdates = GetAllOrderDetailsFromDB(odUpdates).Where(w => w.InvoiceType == 2);///.Select(s => s.InvoicesId).Distinct();
            var ids = odisUpdates.Select(s => s.InvoicesId).Distinct();
            //Retrieve all related Invoices
            var invs = db.Invoices.Where(w => ids.Contains(w.Id)).ToList();

            foreach (var rel in invs)
            {
                var crOdis = r.ReceiptDetails.Select(s => s.OrderDetailId);
                // Filters out OrderDetails from other Invoices
                var checkForAllDetails = db.OrderDetailInvoices.Include("OrderDetail").Where(w => w.InvoicesId == rel.Id && !crOdis.Contains(w.OrderDetailId)).Select(s => new
                {
                    PaidStatus = s.OrderDetail.PaidStatus,
                    OrderDetailId = s.OrderDetailId
                });
                var markAsInvoiced = false;
                // If checkForAllDetails not empty 
                if (checkForAllDetails.Count() > 0)
                {
                    markAsInvoiced = checkForAllDetails.All(s => s.PaidStatus > 0) && r.ReceiptDetails.All(a => a.PaidStatus > 0);
                }
                else //Single invoice for complete Order
                {
                    markAsInvoiced = r.ReceiptDetails.All(a => a.PaidStatus > 0);
                    //odisUpdates.All(a => a.IsInvoiced == true);
                }

                if (markAsInvoiced)
                    UpdateRelativeInvoiceStatus(rel, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odi"></param>
        /// <param name="selectedQuantity"></param>
        private void UpdateOrderDetailWithSplitQuantity(OrderDetailInvoices odi, double selectedQuantity, double oldQuantity, OrderDetail odPrevious, bool hasPayments, int modifiedCounter, ReceiptDetails receiptDetailFromPos)
        {
            if (odi.IsExtra == false)
            {
                OrderDetailInvoices startOdi = db.OrderDetailInvoices.Where(w => w.IsExtra == false && w.OrderDetailIgredientsId == null && w.OrderDetailId == odi.OrderDetailId).FirstOrDefault();//LastOrDefault
                long orderDetailId = odi.OrderDetailId ?? 0;
                OrderDetail startOd = db.OrderDetail.Where(w => w.Id == orderDetailId).FirstOrDefault();
                OrderDetail od = new OrderDetail
                {
                    OrderId = startOd.OrderId,
                    ProductId = startOd.ProductId,
                    KitchenId = startOd.KitchenId,
                    KdsId = startOd.KdsId,
                    PreparationTime = startOd.PreparationTime,
                    TableId = startOd.TableId,
                    Status = startOd.Status,
                    StatusTS = startOd.StatusTS,
                    Price = startOd.Price,
                    PriceListDetailId = startOd.PriceListDetailId,
                    Qty = startOd.Qty,
                    SalesTypeId = startOd.SalesTypeId,
                    Discount = startOd.Discount,
                    PaidStatus = startOd.PaidStatus,
                    TransactionId = startOd.TransactionId,
                    TotalAfterDiscount = startOd.TotalAfterDiscount,
                    GuestId = startOd.GuestId,
                    Couver = startOd.Couver,
                    Guid = new Guid(),
                    //Guid = Guid.NewGuid(),
                    IsDeleted = startOd.IsDeleted,
                    PendingQty = startOd.PendingQty
                };
                // OrderDetailInvoices old entry
                startOdi.Discount = startOdi.Discount - odi.Discount;
                startOdi.ReceiptSplitedDiscount = startOdi.ReceiptSplitedDiscount - odi.ReceiptSplitedDiscount;
                startOdi.Net = startOdi.Net - odi.Net;
                startOdi.VatAmount = startOdi.VatAmount - odi.VatAmount;
                startOdi.TaxAmount = startOdi.TaxAmount - odi.TaxAmount;
                startOdi.Total = startOdi.Total - odi.Total;
                startOdi.TotalBeforeDiscount = startOdi.TotalBeforeDiscount - odi.TotalBeforeDiscount;
                startOdi.TotalAfterDiscount = startOdi.TotalAfterDiscount - odi.TotalAfterDiscount;
                startOdi.Qty = startOdi.Qty - odi.Qty;
                db.Entry(startOdi).State = EntityState.Modified;
                // OrderDetail entries
                decimal newQuantityOd = (decimal)selectedQuantity;
                decimal oldQuantityOd = (decimal)oldQuantity;
                decimal DiscountOd = od.Discount ?? 0;
                od.Discount = (newQuantityOd * DiscountOd) / oldQuantityOd;
                od.Discount = (decimal)Math.Round((double)od.Discount, 2);
                startOd.Discount = DiscountOd - od.Discount;
                decimal TotalAfterOd = od.TotalAfterDiscount ?? 0;
                od.TotalAfterDiscount = (newQuantityOd * TotalAfterOd) / oldQuantityOd;
                od.TotalAfterDiscount = (decimal)Math.Round((double)od.TotalAfterDiscount, 2);
                startOd.TotalAfterDiscount = TotalAfterOd - od.TotalAfterDiscount;
                od.Qty = (double)newQuantityOd;
                startOd.Qty = (double)oldQuantityOd - od.Qty;
                od.PaidStatus = hasPayments ? (byte)2 : (byte)1;
                db.Entry(startOd).State = EntityState.Modified;
                db.OrderDetail.Add(od);
                odi.OrderDetail = od;
                receiptDetailFromPos.OrderDetailId = od.Id;
                // Re-evaluate item sort of order detail invoices of previous invoice
                long iPreviousId = startOdi.InvoicesId ?? 0;
                List<OrderDetailInvoices> odisPrevious = db.OrderDetailInvoices.Where(w => w.InvoicesId == iPreviousId).ToList();
                int index = 0;
                foreach (OrderDetailInvoices item in odisPrevious)
                {
                    index++;
                    item.ItemSort = index;
                    db.Entry(item).State = EntityState.Modified;
                }
                // OrderDetailInvoices new entry
                OrderDetailInvoices virtualStartOdi = new OrderDetailInvoices
                {
                    OrderDetail = odi.OrderDetail,
                    StaffId = startOdi.StaffId,
                    PosInfoDetailId = startOdi.PosInfoDetailId,
                    Counter = startOdi.Counter,
                    CreationTS = startOdi.CreationTS,
                    IsPrinted = startOdi.IsPrinted,
                    CustomerId = startOdi.CustomerId,
                    InvoicesId = startOdi.InvoicesId,
                    IsDeleted = startOdi.IsDeleted,
                    OrderDetailIgredientsId = odi.OrderDetailIgredientsId,
                    Price = startOdi.Price,
                    Discount = odi.Discount,
                    Net = odi.Net,
                    VatRate = startOdi.VatRate,
                    VatAmount = odi.VatAmount,
                    VatId = startOdi.VatId,
                    TaxId = startOdi.TaxId,
                    VatCode = startOdi.VatCode,
                    TaxAmount = odi.TaxAmount,
                    Qty = odi.Qty,
                    Total = odi.Total,
                    PricelistId = startOdi.PricelistId,
                    ProductId = startOdi.ProductId,
                    Description = startOdi.Description,
                    ItemCode = startOdi.ItemCode,
                    ItemRemark = startOdi.ItemRemark,
                    InvoiceType = startOdi.InvoiceType,
                    TableId = startOdi.TableId,
                    TableCode = startOdi.TableCode,
                    RegionId = startOdi.RegionId,
                    OrderNo = startOdi.OrderNo,
                    OrderId = startOdi.OrderId,
                    IsExtra = startOdi.IsExtra,
                    PosInfoId = startOdi.PosInfoId,
                    EndOfDayId = startOdi.EndOfDayId,
                    Abbreviation = startOdi.Abbreviation,
                    DiscountId = startOdi.DiscountId,
                    SalesTypeId = startOdi.SalesTypeId,
                    ProductCategoryId = startOdi.ProductCategoryId,
                    CategoryId = startOdi.CategoryId,
                    ItemPosition = startOdi.ItemPosition,
                    ItemSort = index + modifiedCounter,
                    ItemRegion = startOdi.ItemRegion,
                    RegionPosition = startOdi.RegionPosition,
                    ItemBarcode = startOdi.ItemBarcode,
                    TotalBeforeDiscount = odi.TotalBeforeDiscount,
                    TotalAfterDiscount = odi.TotalAfterDiscount,
                    ReceiptSplitedDiscount = odi.ReceiptSplitedDiscount
                };
                db.OrderDetailInvoices.Add(virtualStartOdi);
            }
            else
            {
                OrderDetailInvoices startOdi = db.OrderDetailInvoices.Where(w => w.IsExtra == true && w.OrderDetailIgredientsId == odi.OrderDetailIgredientsId && w.OrderDetailId == odi.OrderDetailId).FirstOrDefault();//LastOrDefault
                long orderDetailIngredientId = odi.OrderDetailIgredientsId ?? 0;
                OrderDetailIgredients startOdg = db.OrderDetailIgredients.Where(w => w.Id == orderDetailIngredientId).FirstOrDefault();
                OrderDetailIgredients odg = new OrderDetailIgredients
                {
                    IngredientId = startOdg.IngredientId,
                    Qty = startOdg.Qty,
                    UnitId = startOdg.UnitId,
                    Price = startOdg.Price,
                    PriceListDetailId = startOdg.PriceListDetailId,
                    Discount = startOdg.Discount,
                    TotalAfterDiscount = startOdg.TotalAfterDiscount,
                    IsDeleted = startOdg.IsDeleted
                };
                // OrderDetailInvoices old entry
                startOdi.Discount = startOdi.Discount - odi.Discount;
                startOdi.ReceiptSplitedDiscount = startOdi.ReceiptSplitedDiscount - odi.ReceiptSplitedDiscount;
                startOdi.Net = startOdi.Net - odi.Net;
                startOdi.VatAmount = startOdi.VatAmount - odi.VatAmount;
                startOdi.TaxAmount = startOdi.TaxAmount - odi.TaxAmount;
                startOdi.Total = startOdi.Total - odi.Total;
                startOdi.TotalBeforeDiscount = startOdi.TotalBeforeDiscount - odi.TotalBeforeDiscount;
                startOdi.TotalAfterDiscount = startOdi.TotalAfterDiscount - odi.TotalAfterDiscount;
                startOdi.Qty = startOdi.Qty - odi.Qty;
                db.Entry(startOdi).State = EntityState.Modified;
                // OrderDetailIngredients entries
                decimal newQuantityOdg = (decimal)selectedQuantity;
                decimal oldQuantityOdg = (decimal)oldQuantity;
                decimal DiscountOdg = odg.Discount ?? 0;
                odg.Discount = (newQuantityOdg * DiscountOdg) / oldQuantityOdg;
                odg.Discount = (decimal)Math.Round((double)odg.Discount, 2);
                startOdg.Discount = DiscountOdg - odg.Discount;
                decimal TotalAfterOdg = odg.TotalAfterDiscount ?? 0;
                odg.TotalAfterDiscount = (newQuantityOdg * TotalAfterOdg) / oldQuantityOdg;
                odg.TotalAfterDiscount = (decimal)Math.Round((double)odg.TotalAfterDiscount, 2);
                startOdg.TotalAfterDiscount = TotalAfterOdg - odg.TotalAfterDiscount;
                odg.Qty = (double)newQuantityOdg;
                startOdg.Qty = (double)oldQuantityOdg - odg.Qty;
                db.Entry(startOdg).State = EntityState.Modified;
                odg.OrderDetail = odPrevious;
                db.OrderDetailIgredients.Add(odg);
                odi.OrderDetailIgredients = odg;
                odi.OrderDetail = odPrevious;
                receiptDetailFromPos.OrderDetailId = odPrevious.Id;
                // Re-evaluate item sort of order detail invoices of previous invoice
                long iPreviousId = startOdi.InvoicesId ?? 0;
                List<OrderDetailInvoices> odisPrevious = db.OrderDetailInvoices.Where(w => w.InvoicesId == iPreviousId).ToList();
                int index = 0;
                foreach (OrderDetailInvoices item in odisPrevious)
                {
                    index++;
                    item.ItemSort = index;
                    db.Entry(item).State = EntityState.Modified;
                }
                // OrderDetailInvoices new entry
                OrderDetailInvoices virtualStartOdi = new OrderDetailInvoices
                {
                    OrderDetail = odi.OrderDetail,
                    OrderDetailIgredients = odi.OrderDetailIgredients,
                    StaffId = startOdi.StaffId,
                    PosInfoDetailId = startOdi.PosInfoDetailId,
                    Counter = startOdi.Counter,
                    CreationTS = startOdi.CreationTS,
                    IsPrinted = startOdi.IsPrinted,
                    CustomerId = startOdi.CustomerId,
                    InvoicesId = startOdi.InvoicesId,
                    IsDeleted = startOdi.IsDeleted,
                    OrderDetailIgredientsId = odi.OrderDetailIgredientsId,
                    Price = startOdi.Price,
                    Discount = odi.Discount,
                    Net = odi.Net,
                    VatRate = startOdi.VatRate,
                    VatAmount = odi.VatAmount,
                    VatId = startOdi.VatId,
                    TaxId = startOdi.TaxId,
                    VatCode = startOdi.VatCode,
                    TaxAmount = odi.TaxAmount,
                    Qty = odi.Qty,
                    Total = odi.Total,
                    PricelistId = startOdi.PricelistId,
                    ProductId = startOdi.ProductId,
                    Description = startOdi.Description,
                    ItemCode = startOdi.ItemCode,
                    ItemRemark = startOdi.ItemRemark,
                    InvoiceType = startOdi.InvoiceType,
                    TableId = startOdi.TableId,
                    TableCode = startOdi.TableCode,
                    RegionId = startOdi.RegionId,
                    OrderNo = startOdi.OrderNo,
                    OrderId = startOdi.OrderId,
                    IsExtra = startOdi.IsExtra,
                    PosInfoId = startOdi.PosInfoId,
                    EndOfDayId = startOdi.EndOfDayId,
                    Abbreviation = startOdi.Abbreviation,
                    DiscountId = startOdi.DiscountId,
                    SalesTypeId = startOdi.SalesTypeId,
                    ProductCategoryId = startOdi.ProductCategoryId,
                    CategoryId = startOdi.CategoryId,
                    ItemPosition = startOdi.ItemPosition,
                    ItemSort = index + modifiedCounter,
                    ItemRegion = startOdi.ItemRegion,
                    RegionPosition = startOdi.RegionPosition,
                    ItemBarcode = startOdi.ItemBarcode,
                    TotalBeforeDiscount = odi.TotalBeforeDiscount,
                    TotalAfterDiscount = odi.TotalAfterDiscount,
                    ReceiptSplitedDiscount = odi.ReceiptSplitedDiscount
                };
                db.OrderDetailInvoices.Add(virtualStartOdi);
            }
        }


        private IEnumerable<TransferToPms> CreateTransferToPMS(ReceiptPayments payment, Receipts r, Transactions tr, Invoices i, bool isFiscal, long? hotelId)
        {
            List<TransferToPms> tpms = new List<TransferToPms>();
            try
            {
                if (r.ModifyOrderDetails == Symposium.Models.Enums.ModifyOrderDetailsEnum.PayOffOnly && r.Total < r.ReceiptDetails.Sum(s => s.ItemGross))
                {
                    logger.Info("Fixing Payoff Details for receipt id {0}" + r.Id);
                    var detailsFromDB = db.OrderDetailInvoices.Where(w => w.InvoicesId == r.Id).OrderBy(s => s.ItemSort).ToList();
                    foreach (var item in detailsFromDB)
                    {
                        r.ReceiptDetails.FirstOrDefault(w => w.ItemSort == item.ItemSort && w.OrderDetailId == item.OrderDetailId).ItemGross = item.Total;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            var mappedDepartmentsQuery = (from q in r.ReceiptDetails
                                          join qqq in db.TransferMappings.Where(w => w.PosDepartmentId == r.DepartmentId && w.HotelId == hotelId)
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
            Guest guest = null;

            List<string> customersIdStr = new List<string>();
            List<string> reservationIdStr = new List<string>();
            List<GuestGetIdModel> guestProfileRoomList = new List<GuestGetIdModel>();
            Dictionary<int, int> guestDict = new Dictionary<int, int>();
            //Get GuestIds in case of Null
            foreach (ReceiptPayments rec in r.ReceiptPayments)
            {
                if (rec.AccountType == 3)
                {
                    if (rec.GuestId == null)
                    {
                        GuestGetIdModel tmpModel = new GuestGetIdModel();
                        tmpModel.ProfileNo = (int)rec.ProfileNo;
                        tmpModel.RoomId = (int)rec.RoomId;
                        guestProfileRoomList.Add(tmpModel);
                    }
                }
            }

            if (guestProfileRoomList.Count > 0)
            {
                foreach (GuestGetIdModel model in guestProfileRoomList)
                {
                    iHotelFlows = Resolve<IHotelFlows>();
                    int guestId = iHotelFlows.GetGuestIdFromProfileAndRoom(DBInfo, model);
                    guestDict.Add(model.ProfileNo, guestId);
                }
            }

            foreach (ReceiptPayments rec in r.ReceiptPayments)
            {
                if (rec.AccountType == 3)
                {
                    if (rec.GuestId == null)
                    {
                        customersIdStr.Add(guestDict[(int)rec.ProfileNo].ToString());
                        rec.GuestId = guestDict[(int)rec.ProfileNo];
                        payment.GuestId = rec.GuestId;
                    }
                    else
                    {
                        customersIdStr.Add(rec.GuestId.ToString());
                    }
                    reservationIdStr.Add(rec.ReservationCode.ToString());
                }
            }

            HotelInfo hi = db.HotelInfo.FirstOrDefault();
            if (payment.GuestId != null && payment.GuestId != 0)
            {
                guest = db.Guest.Where(w => w.Id == payment.GuestId).FirstOrDefault();
                PosReservationHelper helper = new PosReservationHelper();
                helper.HotelId = (int)hi.MPEHotel;
                helper.Name = "";
                helper.Room = guest.Room;
                helper.ReservationId = "";
                helper.Page = 0;
                helper.Pagesize = 200;
                helper.PosDepartmentId = (int)r.DepartmentId;
                iHotelFlows = Resolve<IHotelFlows>();
                List<CustomerModel> PMSCustomer = iHotelFlows.GetReservationsNew(DBInfo, helper, (int)hi.Id, null);
                if (PMSCustomer != null)
                {
                    foreach (CustomerModel customer in PMSCustomer)
                    {
                        if (guest.ProfileNo == customer.ProfileNo && guest.ReservationId != customer.OriginalReservationId)
                        {
                            guest.ReservationId = customer.OriginalReservationId;
                        }
                    }
                }
            }

            var accExtra = db.EODAccountToPmsTransfer.Include("Accounts").Where(w => w.PosInfoId == i.PosInfoId && w.AccountId == tr.AccountId).FirstOrDefault();
            var IsNotSendingTransfer = (accExtra != null && (accExtra.Accounts.SendsTransfer ?? false));
            var IsCreditCard = payment.AccountType == 4;
            var points = r.Points;
            if (tr.Amount == 0)
                return null;
            //If is payoff only mark as true else check if is fiscal and mark as false for extec
            var markRoomCharge = Symposium.Models.Enums.ModifyOrderDetailsEnum.ChangePaymentType == r.ModifyOrderDetails || isFiscal == false || (isFiscal == true && Symposium.Models.Enums.ModifyOrderDetailsEnum.PayOffOnly == r.ModifyOrderDetails && r.IsPrinted);
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
                SendTransferRepository stRepository = new SendTransferRepository(db, payment.PosInfoId.Value, mappedDepartmentsQuery.FirstOrDefault().PosDepartmentId.Value);

                foreach (var mdq in mappedDepartmentsQuery)
                {

                    TransferToPms ttp = new TransferToPms();
                    switch ((AccountType)payment.AccountType)
                    {
                        case AccountType.Charge:
                            ttp = stRepository.WriteRoomChargeToTransfer(mdq.ReceiptCounter.Value, mdq.PmsDepartmentId, mdq.PmsDepartment, mdq.PosInfoDetailId.Value,
                                                                             guest.ProfileNo.Value.ToString(), guest.FirstName + " " + guest.LastName, guest.ReservationId.ToString(),
                                                                             guest.RoomId.ToString(), guest.Room, Math.Round((Decimal)((mdq.Total / perc) * 100), 2), markRoomCharge, payment.PMSPaymentId, r.PMSInvoiceId, hotelId);
                            tr.TransferToPms.Add(ttp);
                            tpms.Add(ttp);
                            break;
                        case AccountType.Barcode:
                        case AccountType.Voucher:
                        case AccountType.CreditCard:
                        case AccountType.TicketCompliment:
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
                                                                           r.ModifyOrderDetails == Symposium.Models.Enums.ModifyOrderDetailsEnum.ChangePaymentType, payment.PMSPaymentId, r.PMSInvoiceId, IsNotSendingTransfer, hotelId);
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
                }
            }
            else
            {
                //TODO: Notify User
                string msg = "No department found for products in Receipt {0}" + r.ReceiptNo;
            }
            return tpms;
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

        private CreditTransactions CreateCreditTransaction(Receipts r, ReceiptPayments rp, Invoices i)
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

            CreditTransactions ct = new CreditTransactions()
            {
                CreditCodeId = rp.CreditCodeId,
                CreditAccountId = rp.CreditAccountId,
                CreationTS = DateTime.Now,
                Type = type,
                Description = descr,
                Amount = amount,
                StaffId = r.StaffId,
                PosInfoId = rp.PosInfoId,
                Invoices = i
            };
            return ct;
        }

        private Transactions CreateTransactionsFromReceiptPayments(Receipts r, ReceiptPayments rp, Invoices i)
        {
            Invoice_Guests_Trans igt = new Invoice_Guests_Trans();
            var hotelid = 1;
            Transactions tr = new Transactions
            {
                AccountId = rp.AccountId,
                Amount = rp.Amount,
                DepartmentId = r.DepartmentId,
                Description = "Pay Off",
                ExtDescription = "CreateTransactionsFromReceiptPayments",
                StaffId = r.StaffId,
                TransactionType = rp.TransactionType != null ? rp.TransactionType : GetTransactionType((InvoiceTypesEnum)r.InvoiceTypeType),
                PosInfoId = rp.PosInfoId,
                InOut = GetTransactionInOut((InvoiceTypesEnum)r.InvoiceTypeType),
                Day = i.Day,
                EndOfDayId = r.EndOfDayId == 0 ? null : r.EndOfDayId
            };

            switch ((AccountType)rp.AccountType)
            {
                case AccountType.Cash:
                    if (rp.GuestId != null && rp.GuestId != 0)
                    {
                        var isCheckedIn = CheckCustomerCheckedInStatus(rp.GuestId ?? 0, rp.HotelId ?? 0, rp.Room);
                        if (!isCheckedIn)
                            throw new BusinessException(Symposium.Resources.Errors.ROOMCHARGEONCHECKEDOUTGUESTDENIED);
                        if (r.ModifyOrderDetails == Symposium.Models.Enums.ModifyOrderDetailsEnum.PayOffOnly)
                        {

                            igt = db.Invoice_Guests_Trans.FirstOrDefault(x => x.InvoiceId == i.Id);

                            if (igt == null)
                            {
                                igt = new Invoice_Guests_Trans();
                                igt.GuestId = rp.GuestId;
                                igt.Invoices = i;
                                tr.Invoice_Guests_Trans.Add(igt);
                            }
                            else
                            {
                                igt.GuestId = rp.GuestId;
                                tr.Invoice_Guests_Trans.Add(igt);
                                db.Entry(igt).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            igt.GuestId = rp.GuestId;
                            if (r.CreateTransactions == true)
                            {
                                i.Invoice_Guests_Trans.Add(igt);
                                tr.Invoice_Guests_Trans.Add(igt);
                            }
                            else
                                i.Invoice_Guests_Trans.Add(igt);
                        }
                    }
                    break;
                case AccountType.CreditCard: break;
                case AccountType.Barcode:
                    //WaterParkLogic
                    break;
                case AccountType.Voucher:
                    break;
                case AccountType.Allowence:
                case AccountType.Coplimentary:
                case AccountType.Charge:
                    if (rp.GuestId != null)
                    {
                        var isCheckedIn = CheckCustomerCheckedInStatus(rp.GuestId ?? 0, rp.HotelId ?? 0, rp.Room);
                        if (!isCheckedIn)
                            throw new BusinessException(Symposium.Resources.Errors.ROOMCHARGEONCHECKEDOUTGUESTDENIED);
                        if (r.ModifyOrderDetails == Symposium.Models.Enums.ModifyOrderDetailsEnum.PayOffOnly)
                        {

                            igt = db.Invoice_Guests_Trans.FirstOrDefault(x => x.InvoiceId == i.Id);

                            if (igt == null)
                            {
                                igt = new Invoice_Guests_Trans();
                                igt.GuestId = rp.GuestId;
                                igt.Invoices = i;
                                tr.Invoice_Guests_Trans.Add(igt);
                            }
                            else
                            {
                                igt.GuestId = rp.GuestId;
                                tr.Invoice_Guests_Trans.Add(igt);
                                db.Entry(igt).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            igt.GuestId = rp.GuestId;
                            if (r.CreateTransactions == true)
                            {
                                i.Invoice_Guests_Trans.Add(igt);
                                tr.Invoice_Guests_Trans.Add(igt);
                            }
                            else
                                i.Invoice_Guests_Trans.Add(igt);
                        }
                    }
                    break;
                default:
                    break;
            }

            if ((rp.CreditAccountId ?? 0) != 0)
            {
                if (rp.CreditTransactionAction != (short)CreditTransactionType.None)
                {
                    tr.CreditTransactions.Add(CreateCreditTransaction(r, rp, i));
                }
            }

            var pi = db.PosInfo.FirstOrDefault(x => x.Id == i.PosInfoId);
            var isFiscal = pi != null ? (pi.FiscalType != (int)Models.ExtecrModels.FiscalTypeEnum.Generic) : false;



            HotelInfo hi = rp.HotelId == null ? db.HotelInfo.FirstOrDefault() : db.HotelInfo.FirstOrDefault(x => x.HotelId == rp.HotelId);
            if (hi != null)
            {
                if (hi != null && hi.Type == 0 || hi.Type == 10 || hi.Type == 4)
                {
                    var tpmslist = new List<TransferToPms>();
                    if (tr.Amount != 0 && r.CreateTransactions == true)
                    {
                        if (r.InvoiceTypeType == 3)
                            isFiscal = false;

                        //  If customer of receipt payment is not checked-out, create transfer to pms else do not create
                        if (!rp.isCheckedOut)
                        {
                            tpmslist = CreateTransferToPMS(rp, r, tr, i, isFiscal, hi.HotelId).ToList();
                            foreach (var ttpms in tpmslist)
                            {
                                tr.TransferToPms.Add(ttpms);
                            }
                        }
                        else
                        {
                            logger.Info("Skipping insert to PMS for checked-out client " + rp.ProfileNo + ", order " + tr.OrderId);
                        }
                    }
                }
            }
            return tr;
        }

        public bool ValidateReceipt(ref Receipts r)
        {
            if (r.PdaModuleId != null)
            {
                if ((r.Discount ?? 0) > 0 && r.ReceiptPayments.Sum(s => s.Amount) > r.Total)
                {
                    r.ReceiptPayments.FirstOrDefault().Amount -= r.Discount;
                    // throw new Exception(string.Format("Payment exceeds invoice total. Payment Amount {0} reduced by {1}, New PaidTotal {2}", r.ReceiptPayments.Sum(s => s.Amount) + r.Discount, r.Discount, r.ReceiptPayments.Sum(s => s.Amount)));
                }
            }
            switch ((ModifyOrderDetailsEnum)r.ModifyOrderDetails)
            {
                case ModifyOrderDetailsEnum.FromScratch:

                    break;
                case ModifyOrderDetailsEnum.FromOtherUnmodified:
                case ModifyOrderDetailsEnum.FromOtherUpated:
                    var ods = r.ReceiptDetails.Select(s => s.OrderDetailId);
                    var details = db.OrderDetail.Where(w => ods.Contains(w.Id)).ToList();
                    if (r.InvoiceTypeType != 2 && r.InvoiceTypeType != 3 && r.InvoiceTypeType != 8)
                    {
                        var isInvoiced = details.Any(w => w.PaidStatus > 0);
                        if (isInvoiced == true)
                        {
                            throw new Exception(string.Format("Items allready invoiced.Invoice Id {0} ", r.Id));
                            return false;
                        }
                    }
                    break;
                case ModifyOrderDetailsEnum.PayOffOnly:
                    var receiptId = r.Id;
                    var inv = db.Invoices.Include(i => i.Transactions).FirstOrDefault(w => w.Id == receiptId);
                    if (inv != null)
                    {
                        var newPayments = r.ReceiptPayments.Sum(sm => sm.Amount);
                        var allreadyPaid = inv.Transactions.Sum(sm => sm.Amount);
                        if (newPayments + allreadyPaid > inv.Total)
                        {
                            throw new Exception(string.Format("Payment exceeds invoice total. Invoice Total {0} Current PaidTotal {1}, New PaidTotal {2}", r.Total, allreadyPaid, newPayments));
                            return false;
                        }
                    }
                    break;
                default:
                    break;
            }


            var nullProductCategoriesList = r.ReceiptDetails.Where(w => (w.ProductCategoryId ?? 0) == 0);

            if (nullProductCategoriesList != null)
            {

                foreach (var item in nullProductCategoriesList)
                {
                    if (item.IsExtra == 1)
                    {
                        logger.Info("Found extra{0}." + item.ItemDescr);
                        var masterItem = r.ReceiptDetails.FirstOrDefault(x => x.IsExtra == 0 && x.OrderDetailId == item.OrderDetailId);
                        if (masterItem != null)
                        {
                            item.ProductCategoryId = masterItem.ProductCategoryId;
                            item.SalesTypeId = masterItem.SalesTypeId;
                            logger.Error("New ProductCategoryId {0}." + masterItem.ProductCategoryId);
                        }
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// add new Invoice, Order,InvoiceShippingDetails,OrderDetail,OrderDetailInvoices   to Db for new receipt
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="detailStatus">
        /// νέα απόδειξη ......... FromScratch = 0,
        /// Σε αρχικό ΔΠ  θέλουμε να εκδόσουμε απόδειξη (με εξόφληση ή χωρίς εξόφληση) χωρίς αλλαγή σε αυτό .........FromOtherUnmodified = 1,
        /// Σε αρχικό ΔΠ θέλουμε να εκδόσουμε απόδειξη (με εξόφληση ή χωρίς εξόφληση) αλλά με αλλαγή σε αυτό (πχ εισαγωγή έκπτωση) ......... FromOtherUpated = 2,
        /// Σε αρχική απόδειξη χωρίς εξόφληση θέλουμε να την εξοφλήσουμε .........PayOffOnly = 3,
        /// Αλλαγή τρόπου πληρωμής ChangePaymentType = 4
        /// <param name="externalCommunication"></param>
        /// <returns></returns>
        private Invoices CreateInvoiceFromReceipt(Receipts receipt, ModifyOrderDetailsEnum detailStatus, out bool externalCommunication)
        {
            externalCommunication = true;
            Invoices newInvoice;
            var paidstatus = 1;

            ValidateReceipt(ref receipt);
            Order newOrder = new Order();
            if (detailStatus != ModifyOrderDetailsEnum.PayOffOnly)
            {
                var pi = db.PosInfo.FirstOrDefault(w => w.Id == receipt.PosInfoId);
                var isFiscal = pi != null ? pi.FiscalType == 2 : false;
                if (receipt.Day == null)
                {
                    receipt.Day = DateTime.Now;
                }
                newInvoice = new Invoices
                {
                    Day = (receipt.EndOfDayId == null) ? DateTime.Now : receipt.Day,
                    Description = receipt.InvoiceDescription,
                    Cover = receipt.Cover,
                    Counter = receipt.ReceiptNo,
                    PosInfoDetailId = receipt.PosInfoDetailId,
                    PosInfoId = receipt.PosInfoId,
                    StaffId = receipt.StaffId,
                    TableId = receipt.TableId,
                    Total = receipt.Total,
                    Discount = receipt.Discount,
                    DiscountRemark = receipt.DiscountRemark,
                    ClientPosId = receipt.ClientPosId,
                    PdaModuleId = receipt.PdaModuleId,
                    InvoiceTypeId = receipt.InvoiceTypeId,
                    Net = receipt.Net,
                    Vat = receipt.Vat,
                    TableSum = receipt.TableSum,
                    CashAmount = receipt.CashAmount,
                    BuzzerNumber = receipt.BuzzerNumber,
                    EndOfDayId = receipt.EndOfDayId,
                    LoyaltyDiscount = receipt.LoyaltyDiscount,
                    IsPrinted = isFiscal ? false : receipt.IsPrinted,
                    IsVoided = receipt.InvoiceTypeType == 3 || receipt.InvoiceTypeType == 8,
                    ForeignExchangeCurrency = receipt.ForeignExchangeCurrencyTo,
                    ForeignExchangeTotal = receipt.ForeignExchangeTotal,
                    ForeignExchangeDiscount = receipt.ForeignExchangeDiscount
                };
                newInvoice.PaidTotal = newInvoice.PaidTotal ?? 0;

                InvoiceShippingDetails isd = new InvoiceShippingDetails
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
                    ShippingZipCode = receipt.ShippingZipCode
                };
                if (receipt.CreateTransactions == false)
                {
                    if ((receipt.ReceiptPayments.Count() > 0) && (receipt.ReceiptPayments.FirstOrDefault().GuestId != null))
                    {
                        var i = receipt.ReceiptPayments.FirstOrDefault().GuestId;
                        var g = db.Guest.FirstOrDefault(f => f.Id == i);
                        if (g != null)
                        {
                            isd.CustomerName = g.LastName + " " + g.FirstName;
                        }
                        // submition on new deliveryCustomer Agent to get  customer name for ΔΠ
                    }
                    else if (!String.IsNullOrEmpty(receipt.CustomerName))
                    {
                        isd.CustomerName = receipt.CustomerName;
                    }
                }
                else
                {
                    isd.CustomerName = receipt.CustomerName;
                }
                newInvoice.InvoiceShippingDetails.Add(isd);

                StringBuilder hashsting = new StringBuilder(receipt.Day.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                hashsting.Append(receipt.PosInfoId.Value);
                hashsting.Append(receipt.PosInfoDetailId.Value);
                hashsting.Append(receipt.InvoiceTypeType.Value);
                hashsting.Append(receipt.Total.Value.ToString("000000000.00", CultureInfo.GetCultureInfo("en-US")));
                hashsting.Append(receipt.StaffId);
                hashsting.Append(receipt.ReceiptDetails.Count());
                hashsting.Append(receipt.InvoicesAA);
                if (receipt.ReceiptDetails != null)
                {
                    foreach (var item in receipt.ReceiptDetails)
                    {
                        hashsting.Append(item.ItemDescr);
                        hashsting.Append(item.ItemQty);
                    }
                }
                newInvoice.Hashcode = MD5Helper.GetMD5Hash(hashsting.ToString()) + receipt.Day.Value.ToString("-yyyyMMdd");
                var hashcodeExists = db.Invoices.FirstOrDefault(x => x.Hashcode == newInvoice.Hashcode);
                if (hashcodeExists != null)
                {
                    if (receipt.ExtType == null)
                    {
                        throw new Exception("Invoice with the Same hashCode Exists to DataBase. If this is a false error on Day change the value of seconds!");
                    }
                    else
                    {
                        externalCommunication = false;
                        return null;
                    }
                }
                List<string> customersIdStr = new List<string>();
                List<string> reservationIdStr = new List<string>();
                List<GuestGetIdModel> guestProfileRoomList = new List<GuestGetIdModel>();
                Dictionary<int, int> guestDict = new Dictionary<int, int>();
                //Get GuestIds in case of Null
                foreach (ReceiptPayments rec in receipt.ReceiptPayments)
                {
                    if (rec.AccountType == 3 || rec.AccountType ==1)
                    {
                        if (rec.GuestId == null)
                        {
                            GuestGetIdModel tmpModel = new GuestGetIdModel();
                            tmpModel.ProfileNo =rec.ProfileNo != null ?  (int)rec.ProfileNo : 0;
                            tmpModel.RoomId = rec.RoomId !=null ?  (int)rec.RoomId : 0;
                            guestProfileRoomList.Add(tmpModel);
                        }
                    }
                }

                if(guestProfileRoomList.Count > 0)
                {
                    foreach(GuestGetIdModel model in guestProfileRoomList)
                    {
                        iHotelFlows = Resolve<IHotelFlows>();
                        int guestId = iHotelFlows.GetGuestIdFromProfileAndRoom(DBInfo, model);
                        guestDict.Add(model.ProfileNo, guestId);
                    }
                }

                foreach (ReceiptPayments rec in receipt.ReceiptPayments)
                {
                    if (rec.AccountType == 3 || rec.AccountType == 1)
                    {
                        if (rec.GuestId == null)
                        {
                            customersIdStr.Add(guestDict[rec.ProfileNo != null ? (int)rec.ProfileNo : 0].ToString());
                            rec.GuestId = guestDict[rec.ProfileNo != null ? (int)rec.ProfileNo : 0];
                        }
                        else
                        {
                            customersIdStr.Add(rec.GuestId.ToString());
                        }
                        reservationIdStr.Add(rec.ReservationCode.ToString());
                    }
                }
                string joinedCustomerIds = string.Join(",", customersIdStr);
                string joinedReservationIds = string.Join(",", reservationIdStr);
                newInvoice.CustomersId = joinedCustomerIds;
                newInvoice.ReservationsId = joinedReservationIds;
                // New Invoice Fields
                if (receipt.InvoiceTypeType == 2)
                {
                    newInvoice.IsPaid = 0;
                    newInvoice.PaidTotal = 0;
                }
                else
                {
                    if (receipt.CreateTransactions == false)
                    {
                        newInvoice.IsPaid = 0;
                    }
                    else
                    {
                        newInvoice.PaidTotal = receipt.ReceiptPayments != null ? receipt.ReceiptPayments.Sum(sm => sm.Amount) : 0;
                        if (newInvoice.Total - newInvoice.PaidTotal > 0) newInvoice.IsPaid = 1; else newInvoice.IsPaid = 2;
                    }
                }
                newInvoice.PaymentsDesc = receipt.ReceiptPayments != null && receipt.ReceiptPayments.Count() > 0 ? receipt.ReceiptPayments.Select(s => s.AccountDescription).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) : "";
                //Trim Column Rooms in Invoices
                string rooms = receipt.ReceiptPayments != null && receipt.ReceiptPayments.Count() > 0 ? receipt.ReceiptPayments.Select(s => s.Room).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) : "";
                newInvoice.Rooms = rooms.Trim();
                receipt.IsPaid = newInvoice.IsPaid;
                if (detailStatus == ModifyOrderDetailsEnum.FromScratch)
                    newInvoice.OrderNo = receipt.ReceiptDetails.Select(s => s.OrderNo).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Trim().Remove(0, 1);
                else
                    newInvoice.OrderNo = receipt.OrderNo.Trim();
                if (detailStatus == ModifyOrderDetailsEnum.FromScratch)
                {
                    newOrder.Day = newInvoice.Day;
                    newOrder.PosId = newInvoice.PosInfoId;
                    newOrder.PosInfoDetailId = newInvoice.PosInfoDetailId;
                    newOrder.ReceiptNo = newInvoice.Counter;
                    newOrder.OrderNo = receipt.ReceiptDetails.FirstOrDefault().OrderNo;
                    newOrder.Total = receipt.Total;
                    newOrder.StaffId = receipt.StaffId;
                    newOrder.Discount = receipt.Discount;
                    newOrder.TotalBeforeDiscount = receipt.Total + (receipt.Discount ?? 0);
                    newOrder.PdaModuleId = receipt.PdaModuleId;
                    newOrder.ClientPosId = receipt.ClientPosId;
                    newOrder.IsDeleted = false;
                    if (receipt.MacroGuidId != null)
                    {
                        newOrder.Couver = receipt.Cover;
                        newOrder.MacroGuidId = receipt.MacroGuidId;
                        newOrder.CouverAdults = receipt.CouverAdults;
                        newOrder.CouverChildren = receipt.CouverChildren;
                    }
                    else
                    {
                        newOrder.Couver = receipt.Cover;
                    }
                    newOrder.OrderOrigin = receipt.OrderOrigin;
                    newOrder.ExtKey = receipt.ExtKey; 
                    newOrder.EstTakeoutDate = receipt.EstTakeoutDate; // Added for Delivery
                    newOrder.IsDelay = receipt.IsDelay;  // Added for Delivery
                    newOrder.ExtType = receipt.ExtType; 
                 
                        newOrder.ExtObj = receipt.ExtObj;
                    newOrder.OrderNotes = receipt.OrderNotes;
                    db.Order.Add(newOrder);
                }
                OrderDetail previousOd = null;
                int modifiedCounter = 0;
                foreach (var det in receipt.ReceiptDetails)
                {
                    if (detailStatus != ModifyOrderDetailsEnum.FromScratch)
                    {
                        if (newOrder.Id != det.OrderId)
                        {
                            newOrder = db.Order.Find(det.OrderId);
                        }
                    }
                    bool hasPayments = receipt.CreateTransactions;
                    modifiedCounter++;
                    OrderDetailInvoices odi = CreateOrderDetailInvoicesFromReceiptDetails(det, newInvoice, receipt.Abbreviation, detailStatus, previousOd, newOrder, hasPayments, modifiedCounter);
                    newInvoice.OrderDetailInvoices.Add(odi);
                    if (det.IsExtra == 0) { previousOd = odi.OrderDetail; }
                    //else
                    //{
                    //    previousOd.OrderDetailIgredients.Add(CreateOrderDetailIngredient(det, previousOd, detailStatus));
                    //}
                }
            }
            else
            {
                newInvoice = db.Invoices.Where(w => w.Id == receipt.Id).FirstOrDefault();
                if (newInvoice != null)
                {
                    receipt.PaidTotal = receipt.ReceiptPayments.Sum(sm => sm.Amount);
                    if (receipt.InvoiceTypeType == 2)
                    {
                        newInvoice.IsPaid = 0;
                    }
                    else
                    {
                        if (receipt.CreateTransactions == false)
                        {
                            newInvoice.IsPaid = 0;
                        }
                        else
                        {
                            if (newInvoice.PaidTotal + receipt.PaidTotal >= newInvoice.Total)
                                newInvoice.PaidTotal = newInvoice.Total;
                            else
                                newInvoice.PaidTotal += receipt.PaidTotal;
                            if (newInvoice.Total - newInvoice.PaidTotal > 0)
                                newInvoice.IsPaid = 1;
                            else
                                newInvoice.IsPaid = 2;
                        }
                    }
                    newInvoice.PaymentsDesc = receipt.ReceiptPayments != null && receipt.ReceiptPayments.Count() > 0 ?
                        receipt.ReceiptPayments.Select(s => s.AccountDescription).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) : "";

                    newInvoice.Rooms = receipt.ReceiptPayments != null && receipt.ReceiptPayments.Count() > 0 ?
                        receipt.ReceiptPayments.Select(s => s.Room).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1) : "";

                    receipt.IsPaid = newInvoice.IsPaid;
                    receipt.IsPrinted = newInvoice.IsPrinted ?? true;
                    newInvoice.StaffId = receipt.StaffId;

                    OrderDetail previousOd = null;
                    int modifiedCounter = 0;
                    foreach (var det in receipt.ReceiptDetails)
                    {
                        if (det.SelectedQuantity != null)
                        {
                            newOrder = db.Order.Find(det.OrderId);
                            bool hasPayments = true;
                            modifiedCounter++;
                            OrderDetailInvoices odi = CreateOrderDetailInvoicesFromReceiptDetails(det, newInvoice, receipt.Abbreviation, detailStatus, previousOd, newOrder, hasPayments, modifiedCounter);
                            newInvoice.OrderDetailInvoices.Add(odi);
                            if (det.IsExtra == 0) { previousOd = odi.OrderDetail; }
                        }
                    }

                    db.Entry(newInvoice).State = EntityState.Modified;
                }
            }
            if (receipt.ReceiptPayments != null)
            {
                foreach (var pms in receipt.ReceiptPayments)
                {
                    Transactions paymentTransaction = CreateTransactionsFromReceiptPayments(receipt, pms, newInvoice);
                    if (detailStatus == ModifyOrderDetailsEnum.PayOffOnly)
                    {
                        paymentTransaction.InvoicesId = newInvoice.Id;
                        db.Transactions.Add(paymentTransaction);
                    }
                    else
                    {
                        if (receipt.CreateTransactions == true)
                            newInvoice.Transactions.Add(paymentTransaction);
                    }
                }
                paidstatus = DeterminePaidStatus(receipt);
            }

            InvoiceStatusUpdate(receipt, paidstatus, detailStatus, newOrder);

            return newInvoice;
        }

        private byte DeterminePaidStatus(Receipts r)
        {
            byte res = 0;
            switch ((InvoiceTypesEnum)r.InvoiceTypeType)
            {
                case InvoiceTypesEnum.Order:
                    res = 0;
                    break;
                case InvoiceTypesEnum.Receipt:
                case InvoiceTypesEnum.Timologio:
                case InvoiceTypesEnum.Void:
                case InvoiceTypesEnum.Coplimentary:
                case InvoiceTypesEnum.Allowance:
                case InvoiceTypesEnum.VoidOrder:
                case InvoiceTypesEnum.Pistosi:
                    //if (r.ReceiptPayments.Count() > 0)
                    if (r.CreateTransactions == true)
                        res = 2;
                    else
                        res = 1;
                    break;
                case InvoiceTypesEnum.PaymentReceipt:
                    break;
                case InvoiceTypesEnum.RefundReceipt:
                    break;
                default:
                    break;
            }
            return res;
        }


        /// <summary>
        /// Marks a non printed receipt as deleted
        /// </summary>
        /// <param name="id"></param>
        public long? MarkInvoiceAsDeleted(long id)
        {
            long? tblId = null;
            var inv = db.Invoices.Include(i => i.OrderDetailInvoices)
                                 .Include(i => i.Transactions.Select(s => s.TransferToPms))
                                 .FirstOrDefault(f => f.Id == id);
            if (inv != null)
            {
                tblId = inv.TableId;
                inv.IsDeleted = true;
                foreach (var odi in inv.OrderDetailInvoices)
                {
                    odi.IsDeleted = true;
                    db.Entry(odi).State = EntityState.Modified;
                    var relatedOdi = db.OrderDetailInvoices.Include(i => i.Invoices).Include(i => i.OrderDetail).Where(w => w.OrderDetailId == odi.OrderDetailId && w.Id != odi.Id).ToList();
                    if (relatedOdi != null)
                    {
                        foreach (var rel in relatedOdi)
                        {
                            //If related invoice = Order then flags as not invoiced and returns OrderDetails to initial status
                            if (rel.InvoiceType == 2)
                            {
                                rel.OrderDetail.PaidStatus = 0;
                                db.Entry(rel.OrderDetail).State = EntityState.Modified;
                                rel.Invoices.IsInvoiced = false;
                                db.Entry(rel.Invoices).State = EntityState.Modified;
                            }
                        }
                    }
                }
                foreach (var t in inv.Transactions)
                {
                    t.IsDeleted = true;
                    foreach (var mdq in t.TransferToPms.ToList())
                    {
                        //if TransferToPMS has Pms Response creates opposite rec
                        // Remove if in order to create opposite rec when extexr is connected and printer(opos) is not 05/09/2018
                        // if something goes wrong uncomment the if
                        //if (!string.IsNullOrEmpty(mdq.PmsResponseId))
                        //{
                        TransferToPms ttp = new TransferToPms();

                        SendTransferRepository stRepository = new SendTransferRepository(db, t.PosInfoId.Value, t.DepartmentId.Value);
                        if (mdq.ProfileId == null)
                        {
                            ttp = stRepository.WriteCashToTransfer(Convert.ToInt64(mdq.ReceiptNo),
                                                                   mdq.PmsDepartmentId,
                                                                   mdq.PmsDepartmentDescription,
                                                                   mdq.PosInfoDetailId.Value,
                                                                   mdq.ProfileName,
                                                                   mdq.RoomDescription,
                                                                   (decimal)mdq.Total * -1,
                                                                   true,
                                                                   true,
                                                                   mdq.PMSPaymentId,
                                                                   mdq.PMSInvoiceId,
                                                                   mdq.SendToPMS ?? false,
                                                                   mdq.HotelId);
                        }
                        else
                        {
                            ttp = stRepository.WriteRoomChargeToTransfer(Convert.ToInt64(mdq.ReceiptNo),
                                                                         mdq.PmsDepartmentId,
                                                                         mdq.PmsDepartmentDescription,
                                                                         mdq.PosInfoDetailId.Value,
                                                                         mdq.ProfileId,
                                                                         mdq.ProfileName,
                                                                         mdq.RegNo,
                                                                         mdq.RoomId,
                                                                         mdq.RoomDescription,
                                                                         (decimal)mdq.Total * -1,
                                                                         true,
                                                                         mdq.PMSPaymentId,
                                                                         mdq.PMSInvoiceId,
                                                                         mdq.HotelId);

                        }
                        ttp.Transactions = t;
                        db.TransferToPms.Add(ttp);
                        //}
                    }
                    db.Entry(t).State = EntityState.Modified;
                }
                db.Entry(inv).State = EntityState.Modified;

            }

            return tblId;
        }
        //Loyalty insert into Actions Table 
        public bool InsertLoyaltyActions(Receipts receipt, HotelInfo hi)
        {
            if (receipt.Points != 0)
            {
                Guest guest = new Guest();
                foreach (ReceiptPayments value in receipt.ReceiptPayments)
                {
                    guest = db.Guest.FirstOrDefault(g => g.Id == value.GuestId);
                }
                if (guest.ProfileNo != null)
                {
                    logger.Info("Customer with id: " + guest.ProfileNo + " complete's points transaction in Action");
                    PMSConnection pmsconnActions = new PMSConnection();
                    string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName + ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                    pmsconnActions.initConn(connStr);
                    string notes = "";
                    int points = 0;
                    points = -receipt.Points;
                    string arrival = guest.Arrival.Substring(0, 10);
                    string daparture = guest.Departure.Substring(0, 10);
                    notes = "Reservation Id: " + guest.ReservationId + " Arrival : " + arrival + " Departure : " + daparture + " Voucher : " + guest.ReservationCode;
                    var actionDetails = pmsconnActions.InsertActions(hi.DBUserName, guest.ProfileNo, receipt.Day, 2, points, notes);
                }
                logger.Info("No customer found for add points into Action");
            }
            else
            {
                logger.Info("Points = 0");
            }
            return true;
        }

        public bool CheckCustomerCheckedInStatus(long GuestId, long HotelId, string Room)
        {
            /// Customers came from Hotelizer. Always true because this is not our system. His problem
            if (MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi"))
                return true;

            logger.Info("Getting check-in status of customer with GuestId: " + GuestId + ", HotelId:" + HotelId + ", Room:" + Room);
            var found = false;
            try
            {
                HotelInfo hotelInfo = db.HotelInfo.Where(x => x.HotelId == HotelId).FirstOrDefault();
                if (hotelInfo == null)
                {
                    hotelInfo = db.HotelInfo.FirstOrDefault();
                }
                if (hotelInfo != null)
                {
                    //if (HotelId == 0)
                    //{
                    //    HotelId = hotelInfo.HotelId ?? 0;
                    //}
                    byte CustomerPolicy = 0;
                    switch (hotelInfo.Type)
                    {
                        case 0:
                        case 10:
                            CustomerPolicy = (byte)CustomerPolicyEnum.HotelInfo;
                            break;
                        case 2:
                            CustomerPolicy = (byte)CustomerPolicyEnum.Other;
                            break;
                        case 3:
                            CustomerPolicy = (byte)CustomerPolicyEnum.Delivery;
                            break;
                        case 4:
                            CustomerPolicy = (byte)CustomerPolicyEnum.PmsInterface;
                            break;
                        default:
                            CustomerPolicy = (byte)CustomerPolicyEnum.NoCustomers;
                            break;
                    };
                    if (CustomerPolicy == (byte)CustomerPolicyEnum.HotelInfo || CustomerPolicy == (byte)CustomerPolicyEnum.PmsInterface)
                    {
                        if (hotelInfo.Type == 10)
                        {
                            found = true;
                        }
                        else
                        {
                            ProtelRepository pr = new ProtelRepository(hotelInfo.ServerName, hotelInfo.DBUserName, hotelInfo.DBPassword, hotelInfo.DBName, hotelInfo.allHotels, hotelInfo.HotelType);
                            IEnumerable<Customers> customers = pr.GetReservations(hotelInfo.HotelId ?? 0, "", Room, 0, 100);
                            Guest guest = db.Guest.Find(GuestId);
                            foreach (Customers c in customers)
                            {
                                if ((c.ReservationId2 == guest.ReservationId || c.ReservationId == guest.ReservationId) && c.ReservStatus != 2)
                                {
                                    found = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        found = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while checking if customer with Id " + GuestId + " is checked in in hotel with Id " + HotelId + " in room " + Room);
                logger.Error(e.ToString());
                found = false;
            }
            return found;
        }

        public int SaveChanges()
        {

            return db.SaveChanges();
        }
        void IDisposable.Dispose()
        {
            db.Dispose();
        }
    }


    public class InvoiceForStatusModel
    {
        public long InvoicesId { get; set; }
        //Abbreviation = s.FirstOrDefault().Abbreviation,
        //InvoiceType = s.FirstOrDefault().InvoiceType,
        public bool IsInvoiced { get; set; }
        public bool IsVoided { get; set; }
        public int? InvoiceType { get; set; }
    }
}