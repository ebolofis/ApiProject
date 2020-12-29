using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems;
using Symposium.WebApi.DataAccess.Interfaces.DAO.Goodys;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.Goodys;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Goodys
{
    public class GoodysDT : IGoodysDT
    {
        string connectionString;
        IGoodysDAO GoodysDAO;
        IUsersToDatabasesXML usersToDatabases;
        IDA_CustomerDT customerDao;
        IDA_AddressesDT addressDao;

        public GoodysDT(IGoodysDAO GoodysDAO, IUsersToDatabasesXML usertodbs, 
            IDA_CustomerDT customerDao, IDA_AddressesDT addressDao)
        {
            this.GoodysDAO = GoodysDAO;
            this.usersToDatabases = usertodbs;
            this.customerDao = customerDao;
            this.addressDao = addressDao;
        }
        public long GetOpenOrders(DBInfoModel DBInfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return   GoodysDAO.GetOpenOrders(db);
            }
             
        }

        
        public OrderModel GetGoodysExternalOrderID(long InvoiceId, DBInfoModel dbInfo)
        {
            OrderDTO goodysOrder;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                goodysOrder = GoodysDAO.GetGoodysExternalOrderID(db, InvoiceId);
            }
            return AutoMapper.Mapper.Map<OrderModel>(goodysOrder);
        }

        /// <summary>
        /// Return's a Login responce model
        /// AccountId as External key ends with "GO" "Goodys Old"
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        /// 
        public long GetInvoiceid(DBInfoModel dbinfo, long orderno)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return GoodysDAO.GetInvoiceid(db, orderno);
            }
        }
        public GoodysLoginResponceModel GetLoginResponceModel(DBInfoModel dbInfo, string AccountId)
        {
            long custId;
            long.TryParse(AccountId, out custId);
            DACustomerModel cust;
            if (custId < 1)
                cust = customerDao.GetCustomer(dbInfo, AccountId + "GO");
            else
                cust = customerDao.GetCustomer(dbInfo, custId);
            if (cust == null) //External key is numeric but not db Id. Sow search for customer based on External key plus GO
                cust = customerDao.GetCustomer(dbInfo, custId.ToString() + "GO");
            if (cust == null)
                return null;

            GoodysLoginResponceModel res = new GoodysLoginResponceModel();
            res.firstName = cust.FirstName;
            res.accountStatus = cust.IsDeleted ? "Enactive" : "Active";
            res.id = cust.Id.ToString();
            res.lastName = cust.LastName;
            res.location = cust.Phone1;
            if (string.IsNullOrEmpty(res.location))
                res.location = cust.Phone2;
            if (string.IsNullOrEmpty(res.location))
                res.location = cust.Mobile;
            res.name = !string.IsNullOrEmpty(cust.Email) ? cust.Email : cust.LastName;
            res.type = "WEB";
            res.addressList = new List<GoodysLoginAddressResponceModel>();

            List<DA_AddressModel> addresses = addressDao.getCustomerAddresses(dbInfo, cust.Id);
            foreach (DA_AddressModel item in addresses)
            {
                if (item.IsDeleted)
                    continue;
                GoodysLoginAddressResponceModel tmp = new GoodysLoginAddressResponceModel();
                tmp.addressComment = item.Notes;
                tmp.addressId = item.Id.ToString();
                tmp.addressNAme = item.FriendlyName;
                tmp.addresssFloor = item.Floor;
                tmp.county = item.Area;
                tmp.isPrimary = item.Id == cust.LastAddressId ? "Y" : "N";
                tmp.NNHome = item.AddressNo;
                tmp.phoneNumber = res.location;
                tmp.postalCode = item.Zipcode;
                tmp.state = item.Area;
                tmp.streetAddress = item.AddressStreet;
                tmp.streetAlias = item.FriendlyName;
                tmp.isShipping = item.AddressType == 0; // item.isShipping ?? true;
                res.addressList.Add(tmp);
            }
            if (res.addressList != null && res.addressList.Count > 0)
            {
                var fld = res.addressList.Find(f => f.isPrimary == "Y");
                if (fld == null)
                    res.addressList[0].isPrimary = "Y";
            }

            return res;
        }

    }
}
