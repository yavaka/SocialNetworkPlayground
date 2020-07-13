namespace SocialMedia.Web.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Web.Infrastructure;
    using System.Threading.Tasks;

    public class TagFriendsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ITaggedUserService _taggedUserService;

        public TagFriendsController(
            UserManager<User> userManager,
            ITaggedUserService taggedUserService)
        {
            this._userManager = userManager;
            this._taggedUserService = taggedUserService;
        }

        public IActionResult AddTaggedFriendLocal(string taggedId)
        {
            var serviceModel = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");
            var invokedFrom = TempData
                .Get<string>("invokedFrom")
                .Trim('"');

            //Check is the last tagged friend is already been tagged.
            if (serviceModel.TaggedFriends.Count > 0 &&
                serviceModel.TaggedFriends.Last().Id == taggedId)
            {
                return RedirectToAction(
                    invokedFrom,
                    "Posts");
            }

            //Adds tagged user in tagged collection of the view model
            var taggedFriend = new UserServiceModel(
                this._userManager
                    .Users
                        .FirstOrDefault(i => i.Id == taggedId));

            serviceModel.TaggedFriends.Add(taggedFriend);

            //Remove tagged user from user friends list of view model
            //The concept is that if any of the tagged users exist in this collection, 
            //it does not make sense to be tagged twice in one post.
            taggedFriend = serviceModel.UntaggedFriends
                    .FirstOrDefault(u => u.Id == taggedFriend.Id);
            serviceModel.UntaggedFriends.Remove(taggedFriend);

            TempData.Keep("tagFriendsServiceModel");
            TempData["tagFriendsServiceModel"] = JsonConvert.SerializeObject(serviceModel);

            return RedirectToAction(
                invokedFrom,
                "Posts");
        }

        public async Task<IActionResult> TagFriendAsync(string taggedId, int? postId)
        {
            if (taggedId == null ||
                postId == null)
            {
                return NotFound();
            }

            var taggerId = this._userManager.GetUserId(User);

            await this._taggedUserService.TagFriend(taggerId, taggedId, (int)postId);

            var invokedFrom = TempData
               .Get<string>("invokedFrom")
               .Trim('"');

            return RedirectToAction(
                invokedFrom,
                "Posts",
                new { id = postId });
        }

        public async Task<IActionResult> RemoveTaggedFriendAsync(string taggedId, int? postId)
        {
            if (taggedId == null ||
                postId == null)
            {
                return NotFound();
            }

            await this._taggedUserService.RemoveTaggedFriend(taggedId, (int)postId);

            var invokedFrom = TempData
               .Get<string>("invokedFrom")
               .Trim('"');

            return RedirectToAction(
                invokedFrom,
                "Posts",
                new { id = postId });
        }

        //public IActionResult AddTaggedFriendLocal(CommentViewModel viewModel, string taggedId, string invokedFrom)
    }
}