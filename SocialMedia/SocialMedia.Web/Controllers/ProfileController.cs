namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.Profile;
    using SocialMedia.Web.Infrastructure;

    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IProfileService _profileService;

        public ProfileController(
            UserManager<User> userManager,
            IProfileService profileService)
        {
            this._userManager = userManager;
            this._profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string userId)
        {
            var currentUserId = this._userManager.GetUserId(User);

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