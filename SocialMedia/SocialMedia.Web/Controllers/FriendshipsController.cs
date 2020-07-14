namespace SocialMedia.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Data.Models;
    using SocialMedia.Models.ViewModels;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Services.Models;

    public class FriendshipsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IFriendshipService _friendshipService;

        public FriendshipsController(
            UserManager<User> userManager,
            IFriendshipService friendshipService)
        {
            this._userManager = userManager;
            this._friendshipService = friendshipService;
        }

        public async Task<IActionResult> Friends()
        {
            var currentUserId = this._userManager
                .GetUserId(User);

            var friends = await this._friendshipService
                .GetFriendsAsync(currentUserId);

            return View(friends);
        }

        public async Task<IActionResult> NonFriends()
        {
            var currentUserId = this._userManager
                .GetUserId(User);

            var nonFriends = await this._friendshipService
                .GetNonFriendsAsync(currentUserId);

            return View(nonFriends);
        }

        [HttpGet]
        public async Task<IActionResult> FriendshipStatus(string userId, string invokedFrom)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var currentUserId = this._userManager.GetUserId(User);

            if (currentUserId == userId)
            {
                return RedirectToAction("Index", "Profile");
            }

            var friendshipStatus = await this._friendshipService
                .GetFriendshipStatusAsync(currentUserId, userId);

            switch (friendshipStatus)
            {
                case -1:
                    TempData["friendshipStatus"] = "-1";
                    break;
                case 0:
                    TempData["friendshipStatus"] = $"{friendshipStatus} {invokedFrom}";
                    break;
                case 1:
                    TempData["friendshipStatus"] = friendshipStatus.ToString();
                    break;
            }

            return RedirectToAction("Index", "Profile", new { userId = userId });
        }

        public async Task<IActionResult> FriendRequests()
        {
            var model = new FriendshipServiceModel();

            var currentUserId = this._userManager.GetUserId(User);

            model.Requests = await this._friendshipService
                .GetFriendRequestsAsync(currentUserId);

            model.PendingRequests = await this._friendshipService
                .GetPendingRequestsAsync(currentUserId);

            return View(model);
        }

        public async Task<IActionResult> SendRequestAsync(string addresseeId)
        {
            if (addresseeId == null)
            {
                return NotFound();
            }

            var currentUserId = this._userManager.GetUserId(User);

            await this._friendshipService.SendRequestAsync(currentUserId, addresseeId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> AcceptAsync(string requesterId)
        {
            var currentUserId = this._userManager.GetUserId(User);

            await this._friendshipService.AcceptRequestAsync(currentUserId, requesterId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> RejectAsync(string requesterId)
        {
            var currentUserId = this._userManager.GetUserId(User);

            await this._friendshipService.RejectRequestAsync(currentUserId, requesterId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> CancelInvitationAsync(string addresseeId)
        {
            var currentUserId = this._userManager.GetUserId(User);

            await this._friendshipService.CancelInvitationAsync(currentUserId, addresseeId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> UnfriendAsync(string friendId) 
        {
            var currentUserId = this._userManager.GetUserId(User);

            await this._friendshipService.UnfriendAsync(currentUserId, friendId);

            return RedirectToAction(nameof(Friends));
        }
    }
}