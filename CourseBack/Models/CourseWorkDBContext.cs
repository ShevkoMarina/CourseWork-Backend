using System;
using CourseBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CourseBack
{
    public partial class CourseWorkDBContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<SavedItem> Items { get; set; }

        public CourseWorkDBContext()
        {
        }

        public CourseWorkDBContext(DbContextOptions<CourseWorkDBContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:course-work-db-server.database.windows.net,1433;Initial Catalog=CourseWorkDB;User ID=mnshevko;Password=Kfdhtynbq1;Trusted_Connection=False;Encrypt=True;");
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
