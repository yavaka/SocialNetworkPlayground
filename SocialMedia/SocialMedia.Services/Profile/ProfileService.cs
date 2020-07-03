namespace SocialMedia.Services.Profile
{
    using Microsoft.AspNetCore.Identity;
    using SocialMedia.Models;

    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;

        public ProfileService(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }
    }
}
