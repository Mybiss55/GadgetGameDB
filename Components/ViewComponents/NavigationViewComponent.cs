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
            new MenuItem { Controller = "Store", Action = "ViewMyCart", Label = "Cart", },
            new MenuItem { Controller = "Store", Action = "Orders", Label = "Orders", },

            new MenuItem { Controller = "Orders", Action = "Index", Label = "Admin", Authorized = true, AllowedRoles = new List<string> { "Administrator" },
                DropdownItems = new List<MenuItem>
                {
                    new MenuItem { Controller = "Genres", Action = "Index", Label = "Genre" },
                    new MenuItem { Controller = "Games", Action = "Index", Label = "Games" },
                    new MenuItem { Controller = "Orders", Action = "Index", Label = "Orders" },
                    new MenuItem { Controller = "Carts", Action = "Index", Label = "Carts" },
                } },
            new MenuItem { Controller = "Home", Action = "Privacy", Label = "Privacy" },
        };

            return View(menuItems);
        }
    }
}