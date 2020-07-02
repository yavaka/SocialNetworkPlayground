using System.Collections.Generic;

namespace SocialMedia.Models.ViewModels
{
    using Services.Models;

    public class CommentTagFriendsViewModel
    {
        public CommentTagFriendsViewModel() => this.UserFriends = new List<User>();

        public Comment Comment { get; set; }

        public User CurrentUser { get; set; }

        public ICollection<User> UserFriends { get; set; }

        public IEnumerable<UserServiceModel> Tagged { get; set; }

        public string Message { get; set; }
    }
}
