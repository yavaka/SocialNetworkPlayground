using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    using Services.Models;

    public class PostTagFriendsViewModel
    {
        public PostTagFriendsViewModel()
        {
            this.UserFriends = new List<User>();
            this.Tagged = new List<UserServiceModel>();
            this.Comments = new List<CommentTagFriendsViewModel>();
        }

        public Post Post { get; set; }

        public User CurrentUser { get; set; }

        public ICollection<User> UserFriends { get; set; }

        public IEnumerable<UserServiceModel> Tagged { get; set; }

        public ICollection<CommentTagFriendsViewModel> Comments{ get; set; }

        public string Message { get; set; }
    }
}
