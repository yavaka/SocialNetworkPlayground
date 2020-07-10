namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using SocialMedia.Data.Models;

    public class TaggedUserService : ITaggedUserService
    {
        private readonly SocialMediaDbContext _data;

        public TaggedUserService(SocialMediaDbContext data) => this._data = data;

        public ICollection<TagFriends> GetTagFriendsEntities(string taggerId, IEnumerable<string> taggedFriendsIds)
        {
            var entities = new List<TagFriends>();
            foreach (var taggedId in taggedFriendsIds)
            {
                entities.Add(new TagFriends 
                {
                    TaggerId = taggerId,
                    TaggedId = taggedId
                });
            }
            return entities;
        }

        public async Task<ICollection<UserServiceModel>> GetTaggedFriendsByPostIdAsync(int postId)
        => await this._data
              .TagFriends
              .Where(i => i.PostId == postId)
              .Select(u => new UserServiceModel
              {
                  Id = u.TaggedId,
                  UserName = u.Tagged.UserName,
                  FullName = u.Tagged.FullName
              })
              .ToListAsync();
    }
}
