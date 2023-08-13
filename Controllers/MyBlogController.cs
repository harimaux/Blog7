using BlogService.Data;
using BlogService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog7.Models;
using BlogService.DBmodels;

namespace Blog7.Controllers
{
    public class MyBlogController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public MyBlogController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
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
        public IActionResult Index(int page = 1)
        {
            // Get logged user details
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            var vm = new MainVM();

            //Get post categories
            if (user != null && _dbContext.PostCategory != null)
            {
                vm.PostCategory = _dbContext.PostCategory.ToList();
            }

            // Get user posts
            if (user != null && _dbContext.Posts != null)
            {
                int pageSize = 3; // Number of posts to display per page
                var userPosts = _dbContext.Posts
                    .Where(x => x.OwnerId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();

                int totalCount = userPosts.Count;
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Ensure the requested page is within the valid range
                page = Math.Max(1, Math.Min(page, totalPages));

                // Get the posts for the requested page
                var postsForPage = userPosts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                vm.PostsList = postsForPage;
                vm.CurrentPage = page;
                vm.TotalPages = totalPages;
            }

            return View(vm);
        }



        [HttpPost]
        public async Task<IActionResult> SavePost(TextEditor formData)
        {
            var vm = new MainVM();

            if (!ModelState.IsValid)
            {
                // Model is not valid, return the form with validation errors
                return PartialView("PostError", formData);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Get post categories
            if (userId != null && _dbContext.PostCategory != null)
            {
                vm.PostCategory = _dbContext.PostCategory.ToList();
            }

            var newPost = new Post
            {
                Title = formData.Title,
                Category = string.Join(",", formData.Category ?? Array.Empty<string>()),
                Content = formData.RichContent,
                OwnerId = userId,
                CreatedAt = DateTime.Now
            };

            _dbContext.Posts!.Add(newPost);
            await _dbContext.SaveChangesAsync();

            vm.Post = newPost;

            return PartialView("_Post", vm);
        }


        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            if (_dbContext.Posts != null)
            {
                var post = await _dbContext.Posts.FindAsync(postId);

                if (post == null)
                {
                    return NotFound(); // Or any appropriate error response
                }

                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();

                // Redirect to an appropriate view or action
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
