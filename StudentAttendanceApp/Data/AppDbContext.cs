using Microsoft.EntityFrameworkCore;
using StudentAttendanceApp.Models;

namespace StudentAttendanceApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Branch> Branches { get; set; }
            public DbSet<Track> Tracks { get; set; }
            public DbSet<StudentGroup> StudentGroups { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<Role> Roles { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // 🛡️ لحل مشاكل Cascade Delete
                modelBuilder.Entity<Track>()
                    .HasOne(t => t.Branch)
                    .WithMany(b => b.Tracks)
                    .HasForeignKey(t => t.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<StudentGroup>()
                    .HasOne(g => g.Track)
                    .WithMany(t => t.Groups)
                    .HasForeignKey(g => g.TrackId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<User>()
                    .HasOne(u => u.Branch)
                    .WithMany(b => b.Users)
                    .HasForeignKey(u => u.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }


