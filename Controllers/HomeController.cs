using Blog7.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BlogService;
using BlogService.Data;
using BlogService.DBmodels;
using Microsoft.AspNetCore.Identity;

namespace Blog7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpPost]
        public async Task<IActionResult> Upload(StockAvatars model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        image.CopyTo(ms);
                        model.ImageBase64 = Convert.ToBase64String(ms.ToArray());
                    }
                }

                _dbContext.Add(model);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index"); // Redirect to a page showing all uploaded images.
            }

            return View(model);
        }



    }
}