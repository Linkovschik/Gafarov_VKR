using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.MyModels
{
    public class SignTypeModel
    {
        public int SignTypeId { get; set; }
        public string SignTypeName { get; set; }
        public double SignTypePenalty { get; set; }
    }
}