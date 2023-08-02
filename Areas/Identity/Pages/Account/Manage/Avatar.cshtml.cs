using Blog7.Models;
using BlogService;
using BlogService.Data;
using BlogService.DBmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Blog7.Areas.Identity.Pages.Account.Manage
{
    public class AvatarModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public AvatarModel(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Add a property to store the MainVM model
        public MainVM? Mv { get; set; }

        public IActionResult OnGet()
        {
            // Pass the ApplicationDbContext to MainVM's constructor
            Mv = new MainVM();

            if (_dbContext.StockAvatars != null)
            {
                Mv.StockAvatars = _dbContext.StockAvatars.ToList();
            }

            ViewData["Title"] = "Avatar Page";

            return Page();
        }

        public IActionResult OnPostChooseAvatar(int avatarId)
        {
            var Vm = new UserExtraStuff();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_dbContext.UserExtraStuff != null)
            {
                var userExtraContentDB = _dbContext.UserExtraStuff.FirstOrDefault(x => x.UserId == userId);

                if(userExtraContentDB == null)
                {
                    Vm.StockAvatarId = avatarId.ToString();
                    Vm.UserId = userId;

                    _dbContext.UserExtraStuff.Add(Vm);
                    _dbContext.SaveChanges();
                }
                else
                {
                    userExtraContentDB.StockAvatarId = avatarId.ToString();

                    _dbContext.UserExtraStuff.Update(userExtraContentDB);
                    _dbContext.SaveChanges();
                }
            }

            return RedirectToPage("Avatar");
        }

        public IActionResult OnPostUploadAvatar(IFormFile customAvatar)
        {
            // This handler will be executed when the "Upload Avatar" button is clicked
            // in the second form.

            // Use the customAvatar parameter to access the uploaded file.
            // For example, you can save the file to a specific location or database.

            // Perform your logic here.

            return RedirectToPage("Avatar");
        }

    }
}
