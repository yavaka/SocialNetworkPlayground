namespace SocialMedia.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Data.Models;
    using SocialMedia.Web.Models;
    using SocialMedia.Services.Models;
    using SocialMedia.Web.Infrastructure;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Services.Comment;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.User;

    public class CommentsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ICommentService _commentService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly IUserService _userService;

        public CommentsController(
            IFriendshipService friendshipService,
            ICommentService commentService,
            ITaggedUserService taggedUserService,
            IUserService userService)
        {
            this._friendshipService = friendshipService;
            this._commentService = commentService;
            this._taggedUserService = taggedUserService;
            this._userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int postId)
        {
            var currentUser = await this._userService
                .GetCurrentUserAsync(User);

            var viewModel = new CommentViewModel
            {
                Author = currentUser,
                PostId = postId
            };

            //Locally tagged friends
            if (TempData.ContainsKey("tagFriendsServiceModel"))
            {
                viewModel.TagFriends = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");
                TempData.Keep("tagFriendsServiceModel");
            }
            else
            {
                //viewModel.TagFriends = new TagFriendsServiceModel()
                //{
                //    UntaggedFriends = await this._friendshipService
                //            .GetFriendsAsync(currentUser.Id),
                //    TaggedFriends = new List<UserServiceModel>(),
                //    PostId = postId
                //};
                TempData.Set<TagFriendsServiceModel>(
                    "tagFriendsServiceModel",
                    viewModel.TagFriends);
            }

            if (!TempData.ContainsKey("Comments"))
            {
                TempData.Set("Comments", "Create");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Get locally tagged friends
                if (TempData.ContainsKey("tagFriendsServiceModel"))
                {
                    viewModel.TagFriends = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");
                }

                var currentUser = await this._userService
                    .GetCurrentUserAsync(User);

                await this._commentService
                    .AddComment(new CommentServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        PostId = viewModel.PostId,
                        TaggedFriends = viewModel.TagFriends.TaggedFriends
                    });

                if (TempData.ContainsKey("group"))
                {
                    var group = TempData.Get<Group>("group");
                    TempData.Clear();
                    return RedirectToAction(
                        "Details", "Groups", new { groupId = group.GroupId });
                }

                if (TempData.ContainsKey("userId"))
                {
                    var userId = TempData.Get<string>("userId");
                    TempData.Clear();
                    return RedirectToAction("Index", "Profile", new { userId = userId });
                }
                TempData.Clear();
                return RedirectToAction("Index", "Profile");
            }
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var comment = await this._commentService
                .GetComment(id);

            if (comment == null)
            {
                return NotFound();
            }

            var viewModel = new CommentViewModel(comment);

            var friends = await this._friendshipService
                .GetFriendsAsync(viewModel.Author.Id);

            if (comment.TaggedFriends.Count > 0)
            {
                //viewModel.TagFriends = new TagFriendsServiceModel
                //{
                //    UntaggedFriends = this._taggedUserService
                //        .GetUntaggedFriends(comment.TaggedFriends, friends),
                //    TaggedFriends = comment.TaggedFriends
                //};
            }
            else
            {
                //viewModel.TagFriends = new TagFriendsServiceModel
                //{
                //    UntaggedFriends = await this._friendshipService
                //        .GetFriendsAsync(viewModel.Author.Id),
                //    TaggedFriends = new List<UserServiceModel>()
                //};
            }

            if (!TempData.ContainsKey("Comments"))
            {
                TempData.Set("Comments", "Edit");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await this._commentService
                    .EditComment(new CommentServiceModel 
                    {
                        CommentId = viewModel.CommentId,
                        Content = viewModel.Content
                    });

                var group = new Group();
                var userId = string.Empty;

                if (TempData.ContainsKey("group"))
                {
                    group = TempData.Get<Group>("group");
                }
                else if (TempData.ContainsKey("userId"))
                {
                    userId = TempData.Get<string>("userId");
                }

                TempData.Clear();
                if (group.GroupId > 0)
                {
                    return RedirectToAction("Details", "Groups", new { id = group.GroupId });
                }
                else if(userId != string.Empty)
                {
                   return RedirectToAction("Index", "Profile", new { userId = userId});
                }
                return RedirectToAction("Index", "Profile");
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await this._commentService
                .GetComment((int)id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await this._taggedUserService.DeleteTaggedFriendsCommentId(id);
            await this._commentService.DeleteComment(id);

            var group = new Group();
            var userId = string.Empty;

            if (TempData.ContainsKey("group"))
            {
                group = TempData.Get<Group>("group");
            }
            else if (TempData.ContainsKey("userId"))
            {
                userId = TempData.Get<string>("userId");
            }

            TempData.Clear();
            if (group.GroupId > 0)
            {
                return RedirectToAction("Details", "Groups", new { id = group.GroupId });
            }
            else if (userId != string.Empty)
            {
                return RedirectToAction("Index", "Profile", new { userId = userId });
            }
            return RedirectToAction("Index", "Profile");
        }
    }
}
