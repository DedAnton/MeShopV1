﻿// <auto-generated />
using System;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241101001458_AddOrders")]
    partial class AddOrders
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Infrastructure.Database.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.IdentityUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users", "infra");
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Item", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Quantity")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "ProductId");

                    b.ToTable("Items", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Line", b =>
                {
                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<double>("UnitPrice")
                        .HasColumnType("float");

                    b.HasKey("OrderId", "ProductId");

                    b.ToTable("Lines", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Orders", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Payments", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Upc")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("Upc")
                        .IsUnique();

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Shipment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Carrier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TrackNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Shipments", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Unit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ReservedForOrderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("ReservedForOrderId");

                    b.ToTable("Units", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Category", b =>
                {
                    b.HasOne("Infrastructure.Database.Entities.Category", null)
                        .WithMany("Subcategories")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Line", b =>
                {
                    b.HasOne("Infrastructure.Database.Entities.Order", null)
                        .WithMany("OrderLines")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Payment", b =>
                {
                    b.HasOne("Infrastructure.Database.Entities.Order", null)
                        .WithOne("Payment")
                        .HasForeignKey("Infrastructure.Database.Entities.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Product", b =>
                {
                    b.HasOne("Infrastructure.Database.Entities.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.OwnsOne("Infrastructure.Database.Entities.Dimentions", "Dimentions", b1 =>
                        {
                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("Depth")
                                .HasColumnType("float");

                            b1.Property<double>("Height")
                                .HasColumnType("float");

                            b1.Property<double>("Weight")
                                .HasColumnType("float");

                            b1.Property<double>("Width")
                                .HasColumnType("float");

                            b1.HasKey("ProductId");

                            b1.ToTable("Products");

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.Navigation("Category");

                    b.Navigation("Dimentions")
                        .IsRequired();
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Shipment", b =>
                {
                    b.HasOne("Infrastructure.Database.Entities.Order", null)
                        .WithOne("Shipment")
                        .HasForeignKey("Infrastructure.Database.Entities.Shipment", "OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.OwnsOne("Infrastructure.Database.Entities.PostalAddress", "Address", b1 =>
                        {
                            b1.Property<Guid>("ShipmentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("AddressLine1")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("AddressLine2")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipments");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.OwnsOne("Infrastructure.Database.Entities.Recipient", "Recipient", b1 =>
                        {
                            b1.Property<Guid>("ShipmentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Phone")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipments");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Recipient")
                        .IsRequired();
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Unit", b =>
                {
                    b.HasOne("Infrastructure.Database.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Infrastructure.Database.Entities.Order", "Order")
                        .WithMany()
                        .HasForeignKey("ReservedForOrderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Category", b =>
                {
                    b.Navigation("Subcategories");
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Order", b =>
                {
                    b.Navigation("OrderLines");

                    b.Navigation("Payment");

                    b.Navigation("Shipment");
                });
#pragma warning restore 612, 618
        }
    }
}
