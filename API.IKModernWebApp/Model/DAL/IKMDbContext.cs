using Microsoft.EntityFrameworkCore;

namespace API.IKModernWebApp.Model.DAL
{
    public class IKMDbContext : DbContext
    {
        public IKMDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        { }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyGroup> CompanyGroup { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>();
            modelBuilder.Entity<CompanyGroup>();
        }
    }
}
