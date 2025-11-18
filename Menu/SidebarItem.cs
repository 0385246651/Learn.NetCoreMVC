using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace App.Menu
{
  public enum SidebarItemType
  {
    Divider,
    Heading,
    NavItem
  }

  public class SidebarItem
  {
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public SidebarItemType Type { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Area { get; set; }
    public string AwesomeIcon { get; set; }
    public List<SidebarItem> Items { get; set; }
    public string collapseID { get; set; }

    public string GetLink(IUrlHelper urlHelper)
    {
      return urlHelper.Action(Action, Controller, new { area = Area });
    }

    public string RenderHtml(IUrlHelper urlHelper)
    {
      var html = new StringBuilder();

      if (Type == SidebarItemType.Divider)
      {
        html.Append("<hr class=\"border-secondary my-2\">");
      }
      else if (Type == SidebarItemType.Heading)
      {
        html.Append($@"<li class=""nav-item"">
                        <small class=""text-muted text-uppercase px-3 py-2 d-block !text-white"">{Title}</small>
                      </li>");
      }
      else if (Type == SidebarItemType.NavItem)
      {
        if (Items == null)
        {
          // Nav item đơn giản
          var url = GetLink(urlHelper);
          var icon = !string.IsNullOrEmpty(AwesomeIcon) ? $"<i class=\"{AwesomeIcon} me-2\"></i>" : "";
          var activeClass = IsActive ? " active bg-secondary" : "";

          html.Append($@"
            <li class=""nav-item"">
              <a class=""nav-link text-white{activeClass}"" href=""{url}"">
                {icon}<span>{Title}</span>
              </a>
            </li>");
        }
        else
        {
          // Nav item có submenu (collapse)
          var icon = !string.IsNullOrEmpty(AwesomeIcon) ? $"<i class=\"{AwesomeIcon} me-2\"></i>" : "";
          var activeClass = IsActive ? " active" : "";
          var showClass = IsActive ? " show" : "";

          var itemMenu = new StringBuilder();
          foreach (var item in Items)
          {
            var urlItem = item.GetLink(urlHelper);
            var itemActiveClass = item.IsActive ? " active fw-bold" : "";
            itemMenu.Append($@"
              <li>
                <a class=""dropdown-item text-white{itemActiveClass}"" href=""{urlItem}"">{item.Title}</a>
              </li>");
          }

          html.Append($@"
            <li class=""nav-item{activeClass}"">
              <a class=""nav-link text-white"" href=""#{collapseID}"" 
                 data-bs-toggle=""collapse"" 
                 aria-expanded=""{(IsActive ? "true" : "false")}"">
                {icon}<span>{Title}</span>
              </a>
              <div class=""collapse{showClass}"" id=""{collapseID}"">
                <ul class=""nav flex-column ms-3"">
                  {itemMenu}
                </ul>
              </div>
            </li>");
        }
      }

      return html.ToString();
    }
  }
}