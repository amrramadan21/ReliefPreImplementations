using Microsoft.EntityFrameworkCore;
using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presistence.Data.DbContexts
{
    public class ReliefAppDbContext : DbContext
    {
        public ReliefAppDbContext(DbContextOptions<ReliefAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobOffer> JobOffers => Set<JobOffer>();
        public DbSet<OfferDay> OfferDays => Set<OfferDay>();
        public DbSet<Shift> Shifts => Set<Shift>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobOffer>()
                        .Property(o => o.HourlyRate)
                         .HasPrecision(10, 2);

            // JobOffer -> Days
            modelBuilder.Entity<JobOffer>()
                .HasMany(o => o.Days)
                .WithOne(d => d.JobOffer)
                .HasForeignKey(d => d.JobOfferId)
                .OnDelete(DeleteBehavior.Cascade);

            // OfferDay -> Shifts
            modelBuilder.Entity<OfferDay>()
                .HasMany(d => d.Shifts)
                .WithOne(s => s.OfferDay)
                .HasForeignKey(s => s.OfferDayId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
