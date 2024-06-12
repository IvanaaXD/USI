using LangLang.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LangLang.Data
{
    internal class AppDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public AppDbContext() { }
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Host=localhost;Database=usi_data;Username=postgres;Password=1234";
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ExamTerm> ExamTerms { get; set; }
    }
}
