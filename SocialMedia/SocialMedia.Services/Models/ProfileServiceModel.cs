namespace SocialMedia.Services.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public class ProfileServiceModel
    {
        public ProfileServiceModel()
        {
            this.Posts = new List<PostServiceModel>();
        }
        
        public UserServiceModel User { get; set; }

        private ICollection<PostServiceModel> posts; 
        public ICollection<PostServiceModel> Posts 
        {
            get 
            {
                return this.posts
                    .OrderByDescending(d => d.DatePosted)
                    .ToList();
            }
            set 
            {
                this.posts = value;
            }
        }

        public string Message{ get; set; }
    }
}
