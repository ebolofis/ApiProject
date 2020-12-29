﻿using Symposium.Models.Models;
using Symposium.Models.Models.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Promos
{
   public interface IPromosTasks
    {

        /// <summary>
        /// Get Vodafone 1+1 promos
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <returns></returns>
        List<Vodafone11Model> GetVodafone11Promos(DBInfoModel dbInfo);

        Vodafone11Model GetOneOffVodafone11Promos(DBInfoModel dbInfo, long id);
        List<Vodafone11Model> GetVodafone11HeaderPromos(DBInfoModel dbInfo);
        List<Vodafone11ProdCategoriesModel> GetVodafone11DetailsPromos(DBInfoModel dbinfo, long HeaderId);
        List<Vodafone11ProdCategoriesModel> GetVodafoneAll11DetailsPromos(DBInfoModel dbinfo);
        long InsertVodafone11Promos(DBInfoModel dbInfo, Vodafone11Model model);

        long InsertVodafone11Details(DBInfoModel dbInfo, Vodafone11ProdCategoriesModel model);

        long DeleteVodafone11(DBInfoModel dbInfo, long id);
        long DeleteVodafone11Detail(DBInfoModel dbinfo, long id);

        Vodafone11Model UpdateVodafone11Promos(DBInfoModel dbinfo, Vodafone11Model model);
        Vodafone11ProdCategoriesModel UpdateVodafone11DetailsPromos(DBInfoModel dbinfo, Vodafone11ProdCategoriesModel model);
    }
}
