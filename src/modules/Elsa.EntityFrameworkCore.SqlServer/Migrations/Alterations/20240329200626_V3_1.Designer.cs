﻿// <auto-generated />
using System;
using Elsa.EntityFrameworkCore.Modules.Alterations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Elsa.EntityFrameworkCore.SqlServer.Migrations.Alterations
{
    [DbContext(typeof(AlterationsElsaDbContext))]
    [Migration("20240329200626_V3_1")]
    partial class V3_1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Elsa")
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Elsa.Alterations.Core.Entities.AlterationJob", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset?>("CompletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("PlanId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SerializedLog")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("StartedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("WorkflowInstanceId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CompletedAt")
                        .HasDatabaseName("IX_AlterationJob_CompletedAt");

                    b.HasIndex("CreatedAt")
                        .HasDatabaseName("IX_AlterationJob_CreatedAt");

                    b.HasIndex("PlanId")
                        .HasDatabaseName("IX_AlterationJob_PlanId");

                    b.HasIndex("StartedAt")
                        .HasDatabaseName("IX_AlterationJob_StartedAt");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_AlterationJob_Status");

                    b.HasIndex("WorkflowInstanceId")
                        .HasDatabaseName("IX_AlterationJob_WorkflowInstanceId");

                    b.ToTable("AlterationJobs", "Elsa");
                });

            modelBuilder.Entity("Elsa.Alterations.Core.Entities.AlterationPlan", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset?>("CompletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("SerializedAlterations")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerializedWorkflowInstanceFilter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("StartedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CompletedAt")
                        .HasDatabaseName("IX_AlterationPlan_CompletedAt");

                    b.HasIndex("CreatedAt")
                        .HasDatabaseName("IX_AlterationPlan_CreatedAt");

                    b.HasIndex("StartedAt")
                        .HasDatabaseName("IX_AlterationPlan_StartedAt");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_AlterationPlan_Status");

                    b.ToTable("AlterationPlans", "Elsa");
                });
#pragma warning restore 612, 618
        }
    }
}
