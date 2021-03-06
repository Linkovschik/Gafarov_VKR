using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class SignModel
    {
        public int Id { get; set; }

        public PointModel MarkerPoint { get; set; }

        public int DifficultyLevelValue { get; set; }

        public string SignTypeName { get; set; }

        public int UserId { get; set; }

        public bool Editable { get; set; }
    }
}