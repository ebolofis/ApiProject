namespace hit.webpos.entities {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class WebPos : DbContext {
        public WebPos()
            : base("name=WebPos") {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<AllowedMealsPerBoard> AllowedMealsPerBoards { get; set; }
        public virtual DbSet<AllowedMealsPerBoardDetail> AllowedMealsPerBoardDetails { get; set; }
        public virtual DbSet<AssignedPosition> AssignedPositions { get; set; }
        public virtual DbSet<AuthorizedGroup> AuthorizedGroups { get; set; }
        public virtual DbSet<AuthorizedGroupDetail> AuthorizedGroupDetails { get; set; }
        public virtual DbSet<BoardMeal> BoardMeals { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ClientPos> ClientPos { get; set; }
        public virtual DbSet<CreditAccount> CreditAccounts { get; set; }
        public virtual DbSet<CreditCode> CreditCodes { get; set; }
        public virtual DbSet<CreditTransaction> CreditTransactions { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<EndOfDay> EndOfDays { get; set; }
        public virtual DbSet<EndOfDayDetail> EndOfDayDetails { get; set; }
        public virtual DbSet<EndOfDayPaymentAnalysi> EndOfDayPaymentAnalysis { get; set; }
        public virtual DbSet<EndOfDayVoidsAnalysi> EndOfDayVoidsAnalysis { get; set; }
        public virtual DbSet<EODAccountToPmsTransfer> EODAccountToPmsTransfers { get; set; }
        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<HotelInfo> HotelInfoes { get; set; }
        public virtual DbSet<Ingredient_ProdCategoryAssoc> Ingredient_ProdCategoryAssoc { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Invoice_Guests_Trans> Invoice_Guests_Trans { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceShippingDetail> InvoiceShippingDetails { get; set; }
        public virtual DbSet<InvoiceType> InvoiceTypes { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Kd> Kds { get; set; }
        public virtual DbSet<Kitchen> Kitchens { get; set; }
        public virtual DbSet<KitchenInstruction> KitchenInstructions { get; set; }
        public virtual DbSet<KitchenInstructionLogger> KitchenInstructionLoggers { get; set; }
        public virtual DbSet<KitchenRegion> KitchenRegions { get; set; }
        public virtual DbSet<Locker> Lockers { get; set; }
        public virtual DbSet<MealConsumption> MealConsumptions { get; set; }
        public virtual DbSet<MetadataTable> MetadataTables { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderDetailIgredient> OrderDetailIgredients { get; set; }
        public virtual DbSet<OrderDetailIgredientVatAnal> OrderDetailIgredientVatAnals { get; set; }
        public virtual DbSet<OrderDetailInvoice> OrderDetailInvoices { get; set; }
        public virtual DbSet<OrderDetailVatAnal> OrderDetailVatAnals { get; set; }
        public virtual DbSet<OrdersStaff> OrdersStaffs { get; set; }
        public virtual DbSet<OrderStatu> OrderStatus { get; set; }
        public virtual DbSet<PageButton> PageButtons { get; set; }
        public virtual DbSet<PageButtonDetail> PageButtonDetails { get; set; }
        public virtual DbSet<PagePosAssoc> PagePosAssocs { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageSet> PageSets { get; set; }
        public virtual DbSet<PdaModule> PdaModules { get; set; }
        public virtual DbSet<PdaModuleDetail> PdaModuleDetails { get; set; }
        public virtual DbSet<PosInfo> PosInfoes { get; set; }
        public virtual DbSet<PosInfo_KitchenInstruction_Assoc> PosInfo_KitchenInstruction_Assoc { get; set; }
        public virtual DbSet<PosInfo_Pricelist_Assoc> PosInfo_Pricelist_Assoc { get; set; }
        public virtual DbSet<PosInfo_Region_Assoc> PosInfo_Region_Assoc { get; set; }
        public virtual DbSet<PosInfo_StaffPositin_Assoc> PosInfo_StaffPositin_Assoc { get; set; }
        public virtual DbSet<PosInfoDetail> PosInfoDetails { get; set; }
        public virtual DbSet<PosInfoDetail_Excluded_Accounts> PosInfoDetail_Excluded_Accounts { get; set; }
        public virtual DbSet<PosInfoDetail_Pricelist_Assoc> PosInfoDetail_Pricelist_Assoc { get; set; }
        public virtual DbSet<PosInfoKdsAssoc> PosInfoKdsAssocs { get; set; }
        public virtual DbSet<PredefinedCredit> PredefinedCredits { get; set; }
        public virtual DbSet<Pricelist> Pricelists { get; set; }
        public virtual DbSet<PriceList_EffectiveHours> PriceList_EffectiveHours { get; set; }
        public virtual DbSet<PricelistDetail> PricelistDetails { get; set; }
        public virtual DbSet<PricelistMaster> PricelistMasters { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductBarcode> ProductBarcodes { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductExtra> ProductExtras { get; set; }
        public virtual DbSet<ProductForBarcodeEod> ProductForBarcodeEods { get; set; }
        public virtual DbSet<ProductRecipe> ProductRecipes { get; set; }
        public virtual DbSet<ProductSalesHistoryPerDay> ProductSalesHistoryPerDays { get; set; }
        public virtual DbSet<ProductsForEOD> ProductsForEODs { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<RegionLockerProduct> RegionLockerProducts { get; set; }
        public virtual DbSet<ReportList> ReportLists { get; set; }
        public virtual DbSet<SalesType> SalesTypes { get; set; }
        public virtual DbSet<SalesType_PricelistMaster_Assoc> SalesType_PricelistMaster_Assoc { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<StaffAuthorization> StaffAuthorizations { get; set; }
        public virtual DbSet<StaffPosition> StaffPositions { get; set; }
        public virtual DbSet<StaffSchedule> StaffSchedules { get; set; }
        public virtual DbSet<StaffSecheduleDetail> StaffSecheduleDetails { get; set; }
        public virtual DbSet<StatisticsMenu> StatisticsMenus { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreMessage> StoreMessages { get; set; }
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<TablePaySuggestion> TablePaySuggestions { get; set; }
        public virtual DbSet<Tax> Taxes { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }
        public virtual DbSet<TransferMappingDetail> TransferMappingDetails { get; set; }
        public virtual DbSet<TransferMapping> TransferMappings { get; set; }
        public virtual DbSet<TransferToPm> TransferToPms { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<ValidModule> ValidModules { get; set; }
        public virtual DbSet<Vat> Vats { get; set; }
        public virtual DbSet<Version> Versions { get; set; }
        public virtual DbSet<WorkSheet> WorkSheets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Account>()
                .HasMany(e => e.EndOfDayVoidsAnalysis)
                .WithOptional(e => e.Account)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Account>()
                .HasMany(e => e.TablePaySuggestions)
                .WithOptional(e => e.Account)
                .WillCascadeOnDelete();

            modelBuilder.Entity<AllowedMealsPerBoard>()
                .Property(e => e.AllowedDiscountAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<AllowedMealsPerBoard>()
                .Property(e => e.AllowedDiscountAmountChild)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CreditTransaction>()
                .Property(e => e.Amount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.TransferMappings)
                .WithOptional(e => e.Department)
                .HasForeignKey(e => e.PosDepartmentId);

            modelBuilder.Entity<Discount>()
                .Property(e => e.Amount)
                .HasPrecision(9, 2);

            modelBuilder.Entity<EndOfDay>()
                .Property(e => e.Gross)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDay>()
                .Property(e => e.Net)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDay>()
                .HasMany(e => e.EndOfDayDetails)
                .WithOptional(e => e.EndOfDay)
                .WillCascadeOnDelete();

            modelBuilder.Entity<EndOfDay>()
                .HasMany(e => e.EndOfDayVoidsAnalysis)
                .WithOptional(e => e.EndOfDay)
                .WillCascadeOnDelete();

            modelBuilder.Entity<EndOfDay>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.EndOfDay)
                .WillCascadeOnDelete();

            modelBuilder.Entity<EndOfDayDetail>()
                .Property(e => e.VatRate)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDayDetail>()
                .Property(e => e.VatAmount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDayDetail>()
                .Property(e => e.TaxAmount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDayDetail>()
                .Property(e => e.Gross)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDayDetail>()
                .Property(e => e.Net)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDayDetail>()
                .Property(e => e.Discount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<EndOfDayVoidsAnalysi>()
                .Property(e => e.Total)
                .HasPrecision(9, 2);

            modelBuilder.Entity<Guest>()
                .HasMany(e => e.TablePaySuggestions)
                .WithOptional(e => e.Guest)
                .WillCascadeOnDelete();

            modelBuilder.Entity<HotelInfo>()
                .Property(e => e.HotelUri)
                .IsUnicode(false);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Total)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Discount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Vat)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Tax)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Net)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.InvoiceShippingDetails)
                .WithOptional(e => e.Invoice)
                .HasForeignKey(e => e.InvoicesId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.OrderDetailInvoices)
                .WithOptional(e => e.Invoice)
                .HasForeignKey(e => e.InvoicesId);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Invoice)
                .HasForeignKey(e => e.InvoicesId);

            modelBuilder.Entity<InvoiceType>()
                .HasMany(e => e.PosInfoDetails)
                .WithOptional(e => e.InvoiceType)
                .HasForeignKey(e => e.InvoicesTypeId);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.ProductExtras)
                .WithOptional(e => e.Item)
                .HasForeignKey(e => e.ItemsId);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.ProductRecipes)
                .WithOptional(e => e.Item)
                .HasForeignKey(e => e.ItemsId);

            modelBuilder.Entity<Kd>()
                .HasMany(e => e.PosInfoKdsAssocs)
                .WithOptional(e => e.Kd)
                .HasForeignKey(e => e.KdsId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Kd>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Kd)
                .HasForeignKey(e => e.KdsId);

            modelBuilder.Entity<KitchenInstruction>()
                .HasMany(e => e.KitchenInstructionLoggers)
                .WithOptional(e => e.KitchenInstruction)
                .HasForeignKey(e => e.KicthcenInstuctionId);

            modelBuilder.Entity<Order>()
                .Property(e => e.Total)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Order>()
                .Property(e => e.TotalBeforeDiscount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderDetails)
                .WithOptional(e => e.Order)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrdersStaffs)
                .WithOptional(e => e.Order)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderStatus)
                .WithOptional(e => e.Order)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.Discount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.TotalAfterDiscount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetail>()
                .HasMany(e => e.OrderDetailIgredients)
                .WithOptional(e => e.OrderDetail)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrderDetail>()
                .HasMany(e => e.OrderDetailInvoices)
                .WithOptional(e => e.OrderDetail)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrderDetail>()
                .HasMany(e => e.OrderDetailVatAnals)
                .WithOptional(e => e.OrderDetail)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrderDetail>()
                .HasMany(e => e.TablePaySuggestions)
                .WithOptional(e => e.OrderDetail)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrderDetailIgredient>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderDetailIgredient>()
                .Property(e => e.Discount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetailIgredient>()
                .Property(e => e.TotalAfterDiscount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetailIgredient>()
                .HasMany(e => e.OrderDetailInvoices)
                .WithOptional(e => e.OrderDetailIgredient)
                .HasForeignKey(e => e.OrderDetailIgredientsId);

            modelBuilder.Entity<OrderDetailIgredient>()
                .HasMany(e => e.OrderDetailIgredientVatAnals)
                .WithOptional(e => e.OrderDetailIgredient)
                .HasForeignKey(e => e.OrderDetailIgredientsId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrderDetailIgredientVatAnal>()
                .Property(e => e.Gross)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetailIgredientVatAnal>()
                .Property(e => e.Net)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetailIgredientVatAnal>()
                .Property(e => e.VatRate)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetailIgredientVatAnal>()
                .Property(e => e.VatAmount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetailIgredientVatAnal>()
                .Property(e => e.TaxAmount)
                .HasPrecision(9, 4);

            modelBuilder.Entity<OrderDetailInvoice>()
                .Property(e => e.CustomerId)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetailInvoice>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderDetailInvoice>()
                .Property(e => e.Discount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderDetailInvoice>()
                .Property(e => e.TaxAmount)
                .HasPrecision(19, 2);

            modelBuilder.Entity<OrderDetailInvoice>()
                .Property(e => e.Total)
                .HasPrecision(19, 2);

            modelBuilder.Entity<OrderDetailVatAnal>()
                .Property(e => e.Gross)
                .HasPrecision(8, 4);

            modelBuilder.Entity<OrderDetailVatAnal>()
                .Property(e => e.Net)
                .HasPrecision(8, 4);

            modelBuilder.Entity<OrderDetailVatAnal>()
                .Property(e => e.VatRate)
                .HasPrecision(8, 4);

            modelBuilder.Entity<OrderDetailVatAnal>()
                .Property(e => e.VatAmount)
                .HasPrecision(8, 4);

            modelBuilder.Entity<OrderDetailVatAnal>()
                .Property(e => e.TaxAmount)
                .HasPrecision(8, 4);

            modelBuilder.Entity<PageButton>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PageButton>()
                .HasMany(e => e.PageButtonDetails)
                .WithOptional(e => e.PageButton)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PageButtonDetail>()
                .Property(e => e.AddCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PageButtonDetail>()
                .Property(e => e.RemoveCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Page>()
                .HasMany(e => e.PageButtons)
                .WithOptional(e => e.Page)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PosInfo>()
                .HasMany(e => e.Notifications)
                .WithOptional(e => e.PosInfo)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PosInfo>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.PosInfo)
                .HasForeignKey(e => e.PosId);

            modelBuilder.Entity<PosInfo>()
                .HasMany(e => e.PosInfo_Pricelist_Assoc)
                .WithOptional(e => e.PosInfo)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PosInfo>()
                .HasMany(e => e.PosInfo_StaffPositin_Assoc)
                .WithOptional(e => e.PosInfo)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PosInfo>()
                .HasMany(e => e.PosInfoDetails)
                .WithOptional(e => e.PosInfo)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PosInfo>()
                .HasMany(e => e.PosInfoKdsAssocs)
                .WithOptional(e => e.PosInfo)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PosInfoDetail>()
                .HasMany(e => e.OrderDetailInvoices)
                .WithOptional(e => e.PosInfoDetail)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PosInfoDetail>()
                .HasMany(e => e.PosInfoDetail_Pricelist_Assoc)
                .WithOptional(e => e.PosInfoDetail)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Pricelist>()
                .HasMany(e => e.PosInfo_Pricelist_Assoc)
                .WithOptional(e => e.Pricelist)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Pricelist>()
                .HasMany(e => e.PosInfoDetail_Pricelist_Assoc)
                .WithOptional(e => e.Pricelist)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Pricelist>()
                .HasMany(e => e.PricelistDetails)
                .WithOptional(e => e.Pricelist)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Pricelist>()
                .HasOptional(e => e.RegionLockerProduct)
                .WithRequired(e => e.Pricelist);

            modelBuilder.Entity<PricelistDetail>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PricelistDetail>()
                .Property(e => e.PriceWithout)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PricelistMaster>()
                .HasMany(e => e.Pricelists)
                .WithOptional(e => e.PricelistMaster)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PricelistMaster>()
                .HasMany(e => e.SalesType_PricelistMaster_Assoc)
                .WithOptional(e => e.PricelistMaster)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductBarcodes)
                .WithOptional(e => e.Product)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductForBarcodeEods)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasOptional(e => e.ProductsForEOD)
                .WithRequired(e => e.Product);

            modelBuilder.Entity<ProductExtra>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductRecipe>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductSalesHistoryPerDay>()
                .Property(e => e.Qty)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductSalesHistoryPerDay>()
                .Property(e => e.Total)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductSalesHistoryPerDay>()
                .Property(e => e.VatAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductSalesHistoryPerDay>()
                .Property(e => e.Net)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductSalesHistoryPerDay>()
                .Property(e => e.VatRate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductSalesHistoryPerDay>()
                .Property(e => e.UnitPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Region>()
                .HasMany(e => e.Tables)
                .WithOptional(e => e.Region)
                .WillCascadeOnDelete();

            modelBuilder.Entity<RegionLockerProduct>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<RegionLockerProduct>()
                .Property(e => e.Discount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesType>()
                .HasMany(e => e.SalesType_PricelistMaster_Assoc)
                .WithOptional(e => e.SalesType)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Staff>()
                .HasMany(e => e.OrderDetailInvoices)
                .WithOptional(e => e.Staff)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Staff>()
                .HasMany(e => e.OrdersStaffs)
                .WithOptional(e => e.Staff)
                .WillCascadeOnDelete();

            modelBuilder.Entity<StaffPosition>()
                .HasMany(e => e.PosInfo_StaffPositin_Assoc)
                .WithOptional(e => e.StaffPosition)
                .WillCascadeOnDelete();

            modelBuilder.Entity<StaffSchedule>()
                .HasMany(e => e.StaffSecheduleDetails)
                .WithOptional(e => e.StaffSchedule)
                .HasForeignKey(e => e.StaffSceduleId);

            modelBuilder.Entity<Store>()
                .HasMany(e => e.HotelInfoes)
                .WithOptional(e => e.Store)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Store>()
                .HasMany(e => e.Notifications)
                .WithOptional(e => e.Store)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Store>()
                .HasMany(e => e.StoreMessages)
                .WithOptional(e => e.Store)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Table>()
                .HasMany(e => e.OrderDetails)
                .WithOptional(e => e.Table)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TablePaySuggestion>()
                .Property(e => e.Amount)
                .HasPrecision(9, 2);

            modelBuilder.Entity<Tax>()
                .Property(e => e.Percentage)
                .HasPrecision(8, 4);

            modelBuilder.Entity<Tax>()
                .HasMany(e => e.OrderDetailIgredientVatAnals)
                .WithOptional(e => e.Tax)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Tax>()
                .HasMany(e => e.OrderDetailVatAnals)
                .WithOptional(e => e.Tax)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Gross)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Net)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Vat)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Tax)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Transaction>()
                .HasMany(e => e.TransferToPms)
                .WithOptional(e => e.Transaction)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TransferMapping>()
                .HasMany(e => e.TransferMappingDetails)
                .WithOptional(e => e.TransferMapping)
                .HasForeignKey(e => e.TransferMappingsId);

            modelBuilder.Entity<TransferToPm>()
                .Property(e => e.Total)
                .HasPrecision(9, 4);

            modelBuilder.Entity<Vat>()
                .Property(e => e.Percentage)
                .HasPrecision(8, 4);
        }
    }
}
