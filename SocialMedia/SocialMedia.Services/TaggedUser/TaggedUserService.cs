namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class TaggedUserService : ITaggedUserService
    {
        private readonly SocialMediaDbContext _data;

        public TaggedUserService(SocialMediaDbContext data) => this._data = data;

        public async Task<IEnumerable<UserServiceModel>> GetTaggedUsersAsync(IEnumerable<string> tagFriendIds) 
            => await this._data
                .Users
                .Where(u => tagFriendIds.Contains(u.Id))
                .Select(u => new UserServiceModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName
                })
                .ToListAsync();
    }
}
