using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CourseBack.Models
{
    public partial class CourseWorkDatabaseContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public CourseWorkDatabaseContext()
        {
        }

        public CourseWorkDatabaseContext(DbContextOptions<CourseWorkDatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // разобраться с этим
                optionsBuilder.UseSqlServer("Server=course-work-db-server.database.windows.net;Database=CourseWorkDatabase;Trusted_Connection=False;Encrypt=True;User ID=mnshevko;Password=Kfdhtynbq1;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
