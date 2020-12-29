using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.33")]
    public class Version_2_0_0_33
    {
        public List<string> Ver_2_0_0_33 { get; }

        public Version_2_0_0_33()
        {
            Ver_2_0_0_33 = new List<string>();
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Addresses_OwnAddType] ON [dbo].[DA_Addresses] \n"
                           + "( \n"
                           + "	[OwnerId] ASC, \n"
                           + "	[AddressType] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Addresses_OwnAddTypeIsDel] ON [dbo].[DA_Addresses] \n"
                           + "( \n"
                           + "	[OwnerId] ASC, \n"
                           + "	[AddressType] ASC, \n"
                           + "  [IsDeleted] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Pricelist_DAId] ON [dbo].[Pricelist] \n"
                           + "( \n"
                           + "	[DAId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_PricelistDetail_Prod_PrLst] ON [dbo].[PricelistDetail] \n"
                           + "( \n"
                           + "	[ProductId] ASC, \n"
                           + "	[PricelistId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Product_DAId] ON [dbo].[Product] \n"
                           + "( \n"
                           + "	[DAId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Ingredients_DAId] ON [dbo].[Ingredients] \n"
                           + "( \n"
                           + "	[DAId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_InvoiceShippingDetails_Invoice] ON [dbo].[InvoiceShippingDetails] \n"
                           + "( \n"
                           + "	[InvoicesId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_OrderStatus_Time] ON [dbo].[OrderStatus] \n"
                           + "( \n"
                           + "	[TimeChanged] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Order_ExtType_ExtKey] ON [dbo].[Order] \n"
                           + "( \n"
                           + "	[ExtType] ASC, \n"
                           + "	[ExtKey] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Order_ExtType_ExtKey_PosId] ON [dbo].[Order] \n"
                           + "( \n"
                           + "	[ExtType] ASC, \n"
                           + "	[ExtKey] ASC, \n"
                           + "	[PosId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Order_ExtType_ExtKey_StaffId] ON [dbo].[Order] \n"
                           + "( \n"
                           + "	[ExtType] ASC, \n"
                           + "	[ExtKey] ASC, \n"
                           + "	[StaffId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_OrderDetail_OrderId] ON [dbo].[OrderDetail] \n"
                           + "( \n"
                           + "	[OrderId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_OrderDetailInvoices_OrderDetail] ON [dbo].[OrderDetailInvoices] \n"
                           + "( \n"
                           + "	[OrderDetailId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_InvoiceTypes_Id_Type] ON [dbo].[InvoiceTypes] \n"
                           + "( \n"
                           + "	[Id] ASC, \n"
                           + "	[Type] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_Customers_ExtType_ExtCust] ON [dbo].[Delivery_Customers] \n"
                           + "( \n"
                           + "	[ExtType] ASC, \n"
                           + "	[ExtCustId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_CustomersBillingAddress_Cust_ExtKey_ExtType] ON [dbo].[Delivery_CustomersBillingAddress] \n"
                           + "( \n"
                           + "	[CustomerID] ASC, \n"
                           + "	[ExtKey] ASC, \n"
                           + "	[ExtType] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_CustomersBillingAddress_Id_Cust] ON [dbo].[Delivery_CustomersBillingAddress] \n"
                           + "( \n"
                           + "	[ID] ASC, \n"
                           + "	[CustomerID] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_CustomersShippingAddress_Cust_ExtKey_ExtType] ON [dbo].[Delivery_CustomersShippingAddress] \n"
                           + "( \n"
                           + "	[CustomerID] ASC, \n"
                           + "	[ExtKey] ASC, \n"
                           + "	[ExtType] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_CustomersShippingAddress_Id_Cust] ON [dbo].[Delivery_CustomersShippingAddress] \n"
                           + "( \n"
                           + "	[ID] ASC, \n"
                           + "	[CustomerID] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_CustomersPhones_Cust_Id] ON [dbo].[Delivery_CustomersPhones] \n"
                           + "( \n"
                           + "	[CustomerID] ASC, \n"
                           + "	[ID] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_CustomersPhones_Cust_Phone] ON [dbo].[Delivery_CustomersPhones] \n"
                           + "( \n"
                           + "	[CustomerID] ASC, \n"
                           + "	[PhoneNumber] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Delivery_CustomersPhonesAndAddress] ON [dbo].[Delivery_CustomersPhonesAndAddress] \n"
                           + "( \n"
                           + "	[CustomerID] ASC, \n"
                           + "	[PhoneID] ASC, \n"
                           + "	[AddressID] ASC, \n"
                           + "	[IsShipping] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Guest_Profile] ON [dbo].[Guest] \n"
                           + "( \n"
                           + "	[ProfileNo] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Customers_Email_IsDel] ON [dbo].[DA_Customers] \n"
                           + "( \n"
                           + "	[Email] ASC, \n"
                           + "	[IsDeleted] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Customers_Email_Sess_IsDel] ON [dbo].[DA_Customers] \n"
                           + "( \n"
                           + "	[Email] ASC, \n"
                           + "	[SessionKey] ASC, \n"
                           + "	[IsDeleted] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Customers_ExtId1] ON [dbo].[DA_Customers] \n"
                           + "( \n"
                           + "	[ExtId1] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_GeoPolygons_Active] ON [dbo].[DA_GeoPolygons] \n"
                           + "( \n"
                           + "	[IsActive] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Orders_Cust_Ship] ON [dbo].[DA_Orders] \n"
                           + "( \n"
                           + "	[CustomerId] ASC, \n"
                           + "	[ShippingAddressId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Orders_ExtId1] ON [dbo].[DA_Orders] \n"
                           + "( \n"
                           + "	[ExtId1] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Orders_IsSend] ON [dbo].[DA_Orders] \n"
                           + "( \n"
                           + "	[IsSend] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_Orders_IsSend_Id] ON [dbo].[DA_Orders] \n"
                            + "( \n"
                            + "	[IsSend] ASC, \n"
                            + "	[Id] ASC \n"
                            + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_ScheduledTaskes_Failed] ON [dbo].[DA_ScheduledTaskes] \n"
                           + "( \n"
                           + "	[FaildNo] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_ScheduledTaskes_Store_Action_Short] ON [dbo].[DA_ScheduledTaskes] \n"
                           + "( \n"
                           + "	[StoreId] ASC, \n"
                           + "	[Action] ASC, \n"
                           + "	[Short] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_Staff_Code_Pass] ON [dbo].[Staff] \n"
                           + "( \n"
                           + "	[Code] ASC, \n"
                           + "	[Password] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_LoyalPoints_Owner_Store] ON [dbo].[DA_LoyalPoints] \n"
                           + "( \n"
                           + "	[OrderId] ASC, \n"
                           + "	[StoreId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_LoyalRedeemFreeProduct_Points] ON [dbo].[DA_LoyalRedeemFreeProduct] \n"
                           + "( \n"
                           + "	[Points] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_OrderStatus_OrderDAId] ON [dbo].[DA_OrderStatus] \n"
                           + "( \n"
                           + "	[OrderDAId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_ShortageProds_ProdId] ON [dbo].[DA_ShortageProds] \n"
                           + "( \n"
                           + "	[ProductId] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_ShortageProds_Store_ShortType] ON [dbo].[DA_ShortageProds] \n"
                           + "( \n"
                           + "	[StoreId] ASC, \n"
                           + "	[ShortType] ASC \n"
                           + ")");
            Ver_2_0_0_33.Add("CREATE NONCLUSTERED INDEX [IX_DA_ShortageProds_StoreId] ON [dbo].[DA_ShortageProds] \n"
                           + "( \n"
                           + "	[StoreId] ASC \n"
                           + ")");
        }
    }
}
