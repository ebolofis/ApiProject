using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class StaffDAO: IStaffDAO
    {
        IGenericDAO<StaffDTO> genstaff;
        public StaffDAO(IGenericDAO<StaffDTO> _genstaff)
        {
            genstaff = _genstaff;
        }
        /// <summary>
        /// Function uses Gen Staff and Gets Staff by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> Staff  by id  </returns>
        public StaffDTO SelectById(IDbConnection db, long Id)
        {
            return genstaff.Select(db, Id);
        }

        /// <summary>
        /// Returns List of table entries using generic DAO
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isDeleted">If set to true then it returns all</param>
        /// <returns></returns>
        public List<StaffDTO> SelectAll(IDbConnection db, bool isDeleted = false)
        {
            string wq = " where isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                return genstaff.Select(db);
            }
            else
            {
                return genstaff.Select(db, wq, null);
            }
        }
    }
}
