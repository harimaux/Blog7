using Blog7.Models;
using BlogService;
using BlogService.Data;
using BlogService.DBmodels;
using ImageMagick;
using Microsoft.AspNetCore.Components.Forms;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_dbContext.StockAvatars != null)
            {
                Mv.StockAvatars = _dbContext.StockAvatars.ToList();
            }

            if(_dbContext.UserExtraStuff != null)
            {
                var userExtraContentDB = _dbContext.UserExtraStuff.FirstOrDefault(x => x.UserId == userId);

                if (userExtraContentDB != null && userExtraContentDB.CustomAvatarImage != null)
                {
                    Mv.UserExtraStuff.CustomAvatarImage = userExtraContentDB.CustomAvatarImage;
                }
                else if(userExtraContentDB != null && userExtraContentDB.StockAvatarId != null)
                {
                    var stockAvatar = _dbContext.StockAvatars?.FirstOrDefault(x => x.Id.ToString() == userExtraContentDB.StockAvatarId);
                    Mv.UserExtraStuff.CustomAvatarImage = stockAvatar?.ImageBase64;
                }
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
                    Vm.CustomAvatarImage = null;

                    _dbContext.UserExtraStuff.Add(Vm);
                    _dbContext.SaveChanges();
                }
                else
                {
                    userExtraContentDB.StockAvatarId = avatarId.ToString();
                    userExtraContentDB.CustomAvatarImage = null;

                    _dbContext.UserExtraStuff.Update(userExtraContentDB);
                    _dbContext.SaveChanges();
                }
            }

            return RedirectToPage("Avatar");
        }

        public IActionResult OnPostUploadAvatar(IFormFile customAvatar)
        {

            if (customAvatar == null)
            {
                ViewData["ErrorMessage"] = "Please select an avatar to upload.";

                return OnGet();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var fileStream = customAvatar.OpenReadStream();
            var image = new MagickImage(fileStream);

            int targetWidth = 300;
            int targetHeight = 300;

            double targetAspectRatio = (double)targetWidth / targetHeight;
            double originalAspectRatio = (double)image.Width / image.Height;

            if (originalAspectRatio > targetAspectRatio)
            {
                // Crop horizontally
                int cropWidth = (int)Math.Round(image.Height * targetAspectRatio);
                int x = (image.Width - cropWidth) / 2;
                int y = 0;
                image.Crop(new MagickGeometry(x, y, cropWidth, image.Height));
            }
            else
            {
                // Crop vertically
                int cropHeight = (int)Math.Round(image.Width / targetAspectRatio);
                int x = 0;
                int y = (image.Height - cropHeight) / 2;
                image.Crop(new MagickGeometry(x, y, image.Width, cropHeight));
            }

            image.Resize(targetWidth, targetHeight);

            // Convert the modified image to Base64
            string base64Image;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Write(memoryStream);
                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }

            if(_dbContext.UserExtraStuff != null)
            {
                var userExtraContentDB = _dbContext.UserExtraStuff.FirstOrDefault(x => x.UserId == userId);

                if (userExtraContentDB == null)
                {
                    var Vm = new UserExtraStuff();

                    Vm.CustomAvatarImage = base64Image;
                    Vm.UserId = userId;

                    _dbContext.UserExtraStuff.Add(Vm);
                    _dbContext.SaveChanges();
                }
                else
                {
                    userExtraContentDB.CustomAvatarImage = base64Image;
                    userExtraContentDB.StockAvatarId = null;

                    _dbContext.UserExtraStuff.Update(userExtraContentDB);
                    _dbContext.SaveChanges();
                }
            }


            return RedirectToPage("Avatar");
        }

    }
}
