namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.Profile;
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
        public async Task<IActionResult> IndexAsync(
            string userId, 
            string friendshipStatus)
        {
            var currentUserId = this._userService
                .GetUserId(User);

            ProfileServiceModel profile;

            if (userId != null)
            {
                profile = await this._profileService.GetProfileAsync(userId);

                if (friendshipStatus == null)
                {
                    return RedirectToAction("FriendshipStatus", "Friendships", new { userId = userId });
                }

                //Depending on the friendship status it will be generated different layout.
                profile.Message = friendshipStatus;
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