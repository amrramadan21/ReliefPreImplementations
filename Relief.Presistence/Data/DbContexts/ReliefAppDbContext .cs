using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presistence.Data.DbContexts
{
    public class ReliefAppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ReliefAppDbContext(DbContextOptions<ReliefAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobOffer> JobOffers => Set<JobOffer>();
        public DbSet<OfferShift> OfferDays => Set<OfferShift>();
        public DbSet<CareHomeUser> CareHomeUsers => Set<CareHomeUser>();
        public DbSet<IndividualCareHomeUser> IndividualUsers => Set<IndividualCareHomeUser>();
        public DbSet<PswUser> PswUsers => Set<PswUser>();





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobOffer>()
                        .Property(o => o.HourlyRate)
                         .HasPrecision(10, 2);

            // JobOffer -> Days
            modelBuilder.Entity<JobOffer>()
                .HasMany(o => o.Shifts)
                .WithOne(d => d.JobOffer)
                .HasForeignKey(d => d.JobOfferId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(U => U.Address)
                .WithOne(A => A.User)
                .HasForeignKey<ApplicationUser>(U => U.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
