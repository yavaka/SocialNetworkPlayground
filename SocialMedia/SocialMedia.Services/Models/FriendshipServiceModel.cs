namespace SocialMedia.Services.Models
{
    using System.Collections.Generic;
    
    public class FriendshipServiceModel
    {
        public UserServiceModel Addressee { get; set; }

        public UserServiceModel Requester { get; set; }

        //0 = Pending
        //1 = Accepted
        public int Status { get; set; }

        /// <summary>
        /// Users who sent request to the current user
        /// </summary>
        public IEnumerable<UserServiceModel> Requests { get; set; }

        /// <summary>
        /// Current user`s pending requests
        /// </summary>
        public IEnumerable<UserServiceModel> PendingRequests { get; set; }
    }
}
