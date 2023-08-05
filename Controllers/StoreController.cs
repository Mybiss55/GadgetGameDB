using GadgetIsLanding.Data;
using GadgetIsLanding.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GadgetIsLanding.Controllers
{
    public class StoreController : Controller
    {
        // Property for Database connection
        ApplicationDbContext _context;

        // Constructor
        public StoreController(ApplicationDbContext context)
        {
            _context = context;
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
                .Include(cart => cart.CartItems)
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

            if(game == null)
            {
                return NotFound();
            }

            // Create cart item

            var CartItem = new CartItem
            {
                Cart = cart,
                Game = game,
                Price = game.Price
            };
            
            if(ModelState.IsValid)
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
    }
}
