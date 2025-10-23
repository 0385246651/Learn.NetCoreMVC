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

##gen scss tỏng thư mục assets thành css troing thư mục se
wwwroot/css (sẽ tự đông thay đổi khi save file scss) - có thể áp dụng cho sass

```
gulp
```

## Dùng LibMan (Library Manager của .NET)

```
libman install @fortawesome/fontawesome-free --provider unpkg --destination wwwroot/lib/fontawesome
```

##Sau đó thêm vào \_Layout.cshtml:

```
<link rel="stylesheet" href="~/lib/fontawesome/css/all.min.css" />
```

## DÙng bootstrap icon

```
libman install bootstrap-icons --provider unpkg --destination wwwroot/lib/bootstrap-icons
```

##Sau đó thêm vào \_Layout.cshtml:

```
<link rel="stylesheet" href="~/lib/bootstrap-icons/font/bootstrap-icons.css" />
```

## Libman lấy thư viện ở cdnjs.com. thêm 1 khối thư viện mới và chạy câu lệnh

```
libman restore
```

## Tích hợp gói elFinder.NetCore

```
dotnet add package elFinder.NetCore
```
