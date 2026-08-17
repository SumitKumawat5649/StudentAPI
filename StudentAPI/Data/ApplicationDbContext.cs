using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;
using System.Collections.Generic;

namespace StudentAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Course> Course { get; set; }
        public DbSet<Exam> Exam { get; set; }
        public DbSet<Mark> Mark { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         
            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Student)
                .WithMany()
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Exam)
                .WithMany()
                .HasForeignKey(m => m.ExamId)
                .OnDelete(DeleteBehavior.NoAction); 
        }

    }
}