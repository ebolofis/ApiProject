using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IPricelistDetailDAO //: IGenericDAO<PricelistDetailDTO>
    {

        /// <summary>
        /// Returns first encounter of pricelist detail for selected product and selected pricelist
        /// </summary>
        /// <param name="db"> DB Connection </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <returns> Pricelist detail database model </returns>
        PricelistDetailDTO SelectPricelistDetailForProductAndPricelist(IDbConnection db, long productId, long pricelistId, IDbTransaction dbTransact = null);

        /// <summary>
        /// Updates selected pricelist detail
        /// </summary>
        /// <param name="db"> DB Connection </param>
        /// <param name="pricelistDetail"> Pricelist detail object </param>
        /// <returns> Pricelist detail database model </returns>
        PricelistDetailDTO UpdatePricelistDetail(IDbConnection db, PricelistDetailDTO pricelistDetail);

    }
}
