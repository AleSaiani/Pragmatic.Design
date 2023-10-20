﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pragmatic.Design.Core.Persistence;

#nullable disable

namespace Pragmatic.Design.WebApi.Host.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Pragmatic.Design.ExampleApp.Example1.Domain.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("EntityId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Pragmatic.Design.ExampleApp.Example1.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("DetailId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("ProductDetails", (string)null);
                });

            modelBuilder.Entity("Pragmatic.Design.ExampleApp.Example1.Domain.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductCategory");
                });

            modelBuilder.Entity("Pragmatic.Design.ExampleApp.Example1.Domain.ProductDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EntityId")
                        .IsUnique();

                    b.ToTable("ProductDetail");
                });

            modelBuilder.Entity("ProductProductCategory", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("int");

                    b.Property<int>("EntitiesId")
                        .HasColumnType("int");

                    b.HasKey("CategoriesId", "EntitiesId");

                    b.HasIndex("EntitiesId");

                    b.ToTable("EntityCategories", (string)null);
                });

            modelBuilder.Entity("Pragmatic.Design.ExampleApp.Example1.Domain.Order", b =>
                {
                    b.HasOne("Pragmatic.Design.ExampleApp.Example1.Domain.Product", "Product")
                        .WithMany("Orders")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Pragmatic.Design.ExampleApp.Example1.Domain.ProductDetail", b =>
                {
                    b.HasOne("Pragmatic.Design.ExampleApp.Example1.Domain.Product", "Product")
                        .WithOne("Detail")
                        .HasForeignKey("Pragmatic.Design.ExampleApp.Example1.Domain.ProductDetail", "EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ProductProductCategory", b =>
                {
                    b.HasOne("Pragmatic.Design.ExampleApp.Example1.Domain.ProductCategory", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pragmatic.Design.ExampleApp.Example1.Domain.Product", null)
                        .WithMany()
                        .HasForeignKey("EntitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pragmatic.Design.ExampleApp.Example1.Domain.Product", b =>
                {
                    b.Navigation("Detail")
                        .IsRequired();

                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
