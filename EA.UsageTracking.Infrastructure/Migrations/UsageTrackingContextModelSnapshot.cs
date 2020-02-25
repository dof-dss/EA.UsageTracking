﻿// <auto-generated />
using System;
using EA.UsageTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EA.UsageTracking.Infrastructure.Migrations
{
    [DbContext(typeof(UsageTrackingContext))]
    partial class UsageTrackingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EA.UsageTracking.Core.Entities.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("EA.UsageTracking.Core.Entities.ApplicationEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.ToTable("ApplicationEvents");
                });

            modelBuilder.Entity("EA.UsageTracking.Core.Entities.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.ToTable("ApplicationUsers");
                });

            modelBuilder.Entity("EA.UsageTracking.Core.Entities.UsageItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ApplicationEventId")
                        .HasColumnType("int");

                    b.Property<int?>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<int?>("ApplicationUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationEventId");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("UsageItems");
                });

            modelBuilder.Entity("EA.UsageTracking.Core.Entities.ApplicationEvent", b =>
                {
                    b.HasOne("EA.UsageTracking.Core.Entities.Application", null)
                        .WithMany("ApplicationEvents")
                        .HasForeignKey("ApplicationId");
                });

            modelBuilder.Entity("EA.UsageTracking.Core.Entities.ApplicationUser", b =>
                {
                    b.HasOne("EA.UsageTracking.Core.Entities.Application", null)
                        .WithMany("ApplicationUsers")
                        .HasForeignKey("ApplicationId");
                });

            modelBuilder.Entity("EA.UsageTracking.Core.Entities.UsageItem", b =>
                {
                    b.HasOne("EA.UsageTracking.Core.Entities.ApplicationEvent", "ApplicationEvent")
                        .WithMany()
                        .HasForeignKey("ApplicationEventId");

                    b.HasOne("EA.UsageTracking.Core.Entities.Application", "Application")
                        .WithMany()
                        .HasForeignKey("ApplicationId");

                    b.HasOne("EA.UsageTracking.Core.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");
                });
#pragma warning restore 612, 618
        }
    }
}
