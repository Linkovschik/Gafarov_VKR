namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DifficultyLevels
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DifficultyLevels()
        {
            ManeuverDifficlutyLevels = new HashSet<ManeuverDifficlutyLevels>();
            SignDifficlutyLevels = new HashSet<SignDifficlutyLevels>();
        }

        public int Id { get; set; }

        public int Value { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManeuverDifficlutyLevels> ManeuverDifficlutyLevels { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SignDifficlutyLevels> SignDifficlutyLevels { get; set; }
    }
}
