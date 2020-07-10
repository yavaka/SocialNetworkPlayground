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

    public class PostsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IFriendshipService _friendshipService;
        private readonly IPostService _postService;

        public PostsController(
            UserManager<User> userManager,
            IFriendshipService friendshipService,
            IPostService postService)
        {
            this._userManager = userManager;
            this._friendshipService = friendshipService;
            this._postService = postService;
        }


        //#region Posts

        //// GET: UserPosts
        //public async Task<IActionResult> UserPosts()
        //{
        //    var user = await this._userManager.GetUserAsync(User);

        //    TempData["userPost"] = "true";

        //    var posts = await _context.Posts
        //        .Where(a => a.AuthorId == user.Id && a.GroupId == null)
        //        .ToListAsync();

        //    return View(posts);
        //}
        //#endregion

        //// GET: Posts/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    var user = await this._userManager.GetUserAsync(User);

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //Gets the post and its tag friend entities
        //    var post = await _context.Posts
        //        .Include(i => i.TaggedUsers)
        //        .Include(i => i.Author)
        //        .FirstOrDefaultAsync(m => m.PostId == id);

        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewModel = new PostTagFriendsViewModel()
        //    {
        //        CurrentUser = user,
        //        Post = post,
        //        Tagged = GetTaggedFriends(post.PostId, user.Id)
        //    };


        //    //Pass current postId to CommentsController
        //    TempData["postId"] = id;

        //    return View(ViewModel);
        //}

        // GET: Posts/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await this._userManager.GetUserAsync(User);

            var postViewModel = new PostViewModel
            {
                CurrentUser = new UserServiceModel(currentUser),
            };

            if (TempData.ContainsKey("Group"))
            {
                postViewModel.Group = TempData.Get<Group>("Group");
                TempData.Keep("Group");
            }

            //Locally tagged friends
            if (TempData.ContainsKey("tagFriendsServiceModel"))
            {
                postViewModel.TagFriends = TempData.Get<TagFriendsServiceModel>("tagFriendsServiceModel");
                TempData.Keep("tagFriendsServiceModel");
            }
            else
            {
                postViewModel.TagFriends = new TagFriendsServiceModel()
                {
                    UntaggedFriends = await this._friendshipService
                            .GetFriendsAsync(currentUser.Id),
                    TaggedFriends = new List<UserServiceModel>()
                };
                TempData.Set<TagFriendsServiceModel>(
                    "tagFriendsServiceModel",
                    postViewModel.TagFriends);
            }

            if (!TempData.ContainsKey("invokedFrom"))
            {
                TempData["invokedFrom"] = "Create";
            }

            return View(postViewModel);
        }

        // POST: Posts/Create
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
                if (TempData.ContainsKey("Group"))
                {
                    viewModel.Group = TempData.Get<Group>("Group");
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

                if (viewModel.Group != null)
                {
                    return RedirectToAction("Details", "Groups", new { id = viewModel.Group.GroupId });
                }
                return RedirectToAction("Index", "Profile");
            }
            return View(viewModel);
        }

        ////GET: Posts/Edit/5
        //public async Task<IActionResult> Edit(int? id, string invokedFrom)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts.FindAsync(id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await this._userManager.GetUserAsync(User);

        //    ViewModel = new PostTagFriendsViewModel();
        //    ViewModel.CurrentUser = user;
        //    ViewModel.Post = post;
        //    ViewModel.Tagged = GetTaggedFriends(post.PostId, user.Id);
        //    /*If this method is invoked before GetTaggedFriends, 
        //      there will add all of the current user`s friends.
        //      Let`s get that user x is already tagged from the creation of the post.
        //      It does not make sense the current user to be allowed to tag user x twice.*/
        //    ViewModel.UserFriends = GetUserFriends(user);
        //    ViewModel.Message = invokedFrom;

        //    return View(ViewModel);
        //}

        //// POST: Posts/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit([FromForm]PostTagFriendsViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var user = await this._userManager.GetUserAsync(User);
        //            ViewModel.CurrentUser = user;

        //            var post = await this._context.Posts
        //                .Include(i => i.TaggedUsers)
        //                .FirstOrDefaultAsync(i => i.PostId == viewModel.Post.PostId);

        //            post.Author = user;
        //            post.AuthorId = user.Id;
        //            post.Content = viewModel.Post.Content;

        //            //Local tag friend entities
        //            var tagFriendEntities = TagFriendEntities(post);
        //            //Connected tag friend entities (In the db)
        //            var postTagFriendEntities = post.TaggedUsers;
        //            //If there is a mismatch between any record in the Local collection will be deleted from the Connected collection 
        //            RemoveTaggedFriendRecords(postTagFriendEntities, tagFriendEntities);
        //            //If there is a mismatch between any record in the Connected collection will be added from the Local collection 
        //            AddLocalTaggedFriends(postTagFriendEntities, tagFriendEntities);

        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PostExists(viewModel.Post.PostId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        if (ViewModel.Message == "profile page")
        //        {
        //            ViewModel = new PostTagFriendsViewModel();
        //            return RedirectToAction("Index", "Profile");
        //        }

        //        ViewModel = new PostTagFriendsViewModel();
        //        return RedirectToAction(nameof(UserPosts));
        //    }

        //    return View(viewModel);
        //}

        ////TODO: The DELETE statement conflicted with the REFERENCE constraint "TagFriendsToPost_FK". The conflict occurred in database "SocialMedia", table "dbo.TagFriends", column 'PostId'.
        //// GET: Posts/Delete/5
        //public async Task<IActionResult> Delete(int? id, string invokedFrom)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .FirstOrDefaultAsync(m => m.PostId == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewModel.Message = invokedFrom;

        //    return View(post);
        //}

        //// POST: Posts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var post = await _context.Posts
        //        .Include(i => i.TaggedUsers)
        //        .Include(c => c.Comments)
        //        .FirstOrDefaultAsync(i => i.PostId == id);

        //    //Gets all comments tagged users
        //    var commentsTaggedUsers = GetCommentsTagFriendEntities(post.Comments);

        //    //Removes all tagged friends in this post and this post`s comments
        //    _context.TagFriends.RemoveRange(post.TaggedUsers.ToList());
        //    _context.TagFriends.RemoveRange(commentsTaggedUsers);
        //    _context.Posts.Remove(post);

        //    await _context.SaveChangesAsync();

        //    if (ViewModel.Message == "profile page")
        //    {
        //        ViewModel = new PostTagFriendsViewModel();
        //        return RedirectToAction("Index", "Profile");
        //    }

        //    return RedirectToAction(nameof(UserPosts));
        //}

        //private bool PostExists(int id)
        //{
        //    return _context.Posts.Any(e => e.PostId == id);
        //}

        //#endregion







        ////TODO: Comments service: GetCommentsTaggedUsers(Collection of Comments)
        //private ICollection<TagFriends> GetCommentsTagFriendEntities(ICollection<Comment> postComments)
        //{
        //    var taggedUsers = new List<TagFriends>();
        //    foreach (var commentId in postComments.Select(i => i.Id))
        //    {
        //        //Gets the comment with tagged users collection
        //        var comment = this._context.Comments
        //            .Include(i => i.TaggedUsers)
        //            .FirstOrDefault(i => i.Id == commentId);

        //        taggedUsers.AddRange(comment.TaggedUsers);
        //    }
        //    return taggedUsers;
        //}

        ////TODO: Tag friends service : Entities(Post post)
        //private ICollection<TagFriends> TagFriendEntities(Post post)
        //{
        //    if (ViewModel.Tagged.Count == 0)
        //    {
        //        return new List<TagFriends>();
        //    }

        //    foreach (var tagged in ViewModel.Tagged)
        //    {
        //        if (!post.TaggedUsers.Any(i => i.TaggedId == tagged.Id &&
        //                                    i.TaggerId == ViewModel.CurrentUser.Id &&
        //                                    i.PostId == post.PostId))
        //        {
        //            post.TaggedUsers.Add(new TagFriends()
        //            {
        //                TaggerId = ViewModel.CurrentUser.Id,
        //                TaggedId = tagged.Id,
        //            });
        //        }
        //    }
        //    //change it then
        //    return post.TaggedUsers;
        //}

        ////TODO: Tag friends service : Entities
        //private ICollection<TagFriends> TagFriendEntities()
        //{
        //    var tagFriendsEntities = new List<TagFriends>();
        //    foreach (var tagged in ViewModel.Tagged)
        //    {
        //        var tagFriendsEntity = new TagFriends()
        //        {
        //            TaggerId = ViewModel.CurrentUser.Id,
        //            TaggedId = tagged.Id,
        //        };
        //        tagFriendsEntities.Add(tagFriendsEntity);
        //    }
        //    return tagFriendsEntities;
        //}


        ////TODO:Tag friends service : RemoveTaggedFriendById(string id)
        //public IActionResult RemoveTaggedFriend(string taggedId)
        //{
        //    //Gets the tagged user
        //    var taggedUser = ViewModel.Tagged
        //        .FirstOrDefault(i => i.Id == taggedId);

        //    //Removes the tagged user 
        //    ViewModel.Tagged
        //        .Remove(taggedUser);

        //    //Adds already non tagged user in the UserFriends, 
        //    //so the current user can decide to tag it again in the same post.
        //    ViewModel.UserFriends.Add(taggedUser);

        //    return View("Edit", ViewModel);
        //}

        ////TODO:Tag friends service : GetTaggedFriendsByPostIdAndUserId(int postId)
        //private ICollection<User> GetTaggedFriends(int postId, string userId)
        //{
        //    //TagFriend entities where users are tagged by the current user
        //    var tagFriendsEntities = this._context.TagFriends
        //        .Where(i => i.PostId == postId && i.TaggerId == userId)
        //        .Include(i => i.Tagged)
        //        .ToList();

        //    if (tagFriendsEntities != null)
        //    {
        //        return tagFriendsEntities
        //            .Select(i => i.Tagged)
        //            .ToList();
        //    }

        //    return null;
        //}

        ////Remove TagFriend entity records while edit the post
        //private void RemoveTaggedFriendRecords(ICollection<TagFriends> postTaggedFriends, ICollection<TagFriends> tagFriendEntities)
        //{
        //    //Compare already tagged friends collection in the post and
        //    //edited tag friends collection 
        //    var removedTagFriendEntities = postTaggedFriends.Except(tagFriendEntities)
        //        .ToList();

        //    this._context.TagFriends.RemoveRange(removedTagFriendEntities);
        //}

        ////Add TagFriend entities which are on local collection to the db 
        //private void AddLocalTaggedFriends(ICollection<TagFriends> postTagFriendEntities, ICollection<TagFriends> tagFriendEntities)
        //{
        //    var addedTagFriendEntities = tagFriendEntities.Except(postTagFriendEntities)
        //        .ToList();

        //    this._context.TagFriends.AddRange(addedTagFriendEntities);
        //}

        ////TODO:Friendship service : GetUserFriends
        //private List<User> GetUserFriends(User currentUser)
        //{
        //    //Get current user friendships where it is addressee or requester
        //    var friendships = this._context.Friendships
        //        .Where(i => i.AddresseeId == currentUser.Id && i.Status == 1 ||
        //                    i.RequesterId == currentUser.Id && i.Status == 1)
        //        .Include(i => i.Addressee)
        //        .Include(i => i.Requester)
        //        .ToList();

        //    var friends = new List<User>();
        //    //Add all friends
        //    foreach (var friendship in friendships)
        //    {
        //        //If addressee is the current user, add requester if it is not already tagged
        //        if (friendship.AddresseeId == currentUser.Id &&
        //            !ViewModel.Tagged.Contains(friendship.Requester))
        //        {
        //            /*this._context.Users
        //                .FirstOrDefault(i => i.Id == friendship.RequesterId)*/
        //            friends.Add(friendship.Requester);
        //        }
        //        else if (friendship.RequesterId == currentUser.Id &&
        //            !ViewModel.Tagged.Contains(friendship.Addressee)) //If requester is current user, add addressee
        //        {
        //            //this._context.Users
        //            //    .FirstOrDefault(i => i.Id == friendship.AddresseeId)
        //            friends.Add(friendship.Addressee);
        //        }
        //    }

        //    return friends;
        //}
    }

}
