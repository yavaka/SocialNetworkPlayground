namespace SocialMedia.Web.Models
{
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using System;

    public class PostViewModel
    {
        public PostViewModel()
        {
        }

        public PostViewModel(PostServiceModel post)
        {
            this.PostId = post.PostId;
            this.Content = post.Content;
            this.DatePosted = post.DatePosted;
            this.Author = post.Author;
            this.Group = post.Group;
        }

        public int PostId { get; set; }

        public string Content { get; set; }

        public DateTime? DatePosted { get; set; }

        public UserServiceModel Author { get; set; }

        public Group Group { get; set; }

        public UserServiceModel CurrentUser { get; set; }

        public TagFriendsServiceModel TagFriends{ get; set; }
    }
}
