using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IDepartmentDT
    {
        /// <summary>
        /// Return's Department Description
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        string GetDepartmentDescr(DBInfoModel Store, long DepartmentId);
    }
}
