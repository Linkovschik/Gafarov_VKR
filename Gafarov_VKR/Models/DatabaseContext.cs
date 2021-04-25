using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Gafarov_VKR.Models
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DatabaseContext")
        {
        }

        public virtual DbSet<AcceptedManeuvers> AcceptedManeuvers { get; set; }
        public virtual DbSet<AcceptedSigns> AcceptedSigns { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<DifficultyLevels> DifficultyLevels { get; set; }
        public virtual DbSet<EndManeuverMarkers> EndManeuverMarkers { get; set; }
        public virtual DbSet<ManeuverDifficlutyLevels> ManeuverDifficlutyLevels { get; set; }
        public virtual DbSet<Maneuvers> Maneuvers { get; set; }
        public virtual DbSet<ManeuverTypePenalties> ManeuverTypePenalties { get; set; }
        public virtual DbSet<ManeuverTypes> ManeuverTypes { get; set; }
        public virtual DbSet<Markers> Markers { get; set; }
        public virtual DbSet<OtherManeuverPoints> OtherManeuverPoints { get; set; }
        public virtual DbSet<SignDifficlutyLevels> SignDifficlutyLevels { get; set; }
        public virtual DbSet<SignMarkers> SignMarkers { get; set; }
        public virtual DbSet<Signs> Signs { get; set; }
        public virtual DbSet<SignTypePenalties> SignTypePenalties { get; set; }
        public virtual DbSet<SignTypes> SignTypes { get; set; }
        public virtual DbSet<StartManeuverMarkers> StartManeuverMarkers { get; set; }
        public virtual DbSet<SuggestedManeuvers> SuggestedManeuvers { get; set; }
        public virtual DbSet<SuggestedSigns> SuggestedSigns { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DifficultyLevels>()
                .HasMany(e => e.ManeuverDifficlutyLevels)
                .WithRequired(e => e.DifficultyLevels)
                .HasForeignKey(e => e.DifficultyLevel_Id);

            modelBuilder.Entity<DifficultyLevels>()
                .HasMany(e => e.SignDifficlutyLevels)
                .WithRequired(e => e.DifficultyLevels)
                .HasForeignKey(e => e.DifficultyLevel_Id);

            modelBuilder.Entity<Maneuvers>()
                .HasMany(e => e.AcceptedManeuvers)
                .WithRequired(e => e.Maneuvers)
                .HasForeignKey(e => e.Maneuver_Id);

            modelBuilder.Entity<Maneuvers>()
                .HasMany(e => e.EndManeuverMarkers)
                .WithRequired(e => e.Maneuvers)
                .HasForeignKey(e => e.Maneuver_Id);

            modelBuilder.Entity<Maneuvers>()
                .HasMany(e => e.ManeuverDifficlutyLevels)
                .WithRequired(e => e.Maneuvers)
                .HasForeignKey(e => e.Maneuver_Id);

            modelBuilder.Entity<Maneuvers>()
                .HasMany(e => e.OtherManeuverPoints)
                .WithRequired(e => e.Maneuvers)
                .HasForeignKey(e => e.Maneuver_Id);

            modelBuilder.Entity<Maneuvers>()
                .HasMany(e => e.StartManeuverMarkers)
                .WithRequired(e => e.Maneuvers)
                .HasForeignKey(e => e.Maneuver_Id);

            modelBuilder.Entity<Maneuvers>()
                .HasMany(e => e.SuggestedManeuvers)
                .WithRequired(e => e.Maneuvers)
                .HasForeignKey(e => e.Maneuver_Id);

            modelBuilder.Entity<ManeuverTypes>()
                .HasMany(e => e.Maneuvers)
                .WithRequired(e => e.ManeuverTypes)
                .HasForeignKey(e => e.ManeuverType_Id);

            modelBuilder.Entity<ManeuverTypes>()
                .HasMany(e => e.ManeuverTypePenalties)
                .WithRequired(e => e.ManeuverTypes)
                .HasForeignKey(e => e.ManeuverType_Id);

            modelBuilder.Entity<Markers>()
                .HasMany(e => e.EndManeuverMarkers)
                .WithRequired(e => e.Markers)
                .HasForeignKey(e => e.Marker_Id);

            modelBuilder.Entity<Markers>()
                .HasMany(e => e.OtherManeuverPoints)
                .WithRequired(e => e.Markers)
                .HasForeignKey(e => e.Marker_Id);

            modelBuilder.Entity<Markers>()
                .HasMany(e => e.SignMarkers)
                .WithRequired(e => e.Markers)
                .HasForeignKey(e => e.Marker_Id);

            modelBuilder.Entity<Markers>()
                .HasMany(e => e.StartManeuverMarkers)
                .WithRequired(e => e.Markers)
                .HasForeignKey(e => e.Marker_Id);

            modelBuilder.Entity<Signs>()
                .HasMany(e => e.AcceptedSigns)
                .WithRequired(e => e.Signs)
                .HasForeignKey(e => e.Sign_Id);

            modelBuilder.Entity<Signs>()
                .HasMany(e => e.SignDifficlutyLevels)
                .WithRequired(e => e.Signs)
                .HasForeignKey(e => e.Sign_Id);

            modelBuilder.Entity<Signs>()
                .HasMany(e => e.SignMarkers)
                .WithRequired(e => e.Signs)
                .HasForeignKey(e => e.Sign_Id);

            modelBuilder.Entity<Signs>()
                .HasMany(e => e.SuggestedSigns)
                .WithRequired(e => e.Signs)
                .HasForeignKey(e => e.Sign_Id);

            modelBuilder.Entity<SignTypes>()
                .HasMany(e => e.Signs)
                .WithRequired(e => e.SignTypes)
                .HasForeignKey(e => e.SignType_Id);

            modelBuilder.Entity<SignTypes>()
                .HasMany(e => e.SignTypePenalties)
                .WithRequired(e => e.SignTypes)
                .HasForeignKey(e => e.SignType_Id);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.AcceptedManeuvers)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.User_Id);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.AcceptedSigns)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.User_Id);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Admin)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.User_Id);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.ManeuverDifficlutyLevels)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.User_Id);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.SignDifficlutyLevels)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.User_Id);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.SuggestedManeuvers)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.User_Id);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.SuggestedSigns)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.User_Id);
        }
    }
}
