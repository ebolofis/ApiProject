using Symposium.Models.Models;
using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Goodys
{
    public interface IGoodysDT
    {
        OrderModel GetGoodysExternalOrderID(long InvoiceId, DBInfoModel dbInfo);

        /// <summary>
        /// Return's a Login responce model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        
        GoodysLoginResponceModel GetLoginResponceModel(DBInfoModel dbInfo, string AccountId);
        long GetOpenOrders(DBInfoModel DBInfo);
        long GetInvoiceid(DBInfoModel dbinfo, long orderno);
    }
}