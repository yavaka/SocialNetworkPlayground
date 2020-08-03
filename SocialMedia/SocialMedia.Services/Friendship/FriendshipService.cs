namespace SocialMedia.Services.Friendship
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FriendshipService : IFriendshipService
    {
        private readonly SocialMediaDbContext _data;

        public FriendshipService(SocialMediaDbContext data)=>this._data = data;

        public async Task<IList<UserServiceModel>> GetFriendsAsync(string userId)
        {
            //Get current user friendships where it is addressee or requester
            var friendships = await GetFriendshipsByUserIdAsync(userId);

            var addressee = friendships
                .Select(a => a.Addressee)
                .ToList();

            var requesters = friendships
                .Select(r => r.Requester)
                .ToList();

            var friends = addressee
                .Concat(requesters)
                .ToList();

            friends.RemoveAll(u => u.Id == userId);

            return friends;
        }

        public async Task<ICollection<UserServiceModel>> GetNonFriendsAsync(string userId)
        {
            var nonFriends = await this._data.Users
                .Where(i => i.Id != userId)
                .Select(u => new UserServiceModel(u))
                .ToListAsync();

            var friends = await GetFriendsAsync(userId);
            var pendingRequests = await GetPendingRequestsAsync(userId);
            var friendRequests = await GetFriendRequestsAsync(userId);

            nonFriends.RemoveAll(u =>
                friends.Any(f => f.Id == u.Id));
            nonFriends.RemoveAll(u =>
                pendingRequests.Any(f => f.Id == u.Id));
            nonFriends.RemoveAll(u =>
                friendRequests.Any(f => f.Id == u.Id));

            return nonFriends;
        }

        private async Task<IEnumerable<FriendshipServiceModel>> GetFriendshipsByUserIdAsync(string userId)
           => await this._data.Friendships
                .Where(i => i.AddresseeId == userId && i.Status == Status.Accepted ||
                            i.RequesterId == userId && i.Status == Status.Accepted)
                .Select(f => new FriendshipServiceModel
                {
                    Addressee = new UserServiceModel(f.Addressee),
                    Requester = new UserServiceModel(f.Requester)
                })
                .ToListAsync();

        public async Task<ServiceModelFRStatus> GetFriendshipStatusAsync(string currentUserId, string userId)
        {
            var friendship = await this._data
                .Friendships
                    .FirstOrDefaultAsync(u =>
                        u.AddresseeId == currentUserId && u.RequesterId == userId ||
                        u.AddresseeId == userId && u.RequesterId == currentUserId);

            if (friendship == null)
            {
                return ServiceModelFRStatus.NonFriends;
            }

            switch (friendship.Status)
            {
                case Status.Accepted:
                    return ServiceModelFRStatus.Accepted;
                case Status.Pending:
                    return ServiceModelFRStatus.Pending;
            }
            return ServiceModelFRStatus.Request;
        }

        public async Task<IEnumerable<UserServiceModel>> GetFriendRequestsAsync(string currentUserId)
        => await this._data
            .Friendships
                .Where(a => a.AddresseeId == currentUserId && a.Status == Status.Pending)
                .Select(r => new UserServiceModel(r.Requester))
                .ToListAsync();

        public async Task<IEnumerable<UserServiceModel>> GetPendingRequestsAsync(string currentUserId)
        => await this._data.Friendships
                .Where(r => r.RequesterId == currentUserId && r.Status == Status.Pending)
                .Select(a => new UserServiceModel(a.Addressee))
                .ToListAsync();

        public async Task<EntityState> SendRequestAsync(string currentUserId, string addresseeId)
        {
            var friendship = new Friendship()
            {
                AddresseeId = addresseeId,
                RequesterId = currentUserId,
                Status = Status.Pending
            };

            if (await IsFriendshipExistAsync(currentUserId, addresseeId))
            {
                return EntityState.Unchanged;
            }

            await this._data.Friendships.AddAsync(friendship);
            await this._data.SaveChangesAsync();

            return EntityState.Added;
        }
       
        private async Task<bool> IsFriendshipExistAsync(string currentUserId, string addresseeId)
        => await this._data
            .Friendships
                .AnyAsync(u => u.RequesterId == currentUserId &&
                            u.AddresseeId == addresseeId);

        public async Task<EntityState> AcceptRequestAsync(string currentUserId, string requesterId)
        {
            var friendship = await GetFriendshipAsync(requesterId, currentUserId);

            if (friendship.Status == Status.Pending)
            {
                friendship.Status = Status.Accepted;

                await this._data.SaveChangesAsync();

                return EntityState.Modified;
            }
            return EntityState.Unchanged;
        }

        public async Task<EntityState> RejectRequestAsync(string currentUserId, string requesterId)
        {
            var friendship = await GetFriendshipAsync(requesterId, currentUserId);

            return await RemoveFriendshipAsync(friendship);
        }

        public async Task<EntityState> CancelInvitationAsync(string currentUserId, string addresseeId)
        {
            var friendship = await GetFriendshipAsync(currentUserId, addresseeId);

            return await RemoveFriendshipAsync(friendship);
        }

        public async Task<EntityState> UnfriendAsync(string currentUserId, string friendId)
        {
            var friendship = await GetFriendshipAsync(currentUserId, friendId) == null ? 
                await GetFriendshipAsync(friendId, currentUserId) : 
                await GetFriendshipAsync(currentUserId, friendId);

            return await RemoveFriendshipAsync(friendship);
        }

        private async Task<Friendship> GetFriendshipAsync(string requesterId, string addresseeId)
        => await this._data
            .Friendships
                .FirstOrDefaultAsync(i => i.RequesterId == requesterId &&
                                        i.AddresseeId == addresseeId);

        private async Task<EntityState> RemoveFriendshipAsync(Friendship friendship) 
        {
            if (friendship != null)
            {
                this._data.Friendships.Remove(friendship);

                await this._data.SaveChangesAsync();

                return EntityState.Deleted;
            }
            return EntityState.Unchanged;
        }

    }
}
