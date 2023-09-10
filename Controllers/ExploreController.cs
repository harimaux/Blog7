using BlogService.Data;
using BlogService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Blog7.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Blog7.Controllers
{
    public class ExploreController : Controller
    {

        private readonly ILogger<ExploreController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ExploreController(ILogger<ExploreController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ApplicationDbContext Get_dbContext()
        {
            return _dbContext;
        }


        [Authorize]
        public IActionResult Index(bool runSearch, string categoryId, int page)
        {
            var vm = new MainVM
            {
                PostCategory = _dbContext?.PostCategory?.ToList()
            };

            if (runSearch == true)
            {
                SearchPosts(categoryId, page);

                if (TempData.ContainsKey("passModel"))
                {
                    vm = TempData["passModel"] as MainVM;
                    TempData.Remove("passModel");
                }
            }

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SearchPosts(string categoryId, int page = 1)
        {
            var vm = new MainVM();
 
            vm.PostCategory = _dbContext?.PostCategory?.ToList();

            if (vm.PostCategory != null)
            {
                foreach (var item in vm.PostCategory)
                {
                    if(item.Id.ToString() == categoryId)
                    {
                        // Get all posts with searched category
                        if(_dbContext?.Posts != null)
                        {
                            int pageSize = 3; // Number of posts to display per page

                            var allPosts = _dbContext.Posts
                                .Where(x => x.Category != null && x.Category.Contains(categoryId))
                                .OrderByDescending(x => x.CreatedAt)
                                .ToList();

                            vm.PostsList = allPosts;
                            int totalCount = allPosts.Count;
                            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                            // Ensure the requested page is within the valid range
                            page = Math.Max(1, Math.Min(page, totalPages));

                            // Get the posts for the requested page
                            var postsForPage = allPosts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                            vm.PostsList = postsForPage;
                            vm.CurrentPage = page;
                            vm.TotalPages = totalPages;
                            vm.PageCategoryId = categoryId;
                        }
                    }
                }

            }

            if(page == 1)
            {
                return PartialView("_SearchResults", vm);
            }
            else
            {
                TempData["passModel"] = vm; // Store the model in TempData
                return RedirectToAction("Index", new { runSearch = true, categoryId, page });
            }



        }
    }
}


