using log4net;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_LoyaltyFlows : IDA_LoyaltyFlows
    {
        IDA_LoyaltyTasks loyaltyTasks;
        IStaffTasks staffTasks;
        protected ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_LoyaltyFlows(IDA_LoyaltyTasks _loyaltyTasks, IStaffTasks staffTasks)
        {
            this.loyaltyTasks = _loyaltyTasks;
            this.staffTasks = staffTasks;
        }

        public List<DA_LoyalPointsModels> GetCustomerDa_LoyalPointsHistory(DBInfoModel dBInfo, long CustomerId)
        {
            return loyaltyTasks.GetCustomerDa_LoyalPointsHistory(dBInfo,CustomerId);
        }

        /// <summary>
        /// Get Loyalty Configuration Tables
        /// </summary>
        /// <returns>Επιστρέφει τα περιεχόμενα των πινάκων DA_Loyalty  εκτός του DA_LoyalPoints</returns>
        public DA_LoyaltyFullConfigModel GetLoyaltyConfig(DBInfoModel dbInfo)
        {
            return loyaltyTasks.GetLoyaltyConfig(dbInfo);
        }

        /// <summary>
        /// Set Loyalty Configuration Tables
        /// </summary>
        /// <param name="Model">DA_LoyaltyFullConfigModel</param>
        /// <returns></returns>
        public long SetLoyaltyConfig(DBInfoModel dbInfo, DA_LoyaltyFullConfigModel Model)
        {
            return loyaltyTasks.SetLoyaltyConfig(dbInfo, Model);
        }

        /// <summary>
        /// Insert Loyalty Gain Amount Range Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertGainAmountRange(DBInfoModel dbInfo, DA_LoyalGainAmountRangeModel Model)
        {
            return loyaltyTasks.InsertGainAmountRange(dbInfo, Model);
        }

        /// <summary>
        /// Delete Loyalty Gain Points Range Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRangeRow(DBInfoModel dbInfo, long Id)
        {
            return loyaltyTasks.DeleteRangeRow(dbInfo, Id);
        }

        /// <summary>
        /// Delte All Loyalty Gain Amount Range
        /// </summary>
        /// <returns></returns>
        public long DeleteGainAmountRange(DBInfoModel dbInfo)
        {
            return loyaltyTasks.DeleteGainAmountRange(dbInfo);
        }

        /// <summary>
        /// Insert Redeem Free Product Model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertRedeemFreeProduct(DBInfoModel dbInfo, DA_LoyalRedeemFreeProductModel Model)
        {
            return loyaltyTasks.InsertRedeemFreeProduct(dbInfo, Model);
        }

        /// <summary>
        /// Delete Loyalty Redeem Free Product Row By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRedeemFreeProductRow(DBInfoModel dbInfo, long Id)
        {
            return loyaltyTasks.DeleteRedeemFreeProductRow(dbInfo, Id);
        }

        /// <summary>
        /// Delte All Redeem Free Product
        /// </summary>
        /// <returns></returns>
        public long DeleteRedeemFreeProduct(DBInfoModel dbInfo)
        {
            return loyaltyTasks.DeleteRedeemFreeProduct(dbInfo);
        }

        /// <summary>
        /// Find Total Loyalty Point of a Customer
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <returns>Tο σύνολο των πόντων του πελάτη </returns>
        public int GetLoyaltyPoints(DBInfoModel dbInfo, long Id)
        {
            return loyaltyTasks.GetLoyaltyPoints(dbInfo, Id);
        }

        /// <summary>
        /// Choose Loyalty Redeem Options
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <param name="Amount">Order Total</param>
        /// <returns>Επιστρέφει λίστα με επιλογές  που έχει ο πελάτης(κατά τη διάρκεια της παραγγελίας του) να καταναλώσει τους  πόντους του</returns>
        public DA_LoyaltyRedeemOptionsModel GetLoyaltyRedeemOptions(DBInfoModel dbInfo, long Id, decimal Amount)
        {
            return loyaltyTasks.GetLoyaltyRedeemOptions(dbInfo, Id, Amount);
        }

        ///// <summary>
        ///// εισαγωγή πόντων στο table DA_LoyalPoints από παραγγελία
        ///// </summary>
        ///// <param name="Model"></param>
        ///// <returns></returns>
        //public long InsertPointsFromOrder(Store Store, DA_OrderModel Model)
        //{
        //    return loyaltyTasks.InsertPointsFromOrder(Store, Model);
        //}

        /// <summary>
        /// εισαγωγή Αρχικών πόντων στο table DA_LoyalPoints
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertInitPoints(DBInfoModel dbInfo, DACustomerModel Model)
        {
            return loyaltyTasks.InsertInitPoints(dbInfo, Model);
        }

        ///// <summary>
        ///// Κατανάλωση Πόντων του Πελάτη με Βάση τους πόντους που δίνει ο Client
        ///// </summary>
        ///// <param name="Model"></param>
        ///// <returns></returns>
        //public long RedeemCustPoints(Store Store, DA_OrderModel Model)
        //{
        //    return loyaltyTasks.RedeemCustPoints(Store, Model);
        //}

        /// <summary>
        /// Διαγραφή κερδισμένων πόντων από table DA_LoyalPoints βάση Customerid
        /// </summary>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        public long DeleteCustomerGainPoints(DBInfoModel dbInfo, long Id)
        {
            return loyaltyTasks.DeleteCustomerGainPoints(dbInfo, Id);
        }

        /// <summary>
        /// Διαγραφή κερδισμένων πόντων από table DA_LoyalPoints βάση orderid
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId">Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0) </param>
        /// <returns></returns>
        public long DeleteGainPoints(DBInfoModel dbInfo, long Id, long StoreId)
        {
            return loyaltyTasks.DeleteGainPoints(dbInfo, Id, StoreId);
        }

        /// <summary>
        /// Διαγραφή πόντων από table DA_LoyalPoints βάση παλαιότητας
        /// </summary>
        /// <returns></returns>
        public long DeletePoints(DBInfoModel dbInfo)
        {
            return loyaltyTasks.DeletePoints(dbInfo);
        }

        /// <summary>
        /// θέτει πόντους (gained/redeemed) από παραγγελία καταστήματος. Return gained points
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="model">DA_LoyaltyStoreSetPointsModel</param>
        public int setPointsFromStore(DBInfoModel dbInfo, DA_LoyaltyStoreSetPointsModel model, long staffId)
        {

            //1. Get store Id (if there is no store then throw exception)
            if (staffId != 0 && model.StoreId == 0)
            {
                long daStoreId = staffTasks.GetDaStore(dbInfo, staffId);
                if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);
                model.StoreId = daStoreId;
            }
            //2. calculate gained points
            DA_OrderModel ordermodel = AutoMapper.Mapper.Map<DA_OrderModel>(model);
            int gained = CalcPointsFromOrder(dbInfo, ordermodel);

            //3. validate redeem points
            loyaltyTasks.CheckRedeemPoints(dbInfo, ordermodel);

            //4. add points to DB 
            loyaltyTasks.AddPoints(dbInfo, model.Id, model.CustomerId, gained, DateTime.Now, 1, model.StoreId);
            loyaltyTasks.AddPoints(dbInfo, model.Id, model.CustomerId, model.PointsRedeem, DateTime.Now, 2, model.StoreId);

            //5. return gained points
            return gained;
        }

        /// <summary>
        /// υπολογίζει τους κερδισμένους πόντους (χωρίς να τους εισάγει στην DB) από μία παραγγελία
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="model">DA_LoyaltyStoreSetPointsModel</param>
        public int CalcPointsFromOrder(DBInfoModel dbInfo, DA_LoyaltyStoreSetPointsModel model)
        {
            // calculate gained points
            DA_OrderModel ordermodel = AutoMapper.Mapper.Map<DA_OrderModel>(model);
            int gained = loyaltyTasks.CalcPointsFromOrder(dbInfo, ordermodel);
            return gained;
        }

        /// <summary>
        /// υπολογίζει τους κερδισμένους πόντους (χωρίς να τους εισάγει στην DB) από μία DA παραγγελία
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="model">DA_LoyaltyStoreSetPointsModel</param>
        public int CalcPointsFromOrder(DBInfoModel dbInfo, DA_OrderModel daOrder)
        {
            // calculate gained points
            return loyaltyTasks.CalcPointsFromOrder(dbInfo, daOrder);
        }

        public void SavePointsFromLoyaltyAdmin(DBInfoModel DBInfo, DA_LoyalPointsModels model)
        {
            //save gained loyalty points
            loyaltyTasks.SavePointsFromLoyaltyAdmin(DBInfo, model);
        }

        /// <summary>
        /// Return the list of last Loyalty entries of the Current Customer
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="customerId"></param>
        /// <param name="entries"></param>
        /// <param name="ptype"></param>
        /// <returns></returns>
        public List<DA_LoyalPointsModels> GetCustomerLoyaltyPointsHistory(DBInfoModel DBInfo, long customerId, int entries, DA_LoyaltyHistory ptype)
        {
            List<DA_LoyalPointsModels> customerLoyaltyPointsHistory = null;
            try
            {
                customerLoyaltyPointsHistory = loyaltyTasks.GetCustomerLoyaltyPointsHistory(DBInfo, customerId, entries, ptype);
            }
            catch (Exception e)
            {
                logger.Error("Error fetching loyalty points history of customer with Id: " + customerId);
                logger.Error(e.ToString());
            }
            return customerLoyaltyPointsHistory;
        }

    }
}
