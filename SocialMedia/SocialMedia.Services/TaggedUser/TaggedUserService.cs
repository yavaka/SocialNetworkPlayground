namespace SocialMedia.Services.TaggedUser
{
    using System;
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

        public async Task<EntityState> TagFriendPost(string taggerId, string taggedId, int postId)
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

        public async Task<EntityState> TagFriendComment(string taggerId, string taggedId, int commentId)
        {
            await this._data.AddAsync(new TagFriends
            {
                TaggerId = taggerId,
                TaggedId = taggedId,
                CommentId = commentId
            });

            await this._data.SaveChangesAsync();

            return EntityState.Added;
        }

        public async Task<EntityState> RemoveTaggedFriendPost(string taggedId, int postId)
        {
            var entity = await this._data.TagFriends
                .FirstOrDefaultAsync(u => u.TaggedId == taggedId &&
                                         u.PostId == postId);

            this._data.TagFriends.Remove(entity);
            await this._data.SaveChangesAsync();

            return EntityState.Deleted;
        }

        public async Task<EntityState> RemoveTaggedFriendComment(string taggedId, int commentId)
        {
            var entity = await this._data.TagFriends
                .FirstOrDefaultAsync(u => u.TaggedId == taggedId &&
                                        u.CommentId == commentId);
            this._data.TagFriends.Remove(entity);
            await this._data.SaveChangesAsync();

            return EntityState.Deleted;
        }

        public async Task<EntityState> DeleteTaggedFriendsPostId(int postId)
        {
            var entities = await this._data.TagFriends
                 .Where(p => p.PostId == postId)
                 .ToListAsync();

            this._data.TagFriends.RemoveRange(entities);
            await this._data.SaveChangesAsync();

            return EntityState.Deleted;
        }

        public async Task<EntityState> DeleteTaggedFriendsCommentId(int commentId)
        {
            var entities = await this._data.TagFriends
                 .Where(c => c.CommentId == commentId)
                 .ToListAsync();

            this._data.TagFriends.RemoveRange(entities);
            await this._data.SaveChangesAsync();

            return EntityState.Deleted;
        }

        public ICollection<UserServiceModel> GetUntaggedFriends(
            List<UserServiceModel> taggedFriends,
            List<UserServiceModel> friends)
        {
            foreach (var tagged in taggedFriends)
            {
                var taggedFriendIndex = GetTaggedFriendIndex(friends, tagged.Id);
                friends[taggedFriendIndex].Checked = true;
            }
            return friends;
        }

        private int GetTaggedFriendIndex(List<UserServiceModel> usersCollection, string taggedId)
        {
            for (int i = 0; i < usersCollection.Count; i++)
            {
                if (usersCollection[i].Id == taggedId)
                {
                    return i;
                }
            }
            return -1;
        }

        public async Task DeleteTaggedFriendsInComments(ICollection<int> commentsIds)
        {
            var entities = await this._data.TagFriends
                .Where(i => commentsIds
                        .Contains((int)i.CommentId))
                .ToListAsync();

            if (entities.Count > 0)
            {
                this._data.TagFriends.RemoveRange(entities);
                await this._data.SaveChangesAsync();
            }
        }

        public async Task UpdateTaggedFriendsInPostAsync(
            IList<UserServiceModel> taggedFriends,
            int postId,
            string taggerId)
        {
            //Get tag friends entities
            var tagFriendsEntities = await this._data.TagFriends
                .Where(t => t.PostId == postId &&
                        t.TaggerId == taggerId)
                .ToListAsync();

            for (int i = 0; i < tagFriendsEntities.Count; i++)
            {
                //This action shows that the current friend is not untagged/modified.
                if (taggedFriends.Any(t => t.Id == tagFriendsEntities[i].TaggedId))
                {
                    var taggedFriendIndex = GetTaggedFriendIndex(
                        taggedFriends.ToList(),
                        tagFriendsEntities[i].TaggedId);
                    taggedFriends.RemoveAt(taggedFriendIndex);
                }
                //This action shows that the current friend is untagged/modified.
                else if (!taggedFriends.Any(t => t.Id == tagFriendsEntities[i].TaggedId))
                {
                    await RemoveTaggedFriendPost(tagFriendsEntities[i].TaggedId, postId);
                }
            }

            //This action check for newly tagged friends
            if (taggedFriends.Count > 0)
            {
                foreach (var tagged in taggedFriends)
                {
                    await TagFriendPost(taggerId, tagged.Id, postId);
                }
            }
        }
    }
}
