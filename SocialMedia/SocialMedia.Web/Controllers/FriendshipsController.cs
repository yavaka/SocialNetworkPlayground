﻿namespace SocialMedia.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.User;

    public class FriendshipsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IUserService _userService;

        public FriendshipsController(
            IFriendshipService friendshipService,
            IUserService userService)
        {
            this._friendshipService = friendshipService;
            this._userService = userService;
        }

        public async Task<IActionResult> Friends()
        {
            TempData.Clear();
            var currentUserId = this._userService
                .GetUserId(User);

            var friends = await this._friendshipService
                .GetFriendsAsync(currentUserId);

            return View(friends);
        }

        public async Task<IActionResult> NonFriends()
        {
            TempData.Clear();
            var currentUserId = this._userService
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

            var currentUserId = this._userService
                .GetUserId(User);

            if (currentUserId == userId)
            {
                return RedirectToAction("Index", "Profile");
            }

            var friendshipStatus = await this._friendshipService
                .GetFriendshipStatusAsync(currentUserId, userId);

            var status = string.Empty;

            switch (friendshipStatus)
            {
                case -1:
                    status = "-1";
                    break;
                case 0:
                    status = $"{friendshipStatus} {invokedFrom}";
                    break;
                case 1:
                    status = friendshipStatus.ToString();
                    break;
            }

            return RedirectToAction("Index", "Profile", new { userId = userId, friendshipStatus = status });
        }

        public async Task<IActionResult> FriendRequests()
        {
            var model = new FriendshipServiceModel();

            var currentUserId = this._userService
                .GetUserId(User);

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

            var currentUserId = this._userService.GetUserId(User);

            await this._friendshipService.SendRequestAsync(currentUserId, addresseeId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> AcceptAsync(string requesterId)
        {
            var currentUserId = this._userService.GetUserId(User);

            await this._friendshipService.AcceptRequestAsync(currentUserId, requesterId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> RejectAsync(string requesterId)
        {
            var currentUserId = this._userService.GetUserId(User);

            await this._friendshipService.RejectRequestAsync(currentUserId, requesterId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> CancelInvitationAsync(string addresseeId)
        {
            var currentUserId = this._userService.GetUserId(User);

            await this._friendshipService.CancelInvitationAsync(currentUserId, addresseeId);

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> UnfriendAsync(string friendId) 
        {
            var currentUserId = this._userService.GetUserId(User);

            await this._friendshipService.UnfriendAsync(currentUserId, friendId);

            return RedirectToAction(nameof(Friends));
        }
    }
}