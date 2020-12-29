using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs.Delivery
{
    public class DeliveryCustomersDTO : IDeliveryCustomersDAO
    {
        IGenericDAO<Delivery_CustomersDTO> genCustomer;
        IGenericDAO<Delivery_CustomersTypesDTO> genCustomersTypes;

        public DeliveryCustomersDTO(IGenericDAO<Delivery_CustomersDTO> _genCustomer, IGenericDAO<Delivery_CustomersTypesDTO> _genCustomersTypes)
        {
            this.genCustomer = _genCustomer;
            this.genCustomersTypes = _genCustomersTypes;
        }
        public DeliveryCustomerLookupModel GetLookups(IDbConnection db)
        {
            Tuple<List<Delivery_CustomersTypesDTO>, List<Delivery_PhoneTypesDTO>, List<Delivery_AddressTypesDTO>, List<PricelistDTO>>
                tuple = genCustomersTypes.Select4Queries<Delivery_PhoneTypesDTO, Delivery_AddressTypesDTO, PricelistDTO>(
                db,
                "select * from Delivery_CustomersTypes",
                "select * from Delivery_PhoneTypes",
                "select * from Delivery_AddressTypes",
                "select * from Pricelist where (isDeleted is null or isDeleted != 1) and Status = 1",
                null);
            DeliveryCustomerLookupModel res = new DeliveryCustomerLookupModel
            {
                CustomerType = AutoMapper.Mapper.Map<List<DeliveryCustomerTypeModel>>(tuple.Item1),
                PhoneType = AutoMapper.Mapper.Map<List<DeliveryPhoneTypesModel>>(tuple.Item2),
                AddressType = AutoMapper.Mapper.Map<List<DeliveryAddressTypesModel>>(tuple.Item3),
                Pricelist = AutoMapper.Mapper.Map<List<PricelistModel>>(tuple.Item4)
            };

            return res;
        }

        /// <summary>
        /// Select Customer By Provided Id and Connection
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id">Delivery_Customer Id</param>
        /// <returns> Single DTO of Delivery Customer </returns>
        public Delivery_CustomersDTO SelectById(IDbConnection db, long Id)
        {
            return genCustomer.Select(db, "where ID = @CustomerId and isnull(IsDeleted , 0) = 0 ", new { CustomerId = Id }).FirstOrDefault();
        }

        /// <summary>
        /// Return first Customer that has ExtId rejistered under Extype
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ExtId">External Customer Id is the identification that is used as unique id on the external system </param>
        /// <param name="ExType">The type - Enum defined by the external system</param>
        /// <returns> Single DTO of Delivery Customer First or default </returns>
        public Delivery_CustomersDTO SelectExternalIdType(IDbConnection db, string ExtId, int? ExType)
        {
            string wq = "where ExtCustId = @EXCustomerId  and isnull(IsDeleted , 0) = 0 and ExtType = @EXType ";
            return genCustomer.Select(db, wq, new { EXCustomerId = ExtId, EXType = ExType }).FirstOrDefault();
        }
















        /// <summary>
        /// Provided a Phone LIST  model and a customer id 
        /// loops over list to 
        /// if ID == 0 AND Deleted false then it creates new 
        /// if ISDELETEd true and ID != 0 then deletes it 
        /// ELSE if non of those 2 it updates
        /// 
        /// ON DELETE if there is a shippingDetail or Hist with this there is no problem as there is no id mapped
        /// </summary>
        /// <param name="db"></param>
        /// <param name="addresses">LIST with delivery ship adresses </param>
        /// <param name="CustId"></param>
        public void UpdateCustomerPhones(IDbConnection db, List<DeliveryCustomersPhonesModel> modelPhones, long CustId)
        {

            try
            {
                if (modelPhones == null)
                {
                    return;
                }
                foreach (DeliveryCustomersPhonesModel phone in modelPhones)
                {
                    Delivery_CustomersPhonesDTO p = AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone);
                    //Add
                    if (phone.IsDeleted != true && phone.ID == 0)
                    {
                        p.CustomerID = CustId;
                        long rph = db.Insert<long>(p); //genPhone.Insert(db, p);
                    }
                    //Delete
                    else if (phone.IsDeleted == true && phone.ID != 0)
                    {
                        //int billacnt = db.RecordCount<InvoiceShippingDetailsDTO>(" where Phone = @phone ", new { phone = phone.ID });
                        //int billacnth = db.RecordCount<InvoiceShippingDetails_HistDTO>(" where Phone = @phone ", new { phone = phone.ID });
                        int dbr = db.Delete(p);  //genPhone.Delete(db, p);
                    }
                    //Update
                    else if (phone.IsDeleted != true && phone.ID != 0)
                    {
                        long up = db.Update(p); //genPhone.Update(db, p);
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Provided a billing address LIST  model and a customer id 
        /// loops over list to 
        /// if ID == 0 AND Deleted false then it creates new 
        /// if ISDELETEd true and ID != 0 then deletes it 
        /// ELSE if non of those 2 it updates
        /// 
        /// ON DELETE if there is a shippingDetail or Hist with this then it updates as is deleted
        /// </summary>
        /// <param name="db"></param>
        /// <param name="addresses">LIST with delivery ship adresses </param>
        /// <param name="CustId"></param>
        public void UpdateBillindAddresses(IDbConnection db, List<DeliveryCustomersBillingAddressModel> addresses, long CustId)
        {
            try
            {
                if (addresses == null)
                {
                    return;
                }
                foreach (DeliveryCustomersBillingAddressModel adr in addresses)
                {
                    Delivery_CustomersBillingAddressDTO p = AutoMapper.Mapper.Map<Delivery_CustomersBillingAddressDTO>(adr);
                    //Add
                    if (adr.IsDeleted != true && adr.ID == 0)
                    {
                        p.CustomerID = CustId;
                        long rph = db.Insert<long>(p); //genBillAddress.Insert(db, p);
                    }
                    //Delete
                    else if (adr.IsDeleted == true && adr.ID != 0)
                    {
                        int billacnt = db.RecordCount<InvoiceShippingDetailsDTO>(" where BillingAddressId = @baid ", new { baid = adr.ID });
                        int billacnth = db.RecordCount<InvoiceShippingDetails_HistDTO>(" where BillingAddressId = @baid ", new { baid = adr.ID });
                        if (billacnt == 0 && billacnth == 0) { int dbr = db.Delete(p); } else { long up = db.Update(p); }
                        // genBillAddress.Delete(db, p);
                    }
                    //Update
                    else if (adr.IsDeleted != true && adr.ID != 0)
                    {
                        long up = db.Update(p); //genBillAddress.Update(db, p);
                    }
                }
            }
            catch (Exception ex) { throw ex; };
        }

        /// <summary>
        /// Provided a shipping address LIST  model and a customer id 
        /// loops over list to 
        /// if ID == 0 AND Deleted false then it creates new 
        /// if ISDELETEd true and ID != 0 then deletes it 
        /// ELSE if non of those 2 it updates
        /// 
        /// ON DELETE if there is a shippingDetail or Hist with this then it updates as is deleted
        /// </summary>
        /// <param name="db"></param>
        /// <param name="addresses">LIST with delivery ship adresses </param>
        /// <param name="CustId"></param>
        public void UpdateShippingAddresses(IDbConnection db, List<DeliveryCustomersShippingAddressModel> addresses, long CustId)
        {
            try
            {
                if (addresses == null) { return; }
                foreach (DeliveryCustomersShippingAddressModel adr in addresses)
                {
                    Delivery_CustomersShippingAddressDTO p = AutoMapper.Mapper.Map<Delivery_CustomersShippingAddressDTO>(adr);
                    //Add
                    if (adr.IsDeleted != true && adr.ID == 0)
                    {
                        p.CustomerID = CustId;
                        long rph = db.Insert<long>(p);// genShipAddress.Insert(db, p);
                    }
                    //Delete
                    else if (adr.IsDeleted == true && adr.ID != 0)
                    {
                        int shipacnt = db.RecordCount<InvoiceShippingDetailsDTO>(" where ShippingAddressId = @said ", new { said = adr.ID });
                        int shipacnth = db.RecordCount<InvoiceShippingDetails_HistDTO>(" where ShippingAddressId = @said ", new { said = adr.ID });
                        if (shipacnt == 0 && shipacnth == 0) { int dbr = db.Delete(p); } else { long up = db.Update(p); }
                    }
                    //Update
                    else if (adr.IsDeleted != true && adr.ID != 0)
                    {
                        long up = db.Update(p);// genShipAddress.Update(db, p);
                    }
                }
            }
            catch (Exception ex) { throw ex; };
        }


        public void UpsertSearchCustomerPhones(IDbConnection db, List<DeliveryCustomersPhonesModel> modelPhones, long CustId)
        {

            try
            {
                //if empty skip
                if (modelPhones == null || modelPhones.Count() < 1)
                {
                    return;
                }
                //all unselected
                long allunselected = db.Execute("UPDATE Delivery_CustomersPhones SET IsSelected = 0 where CustomerID = @CustomerID ", new { CustomerID = CustId });
                foreach (DeliveryCustomersPhonesModel phone in modelPhones)
                {
                    //CAUTION CARE Difference  of DTO.phonetype FK not null and Model where phonetype nullable
                    Delivery_CustomersPhonesDTO loaded = db.GetList<Delivery_CustomersPhonesDTO>("where  PhoneNumber = @cphone and CustomerID = @CustomerID ", new { cphone = phone.PhoneNumber.Trim(), CustomerID = CustId }).FirstOrDefault();
                    if (loaded != null)
                    {
                        if (phone.PhoneType == null) { phone.PhoneType = loaded.PhoneType; }
                        Delivery_CustomersPhonesDTO p = AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone);
                        //Update
                        p.ID = loaded.ID;
                        p.CustomerID = loaded.CustomerID;
                        long up = db.Update(p);
                    }
                    else
                    {
                        if (phone.PhoneType == null) { Delivery_PhoneTypesDTO pt = db.GetList<Delivery_PhoneTypesDTO>("", new { }).FirstOrDefault(); phone.PhoneType = pt.ID; }
                        Delivery_CustomersPhonesDTO p = AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone);
                        //Insert
                        p.CustomerID = CustId;
                        long rph = db.Insert<long>(p);
                    }
                }
            }
            catch (Exception e) { throw e; }
        }
        public void UpsertSearchBillindAddresses(IDbConnection db, List<DeliveryCustomersBillingAddressModel> addresses, long CustId)
        {
            try
            {
                if (addresses == null || addresses.Count() < 1)
                {
                    return;
                }
                long allunselected = db.Execute("UPDATE Delivery_CustomersBillingAddress SET IsSelected = 0 where CustomerID = @CustomerID ", new { CustomerID = CustId });

                foreach (DeliveryCustomersBillingAddressModel adr in addresses)
                {
                    string wq = "where CustomerID = @CustomerID and AddressStreet = @astr and AddressNo = @ano ";
                    if (!String.IsNullOrEmpty(adr.City)) { wq += " and City = @acity "; } else { wq += " and isnull(City , '') = '' "; }
                    if (!String.IsNullOrEmpty(adr.Zipcode)) { wq += " and Zipcode = @azip "; } else { wq += " and isnull(Zipcode , '') = '' "; }

                    Delivery_CustomersBillingAddressDTO loaded = db.GetList<Delivery_CustomersBillingAddressDTO>(wq
                        , new
                        {
                            astr = adr.AddressStreet.Trim(),
                            ano = (adr.AddressNo ?? "").Trim(),
                            acity = (adr.City ?? "").Trim(),
                            azip = (adr.Zipcode ?? "").Trim(),
                            CustomerID = CustId
                        }).FirstOrDefault();



                    if (loaded != null)
                    {
                        if (adr.Type == null)
                        {
                            adr.Type = loaded.Type;
                        }
                        Delivery_CustomersBillingAddressDTO p = AutoMapper.Mapper.Map<Delivery_CustomersBillingAddressDTO>(adr);
                        p.ID = loaded.ID;
                        p.CustomerID = loaded.CustomerID;
                        long up = db.Update(p);
                    }
                    else
                    {
                        if (adr.Type == null)
                        {
                            Delivery_AddressTypesDTO pt = db.GetList<Delivery_AddressTypesDTO>("", new { }).FirstOrDefault();
                            adr.Type = pt.ID;
                        }
                        Delivery_CustomersBillingAddressDTO p = AutoMapper.Mapper.Map<Delivery_CustomersBillingAddressDTO>(adr);
                        p.CustomerID = CustId;
                        long rph = db.Insert<long>(p);
                    }
                }
            }
            catch (Exception ex) { throw ex; };
        }
        public void UpsertSearchShippingAddresses(IDbConnection db, List<DeliveryCustomersShippingAddressModel> addresses, long CustId)
        {
            try
            {
                if (addresses == null || addresses.Count() < 1)
                {
                    return;
                }
                long allunselected = db.Execute("UPDATE Delivery_CustomersShippingAddress SET IsSelected = 0 where CustomerID = @CustomerID ", new { CustomerID = CustId });

                foreach (DeliveryCustomersShippingAddressModel adr in addresses)
                {
                    string wq = "where CustomerID = @CustomerID and AddressStreet = @astr and AddressNo = @ano ";
                    if (!String.IsNullOrEmpty(adr.City)) { wq += " and City = @acity "; } else { wq += " and isnull(City , '') = '' "; }
                    if (!String.IsNullOrEmpty(adr.Zipcode)) { wq += " and Zipcode = @azip "; } else { wq += " and isnull(Zipcode , '') = '' "; }

                    Delivery_CustomersShippingAddressDTO loaded = db.GetList<Delivery_CustomersShippingAddressDTO>(wq, new { astr = adr.AddressStreet.Trim(), ano = (adr.AddressNo ?? "").Trim(), acity = (adr.City ?? "").Trim(), azip = (adr.Zipcode ?? "").Trim(), CustomerID = CustId }).FirstOrDefault();

                    if (loaded != null)
                    {
                        if (adr.Type == null)
                        {
                            adr.Type = loaded.Type;
                        }
                        Delivery_CustomersShippingAddressDTO p = AutoMapper.Mapper.Map<Delivery_CustomersShippingAddressDTO>(adr);
                        p.ID = loaded.ID;
                        p.CustomerID = loaded.CustomerID;
                        long up = db.Update(p);
                    }
                    else
                    {
                        if (adr.Type == null)
                        {
                            Delivery_AddressTypesDTO pt = db.GetList<Delivery_AddressTypesDTO>("", new { }).FirstOrDefault();
                            adr.Type = pt.ID;
                        }
                        Delivery_CustomersShippingAddressDTO p = AutoMapper.Mapper.Map<Delivery_CustomersShippingAddressDTO>(adr);
                        p.CustomerID = CustId;
                        long rph = db.Insert<long>(p);
                    }
                }
            }
            catch (Exception ex) { throw ex; };
        }
    }
}
