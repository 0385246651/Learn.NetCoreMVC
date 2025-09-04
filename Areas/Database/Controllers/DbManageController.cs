using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbContext;
        public DbManageController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

    }
}
