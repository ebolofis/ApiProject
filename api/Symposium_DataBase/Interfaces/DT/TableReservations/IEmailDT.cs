using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations
{
    public interface IEmailDT
    {
        /// <summary>
        /// Send an Email to Customers
        /// </summary>
        /// <returns></returns>
        string SendEmailToCustomers(DBInfoModel Store, ExtendedReservetionModel Model);
    }
}
