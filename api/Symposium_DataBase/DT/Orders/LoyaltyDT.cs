using AutoMapper;
using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Orders;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Orders
{
    public class LoyaltyDT : ILoyaltyDT
    {
        string connectionString;
        IGenericDAO<LoyaltyDTO> dao;
        IUsersToDatabasesXML usersToDatabases;

        public LoyaltyDT(IGenericDAO<LoyaltyDTO> _dao, IUsersToDatabasesXML _usersToDatabases)
        {
            dao = _dao;
            usersToDatabases = _usersToDatabases;
        }

        /// <summary>
        /// Returns a list of Loyalty based on LoyaltyId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="LoyalltyId"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByLoyalltyId(DBInfoModel Store, string LoyalltyId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Mapper.Map<List<LoyaltyModel>>(dao.Select(db, "WHERE LoyalltyId = @LoyalltyId", new { LoyalltyId = LoyalltyId }));
            }
        }

        /// <summary>
        /// Returns a list of Loyalty based on DAOrderId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByDAOrderID(DBInfoModel Store, long DAOrderId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Mapper.Map<List<LoyaltyModel>>(dao.Select(db, "WHERE DAOrderId = @DAOrderId", new { DAOrderId = DAOrderId }));
            }
        }

        /// <summary>
        /// Returns a list of Loyalty based on Invoicesid
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByInvoicesId(DBInfoModel Store, long InvoicesId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Mapper.Map<List<LoyaltyModel>>(dao.Select(db, "WHERE InvoicesId = @InvoicesId", new { InvoicesId = InvoicesId }));
            }
        }

        /// <summary>
        /// Returns a list of Loyalty based on Coupon Code
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CouponCode"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByCouponCode(DBInfoModel Store, string CouponCode)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Mapper.Map<List<LoyaltyModel>>(dao.Select(db, "WHERE CouponCode = @CouponCode", new { CouponCode = CouponCode }));
            }
        }

        /// <summary>
        /// Returns a list of Loyalty based on Gift Card Code
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="GiftcardCode"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByGiftcardCode(DBInfoModel Store, string GiftcardCode)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Mapper.Map<List<LoyaltyModel>>(dao.Select(db, "WHERE GiftcardCode = @GiftcardCode", new { GiftcardCode = GiftcardCode }));
            }
        }

        /// <summary>
        /// Returns a list of Loyalty based on Channel
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByChannel(DBInfoModel Store, string Channel)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Mapper.Map<List<LoyaltyModel>>(dao.Select(db, "WHERE Channel = @Channel", new { Channel = Channel }));
            }
        }

        /// <summary>
        /// Returns a list of Loyalty based on Coupon Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CouponType"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByCouponType(DBInfoModel Store, string CouponType)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Mapper.Map<List<LoyaltyModel>>(dao.Select(db, "WHERE CouponType = @CouponType", new { CouponType = CouponType }));
            }
        }

        /// <summary>
        /// update loyalty model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, LoyaltyModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dao.Update(db, Mapper.Map<LoyaltyDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new loyalty on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, LoyaltyModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dao.Insert(db, Mapper.Map<LoyaltyDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// Delete a loyalty from db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int DeleteModel(DBInfoModel Store, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dao.Delete(db, Id);
            }
        }
    }
}
