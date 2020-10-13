namespace SocialMedia.Data.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ThumbnailImageData { get; set; }
        public byte[] OriginalImageData { get; set; }
        public byte[] MediumImageDate { get; set; }
        public string UploaderId { get; set; }
        public User Uploader{ get; set; }
    }
}
