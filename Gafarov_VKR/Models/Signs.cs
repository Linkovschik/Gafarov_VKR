namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Signs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Signs()
        {
            AcceptedSigns = new HashSet<AcceptedSigns>();
            SignDifficlutyLevels = new HashSet<SignDifficlutyLevels>();
            SignMarkers = new HashSet<SignMarkers>();
            SuggestedSigns = new HashSet<SuggestedSigns>();
        }

        public int Id { get; set; }

        public int SignType_Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AcceptedSigns> AcceptedSigns { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SignDifficlutyLevels> SignDifficlutyLevels { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SignMarkers> SignMarkers { get; set; }

        public virtual SignTypes SignTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuggestedSigns> SuggestedSigns { get; set; }
    }
}
