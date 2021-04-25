namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ManeuverTypePenalties
    {
        public int Id { get; set; }

        public float Penalty { get; set; }

        public int ManeuverType_Id { get; set; }

        public virtual ManeuverTypes ManeuverTypes { get; set; }
    }
}
