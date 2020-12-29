using NCrontab;
using Symposium.Models.Models.Scheduler;
using Symposium_SchedulerJob.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_SchedulerJob.Helper
{
    public class AddJobToList
    {
        /// <summary>
        /// Add's or update's a job from job list
        /// </summary>
        /// <param name="Job"></param>
        /// <param name="Cron"></param>
        /// <param name="Status"></param>
        /// <param name="JobList"></param>
        public void UpSertAJobToList(object Job, string Cron, string JobName, bool Status, ref List<TimerModel> JobList, ParametersSchedulerModel StartParams)
        {
            if (JobList == null)
                JobList = new List<TimerModel>();

            var start = DateTime.Now;
            var end = start.AddMonths(25);
            var s = CrontabSchedule.Parse(Cron);
            var occurrences = s.GetNextOccurrences(start, end).Take(1).Select(x => x.ToString("  ddd, dd MMM yyyy  HH:mm")).FirstOrDefault();

            var fld = JobList.Find(f => f.ExecuteClass == Job);
            if (fld != null)
            {
                //fld.Running = false;
                fld.NextExecution = DateTime.Parse(occurrences);
                fld.Running = Status;
            }
            else
                JobList.Add(new TimerModel { ExecuteClass = Job, NextExecution = DateTime.Parse(occurrences), Running = Status, cron = Cron, JobName = JobName, StartParams = StartParams });
        }

        /// <summary>
        /// Remove's a job from jobList
        /// </summary>
        /// <param name="JobName"></param>
        /// <param name="JobList"></param>
        public void RemoveJobFromJobsList(string JobName, ref List<TimerModel> JobList)
        {
            var fld = JobList.Find(f => f.JobName == JobName);
            if (fld != null)
                JobList.Remove(fld);
        }
    }
}
