namespace SocialMedia.Services.User
{
    using SocialMedia.Services.Models;
    using System.Threading.Tasks;
    using System.Security.Claims;

    public interface IUserService
    {
        Task<UserServiceModel> GetUserByIdAsync(string userId);

        Task<UserServiceModel> GetCurrentUserAsync(ClaimsPrincipal principal);

        string GetUserId(ClaimsPrincipal principal);
    }
}
