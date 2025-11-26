using Microsoft.EntityFrameworkCore;
using BackendProFinAPi.Models;

namespace BackendProFinAPi.Config
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- DbSets ---
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<SaleModel> Sales { get; set; }
        public DbSet<WarrantyModel> Warranties { get; set; }
        public DbSet<RepaymentModel> Repayments { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<SaleDetailModel> SaleDetails { get; set; }
        public DbSet<ProductTypeModel> ProductTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- UserModel ---
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");

                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasMany(u => u.Products)
                      .WithOne(p => p.Creator)
                      .HasForeignKey(p => p.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- ProductModel ---
            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.ToTable("Products");

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(p => p.ProductType)
                      .WithMany(pt => pt.Products)
                      .HasForeignKey(p => p.ProductTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- ProductTypeModel ---
            modelBuilder.Entity<ProductTypeModel>(entity =>
            {
                entity.ToTable("ProductTypes");
            });

            // --- CustomerModel ---
            modelBuilder.Entity<CustomerModel>(entity =>
            {
                entity.ToTable("Customers");

                entity.HasMany(c => c.Sales)
                      .WithOne(s => s.Customer)
                      .HasForeignKey(s => s.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- EmployeeModel ---
            modelBuilder.Entity<EmployeeModel>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasMany(e => e.Sales)
                      .WithOne(s => s.Employee)
                      .HasForeignKey(s => s.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- SaleModel ---
            modelBuilder.Entity<SaleModel>(entity =>
            {
                entity.ToTable("Sales");

                entity.HasMany(s => s.Repayments)
                      .WithOne(r => r.Sale)
                      .HasForeignKey(r => r.SaleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Warranty)
                      .WithOne(w => w.Sale)
                      .HasForeignKey<WarrantyModel>(w => w.SaleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.SaleDetails)
                      .WithOne(sd => sd.Sale)
                      .HasForeignKey(sd => sd.SaleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- SaleDetailModel ---
            modelBuilder.Entity<SaleDetailModel>(entity =>
            {
                entity.ToTable("SaleDetails");

                entity.HasOne(sd => sd.Product)
                      .WithMany(p => p.SaleDetails)
                      .HasForeignKey(sd => sd.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- WarrantyModel ---
            modelBuilder.Entity<WarrantyModel>(entity =>
            {
                entity.ToTable("Warranties");

                entity.HasIndex(w => w.SaleId).IsUnique();
            });

            // --- RepaymentModel ---
            modelBuilder.Entity<RepaymentModel>(entity =>
            {
                entity.ToTable("Repayments");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
