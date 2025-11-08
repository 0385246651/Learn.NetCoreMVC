using App.Data;
using App.Models;
using App.Models.Blog;
using App.Models.Product;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace App.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DbManageController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbManageController(AppDbContext dbContext, ILogger<DbManageController> logger,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        // GET: DbManage
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DeleteDb()
        {
            // Xóa database


            return View();
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpPost]
        // Nhưng MVC có một rule: nếu method name kết thúc bằng "Async", thì phần "Async" sẽ bị bỏ qua khi tạo route và link.
        public async Task<IActionResult> DeleteDbAsync()
        {
            // Xóa database
            var success =
            await _dbContext.Database.EnsureDeletedAsync();

            StatusMessage = success ? "Xóa database thành công" : "Xóa database thất bại";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            // Xóa database
            await _dbContext.Database.MigrateAsync();

            StatusMessage = "Cập nhật database thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SeedDataAsync()
        {
            var rolenames = typeof(RoleName).GetFields()
                .ToList();

            foreach (var r in rolenames)
            {
                var rolename = (string)r.GetRawConstantValue();
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));

                }
            }

            // Tạo user admin admin / admin123, admin@example.com
            var useradmin = await _userManager.FindByEmailAsync("admin@example.com");
            if (useradmin == null)
            {
                useradmin = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    HomeAdress = "Hà Nội",
                    EmailConfirmed = true,
                };
                var result = await _userManager.CreateAsync(useradmin, "admin123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(useradmin, RoleName.Administrator);
                }
            }

            //seed thêm dữ liệu mẫu ở đây   post và category
            SeedPostCategory();

            //seed thêm dữ liệu mẫu ở đây   product và categoryproduct
            SeedProductCategory();

            StatusMessage = "Đã cập nhật dữ liệu mẫu (seed data) thành công!";
            return RedirectToAction(nameof(Index));
        }

        private void SeedProductCategory()
        {
            // Xóa dữ liệu cũ đã có [fakeData] trong nội dung
            _dbContext.CategoryProduct.RemoveRange(_dbContext.CategoryProduct.Where(c => c.Description.Contains("[fakeData]")));
            _dbContext.Product.RemoveRange(_dbContext.Product.Where(p => p.Content.Contains("[fakeData]")));

            _dbContext.SaveChanges();
            // CategoryProduct
            Randomizer.Seed = new Random(8675309);
            var fakerCategory = new Faker<CategoryProduct>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"Nhom SP{cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentences(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();

            // Thiết lập quan hệ cha con
            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;

            var categories = new CategoryProduct[] { cate1, cate2, cate12, cate11, cate21, cate211 };
            _dbContext.CategoryProduct.AddRange(categories);

            // Product
            var rCateIndex = new Random();
            int bv = 1;
            // Lấy user đan đăng nhập hiện tại
            var user = _userManager.GetUserAsync(this.User).Result;
            var fakerProduct = new Faker<ProductModel>();
            fakerProduct.RuleFor(p => p.AuthorId, f => user.Id);
            fakerProduct.RuleFor(p => p.Content, f => f.Commerce.ProductDescription() + "[fakeData]");
            fakerProduct.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2024, 1, 1), new DateTime(2025, 10, 1)));
            fakerProduct.RuleFor(p => p.Description, f => f.Lorem.Sentences(3));
            fakerProduct.RuleFor(p => p.Published, f => true);
            fakerProduct.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerProduct.RuleFor(p => p.Title, f => $"SP {bv++} " + f.Commerce.ProductName());
            fakerProduct.RuleFor(p => p.Price, f => int.Parse(f.Commerce.Price(500, 1000, 0)));

            List<ProductModel> products = new List<ProductModel>();
            List<ProductCategoryProduct> product_categories = new List<ProductCategoryProduct>();


            for (int i = 0; i < 40; i++)
            {
                // tạo bài viết
                var product = fakerProduct.Generate();
                // để DateUpdated = DateCreated
                product.DateUpdated = product.DateCreated;
                // thêm vào danh sách
                products.Add(product);
                product_categories.Add(new ProductCategoryProduct()
                {
                    Product = product,
                    // chọn ngẫu nhiên 1 trong 5 category đầu tiên 
                    Category = categories[rCateIndex.Next(5)]
                });
            }

            _dbContext.AddRange(products);
            _dbContext.AddRange(product_categories);
            //END POST
            _dbContext.SaveChanges();
        }

        private void SeedPostCategory()
        {
            // Xóa dữ liệu cũ đã có [fakeData] trong nội dung
            _dbContext.Categories.RemoveRange(_dbContext.Categories.Where(c => c.Description.Contains("[fakeData]")));
            _dbContext.Posts.RemoveRange(_dbContext.Posts.Where(p => p.Content.Contains("[fakeData]")));

            _dbContext.SaveChanges();
            // CATEGORY
            Randomizer.Seed = new Random(8675309);
            var fakerCategory = new Faker<Category>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"CM{cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentences(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();

            // Thiết lập quan hệ cha con
            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;

            var categories = new Category[] { cate1, cate2, cate12, cate11, cate21, cate211 };
            _dbContext.Categories.AddRange(categories);

            // POST
            var rCateIndex = new Random();
            int bv = 1;
            // Lấy user đan đăng nhập hiện tại
            var user = _userManager.GetUserAsync(this.User).Result;
            var fakerPost = new Faker<Post>();
            fakerPost.RuleFor(p => p.AuthorId, f => user.Id);
            fakerPost.RuleFor(p => p.Content, f => f.Lorem.Paragraphs(7) + "[fakeData]");
            fakerPost.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2024, 1, 1), new DateTime(2025, 10, 1)));
            fakerPost.RuleFor(p => p.Description, f => f.Lorem.Sentences(3));
            fakerPost.RuleFor(p => p.Published, f => true);
            fakerPost.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerPost.RuleFor(p => p.Title, f => $"Bài {bv++} " + f.Lorem.Sentence(3, 4).Trim('.'));

            List<Post> posts = new List<Post>();
            List<PostCategory> post_categories = new List<PostCategory>();


            for (int i = 0; i < 40; i++)
            {
                // tạo bài viết
                var post = fakerPost.Generate();
                // để DateUpdated = DateCreated
                post.DateUpdated = post.DateCreated;
                // thêm vào danh sách
                posts.Add(post);
                post_categories.Add(new PostCategory()
                {
                    Post = post,
                    // chọn ngẫu nhiên 1 trong 5 category đầu tiên 
                    Category = categories[rCateIndex.Next(5)]
                });
            }

            _dbContext.AddRange(posts);
            _dbContext.AddRange(post_categories);
            //END POST
            _dbContext.SaveChanges();
        }
    }
}
