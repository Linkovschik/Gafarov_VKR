using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class ManeuverModel
    {
        public int Id { get; set; }

        public PointModel StartMarkerPoint { get; set; }

        public PointModel EndMarkerPoint { get; set; }

        public List<PointModel> OtherPoints { get; set; }

        public int DifficultyLevelValue { get; set; }

        public string ManeuverTypeName { get; set; }

        public int UserId { get; set; }

        public bool Editable { get; set; }
    }
}