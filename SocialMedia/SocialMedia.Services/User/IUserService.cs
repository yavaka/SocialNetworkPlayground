namespace SocialMedia.Services.User
{
    using System.Threading.Tasks;
    using System.Security.Claims;

    public interface IUserService
    {
        Task<UserServiceModel> GetUserByIdAsync(string userId);

        Task<UserServiceModel> GetCurrentUserAsync(ClaimsPrincipal principal);

        string GetUserId(ClaimsPrincipal principal);
    }
}
