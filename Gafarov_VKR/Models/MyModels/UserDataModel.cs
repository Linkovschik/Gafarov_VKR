using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class UserDataModel
    {
        public List<List<object>> SignProblems { get; set; }

        public List<List<object>> ManeuverProblems { get; set; }

        public PointModel StartPosition { get; set; }

        public int Time { get; set; }

        public int Speed { get; set; }
    }
}