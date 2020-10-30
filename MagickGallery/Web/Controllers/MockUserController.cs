namespace Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.WebSockets;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Web.Data;
    using Web.Models;

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

        [HttpGet]
        public IActionResult AddUser() 
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AddUser(MockUser newUser)
        {
            this._data.Add(newUser);
            this._data.SaveChanges();
            return RedirectToAction("Index");
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

        [HttpGet]
        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(MockUser newUser)
        {
            var isEmailExist = this._data.MockUsers
                .Any(e =>e.Email == newUser.Email);

            if (!isEmailExist)
            {
                this._data.MockUsers.Add(newUser);
                this._data.SaveChanges();
            }
            else 
            {
                TempData["email"] = "Email already exists";
            }

            return RedirectToAction("Index");
        }
    }
}