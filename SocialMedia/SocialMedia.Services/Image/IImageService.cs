namespace SocialMedia.Services.Image
{
    using SocialMedia.Services.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IImageService : IService
    {
        Task AddImage(ImageServiceModel serviceModel);
        Task DeleteAvatar(string userId);
        Task DeleteImageAsync(int imageId);
        string GetAvatar(string userId);
        Task<IEnumerable<string>> GetAllImagesByUserIdAsync(string userId);
        bool IsThereAvatar(string userId);
        Task<bool> IsImageExistAsync(int imageId);
    }
}
