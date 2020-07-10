namespace SocialMedia.Services.Models
{
    using SocialMedia.Data.Models;
    using SocialMedia.Models.ViewModels;
    using System;
    using System.Collections.Generic;

    public class PostServiceModel
    {
        public PostServiceModel()
        {
            this.TaggedFriends = new List<UserServiceModel>();
            this.Comments = new List<CommentTagFriendsViewModel>();
        }

        public int PostId{ get; set; }

        public string Content { get; set; }

        public DateTime DatePosted{ get; set; }
       
        public UserServiceModel Author { get; set; }
       
        public Group Group { get; set; }////// Change with GroupServiceModel

        public ICollection<UserServiceModel> TaggedFriends{ get; set; }

        public ICollection<CommentTagFriendsViewModel> Comments{ get; set; }////// Change with CommentServiceModel
    }
}
