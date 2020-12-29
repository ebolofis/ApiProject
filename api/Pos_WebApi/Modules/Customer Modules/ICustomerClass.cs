using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using log4net;
using Pos_WebApi.Models;

namespace Pos_WebApi.Customer_Modules
{
   public interface ICustomerClass
    {
        /// <summary>
        /// check staff's authorization based on staffid or identification
        /// </summary>
        /// <param name="stIdentification"></param>
        /// <param name="staff">staff</param>
        /// <param name="ActionContext"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        bool CheckStaffAuthorization(StaffIdentification stIdentification, out Staff staff, HttpActionContext ActionContext, PosEntities db);

        /// <summary>
        /// check staff's authorization based on username and password
        /// </summary>
        /// <param name="username">staff.code</param>
        /// <param name="password">staff.password</param>
        /// <param name="staff">staff</param>
        /// <param name="ActionContext"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        bool CheckStaffAuthorization(ref string username, ref string password, out Staff staff, HttpActionContext ActionContext, PosEntities db);

    }
}
