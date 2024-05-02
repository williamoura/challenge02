using Microsoft.EntityFrameworkCore;
using Challenge02.Domain.Models;

namespace Challenge02.Infraestructure.Repository
{
    public class DesafioContext : DbContext
    {
        public DesafioContext(DbContextOptions<DesafioContext> options)
            : base(options)
        {
        }

        public DbSet<Dev> Devs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dev>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Dev");
            });
        }
    }
}