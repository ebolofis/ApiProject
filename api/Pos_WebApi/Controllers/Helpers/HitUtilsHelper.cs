using Newtonsoft.Json;
using Pos_WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Pos_WebApi.Helpers
{
    public static  class HitUtilsHelper
    {

        public static Object EndOfDayProductSalesComparer(PosEntities db, string filters)
        {
            var flts = JsonConvert.DeserializeObject<StatisticFilters>(filters);
            List<long> eod = new List<long>();
            List<long?> posList = new List<long?>();
            List<long?> staffList = new List<long?>();
            int repno = 0;
            if (String.IsNullOrEmpty(filters))
                eod.Add(0);
            else
            {
                if (flts.UseEod)
                    if (flts.UsePeriod)
                    {
                        eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(flts.FromDate) <= EntityFunctions.TruncateTime(w.FODay) &&
                                                            EntityFunctions.TruncateTime(flts.ToDate) >= EntityFunctions.TruncateTime(w.FODay)).Select(s => s.Id).ToList();
                    }
                    else
                    {
                        eod = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == EntityFunctions.TruncateTime(flts.FODay)).Select(s => s.Id).ToList();
                    }
                else
                    eod.Add(0);
                posList.AddRange(flts.PosList.ToList());
                staffList.AddRange(flts.StaffList.ToList());
                repno = flts.ReportType;
            };

            var inHist = db.ProductSalesHistoryPerDay.Select(s => new { EodId = s.EodId, PosInfoId = s.PosInfoId, Total = s.Total }).GroupBy(g => g.EodId).Select(ss => new { EodId = ss.Key, PosInfoId = ss.FirstOrDefault().PosInfoId, Total = ss.Sum(sm => sm.Total) }).OrderBy(o => o.EodId);

            var allData = from q in db.EndOfDay.ToList().Where(w => eod.Contains(w.Id) /*&& posList.Contains(w.PosInfoId)*/)
                          join qq in inHist on q.Id equals qq.EodId into f
                          from ih in f.DefaultIfEmpty()
                          join pi in db.PosInfo on q.PosInfoId equals pi.Id
                          select new
                          {
                              EodId = q.Id,
                              PosInfoId = q.PosInfoId,
                              PosInfoDescription = pi.Description,
                              FODay = q.FODay,
                              CloseId = q.CloseId,
                              Total = q.Gross,
                              HistorySummary = ih != null ? ih.Total : 0

                          };
            return new { processedResults = allData.OrderBy(o => o.FODay)};
        }
     
    }
}