using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System.Data;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class ProductDAO : IProductDAO
    {

        IGenericDAO<ProductDTO> genprod;
        public ProductDAO(IGenericDAO<ProductDTO> _genprod)
        {
            genprod = _genprod;
        }

        /// <summary>
        /// Function uses gen Product and Gets Product by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Pos Info refered to Id asked </returns>
        public ProductDTO SelectById(IDbConnection db, long Id)
        {
            return genprod.Select(db, Id);
        }

        /// <summary>
        /// Function uses gen Product and Gets First Product by code provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="code"></param>
        /// <param name="isDeleted">If set to true then it returns collection from all</param>
        /// <returns></returns>
        public ProductDTO SelectByCode(IDbConnection db, string code, bool isDeleted = false)
        {
            string wq = " and isnull(isdeleted ,0) = 0 ";
            if (isDeleted == true)
            {
                wq = "";
            }
            return genprod.SelectFirst("Select * From Product Where code = @Code " + wq , new { Code = code }, db);
        }
    }
}
