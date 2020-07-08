namespace SocialMedia.Services.Post
{
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPostService
    {
        Task<Post> GetPostByIdAsync(int postId);

        Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId);
    }
}
