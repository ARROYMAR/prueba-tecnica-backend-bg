using CarritoApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarritoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // La base se crea con CarritoDB.sql (no con migraciones), por eso
        // aquí solo mapeamos las entidades a las tablas y columnas existentes.
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Usuario).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Password).HasMaxLength(500).IsRequired();
            entity.Property(u => u.Rol).HasMaxLength(20).IsRequired();
            entity.HasIndex(u => u.Usuario).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}
