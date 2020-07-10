namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using SocialMedia.Data.Models;

    public interface ITaggedUserService
    {
        ICollection<TagFriends> GetTagFriendsEntities(string taggerId, IEnumerable<string> taggedFriendsIds);

        Task<ICollection<UserServiceModel>> GetTaggedFriendsByPostIdAsync(int postId);

        //Task<ICollection<UserServiceModel>> GetTaggedFriendsByCommentIdAsync(int commentId);
    }
}
