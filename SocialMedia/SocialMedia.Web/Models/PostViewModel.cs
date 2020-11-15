namespace SocialMedia.Web.Models
{
    using Newtonsoft.Json.Linq;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.User;
    using System;

    public class PostViewModel
    {
        public int PostId { get; set; }

        public string Content { get; set; }

        public DateTime? DatePosted { get; set; }

        public UserServiceModel Author { get; set; }

        public int? GroupId { get; set; }

        // It comes as a JSON
        public string TaggedFriends { get; set; }

        public TagFriendsServiceModel TagFriends{ get; set; }
    }
}
