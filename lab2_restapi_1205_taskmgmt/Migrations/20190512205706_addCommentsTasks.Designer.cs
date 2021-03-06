﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using lab2_restapi_1205_taskmgmt.Models;

namespace lab2_restapi_1205_taskmgmt.Migrations
{
    [DbContext(typeof(TasksDbContext))]
    [Migration("20190512205706_addCommentsTasks")]
    partial class addCommentsTasks
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("lab2_restapi_1205_taskmgmt.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Important");

                    b.Property<int?>("TaskId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("lab2_restapi_1205_taskmgmt.Models.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Added");

                    b.Property<DateTime?>("ClosedAt");

                    b.Property<DateTime?>("Deadline");

                    b.Property<string>("Description");

                    b.Property<int>("Importance");

                    b.Property<int>("State");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("lab2_restapi_1205_taskmgmt.Models.Comment", b =>
                {
                    b.HasOne("lab2_restapi_1205_taskmgmt.Models.Task")
                        .WithMany("Comments")
                        .HasForeignKey("TaskId");
                });
#pragma warning restore 612, 618
        }
    }
}
