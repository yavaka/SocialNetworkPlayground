using SocialMedia.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Web.Models
{
    public class GroupViewModel
    {
        public GroupViewModel()
        {
            this.Members = new List<UserServiceModel>();
            this.Posts = new List<PostServiceModel>();
        }

        public int GroupId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public UserServiceModel Admin { get; set; }
        
        public string CurrentUserId{ get; set; }

        public bool IsCurrentUserMember { get; set; }

        public ICollection<UserServiceModel> Members { get; set; }

        public ICollection<PostServiceModel> Posts { get; set; }
    }
}
