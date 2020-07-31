namespace SocialMedia.Web.Models
{
    using SocialMedia.Services.Models;
    using System;

    public class PostViewModel
    {
        public int PostId { get; set; }

        public string Content { get; set; }

        public DateTime? DatePosted { get; set; }

        public UserServiceModel Author { get; set; }

        public int? GroupId { get; set; }

        public TagFriendsServiceModel TagFriends{ get; set; }
    }
}
