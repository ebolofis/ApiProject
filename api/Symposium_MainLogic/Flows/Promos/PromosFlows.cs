using Symposium.Models.Models;
using Symposium.Models.Models.Promos;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Promos;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Promos
{
    /// <summary>
    /// Marketing promotions
    /// </summary>
    public class PromosFlows : IPromosFlows
    {

        IPromosTasks promosTasks;



        public PromosFlows(IPromosTasks promosTasks)
        {
            this.promosTasks = promosTasks;
        }

        /// <summary>
        /// Get Vodafone 1+1 promos
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <returns></returns>
        public List<Vodafone11Model> GetVodafone11Promos(DBInfoModel dbInfo)
        {
            return promosTasks.GetVodafone11Promos(dbInfo);
        }
        //Get 1 Header + Details

        public List<Vodafone11Model> GetVodafone11HeaderPromos(DBInfoModel dbInfo)
        {
            return promosTasks.GetVodafone11HeaderPromos(dbInfo);
        }
        public List<Vodafone11ProdCategoriesModel> GetVodafone11DetailsPromos(DBInfoModel dbinfo, long HeaderId)
        {
            return promosTasks.GetVodafone11DetailsPromos(dbinfo, HeaderId);
        }
        public List<Vodafone11ProdCategoriesModel> GetVodafoneAll11DetailsPromos(DBInfoModel dbinfo)
        {
            return promosTasks.GetVodafoneAll11DetailsPromos(dbinfo);
        }
        public Vodafone11Model GetOneOffVodafone11Promos(DBInfoModel dbinfo, long id)
        {
            return promosTasks.GetOneOffVodafone11Promos(dbinfo, id);
        }
        // Insert Vodafone 1+1 Header
        public long InsertVodafone11Promos(DBInfoModel dbInfo, Vodafone11Model model)
        {
            return promosTasks.InsertVodafone11Promos(dbInfo, model);
        }
        //Insert Vodafone 1+1 Details
        public long InsertVodafone11DetailsPromos(DBInfoModel dbinfo, Vodafone11ProdCategoriesModel model)
        {
            return promosTasks.InsertVodafone11Details(dbinfo, model);
        }
        //public longInsertDetailToHeader(Vodafone11ProdCategoriesModel model, long id)


        //Delete Vodafone 1+1 Header
        public long DeleteVodafone11Promos(DBInfoModel dbInfo, long id)
        {
            return promosTasks.DeleteVodafone11(dbInfo, id);
        }
        //Delete Vodafone 1+1 Details 
        public long DeleteVodafone11DetailPromos(DBInfoModel dbinfo, long id)
        {
            return promosTasks.DeleteVodafone11Detail(dbinfo, id);
        }

        public Vodafone11Model UpdateVodafone11Promos(DBInfoModel dbinfo, Vodafone11Model model)
        {
            return promosTasks.UpdateVodafone11Promos(dbinfo, model);
        }
        public Vodafone11ProdCategoriesModel UpdateVodafone11DetailsPromos(DBInfoModel dbinfo, Vodafone11ProdCategoriesModel model)
        {
            return promosTasks.UpdateVodafone11DetailsPromos(dbinfo, model);
        }
    }
}
