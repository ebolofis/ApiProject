using Symposium.Models.Models;
using Symposium.Models.Models.Promos;
using Symposium.WebApi.DataAccess.Interfaces.DT.Promos;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Promos
{
    /// <summary>
    /// Marketing promotions
    /// </summary>
    public class PromosTasks : IPromosTasks
    {
        IVodafone11DT vodafone11DT;


        public PromosTasks(IVodafone11DT vodafone11DT)
        {
            this.vodafone11DT = vodafone11DT;
        }

        /// <summary>
        /// Get Vodafone 1+1 promos
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <returns></returns>
        public List<Vodafone11Model> GetVodafone11Promos(DBInfoModel dbInfo)
        {
            return vodafone11DT.GetVodafone11Promos(dbInfo);
        }
        public List<Vodafone11Model> GetVodafone11HeaderPromos(DBInfoModel dbInfo)
        {
            return vodafone11DT.GetVodafone11HeaderPromos(dbInfo);
        }
        public List<Vodafone11ProdCategoriesModel> GetVodafone11DetailsPromos(DBInfoModel dbinfo, long HeaderId)
        {
            return vodafone11DT.GetVodafone11DetailsPromos(dbinfo, HeaderId);
        }
        public List<Vodafone11ProdCategoriesModel> GetVodafoneAll11DetailsPromos(DBInfoModel dbinfo)
        {
            return vodafone11DT.GetVodafoneAll11DetailsPromos(dbinfo);
        }
        //Get OneOfVodafone11 Header/Detail
        public Vodafone11Model GetOneOffVodafone11Promos(DBInfoModel dbinfo, long Id)
        {
            return vodafone11DT.GetOneOffVodafone11Promos(dbinfo, Id);
        }
        //Insert Vodafone 1+1 Header
        public long InsertVodafone11Promos(DBInfoModel dbInfo, Vodafone11Model model)
        {
            long newPromoId = vodafone11DT.InsertVodafone11Promos(dbInfo, model);

            return newPromoId;
        }
        //Insert Vodafone 1+1 Detail
        public long InsertVodafone11Details(DBInfoModel dbInfo, Vodafone11ProdCategoriesModel model)
        {
            long detailsid = vodafone11DT.InsertVodafone11Details(dbInfo, model);

            return detailsid;
        }
        // Delete Vodafone 1+1 Header
        public long DeleteVodafone11(DBInfoModel dbInfo, long id)
        {
            long DeletedHeaderId = vodafone11DT.DeleteVodafone11(dbInfo, id);
            return DeletedHeaderId;
        }

        //Delete Vodafone 1+1 Detail
        public long DeleteVodafone11Detail(DBInfoModel dbinfo, long id)
        {
            long DeletedDetailId = vodafone11DT.DeleteVodafone11Detail(dbinfo, id);
            return DeletedDetailId;
        }

        public Vodafone11Model UpdateVodafone11Promos(DBInfoModel dbinfo, Vodafone11Model model)
        {
            return vodafone11DT.UpdateVodafone11Promos(dbinfo, model);

        }
        public Vodafone11ProdCategoriesModel UpdateVodafone11DetailsPromos(DBInfoModel dbinfo, Vodafone11ProdCategoriesModel model)
        {
            return vodafone11DT.UpdateVodafone11DetailsPromos(dbinfo, model);
        }
    }
}
