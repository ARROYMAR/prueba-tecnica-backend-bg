using CarritoApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CarritoApi.Data;

/// <summary>
/// La base se crea con database/CarritoDB.sql (no con migraciones), por eso aquí solo se
/// mapean las entidades a las tablas existentes. Las propiedades tienen el mismo nombre que
/// las columnas, salvo Usuario.NombreUsuario y Producto.NombreProducto.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<CarritoCompra> CarritosCompra => Set<CarritoCompra>();
    public DbSet<CarritoCompraDetalle> CarritoCompraDetalles => Set<CarritoCompraDetalle>();
    public DbSet<Orden> Ordenes => Set<Orden>();
    public DbSet<OrdenDetalle> OrdenDetalles => Set<OrdenDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQL Server (DATETIME2) no guarda la zona horaria. Las fechas se graban en UTC,
        // así que al leerlas se marcan como UTC para que el JSON salga con "Z"
        // y el navegador las convierta a la hora local del usuario.
        var utc = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario", "dbo");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.NombreUsuario).HasColumnName("Usuario").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Password).HasMaxLength(500).IsRequired();
            entity.Property(u => u.Rol).HasMaxLength(20).IsRequired();
            entity.Property(u => u.FechaCreacion).ValueGeneratedOnAdd().HasConversion(utc);
            entity.HasIndex(u => u.NombreUsuario).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria", "dbo");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Descripcion).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto", "dbo");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Codigo).HasMaxLength(30).IsRequired();
            entity.Property(p => p.NombreProducto).HasColumnName("Producto").HasMaxLength(150).IsRequired();
            entity.Property(p => p.Descripcion).HasMaxLength(500);
            entity.Property(p => p.Precio).HasPrecision(18, 2);
            entity.HasIndex(p => p.Codigo).IsUnique();
            entity.HasOne(p => p.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(p => p.CategoriaId);
        });

        modelBuilder.Entity<CarritoCompra>(entity =>
        {
            entity.ToTable("CarritoCompra", "dbo");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FechaCreacion).HasConversion(utc);
            entity.Property(c => c.FechaModificacion).HasConversion(utc);
            entity.HasIndex(c => c.UsuarioId).IsUnique();
            entity.HasMany(c => c.Detalles)
                  .WithOne(d => d.CarritoCompra)
                  .HasForeignKey(d => d.CarritoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CarritoCompraDetalle>(entity =>
        {
            entity.ToTable("CarritoCompraDetalle", "dbo");
            entity.HasKey(d => d.Id);
            entity.HasIndex(d => new { d.CarritoId, d.ProductoId }).IsUnique();
            entity.HasOne(d => d.Producto)
                  .WithMany()
                  .HasForeignKey(d => d.ProductoId);
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.ToTable("Orden", "dbo");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrdenCreacion).HasConversion(utc);
            entity.Property(o => o.Subtotal).HasPrecision(18, 2);
            entity.Property(o => o.PorcDescuento).HasPrecision(5, 2);
            entity.Property(o => o.Descuento).HasPrecision(18, 2);
            entity.Property(o => o.Total).HasPrecision(18, 2);
            entity.HasMany(o => o.Detalles)
                  .WithOne(d => d.Orden)
                  .HasForeignKey(d => d.OrdenId);
        });

        modelBuilder.Entity<OrdenDetalle>(entity =>
        {
            entity.ToTable("OrdenDetalle", "dbo");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.ProductoCodigo).HasMaxLength(30).IsRequired();
            entity.Property(d => d.ProductoNombre).HasMaxLength(150).IsRequired();
            entity.Property(d => d.Precio).HasPrecision(18, 2);
            entity.Property(d => d.Total).HasPrecision(18, 2);
        });
    }
}
