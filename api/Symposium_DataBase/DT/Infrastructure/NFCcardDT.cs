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
    public class NFCcardDT:INFCcardDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<NFCcardDTO> NFCcardGenericDao;
        IGenericITableDAO<NFCcardDTO> NFCcardGenericTableDao;

        public NFCcardDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<NFCcardDTO> NFCcardGenericDao, IGenericITableDAO<NFCcardDTO> NFCcardGenericTableDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.NFCcardGenericDao = NFCcardGenericDao;
            this.NFCcardGenericTableDao = NFCcardGenericTableDao;
        }

        /// <summary>
        /// Get NFC card
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <returns>List of all nfc card registers</returns>
        public List<NFCcardModel> GetNFCInfo(DBInfoModel Store)
        {
            List<NFCcardDTO> nfccardlist;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                nfccardlist = NFCcardGenericDao.Select(db);
            }

            return AutoMapper.Mapper.Map<List<NFCcardModel>>(nfccardlist);
        }

        /// <summary>
        /// Get first NFC card
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <returns>First or default of NFC model on db context</returns>
        public NFCcardModel GetFirstNFCInfo(DBInfoModel Store)
        {
            NFCcardDTO nfccardlist;

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                nfccardlist = NFCcardGenericDao.SelectFirst(db, "", null);
            }

            return AutoMapper.Mapper.Map<NFCcardModel>(nfccardlist);
        }

        /// <summary>
        /// Upsert NFC card
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="nfcCard">Model to update register</param>
        /// <returns>Id of model updated</returns>
        public long UpdateNFCcard(DBInfoModel Store, NFCcardModel nfcCard)
        {
            NFCcardDTO temp;

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
               temp = NFCcardGenericTableDao.Upsert(db, AutoMapper.Mapper.Map<NFCcardDTO>(nfcCard));
            }

            return temp.Id;
        }

    }
}
