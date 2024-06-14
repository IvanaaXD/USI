using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace LangLang.Data
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ExamTerm> ExamTerms { get; set; }
        public AppDbContext() {
        }
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .Property(c => c.WorkDays)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), x)).ToList()
                )
                .HasColumnType("text");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.Languages)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => (Language)Enum.Parse(typeof(Language), x)).ToList()
                )
                .HasColumnType("text");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.LevelOfLanguages)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => (LanguageLevel)Enum.Parse(typeof(LanguageLevel), x)).ToList()
                    )
                .HasColumnType("text");
            modelBuilder.Entity<Teacher>()
                .Property(t => t.StartedWork)
                .HasColumnType("date");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.DateOfBirth)
                .HasColumnType("date");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Host=localhost;Database=usi_data;Username=postgres;Password=1234";
                optionsBuilder.UseNpgsql(connectionString);
            }
        }


    }
}
