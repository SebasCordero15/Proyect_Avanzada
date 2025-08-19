using Microsoft.EntityFrameworkCore;
using PAW.Models.Entities;

namespace PAW.Data.MSSql
{
    public partial class ProyectDbContext : DbContext
    {
        public ProyectDbContext() { }
        public ProyectDbContext(DbContextOptions<ProyectDbContext> options) : base(options) { }

        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<Tablero> Tableros { get; set; } = null!;
        public virtual DbSet<Listum> Lista { get; set; } = null!;
        public virtual DbSet<Tarjetum> Tarjeta { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USUARIO
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(e => e.Id).HasName("PK_Usuario_Id");
                entity.Property(e => e.Nombre).HasMaxLength(100);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.Clave).HasMaxLength(255);
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            // TABLERO
            modelBuilder.Entity<Tablero>(entity =>
            {
                entity.ToTable("Tablero");
                entity.HasKey(e => e.Id).HasName("PK_Tablero_Id");
                entity.Property(e => e.Titulo).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getdate())");

                // Tablero -> Usuario (creador). No cascada al eliminar usuario.
                entity.HasOne(d => d.Usuario)
                      .WithMany(p => p.Tableros)
                      .HasForeignKey(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Tablero_Usuario");
            });

            // LISTA  (CASCADE a Tablero)
            modelBuilder.Entity<Listum>(entity =>
            {
                entity.ToTable("Lista");
                entity.HasKey(e => e.Id).HasName("PK_Lista_Id");
                entity.Property(e => e.Titulo).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Orden).IsRequired();

                // ✅ Cascada: al borrar un Tablero se borran sus Listas
                entity.HasOne(d => d.Tablero)
                      .WithMany(p => p.Lista)
                      .HasForeignKey(d => d.TableroId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_Lista_Tablero");
            });

            // TARJETA (CASCADE a Lista)
            modelBuilder.Entity<Tarjetum>(entity =>
            {
                entity.ToTable("Tarjeta");
                entity.HasKey(e => e.Id).HasName("PK_Tarjeta_Id");
                entity.Property(e => e.Titulo).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion);
                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("(getdate())");
                entity.Property(e => e.FechaVencimiento)
                      .HasColumnType("datetime");

                // ✅ Cascada: al borrar una Lista se borran sus Tarjetas
                entity.HasOne(d => d.Lista)
                      .WithMany(p => p.Tarjeta)
                      .HasForeignKey(d => d.ListaId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_Tarjeta_Lista");

                // Si borran al usuario asignado, la tarjeta queda sin asignación
                entity.HasOne(d => d.UsuarioAsignado)
                      .WithMany(p => p.Tarjeta)
                      .HasForeignKey(d => d.UsuarioAsignadoId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Tarjeta_UsuarioAsignado");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
