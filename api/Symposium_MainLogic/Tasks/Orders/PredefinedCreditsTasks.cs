using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class PredefinedCreditsTasks : IPredefinedCreditsTasks
    {
        IPredefinedCreditsDT predefinedCreditsDT;
        public PredefinedCreditsTasks(IPredefinedCreditsDT pcDT)
        {
            this.predefinedCreditsDT = pcDT;
        }

        /// <summary>
        /// return all predefined credits (προκαθορισμένα  ποσά  για Ticket Restaurant/κουπόνια). 
        ///  GET api/PredefinedCredits
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns>list of PredefinedCredits</returns>
        public PredefinedCreditsModelsPreview GetPredefinedCredits(DBInfoModel Store, string storeid)
        {
            // get the results
            PredefinedCreditsModelsPreview getPages = predefinedCreditsDT.GetPredefinedCredits(Store, storeid);

            return getPages;
        }
    }
}
