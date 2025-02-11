using CvBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CvBuilder.Data
{
    public class CvBuilderContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Language> Languages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Read connection string from appsettings.json
            var path = Path.Combine(AppContext.BaseDirectory, "Configuration/appsettings.json");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Ensures correct base directory
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Resumes)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.WorkExperiences)
                .WithOne(we => we.Resume)
                .HasForeignKey(we => we.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Educations)
                .WithOne(e => e.Resume)
                .HasForeignKey(e => e.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Skills)
                .WithOne(s => s.Resume)
                .HasForeignKey(s => s.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Languages)
                .WithOne(l => l.Resume)
                .HasForeignKey(l => l.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}