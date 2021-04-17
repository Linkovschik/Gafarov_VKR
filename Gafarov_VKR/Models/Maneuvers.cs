namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Maneuvers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Maneuvers()
        {
            AcceptedManeuvers = new HashSet<AcceptedManeuvers>();
            EndManeuverMarkers = new HashSet<EndManeuverMarkers>();
            ManeuverDifficlutyLevels = new HashSet<ManeuverDifficlutyLevels>();
            StartManeuverMarkers = new HashSet<StartManeuverMarkers>();
            SuggestedManeuvers = new HashSet<SuggestedManeuvers>();
        }

        public int Id { get; set; }

        public int ManeuverType_Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AcceptedManeuvers> AcceptedManeuvers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndManeuverMarkers> EndManeuverMarkers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManeuverDifficlutyLevels> ManeuverDifficlutyLevels { get; set; }

        public virtual ManeuverTypes ManeuverTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StartManeuverMarkers> StartManeuverMarkers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuggestedManeuvers> SuggestedManeuvers { get; set; }
    }
}
