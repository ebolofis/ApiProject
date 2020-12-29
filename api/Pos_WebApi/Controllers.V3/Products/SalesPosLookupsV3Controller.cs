using Microsoft.AspNet.SignalR;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/SalesPosLookups")]
    public class SalesPosLookupsV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        ISalesPosLookupsFlows salesPosLookupsFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public SalesPosLookupsV3Controller(ISalesPosLookupsFlows spl)
        {
            this.salesPosLookupsFlows = spl;
        }

        /// <summary>
        /// Get the set of data POS needs:
        /// <para>1. get communication info from every pms </para> 
        /// <para>2. create sp ReservationInfo and ProtelDepartments for every pms </para>
        /// <para>3. get Posinfo and the related data based on client type </para>
        /// <para>4. get salesTypes (Τύποι πώλησης) </para>
        /// <para>5. get pricelists (και τα SalesType) που αφορούν το συγκεκριμένο pos </para>
        /// <para>6. get the active staff for the specific pos  </para>
        /// <para>7. get storeinfoid (not needed, client already got store.) </para>
        /// <para>8. get all active Accounts(Οι δυνατοί τρόποι πληρωμής) except Credit Cards (Type=4) </para>
        /// <para>9. get Credit Cards and assign them with pms's rooms </para>
        /// <para>10. get hotelInfo (Πληροφορίες επικοινωνίας με το pms) </para>
        /// <para>11. get TransferMappings (αντιστοιχίες μεταξύ τμημάτων του PMS και των δευτερευόντων κατηγοριών των προϊόντων (ProductCategory) για αποστολή χρεώσεων στα τμήματα του PMS) </para>
        /// <para>12. get KitchenInstructions (Μηνύματα προς τη κουζίνα) for specific POS.  </para>
        /// <para>13. determine CustomerPolicy based on HotelInfo.Type of the first record in table HotelInfo </para>
        /// <para>14. set hasCustomers=true if customerpolicy is HotelInfo or Other or PmsInterface </para>
        /// <para>15. get delivery's url for searching customer </para>
        /// <para>16. get RegionLockerProduct (το product ‘Locker’ )included Product table, Vats </para>
        /// <para>17. get allowedboardMeals (δικαιωμένα) </para>
        /// <para>18. get Vats </para>
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="ipAddress">ip η οποία χαρακτηρίζει μοναδικά ένα POS σε μία DB. Για Client POS (type=11) έχει δομή ip,clientPosCode  ex: 1.1.1.1,23</param>
        /// <param name="type">τύπος client. 1: POS, 11: client POS, 10: PDA</param>
        /// <returns>Create new anonymous onbect to return the aquired data</returns>
        [HttpGet, Route("GetPosByIp/storeid/{storeid}/ipAddress/{ipAddress}/type/{type}")]
        public HttpResponseMessage GetPosByIp(string storeid, string ipAddress, int type = 1)
        {
            SalesPosLookupsModelsPreview results = salesPosLookupsFlows.GetPosByIp(DBInfo, storeid, ipAddress, type);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }
    }
}