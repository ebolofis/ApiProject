using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Symposium.Models.Models.ExternalSystems.Efood
{


//Represents a record in EFoodBucket table
public class ExtDeliveryBucketModel
{
    /// <summary>
    /// Order id from efood ()
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Efood Order as json
    /// </summary>
    public string Json { get; set; }

    /// <summary>
    /// CreateDate
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// CreateDate
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Errors-Mismatches
    /// </summary>
    // public string Errors { get; set; }
}
}
