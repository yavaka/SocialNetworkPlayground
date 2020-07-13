namespace SocialMedia.Services.Post
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Services.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPostService
    {
        Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId);

        Task<EntityState> AddPost(PostServiceModel serviceModel);

        Task<EntityState> EditPost(PostServiceModel serviceModel);

        Task<PostServiceModel> GetPost(int id);

    }
}
