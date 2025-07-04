﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PAW.Data.Models;

public partial class ProyectDbContext : DbContext
{
    public ProyectDbContext()
    {
    }

    public ProyectDbContext(DbContextOptions<ProyectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Listum> Lista { get; set; }

    public virtual DbSet<Tablero> Tableros { get; set; }

    public virtual DbSet<Tarjetum> Tarjeta { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-1MSSGI4\\SQLEXPRESS;Database=ProyectDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comentar__3214EC07BFB07018");

            entity.ToTable("Comentario");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Tarjeta).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.TarjetaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comentari__Tarje__59063A47");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comentari__Usuar__59FA5E80");
        });

        modelBuilder.Entity<Listum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lista__3214EC0784112719");

            entity.Property(e => e.Titulo).HasMaxLength(100);

            entity.HasOne(d => d.Tablero).WithMany(p => p.Lista)
                .HasForeignKey(d => d.TableroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lista__TableroId__5070F446");
        });

        modelBuilder.Entity<Tablero>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tablero__3214EC07F01FE64A");

            entity.ToTable("Tablero");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Titulo).HasMaxLength(100);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Tableros)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tablero__Usuario__4D94879B");
        });

        modelBuilder.Entity<Tarjetum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tarjeta__3214EC0727E83F09");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaVencimiento).HasColumnType("datetime");
            entity.Property(e => e.Titulo).HasMaxLength(100);

            entity.HasOne(d => d.Lista).WithMany(p => p.Tarjeta)
                .HasForeignKey(d => d.ListaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tarjeta__ListaId__5441852A");

            entity.HasOne(d => d.UsuarioAsignado).WithMany(p => p.Tarjeta)
                .HasForeignKey(d => d.UsuarioAsignadoId)
                .HasConstraintName("FK__Tarjeta__Usuario__5535A963");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC0738918D42");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A1996C2914F").IsUnique();

            entity.Property(e => e.Clave).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
