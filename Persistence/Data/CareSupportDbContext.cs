using DomainLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersistenceLayer.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceLayer.Data
{
    public class CareSupportDbContext : IdentityDbContext<ApplicationUser>
    {
        public CareSupportDbContext(DbContextOptions<CareSupportDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<PSWProfile> PSWProfiles { get; set; }
        public DbSet<JobOffer> JobOffers { get; set; }
        public DbSet<JobRequest> JobRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PSWProfile>()
                .Property(p => p.HourlyRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<JobOffer>()
                .Property(o => o.HourlyRate)
                .HasPrecision(18, 2);
        }
    }
}
