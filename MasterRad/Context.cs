using MasterRad.Entities;
using MasterRad.SeedData;
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

        public DbSet<ExceptionLogEntity> ExceptionLog { get; set; }
        public DbSet<TemplateEntity> Templates { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<SolutionColumnEntity> SolutionColums { get; set; }
        public DbSet<AzureSqlUserMapEntity> AzureSqlUserMap { get; set; }
        public DbSet<SynthesisTestEntity> SynthesisTests { get; set; }
        public DbSet<SynthesisTestStudentEntity> SynthesysTestStudents { get; set; }
        public DbSet<SynthesisEvaluationEntity> SynthesysTestEvaluations { get; set; }
        public DbSet<AnalysisTestEntity> AnalysisTests { get; set; }
        public DbSet<AnalysisTestStudentEntity> AnalysisTestStudents { get; set; }
        public DbSet<AnalysisEvaluationEntity> AnalysisTestEvaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExceptionLogEntity>().ToTable("ExceptionLog");
            modelBuilder.Entity<RequestLogEntity>().ToTable("RequestLog");
            modelBuilder.Entity<TemplateEntity>().ToTable("Template");
            modelBuilder.Entity<TaskEntity>().ToTable("Task");
            modelBuilder.Entity<SolutionColumnEntity>().ToTable("SolutionColumn");
            modelBuilder.Entity<AzureSqlUserMapEntity>().ToTable("AzureSqlUserMap");
            modelBuilder.Entity<SynthesisTestEntity>().ToTable("SynthesisTest");
            modelBuilder.Entity<SynthesisTestStudentEntity>().ToTable("SynthesisTestStudent");
            modelBuilder.Entity<SynthesisEvaluationEntity>().ToTable("SynthesisEvaluation");
            modelBuilder.Entity<AnalysisTestEntity>().ToTable("AnalysisTest");
            modelBuilder.Entity<AnalysisTestStudentEntity>().ToTable("AnalysisTestStudent");
            modelBuilder.Entity<AnalysisEvaluationEntity>().ToTable("AnalysisEvaluation");

            //seed data - DEV ONLY (comment out for production)

            #region Seed_Data
            //modelBuilder.Entity<AzureSqlUserMapEntity>() 
            //    .HasData(SeedHelper.SeedData<AzureSqlUserMapEntity>("students"));

            //modelBuilder.Entity<TemplateEntity>()
            //   .HasData(SeedHelper.SeedData<TemplateEntity>("templates"));

            //modelBuilder.Entity<TaskEntity>()
            //    .HasData(SeedHelper.SeedData<TaskEntity>("tasks"));
            #endregion

            //unique constraints
            modelBuilder.Entity<TemplateEntity>()
               .HasIndex(s => s.Name)
               .IsUnique();

            modelBuilder.Entity<TaskEntity>()
               .HasIndex(s => s.Name)
               .IsUnique();

            modelBuilder.Entity<SynthesisTestEntity>()
               .HasIndex(s => s.Name)
               .IsUnique();

            modelBuilder.Entity<AnalysisTestEntity>()
              .HasIndex(s => s.Name)
              .IsUnique();

            modelBuilder.Entity<AzureSqlUserMapEntity>()
              .HasIndex(s => s.SqlUsername)
              .IsUnique();

            //delete template
            modelBuilder.Entity<TemplateEntity>()
                .HasMany(tm => tm.Tasks)
                .WithOne(ts => ts.Template)
                .HasForeignKey(ts => ts.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //delete task (SynthesisTest)
            modelBuilder.Entity<TaskEntity>()
                .HasMany(ts => ts.SynthesisTests)
                .WithOne(st => st.Task)
                .HasForeignKey(st => st.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //delete task (SolutionColumn)
            modelBuilder.Entity<TaskEntity>()
                .HasMany(ts => ts.SolutionColumns)
                .WithOne(sc => sc.Task)
                .HasForeignKey(sc => sc.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            //SynthesisTestStudentEntity: SynthesisTestEntity-many2many-StudentEntity
            modelBuilder.Entity<SynthesisTestStudentEntity>()
                .HasKey(sts => new { sts.StudentId, sts.SynthesisTestId });

            modelBuilder.Entity<SynthesisTestStudentEntity>()
                .HasOne(sts => sts.SynthesisTest)
                .WithMany(st => st.SynthesisTestStudents)
                .HasForeignKey(sts => sts.SynthesisTestId);

            modelBuilder.Entity<SynthesisTestStudentEntity>()
                .HasOne(sts => sts.Student)
                .WithMany(s => s.SynthesisTestStudents)
                .HasForeignKey(sts => sts.StudentId);

            //delete SynthesisTest
            modelBuilder.Entity<SynthesisTestEntity>()
                .HasMany(st => st.SynthesisTestStudents)
                .WithOne(sts => sts.SynthesisTest)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //delete SynthesisTestStudentEntity:
            modelBuilder.Entity<SynthesisTestStudentEntity>()
                .HasMany(sts => sts.AnalysisTests)
                .WithOne(at => at.SynthesisTestStudent)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //AnalysisTestStudentEntity: AnalysisTest-many2many-StudentEntity
            modelBuilder.Entity<AnalysisTestStudentEntity>()
                .HasKey(sts => new { sts.StudentId, sts.AnalysisTestId });

            modelBuilder.Entity<AnalysisTestStudentEntity>()
                .HasOne(sts => sts.AnalysisTest)
                .WithMany(st => st.AnalysisTestStudents)
                .HasForeignKey(sts => sts.AnalysisTestId);

            modelBuilder.Entity<AnalysisTestStudentEntity>()
                .HasOne(sts => sts.Student)
                .WithMany(s => s.AnalysisTestStudents)
                .HasForeignKey(sts => sts.StudentId);

            //delete SynthesisTest
            modelBuilder.Entity<AnalysisTestEntity>()
                .HasMany(at => at.AnalysisTestStudents)
                .WithOne(ats => ats.AnalysisTest)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //delete student
            modelBuilder.Entity<AzureSqlUserMapEntity>()
                .HasMany(s => s.SynthesisTestStudents)
                .WithOne(sps => sps.Student)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AzureSqlUserMapEntity>()
                .HasMany(s => s.AnalysisTestStudents)
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
