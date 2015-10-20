using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

using Domain.Helpers;
using Domain.Entities;
using Domain.Abstract;

namespace Domain.Contexts
{
    public class InoDriveContext : IdentityDbContext<User>
    {
        public InoDriveContext()
            : base("InoDrive")
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourProfile> TourProfiles { get; set; }
        public DbSet<WayPoint> WayPoints { get; set; }
       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }
}
