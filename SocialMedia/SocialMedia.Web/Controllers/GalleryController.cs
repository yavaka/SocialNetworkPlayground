namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Services.Image;
    using SocialMedia.Services.Stream;
    using SocialMedia.Services.User;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GalleryController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IStreamService _streamService;
        private readonly IUserService _userService;

        public GalleryController(
            IImageService imageService,
            IStreamService streamService,
            IUserService userService)
        {
            this._imageService = imageService;
            this._streamService = streamService;
            this._userService = userService;
        }

        public async Task<IActionResult> IndexAsync(string userId)
        {
            var user = await this._userService
                .GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var images = await this._imageService
                .GetAllImagesByUserIdAsync(userId);

            return View(images);
        }

        [HttpGet]
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(string _)
        {
            var userId = this._userService
                .GetUserId(User);

            var images = Request.Form.Files;

            if (images == null)
            {
                return RedirectToAction(
                    nameof(IndexAsync), 
                    new { userId = userId });
            }

            foreach (var image in images)
            {
                var memoryStream = await this._streamService
                    .CopyFileToMemoryStreamAsync(image);

                await this._imageService.AddImage(new ImageServiceModel()
                {
                    ImageTitle = image.FileName,
                    ImageData = memoryStream.ToArray(),
                    UploaderId = userId,
                    IsAvatar = false
                });
            }

            return RedirectToAction(
                "Index", 
                new { userId = userId });
        }

        //[HttpGet]
        //public IActionResult DeleteImage(int imageId) 
        //{

        //}
    }
}