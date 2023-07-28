using Microsoft.AspNetCore.Identity;

namespace GadgetIsLanding.Models
{
    public class User : IdentityUser
    {
        public List<Cart> Cart { get; set; }

        public List<Order> Order { get; set; }
    }
}
