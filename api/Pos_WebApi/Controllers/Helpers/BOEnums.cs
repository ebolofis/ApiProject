namespace Pos_WebApi.Helpers {
    public enum EntitiesForLookUpFactoryEnum {
        SetupProductCategories = 0,
        PricelistDetail = 1,
        SetupInvoiceTypes = 2,
        SetupAccounts = 3,
        SetupTransactionTypes = 4,
        SetupDiscount = 5,
        SetupPredefinedCredits = 6,
        SetupKitchenInstruction = 7,
        SetupAccountMapping = 8,
        SetupAuthorizedGroupDetail = 9,
        SetupExcludedAccountSettings = 10,
        SetupPosInfoDetailPricelistAssoc = 11,
        SetupPdaModules = 12,
        SetupItems = 13,
        SetupClientPos = 14,
        SetupPricelist = 15,
        SetupPricelistMaster = 16,
        SetupPosInfo = 17,
        SetupPosInfoDetail = 18,
        SetupTransferMappings = 19,
        SetupIngredients = 20,
        CustomerPolicyEnum = 21,

        RegionLockerProduct = 22,
        ManageExternalProductsManager = 23,
        Payrole = 24,
        ProductsManage = 25,
        ManageReceipts = 26,
        ManagePages = 27,

        NFCConfiguration = 28,
        SetupCombo = 29,
        SetupComboDetail = 30,
        SetupProductCat = 36,
        SetupStores=37,
        SetupProductandProductCategories=38,
        SetupPromoDiscountType=39,
        SetupMainMessages=40,

        //a lookup to load BI policy over project
        BusinessIntelligence = 100

    }
    public enum AccountType {
        Cash = 1,
        Coplimentary = 2,
        Charge = 3,
        CreditCard = 4,
        Barcode = 5,
        Voucher = 6,
        Allowence = 9,
        /// <summary>
        /// Type like Credit Card but prepaid....
        /// </summary>
        TicketCompliment = 10
    }
    public enum DiscountTypeEnum {
        Percentage = 0,
        Quantitative = 1,
        OpenPercentage = 2,
        OpenQuantitative = 3,
        FinalPrice = 4
    }

    /// <summary>
    /// Type of POS (see PosInfo.Type)
    /// </summary>
    public enum ModuleType {
        Pos = 1,
        Kds = 2,
        Cashier = 3,
        Receipt = 4
    }

    //public enum TransactionType {
    //    OpenCashier = 1,
    //    CloseCashier = 2,
    //    Sale = 3,
    //    Cancel = 4,
    //    Other = 5,
    //    Antilogismos = 6,
    //    CreditCode = 7,
    //    OpenLocker = 8,
    //    CloseLocker = 9,
    //    Tips = 10
    //}

    public enum StaffWorksheetStatus {
        CheckedIn = 1,
        CheckedOut
    }

    public enum InvoiceTypesEnum {
        Receipt = 1,
        Order = 2,
        Void = 3,
        Coplimentary = 4,
        Allowance = 5,
        Timologio = 7,
        VoidOrder = 8,
        PaymentReceipt = 11,
        RefundReceipt = 12,
        Pistosi=13,
        PreBill = 14
    }

    public enum OrderDetailUpdateType {
        PayOff = 1,
        UnPaidCancel = 2,
        PaidCancel = 3,
        Kds = 4,
        InvoiceOnly = 5
    }

    public enum CreditTransactionType {
        RemoveCredit = 0,
        AddCredit = 1,
        ReturnAllCredits = 2,
        ReturnCredit = 3,
        PayLocker = 4,
        ReturnLocker = 5,
        None = 6
    }
    public enum PricelistTypesEnum {
        Normal = 1,
        Coplimentary = 3,
        HappyHour = 9,
        Delivery = 5,
        Allowance = 4
    }
    public enum MetadataFieldTypeEnum {
        String = 1,
        Currency = 2,
        Date = 3,
        Time = 4,
        Bool = 5,
        Integer = 6,
        DateTime = 7
    }

    public enum CustomerPolicyEnum { NoCustomers = 0, HotelInfo = 1, Other = 2, Delivery = 3, PmsInterface = 4 }
    public enum HotelInfoCustomerPolicyEnum { NoCustomers = 1, HotelInfo = 0, Other = 2, Delivery = 3, PmsInterface = 4 }

    public enum OrderStatusEnum {
        Received = 0,
        Pending = 1,
        Prepared = 2,
        Ready = 3,
        Onroad = 4,
        Canceled = 5,
        Complete = 6,
        Returned = 7
        //Received = 0,
        //Preparing,
        //Ready,
        //Printed,
        //Assigned,
        //Delivered,
        ////Canceled,
        //NotDelivered,
        //Waitingforprint,
        //Waitingassignment
    }

    public enum DeliveryStatusEnum {
        Received = 0,
        Preparing = 1,
        Ready = 2,
        Printed = 3,
        Assigned = 4,
        Completed = 5,
        Canceled = 6,
        Invoiced = 7,
        Waitingforprint = 8,
        Waitingassignment = 9,
        Delivered = 10,
        Returned = 12
    }

    /// <summary>
    /// Ενέργειες που πρέπει να γίνουν σε μία απόδειξη (νεα απόδειξη, αλλαγή σε απόδειξη κτλ )  
    /// </summary>
    public enum ModifyOrderDetailsEnum {
        /// <summary>
        /// νέα απόδειξη
        /// </summary>
        FromScratch = 0,
        /// <summary>
        /// Σε αρχικό ΔΠ  θέλουμε να εκδόσουμε απόδειξη (με εξόφληση ή χωρίς εξόφληση) χωρίς αλλαγή σε αυτό
        /// </summary>
        FromOtherUnmodified = 1,
        /// <summary>
        /// Σε αρχικό ΔΠ θέλουμε να εκδόσουμε απόδειξη (με εξόφληση ή χωρίς εξόφληση) αλλά με αλλαγή σε αυτό (πχ εισαγωγή έκπτωση)
        /// </summary>
        FromOtherUpated = 2,
        /// <summary>
        /// Σε αρχική απόδειξη χωρίς εξόφληση θέλουμε να την εξοφλήσουμε
        /// </summary>
        PayOffOnly = 3,
        /// <summary>
        /// Αλλαγή τρόπου πληρωμής
        /// </summary>
        ChangePaymentType = 4
    }

    public enum MessageStatusEnum {
        Inactive = 0,
        Active = 1
    }

    public enum FiscalType {
        Generic = 1,
        OPOS = 2
    }

    public enum LockerActionEnum {
        Open,
        CloseNoDiscount,
        CloseWithDiscount,
        ReturnAmount
    }

    public enum LoginToOrderModeEnum {
        None = 0,
        WithPassword = 1,
        WithoutPassword = 2
    }

    public enum KeyboardTypeEnum {
        Numeric = 0,
        Full = 1,
        Card =2, 
        NumericCard = 3,
        FullCard = 4,
        None =5
    }


    public enum PosInfoDetailTemplateTransactionTypeEnum {
        SingleButton = 0,
        SeparateButtons = 1

    }



    public enum OnlineRegistrationStatus {
        Active = 1,
        Finished = 2
    }

    public enum OnlineRegistrationPayments {
        Paypal = 0,
        Card = 1,
        PayOnDelivery = 2
    }

    public enum CreditTransactionEnum {
        None = 0,
        Add = 1,
        Remove = 2,
        Return = 3
    }

    public enum ExternalProductMappingEnum {
        OnlineRegistration = 0,
        Locker,
        ProductForEod,
        Dump
    }
    public enum PayroleEnum {
        Start = 0,
        End = 1,
    }

    /// <summary>
    /// Desribe what part of the receipt EXTECR will print.
    /// </summary>
    public enum PrintType
    {
        /// <summary>
        /// When a receipt signaled from webapi, Print the whole receipt at once.
        /// </summary>
        PrintWhole = 0,
        /// <summary>
        /// When a receipt signaled from webapi, Print only the last item.
        /// </summary>
        PrintItem = 1,
        /// <summary>
        /// Print the receipt's footer only
        /// </summary>
        PrintEnd = 2,
        /// <summary>
        /// Cancel the current receipt
        /// </summary>
        CancelCurrentReceipt = 3,
        /// <summary>
        /// Print only the last extra of the last item.
        /// </summary>
        PrintExtra = 4,
        /// <summary>
        /// Print the discount of the last item.
        /// </summary>
        PrintItemDiscount = 5
    }

    /// <summary>
    /// Type of possible clients.
    /// </summary>
    public enum Clients {
        BO = 0,
        POS = 1,
        PDA = 2,
        EXTECR = 3,
        DA = 4,
        KDS = 5
    }

    /// <summary>
    /// Type of Pagebuttons and  their functionality clients.
    /// </summary>
    public enum PageButtonEnum {

        Empty = 0,
        Product = 1,
        SalesTypes = 2,
        Navigate = 5,
        NavWithPricelist = 6,
        Pricelist = 7,
        OpenItem = 8,
        KitchenInstruction = 9,
        Weighted = 10,
        WeightedOpenItem = 11,

    }
    /// <summary>
    /// This is an Enum characterizing Product Extras Type on PageButtons and other ProductDetails Flat Calls
    /// </summary>
    public enum ProductDetailTypeEnum {
        Recipe =0 ,
        Extras = 1 ,
        ExtrasAssoc = 2,
    }
}
