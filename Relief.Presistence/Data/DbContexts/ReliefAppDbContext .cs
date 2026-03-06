using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Entities.Offers;
using Relief.Domain.Entities.Users;

namespace Relief.Presistence.Data.DbContexts
{
    public class ReliefAppDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ReliefAppDbContext(DbContextOptions<ReliefAppDbContext> options)
            : base(options)
        {
        }

        // ============================
        // DbSets
        // ============================

        public DbSet<JobOffer> JobOffers => Set<JobOffer>();
        public DbSet<OfferShift> OfferShifts => Set<OfferShift>();

        public DbSet<CareHomeUser> CareHomeUsers => Set<CareHomeUser>();
        public DbSet<IndividualCareHomeUser> IndividualUsers => Set<IndividualCareHomeUser>();
        public DbSet<PswUser> PswUsers => Set<PswUser>();

        public DbSet<FileMetadata> Files => Set<FileMetadata>();

        public DbSet<JopRequest> JopRequests => Set<JopRequest>();
        public DbSet<JobRequestItem> JobRequestItems => Set<JobRequestItem>();

        public DbSet<Address> Addresses => Set<Address>();

        // ============================
        // Fluent API
        // ============================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // Address 1:1 with ApplicationUser
            // =========================================================
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Address)
                .WithOne(a => a.User)
                .HasForeignKey<ApplicationUser>(u => u.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // CareHome 1:1 with ApplicationUser (Shared PK)
            // =========================================================
            modelBuilder.Entity<CareHomeUser>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.CareHomeUser)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<CareHomeUser>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // IndividualCareHome 1:1 (Shared PK)
            // =========================================================
            modelBuilder.Entity<IndividualCareHomeUser>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.IndividualCareHomeUser)
                .WithOne(i => i.ApplicationUser)
                .HasForeignKey<IndividualCareHomeUser>(i => i.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // PSW 1:1 (Shared PK with ApplicationUser)
            // =========================================================
            modelBuilder.Entity<PswUser>()
                .HasKey(p => p.ApplicationUserId);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.PswUser)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<PswUser>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // PswUser → JopRequests
            // =========================================================
            modelBuilder.Entity<JopRequest>()
                .HasOne(r => r.PswUser)
                .WithMany(p => p.JobRequests)
                .HasForeignKey(r => r.PswId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // JobOffer precision
            // =========================================================
            modelBuilder.Entity<JobOffer>()
                .Property(o => o.HourlyRate)
                .HasPrecision(10, 2);

            // =========================================================
            // CareHome → JobOffers
            // =========================================================
            modelBuilder.Entity<CareHomeUser>()
                .HasMany(c => c.JobOffers)
                .WithOne(j => j.CareHomeUser)
                .HasForeignKey(j => j.CareHomeId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // Individual → JobOffers
            // =========================================================
            modelBuilder.Entity<IndividualCareHomeUser>()
                .HasMany(i => i.JobOffers)
                .WithOne(j => j.IndividualCareHomeUser)
                .HasForeignKey(j => j.IndividualId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // JobOffer → OfferShifts
            // =========================================================
            modelBuilder.Entity<JobOffer>()
                .HasMany(o => o.Shifts)
                .WithOne(s => s.JobOffer)
                .HasForeignKey(s => s.JobOfferId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // PswUser → Files (ALL OPTIONAL + Restrict)
            // =========================================================
            modelBuilder.Entity<PswUser>()
                .HasOne(p => p.CVFile)
                .WithMany()
                .HasForeignKey(p => p.CVFileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PswUser>()
                .HasOne(p => p.PswCertificateFile)
                .WithMany()
                .HasForeignKey(p => p.PswCertificateFileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PswUser>()
                .HasOne(p => p.ProofIdentityFile)
                .WithMany()
                .HasForeignKey(p => p.ProofIdentityFileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PswUser>()
                .HasOne(p => p.ImmunizationRecordFile)
                .WithMany()
                .HasForeignKey(p => p.ImmunizationRecordFileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PswUser>()
                .HasOne(p => p.CriminalRecordFile)
                .WithMany()
                .HasForeignKey(p => p.CriminalRecordFileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PswUser>()
                .HasOne(p => p.FirstAidOrCPRFile)
                .WithMany()
                .HasForeignKey(p => p.FirstAidOrCPRFileId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // JopRequest → Items
            // =========================================================
            modelBuilder.Entity<JopRequest>()
                .HasMany(r => r.Items)
                .WithOne(i => i.JopRequest)
                .HasForeignKey(i => i.JopRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // JobRequestItem → OfferShift
            // =========================================================
            modelBuilder.Entity<JobRequestItem>()
                .HasOne(i => i.OfferShift)
                .WithMany()
                .HasForeignKey(i => i.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            //Updates

            // =========================================================
            // JobRequest → JobOffer
            // =========================================================
            modelBuilder.Entity<JopRequest>()
                .HasOne(r => r.JobOffer)
                .WithMany()
                .HasForeignKey(r => r.JobOfferId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // OfferShift → AssignedPsw
            // =========================================================
            modelBuilder.Entity<OfferShift>()
                .HasOne(s => s.AssignedPsw)
                .WithMany()
                .HasForeignKey(s => s.AssignedPswId)
                .OnDelete(DeleteBehavior.SetNull);


        }

    }
}