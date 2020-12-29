using AutoMapper;
using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class ProductDT : IProductDT
    {
        IGenericDAO<ProductDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        IUnitsDT unitDT;
        IProductCategoriesDT prodCategDT;
        IIngredientsDT ingredientsDT;
        string connectionString;

        public ProductDT(IGenericDAO<ProductDTO> dt, IUsersToDatabasesXML usersToDatabases,
            IUnitsDT unitDT, IProductCategoriesDT prodCategDT, IIngredientsDT ingredientsDT)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
            this.unitDT = unitDT;
            this.prodCategDT = prodCategDT;
            this.ingredientsDT = ingredientsDT;
        }

        /// <summary>
        /// Get Products(Id, Description) For Dropdown List with DAid
        /// </summary>
        /// <returns></returns>
        public List<ProductsComboList> GetComboListDA(DBInfoModel dbInfo)
        {
            List<ProductsComboList> productsCombo = new List<ProductsComboList>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetShortages = @"SELECT p.DAId AS Id, p.[Description] AS Descr, p.Code AS Code FROM Product AS p ORDER BY p.[Description]";
                productsCombo = db.Query<ProductsComboList>(sqlGetShortages).ToList();
            }

            return productsCombo;
        }
        /// <summary>
        /// Get Products(Id, Description) For Dropdown List 
        /// </summary>
        /// <returns></returns>
        public List<ProductsComboList> GetComboList(DBInfoModel dbInfo)
        {
            List<ProductsComboList> productsCombo = new List<ProductsComboList>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetShortages = @"SELECT Id AS Id, p.[Description] AS Descr FROM Product AS p ORDER BY p.[Description] ";
                productsCombo = db.Query<ProductsComboList>(sqlGetShortages).ToList();
            }

            return productsCombo;
        }

        /// <summary>
        /// Return the extended list of products (active only). Every product contains the list of (active) Extras.
        /// </summary>
        /// <returns></returns>
        public List<ProductExtModel> GetExtentedList(DBInfoModel Store)
        {

            string sql = @"
                  select distinct
	                Product.*,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code
	            from Product 
	            left outer join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

	       union all

	            select distinct
	                Product.*,
	                Ingredients.id id,productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(MinQty,0)MinQty,isnull(MaxQty,0)MaxQty,ProductRecipe.UnitId UnitId,Ingredients.Code
	            from Product 
	            left outer join ProductRecipe on Product.Id=ProductRecipe.productid
	            left outer join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
				left outer join (
					select distinct
						Product.*,
						Ingredients.id ingid
					from Product 
					left outer join ProductExtras on Product.Id=ProductExtras.productid
					left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
					left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
					where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
				) tmp on tmp.Id=product.Id and isnull(tmp.ingid,0) = isnull(Ingredients.id,0)
	            where (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) and tmp.id is null

			union all

				select distinct Product.*,
	                Ingredients.id id,Product.id productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,Ingredients.UnitId UnitId,Ingredients.Code 
	            from Product 
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null


            ";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0); //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          });
                List<ProductExtModel> list = lookup.Select(x => x.Value).ToList<ProductExtModel>();
                foreach (var prodex in list)
                {
                    prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                }
                return list;
            }
        }



        public List<ProductExtModel> GetDepartmentExtentedList(DBInfoModel Store, long PosDepartmentId)
        {
            string sql = @"             
               select distinct
	                Product.*,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code
	            from Product 
				join TransferMappings tm on PosDepartmentId=" + PosDepartmentId + @" and  Product.Id=tm.ProductId
	            left outer join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
	       union all
	                          select distinct
	                Product.*,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code 
	            from Product 
				 left outer join ProductExtras on Product.Id=ProductExtras.productid
	            left outer join ProductRecipe on Product.Id=ProductRecipe.productid
	            left outer join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
				left outer join (
					select distinct
						Product.*,
						Ingredients.id ingid
					from Product 
					join TransferMappings tm1 on PosDepartmentId=" + PosDepartmentId+ @"  and  Product.Id=tm1.ProductId
					left outer join ProductExtras on Product.Id=ProductExtras.productid
					left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
					left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
					where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
				) tmp on tmp.Id=product.Id and isnull(tmp.ingid,0) = isnull(Ingredients.id,0)
	            where (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) and tmp.id is null
			union all
				select distinct Product.*,
	                Ingredients.id id,Product.id productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,Ingredients.UnitId UnitId,Ingredients.Code 
	            from Product 
				join TransferMappings tm on PosDepartmentId=" + PosDepartmentId + @" and  Product.Id=tm.ProductId
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

            ";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0); //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          });
                List<ProductExtModel> list = lookup.Select(x => x.Value).ToList<ProductExtModel>();
                foreach (var prodex in list)
                {
                    prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                }
                return list;
            }
        }




        public List<ProductExtModel> GetDepartmentExtentedList(DBInfoModel Store, long PosDepartmentId, long pricelistId)
        {
            string sql = @"
                                           
               select distinct
	                Product.*,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code
	            from Product 
				join TransferMappings tm on PosDepartmentId=" + PosDepartmentId + @" and  Product.Id=tm.ProductId  and PriceListId= " + pricelistId + @"
	            left outer join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
	       union all
	                select distinct
	                Product.*,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code 
	            from Product 
				 left outer join ProductExtras on Product.Id=ProductExtras.productid
	            left outer join ProductRecipe on Product.Id=ProductRecipe.productid
	            left outer join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
				left outer join (
					select distinct
						Product.*,
						Ingredients.id ingid
					from Product 
					join TransferMappings tm1 on PosDepartmentId=" + PosDepartmentId + @" and  Product.Id=tm1.ProductId and PriceListId= " + pricelistId + @"
					left outer join ProductExtras on Product.Id=ProductExtras.productid
					left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
					left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
					where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
				) tmp on tmp.Id=product.Id and isnull(tmp.ingid,0) = isnull(Ingredients.id,0)
	            where (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) and tmp.id is null
			union all
				select distinct Product.*,
	                Ingredients.id id,Product.id productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,Ingredients.UnitId UnitId,Ingredients.Code 
	            from Product 
				join TransferMappings tm on PosDepartmentId=" + PosDepartmentId + @" and  Product.Id=tm.ProductId  and PriceListId= " + pricelistId + @"
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

            ";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0); //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          });
                List<ProductExtModel> list = lookup.Select(x => x.Value).ToList<ProductExtModel>();
                foreach (var prodex in list)
                {
                    prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                }
                return list;
            }
        }

        /// <summary>
        /// Return the extended list of products (active only). Every product contains the list of (active) Extras.  Prices ARE INCLUDED. 
        /// </summary>
        /// <param name="PricelistId">Pricelist Id</param>
        /// <returns></returns>
        public List<ProductExtModel> GetExtentedList(DBInfoModel Store, long PricelistId)
        {


            string sql = @"
              select distinct
	                Product.*, ISNULL(pd.Price,0) Price,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,
	                Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,
	                isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code, ISNULL(pdi.Price,0) Price--, 0 isRecipe
	            from Product 
	            left outer join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

	       union all

	            select distinct
	                Product.*, ISNULL(pd.Price,0) Price,
	                Ingredients.id id,ProductRecipe.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,
	                isnull(MinQty,0)MinQty,isnull(MaxQty,0)MaxQty,ProductRecipe.UnitId UnitId,Ingredients.Code, ISNULL(pdi.Price,0) Price--,1 isRecipe
	            from Product 
	            left outer join ProductRecipe on Product.Id=ProductRecipe.productid
	            left outer join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
				left outer join (
					select distinct
						Product.*,
						Ingredients.id ingid
					from Product 
					left outer join ProductExtras on Product.Id=ProductExtras.productid
					left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
					left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
					where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
				) tmp on tmp.Id=product.Id and isnull(tmp.ingid,0) = isnull(Ingredients.id,0)
				INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) and tmp.id is null

			union all

				select distinct Product.*, ISNULL(pd.Price,0) Price,
	                Ingredients.id id,Product.id productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,
	                isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,
	                Ingredients.UnitId UnitId,Ingredients.Code, ISNULL(pdi.Price,0) Price --, 0 isRecipe
	            from Product 
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
            ";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0); //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          });
                List<ProductExtModel> list = lookup.Select(x => x.Value).ToList<ProductExtModel>();
                foreach (var prodex in list)
                {
                    prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                }
                return list;
            }
        }

        /// <summary>
        /// Return List of Pruducts Categories (acrive only). 
        /// Every List of Pruducts Categories includes the extended list of products (active only). 
        /// Every product contains the list of (active) Ingredient Categories. 
        /// Every Ingredient Category includes the extended list of Ingredients (active only).
        /// </summary>
        /// <returns></returns>
        public List<ProductCategoriesSkroutzExtModel> GetSkroutzProducts(DBInfoModel Store, long PricelistId)
        {
            string sql = @"
              select DISTINCT
					ProductCategories.Code AS id,
					ProductCategories.[Description] AS name, 
					ProductCategories.[Description] AS [description],
					ProductCategories.CategoryId AS catalog_id,
					0 AS orderno,
	                Product.Code AS id,
	                Product.[Description] AS name,
	                Product.ExtraDescription AS [description],
	                Product.ProductCategoryId AS section_id,
	                Product.ImageUri AS [image],
	                ISNULL(pd.Price,0) * 100 AS price_in_cents,
	                ic.Code id, 
	                ic.[Description] AS name, 
	                0 AS base_price, 
	                ic.IsUnique AS multiple_selections,
	                Ingredients.Code AS id,
	                Ingredients.[Description] AS name, 
	                ISNULL(pdi.Price,0) * 100 AS price_in_cents,
                    0 AS selected
	            from ProductCategories 
	            LEFT OUTER JOIN Product ON Product.ProductCategoryId = ProductCategories.Id 
	            left outer join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            LEFT OUTER JOIN IngredientCategories AS ic ON Ingredients.IngredientCategoryId = ic.Id
	            INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is NULL

	       union all

	            select DISTINCT
					ProductCategories.Code AS id,
					ProductCategories.[Description] AS name, 
					ProductCategories.[Description] AS [description],
					ProductCategories.CategoryId AS catalog_id,
					0 AS orderno,
	                Product.Code AS id,
	                Product.[Description] AS name,
	                Product.ExtraDescription AS [description],
	                Product.ProductCategoryId AS section_id,
	                Product.ImageUri AS [image],
	                ISNULL(pd.Price,0) * 100 AS price_in_cents,
	                ic.Code id, 
	                ic.[Description] AS name, 
	                0 AS base_price, 
	                ic.IsUnique AS multiple_selections,              
	                Ingredients.Code AS id,
	                Ingredients.[Description] AS name, 
	                ISNULL(pdi.Price,0) * 100 AS price_in_cents,
                    0 AS selected
	            from ProductCategories 
	            LEFT OUTER JOIN Product ON Product.ProductCategoryId = ProductCategories.Id 
	            left outer join ProductRecipe on Product.Id=ProductRecipe.productid
	            left outer join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
				left outer join (
					select distinct
						Product.*,
						Ingredients.id ingid
					from Product 
					left outer join ProductExtras on Product.Id=ProductExtras.productid
					left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
					left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
					where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
				) tmp on tmp.Id=product.Id and isnull(tmp.ingid,0) = isnull(Ingredients.id,0)
				LEFT OUTER JOIN IngredientCategories AS ic ON Ingredients.IngredientCategoryId = ic.Id
				INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) and tmp.id is null

			union all

				select distinct 
					ProductCategories.Code AS id,
					ProductCategories.[Description] AS name, 
					ProductCategories.[Description] AS [description],
					ProductCategories.CategoryId AS catalog_id,
					0 AS orderno,
	                Product.Code AS id,
	                Product.[Description] AS name,
	                Product.ExtraDescription AS [description],
	                Product.ProductCategoryId AS section_id,
	                Product.ImageUri AS [image],
	                ISNULL(pd.Price,0) * 100 AS price_in_cents,
	                ic.Code id, 
	                ic.[Description] AS name, 
	                0 AS base_price, 
	                ic.IsUnique AS multiple_selections,
	                Ingredients.Code AS id,
	                Ingredients.[Description] AS name, 
	                ISNULL(pdi.Price,0) * 100 AS price_in_cents,
                    0 AS selected
	            from ProductCategories 
	            LEFT OUTER JOIN Product ON Product.ProductCategoryId = ProductCategories.Id 
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            LEFT OUTER JOIN IngredientCategories AS ic ON Ingredients.IngredientCategoryId = ic.Id
	            INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where  (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
            ";

            string sqlGetProductId = @"SELECT p.Id FROM Product AS p WHERE p.Code =@code";
            string sqlGetIngredientId = @"SELECT i.Id FROM Ingredients AS i WHERE i.Code =@code";

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<string, ProductCategoriesSkroutzExtModel>();//Create a data structure to store products Categories uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductCategoriesSkroutzExtModel, ProductSkroutzModel, IngredientCategoriesSkroutzModel, IngredientSkroutzModel, ProductCategoriesSkroutzExtModel>(
                          sql,
                          (productCateg, product, ingredientCateg, ingredient) =>
                          {
                              ProductCategoriesSkroutzExtModel prodCat;
                              if (!lookup.TryGetValue(productCateg.id, out prodCat))
                              {
                                  prodCat = productCateg;//the product Category does not exist into lookup dictionary
                                  lookup.Add(productCateg.id, productCateg);
                              }

                              if (product != null && product.id != null && ingredient != null && ingredient.id != null && prodCat.Plate.FindIndex(x => x.id == product.id) < 0)
                              {
                                  prodCat.Plate.Add(product);
                              }


                              //product.Option.Add(ingredientCateg);
                              foreach (ProductSkroutzModel p in prodCat.Plate)
                              {
                                  if (ingredientCateg != null && p.Option.FindIndex(x => x.id == ingredientCateg.id) < 0)
                                  {
                                      p.Option.Add(ingredientCateg);
                                  }
                              }



                              //ingredientCateg.Choice.Add(ingredient);
                              foreach (ProductSkroutzModel p in prodCat.Plate)
                              {
                                  foreach (IngredientCategoriesSkroutzModel e in p.Option)
                                  {
                                      if (ingredient != null && e.id == ingredientCateg.id && e.Choice.FindIndex(x => x.id == ingredient.id) < 0)
                                      {
                                          long productId = db.Query<long>(sqlGetProductId, new { code = product.id }).FirstOrDefault();
                                          Nullable<long> ingredientId = 0;
                                          if (ingredient != null)
                                          {
                                              ingredientId = db.Query<long>(sqlGetIngredientId, new { code = ingredient.id }).FirstOrDefault();
                                          }
                                          else
                                          {
                                              ingredientId = 0;
                                          }
                                          ingredient.selected = ingredientsDT.IsProductRecipe(Store, productId, (long)ingredientId); //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                          e.Choice.Add(ingredient);
                                      }
                                  }
                              }

                              return prodCat;
                          }, new { PriceListId = PricelistId });
                List<ProductCategoriesSkroutzExtModel> list = lookup.Select(x => x.Value).ToList<ProductCategoriesSkroutzExtModel>();
                foreach (var prodex in list)
                {
                    prodex.Plate = prodex.Plate.Where(x => x != null).ToList();
                }
                return list;
            }
        }

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. 
        /// If product is not active or it is not exists into DB then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ProductId">Product Id</param>
        /// <returns></returns>
        public ProductExtModel GetProductExt(DBInfoModel Store, long ProductId)
        {
            string sql = @"
                select distinct
	                Product.*,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code, 0 isRecipe
	            from Product 
	            inner join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            inner join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            where Product.Id=@ProductId and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

	       union all

	            select distinct
	                Product.*,
	                Ingredients.id id,productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(MinQty,0)MinQty,isnull(MaxQty,0)MaxQty,ProductRecipe.UnitId UnitId,Ingredients.Code,1 isRecipe
	            from Product 
	            inner join ProductRecipe on Product.Id=ProductRecipe.productid
	            inner join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
	            where Product.Id=@ProductId and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) 

	       union all

				select distinct Product.*,
	                Ingredients.id id,Product.id productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,Ingredients.UnitId UnitId,Ingredients.Code , 0 isRecipe
	            from Product 
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            where Product.Id=@ProductId and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
           ";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0); //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          }, new { ProductId = ProductId });
                ProductExtModel prodex = lookup.Select(x => x.Value).ToList<ProductExtModel>().FirstOrDefault();
                //     prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                return prodex;
            }

        }

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. 
        /// If product is not active or it is not exists into DB then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Code">Product Code</param>
        /// <returns></returns>
        public ProductExtModel GetProductExt(DBInfoModel Store, string Code)
        {


            string sql = @"
            select distinct
	                Product.*,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code--, 0 isRecipe
	            from Product 
	            left outer join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

	       union all

	            select distinct
	                Product.*,
	                Ingredients.id id,productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(MinQty,0)MinQty,isnull(MaxQty,0)MaxQty,ProductRecipe.UnitId UnitId,Ingredients.Code--,1 isRecipe
	            from Product 
	            left outer join ProductRecipe on Product.Id=ProductRecipe.productid
	            left outer join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
				left outer join (
					select distinct
						Product.*,
						Ingredients.id ingid
					from Product 
					left outer join ProductExtras on Product.Id=ProductExtras.productid
					left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
					left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id 
					where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
				) tmp on tmp.Id=product.Id and isnull(tmp.ingid,0) = isnull(Ingredients.id,0)
	            where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) and tmp.id is null

              union all

				select distinct Product.*,
	                Ingredients.id id,Product.id productid,isnull(Ingredients.Description,'-')Description,isnull(ExtendedDescription,'-')ExtendedDescription,isnull(Ingredients.SalesDescription,'-')SalesDescription,isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,Ingredients.UnitId UnitId,Ingredients.Code --, 0 isRecipe
	            from Product 
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
          ";

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0);  //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          }, new { Code = Code });
                ProductExtModel prodex = lookup.Select(x => x.Value).ToList<ProductExtModel>().FirstOrDefault();
                //  prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                return prodex;
            }

        }

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. Prices ARE INCLUDED. 
        /// If product is not active or it is not exists into DB or does not belong to price-list then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Code">Product Code</param>
        /// <param name="PricelistId">Pricelist Id</param>
        /// <returns></returns>
        public ProductExtModel GetProductExt(DBInfoModel Store, string Code, long PricelistId)
        {


            string sql = @"
              select distinct
                    Product.*, isnull(pld.Price,0) Price,
                    Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,
                        isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code, isnull(pldIng.Price,0) Price--, 0 isRecipe
                from Product
                left outer join ProductExtras on Product.Id=ProductExtras.productid
                left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
                left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id
                INNER JOIN PriceListDetail pld on pld.ProductId = Product.Id and pld.PricelistId = @PriceListId
                LEFT OUTER JOIN PriceListDetail pldIng on pldIng.IngredientId = Ingredients.id and pldIng.PricelistId = @PriceListId
                where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

           union all

                select distinct
                    Product.*,isnull(pld.Price,0) Price,
                    Ingredients.id id,ProductRecipe.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,isnull(MinQty,0)MinQty,
                    isnull(MaxQty,0)MaxQty,ProductRecipe.UnitId UnitId,Ingredients.Code, isnull(pldIng.Price,0) Price--,1 isRecipe
                from Product
                left outer join ProductRecipe on Product.Id=ProductRecipe.productid
                left outer join Ingredients on ProductRecipe.IngredientId=Ingredients.id
                left outer join (
                    select distinct
                        Product.*,
                        Ingredients.id ingid
                    from Product
                    left outer join ProductExtras on Product.Id=ProductExtras.productid
                    left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
                    left outer join Ingredients on ProductExtras.IngredientId=Ingredients.id
                    where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
                ) tmp on tmp.Id=product.Id and isnull(tmp.ingid,0) = isnull(Ingredients.id,0)
                INNER JOIN PriceListDetail pld on pld.ProductId = Product.Id and pld.PricelistId = @PriceListId
                LEFT OUTER JOIN PriceListDetail pldIng on pldIng.IngredientId = Ingredients.id and pldIng.PricelistId = @PriceListId
                where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) and tmp.id is null

              union all

                select distinct Product.*,isnull(pld.Price,0) Price,
                    Ingredients.id id,Product.id productid,isnull(Ingredients.Description,'-')Description,isnull(ExtendedDescription,'-')ExtendedDescription,
                    isnull(Ingredients.SalesDescription,'-')SalesDescription,isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,
                    isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,Ingredients.UnitId UnitId,Ingredients.Code, isnull(pldIng.Price,0) Price --, 0 isRecipe
                from Product
                left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
                left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
                left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id
                INNER JOIN PriceListDetail pld on pld.ProductId = Product.Id and pld.PricelistId = @PriceListId
                LEFT OUTER JOIN PriceListDetail pldIng on pldIng.IngredientId = Ingredients.id and pldIng.PricelistId = @PriceListId
                where Product.Code=@Code and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
          ";

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0);  //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          }, new { Code = Code, PriceListId = PricelistId });
                ProductExtModel prodex = lookup.Select(x => x.Value).ToList<ProductExtModel>().FirstOrDefault();
                //  prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                return prodex;
            }

        }

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. Prices ARE INCLUDED.  
        /// If product is not active or it is not exists into DB or does not belong to price-list then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="PricelistId">Pricelist Id</param>
        /// <returns></returns>
        public ProductExtModel GetProductExt(DBInfoModel Store, long ProductId, long PricelistId)
        {
            string sql = @"
               select distinct
	                Product.*, ISNULL(pd.Price,0) Price,
	                Ingredients.id id,ProductExtras.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,
	                isnull(ProductExtras.MinQty,0)MinQty,isnull(ProductExtras.MaxQty,0)MaxQty,ProductExtras.UnitId UnitId,Ingredients.Code, 0 isRecipe, 
	                ISNULL(pdi.Price,0) Price
	            from Product 
	            inner join ProductExtras on Product.Id=ProductExtras.productid
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = ProductExtras.IngredientId
	            inner join Ingredients on ProductExtras.IngredientId=Ingredients.id 
	            INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where Product.Id=@ProductId and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null

	       union all

	            select distinct
	                Product.*, ISNULL(pd.Price,0) Price,
	                Ingredients.id id,ProductRecipe.productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,
	                isnull(MinQty,0)MinQty,isnull(MaxQty,0)MaxQty,ProductRecipe.UnitId UnitId,Ingredients.Code,1 isRecipe, ISNULL(pdi.Price,0) Price
	            from Product 
	            inner join ProductRecipe on Product.Id=ProductRecipe.productid
	            inner join Ingredients on ProductRecipe.IngredientId=Ingredients.id 
	            INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where Product.Id=@ProductId and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0) 

	       union all

				select distinct Product.*, ISNULL(pd.Price,0) Price,
	                Ingredients.id id,Product.id productid,Ingredients.Description,ExtendedDescription,Ingredients.SalesDescription,
	                isnull(Ingredient_ProdCategoryAssoc.MinQty,0)MinQty,isnull(Ingredient_ProdCategoryAssoc.MaxQty,10)MaxQty,
	                Ingredients.UnitId UnitId,Ingredients.Code , 0 isRecipe, ISNULL(pdi.Price,0) Price
	            from Product 
	            left outer join Ingredient_ProdCategoryAssoc on Product.ProductCategoryId=Ingredient_ProdCategoryAssoc.ProductCategoryId
				left outer join ProductRecipe pr on Product.Id=pr.productid and pr.IngredientId  = Ingredient_ProdCategoryAssoc.IngredientId
	            left outer join Ingredients on Ingredient_ProdCategoryAssoc.IngredientId=Ingredients.id 
	            INNER JOIN PricelistDetail AS pd ON pd.ProductId = product.Id AND pd.PricelistId = @PriceListId
	            LEFT OUTER JOIN PricelistDetail AS pdi ON pdi.IngredientId = Ingredients.Id AND pdi.PricelistId = @PriceListId
	            where Product.Id=@ProductId and (Product.IsDeleted is null or Product.IsDeleted=0) and (Ingredients.IsDeleted is null or Ingredients.IsDeleted=0)  and pr.id is null
           
           ";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            var lookup = new Dictionary<long, ProductExtModel>();//Create a data structure to store products uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<ProductExtModel, ProductExtrasIngredientsModel, ProductExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (product, prextras) =>
                          {
                              ProductExtModel prod;
                              if (!lookup.TryGetValue(product.Id, out prod))
                              {
                                  prod = product;//the product does not exit into lookup dictionary
                                  lookup.Add(product.Id, product);
                              }

                              if (prextras != null && prextras.Id != null && prod.Extras.FindIndex(x => x.Id == prextras.Id) < 0)
                              {
                                  prextras.isRecipe = ingredientsDT.IsProductRecipe(Store, product.Id, prextras.Id ?? 0); //  <--- GET IF EXTRA IS IN RECIPY ---<<<
                                  prod.Extras.Add(prextras);
                              }
                              return prod;
                          }, new { ProductId = ProductId, PriceListId = PricelistId });
                ProductExtModel prodex = lookup.Select(x => x.Value).ToList<ProductExtModel>().FirstOrDefault();
                //     prodex.Extras = prodex.Extras.Where(x => x != null).ToList();
                return prodex;
            }

        }


        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductSched_Model item in model)
                {
                    item.UnitId = unitDT.GetIdByDAIs(Store, item.UnitId ?? 0);
                    item.ProductCategoryId = prodCategDT.GetIdByDAIs(Store, item.ProductCategoryId ?? 0);
                }
                results = this.dt.Upsert(db, Mapper.Map<List<ProductDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<ProductSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    item.Id = GetIdByDAIs(Store, item.TableId ?? 0) ?? 0;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<ProductDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, ProductModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<ProductDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<ProductModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<ProductDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, ProductModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<ProductDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public long? GetIdByDAIs(DBInfoModel Store, long dAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                ProductDTO tmp = dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                if (tmp == null)
                    return null;
                else
                    return tmp.Id;
            }
        }

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public ProductModel GetModelByDAIs(DBInfoModel Store, long dAId, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            if (dbTran != null)
                return Mapper.Map<ProductModel>(dt.SelectFirst(dbTran, "WHERE DAId = @DAId", new { DAId = dAId }, dbTransact));
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return Mapper.Map<ProductModel>(dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId }, dbTransact));
                }
            }
        }


        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public ProductModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            if (dbTran != null)
                return Mapper.Map<ProductModel>(dt.SelectFirst(dbTran, "WHERE Id = @Id", new { Id = Id }, dbTransact));
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return Mapper.Map<ProductModel>(dt.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }, dbTransact));
                }
            }
        }


        /// <summary>
        /// get list of product ids and descriptions based on given product ids
        /// </summary>
        /// <param name="productIdList">List<long></param>
        /// <returns>List<ProductDescription></returns>
        public List<ProductDescription> GetProductDescriptions(DBInfoModel DBInfo, ProductIdsList productIdList)
        {
            List<ProductDescription> result = new List<ProductDescription>();

            if (productIdList == null)
            {
                return result;
            }
            if(productIdList.productIdList.Count == 0)
            {
                return result;
            }

            string ids = String.Join(",", productIdList.productIdList);

            string SqlData = @"SELECT p.Id, p.[Description] AS Descr FROM Product AS p WHERE p.Id IN (" + ids + ")";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                result = db.Query<ProductDescription>(SqlData).ToList();
            }

            return result;
        }
    }
}
