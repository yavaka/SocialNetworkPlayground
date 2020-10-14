namespace SocialMedia.Services.Image
{
    public class ImageServiceModel
    {
        public int ImageId { get; set; }
        public string ImageTitle { get; set; }
        public byte[] OriginalImageData { get; set; }
        public byte[] ThumbnailImageData { get; set; }
        public byte[] MediumImageData { get; set; }
        public string UploaderId { get; set; }
    }
}
