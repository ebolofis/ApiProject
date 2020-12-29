using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class PricelistDetailDAO : IPricelistDetailDAO
    {
        IGenericDAO<PricelistDetailDTO> genPriceDet;
        IGenericITableDAO<PricelistDetailDTO> genTPriceDet;

        public PricelistDetailDAO(IGenericDAO<PricelistDetailDTO> _genPriceDet, IGenericITableDAO<PricelistDetailDTO> _genTPriceDet)
        {
            genPriceDet = _genPriceDet;
            genTPriceDet = _genTPriceDet;
        }

        /// <summary>
        /// Returns first encounter of pricelist detail for selected product and selected pricelist
        /// </summary>
        /// <param name="db"> DB Connection </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <returns> Pricelist detail database model </returns>
        public PricelistDetailDTO SelectPricelistDetailForProductAndPricelist(IDbConnection db, long productId, long pricelistId, IDbTransaction dbTransact = null)
        {
            return genPriceDet.SelectFirst(db, "where ProductId = @prodId and PricelistId = @prlId", new { prodId = productId, prlId = pricelistId }, dbTransact);
        }

        /// <summary>
        /// Updates selected pricelist detail
        /// </summary>
        /// <param name="db"> DB Connection </param>
        /// <param name="pricelistDetail"> Pricelist detail object </param>
        /// <returns> Pricelist detail database model </returns>
        public PricelistDetailDTO UpdatePricelistDetail(IDbConnection db, PricelistDetailDTO pricelistDetail)
        {
            return genTPriceDet.Upsert(db, pricelistDetail);
        }

    }
}
