using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Repositories
{
    public class LookUpRepository 
    {
        //  internal IStoreContext _ctx = null;
        protected PosEntities DbContext;

        public LookUpRepository(PosEntities db)
        {
            this.DbContext = db;
        }

        public void Dispose()
        {
            //    ctx.Dispose();
        }

        //public PDALookUpsModel GetPdaLookUps(string ip)
        //{
        //    throw new NotImplementedException();
        //}

        public PosLookUpsModel GetPosLookUps(string ip)
        {
            PosLookUpsModel pm = new PosLookUpsModel();
            var module = DbContext.PosInfo.SingleOrDefault(s => s.IPAddress == ip);
            if (module == null)
                throw new NotImplementedException();
            DbContext.Entry(module).Reference(r => r.Department).Load();
            DbContext.Entry(module).Collection(c => c.PosInfoDetail).Load();
            DbContext.Entry(module).Collection(c => c.PosInfo_StaffPositin_Assoc).Load();

            var staffTemp = DbContext.PosInfo_StaffPositin_Assoc.Include("AssignedPositions.Staff.StaffAuthorization").Where(w => w.PosInfoId == module.Id)
                                                     .SelectMany(s => s.StaffPosition.AssignedPositions)
                                                     .Select(ss => ss.Staff).Where(w => (w.IsDeleted ?? false) == false).Distinct().Select(s => s.Id);

            var tempStaff = module.PosInfo_StaffPositin_Assoc.Select(s => s.StaffPosition).SelectMany(sm => sm.AssignedPositions).Select(s => s.Staff);
            pm.ModuleId = module.Id;
            pm.IPAddress = module.IPAddress;
            pm.DepartmentId = module.DepartmentId;
            pm.DepartmentDescription = module.DepartmentId != null ? module.Department.Description : "";
            pm.PaymentOptions = module.PosInfoDetail.Select(s => new PaymentOptions()
            {
                Id = s.Id,
                Abbreviation = s.Abbreviation,
                ButtonDescription = s.ButtonDescription,
                Counter = s.Counter,
                CreateTransaction = s.CreateTransaction ?? false,
                Description = s.Description,
                GroupId = s.GroupId,
                InvoiceId = s.InvoiceId,
                InvoiceTypeId = s.InvoicesTypeId,
                IsCancel = s.IsCancel ?? false,
                IsInvoice = s.IsInvoice ?? false,
                PosInfoId = s.PosInfoId
            });
            //    pm.StaffList = staff;
            return pm;
        }
    }
}
