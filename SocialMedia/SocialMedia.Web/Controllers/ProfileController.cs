namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Models.ViewModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Data.Models;

    public class ProfileController : Controller
    {
        private readonly ITaggedUserService _taggedUserService;

        private readonly SocialMediaDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProfileController(SocialMediaDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        private ProfileViewModel ViewModel { get; set; } = new ProfileViewModel();

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            this.ViewModel.CurrentUser = await this._userManager.GetUserAsync(User);

            //Get all post, comments and tagged users in ViewModel
            this.ViewModel.Posts = await GetPostViewModelsByUserIdAsync(ViewModel.CurrentUser.Id);

            return View(ViewModel);
        }

        private async Task<ICollection<PostTagFriendsViewModel>> GetPostViewModelsByUserIdAsync(string userId)
        {
            var postViewModels = new List<PostTagFriendsViewModel>();

            var posts = await this._context.Posts
                .Where(i => i.AuthorId == userId)
                .ToListAsync();

            foreach (var post in posts)
            {
                postViewModels.Add(await NewPostViewModelAsync(post.PostId));
            }

            return postViewModels;
        }

        private async Task<PostTagFriendsViewModel> NewPostViewModelAsync(int postId)
        {
            var postViewModel = new PostTagFriendsViewModel
            {
                Post = await this.GetPostByIdAsync(postId)
            };

            if (postViewModel.Post.TaggedUsers.Count > 0)
            {
                postViewModel.Tagged = await _taggedUserService
                    .GetTaggedUsersAsync(postViewModel.Post.TaggedUsers.Select(u => u.TaggedId));
            }

            if (postViewModel.Post.Comments.Count > 0)
            {
                postViewModel.Comments = await GetCommentViewModelsByPostIdAsync(postId);
            }

            return postViewModel;
        }

        private async Task<ICollection<CommentTagFriendsViewModel>> GetCommentViewModelsByPostIdAsync(int postId)
        {
            var commentViewModels = new List<CommentTagFriendsViewModel>();

            var comments = await this._context.Comments
                .Include(a => a.Author)
                .Include(t => t.TaggedUsers)
                .Where(p => p.CommentedPostId == postId)
                .ToListAsync();

            foreach (var comment in comments)
            {
                commentViewModels.Add(await NewCommentViewModelAsync(comment));
            }

            return commentViewModels;
        }

        private async Task<CommentTagFriendsViewModel> NewCommentViewModelAsync(Comment comment)
        {
            return new CommentTagFriendsViewModel
            {
                Comment = comment,
                Tagged = await this._taggedUserService.GetTaggedUsersAsync(
                    comment
                        .TaggedUsers
                        .Select(u => u.TaggedId))
            };
        }

        private async Task<Post> GetPostByIdAsync(int postId)
        {
            return await this._context.Posts
                .Include(c => c.Comments)
                .Include(g => g.Group)
                .Include(t => t.TaggedUsers)
                .FirstOrDefaultAsync(i => i.PostId == postId);
        }
    }
}