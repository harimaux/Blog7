using BlogService.Data;
using BlogService.DBmodels;
using Microsoft.EntityFrameworkCore;

namespace Blog7.Models
{
    public class MainVM
    {
        //DB Models
        public Post Post { get; set; }
        public List<Post>? PostsList { get; set; }
        public List<PostCategory>? PostCategory { get; set; }
        public UserExtraStuff UserExtraStuff { get; set; }
        public List<StockAvatars>? StockAvatars { get; set; }

        //Project Models
        public TextEditor TextEditor { get; set; }
        

        //Pagination on user post page
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public MainVM()
        {

            Post = new Post();

            UserExtraStuff = new UserExtraStuff();

            TextEditor = new TextEditor();

            PostsList = new List<Post>();

            PostCategory = new List<PostCategory>();

            StockAvatars = new List<StockAvatars>();
        }

    }
}
