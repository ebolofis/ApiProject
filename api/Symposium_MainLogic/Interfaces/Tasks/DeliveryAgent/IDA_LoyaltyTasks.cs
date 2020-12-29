using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_LoyaltyTasks
    {

        /// <summary>
        /// Get DA_LoyalPoints List
        /// </summary>
        /// <param name="dBInfo"></param>
        /// <returns></returns>
        List<DA_LoyalPointsModels> GetCustomerDa_LoyalPointsHistory(DBInfoModel dBInfo,long CustomerId);
        /// <summary>
        /// Get Loyalty Configuration Tables
        /// </summary>
        /// <returns>Επιστρέφει τα περιεχόμενα των πινάκων DA_Loyalty  εκτός του DA_LoyalPoints</returns>
        DA_LoyaltyFullConfigModel GetLoyaltyConfig(DBInfoModel dbInfo);

        /// <summary>
        /// Set Loyalty Configuration Tables
        /// </summary>
        /// <param name="Model">DA_LoyaltyFullConfigModel</param>
        /// <returns></returns>
        long SetLoyaltyConfig(DBInfoModel dbInfo, DA_LoyaltyFullConfigModel Model);

        /// <summary>
        /// Insert Loyalty Gain Amount Range Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long InsertGainAmountRange(DBInfoModel dbInfo, DA_LoyalGainAmountRangeModel Model);

        /// <summary>
        /// Delete Loyalty Gain Points Range Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteRangeRow(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Delte All Loyalty Gain Amount Range
        /// </summary>
        /// <returns></returns>
        long DeleteGainAmountRange(DBInfoModel dbInfo);

        /// <summary>
        /// Insert Redeem Free Product Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long InsertRedeemFreeProduct(DBInfoModel dbInfo, DA_LoyalRedeemFreeProductModel Model);

        /// <summary>
        /// Delete Loyalty Redeem Free Product Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteRedeemFreeProductRow(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Delte All Redeem Free Product
        /// </summary>
        /// <returns></returns>
        long DeleteRedeemFreeProduct(DBInfoModel dbInfo);

        /// <summary>
        /// Find Total Loyalty Point of a Customer
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <returns>Tο σύνολο των πόντων του πελάτη </returns>
        int GetLoyaltyPoints(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Choose Loyalty Redeem Options
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <param name="Amount">Order Total</param>
        /// <returns>Επιστρέφει λίστα με επιλογές  που έχει ο πελάτης(κατά τη διάρκεια της παραγγελίας του) να καταναλώσει τους  πόντους του</returns>
        DA_LoyaltyRedeemOptionsModel GetLoyaltyRedeemOptions(DBInfoModel dbInfo, long Id, decimal Amount);

        /// <summary>
        /// υπολογισμός κερδισμένων πόντων σε παραγγελία 
        /// </summary>
        /// <param name="Model">order</param>
        /// <returns>gain points</returns>
        int CalcPointsFromOrder(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// εισαγωγή Αρχικών πόντων στο table DA_LoyalPoints
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long InsertInitPoints(DBInfoModel dbInfo, DACustomerModel Model);

        /// <summary>
        /// Validate Κατανάλωσης Πόντων του Πελάτη με Βάση τους πόντους που δίνει ο Client. 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        void CheckRedeemPoints(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// Διαγραφή κερδισμένων πόντων από table DA_LoyalPoints βάση Customerid
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        long DeleteCustomerGainPoints(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Διαγραφή κερδισμένων πόντων από table DA_LoyalPoints βάση orderid
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId">Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0) </param>
        /// <returns></returns>
        long DeleteGainPoints(DBInfoModel dbInfo, long Id, long StoreId);

        /// <summary>
        /// Διαγραφή πόντων από table DA_LoyalPoints βάση παλαιότητας
        /// </summary>
        /// <returns></returns>
        long DeletePoints(DBInfoModel dbInfo);

        /// <summary>
        /// Add loyalty points (gained/redeemed) to tables DA_LoyalPoints and DA_Orders
        /// </summary>
        ///  <param name="dbInfo">Store</param>
        /// <param name="OrderId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="Points"> gain/redeem points </param>
        /// <param name="type">1= gain points. 2= redeem points </param>
        /// <param name="StoreId">Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0) </param>
        void AddPoints(DBInfoModel dbInfo, long OrderId, long CustomerId, int Points, DateTime Date, int type, long StoreId);


        void SavePointsFromLoyaltyAdmin(DBInfoModel DBInfo, DA_LoyalPointsModels model);

        /// <summary>
        /// Return the list of last Loyalty entries of the Current Customer
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="customerId"></param>
        /// <param name="entries"></param>
        /// <param name="ptype"></param>
        /// <returns></returns>
        List<DA_LoyalPointsModels> GetCustomerLoyaltyPointsHistory(DBInfoModel DBInfo, long customerId, int entries, DA_LoyaltyHistory ptype);

    }
}
