using Microsoft.EntityFrameworkCore;
using SocialMedia.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Services.Comment
{
    public interface ICommentService
    {
        Task<EntityState> AddComment(CommentServiceModel commentServiceModel);
        
        Task<EntityState> EditComment(CommentServiceModel serviceModel);
       
        Task<EntityState> RemoveComment(int id);

        Task<CommentServiceModel> GetComment(int id);

        Task<ICollection<CommentServiceModel>> GetCommentsByPostIdAsync(int postId);
    }
}
