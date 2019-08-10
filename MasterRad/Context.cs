using MasterRad.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<UnhandledExceptionLogEntity> UnhandledExceptionLog { get; set; }
        public DbSet<DbTemplateEntity> DbTemplates { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<SolutionColumnEntity> SolutionColums { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnhandledExceptionLogEntity>().ToTable("UnhandledExceptionLog");
            modelBuilder.Entity<DbTemplateEntity>().ToTable("DbTemplate");
            modelBuilder.Entity<TaskEntity>().ToTable("Task");
            modelBuilder.Entity<SolutionColumnEntity>().ToTable("SolutionColumn");
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
