﻿// <auto-generated />
using System;
using GGStream.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GGStream.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("GGStream.Models.Collection", b =>
                {
                    b.Property<string>("URL")
                        .HasColumnType("TEXT");

                    b.Property<string>("BaseColor")
                        .HasColumnType("TEXT");

                    b.Property<string>("CallLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .HasColumnType("TEXT");

                    b.Property<int>("InstructionType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Private")
                        .HasColumnType("INTEGER");

                    b.HasKey("URL");

                    b.ToTable("Collection");
                });

            modelBuilder.Entity("GGStream.Models.Stream", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CollectionURL")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("StreamKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CollectionURL");

                    b.ToTable("Stream");
                });

            modelBuilder.Entity("GGStream.Models.Stream", b =>
                {
                    b.HasOne("GGStream.Models.Collection", "Collection")
                        .WithMany("Streams")
                        .HasForeignKey("CollectionURL");
                });
#pragma warning restore 612, 618
        }
    }
}
