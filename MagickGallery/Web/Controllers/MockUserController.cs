namespace Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Web.Data;

    public class MockUserController : Controller
    {
        private readonly ApplicationDbContext _data;

        public MockUserController(ApplicationDbContext data) => this._data = data;

        public IActionResult Index()
        {
            var users = this._data.MockUsers
                .ToList();

            return View(users);
        }

        public IActionResult FindUser()
        {
            return View();
        }

        [HttpGet]
        public async Task<string> GetMostRecentUsers()
        {
            var users = await this._data.MockUsers
                .Take(5)
                .ToArrayAsync();
            var result = JsonSerializer.Serialize(users);

            return result;
        }
    }
}