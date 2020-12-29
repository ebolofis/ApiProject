
using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalDelivery;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalDelivery;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Tasks.ExternalDelivery
{
    public class ForkeyTasks : IForkeyTasks
    {

        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IForkeyDT forkeyDT; IDeliveryCustomersDT delcustDB; IGuestDT guestDB; IDeliveryOrdersDT doDt;
        ICustomJsonSerializers cjson; IWebApiClientHelper wapich;

        public ForkeyTasks(IForkeyDT _forkeyDT, IDeliveryCustomersDT _delcustDB, IGuestDT _guestDB, ICustomJsonSerializers _cjson, IWebApiClientHelper _wapich, IDeliveryOrdersDT _doDt)
        {
            this.forkeyDT = _forkeyDT; this.delcustDB = _delcustDB; this.guestDB = _guestDB; this.cjson = _cjson; this.doDt = _doDt;
            this.wapich = _wapich;
        }

        /// <summary>
        /// Task to return Basic lookpups used to transform ForkeyOrders to Receipt objects 
        /// Contains list of  [posinfo , posinfodetail, pricelists , salestypes , accounts , staff]
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public ForkeyLookups GetLookups(DBInfoModel Store) => forkeyDT.GetLookups(Store);

        /// <summary>
        /// Checks online order and returns an Enum Error enumeration 
        /// to define error cause on online order processed
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public DeliveryForkeyErrorEnum CheckValidOrder(DBInfoModel store, ForkeyDeliveryOrder order, ForkeyLookups lookups)
        {
            // Order Exists 
            if (forkeyDT.CheckExist(store, order))
                return DeliveryForkeyErrorEnum.ALLREADY_PROCESSED;
            // No name on order
            if (string.IsNullOrEmpty(order.user_name))
                return DeliveryForkeyErrorEnum.MISSING_USER_NAME;
            // No address on order
            if (string.IsNullOrEmpty(order.delivery_address.address))
                return DeliveryForkeyErrorEnum.MISSING_ADDRESS;
            // No telephone on order 
            if (string.IsNullOrEmpty(order.user_tel))
                return DeliveryForkeyErrorEnum.MISSING_TEL;
            // On invoice TYpe 7 (timologio) no valid data
            if (order.invoice_type == "INVOICE" && (string.IsNullOrEmpty(order.invoice_profession) || string.IsNullOrEmpty(order.invoice_tax_office) || string.IsNullOrEmpty(order.invoice_vat_num) || string.IsNullOrEmpty(order.invoice_name) || string.IsNullOrEmpty(order.invoice_address)))
            {
                return DeliveryForkeyErrorEnum.MISSING_INVOICE_INFO;
            }
            // No dishes to proccess avoid to proceed without data 
            if (order.dishes.Count <= 0)
            {
                return DeliveryForkeyErrorEnum.NO_DISHES;
            }
            // Order's vat  VS DB vats and existance
            if (lookups.VatList.Count() > 0 && lookups.VatList.Find(xx => xx.Percentage.Equals(order.vat * 100 - 100)) == null)
            {
                return DeliveryForkeyErrorEnum.VAT_MISMATCH;
            }
            else if (lookups.VatList.Count() == 0)
            {
                throw new Exception("No Vats on current Database");
            }

            //Else no error
            return DeliveryForkeyErrorEnum.NoError;
        }

        
        /// <summary>
        /// Uses tasks Methods to create addresses and phones 
        /// uses updatecustomer method to overwrite customer loaded wtih extID
        /// making all than current created entities unselected and upserts creates entities to lists of customer
        /// returns Delivery Customer ready for Upsert Method of DCustomers DT
        /// </summary>
        /// <param name="store"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DeliveryCustomerModel CreateOnlineCustomer(DBInfoModel store, ForkeyDeliveryOrder order, DeliveryCustomerLookupModel lookups)
        {
            DeliveryCustomerModel newDC = CustomerFromForkeyOrder(order, lookups);
            DeliveryCustomersShippingAddressModel newDC_ShipAdd = ShippingAddressFromForkeyOrder(order, lookups);
            DeliveryCustomersBillingAddressModel newDC_BillAdd = BillingAddressFromForkeyOrder(order, lookups);
            DeliveryCustomersPhonesModel newDC_phones = PhoneFromForkeyOrder(order, lookups);
            if (newDC_ShipAdd != null) { newDC.ShippingAddresses = new List<DeliveryCustomersShippingAddressModel>() { newDC_ShipAdd }; }
            if (newDC_BillAdd != null) { newDC.BillingAddresses = new List<DeliveryCustomersBillingAddressModel>() { newDC_BillAdd }; }
            if (newDC_phones != null) { newDC.Phones = new List<DeliveryCustomersPhonesModel>() { newDC_phones }; }

            // Local Entity loaded by external key and type
            DeliveryCustomerModel localEntity = delcustDB.GetCustomerByExtKeyId(store, newDC.ExtCustId, newDC.ExtType);

            return (localEntity == null) ? newDC : UpdateLocalCustomerFromOnline(localEntity, newDC);
        }

        /// <summary>
        /// Customer from forkey order Creates a dummy customer to compare with local customer loaded by ext id from delivery system
        /// </summary>
        /// <param name="order"> Provided forkey order </param>
        /// <returns> A Delivery Customer based on WebPos DB </returns>
        public DeliveryCustomerModel CustomerFromForkeyOrder(ForkeyDeliveryOrder order, DeliveryCustomerLookupModel lookups)
        {
            int? ttype = lookups.CustomerType.Count > 0 ? lookups.CustomerType[0].ID : 0;
            DeliveryCustomerModel res = new DeliveryCustomerModel
            {
                ID = 0,
                LastName = order.user_name,
                email = order.user_email,
                CustomerType = ttype,
                //Billing Values
                BillingName = order.invoice_name,
                BillingVatNo = order.invoice_vat_num,
                BillingDOY = order.invoice_tax_office,
                BillingJob = order.invoice_profession,
                //Forkey Standard
                ExtCustId = order.user_id,
                ExtType = (int)ExternalSystemOrderEnum.Forkey
            };
            return res;
        }

        /// <summary>
        /// Create a customer phone from order with lookup 
        /// default type on phonetype first or 0 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="lookups"></param>
        /// <returns>Delivery Customer Phone or throws exception </returns>
        public DeliveryCustomersPhonesModel PhoneFromForkeyOrder(ForkeyDeliveryOrder order, DeliveryCustomerLookupModel lookups)
        {
            // PhoneType on database is required to handle save of entity 
            // If there are no entries on type parse actual zero to save entity 
            // and handle it later on customer card if necessary
            int? ttype = lookups.PhoneType.Count > 0 ? lookups.PhoneType[0].ID : 0;
            DeliveryCustomersPhonesModel res = new DeliveryCustomersPhonesModel
            {
                ID = 0, //[Required]
                CustomerID = 0, //[Required]
                PhoneNumber = order.user_tel,
                PhoneType = ttype,
                IsSelected = true,
                IsDeleted = false,
                ExtType = (int)ExternalSystemOrderEnum.Forkey
            };
            return res;
        }

        /// <summary>
        /// From forkey address model on order create Delivery Customer Address model
        /// </summary>
        /// <param name="order"> Forkey Order </param>
        /// <returns> Delivery Customer Order </returns>
        public DeliveryCustomersShippingAddressModel ShippingAddressFromForkeyOrder(ForkeyDeliveryOrder order, DeliveryCustomerLookupModel lookups)
        {
            int? ttype = lookups.AddressType.Count > 0 ? lookups.AddressType[0].ID : 0;
            DeliveryCustomersShippingAddressModel res = new DeliveryCustomersShippingAddressModel
            {
                ID = 0,//[Required]
                CustomerID = 0, //[Required]
                AddressStreet = order.delivery_address.address,
                SpecificIndication = order.delivery_address.specific_indication,
                AddressNo = order.delivery_address.addr_num,
                City = order.delivery_address.town,
                Zipcode = order.delivery_address.zip_code,
                Latitude = order.delivery_address.point.latitude.ToString(),
                Longtitude = order.delivery_address.point.longitude.ToString(),
                Floor = order.delivery_address.flat,
                Type = ttype,
                IsSelected = true,
                //res.IsDeleted = false;
                ExtKey = order.delivery_address.id.ToString(),
                ExtType = (int)ExternalSystemOrderEnum.Forkey,
                ExtObj = ""
            };
            // order.delivery_address.google_address //order.delivery_address.bell //order.delivery_address.flat
            return res;
        }

        /// <summary>
        /// Function that creates a dummy order billing address if forkey order is invoice
        /// </summary>
        /// <param name="order"> ForkeyOrder Provided by Delivery System </param>
        /// <returns> Billing Address if forkey order is invoice else  </returns>
        public DeliveryCustomersBillingAddressModel BillingAddressFromForkeyOrder(ForkeyDeliveryOrder order, DeliveryCustomerLookupModel lookups)
        {
            int? ttype = lookups.AddressType.Count > 0 ? lookups.AddressType[0].ID : 0;
            if (order.invoice_address != null)
            {

                DeliveryCustomersBillingAddressModel res = new DeliveryCustomersBillingAddressModel
                {
                    ID = 0,//[Required]
                    CustomerID = 0, //[Required]
                    AddressStreet = order.invoice_address,
                    SpecificIndication = order.delivery_address.specific_indication,
                    Type = ttype,
                    //res.AddressNo = order.delivery_address.addr_num; //res.City = order.delivery_address.town; //res.Zipcode = order.delivery_address.zip_code; //res.Latitude = order.delivery_address.point.latitude.ToString(); //res.Longtitude = order.delivery_address.point.longitude.ToString(); //res.Floor = order.delivery_address.flat; //res.Type = null;
                    //res.IsDeleted = false;
                    IsSelected = true,
                    ExtKey = null,//order.delivery_address.id.ToString();
                    ExtType = (int)ExternalSystemOrderEnum.Forkey,
                    ExtObj = order.invoice_address
                };

                // order.delivery_address.google_address //order.delivery_address.bell //order.delivery_address.flat
                return res;
            }
            else { return null; }
        }

        /// <summary>
        /// Provide a local loaded entity and an online model created from the order
        /// Manages Billing info if they are exist (exists means that order will be treated as an invoice **Timologio for forkey orders )
        /// Manages Addresses making them selected False and if address is found then it is been updated with values provided
        /// Also Address on Billing is only matched through ExtObj cause on forkey order it is not provided with id to match and only when invoicetype is Invoice
        /// </summary>
        /// <param name="localEntity"></param>
        /// <param name="online"></param>
        /// <returns></returns>
        public DeliveryCustomerModel UpdateLocalCustomerFromOnline(DeliveryCustomerModel local, DeliveryCustomerModel online)
        {
            // No change on name cause it could be managed through customers form
            local.LastName = online.LastName ?? online.LastName;
            local.email = online.email ?? online.email;
            // If invoice fields exist then they have to shange due to different order
            local.BillingName = online.BillingName ?? online.BillingName;
            local.BillingVatNo = online.BillingVatNo ?? online.BillingVatNo;
            local.BillingDOY = online.BillingDOY ?? online.BillingDOY;
            local.BillingJob = online.BillingJob ?? online.BillingJob;

            //Online ShipAdd is only one as it is just created
            bool addressfound = false;
            foreach (DeliveryCustomersShippingAddressModel sadd in local.ShippingAddresses)
            {
                if (online.ShippingAddresses[0] != null && sadd.ExtKey == online.ShippingAddresses[0].ExtKey && sadd.ExtType == (int)ExternalSystemOrderEnum.Forkey)
                {
                    addressfound = true;
                    sadd.AddressStreet = online.ShippingAddresses[0].AddressStreet;
                    sadd.AddressNo = ((online.ShippingAddresses[0].AddressNo ?? "") + " " + online.ShippingAddresses[0].SpecificIndication ?? "").Trim();
                    sadd.City = online.ShippingAddresses[0].City;
                    sadd.Zipcode = online.ShippingAddresses[0].Zipcode;
                    sadd.Latitude = online.ShippingAddresses[0].Latitude;
                    sadd.Longtitude = online.ShippingAddresses[0].Longtitude;
                    sadd.Floor = online.ShippingAddresses[0].Floor;
                    sadd.ExtObj = online.ShippingAddresses[0].ExtObj;
                    sadd.IsSelected = true;
                }
                else
                {
                    sadd.IsSelected = false;
                }
            }
            //if not found correct customer id and push to return model
            if (!addressfound && local.ShippingAddresses.Count > 0)
            {
                online.ShippingAddresses[0].CustomerID = local.ID;
                local.ShippingAddresses.Add(online.ShippingAddresses[0]);
            }
            // Manage billing Addresses
            addressfound = false;
            foreach (DeliveryCustomersBillingAddressModel badd in local.BillingAddresses)
            {
                if (online.BillingAddresses != null && online.BillingAddresses[0] != null && badd.ExtObj.Equals(online.BillingAddresses[0].ExtObj) && badd.ExtType == (int)ExternalSystemOrderEnum.Forkey)
                {
                    addressfound = true;
                    badd.IsSelected = true;
                }
                else
                {
                    badd.IsSelected = false;
                }
            }
            if (!addressfound && online.BillingAddresses != null && online.BillingAddresses.Count > 0)
            {
                online.BillingAddresses[0].CustomerID = local.ID;
                online.BillingAddresses[0].IsSelected = true;
                local.BillingAddresses.Add(online.BillingAddresses[0]);
            }
            return local;
        }


        /// <summary>
        /// Provided a forkey order a Local Customer Updated and Pos Entities as lookups 
        /// Constructs an External Object to define usage of order, Creates Receipt, Receipt Details from forkeyDT 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="order"></param>
        /// <param name="localUser"></param>
        /// <param name="lookups"></param>
        /// <returns> Receipt as model to post to invoiceRepository </returns>
        public Receipts ManageSingleForkeyOrder(DBInfoModel Store, ForkeyDeliveryOrder order, DeliveryCustomerModel localUser, ForkeyLookups lookups)
        {
            DeliveryCustomersShippingAddressModel saddr = localUser.ShippingAddresses.Find(xx => xx.IsSelected == true && xx.ExtKey.Equals(order.delivery_address.id.ToString()) && xx.ExtType.Equals((int)ExternalSystemOrderEnum.Forkey));
            DeliveryCustomersBillingAddressModel baddr = localUser.BillingAddresses.Find(xx => xx.IsSelected == true && xx.ExtObj.Equals(order.invoice_address) && xx.ExtType.Equals((int)ExternalSystemOrderEnum.Forkey));
            DeliveryCustomersPhonesModel phone = localUser.Phones.Find(xx => xx.IsSelected == true && xx.PhoneNumber.Equals(order.user_tel) && xx.ExtType.Equals((int)ExternalSystemOrderEnum.Forkey));
            if (phone == null)
            {
                phone = new DeliveryCustomersPhonesModel();
                logger.Warn("<<<<<<<<<<<<<          Phone from forky order is not found in database             >>>>>>>>>>>>>>>>");
                if(localUser.Phones.Count > 0)
                    phone = localUser.Phones[0];
                phone.PhoneNumber = order.user_tel;
            }

            ForkeyPosEntities fposents = new ForkeyPosEntities
            {
                PosInfo = lookups.PosInfoList.Find(sp => sp.Id == order.dependencies.PosInfoId),
                PosInfoDetail = lookups.PosInfoDetailList.Find(sp => sp.Id == order.dependencies.PosInfoDetailId),
                Staff = lookups.StaffList.Find(stf => stf.Id == order.dependencies.StaffId),
                InvoiceType = lookups.InvoiceTypeList.Find(inv => inv.Id == order.dependencies.InvoiceTypeId),
                Pricelist = lookups.PricelistList.Find(pl => pl.Id == order.dependencies.PricelistId),
                SalesType = lookups.SalesTypeList.Find(slt => slt.Id == order.dependencies.SalesTypeId),
                Account = lookups.AccountList.Find(acc => acc.Id == order.dependencies.AccountId),
                VatList = lookups.VatList
            };
            //invoice_type είναι RECEIPT ή INVOICE
            bool isValidInvoice = false;
            if (order.invoice_type == "INVOICE")
            {
                if (baddr == null
                    || string.IsNullOrEmpty(localUser.BillingJob) || string.IsNullOrEmpty(localUser.BillingDOY) || string.IsNullOrEmpty(localUser.BillingVatNo)
                    || string.IsNullOrEmpty(localUser.BillingName)
                    //|| string.IsNullOrEmpty(baddr.AddressStreet)
                    )
                {

                    throw new ForkeyException(DeliveryForkeyErrorEnum.MISSING_INVOICE_INFO.ToString());
                }
                else { isValidInvoice = true; }
            }

            ExtForkeyObj nextobj = new ExtForkeyObj
            {
                OrderNo = order.id.ToString(),
                Status = (int)OrderStatusEnum.Received,
                InvoiceCode = (fposents.InvoiceType != null) ? fposents.InvoiceType.Code : "",
                InvoiceType = (fposents.InvoiceType != null) ? fposents.InvoiceType.Type : null,
                AccountType = (fposents.Account != null) ? fposents.Account.Type : null,
                bell = order.delivery_address.bell,
                with_couvert = order.with_couvert,
                payment_method = order.payment_method,
                company_name = order.delivery_address.company_name,
                rendezvous = (order.timeslot != null),
                start_time = (order.timeslot != null) ? order.timeslot.start_time ?? null : (order.assigned_timeslot != null ? (order.assigned_timeslot.start_time ?? null) : null),
                end_time = (order.timeslot != null) ? order.timeslot.end_time ?? null : (order.assigned_timeslot != null ? (order.assigned_timeslot.end_time ?? null) : null),

                isPrinted = false
            };

            GuestModel guest = guestDB.UpdateGuestFromDeliveryCustomer(Store, localUser);
            ReceiptPayments payments = CreateReceiptPaymentsFromForkey(order, fposents, guest);
            List<ReceiptDetails> details = forkeyDT.CreateReceiptDetailsFromProductCodes(Store, order, fposents);

            Receipts newRec = new Receipts();
            try
            {
                newRec.Id = 0;
                newRec.Day =
                //order.created_at ??
                DateTime.Now;
                newRec.CustomerID = localUser.ID;
                newRec.CustomerName = ((string.IsNullOrEmpty(localUser.LastName)) ? "" : localUser.LastName) + ((string.IsNullOrEmpty(localUser.FirstName)) ? "" : localUser.FirstName);
                newRec.Phone = (phone != null ) ? phone.PhoneNumber : null;

                // If Invoice and valid for invoice type 8 ( Timologio )
                // Billling Address details 
                newRec.BillingAddressId = (isValidInvoice) ? baddr.ID : new long?();
                newRec.BillingAddress = (isValidInvoice) ? baddr.AddressStreet : null;
                newRec.BillingCity = (isValidInvoice) ? baddr.City : null;
                newRec.BillingZipCode = (isValidInvoice) ? baddr.Zipcode : null;
                // Billing information
                newRec.BillingName = (isValidInvoice) ? localUser.BillingName : null;
                newRec.BillingVatNo = (isValidInvoice) ? localUser.BillingVatNo : null;
                newRec.BillingDOY = (isValidInvoice) ? localUser.BillingDOY : null;
                newRec.BillingJob = (isValidInvoice) ? localUser.BillingJob : null;

                // Shipping info
                newRec.ShippingAddressId = saddr.ID;
                newRec.ShippingAddress = (string.IsNullOrEmpty(saddr.AddressStreet)) ? "" : (saddr.AddressStreet + (string.IsNullOrEmpty(saddr.AddressNo) ? "" : " " + saddr.AddressNo));
                newRec.ShippingCity = saddr.City;
                newRec.ShippingZipCode = saddr.Zipcode;
                newRec.Floor = saddr.Floor;
                newRec.Latitude = Convert.ToDouble(saddr.Latitude);
                newRec.Longtitude = Convert.ToDouble(saddr.Longtitude);
                newRec.CustomerRemarks = order.delivery_address.comments;

                // or copy those from delivery model returning external object
                newRec.ExtKey = order.id.ToString();
                newRec.ExtType = (int)ExternalSystemOrderEnum.Forkey;
                newRec.ExtObj = cjson.DynamicToJson(nextobj);//"{"OrderNo":"5139493","Status":0,"InvoiceCode":"1","InvoiceType":1,"AccountType":1}"

                newRec.EndOfDayId = null;
                newRec.FODay = null;
                newRec.IsVoided = false;
                newRec.IsPrinted = false;
                newRec.OrderNo = order.id.ToString();
                newRec.ReceiptNo = 0;
                // short? 
                newRec.InvoiceIndex = fposents.PosInfoDetail.InvoiceId;
                newRec.InvoiceTypeId = fposents.PosInfoDetail.InvoicesTypeId;
                newRec.Abbreviation = fposents.PosInfoDetail.Abbreviation;
                newRec.InvoiceDescription = fposents.PosInfoDetail.Description;
                newRec.InvoiceTypeType = (short?)fposents.PosInfoDetail.GroupId;
                // Pos info entities
                newRec.PosInfoId = fposents.PosInfo.Id;
                newRec.PosInfoDescription = fposents.PosInfo.Description;
                newRec.DepartmentId = fposents.PosInfo.DepartmentId;
                newRec.DepartmentDescription = fposents.PosInfo.DepartmentDescription;
                // Pos Info detail entities
                newRec.PosInfoDetailId = fposents.PosInfoDetail.Id;
                newRec.CreateTransactions = fposents.PosInfoDetail.CreateTransaction ?? false;
                // Staff Entities
                newRec.StaffId = fposents.Staff.Id;
                newRec.StaffCode = fposents.Staff.Code;
                newRec.StaffName = fposents.Staff.FirstName;
                newRec.StaffLastName = fposents.Staff.LastName;
                // string PaymentsDesc , 
                newRec.PaidTotal = (order.payment_method.Equals(DeliveryForkeyPaymentEnum.CASH.ToString())
                      || order.payment_method.Equals(DeliveryForkeyPaymentEnum.MPOS.ToString()))
                          ? 0 : order.transaction_money_with_promo;
                newRec.Net = order.transaction_money / order.vat;
                newRec.Vat = order.transaction_money - (order.transaction_money / order.vat);
                newRec.Total = order.transaction_money_with_promo;

                newRec.Discount = order.promo_money;  // Discount value on order
                newRec.DiscountRemark = order.discount_string;

                // Is paid id Paid type 
                // 0 is not paid as payment will be on delivery  // else 2 is all paid as it is paid 
                newRec.IsPaid = 0; //(order.payment_method.Equals(DeliveryForkeyPaymentEnum.CASH.ToString()) || order.payment_method.Equals(DeliveryForkeyPaymentEnum.MPOS.ToString())) ? (short)IsPaidEnum.None : (short)IsPaidEnum.All,
                newRec.ModifyOrderDetails = (int)ModifyOrderDetailsEnum.FromScratch;
                newRec.PrintType = (int)PrintTypeEnum.PrintWhole;
                newRec.Cover = 0;
                // long? ClientPosId , // long? PdaModuleId , // int Points , // string Room , // long? TableId ,  // string TableCode ,// decimal? LoyaltyDiscount , // string DigitalSignatureImage ,

                ///// The ammount the customer give us so we will compute his change.
                // string CashAmount ,
                ///// The number of the buzzer related with current invoice
                // string BuzzerNumber,

                ///// An enumerator concerning the origin of an order (eg. 'local', 'call center', see origin enumerator in BOEnums.cs) 
                newRec.OrderOrigin = (int)OrderOriginEnum.Web;

                ///// How to print a receipt (print the whole receipt or a part of it)
                ///// contains the method which is going to be print as item's second line (for OPOS/OPOS3)
                // string ItemAdditionalInfo,

                ///// true if print without ADHME
                // bool TempPrint,
                // bool? IsAssignedForDelivery, long? AssignedToDeliverStaffId, short? AssignedToDeliverStatus,

                newRec.ReceiptDetails = details;
                newRec.ReceiptPayments = new List<ReceiptPayments>() { payments };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }


            return newRec;
        }


        

        /// <summary>
        /// Task Method to create Receipt Payments for Receipt creating from forkey order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ents"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public ReceiptPayments CreateReceiptPaymentsFromForkey(ForkeyDeliveryOrder order, ForkeyPosEntities ents, GuestModel guest)
        {
            ReceiptPayments ret = new ReceiptPayments
            {
                //long? ReceiptsId  //EndOfDayId = null, // long? ReceiptNo // string Abbreviation =ents.PosInfoDetail.Abbreviation,
                InvoiceType = (short)ents.PosInfoDetail.GroupId,
                AccountId = ents.Account.Id,
                AccountType = ents.Account.Type,
                AccountDescription = ents.Account.Description,
                PosInfoId = ents.PosInfo.Id,
                Amount = order.transaction_money_with_promo,
                SendsTransfer = ents.Account.SendsTransfer,
                GuestId = guest.Id,
                ProfileNo = guest.ProfileNo,
                GuestString = ((string.IsNullOrEmpty(guest.LastName)) ? "" : guest.LastName) + ((string.IsNullOrEmpty(guest.FirstName)) ? "" : guest.FirstName),
                // long? AccountEODRoom  // short? TransactionType // string Room  // int? RoomId  // string ReservationCode  // long? CreditAccountId  // long? CreditCodeId  // string CreditAccountDescription  // short CreditTransactionAction  //// decimal? NewCreditBalance  // long? HotelId 
            };
            return ret;
        }



        /// <summary>
        /// Calls Forkey DT providing a filter for order to get entities or Local Order
        /// Order, Invoices, OrderStatuses
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="forder"></param>
        /// <returns></returns>
        public ForkeyLocalEntities GetForkeyOrderLocalEntitiesTask(DBInfoModel Store, ForkeyDeliveryOrder forder) => forkeyDT.GetForkeyOrderLocalEntities(Store, forder);

        /// <summary>
        /// Gets a Receipt and a forkey order to change Receipt entities for cancel procedure via InvoiceRepo
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="forder"></param>
        /// <param name="lookups"></param>
        /// <returns></returns>
        public Receipts ApplyCancelToReceipt(Receipts rec, ForkeyDeliveryOrder forder, ForkeyLookups lookups)
        {
            PosInfoDetailModel spd;
            PosInfoModel posi = lookups.PosInfoList.Where(p => p.Id == forder.dependencies.PosInfoId).FirstOrDefault();
            if (rec.InvoiceTypeType == 1 || rec.InvoiceTypeType == 4 || rec.InvoiceTypeType == 5 || rec.InvoiceTypeType == 7)
            {
                spd = lookups.PosInfoDetailList.Where(o => o.GroupId == 3 && o.PosInfoId == posi.Id).FirstOrDefault();
            }
            else if (rec.InvoiceTypeType == 2)
            {
                spd = lookups.PosInfoDetailList.Where(o => o.GroupId == 8 && o.PosInfoId == posi.Id).FirstOrDefault();
            }
            else
            {
                throw new Exception("Unknown InvoiceTypeType case to define PosinfoDetail for Cancel Procedure");
            }
            // short? 
            rec.InvoiceIndex = spd.InvoiceId;
            rec.InvoiceTypeId = spd.InvoicesTypeId;
            rec.Abbreviation = spd.Abbreviation;
            rec.InvoiceDescription = spd.Description;
            rec.InvoiceTypeType = (short?)spd.GroupId;

            // Pos info entities
            rec.PosInfoId = posi.Id;
            rec.PosInfoDetailId = spd.Id;
            rec.PosInfoDescription = posi.Description;
            rec.DepartmentId = posi.DepartmentId;
            rec.DepartmentDescription = posi.DepartmentDescription;
            foreach (ReceiptDetails item in rec.ReceiptDetails)
            {
                item.ItemQty *= -1;
                item.Abbreviation = spd.Abbreviation;
            }
            return rec;
        }

        /// <summary>
        /// Calls ForketEntities DT to get Order, InvoicesList and OrderStatus List to return Enum
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="forder"></param>
        /// <returns> On Each Case of match Returns DeliveryForkeyStatusEnum based on state returned </returns>
        public DeliveryForkeyStatusEnum DefineForkeyState(DBInfoModel Store, ForkeyDeliveryOrder forder)
        {
            ForkeyLocalEntities entries = GetForkeyOrderLocalEntitiesTask(Store, forder);
            OrderStatusModel curr;

            dynamic e = cjson.DynamicDeseriallize(entries.Order.ExtObj);
            if (e != null && e.isPrinted != null && e.isPrinted == true)
            {
                return DeliveryForkeyStatusEnum.printed;
            }

            curr = entries.OrderStatusList.Where(x => x.Status == OrderStatusEnum.Canceled).FirstOrDefault();
            if (curr != null) return DeliveryForkeyStatusEnum.cancelled;

            curr = entries.OrderStatusList.Where(x => x.Status == OrderStatusEnum.Complete).FirstOrDefault();
            if (curr != null) return DeliveryForkeyStatusEnum.delivered;

            curr = entries.OrderStatusList.Where(x => x.Status == OrderStatusEnum.Ready).FirstOrDefault();
            if (curr != null) return DeliveryForkeyStatusEnum.assigned;



            return DeliveryForkeyStatusEnum.downloaded; //downloaded = 0 , printed = 1 , assigned = 2, delivered = 3, cancelled = 4
        }

        /// <summary>
        /// Gets forkey entities from DT and based on Order - Invoices - Order Statuses
        /// switches based on logic and patches Order using method on task 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public long PatchOrderFromStatus(string storeid, long orderid, int? extType)
        {

            ForkeyLocalEntities entries = forkeyDT.GetForkeyEntities(storeid, orderid, extType);
            return prePatchOrder(entries);

        }

        /// <summary>
        /// Based on orderno Provided gets forkey entities 
        /// Decides what is the appropriate State and uses patch order to post change on forkey 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public long CancelPatchOrderFromStatus(string storeid, string orderno)
        {
            ForkeyLocalEntities entries = forkeyDT.GetForkeyEntitiesByOrderNo(storeid, orderno);
            return prePatchOrder(entries);
        }


        public long prePatchOrder(ForkeyLocalEntities entries)
        {
            try
            {

                OrderStatusModel curr;
                DeliveryForkeyStatusEnum state = DeliveryForkeyStatusEnum.downloaded;

                dynamic e = cjson.DynamicDeseriallize(entries.Order.ExtObj);


                curr = entries.OrderStatusList.Where(x => x.Status == OrderStatusEnum.Canceled).FirstOrDefault();
                if (curr != null) return PatchOrder(entries.Order.ExtKey, DeliveryForkeyStatusEnum.cancelled);
                curr = entries.OrderStatusList.Where(x => x.Status == OrderStatusEnum.Complete).FirstOrDefault();
                if (curr != null) return PatchOrder(entries.Order.ExtKey, DeliveryForkeyStatusEnum.assigned);
                curr = entries.OrderStatusList.Where(x => x.Status == OrderStatusEnum.Onroad).FirstOrDefault();
                if (curr != null) return PatchOrder(entries.Order.ExtKey, DeliveryForkeyStatusEnum.assigned);
                if (e != null && e.isPrinted != null && e.isPrinted == true)
                {
                    return PatchOrder(entries.Order.ExtKey, DeliveryForkeyStatusEnum.printed);
                }
                return 0;
            }
            catch (Exception ex)
            {
                return -1; //exception here may be that order is not forkey order
            }
        }

        /// <summary>
        /// Uses webconfiguration attributes of forkeyPatch, forkeyAuth to create url callback
        /// then uses IWebApiClientHelper to patch an order back to url with orderId 
        /// Call must be authenticated so url and auth is custom on webconfiguration.AppSettings
        /// </summary>
        /// <param name="forkey_orderId"></param>
        /// <param name="forkey_status"></param>
        /// <returns></returns>
        public long PatchOrder(string forkey_orderId, DeliveryForkeyStatusEnum forkey_status)
        {
            // "https://devel.forky.gr/api/backend/orders/2",
            string pathtopatchRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "forkeyPatch");
            string pathtopatch = pathtopatchRaw.Trim();
            pathtopatch += forkey_orderId;

            //"Bearer ZTdmZmY1Zjc5MTQ4NDQ5ZTEzMzIyZTBkNTY1YmJlMzJlZjYzZDA2OTIyMDkzOGIxOTY2N2YzNDA2ZWVlMDkwOQ",
            string forkeyAuthRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "forkeyAuth");
            string forkeyAuth = forkeyAuthRaw.Trim();

            int returnCode = 0; string ErrorMess = "-";
            dynamic MyDynamic = new System.Dynamic.ExpandoObject();
            MyDynamic.status = forkey_status.ToString();
            logger.Info(">>>>>                Paching to Forky api : " + pathtopatch + "   with Status: '" + forkey_status.ToString() + "'...");
            try
            {
                string result = wapich.PatchRequest<dynamic>(MyDynamic, pathtopatch, forkeyAuth, null, out returnCode, out ErrorMess, "application/json", "OAuth2");
                if (returnCode > 299)
                    logger.Info("<<<<<<<<<<<                      Could not patch to Forky Api : StatusError" + returnCode + "and Error Message: " + ErrorMess + "                       >>>>>>>>>>>>>>>>>>>>");
            }
            catch (Exception ex)
            {
                logger.Error("Exception while Patching ForkeyOrder:" + ex.ToString());
            }
            //όπου OrderID είναι το id της παραγγελίας.
            //Σε αυτό το request θα πρέπει να υπάρχει το header content - type: application / json και το body να περιέχει περιεχόμενο μορφής { "status": "<NewStatus>" } . 
            //Το NewStatus αυτή τη στιγμή μπορεί να πάρει τιμές: downloaded,printed,assigned,delivered,cancelled.Εάν η ενημέρωση είναι επιτυχής επιστρέφεται HTTP status code 204 με κενό περιεχόμενο.
            return 0;
        }

        /// <summary>
        /// Based on invoice id joins entities to update Order.ExtObj.Deseriallized.isPrinted
        /// selects invoices, binded to orderdetailinvs, binded to orderdetail , binded to Order
        /// Deserializes obj then updates isPrinted from Obj changes value then updates orders collected
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="printed"></param>
        /// <returns></returns>
        public bool ChangeForkeyIsPrintedExtObj(DBInfoModel Store, long InvoiceId, bool printed = true)
        {
            List<OrderDTO> orders = forkeyDT.ChangeForkeyIsPrintedExtObj(Store, InvoiceId, printed);
            if (orders != null && orders.Count > 0)
            {
                foreach (OrderDTO o in orders)
                {
                    if (o.ExtType == 3)
                    {
                        long p = PatchOrderFromStatus(Store.Id.ToString(), o.Id, o.ExtType);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Based on order_id provided task gathers Forkey Entities from DTD and returns from invoicelist first invoice that 
        /// has invoicetype = 2 applyies extobj is printed = true and patches order to printed
        /// then returns invoice gothered to signal extcer
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public InvoiceModel PrintCaptainsOrderTask(DBInfoModel store, long orderid, int? extType)
        {
            try
            {
                ForkeyLocalEntities entries = forkeyDT.GetForkeyEntities(store.Id.ToString(), orderid, extType);
                InvoiceModel ret = entries.InvoiceList.Where(q => q.InvoiceTypeId == 2).OrderByDescending(o => o.Day).FirstOrDefault();
                bool apply = true;
                if (extType == (int?)ExternalSystemOrderEnum.Forkey || extType == (int?)ExternalSystemOrderEnum.VivardiaNoKitchen)
                    apply = ChangeForkeyIsPrintedExtObj(store, ret.Id ?? 0, true);
                PatchOrder(entries.Order.ExtKey, DeliveryForkeyStatusEnum.printed);
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
