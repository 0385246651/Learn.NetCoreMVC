using App.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
  [ViewComponent]
  public class CategoryProductSidebar : ViewComponent
  {
    public class CategorySidebarData
    {
      public List<CategoryProduct> Categories { get; set; }
      public int level { get; set; }

      public string categoryslug { get; set; }
    }

    public IViewComponentResult Invoke(CategorySidebarData data)
    {
      // truyền sang view dữ liệu data là Default.cshtml
      return View(data);
    }
  }
}

