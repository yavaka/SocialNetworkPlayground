namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Models;
    using SocialMedia.Services.Post;

    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;

        public ProfileController(
            UserManager<User> userManager,
            IPostService postService)
        {
            this._userManager = userManager;
            this._postService = postService;
        }

        private ProfileServiceModel Model { get; set; } = new ProfileServiceModel();

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            this.Model.CurrentUser = new UserServiceModel(
                await this._userManager.GetUserAsync(User));
            
            this.Model.Posts = await this._postService.GetPostsByUserId(Model.CurrentUser.Id);

            return View(Model);
        }
    }
}