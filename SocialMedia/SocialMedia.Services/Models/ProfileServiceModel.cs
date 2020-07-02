namespace SocialMedia.Services.Models
{
    using System.Collections.Generic;
    using SocialMedia.Models.ViewModels;

    public class ProfileServiceModel
    {
        public ProfileServiceModel()
        {
            this.Posts = new List<PostTagFriendsViewModel>();
        }

        public UserServiceModel CurrentUser { get; set; }

        public ICollection<PostTagFriendsViewModel> Posts { get; set; }
    }
}
