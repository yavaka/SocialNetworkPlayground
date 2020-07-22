namespace SocialMedia.Services.Comment
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Services.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICommentService
    {
        Task<EntityState> AddComment(CommentServiceModel commentServiceModel);

        Task<EntityState> EditComment(CommentServiceModel serviceModel);

        Task<EntityState> DeleteComment(int id);

        Task<CommentServiceModel> GetComment(int id);

        Task<ICollection<CommentServiceModel>> GetCommentsByPostIdAsync(int postId);
    }
}
