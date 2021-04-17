namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SignMarkers
    {
        public int Id { get; set; }

        public int Marker_Id { get; set; }

        public int Sign_Id { get; set; }

        public virtual Markers Markers { get; set; }

        public virtual Signs Signs { get; set; }
    }
}
