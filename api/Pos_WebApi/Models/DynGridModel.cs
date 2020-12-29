using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Models
{
    public class DynGridModel
    {
        public Type Object { get; set; }
        public string EntityName { get; set; }
        public int Id { get; set; }
        public string dynObjKeyRef { get; set; }
        public string Desciption { get; set; }
        public string GridType { get; set; }
        public string ModalTitle { get; set; }
        public string Columns { get; set; }
        public string ColumnDependencies { get; set; } //dropdowns, filters,loadedArrays needed4UserTransactions
        public string GridParams { get; set; } //JSON obj
        public string RowSchema { get; set; }
        public string RowEntity { get; set; }
        public string RowForm { get; set; } //arr of strings 
    }
    public static class GetAllDynGridModels
    {
        public static IEnumerable<DynGridModel> GetAll()
        {
            List<DynGridModel> dynGridModelsList = new List<DynGridModel>();
            //Setup Settings/Invoices 
            DynGridModel obj = new DynGridModel
            {
                EntityName = "InvoiceTypes",
                Id = 1,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Invoice Types",
                Desciption = "This is invoicetypes Description to give info about our grid of type invoicetypes.",
                GridType = "single",
                Columns = "[{ field: \"Id\", name: \"Id\", enableCellEdit: false },{ field: \"Code\", name: \"Code\", enableCellEditOnFocus: false, notEmpty: { message: \"The Code is required and cannot be empty\" },stringLength: { min: 1, max: 10, message: \"Min value 1 Max value 10\" },},{ field: \"Abbreviation\", name: \"Abbreviation\", enableCellEditOnFocus: false, enableCellEdit: true, validators: { notNull: true, minLength: { threshold: 1 }, maxLength: { threshold: 3 } }},{ field: \"Description\", name: \"Description\", enableCellEditOnFocus: false },{ field: \"Type\", name: \"Type\", enableCellEditOnFocus: false, editType: \"dropdown\", editableCellTemplate: \"ui-grid/dropdownEditor\",editDropdownIdLabel: \"id\",editDropdownValueLabel: \"gender\",editDropdownOptionsArray: TypeArray, cellFilter: \"mapDropdown: row.grid.appScope.getArrayType(\'invoiceTypeArray\'):\"id\":\"gender\"\"},{ field: \"IsDeleted\", name: \"IsDeleted\", type: Boolean, cellFilter: \"boolValEnumeration\" },{ field: \"Invoices\", name: \"Invoices\", visible: false },{ field: \"PosInfoDetail\", name: \"PosInfoDetail\", visible: false, },{ field: \"PredefinedCredits\", name: \"PredefinedCredits\", \"visible\": false },{ field: \"IsEdited\", name: \"IsEdited\", enableCellEdit: false, cellFilter: \"stateEvaluator\" }]",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupInvoiceTypes\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"Abbreviation\":{\"type\":\"string\",\"title\":\"Abbreviation\",\"maxLength\":3,\"validationMessage\":\"Max length of input must be 3..\",\"description\":\"Email will be used for evil.\",\"validators\":{\"notEmpty\":{\"message\":\"The cell phone number is required\"}}},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Type\":{\"type\":\"number\",\"title\":\"Type\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}},\"required\":[\"Code\"]}",
                RowEntity = "{\"Code\":\"\", \"Abbreviation\": \"\", \"Description\": \"\", \"Type\": \"\", \"IsDeleted\": false }",
                RowForm = "[\"Code\",\"Abbreviation\",\"Description\",{ \"key\": \"Type\", \"type\": \"strapselect\", \"htmlClass\": \"col - lg - 3 col - md - 3\", \"options\": {\"asyncCallback\": \"vm.getarr(\"Type\")\", \"map\" : {\"valueProperty\": \"Key\", \"nameProperty\": \"Value\"} }},,\"IsDeleted\"]"
            };
            DynGridModel obj2 = new DynGridModel
            {
                EntityName = "Vat",
                Id = 2,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Vat Settings",
                Desciption = "This is Vat types Description",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Percentage\":{\"type\":\"number\",\"title\":\"Percentage\"},\"Code\":{\"type\":\"integer\",\"title\":\"Code\"}}}",
                RowEntity = "{\"Description\": \"\", \"Percentage\": \"\",\"Code\":\"\"}",
                RowForm = "[\"Description\",\"Percentage\",\"Code\"]"
            };
            DynGridModel obj3 = new DynGridModel
            {
                EntityName = "Tax",
                Id = 3,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Tax Settings",
                Desciption = "This is Tax types Description",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Percentage\":{\"type\":\"number\",\"title\":\"Percentage\"}}}",
                RowEntity = "{\"Description\": \"\", \"Percentage\": \"\"}",
                RowForm = "[\"Description\",\"Percentage\"]"
            };
            DynGridModel obj25 = new DynGridModel
            {
                EntityName = "Pricelist",
                Id = 25,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Pricelist Settings",
                Desciption = "This is Pricelist Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupPricelist\",\"columnName\":\"PricelistMasterId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"PosInfoId\":{\"type\":\"number\",\"title\":\"PosInfoId\"},\"AccountId\":{\"type\":\"number\",\"title\":\"AccountId\"},\"PmsRoom\":{\"type\":\"number\",\"title\":\"PmsRoom\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"}}}",
                RowEntity = "{\"Code\": \"\",\"Description\": \"\",\"LookUpPriceListId\": \"\",\"Percentage\": \"\",\"Status\": 1,\"ActivationDate\": \"\",\"DeactivationDate\": \"\",\"SalesTypeId\": \"\",\"Type\": \"\"}",
                //Id Code Description  LookUpPriceListId  Percentage  Status  ActivationDate  DeactivationDate SalesTypeId PricelistMasterId IsDeleted Type
                RowForm = "[\"PosInfoDetailId\",\"PricelistId\"]"
            };
            DynGridModel obj26 = new DynGridModel
            {
                EntityName = "PricelistMaster",
                Id = 26,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Pricelist  Group Settings",
                Desciption = "This is Pricelist  Group Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupPricelistMaster\",\"columnName\":\"PricelistId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"PosInfoId\":{\"type\":\"number\",\"title\":\"PosInfoId\"},\"AccountId\":{\"type\":\"number\",\"title\":\"AccountId\"},\"PmsRoom\":{\"type\":\"number\",\"title\":\"PmsRoom\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"}}}",
                RowEntity = "{\"Description\": \"\",\"Status\": \"\",\"Active\": \"\",\"Pricelist\": \"\",\"SalesType_PricelistMaster_Assoc\": \"\"}",
                //Description Status Active  Pricelist  SalesType_PricelistMaster_Assoc 
                RowForm = "[\"PosInfoDetailId\",\"PricelistId\"]"
            };

            DynGridModel obj21 = new DynGridModel
            {
                EntityName = "PosInfoDetail_Pricelist_Assoc",
                Id = 21,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Excluded Pricelists Settings",
                Desciption = "This is Excluded Pricelists Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupPosInfoDetailPricelistAssoc\",\"columnName\":\"PosInfoDetailId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"PosInfoId\":{\"type\":\"number\",\"title\":\"PosInfoId\"},\"AccountId\":{\"type\":\"number\",\"title\":\"AccountId\"},\"PmsRoom\":{\"type\":\"number\",\"title\":\"PmsRoom\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"}}}",
                RowEntity = "{\"PosInfoDetailId\": \"\",\"PricelistId\": \"\"}",
                RowForm = "[\"PosInfoDetailId\",\"PricelistId\"]"
            };

            //Setup SEttings / Staff
            DynGridModel obj4 = new DynGridModel
            {
                EntityName = "StaffPosition",
                Id = 4,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Staff Positions",
                Desciption = "This is Staff Positions Description",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"}}}",
                RowEntity = "{\"Description\": \"\"}",
                RowForm = "[\"Description\"]"
            };
            DynGridModel obj5 = new DynGridModel
            {
                EntityName = "AuthorizedGroup",
                Id = 5,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Authorized Groups",
                Desciption = "This is Authorized Groups Description",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"ExtendedDescription\":{\"type\":\"string\",\"title\":\"ExtendedDescription\"}}}",
                RowEntity = "{\"Description\": \"\",\"ExtendedDescription\": \"\"}",
                RowForm = "[\"Description\",\"ExtendedDescription\"]"
            };
            DynGridModel obj19 = new DynGridModel
            {
                EntityName = "AuthorizedGroupDetail",
                Id = 19,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Authorized Group Details",
                Desciption = "This is Authorized Group Details Description",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupAuthorizedGroupDetail\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "",
                RowEntity = "{\"AuthorizedGroupId\": 0,\"ActionId\": 0}",
                RowForm = ""
            };

            //Setup Settings / Transactions
            DynGridModel obj6 = new DynGridModel
            {
                EntityName = "Accounts",
                Id = 6,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Accounts Settings",
                Desciption = "This is Accounts Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupAccounts\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Type\":{\"type\":\"number\",\"title\":\"Type\"},\"IsDefault\":{\"type\":\"boolean\",\"title\":\"IsDefault\"},\"SendsTransfer\":{\"type\":\"boolean\",\"title\":\"SendsTransfer\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}, \"PMSPaymentId\":{\"type\":\"number\",\"title\":\"PMSPaymentId\"}}}",
                RowEntity = "{\"Description\": \"\",\"Type\": \"\", \"IsDefault\": false ,\"SendsTransfer\": false, \"IsDeleted\": false, \"PMSPaymentId\":\"\" }",
                RowForm = "[\"Description\",\"Type\",\"IsDefault\",\"SendsTransfer\",\"IsDeleted\",\"PMSPaymentId\"]"
            };
            DynGridModel obj20 = new DynGridModel
            {
                EntityName = "PosInfoDetail_Excluded_Accounts",
                Id = 20,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Excluded Accounts Settings",
                Desciption = "This is Excluded Accounts Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupExcludedAccountSettings\",\"columnName\":\"PosInfoDetailId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"PosInfoId\":{\"type\":\"number\",\"title\":\"PosInfoId\"},\"AccountId\":{\"type\":\"number\",\"title\":\"AccountId\"},\"PmsRoom\":{\"type\":\"number\",\"title\":\"PmsRoom\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"}}}",
                RowEntity = "{\"PosInfoDetailId\": \"\",\"AccountId\": \"\"}",
                RowForm = "[\"PosInfoDetailId\",\"AccountId\"]"
            };
            DynGridModel obj7 = new DynGridModel
            {
                EntityName = "AccountMapping",
                Id = 7,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Account Mapping Settings",
                Desciption = "This is Account Mapping Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupAccountMapping\",\"columnName\":\"AccountId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"PosInfoId\":{\"type\":\"number\",\"title\":\"PosInfoId\"},\"AccountId\":{\"type\":\"number\",\"title\":\"AccountId\"},\"PmsRoom\":{\"type\":\"number\",\"title\":\"PmsRoom\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"}}}",
                RowEntity = "{\"PosInfoId\": \"\",\"AccountId\": \"\", \"PmsRoom\": \"\" ,\"Description\": \"\", \"Status\": false}",
                RowForm = "[\"PosInfoId\",\"AccountId\",\"PmsRoom\",\"Description\",\"Status\"]"
            };

            DynGridModel obj9 = new DynGridModel
            {
                EntityName = "TransactionTypes",
                Id = 9,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your TransactionTypes",
                Desciption = "This is TransactionTypes Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupTransactionTypes\",\"columnName\":\"Code\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"IsIncome\":{\"type\":\"boolean\",\"title\":\"IsIncome\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}}}",
                RowEntity = "{\"Code\":\"\",\"Description\": \"\", \"IsIncome\": false, \"IsDeleted\": false }",
                RowForm = "[\"Code\",\"Description\",\"IsIncome\",\"IsDeleted\"]"
            };
            DynGridModel obj8 = new DynGridModel
            {
                EntityName = "Discount",
                Id = 8,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Discounts",
                Desciption = "This is Discount Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupDiscount\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Type\":{\"type\":\"string\",\"title\":\"Type\"},\"Amount\":{\"type\":\"number\",\"title\":\"Amount\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"},\"Sort\":{\"type\":\"number\",\"title\":\"Sort\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"}}}",
                RowEntity = "{\"Type\":\"\",\"Amount\": \"\", \"Status\": false, \"Sort\": \"\", \"Description\": \"\" }",
                RowForm = "[\"Type\",\"Amount\",\"Status\",\"Sort\",\"Description\"]"
            };
            DynGridModel obj10 = new DynGridModel
            {
                EntityName = "PredefinedCredits",
                Id = 10,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Vouchers - Credits",
                Desciption = "This is Vouchers - Credits Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\", \"enumIdentifier\":\"SetupPredefinedCredits\",\"columnName\":\"InvoiceTypeId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Amount\":{\"type\":\"number\",\"title\":\"Amount\"},\"InvoiceTypeId\":{\"type\":\"number\",\"title\":\"InvoiceTypeId\"}}}",
                RowEntity = "{\"Description\": \"\", \"Amount\": \"\", \"InvoiceTypeId\": \"\" }",
                RowForm = "[\"Description\",\"Amount\",\"InvoiceTypeId\"]"
            };
            // Id Code Description Status IsDeleted
            //SetupSettigs/ Sales
            DynGridModel obj22 = new DynGridModel
            {
                EntityName = "PdaModule",
                Id = 22,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your PDA Modules Options",
                Desciption = "This is PDA Modules Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupPdaModules\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}}}",
                RowEntity = "{\"PosInfoId\": \"\",\"Code\": \"\",\"Description\": \"\",\"PageSetId\": \"\",\"MaxActiveUsers\": \"\", \"Status\": false }",
                RowForm = "[\"PosInfoId\",\"Code\",\"Description\",\"PageSetId\",\"MaxActiveUsers\",\"Status\"]"
            };
            //Id, , Code, Description, PageSetId, MaxActiveUsers, Status
            DynGridModel obj11 = new DynGridModel
            {
                EntityName = "Kitchen",
                Id = 11,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Kitchen Options",
                Desciption = "This is Kitchen Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}}}",
                RowEntity = "{\"Code\": \"\",\"Description\": \"\", \"Status\": \"\", \"IsDeleted\": false }",
                RowForm = "[\"Code\",\"Description\",\"Status\",\"IsDeleted\"]"
            };
            DynGridModel obj12 = new DynGridModel
            {
                EntityName = "KitchenRegion",
                Id = 12,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Kitchen Region Options",
                Desciption = "This is Kitchen Region Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"ItemRegion\":{\"type\":\"string\",\"title\":\"ItemRegion\"},\"RegionPosition\":{\"type\":\"number\",\"title\":\"RegionPosition\"},\"Abbr\":{\"type\":\"string\",\"title\":\"Abbr\"}}}",
                RowEntity = "{\"ItemRegion\": \"\",\"RegionPosition\": \"\", \"Abbr\": \"\" }",
                RowForm = "[\"ItemRegion\",\"RegionPosition\",\"Abbr\",]"
            };
            DynGridModel obj13 = new DynGridModel
            {
                EntityName = "KitchenInstruction",
                Id = 13,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Kitchen Instruction Options",
                Desciption = "This is KitchenInstruction Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\",\"enumIdentifier\":\"SetupKitchenInstruction\" ,\"columnName\":\"KitchenId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"KitchenId\":{\"type\":\"number\",\"title\":\"KitchenId\"},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Message\":{\"type\":\"string\",\"title\":\"Message\"}}}",
                RowEntity = "{\"KitchenId\": \"\", \"Message\": \"\",\"Description\": \"\"}",
                RowForm = "[\"KitchenId\",\"Message\",\"Description\"]"
            };

            DynGridModel obj14 = new DynGridModel
            {
                EntityName = "Kds",
                Id = 14,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Kds Options",
                Desciption = "This is Kds Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"},\"PosInfoId\":{\"type\":\"string\",\"title\":\"PosInfoId\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}}}",
                RowEntity = "{\"Description\": \"\", \"Status\": \"\",\"PosInfoId\": \"\", \"IsDeleted\": false }",
                RowForm = "[\"Description\",\"Status\",\"PosInfoId\",\"IsDeleted\"]"
            };
            DynGridModel obj15 = new DynGridModel
            {
                EntityName = "SalesType",
                Id = 15,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your SalesType Options",
                Desciption = "This is SalesType Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Abbreviation\":{\"type\":\"string\",\"title\":\"Status\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}}}",
                RowEntity = "{\"Description\": \"\", \"Abbreviation\": \"\", \"IsDeleted\": false }",
                RowForm = "[\"Description\",\"Abbreviation\",\"IsDeleted\"]"
            };
            DynGridModel obj16 = new DynGridModel
            {
                EntityName = "Units",
                Id = 16,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Units Options",
                Desciption = "This is Units Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Abbreviation\":{\"type\":\"string\",\"title\":\"Status\"},\"Unit\":{\"type\":\"number\",\"title\":\"Unit\"}}}",
                RowEntity = "{\"Description\": \"\", \"Abbreviation\": \"\", \"Unit\": false }",
                RowForm = "[\"Description\",\"Abbreviation\",\"Unit\"]"
            };
            DynGridModel obj17 = new DynGridModel
            {
                EntityName = "ProductCategories",
                Id = 17,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your ProductCategories Options",
                Desciption = "This is ProductCategories Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\", \"enumIdentifier\":\"SetupProductCategories\",\"columnName\":\"CategoryId\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Type\":{\"type\":\"boolean\",\"title\":\"Type\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"},\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"CategoryId\":{\"type\":\"number\",\"title\":\"CategoryId\"}}}",
                RowEntity = "{\"Description\": \"\", \"Type\": false, \"Status\": true,\"Code\": \"\",\"CategoryId\": \"\" }",
                RowForm = "[\"Description\",\"Type\",\"Status\",\"Code\",\"CategoryId\"]"
                //Id Description Type Status Code  CategoryId
            };
            DynGridModel obj18 = new DynGridModel
            {
                EntityName = "Categories",
                Id = 18,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Categories Options",
                Desciption = "This is Categories Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Status\":{\"type\":\"number\",\"title\":\"Status\"}}}",
                RowEntity = "{\"Description\": \"\", \"Status\": false }",
                RowForm = "[\"Description\",\"Status\"]"
            };
            DynGridModel obj23 = new DynGridModel
            {
                EntityName = "Items",
                Id = 23,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Items",
                Desciption = "This is Items Description to give info about our grid of type items.",
                GridType = "single",
                Columns = "[{ field: \"Id\", name: \"Id\", enableCellEdit: false },{ field: \"Code\", name: \"Code\", enableCellEditOnFocus: false, notEmpty: { message: \"The Code is required and cannot be empty\" },stringLength: { min: 1, max: 10, message: \"Min value 1 Max value 10\" },},{ field: \"Abbreviation\", name: \"Abbreviation\", enableCellEditOnFocus: false, enableCellEdit: true, validators: { notNull: true, minLength: { threshold: 1 }, maxLength: { threshold: 3 } }},{ field: \"Description\", name: \"Description\", enableCellEditOnFocus: false },{ field: \"Type\", name: \"Type\", enableCellEditOnFocus: false, editType: \"dropdown\", editableCellTemplate: \"ui-grid/dropdownEditor\",editDropdownIdLabel: \"id\",editDropdownValueLabel: \"gender\",editDropdownOptionsArray: TypeArray, cellFilter: \"mapDropdown: row.grid.appScope.getArrayType(\'invoiceTypeArray\'):\"id\":\"gender\"\"},{ field: \"IsDeleted\", name: \"IsDeleted\", type: Boolean, cellFilter: \"boolValEnumeration\" },{ field: \"Invoices\", name: \"Invoices\", visible: false },{ field: \"PosInfoDetail\", name: \"PosInfoDetail\", visible: false, },{ field: \"PredefinedCredits\", name: \"PredefinedCredits\", \"visible\": false },{ field: \"IsEdited\", name: \"IsEdited\", enableCellEdit: false, cellFilter: \"stateEvaluator\" }]",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupItems\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"Abbreviation\":{\"type\":\"string\",\"title\":\"Abbreviation\",\"maxLength\":3,\"validationMessage\":\"Max length of input must be 3..\",\"description\":\"Email will be used for evil.\",\"validators\":{\"notEmpty\":{\"message\":\"The cell phone number is required\"}}},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Type\":{\"type\":\"number\",\"title\":\"Type\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}},\"required\":[\"Code\"]}",
                RowEntity = "{\"Description\":\"\", \"ExtendedDescription\": \"\", \"Qty\": \"\", \"UnitId\": \"\", \"VatId\": false }",
                RowForm = "[\"Description\",\"ExtendedDescription\",\"Qty\",\"UnitId\",\"VatId\"]"
            };
            DynGridModel obj24 = new DynGridModel
            {
                EntityName = "ClientPos",
                Id = 24,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Client Pos",
                Desciption = "This is ClientPos Description to give info about our grid of type ClientPos.",
                GridType = "single",
                Columns = "[{ field: \"Id\", name: \"Id\", enableCellEdit: false },{ field: \"Code\", name: \"Code\", enableCellEditOnFocus: false, notEmpty: { message: \"The Code is required and cannot be empty\" },stringLength: { min: 1, max: 10, message: \"Min value 1 Max value 10\" },},{ field: \"Abbreviation\", name: \"Abbreviation\", enableCellEditOnFocus: false, enableCellEdit: true, validators: { notNull: true, minLength: { threshold: 1 }, maxLength: { threshold: 3 } }},{ field: \"Description\", name: \"Description\", enableCellEditOnFocus: false },{ field: \"Type\", name: \"Type\", enableCellEditOnFocus: false, editType: \"dropdown\", editableCellTemplate: \"ui-grid/dropdownEditor\",editDropdownIdLabel: \"id\",editDropdownValueLabel: \"gender\",editDropdownOptionsArray: TypeArray, cellFilter: \"mapDropdown: row.grid.appScope.getArrayType(\'invoiceTypeArray\'):\"id\":\"gender\"\"},{ field: \"IsDeleted\", name: \"IsDeleted\", type: Boolean, cellFilter: \"boolValEnumeration\" },{ field: \"Invoices\", name: \"Invoices\", visible: false },{ field: \"PosInfoDetail\", name: \"PosInfoDetail\", visible: false, },{ field: \"PredefinedCredits\", name: \"PredefinedCredits\", \"visible\": false },{ field: \"IsEdited\", name: \"IsEdited\", enableCellEdit: false, cellFilter: \"stateEvaluator\" }]",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupClientPos\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"Abbreviation\":{\"type\":\"string\",\"title\":\"Abbreviation\",\"maxLength\":3,\"validationMessage\":\"Max length of input must be 3..\",\"description\":\"Email will be used for evil.\",\"validators\":{\"notEmpty\":{\"message\":\"The cell phone number is required\"}}},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Type\":{\"type\":\"number\",\"title\":\"Type\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}},\"required\":[\"Code\"]}",
                RowEntity = "{\"Description\":\"\", \"ExtendedDescription\": \"\", \"Qty\": \"\", \"UnitId\": \"\", \"VatId\": false }",
                RowForm = "[\"Description\",\"ExtendedDescription\",\"Qty\",\"UnitId\",\"VatId\"]"
            };
            DynGridModel obj27 = new DynGridModel
            {
                EntityName = "PosInfo",
                Id = 27,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Pos Info",
                Desciption = "This is  Pos Info Description to give info about our grid of type  Pos Info.",
                GridType = "master",
                Columns = "[{ field: \"Id\", name: \"Id\", enableCellEdit: false },{ field: \"Code\", name: \"Code\", enableCellEditOnFocus: false, notEmpty: { message: \"The Code is required and cannot be empty\" },stringLength: { min: 1, max: 10, message: \"Min value 1 Max value 10\" },},{ field: \"Abbreviation\", name: \"Abbreviation\", enableCellEditOnFocus: false, enableCellEdit: true, validators: { notNull: true, minLength: { threshold: 1 }, maxLength: { threshold: 3 } }},{ field: \"Description\", name: \"Description\", enableCellEditOnFocus: false },{ field: \"Type\", name: \"Type\", enableCellEditOnFocus: false, editType: \"dropdown\", editableCellTemplate: \"ui-grid/dropdownEditor\",editDropdownIdLabel: \"id\",editDropdownValueLabel: \"gender\",editDropdownOptionsArray: TypeArray, cellFilter: \"mapDropdown: row.grid.appScope.getArrayType(\'invoiceTypeArray\'):\"id\":\"gender\"\"},{ field: \"IsDeleted\", name: \"IsDeleted\", type: Boolean, cellFilter: \"boolValEnumeration\" },{ field: \"Invoices\", name: \"Invoices\", visible: false },{ field: \"PosInfoDetail\", name: \"PosInfoDetail\", visible: false, },{ field: \"PredefinedCredits\", name: \"PredefinedCredits\", \"visible\": false },{ field: \"IsEdited\", name: \"IsEdited\", enableCellEdit: false, cellFilter: \"stateEvaluator\" }]",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupPosInfo\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Code\":{\"type\":\"string\",\"title\":\"Code\"},\"Abbreviation\":{\"type\":\"string\",\"title\":\"Abbreviation\",\"maxLength\":3,\"validationMessage\":\"Max length of input must be 3..\",\"description\":\"Email will be used for evil.\",\"validators\":{\"notEmpty\":{\"message\":\"The cell phone number is required\"}}},\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Type\":{\"type\":\"number\",\"title\":\"Type\"},\"IsDeleted\":{\"type\":\"boolean\",\"title\":\"IsDeleted\"}},\"required\":[\"Code\"]}",
                RowEntity = "{\"Description\":\"\", \"ExtendedDescription\": \"\", \"Qty\": \"\", \"UnitId\": \"\", \"VatId\": false }",
                RowForm = "[\"Description\",\"ExtendedDescription\",\"Qty\",\"UnitId\",\"VatId\"]"
            };
            DynGridModel obj28 = new DynGridModel
            {
                EntityName = "PosInfoDetail",
                Id = 28,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Pos Info Detail",
                Desciption = "This is  Pos Info Detail  Description to give info about our grid of type  Pos Info Detail.",
                GridType = "slave",
                Columns = "",
                //Columns = "[{ field: \"Id\", name: \"Id\", enableCellEdit: false },{ field: \"Code\", name: \"Code\", enableCellEditOnFocus: false, notEmpty: { message: \"The Code is required and cannot be empty\" },stringLength: { min: 1, max: 10, message: \"Min value 1 Max value 10\" },},{ field: \"Abbreviation\", name: \"Abbreviation\", enableCellEditOnFocus: false, enableCellEdit: true, validators: { notNull: true, minLength: { threshold: 1 }, maxLength: { threshold: 3 } }},{ field: \"Description\", name: \"Description\", enableCellEditOnFocus: false },{ field: \"Type\", name: \"Type\", enableCellEditOnFocus: false, editType: \"dropdown\", editableCellTemplate: \"ui-grid/dropdownEditor\",editDropdownIdLabel: \"id\",editDropdownValueLabel: \"gender\",editDropdownOptionsArray: TypeArray, cellFilter: \"mapDropdown: row.grid.appScope.getArrayType(\'invoiceTypeArray\'):\"id\":\"gender\"\"},{ field: \"IsDeleted\", name: \"IsDeleted\", type: Boolean, cellFilter: \"boolValEnumeration\" },{ field: \"Invoices\", name: \"Invoices\", visible: false },{ field: \"PosInfoDetail\", name: \"PosInfoDetail\", visible: false, },{ field: \"PredefinedCredits\", name: \"PredefinedCredits\", \"visible\": false },{ field: \"IsEdited\", name: \"IsEdited\", enableCellEdit: false, cellFilter: \"stateEvaluator\" }]",
                //ColumnDependencies ="",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupPosInfoDetail\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "",
                RowSchema = "",
                RowEntity = "",
                RowForm = ""
            };
            DynGridModel obj29 = new DynGridModel
            {
                EntityName = "Ingredients",
                Id = 29,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Ingredients",
                Desciption = "This is  Ingredients  Description to give info about our grid of type Ingredients.",
                GridType = "single",
                Columns = "",
                //[{ field: \"Id\", name: \"Id\", enableCellEdit: false },{ field: \"Code\", name: \"Code\", enableCellEditOnFocus: false, notEmpty: { message: \"The Code is required and cannot be empty\" },stringLength: { min: 1, max: 10, message: \"Min value 1 Max value 10\" },},{ field: \"Abbreviation\", name: \"Abbreviation\", enableCellEditOnFocus: false, enableCellEdit: true, validators: { notNull: true, minLength: { threshold: 1 }, maxLength: { threshold: 3 } }},{ field: \"Description\", name: \"Description\", enableCellEditOnFocus: false },{ field: \"Type\", name: \"Type\", enableCellEditOnFocus: false, editType: \"dropdown\", editableCellTemplate: \"ui-grid/dropdownEditor\",editDropdownIdLabel: \"id\",editDropdownValueLabel: \"gender\",editDropdownOptionsArray: TypeArray, cellFilter: \"mapDropdown: row.grid.appScope.getArrayType(\'invoiceTypeArray\'):\"id\":\"gender\"\"},{ field: \"IsDeleted\", name: \"IsDeleted\", type: Boolean, cellFilter: \"boolValEnumeration\" },{ field: \"Invoices\", name: \"Invoices\", visible: false },{ field: \"PosInfoDetail\", name: \"PosInfoDetail\", visible: false, },{ field: \"PredefinedCredits\", name: \"PredefinedCredits\", \"visible\": false },{ field: \"IsEdited\", name: \"IsEdited\", enableCellEdit: false, cellFilter: \"stateEvaluator\" }]
                //ColumnDependencies ="",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupIngredients\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "",
                RowSchema = "",
                RowEntity = "{\"Description\":\"\", \"ExtendedDescription\":\"\", \"SalesDescription\":\"\", \"Qty\":\"\", \"ItemId\":\"\", \"UnitId\":\"\", \"Code\":\"\", \"IsDeleted\":false, \"Background\":\"\", \"Color\":\"\", \"IngredientCategoryId\":\"\"}",
                RowForm = ""
            };
            DynGridModel obj30 = new DynGridModel
            {
                EntityName = "Staff",
                Id = 30,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Staff",
                Desciption = "This is Staff Description to give info about our grid of type Staff.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "",
                RowSchema = "",
                RowEntity = "{\"Code\":\"\", \"FirstName\":\"\", \"LastName\":\"\", \"ImageUri\":\"\", \"Password\":\"\"}",
                RowForm = ""
            };
            DynGridModel obj31 = new DynGridModel
            {
                EntityName = "Department",
                Id = 31,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Departments",
                Desciption = "This is Department Description to give info about our grid of type Department.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "",
                RowSchema = "",
                RowEntity = "{ \"Description\":\"\"}",
                RowForm = ""
            };
            DynGridModel obj32 = new DynGridModel
            {
                EntityName = "IngredientCategories",
                Id = 32,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your IngredientCategories Options",
                Desciption = "This is IngredientCategories Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Description\":{\"type\":\"string\",\"title\":\"Description\"},\"Type\":{\"type\":\"boolean\",\"title\":\"Type\"},\"Status\":{\"type\":\"boolean\",\"title\":\"Status\"},\"IsUnique\":{\"type\":\"boolean\",\"title\":\"IsUnique\"},\"Code\":{\"type\":\"string\",\"title\":\"Code\"}}}",
                RowEntity = "{\"Description\": \"\", \"Type\": false, \"Status\": true,  \"IsUnique\": true ,\"Code\": \"\"}",
                RowForm = "[\"Description\",\"Type\",\"Status\",\"IsUnique\",\"Code\"]"
            };
            DynGridModel obj33 = new DynGridModel
            {
                EntityName = "Combo",
                Id = 33,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Combo Options",
                Desciption = "This is Combo Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupCombo\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "",
                RowSchema = "",
                RowEntity = "{\"Description\": \"\", \"StartDate\":  \"\", \"EndDate\": \"\", \"StartTime\":  \"\", \"EndTime\":  \"\",\"ProductComboId\": \"\",\"DepartmentId\": \"\"}",
                RowForm = ""
            };
            DynGridModel obj34 = new DynGridModel
            {
                EntityName = "ComboDetail",
                Id = 34,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Combo Details Options",
                Desciption = "This is Combo Detail Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"SetupComboDetail\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "",
                RowSchema = "",
                RowEntity = "{\"ComboId\": \"\",\"ProductComboItemId\": \"\"}",
                RowForm = ""
            };
            DynGridModel obj35 = new DynGridModel
            {
                EntityName = "Payrole",
                Id = 35,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Payrole Options",
                Desciption = "This is Payrole Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"Payrole\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "[{\"key\":\"rowEditWaitInterval\",\"value\":-1},{\"key\":\"enablePaginationControls\",\"value\":true},{\"key\":\"enableCellEditOnFocus\",\"value\":true},{\"key\":\"enableSorting\",\"value\":true},{\"key\":\"enableRowSelection\",\"value\":true},{\"key\":\"multiSelect\",\"value\":false},{\"key\":\"modifierKeysToMultiSelect\",\"value\":false},{\"key\":\"noUnselect\",\"value\":true}]",
                RowSchema = "{\"type\":\"object\",\"properties\":{\"Identification\":{\"type\":\"string\",\"title\":\"Identification\"},\"ActionDate\":{\"type\":\"string\",\"title\":\"Date\"}}}",
                RowEntity = "{\"StaffId\": \"\", \"PosInfoId\": \"\", \"Type\": \"\", \"ActionDate\": \"\", \"Identification\": \"\", \"ShopId\": \"\" }",
                RowForm = "[\"StaffId\",\"PosInfoId\",\"Type\",\"ActionDate\",\"Identification\",\"ShopId\"]"
            };

            DynGridModel obj36 = new DynGridModel
            {
                EntityName = "SetupProductCat",
                Id = 36,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your ProductCat Options",
                Desciption = "This is ProductCat Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"Payrole\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "",
                RowSchema = "",
                RowEntity = "{\"HeaderId\": \"\", \"ProdCategoryId\": \"\", \"Position\": \"\" }",
                RowForm = ""
            };
            DynGridModel obj37 = new DynGridModel
            {
                EntityName = "SetupStores",
                Id = 37,
                dynObjKeyRef = "Id",
                ModalTitle = "Manage your Stores Options",
                Desciption = "This is Stores Description.",
                GridType = "single",
                Columns = "",
                ColumnDependencies = "{\"type\":\"sort\" ,\"enumIdentifier\":\"Payrole\",\"columnName\":\"Type\", \"dependType\": \"editDropdownOptionsArray\"}",
                GridParams = "",
                RowSchema = "",
                RowEntity = "{\"HeaderId\": \"\", \"ProdCategoryId\": \"\", \"Position\": \"\" }",
                RowForm = ""
            };
            //Id Description Abbreviation Unit
            dynGridModelsList.Add(obj);
            dynGridModelsList.Add(obj2);
            dynGridModelsList.Add(obj3);
            dynGridModelsList.Add(obj4);
            dynGridModelsList.Add(obj5);
            dynGridModelsList.Add(obj6);
            dynGridModelsList.Add(obj7);
            dynGridModelsList.Add(obj8);
            dynGridModelsList.Add(obj9);
            dynGridModelsList.Add(obj10);
            dynGridModelsList.Add(obj11);
            dynGridModelsList.Add(obj12);
            dynGridModelsList.Add(obj13);
            dynGridModelsList.Add(obj14);
            dynGridModelsList.Add(obj15);
            dynGridModelsList.Add(obj16);
            dynGridModelsList.Add(obj17);
            dynGridModelsList.Add(obj18);
            dynGridModelsList.Add(obj19);
            dynGridModelsList.Add(obj20);
            dynGridModelsList.Add(obj21);
            dynGridModelsList.Add(obj22);
            dynGridModelsList.Add(obj23);
            dynGridModelsList.Add(obj24);
            dynGridModelsList.Add(obj25);
            dynGridModelsList.Add(obj26);
            dynGridModelsList.Add(obj27);
            dynGridModelsList.Add(obj28);
            dynGridModelsList.Add(obj29);
            dynGridModelsList.Add(obj30);
            dynGridModelsList.Add(obj31);
            dynGridModelsList.Add(obj32);
            dynGridModelsList.Add(obj33);
            dynGridModelsList.Add(obj34);
            dynGridModelsList.Add(obj35);
            dynGridModelsList.Add(obj36);

            return dynGridModelsList;
        }

    }
}
