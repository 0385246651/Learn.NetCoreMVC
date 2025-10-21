using App.Models.Contacts;
using App.Models.Blog;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


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
      modelBuilder.Entity<Category>(entity =>
      {
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
    }


    public DbSet<Contact> Contacts { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Post> Posts { get; set; }
    public DbSet<PostCategory> PostCategories { get; set; }
  }
}