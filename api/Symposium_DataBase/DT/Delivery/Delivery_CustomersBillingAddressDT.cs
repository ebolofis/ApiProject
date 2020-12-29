using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class Delivery_CustomersBillingAddressDT : IDelivery_CustomersBillingAddressDT
    {
        IGenericDAO<Delivery_CustomersBillingAddressDTO> dt;
        IUsersToDatabasesXML users;
        string connectionString;

        public Delivery_CustomersBillingAddressDT(IGenericDAO<Delivery_CustomersBillingAddressDTO> dt,IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }


        /// <summary>
        /// Return's a Delivery Billing Nodel using External Key
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExtKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public Delivery_CustomersBillingAddressModel GetModelByExternalKey(DBInfoModel Store, long CustomerId, string ExtKey, ExternalSystemOrderEnum ExtType)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<Delivery_CustomersBillingAddressModel>(dt.SelectFirst(db,
                    "WHERE CustomerID = @CustomerID AND ExtKey = @ExtKey AND ExtType = @ExtType",
                    new { CustomerID = CustomerId, ExtKey = ExtKey, ExtType = (int)ExtType }));
            }
        }

        public DeliveryCustomersBillingAddressModel GetAddressByLatLng(DBInfoModel Store, string latitude, string longitude)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<DeliveryCustomersBillingAddressModel>(dt.SelectFirst(db,
                    "WHERE Latitude = @lat AND Longtitude = @lng AND IsDeleted is null",
                    new { lat = latitude, lng = longitude }));
            }
        }

    }
}
