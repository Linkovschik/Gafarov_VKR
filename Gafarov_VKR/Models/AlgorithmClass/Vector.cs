using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.AlgorithmClass
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public Vector(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            X = EndPoint.Lat - StartPoint.Lat;
            Y = EndPoint.Lng - StartPoint.Lng;
        }

        public static double GetCOS(Vector v1, Vector v2)
        {
            if(v1.X + v1.Y + v2.X + v2.Y == 0)
            {
                return 0;
            }
            double result = (v1.X * v2.X + v1.Y * v2.Y) /
                (Math.Sqrt(Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2)) +
                Math.Sqrt(Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2)));
            return result;
        }

        public Vector GetRightPerpendicular()
        {
            Point start = new Point() { Lat = EndPoint.Lat, Lng = EndPoint.Lng };
            double x = 0;
            double y = 0;
            
            if(this.X < 0)
            {
                y = 1;
                x = -1 * y * this.Y / this.X;
            }
            else if(this.X > 0)
            {
                y = -1;
                x = -1 * y * this.Y / this.X;
            }
            else if( this.X == 0)
            {
                if (this.Y > 0)
                {
                    y = 0;
                    x = 1;
                }
                else if (this.Y < 0)
                {
                    y = 0;
                    x = -1;
                }
                else
                {
                    y = 0;
                    x = 0;
                }
            }
            Point end = new Point() { Lat = start.Lat + x, Lng = start.Lng + y};
            return new Vector(start, end);
        }
    }
}