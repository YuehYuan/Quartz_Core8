using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebSch.Context.Entities;

namespace WebSch.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SchedularJob> SchedularJob { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SchedularJob>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CronSchedule)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DbConnection)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.GroupId)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Hospital)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Region)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ServerName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
