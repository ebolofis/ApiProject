using Newtonsoft.Json;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Goodys;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Goodys
{
    public class GoodysOrdersFlow : IGoodysOrdersFlow
    {
        IGoodysTasks goodysTasks;
        IDA_OrdersFlows orderFlow;
        IDA_CustomerFlows customerFlow;
        IExtDeliverySystemsTasks extDelSystTask;
        IDA_AddressesFlows daAddressesFlow;
        IVatFlows vatFlow;
        IProductDT productDT;
        IAccountTasks accTask;
        IDA_StoresTasks storeTasks;
        IDA_Store_PriceListAssocTasks daStoresPriceAssoc;
        ILoyaltyTasks loyaltyTasks;

        public GoodysOrdersFlow(IGoodysTasks _goodysTasks, IDA_OrdersFlows _orderFlow, IDA_CustomerFlows _customerFlow,
            IExtDeliverySystemsTasks _extDelSystTask, IDA_AddressesFlows _daAddressesFlow,
            IVatFlows _vatFlow, IProductDT _productDT, IAccountTasks _accTask, IDA_StoresTasks _storeTasks,
            IDA_Store_PriceListAssocTasks _daStoresPriceAssoc, ILoyaltyTasks _loyaltyTasks)
        {
            goodysTasks = _goodysTasks;
            orderFlow = _orderFlow;
            customerFlow = _customerFlow;
            extDelSystTask = _extDelSystTask;
            daAddressesFlow = _daAddressesFlow;
            vatFlow = _vatFlow;
            productDT = _productDT;
            accTask = _accTask;
            storeTasks = _storeTasks;
            daStoresPriceAssoc = _daStoresPriceAssoc;
            loyaltyTasks = _loyaltyTasks;
        }

        /// <summary>
        /// Return's a Login responce model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public GoodysLoginResponceModel GetLoginResponceModel(DBInfoModel dbInfo, string AccountId, bool allAddresses = true)
        {
            return goodysTasks.GetLoginResponceModel(dbInfo, AccountId, allAddresses);
        }

        /// <summary>
        /// Inserts new customer and address on da_customer and da_address table based on a Goodys Registration Model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public GoodysLoginResponceModel RegisterCustomer(DBInfoModel dbInfo, GoodysRegisterModel model)
        {
            //1. External DA_Customer model
            DACustomerExtModel cust = new DACustomerExtModel();

            //2. Inform properties from goodys model
            cust.Email = model.acceMail;
            cust.FirstName = model.fName;
            cust.LastName = model.lName;
            cust.Id = 0;
            cust.IsDeleted = false;
            cust.Notes = model.addrComments;
            cust.Phone1 = model.addrPhoneNum;
            if (model.accountPhone != null && model.addrPhoneNum != null)
                cust.Phone2 = model.accountPhone.IndexOf(model.addrPhoneNum) < 0 ? model.addrPhoneNum : "";
            cust.SendEmail = true;
            cust.SendSms = true;

            //New shipping address
            cust.ShippingAddresses = new List<DA_AddressModel>();
            DA_AddressModel address = new DA_AddressModel();

            //4. Inform properties from goodys model
            address.AddressNo = model.addrNumber;
            address.AddressStreet = model.addrStreetName;
            address.AddressType = 0;
            address.Area = model.addrState;
            address.City = model.addrCounty;
            address.Floor = model.addrFloor;
            address.FriendlyName = model.addrShortDesc;
            address.Id = 0;
            address.IsDeleted = false;
            address.isShipping = true;

            address.Latitude = 0;
            address.Longtitude = 0;

            address.Notes = model.addrComments;
            address.OwnerId = 0;
            address.Zipcode = model.addrPostalCode;

            //5. Get Latitude and Longtitude
            extDelSystTask.GeocodeAddress(dbInfo, address);

            //6. Add address to external model
            cust.ShippingAddresses.Add(address);

            //7. Add model to db
            DACustomerExtModel res = customerFlow.AddCustomer(dbInfo, cust);

            //8 Return a Login Respnce model
            return GetLoginResponceModel(dbInfo, res.Id.ToString(), false);

        }

        /// <summary>
        /// Create's new Address to customer
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public GoodysLoginResponceModel CreateNewAddress(DBInfoModel dbInfo, GoodysLoginAddressResponceModel model)
        {
            //1. Get customer model for propert address.accId
            GoodysLoginResponceModel cust = goodysTasks.GetLoginResponceModel(dbInfo, model.accId, false);
            if (cust == null)
                return null;

            //2 Create new Address model and plugin address model to get geolocation using as result adr
            DA_AddressModel address = new DA_AddressModel();
            DA_AddressModel pluginAdr = new DA_AddressModel();
            DA_AddressModel adr = new DA_AddressModel();

            //3. Inform properties from goodys model
            address.AddressNo = model.NNHome;
            address.AddressStreet = model.streetAddress;
            address.AddressType = 0;
            address.Area = model.state;
            address.City = model.county;
            address.Floor = model.addresssFloor;
            address.FriendlyName = model.streetAlias;
            address.Id = 0;
            address.IsDeleted = false;
            address.isShipping = true;
            //For this records on external key add "GO" (Goodys Old)
            address.ExtId1 = !string.IsNullOrEmpty(model.addressId) ? model.addressId + "GO" : model.addressId;

            address = daAddressesFlow.GeoLocationMaps(dbInfo, address);

            address.Notes = model.addressComment;
            address.OwnerId = long.Parse(cust.id);
            address.Zipcode = model.postalCode;

            //4. Get Latitude and Longtitude
            pluginAdr = AutoMapper.Mapper.Map<DA_AddressModel>(address);
            adr = daAddressesFlow.GeoLocationMaps(dbInfo, pluginAdr);

            address.Latitude = adr.Latitude;
            address.Longtitude = adr.Longtitude;

            //5. Add address to external model
            daAddressesFlow.AddAddress(dbInfo, address, address.OwnerId);

            //6. Return new GoodysLoginResponceModel with customer and all addresses
            return goodysTasks.GetLoginResponceModel(dbInfo, cust.id, false);

        }

        /// <summary>
        /// Deletes an address of a specific customer
        /// For External keys added "GO" (Goodys Old) on then end of the string
        /// </summary>
        /// <param name="dBInfo"></param>
        /// <param name="addressId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public GoodysLoginResponceModel DeleteAddress(DBInfoModel dBInfo, string addressId, string accountId)
        {
            //1. Get customer by extid1 or id
            GoodysLoginResponceModel cust = goodysTasks.GetLoginResponceModel(dBInfo, accountId, false);
            if (cust == null)
                return null;

            //2. Get an external DACustomerModel with all addresses
            DACustomerExtModel extCust = customerFlow.getCustomer(dBInfo, long.Parse(cust.id));
            if (extCust == null)
                return null;

            //3. Check's if Address id is string or not
            long adrId = 0;
            long.TryParse(addressId, out adrId);

            //4. DA_AddressModel to deleted
            DA_AddressModel address = null;

            //5. Find if address exists on ShippingAddresses
            if (extCust.ShippingAddresses != null)
            {
                if (adrId < 1)
                    address = extCust.ShippingAddresses.Find(f => f.ExtId1 == addressId + "GO");
                else
                    address = extCust.ShippingAddresses.Find(f => f.Id == adrId);
            }
            //6. Address id is not a shipping addres and try to find if it is a billing address (Why DACustomerExtModel.BillingAddress is not a list???)
            if (address == null && extCust.BillingAddress != null)
            {
                if (adrId < 1 && extCust.BillingAddress.ExtId1 == addressId + "GO")
                    address = extCust.BillingAddress;
                else if (extCust.BillingAddress.Id == adrId)
                    address = extCust.BillingAddress;
            }

            //7. If address found then delete it
            if (address != null)
            {
                daAddressesFlow.DeleteAddress(dBInfo, address.Id, extCust.Id);
                cust = goodysTasks.GetLoginResponceModel(dBInfo, accountId, false);
            }

            return cust;
        }

        /// <summary>
        /// Find available coupons from property room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private GetCouponsFromStringModel FindCoupons(string room)
        {
            string[] tmp = room.Split(',');
            GetCouponsFromStringModel res = new GetCouponsFromStringModel();

            /// LastItem
            /// 0=> CouponCode, 1=> CouponCampaign, 2=> Giftcards, 3=> LoyaltyID
            int LastItem = -1;

            /// First two elements is for order source and ???
            int idx = 0;
            foreach (string item in tmp)
            {
                if (idx < 2)
                {
                    if (idx == 0)
                        res.WEB = item;
                    else
                        res.DE = item;
                }
                else
                {
                    if (item.Contains("CouponCode"))
                    {
                        res.CouponCode = item.Substring(item.IndexOf(':') + 1, item.Length - item.IndexOf(':') - 1); ;
                        LastItem = 0;
                    }
                    else if (item.Contains("CouponCampaign"))
                    {
                        res.CouponCamp = item.Substring(item.IndexOf(':') + 1, item.Length - item.IndexOf(':') - 1);
                        LastItem = 1;
                    }
                    else if (item.Contains("Giftcards"))
                    {
                        res.GiftCard = item.Substring(item.IndexOf(':') + 1, item.Length - item.IndexOf(':') - 1); ;
                        LastItem = 2;
                    }
                    else if (item.Contains("LoyaltyID"))
                    {
                        res.Loyalty = item.Substring(item.IndexOf(':') + 1, item.Length - item.IndexOf(':') - 1); ;
                        LastItem = 3;
                    }
                    else
                    {
                        switch (LastItem)
                        {
                            case 0:
                                res.CouponCode += "," + item;
                                break;
                            case 1:
                                res.CouponCamp += "," + item;
                                break;
                            case 2:
                                res.GiftCard += "," + item;
                                break;
                            case 3:
                                res.Loyalty += "," + item;
                                break;
                            default:
                                break;
                        }
                    }
                }
                idx++;
            }
            return res;
        }

        /// <summary>
        /// Insert an order to da_orders and returns the given model improved with 3 fields (orderNo, CustomerId and Store)
        /// </summary>
        /// <param name="dBInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public GoodysDA_OrderModel InsertOrder(DBInfoModel dBInfo, GoodysDA_OrderModel model)
        {
            //1. Get customer by extid1 or id
            GoodysLoginResponceModel cust = goodysTasks.GetLoginResponceModel(dBInfo, model.customer.accountId);
            if (cust == null)
                return null;

            //2. Get an external DACustomerModel with all addresses
            DACustomerExtModel extCust = customerFlow.getCustomer(dBInfo, long.Parse(cust.id));
            if (extCust == null)
                return null;

            //Initial Loyalty model and get coupons from property room, TotalOrderPrice, TotalOrderDiscount, TotalOrderTotal, TotalOrderNetAmount,TotalOrderVat
            LoyaltyModel loyalty = null;
            GetCouponsFromStringModel coupons = FindCoupons(model.room);
            if (coupons != null && (!string.IsNullOrEmpty(coupons.CouponCode) || !string.IsNullOrEmpty(coupons.Loyalty) || !string.IsNullOrEmpty(coupons.GiftCard)))
            {
                loyalty = new LoyaltyModel();
                loyalty.Channel = coupons.WEB;
                loyalty.Campaign = coupons.CouponCamp;
                loyalty.CouponCode = coupons.CouponCode;
                loyalty.DateTime = DateTime.Now;
                loyalty.GiftcardCode = coupons.GiftCard;
                loyalty.LoyalltyId = coupons.Loyalty;
            }

            decimal TotalPrice = 0, TotalDiscount = 0, TotalTotal = 0, TotalNetAmount = 0, TotalVatAmount = 0;


            //Change billing info for customer
            if (!string.IsNullOrEmpty(model.customer.bl_address))
            {
                extCust.Doy = model.customer.doy;
                extCust.VatNo = model.customer.afm;
                customerFlow.UpdateCustomer(dBInfo, AutoMapper.Mapper.Map<DACustomerModel>(extCust), true, extCust.Id);
            }


            //No Store Found
            List<DA_StoreModel> allStores = storeTasks.GetStores(dBInfo);
            DA_StoreModel store = allStores.Find(f => f.Code == model.shopId);
            if (store == null)
                return null;

            long tmpAdr;
            DA_AddressModel shippingAddress = null;
            DA_AddressModel billingAddress = null;
            //3. Finding Shipping Address
            if (!string.IsNullOrEmpty(model.customer.addressId) && extCust.ShippingAddresses != null)
            {
                long.TryParse(model.customer.addressId, out tmpAdr);

                if (tmpAdr < 1)
                    shippingAddress = extCust.ShippingAddresses.Find(f => f.ExtId1 == model.customer.addressId + "GO");
                else
                    shippingAddress = extCust.ShippingAddresses.Find(f => f.Id == tmpAdr);
            }

            //4. Shipping address not found (Add new one????)
            if (shippingAddress == null)
            {
                //4.1 Insert new Address and get id
                GoodysLoginAddressResponceModel newAddress = new GoodysLoginAddressResponceModel();
                newAddress.accId = cust.id.ToString();
                newAddress.NNHome = model.customer.addressNo;
                newAddress.streetAddress = model.customer.address1;
                newAddress.county = model.customer.city;
                newAddress.addresssFloor = model.customer.orofos1;
                newAddress.addressId = model.customer.addressId;
                newAddress.postalCode = model.customer.zipcode;

                CreateNewAddress(dBInfo, newAddress);
                extCust = customerFlow.getCustomer(dBInfo, long.Parse(cust.id));

                if (!string.IsNullOrEmpty(model.customer.addressId) && extCust.ShippingAddresses != null)
                {
                    long.TryParse(model.customer.addressId, out tmpAdr);

                    if (tmpAdr < 1)
                        shippingAddress = extCust.ShippingAddresses.Find(f => f.ExtId1 == model.customer.addressId + "GO");
                    else
                        shippingAddress = extCust.ShippingAddresses.Find(f => f.Id == tmpAdr);
                }
            }


            //5. Finding billing address based on street name ????
            if (!string.IsNullOrEmpty(model.customer.bl_address))
            {
                if (extCust.BillingAddress != null)
                {
                    //Update billing address with new data
                    extCust.BillingAddress.AddressStreet = model.customer.bl_address;
                    extCust.BillingAddress.AddressNo = model.customer.bl_address_no;
                    extCust.BillingAddress.City = model.customer.bl_city;

                    daAddressesFlow.UpdateAddress(dBInfo, extCust.BillingAddress, extCust.Id);

                    billingAddress = extCust.BillingAddress;
                }
                else
                {
                    //Insert new billing address
                    DA_AddressModel billAdr = new DA_AddressModel();
                    billAdr.AddressStreet = model.customer.bl_address;
                    billAdr.AddressNo = model.customer.bl_address_no;
                    billAdr.City = model.customer.bl_city;
                    billAdr.AddressType = 1;
                    billAdr.isShipping = false;
                    billAdr.IsDeleted = false;
                    billAdr.OwnerId = extCust.Id;
                    long newBillAddrId = daAddressesFlow.AddAddress(dBInfo, billAdr, billAdr.OwnerId);

                    List<DA_AddressModel> tmpAddr = daAddressesFlow.GetCustomerAddresses(dBInfo, extCust.Id);
                    if (tmpAddr != null)
                        billingAddress = tmpAddr.Find(f => f.Id == newBillAddrId);
                }
            }

            //6. Get a list of vats, Accounts, PriceList from DAStore_PriceListAssoc 
            List<VatModel> vats = vatFlow.GetAllVats(dBInfo);
            VatModel crVat;
            List<AccountModel> accounts = accTask.GetActiveAccounts(dBInfo);
            List<DAStore_PriceListAssocModel> storePriceAssoc = daStoresPriceAssoc.GetDAStore_PriceListAssoc(dBInfo);
            long PriceList = storePriceAssoc.Find(f => f.DAStoreId == (store.Id ?? 0) && f.PriceListType == DAPriceListTypes.ForDelivery).PriceListId;


            //7. Create a list of DA_OrderDetails from mode.products
            List<DA_OrderDetails> details = new List<DA_OrderDetails>();

            //for each item in product list if it is not combine get info about it from GetProductExt using product code. If it is null then all combined items not inserted
            ProductExtModel tmpProd = null;

            foreach (GoodysDA_OrderProductModel item in model.products)
            {
                if (item.isCombined == false)
                {
                    DA_OrderDetails obj = new DA_OrderDetails();
                    tmpProd = productDT.GetProductExt(dBInfo, item.item_code);
                    if (tmpProd != null)
                    {
                        obj.Id = 0;
                        obj.DAOrderId = 0;
                        obj.Extras = new List<DA_OrderDetailsExtrasModel>();

                        obj.ProductId = tmpProd.Id;
                        obj.PriceListId = PriceList < 1 ? -1 : PriceList;
                        obj.Description = item.item_descr;
                        obj.Qnt = item.qty;

                        if ((item.total_disc ?? 0) != 0)    //total_disc final amount
                        {
                            if (item.total_disc == item.total && item.total_disc < 0)
                            {
                                //obj.Price = (((-1) * ((decimal)(item.total_disc ?? 0))) / item.qty) / 100;
                                //obj.Total = 0;
                                //obj.Discount = (-1) * ((decimal)(item.total_disc ?? 0) / 100);
                                obj.Price= (((-1) * ((decimal)item.total)) / item.qty) / 100;
                                obj.Total = (((-1) * ((decimal)item.total)) / item.qty) / 100;
                                obj.Discount = 0;

                                TotalPrice += (-1) * (((decimal)(item.amount * item.qty)) / 100);
                                TotalDiscount += (-1) * ((decimal)(item.total_disc ?? 0) / 100);
                            }
                            else
                            {
                                //obj.Price = (((decimal)item.total / item.qty) / 100);
                                //obj.Total = ((decimal)(item.total_disc ?? 0)) / 100;
                                //obj.Discount = ((decimal)item.total - ((decimal)(item.total_disc ?? 0))) / 100;
                                obj.Price = (((decimal)item.total / item.qty) / 100);
                                obj.Total = (((decimal)item.total /* * item.qty*/) / 100);


                                TotalPrice += (((decimal)(item.amount * item.qty)) / 100);
                                TotalTotal+= ((decimal)(item.total_disc ?? 0)) / 100;
                                TotalDiscount += ((decimal)item.total - ((decimal)(item.total_disc ?? 0))) / 100;
                            }
                        }
                        else
                        {
                            //obj.Price = (((decimal)item.total / item.qty) / 100);
                            //obj.Total = (decimal)item.total / 100;
                            obj.Price = (((decimal)item.total / item.qty) / 100);
                            obj.Total = (((decimal)item.total /* * item.qty*/) / 100);

                            TotalPrice += (((decimal)(item.amount * item.qty)) / 100);
                            TotalTotal += (decimal)item.total / 100;
                        }

                        crVat = vats.Find(f => f.Code == item.item_vat);
                        if (crVat != null)
                        {
                            if (crVat.Percentage != 0)
                            {
                                decimal tempNet = Math.Round(obj.Total / (1 + ((crVat.Percentage ?? 0) / 100)), 4);
                                obj.TotalVat = obj.Total - tempNet;

                                TotalVatAmount+= obj.Total - tempNet;
                                TotalNetAmount += tempNet;
                            }
                            else
                                obj.TotalVat = 0;
                            obj.RateVat = crVat.Percentage ?? 0;
                        }
                        else
                        {
                            obj.TotalVat = 0;
                            obj.RateVat = crVat.Percentage ?? 0;
                        }
                        obj.RateTax = 0;
                        obj.TotalTax = 0;
                        obj.NetAmount = obj.Total - obj.TotalVat;
                        obj.ItemRemark = "";

                        details.Add(obj);
                    }
                    else
                    {
                        throw new Exception("Product with code : " + item.item_code + " (" + item.item_descr + ") not found");
                    }
                }
                else
                {
                    //if main product not exists then no extras added
                    if (tmpProd != null)
                    {
                        ProductExtrasIngredientsModel tmpExtra = tmpProd.Extras.Find(f => f.Code == item.item_code);
                        DA_OrderDetails lastObj = details[details.Count - 1];
                        DA_OrderDetailsExtrasModel extras = new DA_OrderDetailsExtrasModel();
                        extras.Id = 0;
                        extras.OrderDetailId = 0;
                        extras.ExtrasId = tmpExtra == null ? -1 : (tmpExtra.Id ?? -1);
                        extras.Description = item.item_descr;
                        extras.Qnt = item.qty;
                        if ((item.total_disc ?? 0) != 0)
                        {
                            if (item.total_disc < 0)
                            {
                                //extras.Price = ((-1) * ((decimal)(item.total_disc ?? 0))) / 100;
                                extras.Price = ((-1) * ((decimal)item.total)) / 100;

                                TotalPrice += (-1) * ((decimal)(item.amount * item.qty) / 100);
                            }
                            else
                            {
                                //extras.Price = ((decimal)(item.total_disc ?? 0)) / 100;
                                extras.Price = ((decimal)item.total) / 100;

                                TotalPrice += (decimal)(item.amount * item.qty) / 100;
                            }
                            
                        }
                        else
                        {
                            extras.Price = (decimal)item.total / 100;

                            TotalPrice += (decimal)(item.amount * item.qty) / 100;
                        }
                        crVat = vats.Find(f => f.Code == item.item_vat);
                        if (crVat != null)
                        {
                            if (crVat.Percentage != 0)
                            {
                                decimal tempNet = Math.Round(extras.Price / (1 + ((crVat.Percentage ?? 0) / 100)), 4);
                                extras.TotalVat = extras.Price - tempNet;

                                TotalVatAmount += extras.Price - tempNet;
                                TotalNetAmount += tempNet;
                            }
                            else
                                extras.TotalVat = 0;
                            extras.RateVat = crVat.Percentage ?? 0;
                        }
                        else
                        {
                            extras.TotalVat = 0;
                            extras.RateVat = crVat.Percentage ?? 0;
                        }
                        extras.NetAmount = extras.Price - extras.TotalVat;
                        extras.ItemsChanged = false;
                        extras.TotalTax = 0;
                        extras.RateTax = 0;

                        lastObj.Extras.Add(extras);

                    }
                }

            }

            //8. Make a DA_OrderModel for post
            DA_OrderModel postOrder = new DA_OrderModel();
            postOrder.Details = new List<DA_OrderDetails>();
            postOrder.Details.AddRange(details);

            AccountModel acc = null;
            bool isPaint = false;
            if (!string.IsNullOrEmpty(model.payment))
            {
                if (model.payment.Length > 2)
                {
                    acc = accounts.Find(f => f.Type == 4);//Credit Card
                    isPaint = true;
                }
                else
                    acc = accounts.Find(f => f.Type == 1); //Cash
            }
            postOrder.AccountType = (short)(acc == null ? -1 : acc.Id); //payment > 2 then card else cash. payment > 2 add commends
            postOrder.AddLoyaltyPoints = false;

            postOrder.AgentNo = "";//No value needed
            postOrder.BillingAddressId = billingAddress != null ? billingAddress.Id : 0;
            postOrder.CustomerId = extCust.Id;
            postOrder.ExtData = JsonConvert.SerializeObject(model.room);
            postOrder.ExtType = (int)ExternalSystemOrderEnum.GoodysOldProject; //????? 6 Goodys
            postOrder.Id = 0;
            postOrder.InvoiceType = (short)(model.payment.Contains("AP") ? 1 : 7);
            postOrder.IsDelay = false;
            postOrder.IsPaid = isPaint;
            postOrder.IsSend = 1;
            postOrder.ItemsChanged = false;
            //postOrder.NetAmount = details.Sum(s => s.NetAmount) + details.Sum(s => s.Extras.Sum(ss => ss.NetAmount));
            postOrder.OrderDate = model.statusTime;
            postOrder.OrderType = OrderTypeStatus.Delivery; //Always Delivery
            postOrder.Origin = (short)(coupons == null || coupons.WEB.Contains("WEB") ? 1 : 2); //????room.WEB = 1, room.MOBILE = 2
            postOrder.Remarks = model.comments; //???? model.room; 
            postOrder.ShippingAddressId = shippingAddress.Id;
            postOrder.Staffid = -1;
            postOrder.Status = OrderStatusEnum.Received; //
            postOrder.StoreCode = model.shopId;
            postOrder.StoreId = store.Id ?? 0;
            postOrder.Price = TotalPrice;// details.Sum(s => s.Total) + details.Sum(s => s.Extras.Sum(ss => ss.Price));
            postOrder.TotalVat = TotalVatAmount;// details.Sum(s => s.TotalVat) + details.Sum(s => s.Extras.Sum(ss => ss.TotalVat));
            postOrder.Total = TotalPrice - TotalDiscount;// postOrder.Price;
            postOrder.NetAmount = TotalPrice - TotalDiscount - TotalVatAmount;// postOrder.Price - postOrder.TotalVat;
            postOrder.Discount = TotalDiscount;
            postOrder.TotalTax = 0;
            postOrder.ExtObj = JsonConvert.SerializeObject(model);
            postOrder.EstBillingDate = model.statusTime.AddMinutes(store.DeliveryTime ?? 0);
            postOrder.TakeoutDate = model.statusTime.AddMinutes(store.TakeOutTime ?? 0);

            //9. insert new model
            long orderId = orderFlow.InsertOrder(dBInfo, postOrder, postOrder.CustomerId);

            //10. Fetch new order to return values to main model
            DA_ExtOrderModel newOrder = orderFlow.GetOrderById(dBInfo, orderId);
            if (newOrder != null)
            {
                //Add Loyalty to db
                if (loyalty != null)
                {
                    loyalty.DAOrderId = newOrder.OrderModel.Id;
                    loyaltyTasks.InsertModel(dBInfo, loyalty);
                }


                model.orderno = (int)newOrder.OrderModel.OrderNo;
                model.customer.customerid = newOrder.OrderModel.CustomerId.ToString();
                model.customer.store = model.shopId;
            }
            else
                return null;

            return model;
        }
    }
}
