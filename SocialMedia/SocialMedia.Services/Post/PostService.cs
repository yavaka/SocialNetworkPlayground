namespace SocialMedia.Services.Post
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
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

        public async Task<EntityState> AddPost(PostServiceModel serviceModel)
        {
            var post = new Post
            {
                Content = serviceModel.Content,
                DatePosted = serviceModel.DatePosted,
                AuthorId = serviceModel.Author.Id,
                TaggedUsers = this._taggedUserService.GetTagFriendsEntities(
                    serviceModel.Author.Id,
                    serviceModel.TaggedFriends.Select(i => i.Id).ToList())
            };

            if (serviceModel.Group != null)
            {
                post.GroupId = serviceModel.Group.GroupId;
            }

            await this._data.Posts.AddAsync(post);
            await this._data.SaveChangesAsync();

            return EntityState.Added;
        }

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
