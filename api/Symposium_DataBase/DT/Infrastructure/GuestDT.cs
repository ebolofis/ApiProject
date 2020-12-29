using log4net;
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
    public class GuestDT : IGuestDT
    {
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string connectionString;
        IGenericDAO<GuestDTO> genGuest;
        IGuestDAO guestdao;
        IUsersToDatabasesXML usersToDatabases;

        public GuestDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<GuestDTO> _genGuest , IGuestDAO _guestdao)
        {
            this.usersToDatabases = usersToDatabases;
            this.guestdao = _guestdao;
            this.genGuest = _genGuest;
        }

        /// <summary>
        /// Updates Guest from deliverycustomer model using GuestFromDeliveryCustomer below
        /// if guest with profileno like model.ID then updates guest wlse it inserts new register
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public GuestModel UpdateGuestFromDeliveryCustomer(DBInfoModel Store, DeliveryCustomerModel model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            long retid = 0;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    retid = guestdao.upsertGuestFromDeliveryCustomer(db, model);
                    return AutoMapper.Mapper.Map<GuestModel>(genGuest.Select(db, retid));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERUPDATEGUEST + " " + model.ID);
                }
            }
        }

        /// <summary>
        /// Return's a guest model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public GuestModel GetGuestById(DBInfoModel Store, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<GuestModel>(genGuest.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }));
            }
        }

        /// <summary>
        /// Return's A Guest using External Key (ProfileNo)
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ProfileNo"></param>
        /// <returns></returns>
        public GuestModel GetGuestByExternalKey(DBInfoModel Store, int ProfileNo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<GuestModel>(genGuest.SelectFirst(db, "WHERE ProfileNo = @ProfileNo", new { ProfileNo = ProfileNo }));
            }
        }


    }
}
