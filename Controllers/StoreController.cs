using GadgetIsLanding.Data;
using GadgetIsLanding.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;

namespace GadgetIsLanding.Controllers
{
    public class StoreController : Controller
    {
        // Property for Database connection
        ApplicationDbContext _context;
        private IConfiguration _configuration;

        // Constructor
        public StoreController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //Makes this faster, was "public IActionResult Index()"
        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genre
                .OrderBy(genres => genres.Name)
                .ToListAsync();


            return View(genres);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var genreWithGames = await _context.Genre
                .Include(genre => genre.Games)
                .FirstOrDefaultAsync(genre => genre.Id == id);

            return View(genreWithGames);
        }

        public async Task<IActionResult> GameDetails(int? id)
        {
            var game = await _context.Game
              .Include(game => game.Genre)
              .FirstOrDefaultAsync(game => game.Id == id);

            return View(game);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int gameId, int quantity)
        {
            // Get logged in user

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get the UserCart for this user, or create one if it doesn't exist

            var cart = await _context.Cart
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            if (cart == null)
            {
                cart = new Models.Cart { UserId = userId, Active = true };
                await _context.Cart.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            // Find game
            var game = await _context.Game
                .FirstOrDefaultAsync(game => game.Id == gameId);


            // No Product? Run away!

            if (game == null)
            {
                return NotFound();
            }

            // Create cart item

            var CartItem = new CartItem
            {
                Cart = cart,
                Game = game,
                Quantity = quantity,
                Price = game.Price
            };

            if (ModelState.IsValid)
            {
                await _context.AddAsync(CartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewMyCart");
            }

            return NotFound();

        }

        [Authorize]
        public async Task<IActionResult> ViewMyCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Cart
                .Include(cart => cart.User)
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Game)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Cart
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            if (cart == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(cartItem => cartItem.Cart == cart && cartItem.Id == cartItemId);

            if (cartItem != null)
            {
                _context.Remove(cartItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewMyCart");
            }

            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Cart
                .Include(cart => cart.User)
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Game)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            var order = new Models.Order
            {
                UserId = userId,
                Cart = cart,
                Total = cart.CartItems.Sum(cartItem => cartItem.Price * cartItem.Quantity),
                ShippingAddress = "",
                PaymentMethod = Models.PaymentMethod.Stripe,
                };

            ViewData["PaymentMethods"] = new SelectList(Enum.GetValues(typeof(Models.PaymentMethod)));  

            return View(order);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Payment(string shippingAddress, Models.PaymentMethod paymentMethod)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Cart
                .Include(cart => cart.CartItems)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            if(cart == null) { return NotFound(); }

            // Add order data to session

            HttpContext.Session.SetString("ShippingAddress", shippingAddress);
            HttpContext.Session.SetString("PaymentMethod", paymentMethod.ToString());

            // Set Stripe API key
            StripeConfiguration.ApiKey = _configuration.GetSection("Stripe")["SecretKey"];

            // Create payment intent

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(cart.CartItems.Sum(cartItem => cartItem.Price * cartItem.Quantity) * 100),
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "GadgetIsLanding Purchase",
                            },
                        },
                        Quantity = 1,
                    },
                },
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Store/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Store/ViewMyCart",
            };

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> SaveOrder()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Cart
                .Include(cart => cart.CartItems)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            var paymentMethod = HttpContext.Session.GetString("PaymentMethod");
            var shippingAddress = HttpContext.Session.GetString("ShippingAddress");

            var order = new Order()
            {
                UserId = userId,
                Cart = cart,
                Total = cart.CartItems.Sum(cartItem => cartItem.Quantity + cartItem.Price),
                ShippingAddress = shippingAddress,
                PaymentMethod = (Models.PaymentMethod)Enum.Parse(typeof(Models.PaymentMethod), paymentMethod),
                PaymentReceived = true,
            };

            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            cart.Active = false;
            _context.Update(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderDetails", new { orderId = order.Id });
        }

        [Authorize]
        [Route("Store/OrderDetails/{orderId}")]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = await _context.Order
                .Include(order => order.User)
                .Include(order => order.Cart)
                .ThenInclude(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Game)
                .FirstOrDefaultAsync(order => order.Id == orderId && order.UserId == userId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> Orders(int orderId) {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orders = _context.Order
                .Include(o => o.Cart)
                .Include(order => order.User)
                .Where(order => order.UserId == userId);

            return View(orders);

        }
    }
}
