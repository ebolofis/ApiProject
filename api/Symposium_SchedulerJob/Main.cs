using Autofac;
using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.Scheduler;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium_SchedulerJob.Helper;
using Symposium_SchedulerJob.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Symposium_SchedulerJob
{
    public class Main
    {
        private Timer schedTimer;

        string CultureCode = "el-GR";

        public List<TimerModel> ActiveJobs;

        //public ParametersSchedulerModel StartParams = null;

        //private Thread timerThread;

        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IContainer _container;
        private IStoreIdsPropertiesHelper store;



        public Main(IContainer Container, IStoreIdsPropertiesHelper store)
        {
            _container = Container;
            this.store = store;
            ActiveJobs = new List<TimerModel>();
        }

        public void StartTimer()
        {
            logger.Info("Scheduler started ......");

            CultureCode = ConfigurationManager.AppSettings["CultureInfo"];
            if (string.IsNullOrEmpty(CultureCode))
                CultureCode = "el-GR";

            schedTimer = new Timer(30000);
            schedTimer.Elapsed += OnTimedEvent;
            schedTimer.AutoReset = true;
            schedTimer.Enabled = true;
        }


        private void ThreadTask(object Job, string Cron, string JobName)
        {
            //Job.GetType().GetMethod("Start").Invoke(Job, null);
            Task.Factory.StartNew(() =>
            {
                AddJobToList hlp = new AddJobToList();
                try
                {
                    logger.Debug("Job " + JobName + " Started");
                    hlp.UpSertAJobToList(Job, Cron, JobName, true, ref ActiveJobs, null);
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(CultureCode);
                    System.Threading.Thread.CurrentThread.CurrentCulture = ci;
                    Job.GetType().GetMethod("Start").Invoke(Job, null);
                }
                catch (Exception ex)
                {
                    logger.Error("ExecuteJob [Job" + Job.ToString() + "] \r\n " + ex.ToString());
                }
                finally
                {
                    hlp.UpSertAJobToList(Job, Cron, JobName, false, ref ActiveJobs, null);
                    logger.Debug("Job " + JobName + " Finished");
                }
            }

            );
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                string startJob = "";
                AddJobToList hlp = new AddJobToList();

                /*Check if Job Send orders from Server to stores is active */
                startJob = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_ServerOrderExec");
                startJob = startJob.Trim();
                if (!string.IsNullOrEmpty(startJob))
                {
                    var fld = ActiveJobs.Find(f => f.JobName == "Send orders from Server to Client");
                    if (fld == null)
                        ActivateJob(typeof(IDA_HangFireJobsSendOrdersFlows), startJob, "Send orders from Server to Client");
                }
                else
                    hlp.RemoveJobFromJobsList("Send orders from Server to Client", ref ActiveJobs);

                /*Check if Job to update order status from store to server is active */
                startJob = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_ClientOrderExec");
                startJob = startJob.Trim();
                if (!string.IsNullOrEmpty(startJob))
                {
                    var fld = ActiveJobs.Find(f => f.JobName == "Update order status from Client to Server");
                    if (fld == null)
                        ActivateJob(typeof(IDA_HangFireJobsUpdateStatusFlows), startJob, "Update order status from Client to Server");
                }
                else
                    hlp.RemoveJobFromJobsList("Update order status from Client to Server", ref ActiveJobs);

                /*Check if Job for store to send isdealy orders to kitchen is active */
                startJob = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_SendDelayToKitchenExec");
                startJob = startJob.Trim();
                if (!string.IsNullOrEmpty(startJob))
                {
                    var fld = ActiveJobs.Find(f => f.JobName == "Send IsDelay Orders to kitchen");
                    if (fld == null)
                        ActivateJob(typeof(IDA_HangFireJobsCheckIsDelayOnStoresFlows), startJob, "Send IsDelay Orders to kitchen");
                }
                else
                    hlp.RemoveJobFromJobsList("Send IsDelay Orders to kitchen", ref ActiveJobs);

                /*Check if Job to make customer anonymous in server is active */
                startJob = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_MakeCustomerAnonymousExec");
                startJob = startJob.Trim();
                if (!string.IsNullOrEmpty(startJob))
                {
                    var fld = ActiveJobs.Find(f => f.JobName == "Make Customer Anonymous");
                    if (fld == null)
                        ActivateJob(typeof(IDA_HangFireJobsMakeCustomerAnonymousFlows), startJob, "Make Customer Anonymous");
                }
                else
                    hlp.RemoveJobFromJobsList("Make Customer Anonymous", ref ActiveJobs);

                /*Check if Job to delete loyalty points from server is active */
                startJob = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_DeleteLoyaltyPointsExec");
                startJob = startJob.Trim();
                if (!string.IsNullOrEmpty(startJob))
                {
                    var fld = ActiveJobs.Find(f => f.JobName == "Delete Loyalty Points");
                    if (fld == null)
                        ActivateJob(typeof(IDA_LoyaltyExecutionsFlow), startJob, "Delete Loyalty Points");
                }
                else
                    hlp.RemoveJobFromJobsList("Delete Loyalty Points", ref ActiveJobs);

                /*Check if Job to get orders from eFood is active */
                startJob = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_EFoodOrdersExec");
                startJob = startJob.Trim();
                if (!string.IsNullOrEmpty(startJob))
                {
                    var fld = ActiveJobs.Find(f => f.JobName == "Get EFood Orders");
                    if (fld == null)
                        ActivateJob(typeof(IDA_EFoodGetOrdersFlow), startJob, "Get EFood Orders");
                }
                else
                    hlp.RemoveJobFromJobsList("Get EFood Orders", ref ActiveJobs);


                /*Check if Job to update store tables with new changes from server tables is active */
                startJob = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_UpdateClientTablesExec");
                startJob = startJob.Trim();
                bool isServer = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                List<Da_Stores_ParametersModel> storeInfo = new List<Da_Stores_ParametersModel>();

                if (isServer)
                {
                    string AgentStoreIdRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                    string AgentStoreId = AgentStoreIdRaw.Trim().ToLower();
                    DBInfoModel dbInfo = store.GetStores().Find(f => f.Id.ToString().Equals(AgentStoreId));
                    DataSet ds = new DataSet();
                    if (dbInfo != null)
                    {
                        string SQL = "SELECT ds.Id, ds.CronScheduler,ds.Title FROM DA_Stores AS ds WHERE ISNULL(ds.CronScheduler,'') <> ''";
                        string conn = "data source = " + dbInfo.DataSource + "; initial catalog = " + dbInfo.DataBase + "; persist security info = True; user id = " + dbInfo.DataBaseUsername + "; password = " + dbInfo.DataBasePassword + "; MultipleActiveResultSets = True; App = HIT.SymPOSium";
                        using (SqlConnection db = new SqlConnection(conn))
                        {
                            SqlDataAdapter da = new SqlDataAdapter(SQL, db);
                            da.Fill(ds);
                            if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                                foreach (DataRow row in ds.Tables[0].Rows)
                                    storeInfo.Add(new Da_Stores_ParametersModel { Id = (long)row["Id"], Title = (string)row["Title"], CronScheduler = (string)row["CronScheduler"] });
                        }
                    }
                    else logger.Error("Cannot get store by id [" + AgentStoreId.ToString() + "]");

                    if (!string.IsNullOrEmpty(startJob))
                    {
                        //Removes all jobs for store's if exists
                        foreach (Da_Stores_ParametersModel item in storeInfo)
                            hlp.RemoveJobFromJobsList("Update tables from Server to Client [" + item.Id.ToString() + "] " + item.Title, ref ActiveJobs);

                        //Add Job for all stores
                        var fld = ActiveJobs.Find(f => f.JobName == "Update tables from Server to Client");
                        if (fld == null)
                            ActivateJob(typeof(IDA_HangFireJobsUpdateClientTableFlows), startJob, "Update tables from Server to Client");
                    }
                    else
                    {
                        //Removes jobs for update tables for all stores
                        hlp.RemoveJobFromJobsList("Update tables from Server to Client", ref ActiveJobs);

                        //Add's job for each store if is active
                        foreach (Da_Stores_ParametersModel item in storeInfo)
                        {
                            ParametersSchedulerModel parameters = new ParametersSchedulerModel();
                            parameters.StoreIdForUpdate = item.Id;

                            var fld = ActiveJobs.Find(f => f.JobName == "Update tables from Server to Client [" + item.Id.ToString() + "] " + item.Title);
                            if (fld == null)
                                ActivateJob(typeof(IDA_HangFireJobsUpdateClientTableFlows), item.CronScheduler,
                                    "Update tables from Server to Client [" + item.Id.ToString() + "] " + item.Title, parameters);
                        }
                    }
                }

                if (ActiveJobs != null)
                    foreach (TimerModel item in ActiveJobs)
                    {
                        if (!item.Running && item.NextExecution <= DateTime.Now)
                        {
                            ExecuteJob(item.ExecuteClass, item.cron, item.JobName, item.StartParams);
                            //Thread timerThrd = new Thread(new ThreadStart(this.ThreadTask));
                            //timerThrd.IsBackground = true;
                            //timerThrd.Start();
                        }
                        else if (item.Running)
                            logger.Debug("Job " + item.JobName + " running");
                    }
            }
            catch(Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Execute's Job Asynchonus
        /// </summary>
        /// <param name="Job"></param>
        /// <param name="Cron"></param>
        private async void ExecuteJob(object Job, string Cron, string JobName, ParametersSchedulerModel StartParams = null)
        {
            if (!store.ApiRunning)
            {
                //logger.Error("Api not Running");
                return;
            }
            //else
            //    logger.Error(JobName + " - Api Running");

            //Job.GetType().GetMethod("Start").Invoke(Job, null);
            Task.Factory.StartNew(() =>
            {
                AddJobToList hlp = new AddJobToList();
                try
                {
                    logger.Debug("Job " + JobName + " Started");
                    hlp.UpSertAJobToList(Job, Cron, JobName, true, ref ActiveJobs, StartParams);
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(CultureCode);
                    System.Threading.Thread.CurrentThread.CurrentCulture = ci;
                    Job.GetType().GetMethod("Start").Invoke(Job, new object[] { StartParams });
                }
                catch (Exception ex)
                {
                    logger.Error("ExecuteJob [Job" + Job.ToString() + "] \r\n " + ex.ToString());
                }
                finally
                {
                    hlp.UpSertAJobToList(Job, Cron, JobName, false, ref ActiveJobs, StartParams);
                    logger.Debug("Job " + JobName + " Finished");
                }
            }

            );


        }

        /// <summary>
        /// Add new job to List of Active jobs.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cron"></param>
        public void ActivateJob(Type type, string cron, string jobName, ParametersSchedulerModel param = null)
        {
            try
            {
                object curJob = _container.Resolve(type);
                AddJobToList hlp = new AddJobToList();
                hlp.UpSertAJobToList(curJob, cron, jobName, false, ref ActiveJobs, param);
            }
            catch(Exception ex)
            {
                logger.Error("Cannot initialize Job " + jobName + " \r\n" + ex.ToString());
            }
        }

    }


}
