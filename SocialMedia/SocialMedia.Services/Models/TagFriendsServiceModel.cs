namespace SocialMedia.Services.Models
{
    using System.Collections.Generic;

    public class TagFriendsServiceModel
    {
        public ICollection<UserServiceModel> UntaggedFriends { get; set; }

        public ICollection<UserServiceModel> TaggedFriends { get; set; }

        public int? PostId { get; set; }
    }
}
