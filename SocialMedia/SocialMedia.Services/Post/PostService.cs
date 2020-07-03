namespace SocialMedia.Services.Post
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using SocialMedia.Models.ViewModels;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.TaggedUser;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly SocialMediaDbContext _data;
        private readonly ITaggedUserService _taggedUserService;

        public PostService(
            SocialMediaDbContext data,
            ITaggedUserService taggedUserService)
        {
            this._data = data;
            this._taggedUserService = taggedUserService;
        }

        public async Task<Post> GetPostByIdAsync(int postId)
            => await this._data.Posts
                .Include(c => c.Comments)
                .Include(g => g.Group)
                .Include(t => t.TaggedUsers)
                .FirstOrDefaultAsync(i => i.PostId == postId);

        public async Task<ICollection<PostServiceModel>> GetPostsByUserId(string userId)
           /* How can I invoke await GetTaggedUsersAsync and
              await GetCommentsByPostIdAsync in .Select()    */
           => await this._data
                .Posts
                .Where(i => i.AuthorId == userId)
                .Select(p => new PostServiceModel
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    Author = new UserServiceModel(p.Author),
                    Group = p.Group,
                    //TaggedFriends 
                    //Comments
                })
                .ToListAsync();

        #region ExtractToCommentService

        private ICollection<CommentTagFriendsViewModel> GetCommentViewModelsByPostIdAsync(int postId)
        {
            var commentViewModels = new List<CommentTagFriendsViewModel>();

            var comments = this._data.Comments
                .Include(a => a.Author)
                .Include(t => t.TaggedUsers)
                .Where(p => p.CommentedPostId == postId)
                .ToList();

            foreach (var comment in comments)
            {
                commentViewModels.Add(NewCommentViewModel(comment));
            }

            return commentViewModels;
        }

        private CommentTagFriendsViewModel NewCommentViewModel(Comment comment)
        {
            using (this._data)
            {
                return new CommentTagFriendsViewModel
                {
                    Comment = comment,
                    Tagged = comment
                            .TaggedUsers
                            .Select(u => u.Tagged)
                            .ToList()
                };
            }
        }

        #endregion
    }
}
