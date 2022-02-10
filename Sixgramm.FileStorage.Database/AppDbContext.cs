using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Sixgramm.FileStorage.Database.Models;
using Sixgramm.FileStorage.Database.TablesConfiguration;

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
            modelBuilder.ApplyConfiguration(new FileModelsConfiguration());
        }
    }
}