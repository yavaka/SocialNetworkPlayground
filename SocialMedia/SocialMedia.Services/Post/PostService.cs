namespace SocialMedia.Services.Post
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Comment;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.TaggedUser;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly SocialMediaDbContext _data;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;

        public PostService(
            SocialMediaDbContext data,
            ITaggedUserService taggedUserService,
            ICommentService commentService)
        {
            this._data = data;
            this._taggedUserService = taggedUserService;
            this._commentService = commentService;
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
                    serviceModel.TaggedFriends
                        .Select(i => i.Id)
                        .ToList())
            };

            if (serviceModel.Group != null)
            {
                post.GroupId = serviceModel.Group.GroupId;
            }

            await this._data.Posts.AddAsync(post);
            await this._data.SaveChangesAsync();

            return EntityState.Added;
        }

        public async Task<EntityState> EditPost(PostServiceModel serviceModel)
        {
            var post = await this._data.Posts
                .FirstOrDefaultAsync(i => i.PostId == serviceModel.PostId);

            post.Content = serviceModel.Content;

            this._data.Update(post);
            await this._data.SaveChangesAsync();

            return EntityState.Modified;
        }

        public async Task<PostServiceModel> GetPost(int id)
        => await this._data.Posts
            .Select(p => new PostServiceModel
            {
                PostId = p.PostId,
                Content = p.Content,
                DatePosted = p.DatePosted,
                Author = new UserServiceModel(p.Author),
                Group = p.Group,
                TaggedFriends = p.TaggedUsers
                    .Select(t => new UserServiceModel(t.Tagged))
                    .ToList()
            })
            .FirstOrDefaultAsync(i => i.PostId == id);

        public async Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId)
        {
            var posts = this._data.Posts
                .Where(i => i.AuthorId == userId)
                .Select(p => new PostServiceModel
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    Author = new UserServiceModel(p.Author),
                    Group = p.Group,
                    TaggedFriends = p.TaggedUsers
                        .Select(t => new UserServiceModel(t.Tagged))
                        .ToList()
                })
                .ToList();

            foreach (var post in posts)
            {
                post.Comments = await this._commentService
                    .GetCommentsByPostIdAsync(post.PostId);
            }

            //posts.ForEach(async p => p.Comments = await this._commentService
            //    .GetCommentsByPostIdAsync(p.PostId));

            return posts;
        }
    }
}
