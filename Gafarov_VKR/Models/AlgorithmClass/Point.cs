using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.AlgorithmClass
{
    public class Point
    {
        public double Lat { get; set; }

        public double Lng { get; set; }

        public Point()
        {
            Lat = 0;
            Lng = 0;
        }
        public static Point operator +(Point c1, Point c2)
        {
            return new Point { Lat = c1.Lat + c2.Lat, Lng = c1.Lng + c2.Lng };
        }

        public static Point operator -(Point c1, Point c2)
        {
            return new Point { Lat = c1.Lat - c2.Lat, Lng = c1.Lng - c2.Lng };
        }

        //расчёт манхэттенского расстояния между точками, метры
        public static double CalculatePseudoManhattanDistance(Point p1, Point p2)
        {
            double result = 0;
            double R = 6371000; // metres
            double f1 = p1.Lat * Math.PI / 180; // φ, λ in radians
            double f2 = p2.Lat * Math.PI / 180;
            double delta_f = (p2.Lat - p1.Lat) * Math.PI / 180;
            double delta_l = (p2.Lng - p1.Lng) * Math.PI / 180;

            //double a = Math.Sin(delta_f / 2) * Math.Sin(delta_f / 2) +
            //          Math.Cos(f1) * Math.Cos(f2) *
            //          Math.Sin(delta_l / 2) * Math.Sin(delta_l / 2);
            //double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            //result = R * c; // in metres

            double a = Math.Sin(delta_f / 2) * Math.Sin(delta_f / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double latitudeDistance = R * c;

            a = Math.Sin(delta_l / 2) * Math.Sin(delta_l / 2);
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double longtitudeDistance = R * c;

            result = Math.Abs(longtitudeDistance) + Math.Abs(latitudeDistance);

            return result;
        }

        public static double LatProjection(Point p1, Point p2)
        {
            return p2.Lat - p1.Lat;
        }

        public static double LngProjection(Point p1, Point p2)
        {
            return p2.Lng - p1.Lng;
        }
    }
}