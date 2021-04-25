using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class ManeuverTypeModel
    {
        public int ManeuverTypeId { get; set; }
        public string ManeuverTypeName { get; set; }
        public double ManeuverTypePenalty { get; set; }
    }
}