namespace SocialMedia.Services.Post
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Services.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPostService
    {
        Task<EntityState> AddPost(PostServiceModel serviceModel);

        Task<EntityState> EditPost(PostServiceModel serviceModel);
        
        Task<EntityState> DeletePost(int id);

        Task<PostServiceModel> GetPost(int id);

        Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId);

        Task<ICollection<PostServiceModel>> GetPostsByGroupIdAsync(int groupId);
    }
}
