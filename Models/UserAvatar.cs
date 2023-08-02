using BlogService;
using BlogService.Data;
using BlogService.DBmodels;
using Microsoft.AspNetCore.Identity;

namespace Blog7.Models
{
    public class UserAvatar : Blog7.Areas.Identity.Pages.Account.Manage.IndexModel
    {
        public UserAvatar(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext) : base(userManager, signInManager, httpContextAccessor, dbContext)
        {
        }

        public string? AvatarImage { get; set; }


    }
}
