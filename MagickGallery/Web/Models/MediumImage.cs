namespace Web.Models
{

    public class MediumImage
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public int Width => 800;
        public int Height => 450;
    }
}
