using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Spatial;
using System.Device.Location;
using System.Globalization;

namespace GeographyPolygons
{
    public static class PolygonUtils // <--- NOT USED ---<<<<<<
    {
        //****************************************************************
        //Προσοχή, το πρώτο και το τελευταίο σημείο του πολυγώνου πρέπει να είναι ίδιο !
        //Η σειρά του πολυγώνου πρέπει να είναι αριστερόστροφη.
        //****************************************************************

        //Επιστρέφει true όταν υπάρχει intersection, false σε αντίθετη περίπτωση
        //Input List<GeoCoordinate> polygono 1, List<GeoCoordinate> polygono 2        
        public static bool PolygonsIntersects(List<GeoCoordinate> pol1, List<GeoCoordinate> pol2)
        {
            bool res = false;

            DbGeography dbg1 = CreatePolygon(pol1);
            DbGeography dbg2 = CreatePolygon(pol2);
            res = dbg2.Intersects(dbg1);
           
            if (res)
            {
                DbGeography polygon = dbg2.Intersection(dbg1);

                if (polygon.IsClosed == null || polygon.IsClosed == false)
                    res = false;
                else
                    res = true;
            }

            return res;
        }

        //Επιστρέφει true όταν point βρίσκεται μέσα στα όρια του πολυγώνου, false σε αντίθετη περίπτωση
        //Input List<GeoCoordinate> polygono 1, GeoCoordinate point
        public static bool PointInPolygon(List<GeoCoordinate> pol1, GeoCoordinate pt)
        {
            bool res = false;

            DbGeography polygon = CreatePolygon(pol1);
            DbGeography point = CreatePoint(pt);
            res = polygon.Intersects(point);

            return res;
        }



        public static DbGeography CreatePoint(GeoCoordinate point)
        {
            return DbGeography.FromText(String.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", point.Longitude, point.Latitude));           
        }

        public static DbGeography CreatePolygon(List<GeoCoordinate> coordinates)
        {
            var coordinateList = coordinates.ToList();
            if (coordinateList.First() != coordinateList.Last())
            {
                throw new Exception("First and last point do not match. This is not a valid polygon");
            }

            var ci = new CultureInfo("en-US");
            var count = 0;
            var sb = new StringBuilder();
            sb.Append(@"POLYGON((");
            foreach (var coordinate in coordinateList)
            {
                if (count == 0)
                {
                    sb.Append(coordinate.Longitude.ToString(ci) + " " + coordinate.Latitude.ToString(ci));
                }
                else
                {
                    sb.Append("," + coordinate.Longitude.ToString(ci) + " " + coordinate.Latitude.ToString(ci));
                }

                count++;
            }

            sb.Append(@"))");

            return DbGeography.PolygonFromText(sb.ToString(), 4326);
        }
        
    }

 
}
