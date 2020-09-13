namespace SocialMedia.Services.Image
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImageService : IImageService
    {
        private readonly SocialMediaDbContext _data;

        public ImageService(SocialMediaDbContext context) => this._data = context;

        public async Task AddImage(ImageServiceModel serviceModel)
        {
            if (serviceModel == null)
                throw new ArgumentException("Image cannot be null");

            var img = new Image
            {
                ImageTitle = serviceModel.ImageTitle,
                ImageData = serviceModel.ImageData,
                UploaderId = serviceModel.UploaderId,
                IsAvatar = serviceModel.IsAvatar
            };

            this._data.Images.Add(img);
            await this._data.SaveChangesAsync();
        }

        public async Task DeleteAvatar(string userId)
        {
            if (IsThereAvatar(userId))
            {
                var avatar = GetAllImagesEntities(userId)
                    .FirstOrDefault(a => a.IsAvatar);

                this._data.Images.Remove(avatar);
                await this._data.SaveChangesAsync();
            }
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await GetImageEntityAsync(imageId);

            this._data.Images.Remove(image);
            await this._data.SaveChangesAsync();
        }

        public bool IsThereAvatar(string userId)
        => GetAllImagesEntities(userId)
            .Any(a => a.IsAvatar == true);

        public async Task<bool> IsImageExistAsync(int imageId)
        => await this._data.Images
            .AnyAsync(i => i.Id == imageId);

        public string GetAvatar(string userId)
        {
            if (IsThereAvatar(userId))
            {
                var avatar = GetAllImagesEntities(userId)
                    .FirstOrDefault(a => a.IsAvatar);

                return GetImageDataUrl(avatar.ImageData);
            }
            return null;
        }

        public async Task<IEnumerable<string>> GetAllImagesByUserIdAsync(string userId)
        {
            var imageEntities = await this._data.Images
                .Where(u => u.UploaderId == userId && !u.IsAvatar)
                .Select(i => i.ImageData)
                .ToListAsync();

            if (imageEntities.Count > 0)
            {
                var images = new List<string>();

                foreach (var image in imageEntities)
                {
                    images.Add(GetImageDataUrl(image));
                }
                return images;
            }
            return null;
        }

        private ICollection<Image> GetAllImagesEntities(string userId)
         => this._data.Images
            .Where(i => i.UploaderId == userId)
            .ToList();

        private string GetImageDataUrl(byte[] imageData)
        {
            var imageBase64 = Convert.ToBase64String(imageData);

            return string.Format($"data:image/jpg;base64,{imageBase64}");
        }

        private async Task<Image> GetImageEntityAsync(int imageId)
        => await this._data.Images
            .FirstOrDefaultAsync(i => i.Id == imageId);
    }
}
