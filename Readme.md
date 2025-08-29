## Controller

- Là một lớp kế từ thừa lớp Controller : Microsoft.AspNetCore.Mvc.Controller
- Action trong controller là một phương public (không được static)
- Action trả về bất kỳ kiểu dữ liệu nào, thường là IActionResult
- Các dịch vụ inject vào controller qua hàm tạo

## View

- Là file .cshtml
- View cho Action lưu tại: /View/ControllerName/ActionName.cshtml
- Thêm thư mục lưu trữ View:

```
// {0} -> ten Action
// {1} -> ten Controller
// {2} -> ten Area
options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
```

## Truyền dữ liệu sang View

- Model
- ViewData
- ViewBag
- TempData

##Areas
-Là tên dùng để routing

- Là cấu trúc thư mực chưa MVC
- Thiết lập Area cho controller bằng `[Area("AreaName")]`
- Tạo cấu trúc thư mục

```
dotnet aspnet-codegenerator area Product
```
