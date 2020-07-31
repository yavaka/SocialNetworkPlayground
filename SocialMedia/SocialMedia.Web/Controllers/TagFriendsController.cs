namespace SocialMedia.Web.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Web.Infrastructure;
    using System.Threading.Tasks;
    using SocialMedia.Services.User;

    public class TagFriendsController : Controller
    {
        private readonly ITaggedUserService _taggedUserService;
        private readonly IUserService _userService;

        public TagFriendsController(
            ITaggedUserService taggedUserService,
            IUserService userService)
        {
            this._taggedUserService = taggedUserService;
            this._userService = userService;
        }

        //[HttpGet]
        //public async Task<IActionResult> AddTaggedFriendLocalAsync(string taggedId, string returnUrl)
        //{
        //    var serviceModel = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");

        //    var controllerName = returnUrl.Split(new char[] { '/', '&' })[3];
        //    var actionName = returnUrl.Split(new char[] { '/', '&' })[4];

        //    if (TempData.ContainsKey("Comments"))
        //    {
        //        controllerName = "Comments";
        //        actionName = TempData
        //            .Get<string>("Comments")
        //            .Trim('"');
        //    }

        //    //Check is the last tagged friend is already been tagged.
        //    if (serviceModel.TaggedFriends.Count > 0 &&
        //        serviceModel.TaggedFriends.Last().Id == taggedId)
        //    {
        //        return RedirectToAction(
        //            actionName,
        //            controllerName,
        //            new { postId = serviceModel.PostId, returnUrl = returnUrl });
        //    }

        //    //Adds tagged user in tagged collection of the view model
        //    var taggedFriend = await this._userService
        //        .GetUserByIdAsync(taggedId);

        //    serviceModel.TaggedFriends.Add(taggedFriend);

        //    //Remove tagged user from user friends list of view model
        //    //The concept is that if any of the tagged users exist in this collection, 
        //    //it does not make sense to be tagged twice in one post.
        //    taggedFriend = serviceModel.UntaggedFriends
        //            .FirstOrDefault(u => u.Id == taggedFriend.Id);
        //    serviceModel.UntaggedFriends.Remove(taggedFriend);

        //    TempData.Keep("tagFriendsServiceModel");
        //    TempData["tagFriendsServiceModel"] = JsonConvert.SerializeObject(serviceModel);

        //    return RedirectToAction(
        //       actionName,
        //       controllerName,
        //       new { postId = serviceModel.PostId });
        //}

        public async Task<IActionResult> TagFriendAsync(int? id, string taggedId)
        {
            if (taggedId == null ||
                id == null)
            {
                return NotFound();
            }

            var taggerId = this._userService
                .GetUserId(User);

            if (TempData.ContainsKey("Posts"))
            {
                await this._taggedUserService.TagFriendPost(taggerId, taggedId, (int)id);

                var invokedFrom = TempData
                   .Get<string>("Posts")
                   .Trim('"');

                return RedirectToAction(
                    invokedFrom,
                    "Posts",
                    new { id = id });
            }
            if (TempData.ContainsKey("Comments"))
            {
                await this._taggedUserService.TagFriendComment(taggerId, taggedId, (int)id);

                var invokedFrom = TempData
                       .Get<string>("Comments")
                       .Trim('"');

                return RedirectToAction(
                    invokedFrom,
                    "Comments",
                    new { id = id });
            }
            return NotFound();
        }

        public async Task<IActionResult> RemoveTaggedFriendAsync(string taggedId, int? id)
        {
            if (taggedId == null ||
                id == null)
            {
                return NotFound();
            }

            if (TempData.ContainsKey("Posts"))
            {
                await this._taggedUserService.RemoveTaggedFriendPost(taggedId, (int)id);

                var invokedFrom = TempData
                  .Get<string>("Posts")
                  .Trim('"');

                return RedirectToAction(
                    invokedFrom,
                    "Posts",
                    new { id = id });
            }

            if (TempData.ContainsKey("Comments"))
            {
                await this._taggedUserService.RemoveTaggedFriendComment(taggedId, (int)id);

                var invokedFrom = TempData
                  .Get<string>("Comments")
                  .Trim('"');

                return RedirectToAction(
                    invokedFrom,
                    "Comments",
                    new { id = id });
            }

            return NotFound();
        }
    }
}