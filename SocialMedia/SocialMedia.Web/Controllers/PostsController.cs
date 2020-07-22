namespace SocialMedia.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Services.Models;
    using SocialMedia.Web.Models;
    using SocialMedia.Web.Infrastructure;
    using SocialMedia.Services.Post;
    using System;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.Comment;
    using System.Linq;

    public class PostsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IFriendshipService _friendshipService;
        private readonly IPostService _postService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;

        public PostsController(
            UserManager<User> userManager,
            IFriendshipService friendshipService,
            IPostService postService,
            ITaggedUserService taggedUserService,
            ICommentService commentService)
        {
            this._userManager = userManager;
            this._friendshipService = friendshipService;
            this._postService = postService;
            this._taggedUserService = taggedUserService;
            this._commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await this._userManager.GetUserAsync(User);

            var viewModel = new PostViewModel
            {
                CurrentUser = new UserServiceModel(currentUser),
            };

            if (TempData.ContainsKey("group"))
            {
                viewModel.Group = TempData.Get<Group>("group");
                TempData.Keep("group");
            }

            //Locally tagged friends
            if (TempData.ContainsKey("tagFriendsServiceModel"))
            {
                viewModel.TagFriends = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");
                TempData.Keep("tagFriendsServiceModel");
            }
            else
            {
                viewModel.TagFriends = new TagFriendsServiceModel()
                {
                    UntaggedFriends = await this._friendshipService
                            .GetFriendsAsync(currentUser.Id),
                    TaggedFriends = new List<UserServiceModel>()
                };
                TempData.Set<TagFriendsServiceModel>(
                    "tagFriendsServiceModel",
                    viewModel.TagFriends);
            }

            if (!TempData.ContainsKey("Posts"))
            {
                TempData.Set("Posts", "Create");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Get locally tagged friends
                if (TempData.ContainsKey("tagFriendsServiceModel"))
                {
                    viewModel.TagFriends = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");
                }

                //Get group where the post is going to be created
                if (TempData.ContainsKey("group"))
                {
                    viewModel.Group = TempData.Get<Group>("group");
                }

                var currentUser = await this._userManager.GetUserAsync(User);

                await this._postService
                    .AddPost(new PostServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = new UserServiceModel(currentUser),
                        Group = viewModel.Group,
                        TaggedFriends = viewModel.TagFriends.TaggedFriends
                    });

                TempData.Clear();

                if (viewModel.Group != null)
                {
                    return RedirectToAction("Details", "Groups", new { id = viewModel.Group.GroupId });
                }
                return RedirectToAction("Index", "Profile");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await this._postService.GetPost(id);

            if (post == null)
            {
                return NotFound();
            }

            var viewModel = new PostViewModel(post);

            viewModel.CurrentUser = new UserServiceModel(
                await this._userManager.GetUserAsync(User));


            var friends = await this._friendshipService
                .GetFriendsAsync(viewModel.CurrentUser.Id);

            if (post.TaggedFriends.Count > 0)
            {
                viewModel.TagFriends = new TagFriendsServiceModel
                {
                    UntaggedFriends = this._taggedUserService
                        .GetUntaggedFriends(post.TaggedFriends, friends),
                    TaggedFriends = post.TaggedFriends,
                };
            }
            else
            {
                viewModel.TagFriends = new TagFriendsServiceModel
                {
                    UntaggedFriends = await this._friendshipService
                        .GetFriendsAsync(viewModel.CurrentUser.Id),
                    TaggedFriends = new List<UserServiceModel>(),
                };
            }

            if (!TempData.ContainsKey("Posts"))
            {
                TempData.Set("Posts", "Edit");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Get group where the post is for
                if (TempData.ContainsKey("group"))
                {
                    viewModel.Group = TempData.Get<Group>("group");
                }

                await this._postService
                    .EditPost(new PostServiceModel
                    {
                        PostId = viewModel.PostId,
                        Content = viewModel.Content
                    });

                TempData.Clear();

                if (viewModel.Group != null)
                {
                    return RedirectToAction("Details", "Groups", new { id = viewModel.Group.GroupId });
                }
                return RedirectToAction("Index", "Profile");
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
         {
            var post = await this._postService
                .GetPost(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comments = await this._commentService
                .GetCommentsByPostIdAsync(id);

           //Send collection of all comments ids and delete the tagged friends 
            await this._taggedUserService.DeleteTaggedFriendsInComments(comments
                .Select(i =>i.CommentId)
                .ToList());

            await this._taggedUserService.DeleteTaggedFriendsPostId(id);
            await this._postService.DeletePost(id);

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
