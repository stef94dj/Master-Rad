﻿// <auto-generated />
using System;
using MasterRad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MasterRad.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20190811161824_initial-model")]
    partial class initialmodel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MasterRad.Entities.AnalysisPaperEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int>("SPS_StudentId");

                    b.Property<int>("SPS_SynthesisPaperId");

                    b.Property<string>("SqlScript");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("SPS_SynthesisPaperId", "SPS_StudentId")
                        .IsUnique();

                    b.ToTable("AnalysisPaper");
                });

            modelBuilder.Entity("MasterRad.Entities.DbTemplateEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("ModelDescription");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("NameOnServer")
                        .HasMaxLength(200);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.ToTable("DbTemplate");
                });

            modelBuilder.Entity("MasterRad.Entities.SolutionColumnEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ColumnName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<int>("TaskId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("SolutionColumn");
                });

            modelBuilder.Entity("MasterRad.Entities.StudentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisPaperEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int>("STS_StudentId");

                    b.Property<int>("STS_SynthesisTestId");

                    b.Property<string>("SqlScript");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("STS_SynthesisTestId", "STS_StudentId")
                        .IsUnique();

                    b.ToTable("SynthesisPaper");
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisPaperStudentEntity", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("SynthesisPaperId");

                    b.HasKey("StudentId", "SynthesisPaperId");

                    b.HasIndex("SynthesisPaperId");

                    b.ToTable("SynthesisPaperStudent");
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisTestEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<bool>("IsActive");

                    b.Property<string>("ModifiedBy");

                    b.Property<int>("TaskId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("SynthesisTest");
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisTestStudentEntity", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("SynthesisTestId");

                    b.HasKey("StudentId", "SynthesisTestId");

                    b.HasIndex("SynthesisTestId");

                    b.ToTable("SynthesisTestStudent");
                });

            modelBuilder.Entity("MasterRad.Entities.TaskEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<int>("DbTemplateId");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDataSet");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("NameOnServer");

                    b.Property<string>("SolutionSqlScript");

                    b.Property<int?>("TemplateId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("TemplateId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("MasterRad.Entities.UnhandledExceptionLogEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<string>("Cookies");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Exception");

                    b.Property<string>("Headers");

                    b.Property<int>("LogMethod");

                    b.Property<string>("Method");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Path");

                    b.Property<string>("PathBase");

                    b.Property<string>("Protocol");

                    b.Property<string>("Query");

                    b.Property<string>("QueryString");

                    b.Property<string>("SerializeError");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.ToTable("UnhandledExceptionLog");
                });

            modelBuilder.Entity("MasterRad.Entities.AnalysisPaperEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.SynthesisPaperStudentEntity", "SynthesisPaperStudent")
                        .WithOne("AnalysisPaper")
                        .HasForeignKey("MasterRad.Entities.AnalysisPaperEntity", "SPS_SynthesisPaperId", "SPS_StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MasterRad.Entities.SolutionColumnEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.TaskEntity", "Task")
                        .WithMany("SolutionColumns")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisPaperEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.SynthesisTestStudentEntity", "SynthesisTestStudent")
                        .WithOne("SynthesisPaper")
                        .HasForeignKey("MasterRad.Entities.SynthesisPaperEntity", "STS_SynthesisTestId", "STS_StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisPaperStudentEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.StudentEntity", "Student")
                        .WithMany("SynthesisPaperStudents")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MasterRad.Entities.SynthesisPaperEntity", "SynthesisPaper")
                        .WithMany("SynthesisPaperStudents")
                        .HasForeignKey("SynthesisPaperId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisTestEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.TaskEntity", "Task")
                        .WithMany("SynthesisTests")
                        .HasForeignKey("TaskId");
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisTestStudentEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.StudentEntity", "Student")
                        .WithMany("SynthesisTestStudents")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MasterRad.Entities.SynthesisTestEntity", "SynthesisTest")
                        .WithMany("SynthesisTestStudents")
                        .HasForeignKey("SynthesisTestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MasterRad.Entities.TaskEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.DbTemplateEntity", "Template")
                        .WithMany("Tasks")
                        .HasForeignKey("TemplateId");
                });
#pragma warning restore 612, 618
        }
    }
}
