namespace SocialMedia.Services.Models.ViewModels
{
    using SocialMedia.Data.Models;
    using System.Collections.Generic;

    public class PostViewModel
    {
        public PostViewModel()
        {
            this.Friends = new List<UserServiceModel>();
            this.TaggedFriends = new List<UserServiceModel>();
        }

        public Post Post { get; set; }

        public UserServiceModel CurrentUser { get; set; }

        public ICollection<UserServiceModel> Friends { get; set; }
     
        public ICollection<UserServiceModel> TaggedFriends { get; set; }
    }
}
