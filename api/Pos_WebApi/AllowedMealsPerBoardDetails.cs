
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Pos_WebApi
{

using System;
    using System.Collections.Generic;
    
public partial class AllowedMealsPerBoardDetails
{

    public long Id { get; set; }

    public Nullable<long> ProductCategoryId { get; set; }

    public Nullable<long> AllowedMealsPerBoardId { get; set; }



    public virtual AllowedMealsPerBoard AllowedMealsPerBoard { get; set; }

    public virtual ProductCategories ProductCategories { get; set; }

}

}