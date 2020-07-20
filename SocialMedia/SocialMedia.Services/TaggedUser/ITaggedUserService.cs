namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using SocialMedia.Data.Models;

    public interface ITaggedUserService
    {
        ICollection<TagFriends> GetTagFriendsEntities(
            string taggerId, 
            IEnumerable<string> taggedFriendsIds);

        Task<ICollection<UserServiceModel>> GetTaggedFriendsByPostIdAsync(int postId);

        Task<EntityState> TagFriendPost(string taggerId, string taggedId, int postId);

        Task<EntityState> TagFriendComment(string taggerId, string taggedId, int commentId);

        Task<EntityState> RemoveTaggedFriendPost(string taggedId, int postId);

        Task<EntityState> RemoveTaggedFriendComment(string taggedId, int commentId);
        
        Task<EntityState> RemoveTaggedFriendsCommentId(int id);

        ICollection<UserServiceModel> GetUntaggedFriends(
            ICollection<UserServiceModel> taggedFriends,
            ICollection<UserServiceModel> friends);
        
    }
}
