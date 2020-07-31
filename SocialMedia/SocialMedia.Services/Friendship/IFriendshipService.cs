namespace SocialMedia.Services.Friendship
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Services.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFriendshipService
    {
        Task<IList<UserServiceModel>> GetFriendsAsync(string userId);

        Task<ICollection<UserServiceModel>> GetNonFriendsAsync(string userId);

        Task<int> GetFriendshipStatusAsync(string currentUserId, string secondUserId);

        Task<IEnumerable<UserServiceModel>> GetFriendRequestsAsync(string currentUserId);

        Task<IEnumerable<UserServiceModel>> GetPendingRequestsAsync(string currentUserId);

        Task<EntityState> SendRequestAsync(string currentUserId, string addresseeId);

        Task<EntityState> AcceptRequestAsync(string currentUserId, string requesterId);

        Task<EntityState> RejectRequestAsync(string currentUserId, string requesterId);

        Task<EntityState> CancelInvitationAsync(string currentUserId, string addresseeId);

        Task<EntityState> UnfriendAsync(string currentUserId, string friendId);
    }
}
