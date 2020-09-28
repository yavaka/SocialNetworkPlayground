namespace Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OriginalImage
    {
        [Key]
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public int Width { get; set; }
        public int Heigth { get; set; }
    }
}
