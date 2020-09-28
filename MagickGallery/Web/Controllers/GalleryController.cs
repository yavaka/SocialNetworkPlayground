namespace Web.Controllers
{
    using ImageMagick;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Web.Data;
    using Web.Models;
    using Web.ViewModels;

    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _data;

        public GalleryController(ApplicationDbContext data) => this._data = data;

        public IActionResult Index()
        {
            return View(this.GetThumbnailImages());
        }

        [HttpGet]
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddImage(IFormFileCollection files)
        {
            foreach (var file in files)
            {
                // 10 * 1024 * 1024 = 10MB
                if (file.Length > 1024 * 1024 * 10)
                {
                    return View("Image size cannot be more than 10MB!");
                }

                using var image = new MagickImage(file.OpenReadStream());

                ResizeAndSaveToAllResolutions(image);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetImage([FromQuery]int id)
        {
            var result = this._data.MediumImages
                .FirstOrDefault(i => i.Id == id);

            var magickImage = new MagickImage();
            magickImage.Read(result.Data);

            var base64Image = string
                .Format($"data:image/jpg;base64,{magickImage.ToBase64()}");

            return Json(new { base64Image });
        }

        private void ResizeAndSaveToAllResolutions(MagickImage image)
        {
            ResizeAndSaveToThumbnailResolution(image);
            ResizeAndSaveToMediumResolution(image);

            //Original image
            this._data.OriginalImages.Add(new OriginalImage
            {
                Data = image.ToByteArray(),
                Width = image.Width,
                Heigth = image.Height
            });

            this._data.SaveChanges();
        }

        private void ResizeAndSaveToThumbnailResolution(MagickImage image)
        {
            //New medium resolution image
            var thumbnailImage = new ThumbnailImage();

            var ms = new MemoryStream();

            //Custom size of the image
            var size = new MagickGeometry(thumbnailImage.Width, thumbnailImage.Height);
            size.IgnoreAspectRatio = true;

            //Resize the image with the defined size
            image.Resize(size);

            //Save changes from magick image
            image.Write(ms);

            thumbnailImage.Data = ms.ToArray();

            //Add to db
            this._data.ThumbnailImages.Add(thumbnailImage);
        }

        private void ResizeAndSaveToMediumResolution(MagickImage image)
        {
            //New medium resolution image
            var mediumImage = new MediumImage();

            var ms = new MemoryStream();

            //Custom size of the image
            var size = new MagickGeometry(mediumImage.Width,mediumImage.Height);
            size.IgnoreAspectRatio = true;

            //Resize the image
            image.Resize(size);
           
            //Save changes from magick image
            image.Write(ms);

            mediumImage.Data = ms.ToArray();

            //Add to db
            this._data.MediumImages.Add(mediumImage);
        }

        #region ImageService

        private IEnumerable<ImageViewModel> GetThumbnailImages()
        {
            var images = this._data.ThumbnailImages
                .ToList();

            var result = new List<ImageViewModel>();
            foreach (var image in images)
            {
                var magickImage = new MagickImage();
                magickImage.Read(image.Data);

                result.Add(new ImageViewModel
                {
                    Id = image.Id,
                    Base64Image = magickImage.ToBase64()
                });
            }

            return result;
        }

        #endregion

    }
}