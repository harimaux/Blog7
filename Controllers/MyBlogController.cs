using BlogService.Data;
using BlogService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog7.Models;
using BlogService.DBmodels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Blog7.Controllers
{
    public class MyBlogController : Controller
    {

        private readonly ILogger<MyBlogController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public MyBlogController(ILogger<MyBlogController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
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


        [Authorize]
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

        [Authorize]
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





        [Authorize]
        public async Task<IActionResult> Edit(int editId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = new MainVM();

            //Get post categories
            if (userId != null && _dbContext.PostCategory != null)
            {
                vm.PostCategory = await _dbContext.PostCategory.ToListAsync();
            }

            if (_dbContext.Posts != null)
            {
                var userPosts = await _dbContext.Posts.Where(x => x.OwnerId == userId).ToListAsync();

                foreach (var post in userPosts)
                {
                    if(post.Id == editId)
                    {
                        vm.Post.Id = post.Id;
                        vm.Post.CreatedAt = post.CreatedAt;
                        vm.Post.Category = post.Category;
                        vm.Post.Content = post.Content;
                        vm.Post.OwnerId = post.OwnerId;
                        vm.Post.Title = post.Title;
                    }
                }
            }

                return View(vm);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveEditedPost(TextEditor formData, int id)
        {
            var vm = new MainVM();

            if (!ModelState.IsValid)
            {
                // Model is not valid, return the form with validation errors
                return PartialView("PostError", formData);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_dbContext.Posts != null)
            {
                var userPosts = await _dbContext.Posts.Where(x => x.OwnerId == userId).ToListAsync();

                var postToEdit = userPosts.Where(x => x.Id == id).First();

                postToEdit.Title = formData.Title;
                postToEdit.Category = string.Join(",", formData.Category ?? Array.Empty<string>());
                postToEdit.Content = formData.RichContent;
                postToEdit.OwnerId = userId;

                _dbContext.Posts!.Update(postToEdit);
                await _dbContext.SaveChangesAsync();

            }

            return RedirectToAction("Index");
        }




    }
}
