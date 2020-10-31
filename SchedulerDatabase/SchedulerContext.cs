using System;
using Microsoft.EntityFrameworkCore;
using SchedulerDatabase.Models;

namespace SchedulerDatabase
{
    /// <summary>
    /// <see cref="SchedulerContext"/> provides a context for accessing the database of available profiles to use while scheduling a mission.
    /// </summary>
    public class SchedulerContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerContext"/> class.
        /// </summary>
        /// <remarks>Provided for EF Core Migration generation.</remarks>
        public SchedulerContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerContext"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the database file to open.</param>
        public SchedulerContext(string databaseName)
        {
            this.DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));

            this.Database.Migrate();
        }

        /// <summary>
        /// Gets or sets the <see cref="AESProfiles"/>s stored in the database.
        /// </summary>
        public DbSet<AESEncryptorProfile> AESProfiles { get; set; }

        private string DatabaseName { get; set; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={this.DatabaseName}");
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AESEncryptorProfile>().HasKey(m => m.ProfileId);
        }
    }
}
