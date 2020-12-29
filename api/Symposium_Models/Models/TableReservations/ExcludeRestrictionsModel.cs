using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class ExcludeRestrictionsModel
    {
        /// <summary>
        /// Id Record key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id Περιορισμού (TR_Restriction.Id) του Εστιατορίου για τη συγκεκριμένη μέρα
        /// </summary>
        [Required]
        public long RestRestrictAssocId { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// Date of Restriction
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
    }

    public class ExcludeRestrictionsListModel
    {
        public List<ExcludeRestrictionsModel> ExcludeRestrictionsList { get; set; }
    }
}
