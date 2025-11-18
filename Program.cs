using System.Configuration;
using System.Net;
using App.Data;
using App.ExtendMethods;
using App.Menu;
using App.Models;
using App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// cấu hình EF
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     string connectionString = builder.Configuration.GetConnectionString("AppMvcConnectionString");
//     options.UseSqlServer(connectionString);
// });
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
// có thể cài thêm logging provider khác như Serilog, NLog, ...
// services.AddTransient(typeof(ILogger<>), typeof(Logger<>)); //Serilog

// Thêm dịch vụ cho Razor Pages (nếu anh định dùng Razor Pages)
builder.Services.AddRazorPages();

//cấu hình sử dụng session và 
builder.Services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddSession(cfg =>
{                    // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "HaibabaMVC";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
});

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

//Đăng ký dịch vụ Identity
builder.Services.AddIdentity<AppUser, IdentityRole>() //sử dụng lớp AppUser và Role mặc định
    .AddEntityFrameworkStores<AppDbContext>() //sử dụng EF để lưu trữ thông tin user, role
    .AddDefaultTokenProviders(); //sinh token mặc định (dùng cho xác thực email, quên mật khẩu, ...)

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất


    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    options.AccessDeniedPath = "/khongduoctruycap.html";
});

builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            var gconfig = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = gconfig["ClientId"];
            options.ClientSecret = gconfig["ClientSecret"];
            // https://localhost:5001/signin-google
            options.CallbackPath = "/dang-nhap-tu-google";
        })
        .AddFacebook(options =>
        {
            var fconfig = builder.Configuration.GetSection("Authentication:Facebook");
            options.AppId = fconfig["AppId"];
            options.AppSecret = fconfig["AppSecret"];
            options.CallbackPath = "/dang-nhap-tu-facebook";
        })
        // .AddTwitter()
        // .AddMicrosoftAccount()
        ;

// Cấu hình dịch vụ gửi email
// Đọc cấu hình từ appsettings.json
builder.Services.AddOptions();
var mailsetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

// 
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

//
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("ViewManageMenu", b =>
    {
        b.RequireAuthenticatedUser();
        b.RequireRole(RoleName.Administrator);
    });
});

builder.Services.AddTransient<CartService>();

// phải inject đẻ làm tham số cho adminsidebarService
// vì đay là nơi chưa các dịch vụ để chỗ khác inject
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<AdminSidebarService>();

//cấu hình services
// services.AddSingleton<ProductService>();
// services.AddSingleton<ProductService, ProductService>();
// services.AddSingleton(typeof(ProductService));
// builder.Services.AddSingleton(typeof(ProductService), typeof(ProductService));

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

app.UseStaticFiles();
//mở thư mục Uploads để truy cập từ trình duyệt
//mở file tương ứng trong thư mục Uploads khi truy cập đường dẫn /contents
app.UseStaticFiles(new StaticFileOptions()
{
    RequestPath = "/contents",
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads"))
});

//Dùng Session
app.UseSession();

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
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
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
