using Pos_WebApi.Models;
using System.Collections.Generic;

namespace Pos_WebApi.Repositories.PMSRepositories
{
    public interface IPMS_Base
    {
        IEnumerable<Customers> GetReservations(int hotelId, string name, string room, int? page = 0, int? pagesize = 12, string reservationId = "");
        IEnumerable<PmsDepartmentModel> GetPMSDepartments(long hotelid);
    }
}