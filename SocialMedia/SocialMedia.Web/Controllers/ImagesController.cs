namespace SocialMedia.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Mvc;

    public class ImagesController : Controller
    {
        private readonly SocialMediaDbContext db;

        public ImagesController(SocialMediaDbContext db)
            => this.db = db;

        public IActionResult Index()
        {
            var ids = new[] { 1 };

            return this.View(ids);
        }

        public IActionResult Details(int id)
            => this.GetImage(id, i => i.MediumImageDate);

        public IActionResult Thumbnail(int id)
            => this.GetImage(id, i => i.ThumbnailImageData);

        private IActionResult GetImage(int id, Expression<Func<Image, byte[]>> selector)
        {
            this.Response.Headers.Add("Expires", DateTime.UtcNow.AddYears(1).ToString("R"));
            this.Response.Headers.Add("Cache-Control", "public");
            this.Response.Headers.Add("Last-Modified", DateTime.UtcNow.AddMinutes(-1).ToString("R"));

            var imageData = this.db
                .Images
                .Where(i => i.Id == id)
                .Select(selector)
                .FirstOrDefault();

            if (imageData == null)
            {
                return this.BadRequest();
            }

            return File(imageData, "image/png");
        }
    }
}
