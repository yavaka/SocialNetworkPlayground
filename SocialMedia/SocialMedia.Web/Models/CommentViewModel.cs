namespace SocialMedia.Web.Models
{
    using SocialMedia.Services.Models;
    using System;

    public class CommentViewModel
    {
        public CommentViewModel()
        {
        }

        public CommentViewModel(CommentServiceModel comment)
        {
            this.CommentId = comment.CommentId;
            this.Content = comment.Content;
            this.DatePosted = comment.DatePosted;
            this.Author = comment.Author;
            this.PostId = comment.PostId;
        }
        public int CommentId { get; set; }

        public string Content { get; set; }

        public DateTime DatePosted { get; set; }

        public UserServiceModel Author { get; set; }

        public TagFriendsServiceModel TagFriends { get; set; }

        public int PostId { get; set; }
    }
}
