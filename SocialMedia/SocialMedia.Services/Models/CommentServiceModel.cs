namespace SocialMedia.Services.Models
{
    using System;
    using System.Collections.Generic;

    public class CommentServiceModel
    {
        public int CommentId { get; set; }

        public string Content { get; set; }

        public DateTime DatePosted { get; set; }

        public string AuthorId { get; set; }

        public int PostId { get; set; }
        
        public ICollection<UserServiceModel> TaggedFriends{ get; set; }
    }
}
