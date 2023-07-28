using GadgetIsLanding.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
