using log4net;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace Pos_WebApi.Customer_Modules
{
    public class Autogrill : ICustomerClass
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// check staff's authorization based on  identification
        /// </summary>
        /// <param name="stIdentification"></param>
        /// <param name="staff">staff</param>
        /// <param name="ActionContext"></param>
        /// <param name="db"></param>
        /// <returns>if staff is Manager thn return true</returns>
        public bool CheckStaffAuthorization(StaffIdentification stIdentification, out Staff staff, HttpActionContext ActionContext, PosEntities db)
        {
            staff = null;
            try
            {
                string staffIdentification = stIdentification.Identification;
                List<Staff> staffList = db.Staff.Where(s => s.Identification == staffIdentification && s.IsDeleted == null).ToList();
                if (staffList.Count() == 0)
                {
                    logger.Warn("No staff with identification " + stIdentification.Identification + "!");
                    return false;
                }
                else if (staffList.Count() > 1)
                {
                    logger.Warn("More than one staff with the same identification " + stIdentification.Identification + "!");
                    return false;
                }
                else
                {
                    Staff selectedStaff = staffList.FirstOrDefault();
                    staff = selectedStaff;
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// check staff's authorization based on username and password
        /// </summary>
        /// <param name="username">staff.code</param>
        /// <param name="password">staff.password</param>
        /// <param name="staff">staff</param>
        /// <param name="ActionContext"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool CheckStaffAuthorization(ref string username, ref string password, out Staff staff, HttpActionContext ActionContext, PosEntities db)
        {
            staff = null;
            try
            {
                string staffUsername = username;
                string staffPassword = password;
                List<Staff> staffList = db.Staff.Where(s => s.Code == staffUsername && s.Password == staffPassword && s.IsDeleted == null).ToList();
                if (staffList.Count() == 0)
                {
                    logger.Warn("No staff with username " + username + "!");
                    return false;
                }
                else if (staffList.Count() > 1)
                {
                    logger.Warn("More than one staff with the same username " + username + "!");
                    return false;
                }
                else
                {
                    Staff selectedStaff = staffList.FirstOrDefault();
                    var staffChecked = CheckFoundStaff(selectedStaff, db);
                    if (staffChecked)
                    {
                        staff = selectedStaff;
                        return true;
                    }
                    else
                    {
                        logger.Info("User with username " + username + " has no rights to perform this action!");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
        }

        private bool CheckFoundStaff(Staff staff, PosEntities db)
        {
            var authorization = from q in db.Staff.Where(w => w.Id == staff.Id)
                                join y in db.StaffAuthorization on q.Id equals y.StaffId
                                join z in db.AuthorizedGroup on y.AuthorizedGroupId equals z.Id
                                select new
                                {
                                    Description = z.Description
                                };
            foreach (var authorizedGroup in authorization)
            {
                if (authorizedGroup.Description.Equals("Manager"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}