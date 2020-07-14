namespace SocialMedia.Services.Comment
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.TaggedUser;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommentService : ICommentService
    {
        private readonly SocialMediaDbContext _data;
        private readonly ITaggedUserService _taggedUserService;

        public CommentService(
            SocialMediaDbContext data,
            ITaggedUserService taggedUserService)
        {
            this._data = data;
            this._taggedUserService = taggedUserService;
        }

        public async Task<EntityState> AddComment(CommentServiceModel serviceModel)
        {
            await this._data.Comments.AddAsync(
                new Comment
                {
                    Content = serviceModel.Content,
                    DatePosted = serviceModel.DatePosted,
                    AuthorId = serviceModel.AuthorId,
                    CommentedPostId = serviceModel.PostId,
                    TaggedUsers = this._taggedUserService.GetTagFriendsEntities(
                    serviceModel.AuthorId,
                    serviceModel.TaggedFriends
                        .Select(i => i.Id)
                        .ToList())
                });

            await this._data.SaveChangesAsync();

            return EntityState.Added;
        }
    }
}
