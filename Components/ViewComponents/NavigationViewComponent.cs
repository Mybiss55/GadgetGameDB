using Microsoft.AspNetCore.Mvc;
using GadgetIsLanding.Models;
using System.Linq.Expressions;

namespace GadgetIsLanding.Components.ViewComponents
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var menuItems = new List<MenuItem>
        {
            new MenuItem { Controller = "Home", Action = "Index", Label = "Home" },
            new MenuItem { Controller = "Store", Action = "Index", Label = "Store" },
            new MenuItem { Controller = "Genres", Action = "Index", Label = "Genres", DropdownItems = new List<MenuItem> {
                new MenuItem { Controller = "Genres", Action = "Index", Label = "List" },
                new MenuItem { Controller = "Genres", Action = "Create", Label = "Create" },
            }, Authorized = true, AllowedRoles = new List<string> { "Administrator" } },
            new MenuItem { Controller = "Games", Action = "Index", Label = "Games", DropdownItems = new List<MenuItem> {
                new MenuItem { Controller = "Games", Action = "Index", Label = "List" },
                new MenuItem { Controller = "Games", Action = "Create", Label = "Create" },
            }, Authorized = true , AllowedRoles = new List<string> { "Administrator" }},
            new MenuItem { Controller = "Home", Action = "Privacy", Label = "Privacy" },
        };

            return View(menuItems);
        }
    }
}