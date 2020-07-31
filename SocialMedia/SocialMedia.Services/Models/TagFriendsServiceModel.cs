namespace SocialMedia.Services.Models
{
    using System.Collections.Generic;

    public class TagFriendsServiceModel
    {
        public IList<UserServiceModel> Friends { get; set; }

        public IList<UserServiceModel> TaggedFriends { get; set; }

        public int? PostId { get; set; }
    }
}
