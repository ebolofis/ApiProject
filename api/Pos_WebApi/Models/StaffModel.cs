using System.Collections.Generic;


namespace Pos_WebApi.Models
{
    public partial class StaffModel : Staff
    {
    //    [Key]
    //    public int Id { get; set; }
    //    public String Code { get; set; }
    //    public String FirstName { get; set; }
    //    public String LastName { get; set; }
    //    public String ImageUri { get; set; }
    //    public String Password { get; set; }
        public List<long> ActionsId { get; set; }
        public bool IsCheckedIn { get; set; } 
    }
}