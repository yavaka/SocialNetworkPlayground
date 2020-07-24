namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.Profile;
    using SocialMedia.Web.Infrastructure;
    using SocialMedia.Services.User;

    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;

        public ProfileController(
            IProfileService profileService,
            IUserService userService)
        {
            this._profileService = profileService;
            this._userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string userId)
        {
            var currentUserId = this._userService
                .GetUserId(User);

            ProfileServiceModel profile;

            if (userId != null)
            {
                profile = await this._profileService.GetProfileAsync(userId);

                if (!TempData.ContainsKey("friendshipStatus"))
                {
                    return RedirectToAction("FriendshipStatus", "Friendships", new { userId = userId });
                }

                //Depending on the friendship status it will be generated different layout.
                profile.Message = TempData["friendshipStatus"] as string;
                
                if (!TempData.ContainsKey("userId"))
                {
                    TempData.Set("userId", profile.User.Id);
                }
            }
            else //Gets the current user`s profile
            {
                profile = await this._profileService.GetProfileAsync(currentUserId);
            }

            profile.CurrentUserId = currentUserId;

            return View(profile);
        }
    }
}