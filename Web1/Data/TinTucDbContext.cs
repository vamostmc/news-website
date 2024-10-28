using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web1.DataNew;
using Web1.Models;

namespace Web1.Data;

public partial class TinTucDbContext : IdentityDbContext<AppUser>
{
    public TinTucDbContext()
    {
    }

    public TinTucDbContext(DbContextOptions<TinTucDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<TinTuc> TinTucs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotifyType> NotifyTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
        .ToTable("AspNetUsers");

        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.BinhluanId).HasName("PK__BinhLuan__7B6CA14E405369F7");

            entity.ToTable("BinhLuan");

            entity.Property(e => e.BinhluanId).HasColumnName("BinhluanID");
            entity.Property(e => e.NgayGioBinhLuan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(1000);
            entity.Property(e => e.TintucId).HasColumnName("TintucID");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Tintuc).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.TintucId)
                .HasConstraintName("FK__BinhLuan__Tintuc__73BA3083");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.DanhmucId).HasName("PK__DanhMuc__3214EC07E03DBD6D");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.DanhmucId).HasColumnName("DanhmucID");
            entity.Property(e => e.TenDanhMuc).HasMaxLength(255);
            entity.Property(e => e.TrangThai);
        });

        modelBuilder.Entity<TinTuc>(entity =>
        {
            entity.HasKey(e => e.TintucId).HasName("PK__TinTuc__3214EC071B7B629A");

            entity.ToTable("TinTuc");

            entity.Property(e => e.TintucId).HasColumnName("TintucID");
            entity.Property(e => e.DanhmucId).HasColumnName("DanhmucID");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.LuongKhachTruyCap).HasDefaultValue(0);
            entity.Property(e => e.MoTaNgan).HasMaxLength(500);
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayDang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TieuDe).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasDefaultValue(false);
            entity.Property(e => e.NoiDung).HasColumnType("nvarchar(max)");

            entity.HasOne(d => d.Danhmuc).WithMany(p => p.TinTucs)
                .HasForeignKey(d => d.DanhmucId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__TinTuc__DanhMucI__6383C8BA");
        });


        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0720B93E79");

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Type).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_Notifications_NotifyTypes");
        });

        modelBuilder.Entity<NotifyType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__NotifyTy__516F03B5067351CF");

            entity.Property(e => e.TypeName).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
