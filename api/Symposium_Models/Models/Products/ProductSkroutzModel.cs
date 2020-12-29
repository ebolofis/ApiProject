using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Skroutz Section --> ProductCategoery
    /// A Section is the entity under which similar Plates may be logically grouped in a Catalog. A Catalog may have zero or more Sections.
    /// </summary>
    public class ProductCategoriesSkroutzModel
    {
        /// <summary>
        /// A unique identifier for the resource --> ProductCategory Code
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// example: Καφέδες & Ροφήματα --> ProductCategory Description
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// example: Καφέδες & Ροφήματα --> ProductCategory Description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// The ID of the Catalog this Sections belongs to. --> ProductCategory CategoryId
        /// </summary>
        public string catalog_id  { get; set; }

        /// <summary>
        /// A number to dictate the ordering in which sections appear in the Catalog. The lower this number, the earlier the Section will appear. --> ProductCategory Code
        /// minimum: 0
        /// maximum: 1000
        /// </summary>
        public int orderno { get; set; }
    }

    /// <summary>
    /// Skroutz Plate --> Product
    /// A Plate is a purchasable catalog item (e.g. a Pizza, a coffee), along with its all available variation choices and price information. It's the core unit of a Catalog.
    /// </summary>
    public class ProductSkroutzModel
    {
        /// <summary>
        /// A unique identifier for the resource --> Product Code
        /// </summary>
        public string id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        /// <summary>
        /// section_id --> Product ProductCategoryId
        /// The ID of the Section this Plate belongs to.
        /// </summary>
        public string section_id { get; set; }

        public string image { get; set; }

        /// <summary>
        /// Product Price
        /// Initial/base price of the plate. If the initial price of the plate depends on a choice (e.g. "Μέγεθος"), omit this field completely or set it to 0.
        /// default: 0
        /// example: 350
        /// </summary>
        public int price_in_cents { get; set; }

        /// <summary>
        /// List Of Products Ingredient Categories
        /// </summary>
        public List<IngredientCategoriesSkroutzModel> Option { get; set; } = new List<IngredientCategoriesSkroutzModel>();
    }

    /// <summary>
    /// Skroutz Option --> Ingredient Categories
    /// An Option belongs to a Plate and depicts a characteristic of a plate that may be customized by the user via Choices(see below). 
    /// A Plate(e.g. "Espresso Freddo") may have zero or more Options(e.g. "Είδος Ζάχαρης") with each one having 
    /// one or more available Choices(e.g. "Καστανή Ζάχαρη", "Ζαχαρίνη", "Λευκή Ζάχαρη").
    /// </summary>
    public class IngredientCategoriesSkroutzModel
    {
        /// <summary>
        /// A unique identifier for the resource --> IngredientCategories Code
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// name --> IngredientCategories Description
        /// A label for the option.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// base_price 
        /// default: false
        /// True if the plate's initial price is decided by this option, false otherwise. For example, typically a "Μέγεθος" option for a "Freddo Espresso" plate, 
        /// would have this set to true. On the contrary, a "Είδος Ζάχαρης" option for the same plate would have this set to false.
        /// </summary>
        public bool base_price { get; set; }

        /// <summary>
        /// multiple_selections --> IngredientCategories IsUnique
        /// True if multiple choices of this option may be selected, false otherwise.For example, a "Έξτρα Υλικά" option would typically have this set to true. 
        /// On the contrary, a "Μέγεθος" option would have this set to false.
        /// </summary>
        public bool multiple_selections { get; set; }

        /// <summary>
        /// List Of Ingredients for specifoc Ingredient Categories 
        /// </summary>
        public List<IngredientSkroutzModel> Choice { get; set; } = new List<IngredientSkroutzModel>();
    }

    /// <summary>
    /// Skroutz Choice --> Ingredient Categories
    /// A Choice represents a possible selection for a particular Option.See description of Option for more information.
    /// </summary>
    public class IngredientSkroutzModel
    {
        /// <summary>
        /// A unique identifier for the resource --> Ingredient Code
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// name --> Ingredient Description
        /// A label for the choice
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Ingredient Price
        /// Additional cost added to the base price of the plate, if this choice is selected. May be ommited or set to 0 if choice is free of charge.
        /// </summary>
        public int price_in_cents { get; set; }

        /// <summary>
        /// selected --> if Ingredient belong in recipe
        /// True if the choice should be pre-selected by default in the UI, false otherwise.If the parent Option has its multiple_selections field set to false, 
        /// exactly one of its choices must have this field set to true.
        /// </summary>
        public bool selected { get; set; }
    }



    /// <summary>
    /// ProductCategoriesSkroutzModel with the list of Products
    /// </summary>
    public class ProductCategoriesSkroutzExtModel : ProductCategoriesSkroutzModel
    {
        /// <summary>
        /// list of Products
        /// </summary>
        public List<ProductSkroutzModel> Plate { get; set; } = new List<ProductSkroutzModel>();
    }
}
