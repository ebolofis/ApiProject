using Symposium.Models.Interfaces;
using Symposium.Models.Models.Products;
using Symposium.Models.Models.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{

    public class SchedulerHelper
    {
        /// <summary>
        /// Return's list of UpsertResultsModel filled the Field MasterId with correct value.
        /// Master Id is not filled after db update because not exist on sql table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="results"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<UpsertResultsModel> ReturnMasterIds<T>(List<UpsertResultsModel> results, List<T> model) where T : ISchedulerModel
        {
            foreach (UpsertResultsModel item in results)
            {

                var fld = model.Find(f => f.TableId == item.DAID);
                if (fld != null)
                    item.MasterID = fld.MasterId;
            }
            return results;
        }

    }

    public class DA_ScheduledTaskesModel
    {
        /// <summary>
        /// Table Key
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// Το full url για τον κάθε πίνακα, Ο controller που θα κληθεί
        /// </summary>
        public string TableDescr { get; set; }

        /// <summary>
        /// Id for associated table(ex. Pages, PageSet, PriceList....)
        /// </summary>
        public long TableId { get; set; }

        /// <summary>
        /// Id For store to download changes
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        /// 0:ins,1:del,2:upd
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Σειρά με την οποία θα κατέβουν οι πίνακες στο κατάστημα
        /// </summary>
        public int Short { get; set; }
    }

    /// <summary>
    /// Return's List Of Records Per Table for Store Update
    /// </summary>
    public class RecordsForUpdateStoreModel 
    {
        /// <summary>
        /// Store Id from SA_SchedulerTaskes
        /// </summary>
        public long StoreId { get; set; }
        
        /// <summary>
        /// List Of Accounts (Short 1)
        /// </summary>
        public List<AccountSched_Model> Accounts { get; set; }

        /// <summary>
        /// List Of Categories (Short 5)
        /// </summary>
        public List<CategoriesSched_Model> Categories { get; set; }

        /// <summary>
        /// List Of Ingedients Product Association (Short 10)
        /// </summary>
        public List<IngedProdCategAssocSched_Model> IngedProdAssoc { get; set; }

        /// <summary>
        /// List Of Ingredient Categories (Short 15)
        /// </summary>
        public List<IngredCategoriesSched_Model> IngredCategories { get; set; }

        /// <summary>
        /// List Of PageSet (Short 20)
        /// </summary>
        public List<PageSetSched_Model> PageSet { get; set; }

        /// <summary>
        /// List Of Price List Master (Short 25)
        /// </summary>
        public List<PriceListMasterSched_Model> PriceListMaster { get; set; }

        /// <summary>
        /// List Of Sales Type (Short 30)
        /// </summary>
        public List<SalesTypeSched_Model> SalesType { get; set; }

        /// <summary>
        /// List Of Taxes (Short 35)
        /// </summary>
        public List<TaxSched_Model> Taxes { get; set; }

        /// <summary>
        /// List Of Units (Short 40)
        /// </summary>
        public List<UnitsSched_Model> Units { get; set; }

        /// <summary>
        /// List Of Vats (Short 45)
        /// </summary>
        public List<VatSched_Model> Vats { get; set; }

        /// <summary>
        /// List Of Ingredients (Short 50)
        /// </summary>
        public List<IngredientsSched_Model> Ingredients { get; set; }

        /// <summary>
        /// List Of Pages (Short 55)
        /// </summary>
        public List<PagesSched_Model> Pages { get; set; }

        /// <summary>
        /// List Of PriceLists (Short 60) 
        /// </summary>
        public List<PriceListSched_Model> PriceList { get; set; }

        /// <summary>
        /// List Of PriceList Effective Hours (Short 65)
        /// </summary>
        public List<PriceList_EffHoursSched_Model> PriceListEffectHours { get; set; }

        /// <summary>
        /// List Of Product Categories (Short 70)
        /// </summary>
        public List<ProductCategoriesSched_Model> ProductCategories { get; set; }

        /// <summary>
        /// List Of Products (Short 75)
        /// </summary>
        public List<ProductSched_Model> Products { get; set; }

        /// <summary>
        /// List Of ProductRecipies (Short 77)
        /// </summary>
        public List<ProductBarcodesSched_Model> ProductBarcodes { get; set; }

        /// <summary>
        /// List Of ProductRecipies (Short 78)
        /// </summary>
        public List<ProductRecipeSched_Model> ProductRecipies { get; set; }

        /// <summary>
        /// List Of Product Extras (Short 80)
        /// </summary>
        public List<ProductExtrasSched_Model> ProductExtras { get; set; }

        /// <summary>
        /// List Of PriceList Details (Short 85)
        /// </summary>
        public List<PriceListDetailSched_Model> PriceListDetails { get; set; }

        /// <summary>
        /// List Of Page Buttons (Short 90)
        /// </summary>
        public List<PageButtonSched_Model> PageButtons { get; set; }

        /// <summary>
        /// List Of Page Button Details (Short 95)
        /// </summary>
        public List<PageButtonDetSched_Model> PageButtonDetails { get; set; }

        /// <summary>
        /// List of Promotion Headers (Short 100) 
        /// Must added first because it is header on a header detail relationship
        /// </summary>
        public List<PromotionsHeaderSched_Model> PromotionsHeaders { get; set; }

        /// <summary>
        /// List of Promotion Combos (Short 105)
        /// Detail of Promotion Header Table
        /// </summary>
        public List<PromotionsCombosSched_Model> PromotionCombos { get; set; }

        /// <summary>
        /// List of Promotion Discounts (Short 110)
        /// Detail of Promotion Header Table
        /// </summary>
        public List<PromotionsDiscountsSched_Model> PromotionDiscount { get; set; }
    }


    public class ChangeProdCatModel
    {
        public List<long> prodcatlist { get; set; }
        public long Vat { get; set; }

    }

    /// <summary>
    /// Scheduler Account Model
    /// </summary>
    public class AccountSched_Model : AccountModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }


    /// <summary>
    /// Scheduler Categories Model
    /// </summary>
    public class CategoriesSched_Model : CategoriesModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }


    /// <summary>
    /// Scheduler Ingredient_ProdCategoryAssoc Model
    /// </summary>
    public class IngedProdCategAssocSched_Model : Ingredient_ProdCategoryAssocModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler IngredientCategories Model
    /// </summary>
    public class IngredCategoriesSched_Model : IngredientCategoriesModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PageSet Model
    /// </summary>
    public class PageSetSched_Model : PageSetModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PricelistMaster Model
    /// </summary>
    public class PriceListMasterSched_Model : PricelistMasterModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler SalesType Model
    /// </summary>
    public class SalesTypeSched_Model : SalesTypeModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler SalesType Model
    /// </summary>
    public class TaxSched_Model : TaxModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler Units Model
    /// </summary>
    public class UnitsSched_Model : UnitsModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler Vat Model
    /// </summary>
    public class VatSched_Model : VatModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler Ingredients Model
    /// </summary>
    public class IngredientsSched_Model : IngredientsModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler Pages Model
    /// </summary>
    public class PagesSched_Model : PagesModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PriceList Model
    /// </summary>
    public class PriceListSched_Model : PricelistModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PriceList_EffectiveHours Model
    /// </summary>
    public class PriceList_EffHoursSched_Model : PriceList_EffectiveHoursModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler ProductCategories Model
    /// </summary>
    public class ProductCategoriesSched_Model : ProductCategoriesModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler Product Model
    /// </summary>
    public class ProductSched_Model : ProductModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Schediler ProductBarcode Model
    /// </summary>
    public class ProductBarcodesSched_Model : ProductBarcodesModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler ProductRecipies Model
    /// </summary>
    public class ProductRecipeSched_Model : ProductRecipeModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler ProductExtras Model
    /// </summary>
    public class ProductExtrasSched_Model : ProductExtrasModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PriceListDetail Model
    /// </summary>
    public class PriceListDetailSched_Model : PriceListDetailModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }


    /// <summary>
    /// Scheduler PageButton Model
    /// </summary>
    public class PageButtonSched_Model : PageButtonsModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PageButtonDetail Model
    /// </summary>
    public class PageButtonDetSched_Model : PageButtonDetailModel, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PromotionHeader Model
    /// </summary>
    public class PromotionsHeaderSched_Model : PromotionsModels, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PromotionCombos Model
    /// </summary>
    public class PromotionsCombosSched_Model : PromotionsCombos, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }

    /// <summary>
    /// Scheduler PromotionDiscounts Model
    /// </summary>
    public class PromotionsDiscountsSched_Model : PromotionsDiscounts, ISchedulerModel
    {
        public long MasterId { get; set; }

        public string StoreFullURL { get; set; }

        public Nullable<long> TableId { get; set; }

        public int Action { get; set; }
    }
}
