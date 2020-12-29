using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface ITableDT
    {

        /// <summary>
        /// Returns A Table Model By Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        TablesModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null);
    }
}
