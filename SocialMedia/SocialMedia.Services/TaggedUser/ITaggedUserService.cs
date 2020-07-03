namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ITaggedUserService
    {
        Task<ICollection<UserServiceModel>> GetTaggedUsersAsync(IEnumerable<string> tagFriendIds);
    }
}
