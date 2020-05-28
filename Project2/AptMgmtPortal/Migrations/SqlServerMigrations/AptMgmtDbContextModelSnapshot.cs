﻿// <auto-generated />
using System;
using AptMgmtPortal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AptMgmtPortal.Migrations.SqlServerMigrations
{
    [DbContext(typeof(AptMgmtDbContext))]
    partial class AptMgmtDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4");

            modelBuilder.Entity("AptMgmtPortal.Entity.BillingPeriod", b =>
                {
                    b.Property<int>("BillingPeriodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PeriodEnd")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PeriodStart")
                        .HasColumnType("TEXT");

                    b.HasKey("BillingPeriodId");

                    b.ToTable("BillingPeriods");
                });

            modelBuilder.Entity("AptMgmtPortal.Entity.MaintenanceRequest", b =>
                {
                    b.Property<int>("MaintenanceRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CloseReason")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ClosingUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("InternalNotes")
                        .HasColumnType("TEXT");

                    b.Property<string>("MaintenanceRequestType")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpenNotes")
                        .HasColumnType("TEXT");

                    b.Property<int>("OpeningUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ResolutionNotes")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("TimeClosed")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeOpened")
                        .HasColumnType("TEXT");

                    b.Property<string>("UnitNumber")
                        .HasColumnType("TEXT");

                    b.HasKey("MaintenanceRequestId");

                    b.ToTable("MaintenanceRequests");
                });

            modelBuilder.Entity("AptMgmtPortal.Entity.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Amount")
                        .HasColumnType("REAL");

                    b.Property<int>("BillingPeriodId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ResourceType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TenantId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimePaid")
                        .HasColumnType("TEXT");

                    b.HasKey("PaymentId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("AptMgmtPortal.Entity.ResourceUsageRate", b =>
                {
                    b.Property<int>("ResourceUsageRateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PeriodEnd")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PeriodStart")
                        .HasColumnType("TEXT");

                    b.Property<double>("Rate")
                        .HasColumnType("REAL");

                    b.Property<int>("ResourceType")
                        .HasColumnType("INTEGER");

                    b.HasKey("ResourceUsageRateId");

                    b.ToTable("ResourceUsageRates");
                });

            modelBuilder.Entity("AptMgmtPortal.Entity.Tenant", b =>
                {
                    b.Property<int>("TenantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TenantId");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("AptMgmtPortal.Entity.TenantResourceUsage", b =>
                {
                    b.Property<int>("TenantResourceUsageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ResourceType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("SampleTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("TenantId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("UsageAmount")
                        .HasColumnType("REAL");

                    b.HasKey("TenantResourceUsageId");

                    b.ToTable("TenantResourceUsages");
                });

            modelBuilder.Entity("AptMgmtPortal.Entity.Unit", b =>
                {
                    b.Property<int>("UnitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TenantId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UnitNumber")
                        .HasColumnType("TEXT");

                    b.HasKey("UnitId");

                    b.HasIndex("UnitNumber")
                        .IsUnique();

                    b.ToTable("Units");
                });

            modelBuilder.Entity("AptMgmtPortal.Entity.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApiKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserAccountType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
