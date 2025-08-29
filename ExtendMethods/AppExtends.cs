using System.Net;
using Microsoft.AspNetCore.Builder;

namespace App.ExtendMethods
{
  public static class AppExtends
  {
    //phương thức mở rộng cho IApplicationBuilder
    public static void AddStatusCodePage(this IApplicationBuilder app)
    {
      //thêm middleware tùy chỉnh vào pipeline
      app.UseStatusCodePages(
    appError =>
    {
      appError.Run(async context =>
      {
        var response = context.Response;
        var code = response.StatusCode;

        var content = @$"<html>
                <head>
                    <meta charset='utf-8'/>
                    <title>Lỗi {code}</title>
                </head>
                <body>
                <p style='color: red; font-size: 30px'>Có Lỗi {code} - {(HttpStatusCode)code} xảy ra!</p>
                </body>
            </html>";
        response.ContentType = "text/html";
        await response.WriteAsync(content);
      }); // code 404, 500, 403, ...
    }
); // tra ve trang hien thi loi mac dinh
    }
  }
}