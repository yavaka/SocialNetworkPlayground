namespace Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ThumbnailImage
    {
        [Key]
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public int Width => 308;
        public int Height => 225;
    }
}
