using Models;
using Microsoft.EntityFrameworkCore;
using CrashAnalytics.Models;

namespace Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {        }

        public DbSet<ProjectDTO> Projects { get; set; }    
        public DbSet<CrashDTO> Crashes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");
            
            modelBuilder.Entity<CrashDTO>(entity => {
              
                entity
                .HasOne(t => t.Project)
                .WithMany(d => d.Crashes)             
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Type)                
                .HasConversion(typeof(string));

                entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ProjectDTO>(entity => {

                entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();

                entity.Property(p => p.Name)
                .IsRequired();
            });

            modelBuilder.HasPostgresEnum<CrashDTO.DeviceType>();

        }
        
    }

}