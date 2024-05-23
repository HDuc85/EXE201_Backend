﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Service.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Box> Boxes { get; set; }

    public virtual DbSet<BoxItem> BoxItems { get; set; }

    public virtual DbSet<BoxTag> BoxTags { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }



    public virtual DbSet<Cart> Carts { get; set; }


    public virtual DbSet<Color> Colors { get; set; }


    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackMedia> FeedbackMedia { get; set; }




    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemMedia> ItemMedia { get; set; }

    public virtual DbSet<ItemType> ItemTypes { get; set; }

    public virtual DbSet<MediaType> MediaTypes { get; set; }

    public virtual DbSet<Media> Media { get; set; }






    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderStatusLog> OrderStatusLogs { get; set; }

    public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }


    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductTag> ProductTags { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }


    public virtual DbSet<Role> Roles { get; set; }








    public virtual DbSet<Size> Sizes { get; set; }



    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreMember> StoreMembers { get; set; }


    public virtual DbSet<StoreItem> StoreItems { get; set; }


    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagValue> TagValues { get; set; }


    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserStatusLog> UserStatusLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

     
        modelBuilder.Entity<Box>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("box_pkey");

            entity.ToTable("box");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Auther).HasColumnName("auther");
            entity.Property(e => e.BoxName)
                .HasMaxLength(255)
                .HasColumnName("boxName");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.QuantitySold).HasColumnName("quantitySold");
            entity.Property(e => e.Rate).HasColumnName("rate");

            entity.HasOne(d => d.AutherNavigation).WithMany(p => p.Boxes)
                .HasForeignKey(d => d.Auther)
                .HasConstraintName("box_auther_fkey");
        });

        modelBuilder.Entity<BoxItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("boxItem_pkey");

            entity.ToTable("boxItem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BoxId).HasColumnName("boxId");
            entity.Property(e => e.ProductVariantId).HasColumnName("productVariantId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Box).WithMany(p => p.BoxItems)
                .HasForeignKey(d => d.BoxId)
                .HasConstraintName("boxItem_boxId_fkey");

            entity.HasOne(d => d.ProductVariant).WithMany(p => p.BoxItems)
                .HasForeignKey(d => d.ProductVariantId)
                .HasConstraintName("boxItem_productVariantId_fkey");
        });

        modelBuilder.Entity<BoxTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("boxTag_pkey");

            entity.ToTable("boxTag");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BoxId).HasColumnName("boxId");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.TagVauleId).HasColumnName("tagVauleId");

            entity.HasOne(d => d.Box).WithMany(p => p.BoxTags)
                .HasForeignKey(d => d.BoxId)
                .HasConstraintName("boxTag_boxId_fkey");

            entity.HasOne(d => d.TagVaule).WithMany(p => p.BoxTags)
                .HasForeignKey(d => d.TagVauleId)
                .HasConstraintName("boxTag_tagVauleId_fkey");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brand_pkey");

            entity.ToTable("brand");

            entity.HasIndex(e => e.BrandValue, "brand_brandValue_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandValue)
                .HasMaxLength(255)
                .HasColumnName("brandValue");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
        });

     
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cart_pkey");

            entity.ToTable("cart");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Item).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("cart_itemId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("cart_userId_fkey");
        });

    
        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("color_pkey");

            entity.ToTable("color");

            entity.HasIndex(e => e.ColorValue, "color_colorValue_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ColorValue)
                .HasMaxLength(255)
                .HasColumnName("colorValue");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
        });

     
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Item).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("feedback_itemId_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("feedback_orderId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("feedback_userId_fkey");
        });

        modelBuilder.Entity<FeedbackMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feedbackMedia_pkey");

            entity.ToTable("feedbackMedia");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MediaId).HasColumnName("mediaId");

            entity.HasOne(d => d.Feedback).WithMany(p => p.FeedbackMedia)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("feedbackMedia_feedbackId_fkey");

            entity.HasOne(d => d.Media).WithMany(p => p.FeedbackMedia)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("feedbackMedia_mediaId_fkey");
        });

    
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_pkey");

            entity.ToTable("item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.ItemTypeId).HasColumnName("itemTypeId");

            entity.HasOne(d => d.ItemNavigation).WithMany(p => p.Items)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("item_itemId_fkey1");

            entity.HasOne(d => d.Item1).WithMany(p => p.Items)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("item_itemId_fkey");

            entity.HasOne(d => d.ItemType).WithMany(p => p.Items)
                .HasForeignKey(d => d.ItemTypeId)
                .HasConstraintName("item_itemTypeId_fkey");
        });

        modelBuilder.Entity<ItemMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itemMedia_pkey");

            entity.ToTable("itemMedia");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.MediaId).HasColumnName("mediaId");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemMedia)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("itemMedia_itemId_fkey");

            entity.HasOne(d => d.Media).WithMany(p => p.ItemMedia)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("itemMedia_mediaId_fkey");
        });

        modelBuilder.Entity<ItemType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itemType_pkey");

            entity.ToTable("itemType");

            entity.HasIndex(e => e.Type, "itemType_type_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
        });

        modelBuilder.Entity<MediaType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mediaType_pkey");

            entity.ToTable("mediaType");

            entity.HasIndex(e => e.MediaName, "mediaType_mediaName_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MediaName)
                .HasMaxLength(255)
                .HasColumnName("mediaName");
        });

        modelBuilder.Entity<Media>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("media_pkey");

            entity.ToTable("media");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MediaTypeId)
                .HasColumnType("character varying")
                .HasColumnName("mediaTypeId");
            entity.Property(e => e.MediaUrl)
                .HasMaxLength(255)
                .HasColumnName("mediaUrl");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_pkey");

            entity.ToTable("order");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("order_paymentId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("order_userId_fkey");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("orderItem");

            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Item).WithMany()
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("orderItem_itemId_fkey");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("orderItem_orderId_fkey");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderStatus_pkey");

            entity.ToTable("orderStatus");

            entity.HasIndex(e => e.Status, "orderStatus_status_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
        });

        modelBuilder.Entity<OrderStatusLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderStatusLog_pkey");

            entity.ToTable("orderStatusLog");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LogAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("logAt");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.StatusId).HasColumnName("statusId");
            entity.Property(e => e.TextLog)
                .HasMaxLength(1000)
                .HasColumnName("textLog");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusLogs)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("orderStatusLog_orderId_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.OrderStatusLogs)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("orderStatusLog_statusId_fkey");
        });

        modelBuilder.Entity<PaymentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("paymentDetail_pkey");

            entity.ToTable("paymentDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(255)
                .HasColumnName("accountNumber");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.PaymentLinkId)
                .HasMaxLength(255)
                .HasColumnName("paymentLinkId");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(255)
                .HasColumnName("paymentType");
            entity.Property(e => e.TransactionDateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("transactionDateTime");
        });

 
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Auther).HasColumnName("auther");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("productName");
            entity.Property(e => e.QuantitySold).HasColumnName("quantitySold");
            entity.Property(e => e.Rate).HasColumnName("rate");

            entity.HasOne(d => d.AutherNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Auther)
                .HasConstraintName("product_auther_fkey");
        });

        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productTag_pkey");

            entity.ToTable("productTag");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.TagVauleId).HasColumnName("tagVauleId");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("productTag_productId_fkey");

            entity.HasOne(d => d.TagVaule).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.TagVauleId)
                .HasConstraintName("productTag_tagVauleId_fkey");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productVariant_pkey");

            entity.ToTable("productVariant");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brandId");
            entity.Property(e => e.ColorId).HasColumnName("colorId");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SizeId).HasColumnName("sizeId");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(255)
                .HasColumnName("thumbnail");

            entity.HasOne(d => d.Brand).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("productVariant_brandId_fkey");

            entity.HasOne(d => d.Color).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("productVariant_colorId_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("productVariant_productId_fkey");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("productVariant_sizeId_fkey");
        });

    
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.HasIndex(e => e.Role1, "role_role_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Role1)
                .HasMaxLength(255)
                .HasColumnName("role");
        });


        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("size_pkey");

            entity.ToTable("size");

            entity.HasIndex(e => e.SizeValue, "size_sizeValue_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.SizeValue)
                .HasMaxLength(255)
                .HasColumnName("sizeValue");
        });

 
        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.HasIndex(e => e.Status1, "status_status_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Status1)
                .HasMaxLength(255)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("store_pkey");

            entity.ToTable("store");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Location)
                .HasColumnType("character varying")
                .HasColumnName("location");
            entity.Property(e => e.ProductQuantity).HasColumnName("productQuantity");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.StatusId).HasColumnName("statusId");
            entity.Property(e => e.StoreName)
                .HasMaxLength(255)
                .HasColumnName("storeName");
        });

        modelBuilder.Entity<StoreMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("storeMember_pkey");

            entity.ToTable("storeMember");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.StoreId).HasColumnName("storeId");

         

            entity.HasOne(d => d.User)
           .WithMany(p => p.StoreMembers)
           .HasForeignKey(d => d.MemberId)
           .HasConstraintName("storeMember_memberId_fkey");

            entity.HasOne(d => d.Store).WithMany(p => p.StoreMembers)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("storeMember_storeId_fkey");
        });

        modelBuilder.Entity<StoreItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("storeItem_pkey");

            entity.ToTable("storeItem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.StoreId).HasColumnName("storeId");

            entity.HasOne(d => d.Item).WithMany(p => p.StoreItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("storeItem_itemId_fkey");

            entity.HasOne(d => d.Store).WithMany(p => p.StoreItems)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("storeItem_storeId_fkey");
        });

    
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tag_pkey");

            entity.ToTable("tag");

            entity.HasIndex(e => e.TagName, "tag_tagName_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.TagName)
                .HasMaxLength(255)
                .HasColumnName("tagName");
        });

        modelBuilder.Entity<TagValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tagValue_pkey");

            entity.ToTable("tagValue");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TagId).HasColumnName("tagId");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .HasColumnName("value");

            entity.HasOne(d => d.Tag).WithMany(p => p.TagValues)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("tagValue_tagId_fkey");
        });

   
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("account");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "user_phoneNumber_key").IsUnique();

            entity.HasIndex(e => e.UserName, "user_userName_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("userName");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("userRole_roleId_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("userRole_userId_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("userRole_pkey");
                        j.ToTable("userRole");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("userId");
                        j.IndexerProperty<int>("RoleId").HasColumnName("roleId");
                    });
        });

        modelBuilder.Entity<UserStatusLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userStatusLog_pkey");

            entity.ToTable("userStatusLog");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LogAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("logAt");
            entity.Property(e => e.StatusId).HasColumnName("statusId");
            entity.Property(e => e.TextLog)
                .HasMaxLength(1000)
                .HasColumnName("textLog");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Status).WithMany(p => p.UserStatusLogs)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("userStatusLog_statusId_fkey");

            entity.HasOne(d => d.User)
            .WithMany(p => p.UserStatusLogs)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("userStatusLog_userId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}