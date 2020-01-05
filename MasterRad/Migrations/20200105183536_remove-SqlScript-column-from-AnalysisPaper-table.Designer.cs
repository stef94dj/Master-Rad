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
    [Migration("20200105183536_remove-SqlScript-column-from-AnalysisPaper-table")]
    partial class removeSqlScriptcolumnfromAnalysisPapertable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MasterRad.Entities.AnalysisPaperEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ATS_AnalysisTestId");

                    b.Property<int>("ATS_StudentId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("ModifiedBy");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("ATS_StudentId", "ATS_AnalysisTestId")
                        .IsUnique();

                    b.ToTable("AnalysisPaper");
                });

            modelBuilder.Entity("MasterRad.Entities.AnalysisTestEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("Status");

                    b.Property<int>("SynthesisPaperId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("SynthesisPaperId");

                    b.ToTable("AnalysisTest");
                });

            modelBuilder.Entity("MasterRad.Entities.AnalysisTestStudentEntity", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("AnalysisTestId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("NameOnServer")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("StudentId", "AnalysisTestId");

                    b.HasIndex("AnalysisTestId");

                    b.ToTable("AnalysisTestStudent");
                });

            modelBuilder.Entity("MasterRad.Entities.ExceptionLogEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("Exception");

                    b.Property<int>("LogMethod");

                    b.Property<string>("SerializeError");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.ToTable("ExceptionLog");
                });

            modelBuilder.Entity("MasterRad.Entities.RequestLogEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<string>("Cookies");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<int>("ExceptionLogId");

                    b.Property<string>("Headers");

                    b.Property<string>("Method");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Path");

                    b.Property<string>("PathBase");

                    b.Property<string>("Protocol");

                    b.Property<string>("Query");

                    b.Property<string>("QueryString");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("ExceptionLogId");

                    b.ToTable("RequestLog");
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
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("FirstName")
                        .HasMaxLength(255);

                    b.Property<string>("LastName")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Student");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "stud1@student.etf.bg.ac.rs",
                            FirstName = "John",
                            LastName = "Smith"
                        },
                        new
                        {
                            Id = 2,
                            Email = "stud2@student.etf.bg.ac.rs",
                            FirstName = "Jane",
                            LastName = "Rogers"
                        },
                        new
                        {
                            Id = 3,
                            Email = "stud3@student.etf.bg.ac.rs",
                            FirstName = "John",
                            LastName = "Rogers"
                        },
                        new
                        {
                            Id = 4,
                            Email = "stud4@student.etf.bg.ac.rs",
                            FirstName = "Jane",
                            LastName = "Smith"
                        },
                        new
                        {
                            Id = 5,
                            Email = "stud5@student.etf.bg.ac.rs"
                        },
                        new
                        {
                            Id = 6,
                            Email = "stud6@student.etf.bg.ac.rs"
                        },
                        new
                        {
                            Id = 7,
                            Email = "stud7@student.etf.bg.ac.rs"
                        });
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

                    b.Property<string>("PublicDataEvaluationInfo");

                    b.Property<int>("PublicDataEvaluationStatus");

                    b.Property<int>("STS_StudentId");

                    b.Property<int>("STS_SynthesisTestId");

                    b.Property<string>("SecretDataEvaluationInfo");

                    b.Property<int>("SecretDataEvaluationStatus");

                    b.Property<string>("SqlScript");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("STS_StudentId", "STS_SynthesisTestId")
                        .IsUnique();

                    b.ToTable("SynthesisPaper");
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisTestEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("Status");

                    b.Property<int>("TaskId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("TaskId");

                    b.ToTable("SynthesisTest");
                });

            modelBuilder.Entity("MasterRad.Entities.SynthesisTestStudentEntity", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("SynthesisTestId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("NameOnServer")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

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

                    b.Property<string>("Description")
                        .HasMaxLength(8191);

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(63);

                    b.Property<string>("NameOnServer")
                        .HasMaxLength(255);

                    b.Property<string>("SolutionSqlScript");

                    b.Property<int>("TemplateId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("TemplateId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("MasterRad.Entities.TemplateEntity", b =>
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
                        .HasMaxLength(63);

                    b.Property<string>("NameOnServer")
                        .HasMaxLength(255);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Template");
                });

            modelBuilder.Entity("MasterRad.Entities.AnalysisPaperEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.AnalysisTestStudentEntity", "AnalysisTestStudent")
                        .WithOne("AnalysisPaper")
                        .HasForeignKey("MasterRad.Entities.AnalysisPaperEntity", "ATS_StudentId", "ATS_AnalysisTestId");
                });

            modelBuilder.Entity("MasterRad.Entities.AnalysisTestEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.SynthesisPaperEntity", "SynthesisPaper")
                        .WithMany("AnalysisTests")
                        .HasForeignKey("SynthesisPaperId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MasterRad.Entities.AnalysisTestStudentEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.AnalysisTestEntity", "AnalysisTest")
                        .WithMany("AnalysisTestStudents")
                        .HasForeignKey("AnalysisTestId");

                    b.HasOne("MasterRad.Entities.StudentEntity", "Student")
                        .WithMany("AnalysisTestStudents")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MasterRad.Entities.RequestLogEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.ExceptionLogEntity", "Exception")
                        .WithMany()
                        .HasForeignKey("ExceptionLogId")
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
                        .HasForeignKey("MasterRad.Entities.SynthesisPaperEntity", "STS_StudentId", "STS_SynthesisTestId");
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
                        .HasForeignKey("SynthesisTestId");
                });

            modelBuilder.Entity("MasterRad.Entities.TaskEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.TemplateEntity", "Template")
                        .WithMany("Tasks")
                        .HasForeignKey("TemplateId");
                });
#pragma warning restore 612, 618
        }
    }
}
