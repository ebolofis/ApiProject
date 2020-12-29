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
    public class Delivery_CustomersShippingAddressDT : IDelivery_CustomersShippingAddressDT
    {
        IGenericDAO<Delivery_CustomersShippingAddressDTO> dt;
        IUsersToDatabasesXML users;
        string connectionString;

        public Delivery_CustomersShippingAddressDT(IGenericDAO<Delivery_CustomersShippingAddressDTO> dt,IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Return's A Model bases on an External Key and External Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExternalKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public Delivery_CustomersShippingAddressModel GetModelByExternalKey(DBInfoModel Store, long CustomerId, string ExternalKey, ExternalSystemOrderEnum ExtType)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int exType = (int)ExtType;
                return AutoMapper.Mapper.Map<Delivery_CustomersShippingAddressModel>(dt.SelectFirst(db, 
                    "WHERE CustomerID = @CustomerID AND ExtKey = @ExtKey AND ExtType = @ExtType", 
                    new { CustomerID = CustomerId, ExtKey = ExternalKey, ExtType = exType }));
            }
        }

        public DeliveryCustomersShippingAddressModel GetAddressByLatLng(DBInfoModel Store, string latitude, string longitude)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<DeliveryCustomersShippingAddressModel>(dt.SelectFirst(db,
                    "WHERE Latitude = @lat AND Longtitude = @lng AND IsDeleted is null",
                    new { lat = latitude, lng = longitude }));
            }
        }

    }
}
