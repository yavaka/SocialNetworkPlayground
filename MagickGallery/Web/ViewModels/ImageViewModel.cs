namespace Web.ViewModels
{
    public class ImageViewModel
    {
        public int Id { get; set; }
        private string _base64Image;
        public string Base64Image
        {
            get => string.Format($"data:image/jpg;base64,{this._base64Image}"); 
            set => this._base64Image = value; 
        }
    }
}