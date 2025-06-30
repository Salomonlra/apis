namespace RTKlog_ping.DataContext.cs
{
    using Microsoft.EntityFrameworkCore;
    using RTKlog_ping.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PingResult> PingResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PingResult>().HasKey(p => p.ID_Consecutive);
            modelBuilder.Entity<PingResult>().ToTable("log_PingResult");
        }
    }

}
