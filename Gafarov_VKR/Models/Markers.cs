namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Markers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Markers()
        {
            EndManeuverMarkers = new HashSet<EndManeuverMarkers>();
            OtherManeuverPoints = new HashSet<OtherManeuverPoints>();
            SignMarkers = new HashSet<SignMarkers>();
            StartManeuverMarkers = new HashSet<StartManeuverMarkers>();
        }

        public int Id { get; set; }

        public float Lat { get; set; }

        public float Lng { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndManeuverMarkers> EndManeuverMarkers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OtherManeuverPoints> OtherManeuverPoints { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SignMarkers> SignMarkers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StartManeuverMarkers> StartManeuverMarkers { get; set; }
    }
}
