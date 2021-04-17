using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class AcceptedSignModel
    {
        public int SignId { get; set; }

        public PointModel MarkerPoint { get; set; }

        public int DifficultyLevelValue { get; set; }

        public int AverageDifficultyLevel { get; set; }

        public string SignTypeName { get; set; }

        public int CreatorUserId { get; set; }

        public int RatedUserId { get; set; }

        public bool HasNoRatingYet { get; set; }
    }
}