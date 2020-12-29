using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_PhoneticsDT
    {
        /// <summary>
        /// Fill columns Phonetics and PhoneticsArea to ALL rows into table DA_Addresses with the respective phonetic values.
        /// </summary>
         void CreateAllAddressPhonetics(DBInfoModel dbinfo);
        /// <summary>
        /// Search addresses using phonetic data
        /// </summary>
        /// <param name="DBInfoModel"></param>
        /// <param name="search">Address[, Area]</param>
        /// <returns></returns>
        List<DA_AddressModel> SearchAddressPhonetics(DBInfoModel dbinfo, string search);


        /// <summary>
        /// construct sql query to search da_addresses table
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        string ConstructAddressSqlQuery(string search);

        /// <summary>
        /// construct sql query to search da_customers table
        /// </summary>
        /// <param name="search">adress's words<</param>
        /// <returns></returns>
        string ConstructCustomersSqlQuery(string search);



    }
}
