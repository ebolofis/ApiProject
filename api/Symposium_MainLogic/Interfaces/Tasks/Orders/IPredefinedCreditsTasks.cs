using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IPredefinedCreditsTasks
    {
        /// <summary>
        /// Get all predefined credits (προκαθορισμένα  ποσά  για Ticket Restaurant/κουπόνια). 
        ///  GET api/PredefinedCredits
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns>list of PredefinedCredits</returns>
        PredefinedCreditsModelsPreview GetPredefinedCredits(DBInfoModel Store, string storeid);
    }
}
