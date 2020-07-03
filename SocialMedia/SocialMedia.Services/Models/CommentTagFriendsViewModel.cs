namespace SocialMedia.Models.ViewModels
{
    using System.Collections.Generic;
    using Services.Models;
    using SocialMedia.Data.Models;

    public class CommentTagFriendsViewModel
    {
        public CommentTagFriendsViewModel() => this.UserFriends = new List<User>();

        public Comment Comment { get; set; }

        public User CurrentUser { get; set; }

        public ICollection<User> UserFriends { get; set; }

        public ICollection<User> Tagged { get; set; }

        public string Message { get; set; }
    }
}
