namespace Gafarov_VKR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ManeuverDifficlutyLevels
    {
        public int Id { get; set; }

        public int User_Id { get; set; }

        public int Maneuver_Id { get; set; }

        public int DifficultyLevel_Id { get; set; }

        public virtual DifficultyLevels DifficultyLevels { get; set; }

        public virtual Maneuvers Maneuvers { get; set; }

        public virtual Users Users { get; set; }
    }
}
