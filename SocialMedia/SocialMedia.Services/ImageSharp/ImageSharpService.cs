namespace SocialMedia.Services.ImageSharp
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Processing;
    using static SocialMedia.Services.Common.Constants;

    public class ImageSharpService : IImageSharpService
    {
        public async Task<byte[]> ResizeTumbnailImageAsync(IFormFile file)
        {
            using var inputMS = new MemoryStream();
            using var outputMS = new MemoryStream();

            // Copy image to input memory stream
            await file.CopyToAsync(inputMS);

            inputMS.Position = 0;

            // Load the image with ImageSharp
            using var image = SixLabors.ImageSharp.Image
                .Load(inputMS);

            // Resize the image and use Lanczos3 sampler for 
            // high quality thumbnails due to it's sharpening effect
            image.Mutate(x => x.Resize(THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT, KnownResamplers.Lanczos3));

            // Save the resized image on the memory stream
            await image.SaveAsJpegAsync(outputMS);

            return outputMS.ToArray();
        }

        public async Task<byte[]> ResizeMediumImageAsync(IFormFile file)
        {
            using var inputMS = new MemoryStream();
            using var outputMS = new MemoryStream();

            // Copy image to input memory stream
            await file.CopyToAsync(inputMS);

            inputMS.Position = 0;

            // Load the image with ImageSharp
            using var image = SixLabors.ImageSharp.Image
                .Load(inputMS);

            // Resize the image with ImageSharp and use defaul sampler 
            // Bicubic which offers good quality
            image.Mutate(x => x.Resize(MEDIUM_WIDTH, MEDIUM_HEIGHT));

            // Save the resized image on the memory stream
            image.SaveAsJpeg(outputMS);

            return outputMS.ToArray();
        }
    }
}
