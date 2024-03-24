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

            modelBuilder.Entity<ProjectDTO>(entity =>
            {
                entity.ToTable("projects");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

                entity.Property(x => x.Name)
                .HasColumnName("name");

                entity.HasIndex(e => e.Name)                
                .IsUnique();

                entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();

            });

            modelBuilder.Entity<CrashDTO>(entity => {

                entity.ToTable("crashes");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

                entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();

                entity.Property(x => x.Version)
                .HasColumnName("version")
                .HasMaxLength(15)
                .IsRequired();

                entity.Property(x => x.Message)
                .HasColumnName("message")
                .IsRequired();

                entity.Property(x => x.Type)
                .HasColumnName("type")
                .HasConversion<int>()
                .IsRequired();

                // Relationship
                entity.Property(x => x.ProjectId)
                .HasColumnName("project_id");

                entity
                   .HasOne(t => t.Project)
                   .WithMany(d => d.Crashes)
                   .HasForeignKey(x => x.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);
            });
                /*
                base.OnModelCreating(modelBuilder);

                modelBuilder.HasPostgresExtension("uuid-ossp");

                modelBuilder.Entity<CrashDTO>(entity => {

                    entity
                    .HasOne(t => t.Project)
                    .WithMany(d => d.Crashes)             
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                    entity.Property(e => e.Type)                
                    .HasConversion(
                        v=> v.ToString(),
                        v=> (CrashDTO.DeviceType)Enum.Parse(typeof(CrashDTO.DeviceType),v));

                    entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("NOW()")
                    .ValueGeneratedOnAdd();
                });

                modelBuilder.Entity<ProjectDTO>(entity => {

                    entity.HasKey(e => e.Id);

                    entity.HasIndex(e => e.Name)
                    .IsUnique();

                    entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("NOW()")
                    .ValueGeneratedOnAdd();

                    entity.Property(p => p.Name)

                    .IsRequired();
                });

                modelBuilder.HasPostgresEnum<CrashDTO.DeviceType>();
                */
            }
        
    }

}