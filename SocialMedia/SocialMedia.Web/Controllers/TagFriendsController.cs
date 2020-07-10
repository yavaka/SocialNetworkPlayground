namespace SocialMedia.Web.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using SocialMedia.Web.Infrastructure;

    public class TagFriendsController : Controller
    {
        private readonly UserManager<User> _userManager;

        public TagFriendsController(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }

        public IActionResult AddTaggedFriendLocal(string taggedId)
        {
            var serviceModel = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");

            //Check is the last tagged friend is already been tagged.
            if (serviceModel.TaggedFriends.Count > 0)
            {
                if (serviceModel.TaggedFriends.Last().Id == taggedId)
                {
                    return RedirectToAction(
                        TempData["invokedFrom"].ToString(),
                        "Posts");
                }
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
                    .FirstOrDefault(u =>u.Id == taggedFriend.Id);
            serviceModel.UntaggedFriends.Remove(taggedFriend);

            TempData.Keep("tagFriendsServiceModel");
            TempData["tagFriendsServiceModel"] = JsonConvert.SerializeObject(serviceModel);
            
            return RedirectToAction(
                TempData["invokedFrom"].ToString(),
                "Posts");
        }

        //public IActionResult AddTaggedFriendLocal(CommentViewModel viewModel, string taggedId, string invokedFrom)
    }
}