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
    [Migration("20190810153344_add-SolutionColumnEntity")]
    partial class addSolutionColumnEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int>("TaskId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("SolutionColumn");
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

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("DbTemplateId");

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

            modelBuilder.Entity("MasterRad.Entities.SolutionColumnEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.TaskEntity", "Task")
                        .WithMany("SolutionColumns")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MasterRad.Entities.TaskEntity", b =>
                {
                    b.HasOne("MasterRad.Entities.DbTemplateEntity", "Template")
                        .WithMany("Tasks")
                        .HasForeignKey("DbTemplateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}