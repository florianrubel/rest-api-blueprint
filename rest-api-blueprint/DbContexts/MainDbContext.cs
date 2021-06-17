using rest_api_blueprint.Entities;
using rest_api_blueprint.Entities.Authentication;
using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Entities.Social;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rest_api_blueprint.DbContexts
{
    public class MainDbContext : IdentityDbContext<User, Role, string>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options): base(options)
        {

        }

        public DbSet<RefreshToken> Authentication_RefreshTokens { get; set; }
        public DbSet<ExternalLoginToken> Authentication_ExternalLoginTokens { get; set; }

        public DbSet<Address> Geo_Addresses { get; set; }

        public DbSet<Announcement> Social_Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Do not allow cascading deletes! This prevents unwanted deletions and loss of data.
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<User>()
                .HasIndex(u => u.FacebookUserId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.GoogleUserId)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            // Handle added entries
            IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> createdEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added));

            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entityEntry in createdEntries)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTimeOffset.UtcNow;
            }

            // Handle updated entries
            IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> updatedEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Modified));

            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entityEntry in updatedEntries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTimeOffset.UtcNow;
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            // Handle added entries
            IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> createdEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added));

            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entityEntry in createdEntries)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTimeOffset.UtcNow;
            }

            // Handle updated entries
            IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> updatedEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Modified));

            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entityEntry in updatedEntries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTimeOffset.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
