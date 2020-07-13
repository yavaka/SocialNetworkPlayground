namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using SocialMedia.Data.Models;

    public class TaggedUserService : ITaggedUserService
    {
        private readonly SocialMediaDbContext _data;

        public TaggedUserService(SocialMediaDbContext data)
        {
            this._data = data;
        }

        public ICollection<TagFriends> GetTagFriendsEntities(
            string taggerId,
            IEnumerable<string> taggedFriendsIds)
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

        public async Task<EntityState> TagFriend(string taggerId, string taggedId, int postId)
        {
            await this._data.AddAsync(new TagFriends 
            {
                TaggerId = taggerId,
                TaggedId = taggedId,
                PostId = postId
            });

            await this._data.SaveChangesAsync();

            return EntityState.Added;
        }

        public async Task<EntityState> RemoveTaggedFriend(string taggedId, int postId)
        {
            var entity = await this._data.TagFriends
                .FirstOrDefaultAsync(u => u.TaggedId == taggedId &&
                                         u.PostId == postId);

            this._data.TagFriends.Remove(entity);
            await this._data.SaveChangesAsync();

            return EntityState.Deleted;
        }

        public ICollection<UserServiceModel> GetUntaggedFriends(
            ICollection<UserServiceModel> taggedFriends,
            ICollection<UserServiceModel> friends)
        {
            foreach (var tagged in taggedFriends)
            {
                friends = friends
                    .Where(u => u.Id != tagged.Id)
                    .ToList();
            }

            return friends;
        }
    }
}
