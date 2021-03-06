﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchedulerDatabase;

namespace SchedulerDatabase.Migrations
{
    [DbContext(typeof(SchedulerContext))]
    [Migration("20201020042628_AddAdditionalUniqueInfoField")]
    partial class AddAdditionalUniqueInfoField
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("SchedulerDatabase.Models.AESEncyptorProfile", b =>
                {
                    b.Property<Guid>("ProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AdditionalUniqueInfo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<double>("AverageCurrent")
                        .HasColumnType("REAL");

                    b.Property<double>("AverageVoltage")
                        .HasColumnType("REAL");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("NumCores")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlatformAccelerator")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlatformName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderName")
                        .HasColumnType("TEXT");

                    b.Property<int>("TestedAESBitLength")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TestedAESMode")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TestedFrequency")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("TotalTestTime")
                        .HasColumnType("TEXT");

                    b.Property<long>("TotalTestedByteSize")
                        .HasColumnType("INTEGER");

                    b.Property<double>("TotalTestedEnergyJoules")
                        .HasColumnType("REAL");

                    b.HasKey("ProfileId");

                    b.ToTable("AESProfiles");
                });
#pragma warning restore 612, 618
        }
    }
}
