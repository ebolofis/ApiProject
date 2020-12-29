using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class StaffModelsPreview
    {
        public List<StaffModels> StaffModels { get; set; }
    }

    public class StaffModels
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<AssignedPositionsModel> AssignedPositions { get; set; }
        public bool IsCheckedIn { get; set; }
        public string ImageUri { get; set; }
        public List<long> ActionsId { get; set; }
        public List<string> ActionsDescription { get; set; }
        public string Password { get; set; }
        public string Identification { get; set; }
        public Nullable<bool> AllowReporting { get; set; }
        public Nullable<bool> isAssignedToRoute { get; set; }
    }
    public class AssignedPositionsModel
    {
        public long Id { get; set; }
        public Nullable<long> StaffPositionId { get; set; }
        public Nullable<long> StaffId { get; set; }
    }

    public class StaffDeliveryModel
    {
        public long StaffId { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }

        public string ImageUri { get; set; }

        public Nullable<bool> IsOnRoad { get; set; }
        public Nullable<DateTime> StatusTimeChanged { get; set; }

        public long StaffPositionId { get; set; }
        public Nullable<bool> AllowReporting { get; set; }
        public Nullable<bool> isAssignedToRoute { get; set; }
    }

    public class StaffLoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
