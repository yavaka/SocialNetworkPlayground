namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Services.Image;
    using SocialMedia.Services.ImageSharp;
    using SocialMedia.Services.Stream;
    using SocialMedia.Services.User;
    using SocialMedia.Web.Models;
    using System.Threading.Tasks;
    using static Services.Common.Constants;

    public class GalleryController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IStreamService _streamService;
        private readonly IUserService _userService;
        private readonly IImageSharpService _imageSharpService;

        public GalleryController(
            IImageService imageService,
            IStreamService streamService,
            IUserService userService,
            IImageSharpService imageSharpService)
        {
            this._imageService = imageService;
            this._streamService = streamService;
            this._userService = userService;
            this._imageSharpService = imageSharpService;
        }

        public async Task<IActionResult> IndexAsync(string userId)
        {
            var user = await this._userService
                .GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var images = this._imageService
                .GetAllThumbnailImagesByUserId(userId);

            return View(images);
        }

        [HttpGet]
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(IFormFileCollection files)
        {
            // If there is no uploaded images will return the view 
            if (files.Count == 0)
            {
                TempData["filesError"] = "There is no uploaded images";
                return View();
            }

            var userId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            foreach (var file in files)
            {
                if (file.Length > MAX_FILE_SIZE)
                {
                    TempData["filesError"] = "Image size cannot be more than 10MB!";
                }

                var memoryStream = await this._streamService
                    .CopyFileToMemoryStreamAsync(file);

                var imageServiceModel = new ImageServiceModel
                {
                    ImageTitle = file.FileName,
                    OriginalImageData = memoryStream.ToArray(),
                    UploaderId = userId
                };

                // Get current image to thumbnail size
                imageServiceModel.ThumbnailImageData = await this._imageSharpService
                    .ResizeTumbnailImageAsync(file);

                // Get current image to medium size
                imageServiceModel.MediumImageData = await this._imageSharpService
                    .ResizeMediumImageAsync(file);

                await this._imageService.AddImageAsync(imageServiceModel);
            }
            
            return RedirectToAction(
                "Index",
                new { userId = userId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImageAsync(int imageId)
        {
            if (!await this._imageService.IsImageExistAsync(imageId))
            {
                return NotFound($"Image cannot be found!");
            }
            return View(new ImageViewModel
            {
                ImageId = imageId,
                Base64Image = await this._imageService
                    .GetImageByIdAsync(imageId)
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int imageId)
        {
            var userId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            await this._imageService.DeleteImageAsync(imageId);

            return RedirectToAction(
                "Index",
                new { userId = userId });
        }
    }
}