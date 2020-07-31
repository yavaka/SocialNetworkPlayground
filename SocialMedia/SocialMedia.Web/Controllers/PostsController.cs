namespace SocialMedia.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Services.Models;
    using SocialMedia.Web.Models;
    using SocialMedia.Services.Post;
    using System;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.Comment;
    using System.Linq;
    using SocialMedia.Services.User;

    public class PostsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IPostService _postService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;

        public PostsController(
            IFriendshipService friendshipService,
            IPostService postService,
            ITaggedUserService taggedUserService,
            ICommentService commentService,
            IUserService userService)
        {
            this._friendshipService = friendshipService;
            this._postService = postService;
            this._taggedUserService = taggedUserService;
            this._commentService = commentService;
            this._userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Create([FromQuery]int? groupId)
        {
            var currentUserId = this._userService
                .GetUserId(User);

            var viewModel = new PostViewModel
            {
                TagFriends = new TagFriendsServiceModel()
                {
                    Friends = await this._friendshipService
                    .GetFriendsAsync(currentUserId),
                    TaggedFriends = new List<UserServiceModel>()
                }
            };

            //Get group id from return url
            if (groupId != null)
            {
                viewModel.GroupId = (int)groupId;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await this._userService
                    .GetCurrentUserAsync(User);

                //Get tagged friends
                if (viewModel.TagFriends.Friends.Any(c => c.Checked == true))
                {
                    viewModel.TagFriends.TaggedFriends = viewModel.TagFriends.Friends
                        .Where(c => c.Checked == true)
                        .ToList();
                }

                await this._postService
                    .AddPost(new PostServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        GroupId = viewModel.GroupId,
                        TaggedFriends = viewModel.TagFriends.TaggedFriends
                    });

                //It will be redirected to Group/Details/{id}
                if (viewModel.GroupId > 0)
                {
                    return RedirectToAction("Details", "Groups", new { id = viewModel.GroupId });
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

            var currentUser = await this._userService
                .GetCurrentUserAsync(User);
            if (currentUser.Id != post.Author.Id)
            {
                return NotFound();
            }

            var viewModel = new PostViewModel
            {
                PostId = post.PostId,
                Content = post.Content,
                GroupId = post.GroupId,
                Author = post.Author,
            };

            var friends = await this._friendshipService
                .GetFriendsAsync(viewModel.Author.Id);

            if (post.TaggedFriends.Count > 0)
            {
                viewModel.TagFriends = new TagFriendsServiceModel
                {
                    //Compare tagged with untagged friends
                    //For those who are tagged set checked to true
                    Friends = this._taggedUserService
                        .GetUntaggedFriends(post.TaggedFriends.ToList(), friends.ToList())
                        .ToList(),
                    TaggedFriends = new List<UserServiceModel>()
                };
            }
            else
            {
                viewModel.TagFriends = new TagFriendsServiceModel
                {
                    Friends = friends,
                    TaggedFriends = new List<UserServiceModel>()
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var currentUserId = this._userService.GetUserId(User);

                viewModel.TagFriends.TaggedFriends = viewModel.TagFriends.Friends
                    .Where(c => c.Checked == true)
                    .ToList();

                await this._taggedUserService.UpdateTaggedFriendsInPostAsync(
                    viewModel.TagFriends.TaggedFriends,
                    viewModel.PostId,
                    currentUserId);

                await this._postService
                    .EditPost(new PostServiceModel
                    {
                        PostId = viewModel.PostId,
                        Content = viewModel.Content
                    });

                if (viewModel.GroupId != null)
                {
                    return RedirectToAction("Details", "Groups", new { id = viewModel.GroupId });
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
            //If groupId is not null it will be redirected to Group/Details/{groupId}
            var groupId = await this._postService.GetGroupIdOfPost(id);

            var comments = await this._commentService
                .GetCommentsByPostIdAsync(id);

            //Send collection of all comments ids and delete the tagged friends 
            await this._taggedUserService.DeleteTaggedFriendsInComments(comments
                .Select(i => i.CommentId)
                .ToList());

            await this._taggedUserService.DeleteTaggedFriendsPostId(id);
            await this._postService.DeletePost(id);


            if (groupId != null)
            {
                return RedirectToAction("Details", "Groups", new { id = (int)groupId });
            }
            return RedirectToAction("Index", "Profile");
        }
    }
}
