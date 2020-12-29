using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class PredefinedCreditsFlows : IPredefinedCreditsFlows
    {
        IPredefinedCreditsTasks predefinedCreditsTasks;
        public PredefinedCreditsFlows(IPredefinedCreditsTasks pc)
        {
            this.predefinedCreditsTasks = pc;
        }




        /// <summary>
        /// return all predefined credits (προκαθορισμένα  ποσά  για Ticket Restaurant/κουπόνια). 
        ///  GET api/PredefinedCredits
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns>list of PredefinedCredits</returns>
        public PredefinedCreditsModelsPreview GetPredefinedCredits(DBInfoModel dbInfo, string storeid)
        {
            // get the results
            PredefinedCreditsModelsPreview getPages = predefinedCreditsTasks.GetPredefinedCredits(dbInfo, storeid);

            return getPages;
        }
    }
}
