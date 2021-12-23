using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Sixgramm.FileStorage.Database.Models;

namespace Sixgramm.FileStorage.Database
{
    public class AppDbContext:DbContext
    {
        public DbSet<FileModel> FileModels { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<FileModel>(file =>
            {
                file.Property(f => f.Name).IsRequired().HasMaxLength(50);
                file.Property(f => f.Path).IsRequired().HasMaxLength(100);
                file.Property(f => f.Length).IsRequired();
                file.Property(f => f.Types).IsRequired().HasMaxLength(30);
                file.Property(f => f.DateCreated).IsRequired();

            });
        }
    }
}