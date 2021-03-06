using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class AlgorithmResultModel
    {
        public List<int> SignIds { get; set; }
        public List<int> ManeuverIds { get; set; }
        public List<PointModel> Waypoints { get; set; }
        public double Difficulty { get; set; }
        public double AverageDifficulty { get; set; }
        public double AlgorithmTime { get; set; }
    }
}