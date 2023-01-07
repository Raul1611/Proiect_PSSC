using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProiectPSSC.Models;

public partial class ProiectPsscContext : DbContext
{
    public ProiectPsscContext()
    {
    }

    public ProiectPsscContext(DbContextOptions<ProiectPsscContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OrderHeader>? OrderHeaders { get; set; }

    public virtual DbSet<OrderLine>? OrderLines { get; set; }

    public virtual DbSet<Product>? Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=tcp:proiectpssc.database.windows.net,1433;Initial Catalog=ProiectPSSC;Persist Security Info=False;User ID=psscadmin;Password=proiectpssc1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderHeader>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("OrderHeader");

            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Total).HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.ToTable("OrderLine");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLineOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLine_OrderHeader");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderLineProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLine_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
