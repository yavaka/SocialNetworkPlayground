namespace SocialMedia.Services.Post
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly SocialMediaDbContext _data;

        public PostService(SocialMediaDbContext data)
        {
            this._data = data;
        }

        public async Task<Post> GetPostByIdAsync(int postId)
            => await this._data.Posts
                .Include(c => c.Comments)
                .Include(g => g.Group)
                .Include(t => t.TaggedUsers)
                .FirstOrDefaultAsync(i => i.PostId == postId);

        public async Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId)
        {
            var posts = await this._data
                  .Posts
                  .Where(i => i.AuthorId == userId)
                  .Select(p => new PostServiceModel
                  {
                      PostId = p.PostId,
                      Content = p.Content,
                      DatePosted = p.DatePosted,
                      Author = new UserServiceModel(p.Author),
                      Group = p.Group,
                      TaggedFriends = (ICollection<UserServiceModel>)p.TaggedUsers
                            .Select(t => new UserServiceModel(t.Tagged)),
                  })
                  .ToListAsync();

            return posts;
        }

        #region ExtractToCommentService

        //private static ICollection<CommentTagFriendsViewModel> GetCommentViewModelsByPostId(int postId)
        //{
        //    var commentViewModels = new List<CommentTagFriendsViewModel>();

        //    var comments = _data.Comments
        //        .Include(a => a.Author)
        //        .Include(t => t.TaggedUsers)
        //        .Where(p => p.CommentedPostId == postId)
        //        .ToList();

        //    foreach (var comment in comments)
        //    {
        //        commentViewModels.Add(NewCommentViewModel(comment));
        //    }

        //    return commentViewModels;
        //}

        //private static CommentTagFriendsViewModel NewCommentViewModel(Comment comment)
        //  => new CommentTagFriendsViewModel
        //        {
        //            Comment = comment,
        //            Tagged = comment
        //                    .TaggedUsers
        //                    .Select(u => u.Tagged)
        //                    .ToList()
        //        };

        #endregion
    }
}
