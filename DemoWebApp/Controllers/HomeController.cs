using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DemoWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Data;

namespace DemoWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppContext appContext;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            appContext = new AppContext(configuration["ConnectionString"]);
        }

        public async Task<IActionResult> Index()
        {
            var todos = await appContext.Todos.ToListAsync();

            var isAuthorized = false;

            if (Request.Cookies.TryGetValue("Authorization", out var authDate))
            {
                isAuthorized = System.DateTime.Parse(authDate) > System.DateTime.Now.AddSeconds(-5);
            }

            ViewBag.IsAuthorized = isAuthorized;

            return View(todos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login([FromForm] LoginModel formData)
        {
            if (formData.Login == "johndoe" && formData.Password == "123") {
                Response.Cookies.Append("Authorization", System.DateTime.Now.ToString());

                return RedirectToAction("Index");
            }

            return new NotFoundResult();
        }

        public async Task<IActionResult> ToggleTodo([FromQuery] int id)
        {
            var todo = await appContext.Todos.FindAsync(id);

            if (todo == null) return new NotFoundResult();

            todo.Done = !todo.Done;

            await appContext.SaveChangesAsync();

            return new OkResult();
        }
    }
}
