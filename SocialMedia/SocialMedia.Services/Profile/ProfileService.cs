namespace SocialMedia.Services.Profile
{
    using Microsoft.AspNetCore.Identity;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.Post;
    using System.Threading.Tasks;

    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;

        public ProfileService(
            UserManager<User> userManager,
            IPostService postService)
        {
            this._userManager = userManager;
            this._postService = postService;
        }

        public async Task<ProfileServiceModel> GetProfileAsync(string userId)
            => new ProfileServiceModel()
            {
                User = new UserServiceModel(
                    await this._userManager
                        .FindByIdAsync(userId)),
                Posts = await this._postService
                        .GetPostsByUserIdAsync(userId)
            };
    }
}
