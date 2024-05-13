using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BuzzBid.Models;

public partial class BuzzBidContext : DbContext
{
    public BuzzBidContext()
    {
    }

    public BuzzBidContext(DbContextOptions<BuzzBidContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Bidding> Biddings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=CHRIS;Initial Catalog=BuzzBid;Integrated Security=True;Trusted_Connection=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Admin");

            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.UserNameNavigation).WithMany()
                .HasForeignKey(d => d.UserName)
                .HasConstraintName("FK__Admin__UserName__3B0BC30C");
        });

        modelBuilder.Entity<Bidding>(entity =>
        {
            entity.HasKey(e => e.BidId).HasName("PK__Bidding__4A733D9212ABAA86");

            entity.ToTable("Bidding");

            entity.HasIndex(e => new { e.BidItem, e.BidTime }, "UC_Bidding").IsUnique();

            entity.Property(e => e.BidAmount).HasColumnType("money");
            entity.Property(e => e.BidBy).HasMaxLength(50);
            entity.Property(e => e.BidTime).HasColumnType("datetime");

            entity.HasOne(d => d.BidByNavigation).WithMany(p => p.Biddings)
                .HasForeignKey(d => d.BidBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bidding__BidBy__3BFFE745");

            entity.HasOne(d => d.BidItemNavigation).WithMany(p => p.Biddings)
                .HasForeignKey(d => d.BidItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bidding__BidItem__3CF40B7E");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0BADBC40C1");

            entity.ToTable("Category");

            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Item__727E838B9CC390E0");

            entity.ToTable("Item");

            entity.Property(e => e.CancelDate).HasColumnType("datetime");
            entity.Property(e => e.CancelReason).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.GetItNowPrice).HasColumnType("money");
            entity.Property(e => e.ItemName).HasMaxLength(100);
            entity.Property(e => e.ListBy).HasMaxLength(50);
            entity.Property(e => e.ListDate).HasColumnType("datetime");
            entity.Property(e => e.MinSalesPrice).HasColumnType("money");
            entity.Property(e => e.SalesPrice).HasColumnType("money");
            entity.Property(e => e.StartBid).HasColumnType("money");
            entity.Property(e => e.WinDate).HasColumnType("datetime");
            entity.Property(e => e.Winner).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Item__CategoryId__3DE82FB7");

            entity.HasOne(d => d.ListByNavigation).WithMany(p => p.ItemListByNavigations)
                .HasForeignKey(d => d.ListBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Item__ListBy__3EDC53F0");

            entity.HasOne(d => d.WinnerNavigation).WithMany(p => p.ItemWinnerNavigations)
                .HasForeignKey(d => d.Winner)
                .HasConstraintName("FK__Item__Winner__3FD07829");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Rating__FCCDF87CFB230B47");

            entity.ToTable("Rating");

            entity.HasIndex(e => new { e.ItemId, e.RateTime }, "UC_Rating").IsUnique();

            entity.Property(e => e.DeleteDate).HasColumnType("datetime");
            entity.Property(e => e.RateTime).HasColumnType("datetime");
            entity.Property(e => e.Text).HasMaxLength(500);

            entity.HasOne(d => d.Item).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__ItemId__40C49C62");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK__User__C9F28457B0B06466");

            entity.ToTable("User");

            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
