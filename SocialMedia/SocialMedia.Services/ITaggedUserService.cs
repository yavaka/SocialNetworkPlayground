namespace SocialMedia.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ITaggedUserService
    {
        Task<IEnumerable<UserServiceModel>> GetTaggedUsersAsync(IEnumerable<string> tagFriendIds);
    }
}
