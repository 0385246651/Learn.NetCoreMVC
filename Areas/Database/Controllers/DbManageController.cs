using App.Data;
using App.Models;
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
            var useradmin = await _userManager.FindByNameAsync("admin");
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
            StatusMessage = "Đã cập nhật dữ liệu mẫu (seed data) thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
