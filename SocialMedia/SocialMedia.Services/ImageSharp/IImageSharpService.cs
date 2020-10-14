namespace SocialMedia.Services.ImageSharp
{
    using Microsoft.AspNetCore.Http;
    using SocialMedia.Services.Common;
    using System.Threading.Tasks;

    public interface IImageSharpService : IService
    {
        Task<byte[]> ResizeTumbnailImageAsync(IFormFile file);
        Task<byte[]> ResizeMediumImageAsync(IFormFile file);
    }
}
