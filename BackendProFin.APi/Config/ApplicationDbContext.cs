using BackendProFinAPi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Ya no es necesario, pero lo dejo por si lo usas en otros sitios
using Microsoft.AspNetCore.Identity; // Ya no es necesario, pero lo dejo por si lo usas en otros sitios

namespace BackendProFinAPi.Config
{
    // 🔑 CAMBIO CRUCIAL 1: Hereda SOLAMENTE de DbContext, NO de IdentityDbContext
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- DbSets ---
        // 🔑 AGREGAR: Tu modelo de usuario debe estar explícitamente aquí.
        public DbSet<UserModels> Users { get; set; }

        // Tus modelos de negocio
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductTypeModel> ProductTypes { get; set; }
        public DbSet<SaleModel> Sales { get; set; }
        public DbSet<SaleDetailModel> SaleDetails { get; set; }
        public DbSet<RepaymentModel> Repayments { get; set; }
        public DbSet<WarrantyModel> Warranties { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // ⚠️ IMPORTANTE: Si heredas de DbContext, la llamada 'base.OnModelCreating(builder);' 
            // no hace nada de Identity, lo cual es lo que queremos.
            base.OnModelCreating(builder);

            // ============================
            // CONFIGURACIONES DE BUSINESS (Se mantienen)
            // ============================

            // ... (Todas tus configuraciones de relaciones, HasOne, WithMany, DeleteBehavior, etc.) ...

            // ============================
            // TIPOS DECIMALES (Se mantienen)
            // ============================
            // ... (Todas tus configuraciones de tipos decimales) ...
            builder.Entity<ProductModel>().Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            builder.Entity<SaleModel>().Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)");
            builder.Entity<SaleDetailModel>().Property(sd => sd.UnitPriceAtSale)
                .HasColumnType("decimal(18,2)");
            builder.Entity<RepaymentModel>().Property(r => r.Amount)
                .HasColumnType("decimal(18,2)");


            // ============================
            // 🔑 LIMPIEZA DE IDENTITY
            // ============================

            // 1. Mapeo de UserModel: Aquí solo cambias el nombre de la tabla si lo deseas, 
            // EF Core solo mapeará las propiedades que TÚ definiste en UserModel.
            builder.Entity<UserModels>().ToTable("Users");

            // 2. ELIMINAR O COMENTAR: Ya no necesitas ignorar o renombrar las tablas secundarias de Identity, 
            // ya que al heredar de DbContext (simple) EF Core no las conoce ni las intenta crear.
            /*
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            */

            // Si tu UserModel (clase) accidentalmente heredó de IdentityUser, 
            // y no puedes cambiarlo, podrías usar .Ignore() aquí:
            // builder.Entity<UserModel>().Ignore(u => u.NormalizedUserName); 
            // PERO la mejor solución es que UserModel NO herede de nada.
        }
    }
}