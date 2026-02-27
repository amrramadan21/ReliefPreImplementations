using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Entities;
using Relief.Domain.Entities.Users;
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


            // carehome relation with user
            modelBuilder.Entity<ApplicationUser>()
                        .HasOne(u => u.CareHomeUser)
                        .WithOne(c => c.ApplicationUser)
                        .HasForeignKey<CareHomeUser>(c => c.Id)
                        .OnDelete(DeleteBehavior.Cascade);

            // individual carehome relation with user
            modelBuilder.Entity<ApplicationUser>()
                        .HasOne(u => u.IndividualCareHomeUser)
                        .WithOne(i => i.ApplicationUser)
                        .HasForeignKey<IndividualCareHomeUser>(i => i.Id)
                        .OnDelete(DeleteBehavior.Cascade);

            // psw relation with user
            modelBuilder.Entity<ApplicationUser>()
                        .HasOne(u => u.PswUser)
                        .WithOne(p => p.ApplicationUser)
                        .HasForeignKey<PswUser>(p => p.Id)
                        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<JobOffer>()
                        .Property(o => o.HourlyRate)
                         .HasPrecision(10, 2);

            // carehome -> JobOffer
            modelBuilder.Entity<CareHomeUser>()
                .HasMany(c => c.JobOffers)
                .WithOne(j => j.CareHomeUser)
                .HasForeignKey(j => j.CareHomeId)
                .OnDelete(DeleteBehavior.Restrict);

            // individual carehome -> JobOffer
            modelBuilder.Entity<IndividualCareHomeUser>()
                .HasMany(i => i.JobOffers)
                .WithOne(j => j.IndividualCareHomeUser)
                .HasForeignKey(j => j.IndividualId)
                .OnDelete(DeleteBehavior.Restrict);

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
