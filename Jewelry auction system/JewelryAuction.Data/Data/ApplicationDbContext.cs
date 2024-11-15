using Microsoft.EntityFrameworkCore;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Data.Entity;

namespace Jewelry_auction_system.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Wallets> Wallets { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; } // Thêm DbSet cho CategoryType
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Auctions> Auctions { get; set; }
        public DbSet<Bids> Bids { get; set; }
        public DbSet<FeedBacks> FeedBacks { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho ProductCategory
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => pc.ID);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Products)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.CategoryType)
                .WithMany(ct => ct.ProductCategories)
                .HasForeignKey(pc => pc.CategoryTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho CategoryType
            modelBuilder.Entity<CategoryType>()
                .HasOne(ct => ct.Category)
                .WithMany(c => c.CategoryType)
                .HasForeignKey(ct => ct.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Các cấu hình khác
            modelBuilder.Entity<Bids>()
                .HasOne(b => b.Users)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bids>()
                .HasOne(b => b.Auctions)
                .WithMany(a => a.Bids)
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Auctions>()
                .HasOne(a => a.Products)
                .WithMany(p => p.Auctions)
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FeedBacks>()
                .HasOne(fb => fb.User)
                .WithMany(u => u.FeedBacks)
                .HasForeignKey(fb => fb.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FeedBacks>()
                .HasOne(fb => fb.Auction)
                .WithMany(a => a.FeedBacks)
                .HasForeignKey(fb => fb.AuctionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Images>()
                .HasOne(i => i.Products)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Wallets)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Wallets>()
                .HasOne(w => w.Users)
                .WithOne(u => u.Wallets)
                .HasForeignKey<Wallets>(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Products>()
                .HasOne(p => p.Users)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
