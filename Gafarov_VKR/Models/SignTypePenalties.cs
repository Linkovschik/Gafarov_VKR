namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SignTypePenalties
    {
        public int Id { get; set; }

        public float Penalty { get; set; }

        public int SignType_Id { get; set; }

        public virtual SignTypes SignTypes { get; set; }
    }
}
