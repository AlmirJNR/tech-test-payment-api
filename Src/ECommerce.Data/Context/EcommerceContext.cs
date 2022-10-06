using ECommerce.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Context
{
    public partial class EcommerceContext : DbContext
    {
        public EcommerceContext()
        {
        }

        public EcommerceContext(DbContextOptions<EcommerceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Purchase> Purchases { get; set; } = null!;
        public virtual DbSet<PurchaseProduct> PurchaseProducts { get; set; } = null!;
        public virtual DbSet<PurchaseStatus> PurchaseStatuses { get; set; } = null!;
        public virtual DbSet<Seller> Sellers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.HasIndex(e => e.Name, "product_name_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("purchase");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.PurchaseStatusId).HasColumnName("purchase_status_id");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.PurchaseStatus)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.PurchaseStatusId)
                    .HasConstraintName("purchase_purchase_status_id_fkey");

                entity.HasOne(d => d.Seller)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.SellerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_seller_id_fkey");
            });

            modelBuilder.Entity<PurchaseProduct>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.ProductId })
                    .HasName("purchase_product_pkey");

                entity.ToTable("purchase_product");

                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.ProductAmount)
                    .HasColumnName("product_amount")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PurchaseProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_product_product_id_fkey");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseProducts)
                    .HasForeignKey(d => d.PurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_product_purchase_id_fkey");
            });

            modelBuilder.Entity<PurchaseStatus>(entity =>
            {
                entity.ToTable("purchase_status");

                entity.HasIndex(e => e.Status, "purchase_status_status_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<Seller>(entity =>
            {
                entity.ToTable("seller");

                entity.HasIndex(e => e.Cpf, "seller_cpf_key")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "seller_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Telephone, "seller_telephone_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.Cpf).HasColumnName("cpf");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Telephone).HasColumnName("telephone");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
