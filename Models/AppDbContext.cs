using App.Models.Contacts;
using App.Models.Blog;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using App.Models.Product;


namespace App.Models
{
  //App.Model.AppDbContext
  public class AppDbContext : IdentityDbContext<AppUser>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
      base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      foreach (var entityType in modelBuilder.Model.GetEntityTypes())
      {
        var tableName = entityType.GetTableName();
        if (tableName.StartsWith("AspNet"))
        {
          entityType.SetTableName(tableName.Substring(6));
        }
      }

      // dùng fluent API để cấu hình cho Category
      // chỉ mục cho Slug
      // Đánh index cho db để tìm nhanh theo Slug và bắt buộc duy nhất
      modelBuilder.Entity<Category>(entity =>
      {
        //đặt tên bảng để tránh nhầm khi cahjy update DB  . ví dụ bảng Category trùng tên với bảng CategoryProduct
        entity.ToTable("Category");
        entity.HasIndex(c => c.Slug).IsUnique();
      });
      // thiết lập quan hệ nhiều - nhiều giữa Post và Category qua PostCategory
      // 2 khóa chính PostID và CategoryID trong PostCategory 
      modelBuilder.Entity<PostCategory>(entity =>
      {
        entity.HasKey(c => new { c.PostID, c.CategoryID });
      });

      //
      modelBuilder.Entity<Post>(entity =>
      {
        entity.HasIndex(p => p.Slug).IsUnique();
      });

      // Product Category
      // dùng fluent API để cấu hình cho Category
      // chỉ mục cho Slug       // Đánh index cho db để tìm nhanh theo Slug và bắt buộc duy nhất
      modelBuilder.Entity<CategoryProduct>(entity =>
      {
        entity.ToTable("CategoryProduct");
        entity.HasIndex(c => c.Slug).IsUnique();
      });
      // thiết lập quan hệ nhiều - nhiều giữa Post và Category qua PostCategory
      // 2 khóa chính PostID và CategoryID trong PostCategory 
      modelBuilder.Entity<ProductCategoryProduct>(entity =>
      {
        entity.HasKey(c => new { c.ProductID, c.CategoryID });
      });

      //
      modelBuilder.Entity<ProductModel>(entity =>
      {
        entity.HasIndex(p => p.Slug).IsUnique();
      });
    }


    public DbSet<Contact> Contacts { get; set; }

    // Post và product có cấu trúc gần giống nhau nên ta tách riêng ra hai nhóm
    //Post
    public DbSet<Category> Categories { get; set; }

    public DbSet<Post> Posts { get; set; }
    public DbSet<PostCategory> PostCategories { get; set; }

    //Product  
    public DbSet<CategoryProduct> CategoryProduct { get; set; }

    public DbSet<ProductModel> Product { get; set; }
    public DbSet<ProductCategoryProduct> ProductCategoryProduct { get; set; }

    public DbSet<ProductPhoto> ProductPhotos { get; set; }
  }
}