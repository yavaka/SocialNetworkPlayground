namespace SocialMedia.Services.Models
{
    using System;
    using System.Collections.Generic;
    using SocialMedia.Data.Models;

    public class CommentServiceModel
    {
        public CommentServiceModel()
        {
        }

        public CommentServiceModel(Comment comment)
        {
            this.CommentId = comment.Id;
            this.Content = comment.Content;
            this.DatePosted = comment.DatePosted;
            this.Author = new UserServiceModel(comment.Author);
            this.PostId = comment.CommentedPostId;
        }

        public int CommentId { get; set; }

        public string Content { get; set; }

        public DateTime DatePosted { get; set; }

        public UserServiceModel Author { get; set; }

        public int PostId { get; set; }
        
        public ICollection<UserServiceModel> TaggedFriends{ get; set; }
    }
}
