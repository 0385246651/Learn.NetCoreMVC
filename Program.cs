using System.Configuration;
using System.Net;
using App.ExtendMethods;
using App.Models;
using App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// cấu hình EF
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("AppMvcConnectionString");
    options.UseSqlServer(connectionString);
});

// Add services to the container.
builder.Services.AddControllersWithViews();
// có thể cài thêm logging provider khác như Serilog, NLog, ...
// services.AddTransient(typeof(ILogger<>), typeof(Logger<>)); //Serilog

// Thêm dịch vụ cho Razor Pages (nếu anh định dùng Razor Pages)
builder.Services.AddRazorPages();

// Cấu hình để tìm view trong thư mục MyView
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // /View/Controller/Action.cshtml
    // /MyView/Controller/Action.cshtml

    // {0} -> ten Action
    // {1} -> ten Controller
    // {2} -> ten Area
    options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
});

builder.Services.AddSingleton<PlanetService>();

//cấu hình services
// services.AddSingleton<ProductService>();
// services.AddSingleton<ProductService, ProductService>();
// services.AddSingleton(typeof(ProductService));
builder.Services.AddSingleton(typeof(ProductService), typeof(ProductService));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // xac dinh danh tinh 
app.UseAuthorization();  // xac thuc  quyen truy  cap

//tuwj cusstomed middleware
app.AddStatusCodePage(); // them middleware xu ly loi 404, 500, ..


app.MapStaticAssets();

// Đây là phần thay đổi: gọi các phương thức Map* trực tiếp
// Các phương thức này nên được đặt sau app.UseRouting()
app.MapGet("/sayhi", async context =>
{
    await context.Response.WriteAsync($"Hello World {DateTime.Now}!");
});

app.MapRazorPages();

//Tạo route mặc định cho controller
// app.MapControllers
// app.MapDefaultControllerRoute
// app.MapAreaControllerRoute

// [AcceptVerbs]
// [Route]
//[HttpGet]
//[HttpPost]
//[HttpPut]
//[HttpDelete]
//[HttpPatch]
//[HttpHead]
//[HttpOptions]

//Nếu dùng ntn thì chỉ áp dụng trên các controller ko có area
// Nếu có area thì phải dùng app.MapAreaControllerRoute

app.MapAreaControllerRoute(
    name: "product",
    areaName: "ProductManage",
    pattern: "/{controller}/{action=Index}/{id?}"
    ).WithStaticAssets();

app.MapControllerRoute(
    // name: "firstroute",
    // pattern: "start-here/{id}",
    // defaults: new
    // {
    //     conntroler = "First",
    //     action = "ViewProduct",
    //     id = 3
    // }
    //chỉ ra các ràng buộc cho tham số
    // constraints: new
    // {
    //     url = new StringRouteConstraint("^[a-z]{3,6}$"), // url chỉ chấp nhận 3-6 ký tự a-z
    //     id = new RangeRouteConstraint(1, 1000) // id chỉ chấp nhận từ 1-1000
    // },// chi chap nhan id la so;
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    ).WithStaticAssets();

app.Run();
