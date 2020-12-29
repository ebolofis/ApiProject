using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.30")]
    public class Version_2_0_0_30
    {
        public List<string> Ver_2_0_0_30 { get; }

        public Version_2_0_0_30()
        {
            Ver_2_0_0_30 = new List<string>();
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_Addresses_AddressCity' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Addresses') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_Addresses_AddressCity] ON [dbo].[DA_Addresses] ( [AddressStreet] ASC, [City] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_Addresses_ZipCode' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Addresses') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_Addresses_ZipCode] ON [dbo].[DA_Addresses] ( [Zipcode] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_Addresses_OwnerId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Addresses') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_Addresses_OwnerId] ON [dbo].[DA_Addresses] ( [OwnerId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_Addresses_AddressArea' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Addresses') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_Addresses_AddressArea] ON [dbo].[DA_Addresses] ( [AddressStreet] ASC, [Area] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_CustomersEmailPass' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Customers') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_CustomersEmailPass] ON [dbo].[DA_Customers] ( [Email] ASC, [Password] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_CustomersEmail' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Customers') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_CustomersEmail] ON [dbo].[DA_Customers] ( [Email] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_CustomersLastName' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Customers') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_CustomersLastName] ON [dbo].[DA_Customers] ( [LastName] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_CustomersLastNamePhone' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Customers') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_CustomersLastNamePhone] ON [dbo].[DA_Customers] ( [LastName] ASC, [Phone1] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_CustomersVatNo' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Customers') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_CustomersVatNo] ON [dbo].[DA_Customers] ( [VatNo] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_CustomersPhone' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Customers') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_CustomersPhone] ON [dbo].[DA_Customers] ( [Phone1] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_CustomersMobile' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Customers') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_CustomersMobile] ON [dbo].[DA_Customers] ( [Mobile] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_GeoPolygonsStoreId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_GeoPolygons') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_GeoPolygonsStoreId] ON [dbo].[DA_GeoPolygons] ( [StoreId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_GeoPolygonsName' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_GeoPolygons') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_GeoPolygonsName] ON [dbo].[DA_GeoPolygons] ( [Name] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_GeoPolygonsStoreIdName' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_GeoPolygons') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_GeoPolygonsStoreIdName] ON [dbo].[DA_GeoPolygons] ( [StoreId] ASC, [Name] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_GeoPolygonsDetailsPolygonId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_GeoPolygonsDetails') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_GeoPolygonsDetailsPolygonId] ON [dbo].[DA_GeoPolygonsDetails] ( [PolygId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrderDetails_OrderId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_OrderDetails') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrderDetails_OrderId] ON [dbo].[DA_OrderDetails] ( [DAOrderId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrderDetails_OrderIdProduct' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_OrderDetails') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrderDetails_OrderIdProduct] ON [dbo].[DA_OrderDetails] ( [DAOrderId] ASC, [ProductId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrderDetails_OrderIdProductPriceList' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_OrderDetails') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrderDetails_OrderIdProductPriceList] ON [dbo].[DA_OrderDetails] ( [DAOrderId] ASC, [ProductId] ASC, [PriceListId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrderDetails_ProductIdPriceListId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_OrderDetails') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrderDetails_ProductIdPriceListId] ON [dbo].[DA_OrderDetails] ( [ProductId] ASC, [PriceListId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrderDetailsExtrasOrderDetId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_OrderDetailsExtras') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrderDetailsExtrasOrderDetId] ON [dbo].[DA_OrderDetailsExtras] ( [OrderDetailId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrderDetailsExtrasOrderExtrasId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_OrderDetailsExtras') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrderDetailsExtrasOrderExtrasId] ON [dbo].[DA_OrderDetailsExtras] ( [ExtrasId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrderDetailsExtrasOrderOrderDetIdExtrasId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_OrderDetailsExtras') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrderDetailsExtrasOrderOrderDetIdExtrasId] ON [dbo].[DA_OrderDetailsExtras] ( [OrderDetailId] ASC, [ExtrasId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersCustomerId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersCustomerId] ON [dbo].[DA_Orders] ( [CustomerId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersStoreId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersStoreId] ON [dbo].[DA_Orders] ( [StoreId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersCustomerStoreId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersCustomerStoreId] ON [dbo].[DA_Orders] ( [CustomerId] ASC, [StoreId] ASC)");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersShippingAddressId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersShippingAddressId] ON [dbo].[DA_Orders] ( [ShippingAddressId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersOrderDate' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersOrderDate] ON [dbo].[DA_Orders] ( [OrderDate] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersEstBillingDate' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersEstBillingDate] ON [dbo].[DA_Orders] ( [EstBillingDate] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersBillingDate' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersBillingDate] ON [dbo].[DA_Orders] ( [BillingDate] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersEstTakeoutDate' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersEstTakeoutDate] ON [dbo].[DA_Orders] ( [EstTakeoutDate] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersTakeoutDate' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersTakeoutDate] ON [dbo].[DA_Orders] ( [TakeoutDate] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersFinishDate' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersFinishDate] ON [dbo].[DA_Orders] ( [FinishDate] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersDuration' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersDuration] ON [dbo].[DA_Orders] ( [Duration] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_OrdersDuration' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Orders') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_OrdersDuration] ON [dbo].[DA_Orders] ( [Duration] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_ShortageProdsProductStore' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_ShortageProds') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_ShortageProdsProductStore] ON [dbo].[DA_ShortageProds] ( [ProductId] ASC, [StoreId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_ShortageProdsProductStoreShort' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_ShortageProds') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_ShortageProdsProductStoreShort] ON [dbo].[DA_ShortageProds] ( [ProductId] ASC, [StoreId] ASC, [ShortType] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_ShortageProdsShortType' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_ShortageProds') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_ShortageProdsShortType] ON [dbo].[DA_ShortageProds] ( [ProductId] ASC, [StoreId] ASC, [ShortType] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_StoresCode' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Stores') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_StoresCode] ON [dbo].[DA_Stores] ( [Code] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_StoresUserPass' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_Stores') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_StoresUserPass] ON [dbo].[DA_Stores] ( [Username] ASC, [Password] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DAStore_PriceListAssocPriceListStore' AND OBJECT_NAME(idx.OBJECT_ID) = 'DAStore_PriceListAssoc') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DAStore_PriceListAssocPriceListStore] ON [dbo].[DAStore_PriceListAssoc] ( [PriceListId] ASC, [DAStoreId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_LoyalPointsCustomerId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_LoyalPoints') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_LoyalPointsCustomerId] ON [dbo].[DA_LoyalPoints] ( [CustomerId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_LoyalPointsOrderId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_LoyalPoints') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_LoyalPointsOrderId] ON [dbo].[DA_LoyalPoints] ( [OrderId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_LoyalPointsDate' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_LoyalPoints') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_LoyalPointsDate] ON [dbo].[DA_LoyalPoints] ( [Date] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_LoyalPointsCustomerIdOrderId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_LoyalPoints') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_LoyalPointsCustomerIdOrderId] ON [dbo].[DA_LoyalPoints] ( [CustomerId] ASC, [OrderId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_LoyalPointsDateCustomerId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_LoyalPoints') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_LoyalPointsDateCustomerId] ON [dbo].[DA_LoyalPoints] ( [Date] ASC, [CustomerId] ASC )");
            Ver_2_0_0_30.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes idx WHERE idx.name='IX_DA_LoyalPointsDateOrderId' AND OBJECT_NAME(idx.OBJECT_ID) = 'DA_LoyalPoints') \n"
                          + "	CREATE NONCLUSTERED INDEX [IX_DA_LoyalPointsDateOrderId] ON [dbo].[DA_LoyalPoints] ( [Date] ASC, [OrderId] ASC )");
        }
    }
}
