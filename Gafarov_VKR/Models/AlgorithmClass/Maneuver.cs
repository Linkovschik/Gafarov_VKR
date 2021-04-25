using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.AlgorithmClass
{
    public class Maneuver : BaseMark
    {

        public Point EndMarkerPoint { get; set; }

        public Point MediumMarkerPoint { get; set; }

        public string ManeuverTypeName { get; set; }
    }
}