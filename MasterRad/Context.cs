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
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<SynthesisTestEntity> SynthesisTests { get; set; }
        public DbSet<SynthesisTestStudentEntity> SynthesysTestStudents { get; set; }
        public DbSet<SynthesisPaperEntity> SynthesisPapers { get; set; }
        public DbSet<SynthesisPaperStudentEntity> AnalysisTestStudents { get; set; }
        public DbSet<AnalysisPaperEntity> AnalysisPapers { get; set; }
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnhandledExceptionLogEntity>().ToTable("UnhandledExceptionLog");
            modelBuilder.Entity<DbTemplateEntity>().ToTable("DbTemplate");
            modelBuilder.Entity<TaskEntity>().ToTable("Task");
            modelBuilder.Entity<SolutionColumnEntity>().ToTable("SolutionColumn");
            modelBuilder.Entity<StudentEntity>().ToTable("Student");
            modelBuilder.Entity<SynthesisTestEntity>().ToTable("SynthesisTest");
            modelBuilder.Entity<SynthesisTestStudentEntity>().ToTable("SynthesisTestStudent");
            modelBuilder.Entity<SynthesisPaperEntity>().ToTable("SynthesisPaper");
            modelBuilder.Entity<SynthesisPaperStudentEntity>().ToTable("SynthesisPaperStudent");
            modelBuilder.Entity<AnalysisPaperEntity>().ToTable("AnalysisPaper");

            //delete template
            modelBuilder.Entity<DbTemplateEntity>()
                .HasMany(tm => tm.Tasks)
                .WithOne(ts => ts.Template)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //delete task
            modelBuilder.Entity<TaskEntity>()
                .HasMany(ts => ts.SynthesisTests)
                .WithOne(st => st.Task)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //SynthesisTestStudentEntity: SynthesisTestEntity-many2many-StudentEntity
            modelBuilder.Entity<SynthesisTestStudentEntity>()
                .HasKey(sts => new { sts.StudentId, sts.SynthesisTestId });

            modelBuilder.Entity<SynthesisTestStudentEntity>()
                .HasOne(sts => sts.SynthesisTest)
                .WithMany(st => st.SynthesisTestStudents)
                .HasForeignKey(sts => sts.SynthesisTestId);

            modelBuilder.Entity<SynthesisTestStudentEntity>()
                .HasOne(sts => sts.Student)
                .WithMany()
                .HasForeignKey(sts => sts.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull); 

            //SynthesisPaperStudentEntity: SynthesisPaperEntity-many2many-StudentEntity
            modelBuilder.Entity<SynthesisPaperStudentEntity>()
                .HasKey(sts => new { sts.StudentId, sts.SynthesisPaperId });

            modelBuilder.Entity<SynthesisPaperStudentEntity>()
                .HasOne(sts => sts.SynthesisPaper)
                .WithMany(st => st.SynthesisPaperStudents)
                .HasForeignKey(sts => sts.SynthesisPaperId);

            modelBuilder.Entity<SynthesisPaperStudentEntity>()
                .HasOne(sts => sts.Student)
                .WithMany()
                .HasForeignKey(sts => sts.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //delete student
            modelBuilder.Entity<StudentEntity>()
                .HasMany(s => s.SynthesisPaperStudents)
                .WithOne(sps => sps.Student)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentEntity>()
                .HasMany(s => s.SynthesisTestStudents)
                .WithOne(sts => sts.Student)
                .OnDelete(DeleteBehavior.Restrict);
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
