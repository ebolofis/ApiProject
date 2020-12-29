using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface ISalesPosLookupsTasks
    {
        /// <summary>
        /// Get the set of data POS needs:
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="ipAddress">ip η οποία χαρακτηρίζει μοναδικά ένα POS σε μία DB. Για Client POS (type=11) έχει δομή ip,clientPosCode  ex: 1.1.1.1,23</param>
        /// <param name="type">τύπος client. 1: POS, 11: client POS, 10: PDA</param>
        /// <returns>Create new anonymous onbect to return the aquired data</returns>
        SalesPosLookupsModelsPreview GetPosByIp(DBInfoModel Store, string storeid, string ipAddress, int type = 1);
    }
}
