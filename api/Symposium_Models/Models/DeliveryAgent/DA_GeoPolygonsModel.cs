using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_GeoPolygonsBasicModel
    {
        /// <summary>
        /// Polygon Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// κατάστημα στο οποίο ανήκει το πολ. (μπορεί να μη ανήκει σε κανένα πολύγωνο)
        /// </summary>
        public Nullable<long> StoreId { get; set; }
    }

    public class DA_GeoPolygonsModel : DA_GeoPolygonsBasicModel
    {
        public string StoreDescr { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; }

        public string PolygonsColor { get; set; }

        public Nullable<float> Latitude { get; set; }

        public Nullable<float> Longtitude { get; set; }

        public List<DA_GeoPolygonsDetailsModel> Details { get; set; }
    }

    public class DA_GeoPolygonsDetailsModel
    {
        public long Id { get; set; }

        /// <summary>
        /// DA_GeoPolygons.Id
        /// </summary>
        public long PolygId { get; set; }

        public float Latitude { get; set; }

        public float Longtitude { get; set; }
    }
}
