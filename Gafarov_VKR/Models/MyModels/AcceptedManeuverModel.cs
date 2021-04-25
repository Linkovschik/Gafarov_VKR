using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class AcceptedManeuverModel
    {
        public int ManeuverId { get; set; }

        public PointModel StartMarkerPoint { get; set; }

        public PointModel EndMarkerPoint { get; set; }

        public List<PointModel> OtherPoints { get; set; }

        public int DifficultyLevelValue { get; set; }

        public int AverageDifficultyLevel { get; set; }

        public string ManeuverTypeName { get; set; }

        public int CreatorUserId { get; set; }

        public int RatedUserId { get; set; }

        public bool HasNoRatingYet { get; set; }
    }
}