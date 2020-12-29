using Pos_WebApi.Helpers;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Repositories.PMSRepositories;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Enums.Promotions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Pos_WebApi.Repositories.BORepos
{
    public class LookUpFactoryRepository //: ILookUpFactoryRepository
    {
        protected PosEntities DBContext;

        public LookUpFactoryRepository(PosEntities db)
        {
            DBContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public Object GetLookUpsForEntity(EntitiesForLookUpFactoryEnum entityFactory)
        {
            switch (entityFactory)
            {

                case EntitiesForLookUpFactoryEnum.SetupPosInfo: return GetSetupPosInfoLookUps();
                case EntitiesForLookUpFactoryEnum.SetupPosInfoDetail: return GetSetupPosInfoDetailLookUps();


                case EntitiesForLookUpFactoryEnum.SetupProductCategories: return GetSetupProductCategoriesLookUps();
                case EntitiesForLookUpFactoryEnum.SetupProductCat: return GetSetupProductCatLookUps();
                case EntitiesForLookUpFactoryEnum.SetupProductandProductCategories: return GetSetupProductandProductCategoriesLookUps();


                case EntitiesForLookUpFactoryEnum.PricelistDetail: return GetPicelistDetailLookUps();
                case EntitiesForLookUpFactoryEnum.SetupInvoiceTypes: return GetSetupInvoiceTypesLookUps();
                case EntitiesForLookUpFactoryEnum.SetupPricelist: return GetSetupPricelistsLookUps();
                case EntitiesForLookUpFactoryEnum.SetupPricelistMaster: return GetSetupPricelistMasterLookUps();

                case EntitiesForLookUpFactoryEnum.SetupStores: return GetSetupStoresLookUps();

                case EntitiesForLookUpFactoryEnum.SetupPosInfoDetailPricelistAssoc: return GetSetupPosInfoDetailPricelistAssocLookUps();

                case EntitiesForLookUpFactoryEnum.SetupPredefinedCredits: return GetSetupVouchersLookUps();
                case EntitiesForLookUpFactoryEnum.SetupClientPos: return GetSetupClientPosLookUps();

                case EntitiesForLookUpFactoryEnum.SetupPdaModules: return GetSetupPdaModulesLookUps();
                case EntitiesForLookUpFactoryEnum.SetupPromoDiscountType: return GetSetupDiscountType();

                case EntitiesForLookUpFactoryEnum.SetupKitchenInstruction: return GetSetupKitchenInstructionsLookUps();
                case EntitiesForLookUpFactoryEnum.SetupAccounts: return GetSetupAccountsLookUps();
                case EntitiesForLookUpFactoryEnum.SetupExcludedAccountSettings: return GetSetupExcludedAccountSettingsLookUps();
                case EntitiesForLookUpFactoryEnum.SetupAccountMapping: return GetSetupAccountMappingLookUps();
                case EntitiesForLookUpFactoryEnum.SetupTransactionTypes: return GetSetupTransactionTypesLookUps();
                case EntitiesForLookUpFactoryEnum.SetupDiscount: return GetSetupDiscountLookUps();
                case EntitiesForLookUpFactoryEnum.SetupAuthorizedGroupDetail: return GetSetupAuthorizedGroupDetail();
                case EntitiesForLookUpFactoryEnum.SetupItems: return GetSetupItemsLookUps();
                case EntitiesForLookUpFactoryEnum.SetupIngredients: return GetSetupIngredientsLookUps();
                //lookups for RegionLockerProduct
                case EntitiesForLookUpFactoryEnum.RegionLockerProduct: return GetRegionLockerProductLookUps();
                case EntitiesForLookUpFactoryEnum.ManageExternalProductsManager: return GetManageExternalProductsManagerLookUps();
                case EntitiesForLookUpFactoryEnum.Payrole: return GetPayroleLookUps();
                case EntitiesForLookUpFactoryEnum.ProductsManage: return GetProductsManageLookUps();
                //lookups for Receipts Hendling over BO
                case EntitiesForLookUpFactoryEnum.ManageReceipts: return GetManageReceiptsLookUps();
                case EntitiesForLookUpFactoryEnum.ManagePages: return GetManagePagesLookUps();


                case EntitiesForLookUpFactoryEnum.BusinessIntelligence: return GetBusinessIntelligence();
                case EntitiesForLookUpFactoryEnum.NFCConfiguration: return GetNFCConfiguration();
                case EntitiesForLookUpFactoryEnum.SetupCombo: return GetSetupComboLookUps();
                case EntitiesForLookUpFactoryEnum.SetupComboDetail: return GetSetupComboDetailLookUps();


                case EntitiesForLookUpFactoryEnum.SetupMainMessages: return GetSetupMainMessagesLookUps();


                default: return null;
            }

        }



        private Object GetSetupDiscountType()
        {
            List<KeyValuePair<int, string>> list = GeneralUtils.GetEnumList<PromoDiscountTypeEnum>();
            return new { DiscountType = list };
        }
        private Object GetBusinessIntelligence()
        {
            bool uniqueCodePolicy = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "BIPolicy.Product.UniqueCode");
            bool uniqueIngrPolicy = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "BIPolicy.Ingredients.UniqueCode");

            return new
            {
                BIPolicy = new
                {
                    Product = new { UniqueCode = uniqueCodePolicy },
                    Ingredients = new { UniqueCode = uniqueIngrPolicy }
                }
            };

        }
        private Object GetSetupMainMessagesLookUps()
        {
            var mainmessage = DBContext.DA_MainMessages.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            var message = DBContext.DA_Messages.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            var detail = DBContext.DA_MessagesDetails.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            return new { mainmsg = mainmessage, det = detail, msg = message };
        }

        private Object GetSetupProductCategoriesLookUps()
        {
            var category = DBContext.Categories.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            return new { CategoryId = category };
        }
        private Object GetSetupStoresLookUps()
        {
            var store = DBContext.DA_Stores.Select(s => new { Key = s.Id, Value = s.Title }).ToList();
            return new { StoreId = store };
        }

        private Object GetSetupProductCatLookUps()
        {
            var category = DBContext.ProductCategories.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            return new { ProdCategoryId = category };
        }

        private Object GetSetupProductandProductCategoriesLookUps()
        {
            var category = DBContext.ProductCategories.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            var product = DBContext.Product.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            return new { ItemId = product, ItemId1 = category };
        }

        private Object GetPicelistDetailLookUps()
        {
            var vats = DBContext.Vat.Select(s => new { Id = s.Id, Description = s.Description }).ToList();
            var taxes = DBContext.Tax.Select(s => new { Id = s.Id, Description = s.Description }).ToList();

            return new { Vat = vats, Tax = taxes };
        }

        private Object GetSetupTransactionTypesLookUps()
        {
            List<KeyValuePair<int, string>> list = GeneralUtils.GetEnumList<TransactionTypesEnum>();
            return new { Code = list };
        }
        private Object GetSetupDiscountLookUps()
        {
            List<KeyValuePair<int, string>> list = GeneralUtils.GetEnumList<DiscountTypeEnum>();
            return new { Type = list };
        }

        private Object GetSetupInvoiceTypesLookUps()
        {
            List<KeyValuePair<int, string>> list = GeneralUtils.GetEnumList<Pos_WebApi.Helpers.InvoiceTypesEnum>();
            //return list;
            return new { Type = list };
        }
        private Object GetSetupVouchersLookUps()
        {
            var invTypes = DBContext.InvoiceTypes.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            return new { InvoiceTypeId = invTypes };
        }
        private object GetSetupKitchenInstructionsLookUps()
        {
            var kitchenMap = DBContext.Kitchen.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            return new { KitchenId = kitchenMap };
        }

        private object GetSetupAccountsLookUps()
        {
            List<KeyValuePair<int, string>> list = GeneralUtils.GetEnumList<Pos_WebApi.Helpers.AccountType>();
            var hotel = DBContext.HotelInfo.FirstOrDefault();
            ProtelRepository protelRepo = new ProtelRepository(hotel.ServerName, hotel.DBUserName, hotel.DBPassword, hotel.DBName, hotel.allHotels, hotel.HotelType);
            var paymentTypes = protelRepo.GetPMSPaymentTypes().Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            return new { Type = list, PMSPaymentId = paymentTypes };
        }
        private object GetSetupAccountMappingLookUps()
        {
            var listPosInfo = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listacc = DBContext.Accounts.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { PosInfoId = listPosInfo, AccountId = listacc };
        }

        private object GetSetupAuthorizedGroupDetail()
        {
            var listAuthorizedGroup = DBContext.AuthorizedGroup.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listAction = DBContext.Actions.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { AuthorizedGroupId = listAuthorizedGroup, ActionId = listAction };
        }

        private Object GetSetupExcludedAccountSettingsLookUps()
        {
            var listPosInfoDetailId = DBContext.PosInfoDetail.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listAccountId = DBContext.Accounts.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { PosInfoDetailId = listPosInfoDetailId, AccountId = listAccountId };
        }
        private Object GetSetupPricelistsLookUps()
        {
            var listPricelist = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listPricelistMaster = DBContext.PricelistMaster.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listSalesType = DBContext.SalesType.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listLookUpPriceListId = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList(); //table as Pricelists enum as Pricelist
            List<KeyValuePair<int, string>> listType = GeneralUtils.GetEnumList<PricelistTypesEnum>();
            return new { SalesTypeId = listSalesType, LookUpPriceListId = listLookUpPriceListId, PricelistMasterId = listPricelistMaster, Type = listType };
        }
        private Object GetSetupPosInfoDetailPricelistAssocLookUps()
        {
            //            void Main()
            //{
            //                var query = from q in PosInfoDetail
            //                            join qq in PosInfo on q.PosInfoId equals qq.Id
            //                            select new
            //                            {
            //                                PosInfoDetailId = q.Id,
            //                                PosInfoDetaiDesciption = q.Description,
            //                                PosInfoDesciption = qq.Description,
            //                                GroupId = q.GroupId,
            //                                PosInfoId = qq.Id
            //                            };
            //                query.OrderBy(o => o.PosInfoDetailId).GroupBy(za => new { za.PosInfoId, za.GroupId }).Select(wa => new {
            //                    PosInfoId = wa.Key.PosInfoId,
            //                    PosInfoDetailId = wa.FirstOrDefault().PosInfoDetailId,
            //                    PosInfoDetaiDesciption = wa.FirstOrDefault().PosInfoDetaiDesciption,
            //                    PosInfoDesciption = wa.FirstOrDefault().PosInfoDesciption
            //                }).Dump().Distinct();
            //            }
            var listPosInfoDetailId = DBContext.PosInfoDetail.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listPricelistId = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList(); //table as Pricelists enum as Pricelist
            return new { PosInfoDetailId = listPosInfoDetailId, PricelistId = listPricelistId };
        }
        private Object GetSetupPdaModulesLookUps()
        {
            var listPosInfoId = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listPageSetId = DBContext.PageSet.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { PosInfoId = listPosInfoId, PageSetId = listPageSetId };
        }
        private Object GetSetupItemsLookUps()
        {
            var listUnitId = DBContext.Units.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listVatId = DBContext.Vat.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { UnitId = listUnitId, VatId = listVatId };
        }
        private Object GetSetupClientPosLookUps()
        {
            var listPosInfoId = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            //var listThemeId = DBContext.Vat.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { PosInfoId = listPosInfoId };//, VatId = listVatId };
        }
        private Object GetSetupPricelistMasterLookUps()
        {
            var listPosInfoId = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listPricelistId = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { PricelistId = listPricelistId };//, VatId = listVatId };
        }

        private Object GetSetupPosInfoLookUps()
        {
            List<KeyValuePair<int, string>> listType = GeneralUtils.GetEnumList<ModuleType>();
            var listDepartmentId = DBContext.Department.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            List<KeyValuePair<int, string>> listFiscalType = GeneralUtils.GetEnumList<FiscalType>();
            var listAccounts = DBContext.Accounts.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            //var slaveDatalist = DBContext.PosInfoDetail.Select(q => new {Id = q.Id, PosInfoId = q.PosInfoId, PricelistId = q.PricelistId }).ToList();
            List<KeyValuePair<int, string>> listLoginToOrderMode = GeneralUtils.GetEnumList<LoginToOrderModeEnum>();
            List<KeyValuePair<int, string>> listKeyboardType = GeneralUtils.GetEnumList<KeyboardTypeEnum>();
            List<KeyValuePair<int, string>> listPidTemplateTransactionTypeEnum = GeneralUtils.GetEnumList<PosInfoDetailTemplateTransactionTypeEnum>();

            var listPosInfoId = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listPricelistId = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listInvoiceId = DBContext.InvoiceTypes.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listDefaultHotelId = DBContext.HotelInfo.Select(q => new { Key = q.HotelId, Value = q.HotelName }).ToList();
            var listConfiguration = ReadJsonFileNamesFromSubPath();

            //var formatListConfiguration = new List<KeyValuePair<int, string>>();
            //for (int i = listConfiguration.Count - 1; i >= 0 ; i--)
            //{
            //    formatListConfiguration.Add(new KeyValuePair<int, string>(i, listConfiguration[i]));
            //}


            return new
            {
                Type = listType,
                DepartmentId = listDepartmentId,
                FiscalType = listFiscalType,
                AccountId = listAccounts,//slaveData = slaveDatalist,
                PosInfoId = listPosInfoId,
                PricelistId = listPricelistId,
                InvoiceId = listInvoiceId,
                LoginToOrderMode = listLoginToOrderMode,
                KeyboardType = listKeyboardType,
                PidTemplateTransactionTypeEnum = listPidTemplateTransactionTypeEnum,
                DefaultHotelId = listDefaultHotelId,
                Configuration = listConfiguration
            };
            //var listPosInfoId = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            // return new { PricelistId = listPricelistId };//, VatId = listVatId };
        }

        private List<string> ReadJsonFileNamesFromSubPath()
        {
            List<string> fileNames = new List<string>();
            string basePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "MainConfiguration\\Pos";
            List<string> configurationFiles = ReadJsonFileNamesFromPath(basePath);
            foreach (string file in configurationFiles)
            {
                string[] pathNameComponents = file.Split('\\');
                string fullFileName = pathNameComponents[pathNameComponents.Length - 1];
                string[] fileNameComponents = fullFileName.Split('.');
                string fileName = fileNameComponents[0];
                fileNames.Add(fileName);
            }
            return fileNames;
        }

        /// <summary>
        /// Find available json files in given folder path and return a list containing each json file's full pathname
        /// </summary>
        /// <param name="path">Folder path to find json files</param>
        /// <returns>List of json files' full pathnames</returns>
        private List<string> ReadJsonFileNamesFromPath(string folderpath)
        {
            if (!Directory.Exists(folderpath))
            {
                DirectoryInfo di = Directory.CreateDirectory(folderpath);
            }
            List<string> fullFilenames = new List<string>();
            foreach (string fullFilename in Directory.EnumerateFiles(folderpath, "*.json", SearchOption.AllDirectories))
            {
                fullFilenames.Add(fullFilename);
            }
            return fullFilenames;
        }

        private Object GetSetupPosInfoDetailLookUps()
        {
            //List<KeyValuePair<int, string>> listType = GeneralUtils.GetEnumList<ModuleType>();
            //var listDepartmentId = DBContext.Department.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            //var listAccounts = DBContext.Accounts.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            //var slaveDatalist = DBContext.PosInfoDetail.Select(q => new {Id = q.Id, PosInfoId = q.PosInfoId, PricelistId = q.PricelistId }).ToList();
            //var listPricelistId = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            List<KeyValuePair<int, string>> listFiscalType = GeneralUtils.GetEnumList<FiscalType>();
            var listPosInfoId = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listInvoiceId = DBContext.InvoiceTypes.Select(q => new { Key = q.Id, Value = q.Description, Abbr = q.Abbreviation, Type = q.Type, Code = q.Code }).ToList();

            var hoteltypeType = DBContext.HotelInfo.Count() > 0 ? DBContext.HotelInfo.FirstOrDefault().Type : (byte)255;
            if (hoteltypeType != 3)
            {
                var hotel = DBContext.HotelInfo.FirstOrDefault();
                ProtelRepository protelRepo = new ProtelRepository(hotel.ServerName, hotel.DBUserName, hotel.DBPassword, hotel.DBName, hotel.allHotels, hotel.HotelType);
                var invoiceTypes = protelRepo.GetPMSInvoiceTypes(hotel.MPEHotel ?? 0).Select(s => new { Key = s.Id, Value = s.Description }).ToList();
                return new { FiscalType = listFiscalType, PosInfoId = listPosInfoId, InvoicesTypeId = listInvoiceId, PMSInvoiceId = invoiceTypes };
            }
            else
            {
                return new { FiscalType = listFiscalType, PosInfoId = listPosInfoId, InvoicesTypeId = listInvoiceId };
            }
        }
        private Object GetSetupIngredientsLookUps()
        {
            var listItems = DBContext.Items.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listUnits = DBContext.Units.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listIngredientCategories = DBContext.IngredientCategories.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { ItemId = listItems, UnitId = listUnits, IngredientCategoryId = listIngredientCategories };
        }
        private Object GetSetupComboLookUps()
        {
            var listProducts = DBContext.Product.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listDepartments = DBContext.Department.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listProCategories = DBContext.ProductCategories.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listHotelId = DBContext.HotelInfo.Select(q => new { Key = q.HotelId, Value = q.HotelName }).ToList();
            var listmpehotelId = DBContext.HotelInfo.Select(q => new { Key = q.MPEHotel, Value = q.HotelName, Id=q.Id }).ToList();
            var listPricelistId = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listpageId = DBContext.Pages.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { ProductComboId = listProducts, DepartmentId = listDepartments, ProdCatId=listProCategories, hotelId= listHotelId, mpehoteld= listmpehotelId,pricelistId= listPricelistId, pagelist= listpageId };
        }
        private Object GetSetupComboDetailLookUps()
        {
            var listCombos = DBContext.Combo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listProducts = DBContext.Product.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            return new { ComboId = listCombos, ProductComboItemId = listProducts };
        }
        /// <summary>
        /// Function to load entities lookups over RegionLockerProduct manager
        /// loads PosInfo and Pricelists Enum
        /// </summary>
        /// <returns></returns>
        private Object GetRegionLockerProductLookUps()
        {
            var listPosInfo = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listPricelistId = DBContext.Pricelist.Select(q => new { Key = q.Id, Value = q.Description }).ToList(); //table as Pricelists enum as Pricelist
            var category = DBContext.Categories.Select(s => new { Key = s.Id, Value = s.Description }).ToList();
            var pcats = DBContext.ProductCategories.Select(s => new { Key = s.Id, Value = s }).ToList();

            return new { PosInfoId = listPosInfo, PricelistId = listPricelistId, CategoryId = category, ProductCategoryId = pcats };
        }
        private Object GetManageExternalProductsManagerLookUps()
        {
            List<KeyValuePair<int, string>> prodenumType = GeneralUtils.GetEnumList<ExternalProductMappingEnum>();
            return new { ProductEnumType = prodenumType };
        }
        private Object GetPayroleLookUps()
        {
            List<KeyValuePair<int, string>> payroleenumType = GeneralUtils.GetEnumList<PayroleEnum>();
            var listPosInfo = DBContext.PosInfo.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listStaff = DBContext.Staff.Where(w => w.IsDeleted == null).Select(q => new { Key = q.Id, Value = q.LastName }).ToList();
            var listDepartmentId = DBContext.Department.Select(q => new { Key = q.Id, Value = q.Description }).ToList();
            var listStaffPositionId = DBContext.StaffPosition.Select(s => new { Key = s.Id, Value = s.Description }).ToList();

            return new { Type = payroleenumType, PosInfoId = listPosInfo, StaffId = listStaff, ShopId = listDepartmentId, StaffPositionId = listStaffPositionId };
        }

        private Object GetProductsManageLookUps()
        {
            //var svc = BORepos.GenericRepository;
            var Kdslist = DBContext.Kds.Select(q => new { q.Id, q.Description, q.IsDeleted, q.Status, q.PosInfoId }).ToList();
            var Kitchenlist = DBContext.Kitchen.Select(q => new { q.Id, q.Description, q.Status, q.Code, q.IsDeleted }).ToList();
            var KitchenRegionlist = DBContext.KitchenRegion.Select(q => new { q.Id, q.ItemRegion, q.RegionPosition, q.Abbr }).ToList();
            var ProductCategorieslist = DBContext.ProductCategories.Select(q => new { q.Id, q.Description, q.Type, q.Status, q.Code, q.CategoryId }).ToList();
            var Unitslist = DBContext.Units.Select(q => new { q.Id, q.Description, q.Abbreviation, q.Unit }).ToList();
            var Ingredient_ProdCategoryAssoclist = DBContext.Ingredient_ProdCategoryAssoc.Select(q => new { q.Id, q.IngredientId, q.ProductCategoryId, q.Sort }).ToList();
            var Pricelistlist = DBContext.Pricelist.Select(q => new { q.Id, q.Description, q.Code, q.LookUpPriceListId, q.Percentage, q.Status, q.ActivationDate, q.DeactivationDate, q.SalesTypeId, q.PricelistMasterId, q.IsDeleted, q.Type }).ToList();
            var Vatlist = DBContext.Vat.Select(q => new { q.Id, q.Description, q.Percentage, q.Code }).ToList();
            var Taxlist = DBContext.Tax.Select(q => new { q.Id, q.Description, q.Percentage }).ToList();
            var IngredientsList = DBContext.Ingredients.Select(q => new
            {
                q.Id,
                q.Description,
                q.ExtendedDescription,
                q.SalesDescription,
                q.Qty,
                q.UnitId,
                q.ItemId,
                q.Code,
                q.IsDeleted,
                q.Background,
                q.Color
            }).ToList();
            return new
            {
                Kds = Kdslist,
                Kitchen = Kitchenlist,
                KitchenRegion = KitchenRegionlist,
                ProductCategories = ProductCategorieslist,
                Units = Unitslist,
                Ingredient_ProdCategoryAssoc = Ingredient_ProdCategoryAssoclist,
                Pricelist = Pricelistlist,
                Vat = Vatlist,
                Tax = Taxlist,
                Ingredients = IngredientsList
            };
        }

        private Object GetManageReceiptsLookUps()
        {
            List<KeyValuePair<int, string>> invTypeEnum = GeneralUtils.GetEnumList<Pos_WebApi.Helpers.InvoiceTypesEnum>();
            var listPosInfo = DBContext.PosInfo.Select(q => new { q.Id, q.Description }).ToList();
            var listPosInfoDetail = DBContext.PosInfoDetail.Select(q => new
            {
                q.Id,
                q.PosInfoId,
                q.Description,
                q.Abbreviation,
                q.InvoicesTypeId,
                q.InvoiceId,
                q.GroupId,
                q.CreateTransaction,
            }).ToList();
            //var listStaff = DBContext.Staff.Select(q => new { q.Id, q.FirstName, q.LastName }).ToList();
            //var listPricelist =
            var listInvoiceTypes = DBContext.InvoiceTypes.Select(q => new { q.Id, q.Description, q.Type }).ToList();

            return new
            {
                PosInfoId = listPosInfo,
                PosInfoDetailId = listPosInfoDetail,
                //StaffId = listStaff,
                //PricelistId = listPricelist,
                InvoiceTypeId = listInvoiceTypes,
                InvoiceTypesEnum = invTypeEnum,
            };
        }
        private Object GetManagePagesLookUps()
        {
            var listPosInfo = DBContext.PosInfo.Select(q => new { q.Id, q.Description }).ToList();
            var listPriceList = DBContext.Pricelist.Select(q => new { q.Id, q.Description }).ToList();
            var listSalesType = DBContext.SalesType.Select(q => new { q.Id, q.Description }).ToList();
            var listCategory = DBContext.Categories.Select(q => new { q.Id, q.Description }).ToList();
            var listProductCategory = DBContext.ProductCategories.Select(q => new { q.Id, q.Description }).ToList();

            return new
            {
                PosInfo = listPosInfo,
                PriceList = listPriceList,
                SalesType = listSalesType,
                Category = listCategory,
                ProductCategory = listProductCategory,
            };
        }
        /// <summary>
        /// Lookup for Nfc configuration
        /// </summary>
        /// <returns>An obj with lookups needed for nfc configuration device</returns>
        private Object GetNFCConfiguration()
        {
            List<KeyValuePair<int, string>> nfcEnum = GeneralUtils.GetEnumList<NFCCardsEnum>();
            return new
            {
                nfcType = nfcEnum
            };
        }

    }
}
