
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
    
public partial class OnlineRegistration
{

    public string BarCode { get; set; }

    public string FirtName { get; set; }

    public string LastName { get; set; }

    public string Mobile { get; set; }

    public int Dates { get; set; }

    public int Children { get; set; }

    public int Adults { get; set; }

    public int PaymentType { get; set; }

    public decimal ChildTicket { get; set; }

    public decimal AdultTicket { get; set; }

    public System.DateTime RegistrationDate { get; set; }

    public int TotalChildren { get; set; }

    public int TotalAdults { get; set; }

    public int ChildrenEntered { get; set; }

    public int AdultsEntered { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public int Status { get; set; }

    public string Email { get; set; }

    public string OrderNumber { get; set; }

    public Nullable<bool> Gdpr { get; set; }

    public Nullable<bool> isAnonymized { get; set; }

}

}
