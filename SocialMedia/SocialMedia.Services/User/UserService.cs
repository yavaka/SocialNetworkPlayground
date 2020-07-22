namespace SocialMedia.Services.User
{
    using Microsoft.AspNetCore.Identity;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager) => this._userManager = userManager;

        public async Task<UserServiceModel> GetCurrentUserAsync(ClaimsPrincipal principal)
        => new UserServiceModel(
            await this._userManager.GetUserAsync(principal));

        public async Task<UserServiceModel> GetUserByIdAsync(string userId)
        => new UserServiceModel(
            await this._userManager.FindByIdAsync(userId));

        public string GetUserId(ClaimsPrincipal principal)
        => this._userManager.GetUserId(principal);
    }
}
